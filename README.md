# TecnoStore

Proyecto final de Taller de Bases de Datos: sistema de gestión para una tienda de tecnología con taller de reparación.
Tecnología obligatoria: **SQL Server + C# Windows Forms + Visual Studio** (ver `Contexto/ProyectoFinal8Semanas (1).docx`).

## Estructura (igual a la entrega final)

| Carpeta | Contenido |
|---|---|
| `Contexto/` | Requerimientos del profesor y modelo de datos / diccionario |
| `02_SQL/` | Scripts de la base (01 a 12) |
| `03_WindowsForms/` | **La aplicación**: solución de Visual Studio `TecnoStore.sln` (C# Windows Forms, .NET Framework 4.8) |
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

Opcionales, para demostrar: `05_Consultas.sql` (40 consultas), `11_Transacciones.sql` (los 4 procesos con éxito y ROLLBACK) y `12_Pruebas.sql` (26 casos automáticos; ejecútenlo sobre una base recién creada y al final muestra cuántos pasan).

### 2. Abrir y ejecutar la aplicación

1. Abrir `03_WindowsForms/TecnoStore.sln` con **Visual Studio 2022** (o 2019) que tenga la carga de trabajo **"Desarrollo de escritorio de .NET"**.
2. Revisar `App.config`: en `connectionStrings`, la cadena `Unica` usa `Server=localhost` con autenticación de Windows. Si su instancia es SQL Express cambien a `Server=.\SQLEXPRESS`.
3. **F5**. Entrar con un usuario demo (contraseña de todos: `Tecno123!`):

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
| 2 | Menú principal por perfil | `FrmPrincipal` (MDI) | — |
| 6 | Clientes (CRUD + historial) | `FrmClientes` | `usp_ClienteGuardar`, `fn_TotalCompradoCliente`, `fn_ClasificacionCliente`, `vw_VentaCompleta` |
| 8 | Productos (CRUD + filtros + existencias) | `FrmProductos` | `usp_ProductoBuscar`, `usp_ProductoGuardar`, trigger de baja lógica |
| 9 | Proceso principal: venta (maestro-detalle) | `FrmVenta` | `usp_RegistrarVenta` |
| 10 | Proceso secundario: órdenes de taller (maestro-detalle) | `FrmOrdenes` | `usp_RegistrarOrdenServicio`, `usp_OrdenAgregarLinea`, `usp_CambiarEstadoOrden`, `fn_TotalOrden` |
| 11–12 | Cancelaciones e historial con filtros | `FrmHistorialVentas` | `vw_VentaCompleta`, `usp_CancelarVenta` |
| 13 | Reportes (5) + exportar CSV | `FrmReportes` | `usp_ResumenPeriodo`, `vw_TopProductos`, `usp_ActividadEmpleados`, `vw_StockBajo`, `fn_VentasPorCategoria` |
| 14 | Administración: ajustes y auditoría | `FrmAjusteStock`, `FrmAuditoria` | `usp_AjustarStock`, `usp_AjustarPrecio`, tabla `Auditoria` |

Código organizado como pide el documento: `Forms/`, `Models/`, `Data/` (conexión centralizada y repositorios, todo con parámetros), `Utils/`.

**Faltan** para llegar a 14 módulos: CRUD de 3 catálogos (p. ej. categorías, marcas, tipos de servicio), proveedores y la pantalla de compras (`usp_RegistrarCompra` ya existe).

## Avance contra los mínimos (SQL)

| Componente | Hecho | Mínimo |
|---|---|---|
| Tablas / FK | 28 / 38 | 20 / 18 |
| CHECK / UNIQUE / DEFAULT | 32 / 20 / 27 | 8 / 4 / 6 |
| ALTER TABLE documentados | 12 | 8 |
| Relaciones N:M | 3 | 3 |
| Consultas (05_Consultas) | 40 | 12+ |
| Vistas | 6 | 6 |
| Stored Procedures | 14 | 8 |
| Functions | 7 | 4 |
| Triggers | 5 | 4 |
| Roles SQL Server | 4 + login de acceso | 4 |
| Procesos transaccionales | 7 (+1 manual) | 4 |
| Casos de prueba automáticos | 26 (todos pasan) | 20 |

## En Mac

```bash
docker compose up -d
scripts/crear_base.sh
```

La app de Windows Forms no corre en macOS; compila con `dotnet build 03_WindowsForms/TecnoStore.sln -p:EnableWindowsTargeting=true`.
`preview/` (Node.js) muestra pantallas parecidas en el navegador: `cd preview && npm install && node server.js`.
