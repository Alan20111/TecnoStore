// Vista previa de TecnoStore para Mac.
// Simula la app de Windows Forms en el navegador, pero todo lo que muestra
// sale de la base real en SQL Server (Docker): mismas vistas, SP, funciones
// y triggers que consumirá la app en C#. Solo parámetros, nunca SQL concatenado.
const http = require('http');
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const sql = require('mssql');

const PORT = 5173;
// En Windows cambien estos valores con variables de entorno (ver README.md)
const config = {
  server: process.env.DB_SERVER || 'localhost',
  port: +(process.env.DB_PORT || 1433),
  database: 'TecnoStore',
  user: process.env.DB_USER || 'sa',
  password: process.env.DB_PASSWORD || 'TecnoStore#2026',
  options: { encrypt: false, trustServerCertificate: true },
};

// Permisos por rol (matriz del documento: Administrador, Supervisor, Operador, Consulta)
const MODULOS = {
  Administrador: ['inicio', 'productos', 'clientes', 'venta', 'ventas', 'ordenes', 'reportes', 'auditoria'],
  Supervisor:    ['inicio', 'productos', 'clientes', 'venta', 'ventas', 'ordenes', 'reportes', 'auditoria'],
  Operador:      ['inicio', 'productos', 'clientes', 'venta', 'ventas', 'ordenes'],
  Consulta:      ['inicio', 'productos', 'clientes', 'ventas', 'reportes'],
};
const PUEDE = {
  cancelar: ['Administrador', 'Supervisor'],
  vender: ['Administrador', 'Supervisor', 'Operador'],
  avanzarOrden: ['Administrador', 'Supervisor', 'Operador'],
};

const sesiones = new Map();
let pool;

async function q(text, params = {}) {
  const r = pool.request();
  for (const [k, [type, v]] of Object.entries(params)) r.input(k, type, v);
  return (await r.query(text)).recordset;
}
async function sp(name, params = {}, outputs = {}) {
  const r = pool.request();
  for (const [k, [type, v]] of Object.entries(params)) r.input(k, type, v);
  for (const [k, type] of Object.entries(outputs)) r.output(k, type);
  const res = await r.execute(name);
  return { rows: res.recordset || [], out: res.output };
}

const fecha = (v) => (v ? new Date(v) : null);
const vacio = (v) => (v === undefined || v === null || v === '' ? null : v);

