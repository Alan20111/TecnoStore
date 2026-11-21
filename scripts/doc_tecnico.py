# -*- coding: utf-8 -*-
"""Contenido del Documento técnico y del Manual de usuario (sección 20.1).
Los datos (conteos, objetos, pruebas) llegan de generar_documentacion.py,
que los lee de SQL Server; aquí solo va la redacción."""
import os

DESCRIPCION_OBJETOS = {
    # Vistas
    'vw_ProductosActivos': ('Operativa', 'Productos activos con categoría, marca, condición y alerta de stock (OK / Stock bajo / Agotado).'),
    'vw_StockBajo': ('Operativa', 'Productos en o bajo el mínimo con el proveedor más barato (LEFT JOIN a ProductoProveedor).'),
    'vw_VentaCompleta': ('Relacional', 'Venta con cliente, vendedor, sucursal, método de pago y número de líneas.'),
    'vw_OrdenServicioCompleta': ('Relacional', 'Orden con cliente, técnico, estado y días en taller.'),
    'vw_ResumenMensual': ('Analítica', 'Ventas, ingresos, ticket promedio y descuentos por mes.'),
    'vw_TopProductos': ('Analítica', 'Unidades, ingresos y utilidad bruta por producto.'),
    # Procedimientos
    'usp_Login': ('Seguridad', 'Valida usuario y hash de contraseña, registra último acceso y bloquea tras 5 intentos.'),
    'usp_ProductoBuscar': ('Catálogo', 'Búsqueda con parámetros opcionales (texto, categoría, solo alertas).'),
    'usp_ProductoGuardar': ('Catálogo', 'Alta o edición de producto; valida SKU único y precio ≥ costo.'),
    'usp_ClienteGuardar': ('Catálogo', 'Alta o edición de cliente; correo único y con formato.'),
    'usp_ProveedorGuardar': ('Catálogo', 'Alta o edición de proveedor; RFC único de 12 o 13 caracteres.'),
    'usp_UsuarioGuardar': ('Seguridad · transacción', 'Alta o edición de usuario y su rol (Usuario + UsuarioRol); solo Administrador.'),
    'usp_RegistrarVenta': ('Proceso 1 · transacción', 'Venta completa: encabezado, detalle, stock, piezas con serie y garantías.'),
    'usp_RegistrarCompra': ('Proceso 2 · transacción', 'Compra completa: encabezado, detalle, stock, piezas con serie y ProductoProveedor.'),
    'usp_RegistrarOrdenServicio': ('Proceso · transacción', 'Recepción de equipo: orden, diagnóstico e historial.'),
    'usp_OrdenAgregarLinea': ('Proceso · transacción', 'Agrega mano de obra o refacción (descuenta stock) y recalcula el total.'),
    'usp_CambiarEstadoOrden': ('Proceso · transacción', 'Avanza un paso o cancela; al cancelar regresa refacciones al almacén.'),
    'usp_CancelarVenta': ('Proceso 3 · transacción', 'Cancela con motivo: reintegra stock, libera piezas y elimina garantías.'),
    'usp_AjustarStock': ('Proceso 4 · transacción', 'Ajuste de inventario ±20 piezas con motivo; solo Supervisor.'),
    'usp_AjustarPrecio': ('Administrativo · transacción', 'Cambio de precio ±30 % por producto o categoría; solo Supervisor.'),
    'usp_ResumenPeriodo': ('Reporte', 'Ventas e ingresos por mes en un rango de fechas opcional.'),
    'usp_ActividadEmpleados': ('Reporte', 'Ventas, importe y órdenes por empleado con filtros opcionales.'),
    # Funciones
    'fn_TotalCompradoCliente': ('Escalar', 'Total histórico comprado por un cliente.'),
    'fn_ClasificacionCliente': ('Escalar', 'Nivel del cliente según su gasto: Oro, Plata, Bronce, Sin compras.'),
    'fn_MargenProducto': ('Escalar', 'Margen porcentual de un producto.'),
    'fn_UsuarioTieneRol': ('Escalar', 'Si un usuario tiene un rol (Administrador cubre a Supervisor).'),
    'fn_TecnicoHabilitado': ('Escalar', 'Si un técnico activo puede hacer un tipo de servicio (N:M).'),
    'fn_TotalOrden': ('Escalar', 'Costo total de una orden de servicio.'),
    'fn_VentasPorCategoria': ('De tabla', 'Ventas, unidades e ingresos por categoría en un rango de fechas.'),
    # Triggers
    'trg_Producto_Auditoria': ('Auditoría', 'Registra cambios de precio y de stock con usuario de la app y motivo.'),
    'trg_Venta_Auditoria': ('Auditoría', 'Registra cancelaciones y cambios de total de ventas.'),
    'trg_OrdenServicio_Estado': ('Regla / automatización', 'Impide cambiar órdenes cerradas y llena HistorialEstadoOrden.'),
    'trg_Auditoria_SoloLectura': ('Libre', 'La auditoría no se puede modificar ni borrar.'),
    'trg_Producto_BajaLogica': ('Libre', 'Un DELETE de producto con historial se convierte en baja lógica.'),
}

POR_QUE_TRIGGER = {
    'trg_Producto_Auditoria': 'El precio y el stock cambian desde varios lugares (ventas, compras, órdenes, ajustes, la app o SSMS). En la base se registran todos sin depender de quién los cambió.',
    'trg_Venta_Auditoria': 'Una cancelación mueve dinero; queda constancia aunque alguien la haga fuera de la aplicación.',
    'trg_OrdenServicio_Estado': 'La regla "una orden cerrada no cambia" protege también las actualizaciones directas en tabla, y el historial se escribe solo.',
    'trg_Auditoria_SoloLectura': 'Una bitácora que se puede editar no sirve como evidencia; ni siquiera sa puede alterarla sin quitar el trigger.',
    'trg_Producto_BajaLogica': 'Borrar un producto con ventas rompería el historial; el trigger lo desactiva en su lugar.',
}

