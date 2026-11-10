/* =============================================================
   TecnoStore · 07_Seguridad.sql
   Roles de base de datos, usuarios de prueba y permisos.
   EJECUTAR DESPUÉS DE 10_Triggers.sql: los GRANT sobre procedimientos
   y funciones necesitan que ya existan.

   Logins (autenticación SQL Server; requiere modo mixto):
     tecno_login       solo puede ejecutar usp_Login
     tecno_admin       -> rol_administrador
     tecno_supervisor  -> rol_supervisor
     tecno_operador    -> rol_operador
     tecno_consulta    -> rol_consulta
   Contraseña de todos (solo desarrollo): Tecno#2026sql

   Matriz de permisos
   Objeto/Módulo          Administrador  Supervisor        Operador            Consulta
   Catálogos              CRUD           R                 R                   R
   Clientes/Productos     CRUD           R + guardar (SP)  R + clientes (SP)   R
   Venta / Orden          CRUD           Ejecuta + cancela Ejecuta (SP)        R
   Compra / Ajustes       CRUD           Ejecuta (SP)      -                   -
   Reportes               R              R                 R limitado          R
   Seguridad (usuarios)   CRUD           -                 -                   -
   Auditoría              R              R                 -                   -
   Nadie tiene db_owner; nadie modifica la auditoría.
   ============================================================= */
USE master;
GO
DECLARE @logins TABLE (nombre SYSNAME);
INSERT INTO @logins VALUES ('tecno_login'),('tecno_admin'),('tecno_supervisor'),('tecno_operador'),('tecno_consulta');
DECLARE @n SYSNAME, @sql NVARCHAR(400);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT nombre FROM @logins;
OPEN c; FETCH NEXT FROM c INTO @n;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF SUSER_ID(@n) IS NULL
    BEGIN
        SET @sql = N'CREATE LOGIN ' + QUOTENAME(@n) + N' WITH PASSWORD = ''Tecno#2026sql'', CHECK_POLICY = OFF, DEFAULT_DATABASE = TecnoStore;';
        EXEC sp_executesql @sql;
    END
    FETCH NEXT FROM c INTO @n;
END
CLOSE c; DEALLOCATE c;
GO

USE TecnoStore;
GO

/* ---------- Usuarios de base de datos ---------- */
CREATE USER tecno_login      FOR LOGIN tecno_login;
CREATE USER tecno_admin      FOR LOGIN tecno_admin;
CREATE USER tecno_supervisor FOR LOGIN tecno_supervisor;
CREATE USER tecno_operador   FOR LOGIN tecno_operador;
CREATE USER tecno_consulta   FOR LOGIN tecno_consulta;
GO

/* ---------- Roles ---------- */
CREATE ROLE rol_administrador;
CREATE ROLE rol_supervisor;
CREATE ROLE rol_operador;
CREATE ROLE rol_consulta;
GO
ALTER ROLE rol_administrador ADD MEMBER tecno_admin;
ALTER ROLE rol_supervisor    ADD MEMBER tecno_supervisor;
ALTER ROLE rol_operador      ADD MEMBER tecno_operador;
ALTER ROLE rol_consulta      ADD MEMBER tecno_consulta;
GO

/* ---------- Pantalla de login ---------- */
GRANT EXECUTE ON dbo.usp_Login TO tecno_login;
GO

/* ---------- Lectura general ----------
   Todos leen el esquema dbo; después se quitan con DENY las tablas
   sensibles. DENY gana sobre cualquier GRANT. */
GRANT SELECT ON SCHEMA::dbo TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GRANT EXECUTE ON dbo.fn_TotalCompradoCliente  TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GRANT EXECUTE ON dbo.fn_ClasificacionCliente  TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GRANT EXECUTE ON dbo.fn_MargenProducto        TO rol_administrador, rol_supervisor, rol_consulta;
GRANT EXECUTE ON dbo.fn_TotalOrden            TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GRANT EXECUTE ON dbo.usp_ProductoBuscar       TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GRANT EXECUTE ON dbo.usp_ResumenPeriodo       TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GO

/* ---------- Seguridad: solo Administrador ---------- */
DENY SELECT ON dbo.Usuario    TO rol_supervisor, rol_operador, rol_consulta;
DENY SELECT ON dbo.UsuarioRol TO rol_supervisor, rol_operador, rol_consulta;
DENY SELECT ON dbo.Rol        TO rol_operador, rol_consulta;
GO

/* ---------- Auditoría: lectura para Administrador y Supervisor ---------- */
DENY SELECT ON dbo.Auditoria TO rol_operador, rol_consulta;
DENY INSERT, UPDATE, DELETE ON dbo.Auditoria TO rol_administrador, rol_supervisor, rol_operador, rol_consulta;
GO

