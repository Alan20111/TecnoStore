# TecnoStore

Proyecto final de Taller de Bases de Datos: sistema de gestión para una tienda de tecnología con taller de reparación.
Tecnología obligatoria: **SQL Server + C# Windows Forms** (ver `Contexto/ProyectoFinal8Semanas (1).docx`).

## Estructura

| Carpeta | Contenido |
|---|---|
| `Contexto/` | Requerimientos del profesor y modelo de datos / diccionario |
| `02_SQL/` | Scripts de la base, numerados como pide la entrega final |
| `preview/` | Vista previa de las pantallas (estilo Windows Forms) conectada a la base real. Es referencia de diseño, **no** es la app final |
| `docker-compose.yml` | SQL Server en Docker para quien trabaja en Mac |

## Crear la base en Windows (SSMS)

Abrir y ejecutar en SQL Server Management Studio, **en este orden**:

1. `02_SQL/01_CrearBase.sql` (borra y recrea la base `TecnoStore`)
2. `02_SQL/02_CrearTablas.sql`
3. `02_SQL/04_DatosDemo.sql`
4. `02_SQL/06_Vistas.sql`
5. `02_SQL/08_StoredProcedures.sql`
6. `02_SQL/09_Functions.sql`
7. `02_SQL/10_Triggers.sql`

Usuarios demo de la aplicación (contraseña de todos: `Tecno123!`):

| Usuario | Rol |
|---|---|
| `admin` | Administrador |
| `csupervisor` | Supervisor |
| `loperador` | Operador |
| `fconsulta` | Consulta |

## Ver la vista previa (opcional)

Requiere [Node.js](https://nodejs.org) y un login de SQL Server con usuario y contraseña
(autenticación SQL, no de Windows; en SSMS: *Security → Logins*, y habilitar el modo mixto en la instancia).

En PowerShell, desde la carpeta del repo:

```powershell
cd preview
npm install
$env:DB_USER="sa"; $env:DB_PASSWORD="TU_CONTRASEÑA"
node server.js
```

Variables opcionales: `DB_SERVER` (por defecto `localhost`; para SQLEXPRESS suele ser `localhost` con el puerto TCP habilitado) y `DB_PORT` (1433).
Luego abrir <http://localhost:5173>.

## En Mac

```bash
docker compose up -d
for f in 01_CrearBase 02_CrearTablas 04_DatosDemo 06_Vistas 08_StoredProcedures 09_Functions 10_Triggers; do scripts/sql.sh -i /scripts/$f.sql; done
cd preview && npm install && node server.js
```

## Avance contra los mínimos

| Componente | Hecho | Mínimo |
|---|---|---|
| Tablas | 27 | 20 |
| Llaves foráneas | 35 | 18 |
| CHECK / UNIQUE / DEFAULT | 29 / 19 / 21 | 8 / 4 / 6 |
| Vistas | 6 | 6 |
| Stored Procedures | 4 | 8 |
| Functions | 4 | 4 |
| Triggers | 4 | 4 |
| Datos demo | 45 ventas, 122 detalles, 24 órdenes | 40 / 100 |

Pendiente: `03_Alter_Constraints.sql`, `05_Consultas.sql`, `07_Seguridad.sql` (roles de SQL Server), `11_Transacciones.sql`, `12_Pruebas.sql`, 4 SP más y la solución de Windows Forms en C#.