MODULOS = [
    (1, 'Acceso / sesión', 'FrmLogin', 'usp_Login'),
    (2, 'Menú principal por perfil', 'FrmPrincipal (MDI)', 'Matriz de permisos'),
    (3, 'Catálogo 1: Categorías', 'FrmCatalogo', 'INSERT/UPDATE/DELETE parametrizados'),
    (4, 'Catálogo 2: Marcas', 'FrmCatalogo', 'INSERT/UPDATE/DELETE parametrizados'),
    (5, 'Catálogo 3: Tipos de servicio', 'FrmCatalogo', 'INSERT/UPDATE/DELETE parametrizados'),
    (6, 'Clientes (CRUD + historial)', 'FrmClientes', 'usp_ClienteGuardar, fn_TotalCompradoCliente, fn_ClasificacionCliente'),
    (7, 'Proveedores (CRUD + relaciones)', 'FrmProveedores', 'usp_ProveedorGuardar, ProductoProveedor'),
    (8, 'Productos (CRUD + filtros + existencias)', 'FrmProductos', 'usp_ProductoBuscar, usp_ProductoGuardar'),
    (9, 'Proceso principal: venta', 'FrmVenta', 'usp_RegistrarVenta'),
    (10, 'Proceso secundario: órdenes de taller', 'FrmOrdenes', 'usp_RegistrarOrdenServicio, usp_OrdenAgregarLinea, usp_CambiarEstadoOrden'),
    ('10b', 'Tercer maestro-detalle: compras', 'FrmCompra', 'usp_RegistrarCompra'),
    (11, 'Cancelaciones y ajustes', 'FrmHistorialVentas, FrmAjusteStock', 'usp_CancelarVenta, usp_AjustarStock, usp_AjustarPrecio'),
    (12, 'Historial con filtros', 'FrmHistorialVentas', 'vw_VentaCompleta'),
    (13, 'Reportes (5)', 'FrmReportes', 'usp_ResumenPeriodo, vw_TopProductos, usp_ActividadEmpleados, vw_StockBajo, fn_VentasPorCategoria'),
    (14, 'Administración: usuarios y auditoría', 'FrmUsuarios, FrmAuditoria', 'usp_UsuarioGuardar, Auditoria'),
]


