# Requisitos del profesor → dónde está cada uno

Mapa de cada requisito del [documento del proyecto](<Contexto/ProyectoFinal8Semanas (1).docx>) con enlaces directos al archivo y a la línea donde se cumple.
Útil para la revisión y para el guion de la demostración (sección 24).

**Estado:** ✅ cumple · 🟡 parcial · ❌ falta

---

## Resumen (sección 3 · Alcance mínimo)

| Componente | Mínimo | Tenemos | | Dónde |
|---|---|---|---|---|
| Tablas | 20 | 28 | ✅ | [02_CrearTablas.sql](02_SQL/02_CrearTablas.sql) + [EmpleadoTipoServicio](02_SQL/03_Alter_Constraints.sql#L100) |
| Relaciones FK | 18 | 38 | ✅ | [02_CrearTablas.sql](02_SQL/02_CrearTablas.sql) |
| Procesos maestro-detalle | 3 | 3 | ✅ | [Venta](02_SQL/02_CrearTablas.sql#L172), [Compra](02_SQL/02_CrearTablas.sql#L129), [OrdenServicio](02_SQL/02_CrearTablas.sql#L208) |
| CHECK | 8 | 32 | ✅ | p. ej. [CK_Producto_precio_vs_costo](02_SQL/03_Alter_Constraints.sql#L36) |
| UNIQUE | 4 | 20 | ✅ | p. ej. [UQ_Categoria_nombre](02_SQL/02_CrearTablas.sql#L14) |
| Vistas | 6 | 6 | ✅ | [06_Vistas.sql](02_SQL/06_Vistas.sql) |
| Stored Procedures | 8 | 16 | ✅ | [08_StoredProcedures.sql](02_SQL/08_StoredProcedures.sql) |
| Functions | 4 | 7 | ✅ | [09_Functions.sql](02_SQL/09_Functions.sql) |
| Triggers | 4 | 5 | ✅ | [10_Triggers.sql](02_SQL/10_Triggers.sql) |
| Roles SQL Server | 4 | 4 | ✅ | [07_Seguridad.sql](02_SQL/07_Seguridad.sql#L58) |
| Transacciones | 4 | 7 + 1 manual | ✅ | [11_Transacciones.sql](02_SQL/11_Transacciones.sql) |
| Consultas analíticas | 12 | 40 | ✅ | [05_Consultas.sql](02_SQL/05_Consultas.sql) |
| Formularios/módulos | 14 | 15 | ✅ | [03_WindowsForms/TecnoStore/Forms](03_WindowsForms/TecnoStore/Forms) |
| Reportes | 5 | 5 | ✅ | [FrmReportes.cs](03_WindowsForms/TecnoStore/Forms/FrmReportes.cs) |
| Reglas de negocio | 15 | 22 | ✅ | [ver tabla abajo](#14--reglas-de-negocio) |
| Casos de prueba | 20 | 32 | ✅ | [12_Pruebas.sql](02_SQL/12_Pruebas.sql) |

---

## 4 · Modelo de datos (28 tablas)

| Grupo | Tablas |
|---|---|
| Catálogos (6) | [Categoria](02_SQL/02_CrearTablas.sql#L12) · [Marca](02_SQL/02_CrearTablas.sql#L18) · [Condicion](02_SQL/02_CrearTablas.sql#L24) · [MetodoPago](02_SQL/02_CrearTablas.sql#L29) · [TipoServicio](02_SQL/02_CrearTablas.sql#L34) · [EstadoOrden](02_SQL/02_CrearTablas.sql#L40) |
| Entidades maestras (5) | [Sucursal](02_SQL/02_CrearTablas.sql#L47) · [Cliente](02_SQL/02_CrearTablas.sql#L54) · [Empleado](02_SQL/02_CrearTablas.sql#L64) · [Proveedor](02_SQL/02_CrearTablas.sql#L74) · [Producto](02_SQL/02_CrearTablas.sql#L83) |
| Encabezados (3) | [Venta](02_SQL/02_CrearTablas.sql#L172) · [Compra](02_SQL/02_CrearTablas.sql#L129) · [OrdenServicio](02_SQL/02_CrearTablas.sql#L208) |
| Detalles (3) | [DetalleVenta](02_SQL/02_CrearTablas.sql#L187) · [DetalleCompra](02_SQL/02_CrearTablas.sql#L139) · [DetalleOrdenServicio](02_SQL/02_CrearTablas.sql#L224) |
| Relaciones N:M (3) | [UsuarioRol](02_SQL/02_CrearTablas.sql#L115) · [ProductoProveedor](02_SQL/02_CrearTablas.sql#L121) · [EmpleadoTipoServicio](02_SQL/03_Alter_Constraints.sql#L100) |
| Seguridad (2) | [Usuario](02_SQL/02_CrearTablas.sql#L101) · [Rol](02_SQL/02_CrearTablas.sql#L110) |
| Inventario por pieza (3) | [UnidadSerie](02_SQL/02_CrearTablas.sql#L148) · [Reacondicionado](02_SQL/02_CrearTablas.sql#L157) · [Garantia](02_SQL/02_CrearTablas.sql#L197) |
| Auditoría / soporte (3) | [HistorialEstadoOrden](02_SQL/02_CrearTablas.sql#L237) · [Auditoria](02_SQL/02_CrearTablas.sql#L246) · [Configuracion](02_SQL/02_CrearTablas.sql#L257) |

- Diccionario de datos completo (generado de SQL Server): [DiccionarioDatos.pdf](01_Documentacion/DiccionarioDatos.pdf) ✅
- DER: [DER.png](01_Documentacion/DER.png) · [DER.pdf](01_Documentacion/DER.pdf) ✅

## 5.1 · Evolución del esquema (mínimo 8 ALTER) — [03_Alter_Constraints.sql](02_SQL/03_Alter_Constraints.sql)

| # | Cambio | |
|---|---|---|
| 1 | [Agregar columna `rfc` a Cliente](02_SQL/03_Alter_Constraints.sql#L12) | agregar columna |
| 2 | [`fecha_alta` NOT NULL con DEFAULT](02_SQL/03_Alter_Constraints.sql#L18) | columna + DEFAULT |
| 3 | [Datos de cancelación + CHECK](02_SQL/03_Alter_Constraints.sql#L25) | columnas + constraint |
| 4 | [Precio ≥ costo](02_SQL/03_Alter_Constraints.sql#L36) | constraint CHECK |
| 5 | [Ampliar `descripcion_falla`](02_SQL/03_Alter_Constraints.sql#L41) | modificar longitud |
| 6 | [Nueva FK a Sucursal conservando datos](02_SQL/03_Alter_Constraints.sql#L47) | relación nueva |
| 7 | [DEFAULT de estados](02_SQL/03_Alter_Constraints.sql#L62) | valores DEFAULT |
| 8 | [Retirar y recrear CK_Venta_descuento](02_SQL/03_Alter_Constraints.sql#L70) | retirar constraint por nombre |
| 9 | [Bitácora de acceso en Usuario](02_SQL/03_Alter_Constraints.sql#L77) | columnas |
| 10 | [usuario_app y motivo en Auditoria](02_SQL/03_Alter_Constraints.sql#L85) | columnas |
| 11 | [UNIQUE compuesto](02_SQL/03_Alter_Constraints.sql#L94) | UNIQUE |
| 12 | [Tabla N:M EmpleadoTipoServicio con FK por ALTER](02_SQL/03_Alter_Constraints.sql#L100) | relación nueva |

## 6 · Datos demo — [04_DatosDemo.sql](02_SQL/04_DatosDemo.sql)

| Requisito | Mínimo | Tenemos | |
|---|---|---|---|
| Registros por catálogo principal | 10 | 10 (Categoría, Marca, TipoServicio) | ✅ |
| Clientes | 25 | 30 (5 sin compras, 1 inactivo) | ✅ |
| Productos | 30 | 34 (1 sin ventas, 1 inactivo) | ✅ |
| Proveedores / empleados | 10 | 10 / 12 | ✅ |
| Encabezados | 40 | 45 ventas + 12 compras + 24 órdenes | ✅ |
| Detalles | 100 | 122 de venta | ✅ |

## 7.2 · Consultas — [05_Consultas.sql](02_SQL/05_Consultas.sql)

| Requisito | Mín. | Consultas |
|---|---|---|
| WHERE compuesto | 5 | [Q01–Q05](02_SQL/05_Consultas.sql#L24) |
| LIKE / texto | 3 | [Q06–Q08](02_SQL/05_Consultas.sql#L55) |
| Rango de fechas | 3 | [Q09–Q11](02_SQL/05_Consultas.sql#L68) |
| GROUP BY | 4 | [Q12–Q15](02_SQL/05_Consultas.sql#L85) |
| HAVING | 2 | [Q16–Q17](02_SQL/05_Consultas.sql#L107) |
| Funciones fecha/texto/conversión | 4 | [Q18–Q21](02_SQL/05_Consultas.sql#L122) |
| INNER JOIN | 5 | [Q22–Q26](02_SQL/05_Consultas.sql#L146) |
| LEFT JOIN | 3 | [Q27–Q29](02_SQL/05_Consultas.sql#L177) |
| 5 o más tablas | 2 | [Q30 (7 tablas)](02_SQL/05_Consultas.sql#L197), [Q31 (6 tablas)](02_SQL/05_Consultas.sql#L210) |
| Subconsultas normales | 2 | [Q32–Q33](02_SQL/05_Consultas.sql#L221) |
| Subconsultas correlacionadas | 2 | [Q34–Q35](02_SQL/05_Consultas.sql#L235) |
| Operadores SET (3 distintos) | 4 | [UNION](02_SQL/05_Consultas.sql#L249), [UNION ALL](02_SQL/05_Consultas.sql#L255), [INTERSECT](02_SQL/05_Consultas.sql#L263), [EXCEPT](02_SQL/05_Consultas.sql#L268) |
| Paginación | — | [Q40 OFFSET/FETCH](02_SQL/05_Consultas.sql#L273) |

## 7.3 · Vistas — [06_Vistas.sql](02_SQL/06_Vistas.sql)

| Tipo | Vistas |
|---|---|
| Operativas | [vw_ProductosActivos](02_SQL/06_Vistas.sql#L9) · [vw_StockBajo](02_SQL/06_Vistas.sql#L23) |
| Relacionales | [vw_VentaCompleta](02_SQL/06_Vistas.sql#L36) · [vw_OrdenServicioCompleta](02_SQL/06_Vistas.sql#L50) |
| Analíticas | [vw_ResumenMensual](02_SQL/06_Vistas.sql#L64) · [vw_TopProductos](02_SQL/06_Vistas.sql#L75) |

## 8 · Seguridad — [07_Seguridad.sql](02_SQL/07_Seguridad.sql)

| Requisito | Dónde |
|---|---|
| 4 roles | [CREATE ROLE](02_SQL/07_Seguridad.sql#L58) |
| Usuarios de prueba | [logins](02_SQL/07_Seguridad.sql#L1) |
| Matriz de permisos | [encabezado del script](02_SQL/07_Seguridad.sql#L15) · [MatrizPermisos.pdf](01_Documentacion/MatrizPermisos.pdf) |
| GRANT | [lectura general](02_SQL/07_Seguridad.sql#L76), [procesos del operador](02_SQL/07_Seguridad.sql#L104) |
| DENY | [seguridad](02_SQL/07_Seguridad.sql#L86), [auditoría](02_SQL/07_Seguridad.sql#L92), [reporte limitado](02_SQL/07_Seguridad.sql#L99) |
| REVOKE | [demostración](02_SQL/07_Seguridad.sql#L135) |
| Sin db_owner | [administrador por esquema](02_SQL/07_Seguridad.sql#L128) |
| EXECUTE AS / REVERT | [demostraciones](02_SQL/07_Seguridad.sql#L144) |
| Login de la app con hash | [usp_Login](02_SQL/08_StoredProcedures.sql#L34) |

## 9 · Transacciones — [11_Transacciones.sql](02_SQL/11_Transacciones.sql)

| Proceso | SP | Demostración (éxito y ROLLBACK) |
|---|---|---|
| 1 Principal: venta | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) | [ver](02_SQL/11_Transacciones.sql#L33) |
| 2 Secundario: compra | [usp_RegistrarCompra](02_SQL/08_StoredProcedures.sql#L404) | [ver](02_SQL/11_Transacciones.sql#L62) |
| 3 Cancelación | [usp_CancelarVenta](02_SQL/08_StoredProcedures.sql#L638) | [ver](02_SQL/11_Transacciones.sql#L87) |
| 4 Ajuste | [usp_AjustarStock](02_SQL/08_StoredProcedures.sql#L686) | [ver](02_SQL/11_Transacciones.sql#L109) |
| Transacción manual (sin SP) | — | [baja de pieza](02_SQL/11_Transacciones.sql#L126) |
| ACID, aislamiento, lecturas sucias | — | [explicación](02_SQL/11_Transacciones.sql#L1) |

Todas usan BEGIN TRY/CATCH, BEGIN TRANSACTION, COMMIT, ROLLBACK, THROW y SCOPE_IDENTITY().

## 10 · Stored Procedures — [08_StoredProcedures.sql](02_SQL/08_StoredProcedures.sql)

| Grupo | Procedimiento | Parámetros opcionales | Transacción | Usado en WinForms |
|---|---|---|---|---|
| Seguridad | [usp_Login](02_SQL/08_StoredProcedures.sql#L34) | | | [AuthRepository](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L12) |
| Catálogos | [usp_ProductoBuscar](02_SQL/08_StoredProcedures.sql#L72) | ✅ | | [ProductoRepository.Buscar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L57) |
| Catálogos | [usp_ProductoGuardar](02_SQL/08_StoredProcedures.sql#L92) | ✅ | | [ProductoRepository.Guardar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L72) |
| Catálogos | [usp_ClienteGuardar](02_SQL/08_StoredProcedures.sql#L140) | ✅ | | [ClienteRepository.Guardar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L141) |
| Catálogos | [usp_ProveedorGuardar](02_SQL/08_StoredProcedures.sql#L177) | ✅ | | [ProveedorRepository.Guardar](03_WindowsForms/TecnoStore/Data/RepositoriosAdmin.cs#L142) |
| Seguridad | [usp_UsuarioGuardar](02_SQL/08_StoredProcedures.sql#L220) | ✅ | ✅ | [UsuarioRepository.Guardar](03_WindowsForms/TecnoStore/Data/RepositoriosAdmin.cs#L249) |
| Procesos | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) | ✅ | ✅ | [VentaRepository.Registrar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L159) |
| Procesos | [usp_RegistrarCompra](02_SQL/08_StoredProcedures.sql#L404) | | ✅ | [CompraRepository.Registrar](03_WindowsForms/TecnoStore/Data/RepositoriosAdmin.cs#L194) |
| Procesos | [usp_RegistrarOrdenServicio](02_SQL/08_StoredProcedures.sql#L491) | ✅ | ✅ | [OrdenRepository.Registrar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L249) |
| Procesos | [usp_OrdenAgregarLinea](02_SQL/08_StoredProcedures.sql#L544) | ✅ | ✅ | [OrdenRepository.AgregarLinea](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L263) |
| Procesos | [usp_CambiarEstadoOrden](02_SQL/08_StoredProcedures.sql#L602) | | ✅ | [OrdenRepository.CambiarEstado](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L272) |
| Procesos | [usp_CancelarVenta](02_SQL/08_StoredProcedures.sql#L638) | | ✅ | [VentaRepository.Cancelar](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L204) |
| Administrativo | [usp_AjustarStock](02_SQL/08_StoredProcedures.sql#L686) | | ✅ | [ProductoRepository.AjustarStock](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L108) |
| Administrativo | [usp_AjustarPrecio](02_SQL/08_StoredProcedures.sql#L726) | ✅ | ✅ | [FrmAjusteStock](03_WindowsForms/TecnoStore/Forms/FrmAjusteStock.cs#L49) |
| Reportes | [usp_ResumenPeriodo](02_SQL/08_StoredProcedures.sql#L764) | ✅ | | [ReporteRepository](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L282) |
| Reportes | [usp_ActividadEmpleados](02_SQL/08_StoredProcedures.sql#L784) | ✅ | | [ReporteRepository](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L288) |

## 11 · Functions — [09_Functions.sql](02_SQL/09_Functions.sql)

| Función | Tipo | Se usa en |
|---|---|---|
| [fn_TotalCompradoCliente](02_SQL/09_Functions.sql#L9) | escalar | pantalla de clientes ([consulta](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L121)) |
| [fn_ClasificacionCliente](02_SQL/09_Functions.sql#L19) | escalar | pantalla de clientes |
| [fn_MargenProducto](02_SQL/09_Functions.sql#L32) | escalar | [usp_ProductoBuscar](02_SQL/08_StoredProcedures.sql#L72) |
| [fn_UsuarioTieneRol](02_SQL/09_Functions.sql#L43) | escalar | descuentos y ajustes en los SP |
| [fn_TecnicoHabilitado](02_SQL/09_Functions.sql#L57) | escalar | [usp_OrdenAgregarLinea](02_SQL/08_StoredProcedures.sql#L544) |
| [fn_TotalOrden](02_SQL/09_Functions.sql#L70) | escalar | SP de órdenes y [pantalla de órdenes](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L214) |
| [fn_VentasPorCategoria](02_SQL/09_Functions.sql#L79) | de tabla | [reporte por categoría](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L294) |

## 12 · Triggers — [10_Triggers.sql](02_SQL/10_Triggers.sql)

| Trigger | Tipo | Qué hace |
|---|---|---|
| [trg_Producto_Auditoria](02_SQL/10_Triggers.sql#L15) | Auditoría | Registra cambios de precio y stock |
| [trg_Venta_Auditoria](02_SQL/10_Triggers.sql#L41) | Auditoría | Registra cancelaciones de ventas |
| [trg_OrdenServicio_Estado](02_SQL/10_Triggers.sql#L60) | Regla / automatización | Bloquea órdenes cerradas y llena el historial de estados |
| [trg_Auditoria_SoloLectura](02_SQL/10_Triggers.sql#L86) | Libre | La auditoría no se edita ni se borra |
| [trg_Producto_BajaLogica](02_SQL/10_Triggers.sql#L98) | Libre | DELETE de producto con historial = baja lógica |

Todos funcionan con varias filas (`inserted`/`deleted` como conjuntos).

## 13 · Aplicación Windows Forms — [03_WindowsForms](03_WindowsForms)

| # | Módulo | Formulario | |
|---|---|---|---|
| 1 | Acceso / sesión | [FrmLogin](03_WindowsForms/TecnoStore/Forms/FrmLogin.cs) | ✅ |
| 2 | Panel principal por perfil (barra lateral) y Dashboard | [FrmPrincipal](03_WindowsForms/TecnoStore/Forms/FrmPrincipal.cs), [FrmDashboard](03_WindowsForms/TecnoStore/Forms/FrmDashboard.cs) | ✅ |
| 3–5 | Catálogos: Categorías, Marcas, Tipos de servicio (CRUD con INSERT/UPDATE/DELETE parametrizados) | [FrmCatalogo](03_WindowsForms/TecnoStore/Forms/FrmCatalogo.cs) · [CatalogoCrudRepository](03_WindowsForms/TecnoStore/Data/RepositoriosAdmin.cs#L18) | ✅ |
| 6 | Clientes CRUD + historial | [FrmClientes](03_WindowsForms/TecnoStore/Forms/FrmClientes.cs) | ✅ |
| 7 | Proveedores (CRUD + productos que surte) | [FrmProveedores](03_WindowsForms/TecnoStore/Forms/FrmProveedores.cs) | ✅ |
| 8 | Productos CRUD + filtros | [FrmProductos](03_WindowsForms/TecnoStore/Forms/FrmProductos.cs) | ✅ |
| 9 | Proceso principal (venta) | [FrmVenta](03_WindowsForms/TecnoStore/Forms/FrmVenta.cs) | ✅ |
| 10 | Proceso secundario (órdenes de taller) | [FrmOrdenes](03_WindowsForms/TecnoStore/Forms/FrmOrdenes.cs) | ✅ |
| 11 | Cancelaciones / ajustes | [FrmHistorialVentas](03_WindowsForms/TecnoStore/Forms/FrmHistorialVentas.cs#L59), [FrmAjusteStock](03_WindowsForms/TecnoStore/Forms/FrmAjusteStock.cs) | ✅ |
| 12 | Historial con filtros | [FrmHistorialVentas](03_WindowsForms/TecnoStore/Forms/FrmHistorialVentas.cs) | ✅ |
| 13 | Reportes (5) | [FrmReportes](03_WindowsForms/TecnoStore/Forms/FrmReportes.cs) | ✅ |
| 14 | Administración: usuarios y roles, auditoría | [FrmUsuarios](03_WindowsForms/TecnoStore/Forms/FrmUsuarios.cs), [FrmAuditoria](03_WindowsForms/TecnoStore/Forms/FrmAuditoria.cs) | ✅ |
| — | Compras a proveedor (tercer maestro-detalle) | [FrmCompra](03_WindowsForms/TecnoStore/Forms/FrmCompra.cs) | ✅ |

**13.3 · Acceso a SQL Server desde C#**

| Requisito | Dónde |
|---|---|
| Cadena de conexión centralizada | [App.config](03_WindowsForms/TecnoStore/App.config) · [Db.CadenaConexion](03_WindowsForms/TecnoStore/Data/Db.cs#L14) |
| SqlConnection + `using` | [Db.Consulta](03_WindowsForms/TecnoStore/Data/Db.cs#L57) |
| Siempre con parámetros | [Db.P](03_WindowsForms/TecnoStore/Data/Db.cs#L106) |
| Capa de datos separada | [Data/Repositorios.cs](03_WindowsForms/TecnoStore/Data/Repositorios.cs) |
| Organización Forms / Models / Data / Utils | [Forms](03_WindowsForms/TecnoStore/Forms) · [Models](03_WindowsForms/TecnoStore/Models) · [Data](03_WindowsForms/TecnoStore/Data) · [Utils](03_WindowsForms/TecnoStore/Utils) |
| ≥ 4 SP consumidos | 11 (ver tabla de la sección 10) |
| ≥ 3 vistas consumidas | vw_ProductosActivos, vw_VentaCompleta, vw_OrdenServicioCompleta, vw_StockBajo, vw_TopProductos |
| ≥ 1 función en consulta | [fn_TotalCompradoCliente en ClienteRepository](03_WindowsForms/TecnoStore/Data/Repositorios.cs#L121) |
| Evitar doble ejecución | [btnGuardar deshabilitado](03_WindowsForms/TecnoStore/Forms/FrmVenta.cs#L94) |

## 14 · Reglas de negocio

| # | Regla | Capa | Dónde |
|---|---|---|---|
| 1 | No hay cantidades en cero o negativas | CHECK + SP + WinForms | [CK_DetalleVenta_cantidad](02_SQL/02_CrearTablas.sql#L187), [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) |
| 2 | No se vende sin stock | SP | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) |
| 3 | Productos con serie se venden pieza por pieza | SP | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) |
| 4 | Sin folios, SKU ni correos duplicados | UNIQUE + SP | [02_CrearTablas.sql](02_SQL/02_CrearTablas.sql), [usp_ClienteGuardar](02_SQL/08_StoredProcedures.sql#L140) |
| 5 | No se borra información histórica | Trigger | [trg_Producto_BajaLogica](02_SQL/10_Triggers.sql#L98) |
| 6 | Estados de orden válidos, sin saltos | SP + Trigger | [usp_CambiarEstadoOrden](02_SQL/08_StoredProcedures.sql#L602), [trg_OrdenServicio_Estado](02_SQL/10_Triggers.sql#L60) |
| 7 | Fechas lógicas (entrega ≥ recepción, etc.) | CHECK | [OrdenServicio](02_SQL/02_CrearTablas.sql#L208), [Garantia](02_SQL/02_CrearTablas.sql#L197) |
| 8 | Descuentos > 10 % requieren Supervisor | SP + función | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285), [fn_UsuarioTieneRol](02_SQL/09_Functions.sql#L43) |
| 9 | Solo se cancelan ventas de los últimos 30 días | SP | [usp_CancelarVenta](02_SQL/08_StoredProcedures.sql#L638) |
| 10 | Cambios de precio y stock auditados | Trigger | [trg_Producto_Auditoria](02_SQL/10_Triggers.sql#L15) |
| 11 | Precio nunca menor al costo | CHECK | [CK_Producto_precio_vs_costo](02_SQL/03_Alter_Constraints.sql#L36) |
| 12 | Descuento nunca mayor al subtotal | CHECK | [CK_Venta_descuento](02_SQL/03_Alter_Constraints.sql#L70) |
| 13 | No se opera con clientes ni productos inactivos | SP | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) |
| 14 | Totales siempre calculados por la base | SP + función | [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285), [fn_TotalOrden](02_SQL/09_Functions.sql#L70) |
| 15 | Encabezado y detalle se guardan juntos o nada | Transacción | [11_Transacciones.sql](02_SQL/11_Transacciones.sql) |
| 16 | Permisos según rol | Roles SQL + WinForms | [07_Seguridad.sql](02_SQL/07_Seguridad.sql), [FrmPrincipal](03_WindowsForms/TecnoStore/Forms/FrmPrincipal.cs) |
| 17 | Ajustes de stock ±20 piezas, con motivo | SP | [usp_AjustarStock](02_SQL/08_StoredProcedures.sql#L686) |
| 18 | Cambios de precio máximo ±30 % | SP | [usp_AjustarPrecio](02_SQL/08_StoredProcedures.sql#L726) |
| 19 | Técnico solo hace servicios para los que está habilitado | SP + función (N:M) | [fn_TecnicoHabilitado](02_SQL/09_Functions.sql#L57) |
| 20 | Usuario bloqueado tras 5 intentos fallidos | SP | [usp_Login](02_SQL/08_StoredProcedures.sql#L34) |
| 21 | Contraseñas de 8+ caracteres con un número; solo se guarda el hash | SP | [usp_UsuarioGuardar](02_SQL/08_StoredProcedures.sql#L220) |
| 22 | En una compra, cada pieza con serie trae su número y no se repite | SP + WinForms | [usp_RegistrarCompra](02_SQL/08_StoredProcedures.sql#L404), [FrmCompra](03_WindowsForms/TecnoStore/Forms/FrmCompra.cs) |

## 15 · Reportes — [FrmReportes](03_WindowsForms/TecnoStore/Forms/FrmReportes.cs)

| Reporte | Fuente | Filtro de fechas |
|---|---|---|
| Resumen por periodo | [usp_ResumenPeriodo](02_SQL/08_StoredProcedures.sql#L764) (SP) | ✅ |
| Top 10 productos | [vw_TopProductos](02_SQL/06_Vistas.sql#L75) (vista) | |
| Actividad por empleado | [usp_ActividadEmpleados](02_SQL/08_StoredProcedures.sql#L784) | ✅ |
| Stock bajo / alertas | [vw_StockBajo](02_SQL/06_Vistas.sql#L23) (LEFT JOIN) | |
| Ventas por categoría (comparativo) | [fn_VentasPorCategoria](02_SQL/09_Functions.sql#L79) | ✅ |

Exportación a CSV: [btnExportar](03_WindowsForms/TecnoStore/Forms/FrmReportes.cs#L51) (reto opcional).

## 16 · Auditoría

| Requisito | Dónde |
|---|---|
| Tabla central con fecha, usuario, operación, id, valor anterior/nuevo | [Auditoria](02_SQL/02_CrearTablas.sql#L246) + [usuario_app y motivo](02_SQL/03_Alter_Constraints.sql#L85) |
| 2 tipos de cambio sensibles | [precio/stock](02_SQL/10_Triggers.sql#L15), [cancelaciones](02_SQL/10_Triggers.sql#L41) |
| Pantalla solo para perfiles autorizados | [FrmAuditoria](03_WindowsForms/TecnoStore/Forms/FrmAuditoria.cs) |
| No se edita | [trg_Auditoria_SoloLectura](02_SQL/10_Triggers.sql#L86) + [DENY](02_SQL/07_Seguridad.sql#L92) |

## 17 · Casos de prueba — [12_Pruebas.sql](02_SQL/12_Pruebas.sql)

| Tipo | Mínimo | Casos |
|---|---|---|
| Operaciones correctas | 8 | [P01–P10](02_SQL/12_Pruebas.sql#L25) |
| Validaciones | 5 | [P11–P17](02_SQL/12_Pruebas.sql#L127) |
| Transacciones fallidas | 3 | [P18–P20](02_SQL/12_Pruebas.sql#L182) |
| Seguridad | 2 | [P21–P22](02_SQL/12_Pruebas.sql#L213) |
| Triggers / auditoría | 2 | [P23–P26](02_SQL/12_Pruebas.sql#L237) |
| Catálogos, proveedores y usuarios | — | [P27–P32](02_SQL/12_Pruebas.sql#L271) |

Al ejecutarlo muestra la tabla con entrada, esperado, obtenido y PASA/FALLA. Pasan 32 de 32. Detalle en [CasosPrueba.pdf](01_Documentacion/CasosPrueba.pdf) y salida real en [12_Pruebas.txt](04_Evidencias/salidas_sql/12_Pruebas.txt).

## 20 · Entregables finales

| Carpeta / archivo | |
|---|---|
| `01_Documentacion/`: [DocumentoTecnico.pdf](01_Documentacion/DocumentoTecnico.pdf), [ManualUsuario.pdf](01_Documentacion/ManualUsuario.pdf), [DER.png](01_Documentacion/DER.png), [DiccionarioDatos.pdf](01_Documentacion/DiccionarioDatos.pdf), [MatrizPermisos.pdf](01_Documentacion/MatrizPermisos.pdf), [CasosPrueba.pdf](01_Documentacion/CasosPrueba.pdf) | ✅ *falta llenar portada (equipo e integrantes)* |
| `02_SQL/01` a `12` | ✅ [02_SQL](02_SQL) |
| `03_WindowsForms/SolucionVisualStudio` | ✅ [03_WindowsForms](03_WindowsForms) |
| `04_Evidencias/`: [salidas de SQL](04_Evidencias/salidas_sql) ✅ · [capturas de la vista previa](04_Evidencias/capturas_vista_previa) ✅ · [capturas de la app en Windows](04_Evidencias/capturas) | 🟡 *tomarlas en Windows* |
| `README.txt` | ✅ [README.md](README.md) |

## 24 · Guion de la demostración

| Paso | Qué mostrar |
|---|---|
| Constraint desde SQL Server | [P13: precio menor al costo](02_SQL/12_Pruebas.sql#L127) |
| Relación N:M | [EmpleadoTipoServicio](02_SQL/03_Alter_Constraints.sql#L100) |
| Proceso maestro-detalle | Nueva venta en [FrmVenta](03_WindowsForms/TecnoStore/Forms/FrmVenta.cs) |
| Error y ROLLBACK | Vender más piezas de las que hay → [usp_RegistrarVenta](02_SQL/08_StoredProcedures.sql#L285) |
| SP desde la app | Cualquier guardado (ver sección 10) |
| Function en consulta | Columna "Nivel" en Clientes → [fn_ClasificacionCliente](02_SQL/09_Functions.sql#L19) |
| Trigger | Cambiar precio y abrir Auditoría → [trg_Producto_Auditoria](02_SQL/10_Triggers.sql#L15) |
| Cambio de rol y restricción | Entrar como `fconsulta` o `loperador` → [07_Seguridad.sql](02_SQL/07_Seguridad.sql#L144) |
| Pantalla de auditoría | [FrmAuditoria](03_WindowsForms/TecnoStore/Forms/FrmAuditoria.cs) |
| 5 reportes | [FrmReportes](03_WindowsForms/TecnoStore/Forms/FrmReportes.cs) |

---

## Lo que falta

1. **Portada** del documento técnico: nombre del equipo, integrantes y profesor (en [scripts/doc_tecnico.py](scripts/doc_tecnico.py), buscar "(completar)", y volver a generar).
2. **Capturas de la app en Windows** en `04_Evidencias/capturas/`: las de `capturas_vista_previa/` son del diseño en Mac; el profesor pide evidencia de Windows Forms.
3. Probar la solución de Visual Studio en Windows (compila sin errores, pero no se ha ejecutado en Windows).
4. La presentación de la idea (TechStore) en `Contexto/`.

## Cómo regenerar la documentación (Mac)

```bash
docker compose up -d
node preview/server.js &
python3 scripts/generar_documentacion.py
```

Crea la base desde cero, lee tablas, columnas y permisos de SQL Server, ejecuta las consultas, transacciones y los 32 casos de prueba, toma las capturas y genera los PDF.