const rutas = {
  'POST /api/login': async ({ body }) => {
    const rows = await q(`
      SELECT u.id_usuario, u.id_empleado, u.nombre_usuario, CONCAT(e.nombre,' ',e.apellidos) AS nombre,
             e.puesto, s.nombre AS sucursal, r.nombre AS rol
      FROM Usuario u
      JOIN Empleado e   ON e.id_empleado = u.id_empleado
      JOIN Sucursal s   ON s.id_sucursal = e.id_sucursal
      JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario
      JOIN Rol r        ON r.id_rol = ur.id_rol
      WHERE u.nombre_usuario = @u AND u.hash_contrasena = HASHBYTES('SHA2_256', @p)
        AND u.activo = 1 AND e.activo = 1`,
      { u: [sql.VarChar(40), body.usuario || ''], p: [sql.VarChar(100), body.contrasena || ''] });
    if (!rows.length) throw new Error('Usuario o contraseña incorrectos.');
    const token = crypto.randomUUID();
    const user = { ...rows[0], modulos: MODULOS[rows[0].rol] };
    sesiones.set(token, user);
    return { token, user };
  },

  'GET /api/inicio': async () => {
    const [kpi] = await q(`
      SELECT (SELECT COUNT(*) FROM Venta WHERE estado='Completada' AND fecha >= DATEADD(DAY,-30,SYSDATETIME())) AS ventas30,
             (SELECT ISNULL(SUM(total),0) FROM Venta WHERE estado='Completada' AND fecha >= DATEADD(DAY,-30,SYSDATETIME())) AS ingresos30,
             (SELECT COUNT(*) FROM vw_StockBajo) AS alertas,
             (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE es_final = 0) AS ordenes_abiertas`);
    const stock = await q(`SELECT TOP 6 sku, nombre, stock_actual, stock_minimo FROM vw_StockBajo ORDER BY faltante DESC`);
    const ordenes = await q(`SELECT TOP 6 folio, cliente, estado, dias_en_taller FROM vw_OrdenServicioCompleta WHERE es_final = 0 ORDER BY fecha_recepcion`);
    return { kpi, stock, ordenes };
  },

  'GET /api/catalogos': async () => ({
    categorias: await q(`SELECT id_categoria AS id, nombre FROM Categoria ORDER BY nombre`),
    metodos: await q(`SELECT id_metodo_pago AS id, nombre FROM MetodoPago ORDER BY id_metodo_pago`),
    clientes: await q(`SELECT id_cliente AS id, CONCAT(nombre,' ',apellidos) AS nombre FROM Cliente WHERE activo = 1 ORDER BY nombre`),
    productos: await q(`SELECT id_producto AS id, sku, nombre, precio_venta, stock_actual, requiere_serie FROM vw_ProductosActivos WHERE stock_actual > 0 ORDER BY nombre`),
    estados: await q(`SELECT id_estado_orden AS id, nombre, es_final FROM EstadoOrden ORDER BY id_estado_orden`),
    descuentoMax: (await q(`SELECT valor FROM Configuracion WHERE clave='DESCUENTO_MAX_OPERADOR'`))[0].valor,
  }),

  'GET /api/productos': async ({ query }) => (await sp('usp_ProductoBuscar', {
    texto: [sql.VarChar(60), vacio(query.texto)],
    id_categoria: [sql.Int, vacio(query.categoria)],
    solo_alertas: [sql.Bit, query.alertas === '1'],
  })).rows,

  'GET /api/clientes': async ({ query }) => q(`
    SELECT c.id_cliente, CONCAT(c.nombre,' ',c.apellidos) AS cliente, c.correo, c.telefono,
           CAST(c.fecha_registro AS DATE) AS registro, c.activo,
           (SELECT COUNT(*) FROM Venta v WHERE v.id_cliente = c.id_cliente AND v.estado='Completada') AS compras,
           dbo.fn_TotalCompradoCliente(c.id_cliente) AS total_comprado,
           dbo.fn_ClasificacionCliente(c.id_cliente) AS nivel
    FROM Cliente c
    WHERE @t IS NULL OR c.nombre LIKE '%'+@t+'%' OR c.apellidos LIKE '%'+@t+'%' OR c.correo LIKE '%'+@t+'%'
    ORDER BY total_comprado DESC`, { t: [sql.VarChar(60), vacio(query.texto)] }),

  'GET /api/ventas': async ({ query }) => q(`
    SELECT TOP 200 id_venta, folio, fecha, cliente, vendedor, sucursal, metodo_pago, lineas, subtotal, descuento, total, estado
    FROM vw_VentaCompleta
    WHERE (@desde IS NULL OR fecha >= @desde) AND (@hasta IS NULL OR fecha < DATEADD(DAY,1,@hasta))
      AND (@estado IS NULL OR estado = @estado)
      AND (@t IS NULL OR folio LIKE '%'+@t+'%' OR cliente LIKE '%'+@t+'%')
    ORDER BY fecha DESC`, {
    desde: [sql.Date, fecha(vacio(query.desde))], hasta: [sql.Date, fecha(vacio(query.hasta))],
    estado: [sql.VarChar(12), vacio(query.estado)], t: [sql.VarChar(60), vacio(query.texto)],
  }),

  'GET /api/ventas/detalle': async ({ query }) => q(`
    SELECT p.sku, p.nombre, us.numero_serie, dv.cantidad, dv.precio_unitario, dv.subtotal,
           CASE WHEN g.id_garantia IS NULL THEN '' ELSE CONCAT(g.meses,' meses (hasta ', FORMAT(g.fecha_fin,'dd/MM/yyyy'),')') END AS garantia
    FROM DetalleVenta dv
    JOIN Producto p ON p.id_producto = dv.id_producto
    LEFT JOIN UnidadSerie us ON us.id_unidad = dv.id_unidad
    LEFT JOIN Garantia g ON g.id_detalle_venta = dv.id_detalle_venta
    WHERE dv.id_venta = @id ORDER BY dv.id_detalle_venta`, { id: [sql.Int, +query.id] }),

  'POST /api/ventas': async ({ body, user }) => {
    if (!PUEDE.vender.includes(user.rol)) throw new Error('Tu rol no puede registrar ventas.');
    const autoriza = ['Administrador', 'Supervisor'].includes(user.rol) ? user.id_usuario : null;
    const { out } = await sp('usp_RegistrarVenta', {
      id_cliente: [sql.Int, +body.id_cliente],
      id_empleado: [sql.Int, user.id_empleado],
      id_metodo_pago: [sql.Int, +body.id_metodo_pago],
      lineas: [sql.NVarChar(sql.MAX), JSON.stringify(body.lineas || [])],
      descuento_pct: [sql.Decimal(5, 2), +body.descuento_pct || 0],
      id_usuario_autoriza: [sql.Int, autoriza],
    }, { id_venta: sql.Int, folio: sql.VarChar(20) });
    return out;
  },

  'POST /api/ventas/cancelar': async ({ body, user }) => {
    if (!PUEDE.cancelar.includes(user.rol)) throw new Error('Solo Supervisor o Administrador pueden cancelar ventas.');
    await sp('usp_CancelarVenta', { id_venta: [sql.Int, +body.id_venta] });
    return { ok: true };
  },

  'GET /api/ordenes': async ({ query }) => q(`
    SELECT id_orden, folio, fecha_recepcion, cliente, tecnico, equipo_descripcion, descripcion_falla, estado, es_final, total, dias_en_taller
    FROM vw_OrdenServicioCompleta
    WHERE (@abiertas = 0 OR es_final = 0)
    ORDER BY fecha_recepcion DESC`, { abiertas: [sql.Bit, query.abiertas === '1'] }),

  'GET /api/ordenes/historial': async ({ query }) => q(`
    SELECT h.fecha, ea.nombre AS de_estado, en.nombre AS a_estado, ISNULL(u.nombre_usuario,'—') AS usuario
    FROM HistorialEstadoOrden h
    LEFT JOIN EstadoOrden ea ON ea.id_estado_orden = h.id_estado_anterior
    JOIN EstadoOrden en ON en.id_estado_orden = h.id_estado_nuevo
    LEFT JOIN Usuario u ON u.id_usuario = h.id_usuario
    WHERE h.id_orden = @id ORDER BY h.fecha`, { id: [sql.Int, +query.id] }),

  'POST /api/ordenes/estado': async ({ body, user }) => {
    if (!PUEDE.avanzarOrden.includes(user.rol)) throw new Error('Tu rol no puede cambiar órdenes.');
    // SESSION_CONTEXT le dice al trigger qué usuario de la app hizo el cambio
    await q(`EXEC sp_set_session_context N'id_usuario', @u;
             UPDATE OrdenServicio SET id_estado_orden = @e,
                    fecha_entrega = CASE WHEN @e = 5 THEN SYSDATETIME() ELSE fecha_entrega END
             WHERE id_orden = @id;`,
      { u: [sql.Int, user.id_usuario], e: [sql.Int, +body.id_estado], id: [sql.Int, +body.id_orden] });
    return { ok: true };
  },

  'GET /api/reportes': async ({ query }) => {
    const desde = fecha(vacio(query.desde)), hasta = fecha(vacio(query.hasta));
    return {
      resumen: (await sp('usp_ResumenPeriodo', { desde: [sql.Date, desde], hasta: [sql.Date, hasta] })).rows,
      top: await q(`SELECT TOP 10 sku, nombre, categoria, unidades_vendidas, ingresos, utilidad_bruta FROM vw_TopProductos ORDER BY ingresos DESC`),
      categorias: await q(`SELECT * FROM dbo.fn_VentasPorCategoria(@d, @h) ORDER BY ingresos DESC`,
        { d: [sql.Date, desde || new Date('2026-01-01')], h: [sql.Date, hasta || new Date()] }),
      empleados: await q(`
        SELECT CONCAT(e.nombre,' ',e.apellidos) AS empleado, e.puesto, s.nombre AS sucursal,
               COUNT(DISTINCT v.id_venta) AS ventas, ISNULL(SUM(v.total),0) AS vendido,
               (SELECT COUNT(*) FROM OrdenServicio o WHERE o.id_empleado_tecnico = e.id_empleado) AS ordenes
        FROM Empleado e JOIN Sucursal s ON s.id_sucursal = e.id_sucursal
        LEFT JOIN Venta v ON v.id_empleado = e.id_empleado AND v.estado = 'Completada'
        GROUP BY e.id_empleado, e.nombre, e.apellidos, e.puesto, s.nombre
        ORDER BY vendido DESC`),
      stock: await q(`SELECT sku, nombre, categoria, stock_actual, stock_minimo, faltante, proveedor_sugerido FROM vw_StockBajo ORDER BY faltante DESC`),
    };
  },

  'GET /api/auditoria': async ({ user }) => {
    if (!['Administrador', 'Supervisor'].includes(user.rol)) throw new Error('Sin permiso para ver la auditoría.');
    return q(`SELECT TOP 300 fecha, usuario, tabla, operacion, id_registro, valor_anterior, valor_nuevo FROM Auditoria ORDER BY id_auditoria DESC`);
  },
};

