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

// Catálogos administrados con la misma pantalla. Los nombres de tabla son fijos
// aquí; del usuario solo llegan valores como parámetros.
const CATALOGOS = {
  categorias: {
    listar: `SELECT c.id_categoria AS id, c.nombre, c.descripcion AS campo2,
               (SELECT COUNT(*) FROM Producto p WHERE p.id_categoria = c.id_categoria) AS en_uso
             FROM Categoria c WHERE @t IS NULL OR c.nombre LIKE '%'+@t+'%' ORDER BY c.nombre`,
    insertar: 'INSERT INTO Categoria (nombre, descripcion) VALUES (@nombre, @c2)',
    editar: 'UPDATE Categoria SET nombre = @nombre, descripcion = @c2 WHERE id_categoria = @id',
    eliminar: 'DELETE FROM Categoria WHERE id_categoria = @id',
    c2: () => sql.VarChar(200),
  },
  marcas: {
    listar: `SELECT m.id_marca AS id, m.nombre, m.pais_origen AS campo2,
               (SELECT COUNT(*) FROM Producto p WHERE p.id_marca = m.id_marca) AS en_uso
             FROM Marca m WHERE @t IS NULL OR m.nombre LIKE '%'+@t+'%' ORDER BY m.nombre`,
    insertar: 'INSERT INTO Marca (nombre, pais_origen) VALUES (@nombre, @c2)',
    editar: 'UPDATE Marca SET nombre = @nombre, pais_origen = @c2 WHERE id_marca = @id',
    eliminar: 'DELETE FROM Marca WHERE id_marca = @id',
    c2: () => sql.VarChar(60),
  },
  tipos: {
    listar: `SELECT ts.id_tipo_servicio AS id, ts.nombre, ts.precio_base AS campo2,
               (SELECT COUNT(*) FROM DetalleOrdenServicio d WHERE d.id_tipo_servicio = ts.id_tipo_servicio) AS en_uso,
               (SELECT COUNT(*) FROM EmpleadoTipoServicio e WHERE e.id_tipo_servicio = ts.id_tipo_servicio) AS tecnicos
             FROM TipoServicio ts WHERE @t IS NULL OR ts.nombre LIKE '%'+@t+'%' ORDER BY ts.nombre`,
    insertar: 'INSERT INTO TipoServicio (nombre, precio_base) VALUES (@nombre, @c2)',
    editar: 'UPDATE TipoServicio SET nombre = @nombre, precio_base = @c2 WHERE id_tipo_servicio = @id',
    eliminar: 'DELETE FROM TipoServicio WHERE id_tipo_servicio = @id',
    c2: () => sql.Decimal(10, 2),
  },
};
// Igual que FrmPrincipal en C#: cada módulo depende de una capacidad que concede SQL Server
const MODULO_REQUIERE = {
  dashboard: ['ver_reportes'], venta: ['registrar_venta'], ventas: [], clientes: [], ordenes: [], productos: [],
  compras: ['registrar_compra'], proveedores: [], ajustes: ['ajustar_stock', 'ajustar_precio'], categorias: [], marcas: [],
  tipos: [], reportes: ['ver_reportes'], usuarios: ['admin_usuarios'], auditoria: ['ver_auditoria'],
};
const modulosDe = (caps) => Object.keys(MODULO_REQUIERE).filter((m) => !MODULO_REQUIERE[m].length || MODULO_REQUIERE[m].some((c) => caps.includes(c)));
const ES_COSTO = /costo|margen|utilidad|valor_inventario/i;
const sinCostos = (v) => Array.isArray(v) ? v.map(sinCostos)
  : v && typeof v === 'object' && !(v instanceof Date)
    ? Object.fromEntries(Object.entries(v).filter(([k]) => !ES_COSTO.test(k)).map(([k, x]) => [k, sinCostos(x)]))
    : v;