/* ---------- Reportes: el operador ve un reporte limitado ----------
   No ve utilidad ni márgenes (vw_TopProductos, fn_MargenProducto) ni
   el desempeño de sus compañeros (usp_ActividadEmpleados). */
DENY SELECT ON dbo.vw_TopProductos TO rol_operador;
GRANT EXECUTE ON dbo.usp_ActividadEmpleados TO rol_administrador, rol_supervisor, rol_consulta;
GO

/* ---------- Operador: ejecuta procesos, no toca catálogos ---------- */
GRANT EXECUTE ON dbo.usp_RegistrarVenta         TO rol_operador, rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_ClienteGuardar         TO rol_operador, rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_RegistrarOrdenServicio TO rol_operador, rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_OrdenAgregarLinea      TO rol_operador, rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_CambiarEstadoOrden     TO rol_operador, rol_supervisor, rol_administrador;
DENY INSERT, UPDATE, DELETE ON dbo.Categoria    TO rol_operador, rol_consulta;
DENY INSERT, UPDATE, DELETE ON dbo.Marca        TO rol_operador, rol_consulta;
DENY INSERT, UPDATE, DELETE ON dbo.Producto     TO rol_operador, rol_consulta;
DENY INSERT, UPDATE, DELETE ON dbo.Configuracion TO rol_operador, rol_consulta, rol_supervisor;
GO

/* ---------- Supervisor: autoriza, cancela, compra y ajusta ---------- */
GRANT EXECUTE ON dbo.usp_CancelarVenta   TO rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_RegistrarCompra TO rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_AjustarStock    TO rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_AjustarPrecio   TO rol_supervisor, rol_administrador;
GRANT EXECUTE ON dbo.usp_ProductoGuardar TO rol_supervisor, rol_administrador;
GO

/* ---------- Administrador: todo el esquema, sin db_owner ---------- */
GRANT INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::dbo TO rol_administrador;
GO

/* ---------- Demostración de REVOKE ----------
   Se le dio por error al operador lectura de la auditoría; REVOKE
   retira ese GRANT (a diferencia de DENY, no deja una negación). Como
   además existe el DENY de arriba, el operador sigue sin acceso. */
GRANT  EXECUTE ON dbo.usp_AjustarStock TO rol_operador;
REVOKE EXECUTE ON dbo.usp_AjustarStock FROM rol_operador;
GO

/* =============================================================
   Demostraciones con EXECUTE AS USER / REVERT
   (cada bloque imprime el resultado; los errores son esperados)
   ============================================================= */
PRINT '--- Consulta: puede leer productos';
EXECUTE AS USER = 'tecno_consulta';
SELECT COUNT(*) AS productos_visibles FROM dbo.vw_ProductosActivos;
REVERT;
GO

PRINT '--- Consulta: NO puede registrar una venta';
EXECUTE AS USER = 'tecno_consulta';
BEGIN TRY
    EXEC dbo.usp_RegistrarVenta 1, 3, 1, N'[{"id_producto":22,"cantidad":1}]';
END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
REVERT;
GO

PRINT '--- Operador: NO puede ver la auditoría ni los usuarios';
EXECUTE AS USER = 'tecno_operador';
BEGIN TRY SELECT TOP 1 * FROM dbo.Auditoria; END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
BEGIN TRY SELECT TOP 1 nombre_usuario FROM dbo.Usuario; END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
REVERT;
GO

PRINT '--- Operador: NO puede cambiar precios directamente ni con el SP';
EXECUTE AS USER = 'tecno_operador';
BEGIN TRY UPDATE dbo.Producto SET precio_venta = 1 WHERE id_producto = 1; END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
BEGIN TRY EXEC dbo.usp_AjustarPrecio 10, 3, 1; END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
REVERT;
GO

PRINT '--- Supervisor: SÍ puede leer la auditoría, NO puede borrarla';
EXECUTE AS USER = 'tecno_supervisor';
SELECT COUNT(*) AS filas_auditoria FROM dbo.Auditoria;
BEGIN TRY DELETE FROM dbo.Auditoria; END TRY BEGIN CATCH PRINT CONCAT('Esperado: ', ERROR_MESSAGE()); END CATCH
REVERT;
GO

/* Para revisar la matriz real:
SELECT pr.name AS principal, pe.permission_name, pe.state_desc, OBJECT_NAME(pe.major_id) AS objeto, pe.class_desc
FROM sys.database_permissions pe JOIN sys.database_principals pr ON pr.principal_id = pe.grantee_principal_id
WHERE pr.name LIKE 'rol_%' OR pr.name LIKE 'tecno_%'
ORDER BY pr.name, objeto, pe.permission_name;
*/