def documento(d):
    e, tabla, figura, C = d['e'], d['tabla'], d['figura'], d['CONTEO']
    obj = {o['name']: o['tipo'] for o in d['OBJETOS']}
    pasan = sum(p['resultado'] == 'PASA' for p in d['PRUEBAS'])
    der = os.path.join(d['DOC'], 'DER.png')

    def objetos(tipo_sql, cols):
        return tabla(cols, [[ '<code>%s</code>' % e(n), e(DESCRIPCION_OBJETOS.get(n, ('', ''))[0]), e(DESCRIPCION_OBJETOS.get(n, ('', ''))[1])]
                            for n, t in obj.items() if t in tipo_sql])

    h = """
<div class="portada">
  <h1>TecnoStore</h1>
  <div class="sub">Sistema Integral de Gestión para una tienda de tecnología con taller de reparación</div>
  <p style="margin-top:18pt">Proyecto Final Integrador · Taller de Bases de Datos<br>SQL Server + C# Windows Forms + Visual Studio</p>
  <table><tr><th>Equipo</th><td>(completar)</td></tr>
  <tr><th>Integrante 1</th><td>(completar)</td></tr><tr><th>Integrante 2</th><td>(completar)</td></tr><tr><th>Integrante 3</th><td>(completar)</td></tr>
  <tr><th>Profesor</th><td>(completar)</td></tr><tr><th>Fecha</th><td>%(hoy)s</td></tr></table>
</div>

<h2>Contenido</h2>
<ol class="indice">
<li>Descripción del negocio y problema</li><li>Objetivo general y alcance</li><li>Actores y roles</li><li>Módulos</li>
<li>Reglas de negocio</li><li>DER y relaciones</li><li>Diccionario de datos</li><li>Constraints y evolución del esquema</li>
<li>Seguridad y matriz de permisos</li><li>Vistas, procedimientos, funciones y triggers</li><li>Transacciones y manejo de errores</li>
<li>Arquitectura de Windows Forms</li><li>Casos de prueba</li><li>Capturas principales</li><li>Conclusiones y mejoras futuras</li>
</ol>

<h2>1. Descripción del negocio y problema</h2>
<p><b>TecnoStore</b> es una cadena de tres tiendas de tecnología (Centro, Polanco y Satélite) que vende celulares, laptops, computadoras,
tabletas, relojes, audífonos y accesorios, nuevos y reacondicionados, y tiene un taller que repara equipos de clientes.</p>
<p>Hoy el negocio lleva el inventario, las ventas y el taller en hojas de cálculo separadas. Eso provoca: ventas de piezas que ya no hay,
equipos con número de serie que no se sabe a quién se vendieron, garantías que nadie registra, órdenes de taller que "se pierden" entre
estados, descuentos sin control y ningún rastro de quién cambió un precio. El sistema resuelve esos problemas con una base de datos que
<b>protege la integridad por sí misma</b> (constraints, transacciones, triggers y roles) y una aplicación de escritorio para el personal.</p>
<p>Decisión clave del modelo: los equipos con número de serie se controlan <b>pieza por pieza</b> (<code>UnidadSerie</code>); los accesorios
y refacciones se controlan <b>por cantidad</b> (<code>Producto.stock_actual</code>). <code>Producto.requiere_serie</code> distingue ambos casos.</p>

<h2>2. Objetivo general y alcance</h2>
<p><b>Objetivo:</b> diseñar y desarrollar un sistema de escritorio que administre ventas, compras, inventario, taller, clientes, proveedores
y usuarios de TecnoStore, con una base de datos en SQL Server que aplique las reglas del negocio, soporte procesos transaccionales,
limite accesos por rol, registre los cambios sensibles y dé información para decidir.</p>
<p><b>Alcance logrado:</b></p>
%(alcance)s
<p><b>Fuera de alcance:</b> facturación electrónica (CFDI), pagos con tarjeta en línea, tienda web y envíos a domicilio.</p>

<h2>3. Actores y roles</h2>
%(actores)s

<h2>4. Módulos</h2>
%(modulos)s

<h2>5. Reglas de negocio</h2>
<p>Cada regla está implementada en la capa indicada; las que importan para la integridad viven en SQL Server aunque la app también valide.</p>
%(reglas)s

<h2 class="salto">6. DER y relaciones</h2>
<figure><img src="file://%(der)s"><figcaption>Diagrama entidad-relación (también en DER.png y DER.pdf)</figcaption></figure>
<h3>Relaciones importantes</h3>
<p><b>1. Venta → DetalleVenta → UnidadSerie → Garantia.</b> Cada venta tiene una o más líneas. Si el producto requiere serie, cada línea
apunta a una pieza física (<code>id_unidad</code>), y de esa línea nace su garantía (relación 1:1 con UNIQUE en <code>id_detalle_venta</code>).
Así se sabe exactamente qué número de serie se llevó cada cliente y hasta cuándo está cubierto.</p>
<p><b>2. Compra → DetalleCompra → UnidadSerie.</b> Las piezas entran por compra; <code>UnidadSerie.id_detalle_compra</code> dice de qué
compra y proveedor viene cada pieza. Admite NULL para equipos usados que entraron de un cliente y se reacondicionaron.</p>
<p><b>3. OrdenServicio → DetalleOrdenServicio (TipoServicio o Producto) → HistorialEstadoOrden.</b> Cada línea de la orden es mano de obra
o refacción (un CHECK obliga a que tenga al menos una de las dos), y cada cambio de estado queda en el historial con el usuario.</p>
<h3>Relaciones muchos-a-muchos</h3>
<p><b>ProductoProveedor</b> (un producto lo surten varios proveedores y un proveedor surte muchos productos, con el costo de cada uno),
<b>UsuarioRol</b> (usuarios y roles) y <b>EmpleadoTipoServicio</b> (qué servicios puede hacer cada técnico; la usa
<code>fn_TecnicoHabilitado</code> para impedir que se asigne una reparación de placa a quien no está capacitado).</p>

<h2>7. Diccionario de datos</h2>
<p>El diccionario completo, con nombre, tipo, tamaño, nulos, valor por defecto, constraints, descripción y ejemplo de cada columna, está en
<b>DiccionarioDatos.pdf</b> (generado de los catálogos de SQL Server). Resumen por tabla:</p>
%(resumen_tablas)s
<p><b>Uso de NULL en campos importantes:</b> <code>OrdenServicio.fecha_entrega</code> es NULL mientras el equipo está en taller;
<code>numero_serie_equipo</code> lo es cuando el cliente no lo conoce; <code>UnidadSerie.id_detalle_compra</code> cuando la pieza llegó de un
cliente; <code>DetalleVenta.id_unidad</code> cuando el producto se vende por cantidad; <code>Cliente.rfc</code> cuando no pide factura;
<code>HistorialEstadoOrden.id_estado_anterior</code> en el alta. En los demás casos los campos importantes son NOT NULL.</p>

<h2>8. Constraints y evolución del esquema</h2>
%(conteos)s
<h3>CHECK (reglas de dominio)</h3>%(checks)s
<h3>Evolución del esquema (03_Alter_Constraints.sql)</h3>
%(alter)s

<h2>9. Seguridad y matriz de permisos</h2>
<p>La seguridad existe dentro de SQL Server, no solo en la aplicación. Se crearon %(roles)d roles de base de datos con permisos diferenciados
y usuarios de prueba asociados a cada rol. La aplicación puede conectarse con el login de cada perfil (<code>ModoConexion = Roles</code>
en App.config), de modo que aunque alguien quitara una validación del código, SQL Server seguiría negando lo que el rol no tiene.</p>
%(matriz)s
<p>Se usan <b>GRANT</b> (lectura del esquema, ejecución de procedimientos), <b>DENY</b> (tablas de seguridad, auditoría, reporte de
utilidad para el operador, escritura en catálogos) y <b>REVOKE</b> (se retira un permiso dado por error). Ningún usuario operativo tiene
<code>db_owner</code>. La demostración con <code>EXECUTE AS USER</code> / <code>REVERT</code> está al final de 07_Seguridad.sql.
El detalle de permisos reales está en <b>MatrizPermisos.pdf</b>.</p>
<p>Además, las contraseñas de la aplicación se guardan como <code>HASHBYTES('SHA2_256')</code>, nunca en texto plano; tras 5 intentos
fallidos el usuario se bloquea y solo un Administrador lo reactiva.</p>

<h2>10. Vistas, procedimientos, funciones y triggers</h2>
<h3>Vistas (%(n_vistas)d)</h3>%(vistas)s
<h3>Stored Procedures (%(n_procs)d)</h3>
<p>Todos validan sus datos y comunican errores de negocio con <code>THROW</code> (números 50000+), que la aplicación muestra tal cual.
Los de procesos usan transacción. Parámetros opcionales en usp_ProductoBuscar, usp_ResumenPeriodo, usp_ActividadEmpleados,
usp_AjustarPrecio y los de guardado.</p>%(procs)s
<h3>Functions (%(n_funcs)d)</h3>%(funcs)s
<h3>Triggers (%(n_trg)d)</h3>
<p>Todos trabajan con <code>inserted</code> y <code>deleted</code> como conjuntos, así que funcionan con operaciones de varias filas
(por ejemplo, <code>usp_AjustarPrecio</code> cambia una categoría completa y se audita cada producto).</p>%(triggers)s

<h2>11. Transacciones y manejo de errores</h2>
<p>Los procesos que modifican varias tablas usan el mismo patrón: <code>SET XACT_ABORT ON</code>, validaciones con <code>THROW</code>,
<code>BEGIN TRY</code> / <code>BEGIN TRANSACTION</code> / <code>COMMIT</code> y en el <code>BEGIN CATCH</code> un <code>ROLLBACK</code> seguido de
<code>THROW</code> para que la aplicación reciba el mensaje. <code>SCOPE_IDENTITY()</code> obtiene el id del encabezado recién insertado.</p>
%(procesos)s
<p><b>¿Por qué hace falta la transacción en la venta?</b> Si se guardara el detalle pero fallara el descuento de stock, quedaría una venta
cobrada con inventario falso; si la pieza quedara "Vendida" sin garantía, el cliente perdería su cobertura. Con la transacción o pasa
todo o no pasa nada. La demostración con caso exitoso y fallido de los cuatro procesos está en <code>11_Transacciones.sql</code> y su salida
real en <code>04_Evidencias/salidas_sql/11_Transacciones.txt</code>.</p>
<h3>ACID</h3>
<p><b>Atomicidad:</b> la venta se guarda completa o no se guarda. <b>Consistencia:</b> al terminar se cumplen todas las constraints
(stock ≥ 0, precio ≥ costo, totales cuadrados). <b>Aislamiento:</b> otra caja no ve una venta a medias. <b>Durabilidad:</b> tras el
COMMIT la venta sobrevive a un apagón (log de transacciones).</p>
<h3>Lecturas anómalas y niveles de aislamiento</h3>
%(aislamiento)s
<p>Los procedimientos usan el nivel por defecto <b>READ COMMITTED</b> (sin lecturas sucias) y bloquean con <code>UPDLOCK, HOLDLOCK</code>
solo las filas que importan (stock del producto y último folio), para que dos cajas no vendan la última pieza ni generen el mismo folio,
sin bloquear toda la tabla como haría SERIALIZABLE. Al elegir pieza se usa <code>READPAST</code> para saltar piezas que otra caja tiene apartadas.</p>

<h2>12. Arquitectura de Windows Forms</h2>
<p>Solución de Visual Studio <code>03_WindowsForms/TecnoStore.sln</code>, C# con .NET Framework 4.8 (incluido en Windows 10/11; usa
<code>System.Data.SqlClient</code> sin paquetes extra). Organización:</p>
%(arquitectura)s
<p><b>Acceso a datos.</b> La clase <code>Db</code> centraliza la cadena de conexión (App.config), abre cada conexión con <code>using</code>
y le pasa a SQL Server el usuario de la app con <code>sp_set_session_context</code> (lo usan los triggers de auditoría y el historial).
Todo el SQL usa <code>SqlParameter</code> con tipo; ningún texto del usuario se concatena. Los formularios no contienen SQL: llaman a los
repositorios. Las excepciones se atrapan con try/catch y los mensajes de <code>THROW</code> se muestran en un MessageBox.</p>
<p><b>Experiencia de usuario.</b> DataGridView para listados y detalles, ComboBox para toda llave foránea (nunca se escriben IDs),
DateTimePicker para fechas, NumericUpDown para cantidades y precios, ErrorProvider y fondo rojo para campos obligatorios, MessageBox
para confirmar operaciones críticas, estados de lectura/edición con Nuevo, Editar, Guardar, Cancelar y Eliminar, y el botón Guardar se
desactiva mientras se procesa para evitar doble ejecución. Las tablas se refrescan después de cada operación.</p>
<p><b>Consumo de la base:</b> %(n_sp_app)d procedimientos, 5 vistas (vw_ProductosActivos, vw_VentaCompleta, vw_OrdenServicioCompleta,
vw_StockBajo, vw_TopProductos) y funciones en consultas (fn_TotalCompradoCliente, fn_ClasificacionCliente, fn_TotalOrden,
fn_VentasPorCategoria).</p>

<h2>13. Casos de prueba</h2>
<p>%(n_pruebas)d casos automáticos en <code>12_Pruebas.sql</code>; al ejecutarse sobre una base nueva pasan <b>%(pasan)d de %(n_pruebas)d</b>.
Cubren operaciones correctas, validaciones, transacciones fallidas, seguridad y triggers. El detalle con entrada, resultado esperado,
resultado obtenido y evidencia está en <b>CasosPrueba.pdf</b>, que también lista las pruebas a repetir en la aplicación.</p>
%(resumen_pruebas)s

<h2 class="salto">14. Capturas principales</h2>
<div class="nota">Estas capturas son de la <b>vista previa</b> del diseño, que usa la misma base y los mismos procedimientos. Antes de
entregar, sustitúyanlas por capturas de la aplicación Windows Forms ejecutándose en Windows (carpeta <code>04_Evidencias/capturas/</code>).</div>
%(capturas)s

<h2>15. Conclusiones y mejoras futuras</h2>
<p>El proyecto muestra la diferencia entre almacenar datos y construir una solución de información: la base no confía en la aplicación.
Las constraints impiden datos imposibles, las transacciones evitan información a medias, los roles limitan lo que cada perfil puede
hacer aunque se salte la interfaz, los triggers dejan rastro de los cambios sensibles y las vistas, funciones y reportes convierten los
movimientos en información para decidir (qué resurtir, qué se vende, quién vende, cuánto deja cada categoría).</p>
<p><b>Mejoras futuras:</b> facturación electrónica (CFDI) para clientes con RFC; cierre de caja diario; traspasos de inventario entre
sucursales; notificación al cliente cuando su equipo está listo; respaldo automático programado con restauración de prueba; dashboard con
gráficas en la aplicación; y uso de async/await en las consultas largas para que la interfaz no se congele.</p>
"""
    alcance = tabla(['Componente', 'Mínimo', 'Logrado'], [
        ['Tablas / llaves foráneas', '20 / 18', '%d / %d' % (C['tablas'], C['fks'])],
        ['CHECK / UNIQUE / DEFAULT', '8 / 4 / 6', '%d / %d / %d' % (C['checks'], C['uniques'], C['defaults'])],
        ['Procesos maestro-detalle', '3', '3 (venta, compra, orden de servicio)'],
        ['Vistas / Procedimientos / Funciones / Triggers', '6 / 8 / 4 / 4', '%d / %d / %d / %d' % (C['vistas'], C['procs'], C['funcs'], C['triggers'])],
        ['Roles de SQL Server', '4', str(C['roles'])],
        ['Procesos transaccionales', '4', '10 procedimientos con transacción + 1 transacción manual'],
        ['Consultas de la unidad 2', '12+', '40 (05_Consultas.sql)'],
        ['Formularios', '14', '15 módulos'],
        ['Reportes', '5', '5, con exportación a CSV'],
        ['Reglas de negocio', '15', str(len(d['REGLAS']))],
        ['Casos de prueba', '20', str(len(d['PRUEBAS']))],
    ])
    actores = tabla(['Actor', 'Rol en el sistema', 'Qué hace'], [
        ['Gerente', 'Administrador', 'Todo: catálogos, usuarios, seguridad, reportes, auditoría.'],
        ['Supervisor de tienda', 'Supervisor', 'Autoriza descuentos mayores a 10 %, cancela ventas, registra compras, ajusta stock y precios, ve auditoría y reportes.'],
        ['Vendedor / técnico', 'Operador', 'Vende, registra clientes, recibe equipos y avanza órdenes del taller. No ve utilidades ni auditoría.'],
        ['Analista / contador', 'Consulta', 'Solo lectura de catálogos, historial y reportes.'],
        ['Cliente', '(no usa el sistema)', 'Compra y deja equipos; sus datos y su historial viven en Cliente.'],
        ['Proveedor', '(no usa el sistema)', 'Surte mercancía; se registra en Proveedor y ProductoProveedor.'],
    ])
    modulos = tabla(['#', 'Módulo', 'Formulario', 'Objetos SQL que usa'],
                    [[str(n), e(m), '<code>%s</code>' % e(f), e(o)] for n, m, f, o in MODULOS])
    reglas = tabla(['#', 'Regla', 'Capa', 'Dónde'], [[str(i + 1), e(r), e(c), e(w)] for i, (r, c, w) in enumerate(d['REGLAS'])])
    resumen_tablas = tabla(['Tabla', 'Grupo', 'Filas demo', 'Propósito'], [
        ['<b>%s</b>' % e(t), e(g), str(d['FILAS'].get(t, 0)), e(desc)] for t, (g, desc) in d['TABLAS'].items()])
    conteos = tabla(['Tipo', 'Mínimo', 'Cantidad', 'Ejemplo'], [
        ['PRIMARY KEY', '20', str(C['tablas']), 'PK_Venta'],
        ['FOREIGN KEY', '18', str(C['fks']), 'FK_Venta_Cliente'],
        ['UNIQUE', '4', str(C['uniques']), 'UQ_Producto_sku, UQ_Cliente_correo, UQ_Venta_folio'],
        ['CHECK', '8', str(C['checks']), 'CK_Producto_precio_vs_costo'],
        ['DEFAULT', '6', str(C['defaults']), "DF_Venta_fecha (SYSDATETIME()), DF_Cliente_activo (1)"],
    ])
    checks = tabla(['Tabla', 'Constraint', 'Regla'], [[e(c['tabla']), '<code>%s</code>' % e(c['name']), '<code>%s</code>' % e(c['definition'])] for c in d['CHECKS']])
    alter = tabla(['#', 'Cambio', 'Motivo'], [
        ['1', 'Agregar <code>Cliente.rfc</code>', 'Clientes que piden factura'],
        ['2', 'Agregar <code>Producto.fecha_alta</code> NOT NULL con DEFAULT', 'Antigüedad en catálogo; el DEFAULT rellena las filas existentes'],
        ['3', 'Agregar datos de cancelación + CK_Venta_cancelacion', 'Toda cancelación deja fecha y motivo'],
        ['4', 'Agregar CK_Producto_precio_vs_costo', 'No vender por debajo del costo'],
        ['5', 'Ampliar <code>descripcion_falla</code> a 500', 'Los técnicos necesitaban más espacio; ampliar no pierde datos'],
        ['6', 'Agregar <code>OrdenServicio.id_sucursal</code> + FK', 'Nueva relación sobre tabla con datos: NULL → rellenar → NOT NULL → FK'],
        ['7', 'DEFAULT de estados en Venta, Compra y UnidadSerie', 'El estado inicial lo pone la base'],
        ['8', 'DROP y nuevo CK_Venta_descuento', 'Retirar constraint por nombre: el descuento no puede pasar del subtotal'],
        ['9', 'Bitácora de acceso en Usuario', 'Último acceso e intentos fallidos'],
        ['10', 'usuario_app y motivo en Auditoria', 'Saber qué persona hizo el cambio y por qué'],
        ['11', 'UQ_DetalleCompra_compra_producto', 'Un producto una sola vez por compra'],
        ['12', 'Tabla EmpleadoTipoServicio con FK por ALTER', 'Tercera relación N:M: técnicos habilitados por servicio'],
    ])
    matriz = tabla(['Objeto / módulo', 'Administrador', 'Supervisor', 'Operador', 'Consulta'], [[e(x) for x in f] for f in d['MATRIZ']])
    trg = tabla(['Trigger', 'Tipo', 'Qué hace', 'Por qué está en la base'], [
        ['<code>%s</code>' % e(n), e(DESCRIPCION_OBJETOS[n][0]), e(DESCRIPCION_OBJETOS[n][1]), e(POR_QUE_TRIGGER.get(n, ''))]
        for n, t in obj.items() if t == 'SQL_TRIGGER'])
    procesos = tabla(['Proceso', 'Procedimiento', 'Tablas que modifica', 'Validaciones', 'Si algo falla'], [
        ['1 Principal: venta', 'usp_RegistrarVenta', 'Venta, DetalleVenta, Producto, UnidadSerie, Garantia', 'Cliente y productos activos, stock, piezas disponibles, límite de descuento', 'ROLLBACK total'],
        ['2 Secundario: compra', 'usp_RegistrarCompra', 'Compra, DetalleCompra, Producto, UnidadSerie, ProductoProveedor', 'Proveedor activo, cantidades/costos, una serie por pieza sin repetir', 'ROLLBACK total'],
        ['3 Cancelación', 'usp_CancelarVenta', 'Venta, Producto, UnidadSerie, Garantia (+ Auditoria)', 'Existe, no está cancelada, máximo 30 días, motivo', 'Nada cambia'],
        ['4 Ajuste', 'usp_AjustarStock', 'Producto, Auditoria (trigger)', 'Rol Supervisor, ±20 piezas, no negativo, motivo', 'Nada cambia'],
    ])
    aislamiento = tabla(['Problema', 'Qué es', 'Ejemplo en TecnoStore', 'Lo evita'], [
        ['Lectura sucia', 'Leer datos de otra transacción que todavía no hace COMMIT', 'Un reporte cuenta una venta que luego hizo ROLLBACK', 'READ COMMITTED (por defecto)'],
        ['Lectura no repetible', 'Leer dos veces la misma fila y obtener valores distintos', 'Se valida stock = 1 y otra caja lo vende antes de descontar', 'REPEATABLE READ, o UPDLOCK/HOLDLOCK en esa fila'],
        ['Fantasma', 'Repetir una consulta y que aparezcan filas nuevas', 'Calcular el siguiente folio mientras otra caja inserta uno', 'SERIALIZABLE, o HOLDLOCK sobre el rango'],
    ])
    arquitectura = tabla(['Carpeta', 'Contenido'], [
        ['<code>Forms/</code>', 'Un formulario por módulo con su archivo .Designer.cs (editable en el diseñador de Visual Studio).'],
        ['<code>Models/</code>', 'Clases de entidades y de sesión (UsuarioSesion, LineaVenta, Opcion para ComboBox…).'],
        ['<code>Data/</code>', '<code>Db</code> (conexión centralizada y parámetros) y repositorios por módulo con todo el SQL.'],
        ['<code>Utils/</code>', '<code>Ui</code>: mensajes, validación visual y formato de DataGridView.'],
        ['<code>App.config</code>', 'Cadenas de conexión (única o una por rol) y modo de conexión.'],
    ])
    resumen_pruebas = tabla(['Caso', 'Tipo', 'Descripción', 'Resultado'], [
        [e(p['caso']), e(p['tipo']), e(p['descripcion']),
         '<span class="%s">%s</span>' % ('ok' if p['resultado'] == 'PASA' else 'no', e(p['resultado']))] for p in d['PRUEBAS']])
    capturas = ''.join(figura(a, p) for a, p in [
        ('01_login', 'Inicio de sesión'), ('02_inicio_admin', 'Inicio del Administrador'), ('09_nueva_venta', 'Nueva venta'),
        ('10_ordenes', 'Órdenes de servicio'), ('11_compras', 'Compras a proveedor'), ('13_reportes', 'Reportes'),
        ('16_auditoria', 'Auditoría'), ('18_inicio_consulta', 'Menú del perfil Consulta')])
    sp_app = ['usp_Login', 'usp_ProductoBuscar', 'usp_ProductoGuardar', 'usp_ClienteGuardar', 'usp_ProveedorGuardar', 'usp_UsuarioGuardar',
              'usp_RegistrarVenta', 'usp_RegistrarCompra', 'usp_RegistrarOrdenServicio', 'usp_OrdenAgregarLinea', 'usp_CambiarEstadoOrden',
              'usp_CancelarVenta', 'usp_AjustarStock', 'usp_AjustarPrecio', 'usp_ResumenPeriodo', 'usp_ActividadEmpleados']

    cuerpo = h % dict(
        hoy=d['HOY'], alcance=alcance, actores=actores, modulos=modulos, reglas=reglas, der=der, resumen_tablas=resumen_tablas,
        conteos=conteos, checks=checks, alter=alter, roles=C['roles'], matriz=matriz,
        n_vistas=C['vistas'], vistas=objetos(('VIEW',), ['Vista', 'Tipo', 'Descripción']),
        n_procs=C['procs'], procs=objetos(('SQL_STORED_PROCEDURE',), ['Procedimiento', 'Grupo', 'Descripción']),
        n_funcs=C['funcs'], funcs=objetos(('SQL_SCALAR_FUNCTION', 'SQL_INLINE_TABLE_VALUED_FUNCTION'), ['Función', 'Tipo', 'Descripción']),
        n_trg=C['triggers'], triggers=trg, procesos=procesos, aislamiento=aislamiento, arquitectura=arquitectura,
        n_sp_app=len(sp_app), n_pruebas=len(d['PRUEBAS']), pasan=pasan, resumen_pruebas=resumen_pruebas, capturas=capturas)
    return d['pagina']('Documento técnico TecnoStore', cuerpo)