// La operación se valida con la misma capacidad que oculta el botón (y la base la vuelve a validar)
const exige = (user, cap, msg) => { if (!user.caps.includes(cap)) throw new Error(msg || 'Tu perfil no tiene permiso para esta operación.'); };

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
    // Mismo procedimiento que la app: valida el hash y bloquea tras 5 intentos
    const { rows } = await sp('usp_Login', {
      nombre_usuario: [sql.VarChar(40), body.usuario || ''], contrasena: [sql.VarChar(100), body.contrasena || ''],
    });
    if (!rows.length) throw new Error('Usuario o contraseña incorrectos.');
    const token = crypto.randomUUID();
    // Permisos reales de su rol en SQL Server (usp_PermisosApp: EXECUTE AS USER + HAS_PERMS_BY_NAME)
    const permisos = (await sp('usp_PermisosApp', { id_usuario: [sql.Int, rows[0].id_usuario] })).rows;
    const caps = permisos.filter((x) => x.permitido).map((x) => x.capacidad);
    const user = { ...rows[0], caps, modulos: modulosDe(caps) };
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

  // Mismas consultas que Data/DashboardRepository.cs de la app en C#
  'GET /api/dashboard': async ({ user }) => {
    exige(user, 'ver_reportes', 'Tu perfil no tiene Dashboard.');
    // Una sola pantalla: 4 indicadores, 2 gráficas de barras y 2 listas cortas de "requiere atención"
    const [kpi, meses, categorias, alertas, taller] = await Promise.all([
      q(`-- Periodo: últimos 90 días; se compara con los 90 días anteriores
                DECLARE @ini DATE = DATEADD(DAY, -90, CAST(GETDATE() AS DATE));
                DECLARE @ant DATE = DATEADD(DAY, -90, @ini);
                SELECT
                  (SELECT COUNT(*) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ventas_mes,
                  (SELECT COUNT(*) FROM Venta WHERE estado = 'Completada' AND fecha >= @ant AND fecha < @ini) AS ventas_ant,
                  (SELECT ISNULL(SUM(total), 0) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ingresos_mes,
                  (SELECT ISNULL(SUM(total), 0) FROM Venta WHERE estado = 'Completada' AND fecha >= @ant AND fecha < @ini) AS ingresos_ant,
                  (SELECT ISNULL(SUM(dv.cantidad), 0) FROM DetalleVenta dv JOIN Venta v ON v.id_venta = dv.id_venta
                    WHERE v.estado = 'Completada' AND v.fecha >= @ini) AS unidades_mes,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE es_final = 0) AS ordenes_abiertas,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE estado = 'Lista') AS ordenes_listas,
                  (SELECT COUNT(*) FROM vw_StockBajo) AS alertas_stock,
                  (SELECT COUNT(*) FROM vw_StockBajo WHERE stock_actual = 0) AS agotados,
                  (SELECT COUNT(*) FROM Cliente WHERE activo = 1) AS clientes_activos,
                  (SELECT COUNT(*) FROM Cliente WHERE activo = 1 AND fecha_registro >= @ini) AS clientes_nuevos,
                  (SELECT COUNT(*) FROM Garantia WHERE fecha_fin >= CAST(GETDATE() AS DATE)) AS garantias_vigentes,
                  (SELECT ISNULL(SUM(stock_actual * costo), 0) FROM Producto WHERE activo = 1) AS valor_inventario`),
      q(`SELECT mes, ventas, ingresos FROM vw_ResumenMensual
                WHERE mes >= FORMAT(DATEADD(MONTH, -5, GETDATE()), 'yyyy-MM') ORDER BY mes`),
      q(`SELECT categoria, ingresos FROM dbo.fn_VentasPorCategoria(DATEADD(DAY, -90, GETDATE()), GETDATE())
                                 ORDER BY ingresos DESC`),
      q(`SELECT TOP 5 nombre AS producto, stock_actual AS stock, stock_minimo AS minimo
                                 FROM vw_StockBajo ORDER BY faltante DESC`),
      q(`SELECT TOP 5 folio, equipo_descripcion AS equipo, estado, dias_en_taller AS dias
                                 FROM vw_OrdenServicioCompleta WHERE es_final = 0 ORDER BY dias_en_taller DESC`)
    ]);
    return { kpi: kpi[0], meses, categorias: categorias.slice(0, 6), alertas, taller };
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
    exige(user, 'registrar_venta');
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
    exige(user, 'cancelar_venta');
    await q(`EXEC sp_set_session_context N'id_usuario', @u; EXEC usp_CancelarVenta @id, @m;`,
      { u: [sql.Int, user.id_usuario], id: [sql.Int, +body.id_venta], m: [sql.VarChar(200), body.motivo || 'Cancelada desde la vista previa'] });
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
    exige(user, 'gestionar_ordenes');
    // SESSION_CONTEXT le dice al trigger qué usuario de la app hizo el cambio
    await q(`EXEC sp_set_session_context N'id_usuario', @u; EXEC usp_CambiarEstadoOrden @id, @e;`,
      { u: [sql.Int, user.id_usuario], e: [sql.Int, +body.id_estado], id: [sql.Int, +body.id_orden] });
    return { ok: true };
  },

  'GET /api/reportes': async ({ query, user }) => {
    exige(user, 'ver_reportes');
    const desde = fecha(vacio(query.desde)), hasta = fecha(vacio(query.hasta));
    return {
      resumen: (await sp('usp_ResumenPeriodo', { desde: [sql.Date, desde], hasta: [sql.Date, hasta] })).rows,
      top: await q(`SELECT TOP 10 sku, nombre, categoria, unidades_vendidas, ingresos, utilidad_bruta FROM vw_TopProductos ORDER BY ingresos DESC`),
      categorias: await q(`SELECT * FROM dbo.fn_VentasPorCategoria(@d, @h) ORDER BY ingresos DESC`,
        { d: [sql.Date, desde || new Date('2026-01-01')], h: [sql.Date, hasta || new Date()] }),
      empleados: !user.caps.includes('ver_costos') ? null : await q(`
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

  'GET /api/catalogo': async ({ query }) => {
    const c = CATALOGOS[query.tipo];
    if (!c) throw new Error('Catálogo desconocido.');
    return q(c.listar, { t: [sql.VarChar(60), vacio(query.texto)] });
  },

  'POST /api/catalogo': async ({ body, user }) => {
    exige(user, 'editar_catalogos');
    const c = CATALOGOS[body.tipo];
    if (!c) throw new Error('Catálogo desconocido.');
    const nombre = String(body.nombre || '').trim();
    if (!nombre) throw new Error('El nombre es obligatorio.');
    const c2 = body.tipo === 'tipos' ? +body.campo2 || 0 : vacio(String(body.campo2 || '').trim());
    try {
      const r = pool.request();
      r.input('id', sql.Int, body.id ? +body.id : null);
      r.input('nombre', sql.VarChar(80), nombre);
      r.input('c2', c.c2(), c2);
      const res = await r.query(body.id ? c.editar : c.insertar);
      if (res.rowsAffected[0] === 0) throw new Error('El registro ya no existe; recarga la lista.');
    } catch (e) {
      if ([2627, 2601].includes(e.number)) throw new Error(`Ya existe un registro llamado "${nombre}".`);
      throw e;
    }
    return { ok: true };
  },

  'POST /api/catalogo/eliminar': async ({ body, user }) => {
    exige(user, 'editar_catalogos');
    const c = CATALOGOS[body.tipo];
    if (!c) throw new Error('Catálogo desconocido.');
    try {
      const res = await pool.request().input('id', sql.Int, +body.id).query(c.eliminar);
      if (res.rowsAffected[0] === 0) throw new Error('El registro ya no existe.');
    } catch (e) {
      // La FK protege la integridad: no se borra lo que está en uso
      if (e.number === 547) throw new Error('No se puede eliminar: hay productos, órdenes o técnicos que lo usan.');
      throw e;
    }
    return { ok: true };
  },

  'GET /api/proveedores': async ({ query }) => q(`
    SELECT pv.id_proveedor, pv.razon_social, pv.rfc, pv.correo, pv.telefono, pv.activo,
           (SELECT COUNT(*) FROM ProductoProveedor pp WHERE pp.id_proveedor = pv.id_proveedor) AS productos,
           (SELECT COUNT(*) FROM Compra c WHERE c.id_proveedor = pv.id_proveedor) AS compras,
           (SELECT ISNULL(SUM(total),0) FROM Compra c WHERE c.id_proveedor = pv.id_proveedor AND c.estado = 'Recibida') AS total_comprado
    FROM Proveedor pv
    WHERE @t IS NULL OR pv.razon_social LIKE '%'+@t+'%' OR pv.rfc LIKE '%'+@t+'%'
    ORDER BY pv.razon_social`, { t: [sql.VarChar(60), vacio(query.texto)] }),

  'GET /api/proveedores/productos': async ({ query }) => q(`
    SELECT p.sku, p.nombre, pp.costo_proveedor, p.costo AS costo_actual, p.stock_actual
    FROM ProductoProveedor pp JOIN Producto p ON p.id_producto = pp.id_producto
    WHERE pp.id_proveedor = @id ORDER BY p.nombre`, { id: [sql.Int, +query.id] }),

  'POST /api/proveedores': async ({ body, user }) => {
    exige(user, 'guardar_proveedor');
    await sp('usp_ProveedorGuardar', {
      id_proveedor: [sql.Int, body.id ? +body.id : null],
      razon_social: [sql.VarChar(120), String(body.razon_social || '').trim()],
      rfc: [sql.VarChar(13), body.rfc || ''],
      correo: [sql.VarChar(100), vacio(body.correo)],
      telefono: [sql.VarChar(20), vacio(body.telefono)],
      activo: [sql.Bit, !!body.activo],
    });
    return { ok: true };
  },

  'GET /api/compras': async () => ({
    recientes: await q(`
      SELECT TOP 50 c.folio, c.fecha, pv.razon_social AS proveedor, CONCAT(e.nombre,' ',e.apellidos) AS recibio,
             (SELECT COUNT(*) FROM DetalleCompra d WHERE d.id_compra = c.id_compra) AS lineas, c.total, c.estado
      FROM Compra c JOIN Proveedor pv ON pv.id_proveedor = c.id_proveedor JOIN Empleado e ON e.id_empleado = c.id_empleado
      ORDER BY c.fecha DESC`),
    proveedores: await q(`SELECT id_proveedor AS id, razon_social AS nombre FROM Proveedor WHERE activo = 1 ORDER BY razon_social`),
    productos: await q(`SELECT id_producto AS id, sku, nombre, costo, requiere_serie FROM Producto WHERE activo = 1 ORDER BY nombre`),
  }),

  'POST /api/compras': async ({ body, user }) => {
    exige(user, 'registrar_compra');
    const r = pool.request();
    r.input('u', sql.Int, user.id_usuario);
    r.input('prov', sql.Int, +body.id_proveedor);
    r.input('emp', sql.Int, user.id_empleado);
    r.input('lineas', sql.NVarChar(sql.MAX), JSON.stringify(body.lineas || []));
    const res = await r.query(`EXEC sp_set_session_context N'id_usuario', @u;
      DECLARE @id INT, @folio VARCHAR(20);
      EXEC usp_RegistrarCompra @prov, @emp, @lineas, @id OUTPUT, @folio OUTPUT;
      SELECT @folio AS folio;`);
    return res.recordset[0];
  },

  'GET /api/usuarios': async ({ user }) => {
    exige(user, 'admin_usuarios');
    return {
      usuarios: await q(`
        SELECT u.id_usuario, u.id_empleado, ur.id_rol, u.nombre_usuario AS usuario,
               CONCAT(e.nombre,' ',e.apellidos) AS empleado, e.puesto, r.nombre AS rol,
               u.activo, u.ultimo_acceso, u.intentos_fallidos
        FROM Usuario u JOIN Empleado e ON e.id_empleado = u.id_empleado
        LEFT JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario LEFT JOIN Rol r ON r.id_rol = ur.id_rol
        ORDER BY u.nombre_usuario`),
      empleados: await q(`
        SELECT e.id_empleado AS id, CONCAT(e.nombre,' ',e.apellidos,' — ',e.puesto,
               CASE WHEN u.id_usuario IS NULL THEN '' ELSE ' (ya tiene usuario)' END) AS nombre
        FROM Empleado e LEFT JOIN Usuario u ON u.id_empleado = e.id_empleado
        WHERE e.activo = 1 ORDER BY e.nombre`),
      roles: await q(`SELECT id_rol AS id, nombre FROM Rol ORDER BY id_rol`),
    };
  },

  'POST /api/usuarios': async ({ body, user }) => {
    exige(user, 'admin_usuarios');
    await sp('usp_UsuarioGuardar', {
      id_usuario: [sql.Int, body.id ? +body.id : null],
      id_empleado: [sql.Int, +body.id_empleado],
      nombre_usuario: [sql.VarChar(40), body.usuario || ''],
      id_rol: [sql.Int, +body.id_rol],
      contrasena: [sql.VarChar(100), vacio(body.contrasena)],
      activo: [sql.Bit, !!body.activo],
      id_admin: [sql.Int, user.id_usuario],
    });
    return { ok: true };
  },

  'POST /api/ajustes/stock': async ({ body, user }) => {
    exige(user, 'ajustar_stock');
    const r = pool.request();
    r.input('u', sql.Int, user.id_usuario);
    r.input('p', sql.Int, +body.id_producto);
    r.input('c', sql.Int, +body.cantidad);
    r.input('m', sql.VarChar(200), body.motivo || '');
    await r.query(`EXEC sp_set_session_context N'id_usuario', @u; EXEC usp_AjustarStock @p, @c, @m, @u;`);
    return { ok: true };
  },

  'POST /api/ajustes/precio': async ({ body, user }) => {
    exige(user, 'ajustar_precio');
    const r = pool.request();
    r.input('u', sql.Int, user.id_usuario);
    r.input('pct', sql.Decimal(5, 2), +body.porcentaje);
    r.input('cat', sql.Int, +body.id_categoria);
    await r.query(`EXEC sp_set_session_context N'id_usuario', @u; EXEC usp_AjustarPrecio @pct, @u, NULL, @cat;`);
    return { ok: true };
  },

  'GET /api/ajustes': async () => ({
    productos: await q(`SELECT id_producto AS id, CONCAT(sku,' — ',nombre,' (stock ',stock_actual,')') AS nombre
                        FROM Producto WHERE requiere_serie = 0 AND activo = 1 ORDER BY nombre`),
    categorias: await q(`SELECT id_categoria AS id, nombre FROM Categoria ORDER BY nombre`),
  }),

  'GET /api/auditoria': async ({ user }) => {
    exige(user, 'ver_auditoria');
    return q(`SELECT TOP 300 fecha, usuario_app, usuario, tabla, operacion, id_registro, valor_anterior, valor_nuevo, motivo FROM Auditoria ORDER BY id_auditoria DESC`);
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
    const datos = await handler({ body: body ? JSON.parse(body) : {}, query: Object.fromEntries(url.searchParams), user });
    // Sin ver_costos (como tecno_operador en SQL Server) los costos y márgenes no salen del servidor
    send(200, user && !user.caps.includes('ver_costos') ? sinCostos(datos) : datos);
  } catch (e) {
    // Los THROW de SQL Server llegan aquí con su mensaje (stock insuficiente, permisos, etc.)
    send(400, { error: e.originalError?.info?.message || e.message });
  }
}).listen(PORT, async () => {
  pool = await sql.connect(config);
  console.log(`TecnoStore vista previa en http://localhost:${PORT}`);
});