const TIPOS = { '.html': 'text/html; charset=utf-8', '.js': 'text/javascript; charset=utf-8', '.css': 'text/css; charset=utf-8' };

http.createServer(async (req, res) => {
  const url = new URL(req.url, `http://localhost:${PORT}`);
  if (!url.pathname.startsWith('/api/')) {
    const file = path.join(__dirname, 'public', url.pathname === '/' ? 'index.html' : path.normalize(url.pathname));
    if (!file.startsWith(path.join(__dirname, 'public')) || !fs.existsSync(file)) { res.writeHead(404); return res.end(); }
    res.writeHead(200, { 'Content-Type': TIPOS[path.extname(file)] || 'application/octet-stream' });
    return fs.createReadStream(file).pipe(res);
  }
  const handler = rutas[`${req.method} ${url.pathname}`];
  let body = '';
  for await (const chunk of req) body += chunk;
  const send = (status, data) => { res.writeHead(status, { 'Content-Type': 'application/json' }); res.end(JSON.stringify(data)); };
  if (!handler) return send(404, { error: 'Ruta no encontrada' });
  const user = sesiones.get(req.headers['x-token']);
  if (url.pathname !== '/api/login' && !user) return send(401, { error: 'Inicia sesión.' });
  try {
    send(200, await handler({ body: body ? JSON.parse(body) : {}, query: Object.fromEntries(url.searchParams), user }));
  } catch (e) {
    // Los THROW de SQL Server llegan aquí con su mensaje (stock insuficiente, permisos, etc.)
    send(400, { error: e.originalError?.info?.message || e.message });
  }
}).listen(PORT, async () => {
  pool = await sql.connect(config);
  console.log(`TecnoStore vista previa en http://localhost:${PORT}`);
});