def manual(d):
    e, tabla, figura = d['e'], d['tabla'], d['figura']

    def seccion(titulo, captura, pie, pasos, notas=''):
        return ('<h2>%s</h2>%s<ol>%s</ol>%s' % (e(titulo), figura(captura, pie, '92%'),
                                                ''.join('<li>%s</li>' % p for p in pasos), notas))

    h = """<div class="portada"><h1>TecnoStore</h1><div class="sub">Manual de usuario</div>
<p style="margin-top:18pt">Sistema Integral de Gestión · versión 1.0 · %s</p></div>
<div class="nota">Las imágenes son de la vista previa del diseño. La aplicación de Windows Forms tiene las mismas pantallas, campos y
botones; antes de entregar, sustituyan las imágenes por capturas de la aplicación en Windows.</div>
<h2>1. Instalación</h2>
<ol>
<li>En SQL Server Management Studio, abrir y ejecutar (F5) los scripts de la carpeta <code>02_SQL</code> en este orden:
<code>01_CrearBase</code>, <code>02_CrearTablas</code>, <code>03_Alter_Constraints</code>, <code>04_DatosDemo</code>, <code>06_Vistas</code>,
<code>08_StoredProcedures</code>, <code>09_Functions</code>, <code>10_Triggers</code> y al final <code>07_Seguridad</code>.</li>
<li>Abrir <code>03_WindowsForms/TecnoStore.sln</code> con Visual Studio 2019 o 2022 (carga de trabajo "Desarrollo de escritorio de .NET").</li>
<li>En <code>App.config</code>, revisar la cadena <code>Unica</code>: <code>Server=localhost</code> o <code>Server=.\\SQLEXPRESS</code> según su instalación.</li>
<li>Presionar F5.</li>
</ol>
<h2>2. Usuarios de prueba</h2>
%s
<h2>3. Conceptos comunes de todas las pantallas</h2>
<ul>
<li><b>Lectura y edición:</b> al seleccionar un registro de la tabla, sus datos aparecen a la derecha en modo lectura. <b>Nuevo</b> limpia el
formulario; <b>Editar</b> habilita los campos; <b>Guardar</b> confirma; <b>Cancelar</b> descarta.</li>
<li><b>Campos obligatorios</b> llevan asterisco (*). Si falta uno, se marca en rojo y no se guarda.</li>
<li><b>Listas desplegables:</b> clientes, productos, proveedores y demás se eligen de una lista; nunca se escribe un número de id.</li>
<li><b>Mensajes de SQL Server:</b> si la base rechaza algo (stock insuficiente, permiso, duplicado), el mensaje lo explica y no se guarda nada.</li>
<li><b>Bajas:</b> clientes, productos y proveedores con historial no se borran: se desmarcan como "Activo".</li>
<li><b>Atajos:</b> F2 Productos · F3 Clientes · F4 Proveedores · F5 Nueva venta · F6 Órdenes · F7 Compras · Enter busca en los filtros.</li>
</ul>
""" % (d['HOY'], tabla(['Usuario', 'Contraseña', 'Perfil', 'Qué ve'], [
        ['admin', '1234', 'Administrador', 'Todo'],
        ['csupervisor', '1234', 'Supervisor', 'Todo menos usuarios'],
        ['loperador', '1234', 'Operador', 'Ventas, clientes, órdenes, catálogos en lectura'],
        ['fconsulta', '1234', 'Consulta', 'Solo lectura y reportes'],
    ]))
    h += seccion('4. Iniciar sesión', '01_login', 'Inicio de sesión', [
        'Escribir usuario y contraseña y presionar <b>Ingresar</b> (o Enter).',
        'Si la contraseña es incorrecta 5 veces seguidas, el usuario se bloquea; un Administrador lo reactiva en Usuarios.',
        'El menú muestra solo las opciones de su perfil. <b>Archivo → Cerrar sesión</b> vuelve a esta pantalla.'])
    h += seccion('5. Inicio', '02_inicio_admin', 'Inicio', [
        'Muestra ventas e ingresos de los últimos 30 días, alertas de stock y órdenes abiertas.',
        'Las filas amarillas o rojas son productos en o por debajo del mínimo.'])
    h += seccion('6. Productos', '03_productos', 'Productos', [
        'Buscar por nombre o SKU, filtrar por categoría o marcar <b>Solo con alerta de stock</b> y presionar <b>Buscar</b>.',
        'Supervisor o Administrador: <b>Nuevo</b> o <b>Editar</b>, llenar los datos y <b>Guardar</b>. El precio no puede ser menor al costo.',
        'El stock no se edita aquí: entra por <b>Compras</b> y se corrige en <b>Ajustes de inventario</b>.',
        'Administrador: <b>Eliminar</b> borra un producto sin movimientos; si ya tuvo ventas o compras, solo lo desactiva.'])
    h += seccion('7. Clientes', '04_clientes', 'Clientes', [
        'Buscar por nombre, apellidos o correo. Al seleccionar un cliente se ve su historial de compras abajo.',
        '<b>Nuevo</b> / <b>Editar</b> → nombre, apellidos y correo obligatorios; RFC opcional (12 o 13 caracteres).',
        'Para dar de baja, desmarcar <b>Activo</b>: el cliente ya no aparece al vender, pero su historial se conserva.',
        'La columna <b>Nivel</b> (Oro, Plata, Bronce) la calcula SQL Server según el total comprado.'])
    h += seccion('8. Proveedores', '05_proveedores', 'Proveedores', [
        'Al seleccionar un proveedor se ven los productos que surte y a qué costo.',
        'Supervisor o Administrador pueden dar de alta o editar; el RFC no se puede repetir.'])
    h += seccion('9. Catálogos: categorías, marcas y tipos de servicio', '06_categorias', 'Categorías', [
        'Las tres pantallas funcionan igual (menú <b>Catálogos</b>). Solo el Administrador modifica; los demás consultan.',
        'En <b>Tipos de servicio</b> se captura el precio base de la mano de obra.',
        '<b>Eliminar</b> solo funciona si nadie lo usa; si hay productos u órdenes con ese registro, el sistema lo impide.'])
    h += seccion('10. Nueva venta', '09_nueva_venta', 'Nueva venta', [
        'Elegir el <b>cliente</b> y el <b>método de pago</b>.',
        'Elegir un producto, la cantidad y presionar <b>Agregar</b>. Repetir por cada producto. <b>Quitar línea</b> elimina la seleccionada.',
        'Descuento: hasta 10 % cualquier vendedor; más de 10 % solo si quien está en sesión es Supervisor o Administrador.',
        'Presionar <b>Guardar venta</b> y confirmar. Aparece el folio. Si algo falla (por ejemplo, ya no hay stock), no se guarda nada.',
        'Los equipos con número de serie toman automáticamente una pieza disponible y generan su garantía.'])
    h += seccion('11. Órdenes de servicio (taller)', '10_ordenes', 'Órdenes de servicio', [
        '<b>Recibir equipo:</b> elegir cliente y técnico, describir equipo y falla, y <b>Registrar orden</b>. Se agrega el diagnóstico.',
        'Seleccionar una orden para ver su detalle y su historial de estados.',
        '<b>Mano de obra:</b> elegir el servicio y <b>Agregar</b>. Solo se permite si el técnico está habilitado para ese servicio.',
        '<b>Refacción:</b> elegir la pieza y la cantidad y <b>Agregar</b>; sale del almacén en ese momento.',
        '<b>Cambiar estado:</b> la orden avanza un paso a la vez (Recibida → En diagnóstico → En reparación → Lista → Entregada) o se cancela. '
        'Una orden Entregada o Cancelada ya no cambia.'])
    h += seccion('12. Compras a proveedor', '11_compras', 'Compras', [
        'Solo Supervisor y Administrador. Elegir el <b>proveedor</b>.',
        'Elegir producto, cantidad y costo unitario. Si el producto lleva número de serie, escribir un número por renglón (tantos como piezas).',
        '<b>Agregar</b> cada producto y al final <b>Guardar compra</b>. El stock y las piezas entran al guardar; si un número de serie ya existe, no se guarda nada.'])
    h += seccion('13. Historial de ventas y cancelaciones', '12_historial', 'Historial de ventas', [
        'Filtrar por rango de fechas, estado y folio o cliente; seleccionar una venta para ver su detalle, números de serie y garantías.',
        'Supervisor o Administrador: <b>Cancelar venta…</b>, escribir el motivo y confirmar. Solo ventas de los últimos 30 días; el stock regresa al almacén.'])
    h += seccion('14. Reportes', '13_reportes', 'Reportes', [
        'Elegir el rango de fechas y presionar <b>Generar</b>.',
        'Pestañas: Resumen por periodo, Top 10 productos, Actividad por empleado, Stock bajo y Ventas por categoría.',
        '<b>Exportar CSV</b> guarda la pestaña visible para abrirla en Excel.'])
    h += seccion('15. Usuarios y roles', '14_usuarios', 'Usuarios', [
        'Solo Administrador. <b>Nuevo</b>: elegir el empleado, escribir el usuario, el rol y la contraseña (8+ caracteres con un número).',
        '<b>Editar</b>: cambiar rol o bloquear (desmarcar Activo). Dejar la contraseña vacía conserva la actual.',
        'Reactivar un usuario bloqueado reinicia sus intentos fallidos. Nadie puede quitarse a sí mismo el rol de Administrador.'])
    h += seccion('16. Ajustes de inventario y precios', '15_ajustes', 'Ajustes', [
        'Solo Supervisor y Administrador.',
        '<b>Inventario:</b> elegir el producto, la cantidad (+ entra, − sale, máximo 20) y el motivo; <b>Aplicar ajuste</b>.',
        '<b>Precios:</b> elegir la categoría y el porcentaje (máximo ±30 %); <b>Aplicar precios</b>. Si algún precio quedara debajo del costo, no se cambia ninguno.'])
    h += seccion('17. Auditoría', '16_auditoria', 'Auditoría', [
        'Solo Supervisor y Administrador. Muestra cada cambio de precio, de stock y de estado de ventas: fecha, usuario, valor anterior, valor nuevo y motivo.',
        'Es de solo lectura: nadie puede editarla ni borrarla.'])
    h += ('<h2>18. Mensajes frecuentes</h2>' + tabla(['Mensaje', 'Qué significa', 'Qué hacer'], [
        ['Stock insuficiente para uno o más productos', 'Alguien vendió las piezas o no hay suficientes', 'Bajar la cantidad o registrar una compra'],
        ['Un descuento mayor al permitido requiere autorización…', 'El operador no puede dar más de 10 %', 'Pedir a un supervisor que haga la venta'],
        ['Tu perfil no tiene permiso para esta operación', 'SQL Server negó el permiso al rol', 'Entrar con el perfil adecuado'],
        ['Ya existe… con ese correo / SKU / RFC', 'El dato debe ser único', 'Buscar el registro existente'],
        ['No se puede eliminar: … lo usan', 'El registro tiene historial', 'Desactivarlo en lugar de borrarlo'],
        ['Una orden Entregada o Cancelada ya no puede cambiar…', 'La orden está cerrada', 'Abrir una orden nueva si hace falta'],
        ['El usuario está bloqueado', '5 intentos fallidos', 'Un Administrador lo reactiva en Usuarios'],
    ]))
    return d['pagina']('Manual de usuario TecnoStore', h)
