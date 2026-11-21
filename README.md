# TecnoStore

Proyecto final de Taller de Bases de Datos: sistema de gestión para una tienda de tecnología con taller de reparación.
Tecnología obligatoria: **SQL Server + C# Windows Forms + Visual Studio** (ver `Contexto/ProyectoFinal8Semanas (1).docx`).

> 📋 **[REQUISITOS.md](REQUISITOS.md)**: cada requisito del profesor con enlace directo al archivo y línea donde se cumple.

## Estructura (igual a la entrega final)

| Carpeta | Contenido |
|---|---|
| `Contexto/` | Requerimientos del profesor y modelo de datos / diccionario |
| `01_Documentacion/` | [Documento técnico](01_Documentacion/DocumentoTecnico.pdf), [manual de usuario](01_Documentacion/ManualUsuario.pdf), [DER](01_Documentacion/DER.png), [diccionario de datos](01_Documentacion/DiccionarioDatos.pdf), [matriz de permisos](01_Documentacion/MatrizPermisos.pdf) y [casos de prueba](01_Documentacion/CasosPrueba.pdf) y [guía de diseño](01_Documentacion/GuiaDiseno.md) (investigación de patrones de Apple y cómo se aplicaron) |
| `02_SQL/` | Scripts de la base (01 a 12) |
| `03_WindowsForms/` | **La aplicación**: solución de Visual Studio `TecnoStore.sln` (C# Windows Forms, .NET Framework 4.8) |
| `04_Evidencias/` | Salida real de los scripts en SQL Server, capturas de la vista previa y carpeta para las capturas de la app en Windows |
| `preview/` | Solo para ver las pantallas en Mac. **No es parte de la entrega** y en Windows no hace falta (ni Node.js) |
| `docker-compose.yml`, `scripts/` | SQL Server en Docker para quien trabaja en Mac |

## En Windows: paso a paso

### 1. Crear la base en SQL Server Management Studio

Abrir cada archivo de `02_SQL/` y ejecutarlo (F5) **en este orden**:

1. `01_CrearBase.sql` (borra y recrea la base `TecnoStore`)
2. `02_CrearTablas.sql`
3. `03_Alter_Constraints.sql`
4. `04_DatosDemo.sql`
5. `06_Vistas.sql`
6. `08_StoredProcedures.sql`
7. `09_Functions.sql`
8. `10_Triggers.sql`
9. `07_Seguridad.sql` (va al final porque da permisos sobre los procedimientos; requiere modo de autenticación mixto, ver abajo)

Opcionales, para demostrar: `05_Consultas.sql` (40 consultas), `11_Transacciones.sql` (los 4 procesos con éxito y ROLLBACK) y `12_Pruebas.sql` (34 casos automáticos; ejecútenlo sobre una base recién creada y al final muestra cuántos pasan).

### 2. Abrir y ejecutar la aplicación

**Sin Visual Studio:** descarguen `TecnoStore-vX.Y-win.zip` del [último release](https://github.com/Alan20111/TecnoStore/releases/latest), descompriman y abran `TecnoStore.exe`. En la pantalla de inicio de sesión escriban su servidor (`localhost`, `.\SQLEXPRESS` o `NOMBRE-PC\INSTANCIA`); se recuerda para la próxima vez. Si en ese servidor no existe la base `TecnoStore`, la app ofrece crearla con los datos demo (los scripts de `02_SQL` vienen dentro del `.exe`), así que no es obligatorio usar SSMS. Cada cambio al proyecto publica un release nuevo.

**Con Visual Studio:**


1. Abrir `03_WindowsForms/TecnoStore.sln` con **Visual Studio 2022** (o 2019) que tenga la carga de trabajo **"Desarrollo de escritorio de .NET"**.
2. Al abrir la app, escribir su servidor en el inicio de sesión (`localhost`, `.\SQLEXPRESS`…). Usa autenticación de Windows. `App.config` solo define el servidor por defecto y el modo de conexión por roles.
3. **F5**. Entrar con un usuario demo (contraseña de todos: `1234`):

| Usuario | Rol | Qué puede hacer |
|---|---|---|
| `admin` | Administrador | Todo |
| `csupervisor` | Supervisor | Todo menos administrar la base; cancela ventas, ajusta stock y precios, ve auditoría |
| `loperador` | Operador | Ventas, clientes, órdenes de taller; no ve reportes ni auditoría |
| `fconsulta` | Consulta | Solo lectura |

### 3. (Para la demo de seguridad) conectar cada perfil con su login de SQL Server

`07_Seguridad.sql` crea los logins `tecno_admin`, `tecno_supervisor`, `tecno_operador`, `tecno_consulta` y `tecno_login` (contraseña `Tecno#2026sql`).
Para que la app los use, en `App.config` cambien `ModoConexion` de `Unica` a `Roles` (y `Server=` en esas cadenas si no es `localhost`).
Así, aunque alguien quitara una validación de la app, SQL Server sigue negando lo que el rol no tiene.
Requiere habilitar en SSMS: clic derecho en el servidor → *Propiedades* → *Seguridad* → **Modo de autenticación de SQL Server y Windows**, y reiniciar el servicio.

## Módulos de la aplicación

| # | Módulo | Formulario | Usa |
|---|---|---|---|
| 1 | Acceso / sesión | `FrmLogin` | `usp_Login` (hash, bloqueo tras 5 intentos) |
| 2 | Panel principal por perfil (barra lateral) y Dashboard | `FrmPrincipal`, `FrmDashboard` | Indicadores de 90 días, ingresos por mes, ventas por categoría, alertas y taller (vistas y funciones existentes) |
| 3–5 | Catálogos: categorías, marcas, tipos de servicio | `FrmCatalogo` | INSERT/UPDATE/DELETE parametrizados; DELETE controlado por FK |
| 6 | Clientes (CRUD + historial) | `FrmClientes` | `usp_ClienteGuardar`, `fn_TotalCompradoCliente`, `fn_ClasificacionCliente`, `vw_VentaCompleta` |
| 7 | Proveedores (CRUD + productos que surte) | `FrmProveedores` | `usp_ProveedorGuardar`, `ProductoProveedor` |
| 8 | Productos (CRUD + filtros + existencias) | `FrmProductos` | `usp_ProductoBuscar`, `usp_ProductoGuardar`, trigger de baja lógica |
| 9 | Proceso principal: venta (maestro-detalle) | `FrmVenta` | `usp_RegistrarVenta` |
| 10 | Proceso secundario: órdenes de taller (maestro-detalle) | `FrmOrdenes` | `usp_RegistrarOrdenServicio`, `usp_OrdenAgregarLinea`, `usp_CambiarEstadoOrden`, `fn_TotalOrden` |
| — | Compras a proveedor (tercer maestro-detalle) | `FrmCompra` | `usp_RegistrarCompra` |
| 11–12 | Cancelaciones e historial con filtros | `FrmHistorialVentas` | `vw_VentaCompleta`, `usp_CancelarVenta` |
| 13 | Reportes (5) + exportar CSV | `FrmReportes` | `usp_ResumenPeriodo`, `vw_TopProductos`, `usp_ActividadEmpleados`, `vw_StockBajo`, `fn_VentasPorCategoria` |
| 14 | Administración: usuarios, ajustes y auditoría | `FrmUsuarios`, `FrmAjusteStock`, `FrmAuditoria` | `usp_UsuarioGuardar`, `usp_AjustarStock`, `usp_AjustarPrecio`, tabla `Auditoria` |

Código organizado como pide el documento: `Forms/`, `Models/`, `Data/` (conexión centralizada y repositorios, todo con parámetros), `Utils/`.

Los 14 módulos que pide el documento están completos. Detalle de cada pantalla en el [manual de usuario](01_Documentacion/ManualUsuario.pdf).

## Avance contra los mínimos (SQL)

| Componente | Hecho | Mínimo |
|---|---|---|
| Tablas / FK | 28 / 38 | 20 / 18 |
| CHECK / UNIQUE / DEFAULT | 32 / 20 / 27 | 8 / 4 / 6 |
| ALTER TABLE documentados | 12 | 8 |
| Relaciones N:M | 3 | 3 |
| Consultas (05_Consultas) | 40 | 12+ |
| Vistas | 6 | 6 |
| Stored Procedures | 16 | 8 |
| Functions | 7 | 4 |
| Triggers | 5 | 4 |
| Roles SQL Server | 4 + login de acceso | 4 |
| Procesos transaccionales | 10 (+1 manual) | 4 |
| Casos de prueba automáticos | 32 (todos pasan) | 20 |

## En Mac

```bash
docker compose up -d
scripts/crear_base.sh
```

La app de Windows Forms no corre en macOS; compila con `dotnet build 03_WindowsForms/TecnoStore.sln -p:EnableWindowsTargeting=true`.
`preview/` (Node.js) simula las pantallas en el navegador con la base real: `cd preview && npm install && node server.js`.

Para publicar un release nuevo con el `.exe` (después de hacer commit y push):

```bash
scripts/publicar_release.sh "Qué cambió"
```

Para regenerar toda la documentación y evidencias desde la base (con la vista previa corriendo):

```bash
python3 scripts/generar_documentacion.py
```
