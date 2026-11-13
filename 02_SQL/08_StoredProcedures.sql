/* =============================================================
   TecnoStore · 08_StoredProcedures.sql
   16 procedimientos.

   Grupo           Procedimiento                 Transacción
   Seguridad       usp_Login                     —
   Catálogos       usp_ProductoBuscar            —   (parámetros opcionales)
                   usp_ProductoGuardar           —
                   usp_ClienteGuardar            —
                   usp_ProveedorGuardar          —
   Seguridad app   usp_UsuarioGuardar            Sí  (Usuario + UsuarioRol)
   Procesos        usp_RegistrarVenta            Sí  (proceso 1, principal)
                   usp_RegistrarCompra           Sí  (proceso 2, secundario)
                   usp_RegistrarOrdenServicio    Sí
                   usp_OrdenAgregarLinea         Sí
                   usp_CambiarEstadoOrden        Sí
                   usp_CancelarVenta             Sí  (proceso 3, cancelación)
   Administrativo  usp_AjustarStock              Sí  (proceso 4, ajuste)
                   usp_AjustarPrecio             Sí
   Reportes        usp_ResumenPeriodo            —   (parámetros opcionales)
                   usp_ActividadEmpleados        —   (parámetros opcionales)

   Quién hizo el cambio: la app ejecuta al abrir la conexión
     EXEC sp_set_session_context N'id_usuario', <id>;
   y los procedimientos y triggers lo leen con SESSION_CONTEXT.
   ============================================================= */
USE TecnoStore;
GO

/* =========================== Seguridad =========================== */

-- Valida usuario y contraseña (hash SHA2_256). Registra último acceso;
-- tras 5 intentos fallidos el usuario queda bloqueado (activo = 0).
CREATE OR ALTER PROCEDURE usp_Login
    @nombre_usuario VARCHAR(40),
    @contrasena     VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @id INT, @ok BIT, @activo BIT;
    SELECT @id = id_usuario, @activo = activo,
           @ok = CASE WHEN hash_contrasena = HASHBYTES('SHA2_256', @contrasena) THEN 1 ELSE 0 END
    FROM Usuario WHERE nombre_usuario = @nombre_usuario;

    IF @id IS NULL
        THROW 50100, 'Usuario o contraseña incorrectos.', 1;
    IF @activo = 0
        THROW 50101, 'El usuario está bloqueado. Pide a un administrador que lo reactive.', 1;
    IF @ok = 0
    BEGIN
        UPDATE Usuario SET intentos_fallidos = intentos_fallidos + 1,
                           activo = CASE WHEN intentos_fallidos + 1 >= 5 THEN 0 ELSE 1 END
        WHERE id_usuario = @id;
        THROW 50100, 'Usuario o contraseña incorrectos.', 1;
    END

    UPDATE Usuario SET ultimo_acceso = SYSDATETIME(), intentos_fallidos = 0 WHERE id_usuario = @id;

    SELECT u.id_usuario, u.id_empleado, u.nombre_usuario, CONCAT(e.nombre, ' ', e.apellidos) AS nombre,
           e.puesto, s.nombre AS sucursal, r.nombre AS rol
    FROM Usuario u
    JOIN Empleado e    ON e.id_empleado = u.id_empleado
    JOIN Sucursal s    ON s.id_sucursal = e.id_sucursal
    JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario
    JOIN Rol r         ON r.id_rol = ur.id_rol
    WHERE u.id_usuario = @id AND e.activo = 1;
END
GO

/* =========================== Catálogos =========================== */

CREATE OR ALTER PROCEDURE usp_ProductoBuscar
    @texto        VARCHAR(60) = NULL,
    @id_categoria INT         = NULL,
    @solo_alertas BIT         = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_producto, sku, nombre, id_categoria, categoria, marca, condicion,
           precio_venta, costo, stock_actual, stock_minimo, alerta_stock, requiere_serie,
           dbo.fn_MargenProducto(id_producto) AS margen_pct
    FROM vw_ProductosActivos
    WHERE (@texto IS NULL OR nombre LIKE '%' + @texto + '%' OR sku LIKE '%' + @texto + '%')
      AND (@id_categoria IS NULL OR id_categoria = @id_categoria)
      AND (@solo_alertas = 0 OR alerta_stock <> 'OK')
    ORDER BY categoria, nombre;
END
GO

-- Alta (@id_producto NULL) o edición. El stock NO se edita aquí:
-- entra por compras y se corrige con usp_AjustarStock.
CREATE OR ALTER PROCEDURE usp_ProductoGuardar
    @id_producto    INT = NULL OUTPUT,
    @sku            VARCHAR(30),
    @nombre         VARCHAR(120),
    @id_categoria   INT,
    @id_marca       INT,
    @id_condicion   INT,
    @precio_venta   DECIMAL(10,2),
    @costo          DECIMAL(10,2),
    @requiere_serie BIT = 0,
    @stock_minimo   INT = 0,
    @activo         BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET @sku = UPPER(LTRIM(RTRIM(@sku)));
    IF @sku = '' OR LTRIM(@nombre) = ''
        THROW 50110, 'El SKU y el nombre son obligatorios.', 1;
    IF @precio_venta < @costo
        THROW 50111, 'El precio de venta no puede ser menor al costo.', 1;
    IF EXISTS (SELECT 1 FROM Producto WHERE sku = @sku AND id_producto <> ISNULL(@id_producto, 0))
        THROW 50112, 'Ya existe otro producto con ese SKU.', 1;

    IF @id_producto IS NULL
    BEGIN
        INSERT INTO Producto (sku, nombre, id_categoria, id_marca, id_condicion, precio_venta, costo,
                              requiere_serie, stock_minimo, activo)
        VALUES (@sku, @nombre, @id_categoria, @id_marca, @id_condicion, @precio_venta, @costo,
                @requiere_serie, @stock_minimo, @activo);
        SET @id_producto = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        -- No se cambia requiere_serie si ya hay piezas registradas
        IF EXISTS (SELECT 1 FROM Producto p WHERE p.id_producto = @id_producto AND p.requiere_serie <> @requiere_serie)
           AND EXISTS (SELECT 1 FROM UnidadSerie WHERE id_producto = @id_producto)
            THROW 50113, 'No se puede cambiar el control por número de serie: el producto ya tiene piezas registradas.', 1;

        UPDATE Producto
           SET sku = @sku, nombre = @nombre, id_categoria = @id_categoria, id_marca = @id_marca,
               id_condicion = @id_condicion, precio_venta = @precio_venta, costo = @costo,
               requiere_serie = @requiere_serie, stock_minimo = @stock_minimo, activo = @activo
         WHERE id_producto = @id_producto;
        IF @@ROWCOUNT = 0 THROW 50114, 'El producto no existe.', 1;
    END
END
GO

CREATE OR ALTER PROCEDURE usp_ClienteGuardar
    @id_cliente INT = NULL OUTPUT,
    @nombre     VARCHAR(60),
    @apellidos  VARCHAR(80),
    @correo     VARCHAR(100),
    @telefono   VARCHAR(20) = NULL,
    @rfc        VARCHAR(13) = NULL,
    @activo     BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET @correo = LOWER(LTRIM(RTRIM(@correo)));
    IF LTRIM(@nombre) = '' OR LTRIM(@apellidos) = ''
        THROW 50120, 'Nombre y apellidos son obligatorios.', 1;
    IF @correo NOT LIKE '_%@_%._%'
        THROW 50121, 'El correo no tiene un formato válido.', 1;
    IF EXISTS (SELECT 1 FROM Cliente WHERE correo = @correo AND id_cliente <> ISNULL(@id_cliente, 0))
        THROW 50122, 'Ya existe un cliente con ese correo.', 1;
    IF @rfc IS NOT NULL AND LEN(@rfc) NOT IN (12, 13)
        THROW 50123, 'El RFC debe tener 12 o 13 caracteres.', 1;

    IF @id_cliente IS NULL
    BEGIN
        INSERT INTO Cliente (nombre, apellidos, correo, telefono, rfc, activo)
        VALUES (@nombre, @apellidos, @correo, NULLIF(@telefono, ''), UPPER(NULLIF(@rfc, '')), @activo);
        SET @id_cliente = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE Cliente SET nombre = @nombre, apellidos = @apellidos, correo = @correo,
                           telefono = NULLIF(@telefono, ''), rfc = UPPER(NULLIF(@rfc, '')), activo = @activo
        WHERE id_cliente = @id_cliente;
        IF @@ROWCOUNT = 0 THROW 50124, 'El cliente no existe.', 1;
    END
END
GO

CREATE OR ALTER PROCEDURE usp_ProveedorGuardar
    @id_proveedor INT = NULL OUTPUT,
    @razon_social VARCHAR(120),
    @rfc          VARCHAR(13),
    @correo       VARCHAR(100) = NULL,
    @telefono     VARCHAR(20)  = NULL,
    @activo       BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET @rfc = UPPER(LTRIM(RTRIM(@rfc)));
    IF LTRIM(ISNULL(@razon_social, '')) = ''
        THROW 50130, 'La razón social es obligatoria.', 1;
    IF LEN(@rfc) NOT IN (12, 13)
        THROW 50131, 'El RFC debe tener 12 (moral) o 13 (física) caracteres.', 1;
    IF NULLIF(@correo, '') IS NOT NULL AND @correo NOT LIKE '_%@_%._%'
        THROW 50132, 'El correo no tiene un formato válido.', 1;
    IF EXISTS (SELECT 1 FROM Proveedor WHERE rfc = @rfc AND id_proveedor <> ISNULL(@id_proveedor, 0))
        THROW 50133, 'Ya existe un proveedor con ese RFC.', 1;

    IF @id_proveedor IS NULL
    BEGIN
        INSERT INTO Proveedor (razon_social, rfc, correo, telefono, activo)
        VALUES (@razon_social, @rfc, NULLIF(LOWER(@correo), ''), NULLIF(@telefono, ''), @activo);
        SET @id_proveedor = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE Proveedor SET razon_social = @razon_social, rfc = @rfc, correo = NULLIF(LOWER(@correo), ''),
                             telefono = NULLIF(@telefono, ''), activo = @activo
        WHERE id_proveedor = @id_proveedor;
        IF @@ROWCOUNT = 0 THROW 50134, 'El proveedor no existe.', 1;
    END
END
GO

/* ======================== Seguridad de la app ======================== */

/* Alta o edición de un usuario de la aplicación y su rol (Usuario + UsuarioRol
   en una transacción). Solo un Administrador. La contraseña se guarda como
   hash SHA2_256; en edición, NULL conserva la actual. Reactivar un usuario
   reinicia sus intentos fallidos. Un administrador no puede desactivarse ni
   quitarse el rol a sí mismo. */
CREATE OR ALTER PROCEDURE usp_UsuarioGuardar
    @id_usuario     INT = NULL OUTPUT,
    @id_empleado    INT,
    @nombre_usuario VARCHAR(40),
    @id_rol         INT,
    @contrasena     VARCHAR(100) = NULL,
    @activo         BIT = 1,
    @id_admin       INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        SET @nombre_usuario = LOWER(LTRIM(RTRIM(@nombre_usuario)));
        IF dbo.fn_UsuarioTieneRol(@id_admin, 'Administrador') = 0
            THROW 50140, 'Solo un Administrador puede administrar usuarios.', 1;
        IF LEN(@nombre_usuario) < 4 OR @nombre_usuario LIKE '% %'
            THROW 50141, 'El usuario debe tener al menos 4 caracteres y no llevar espacios.', 1;
        IF @id_usuario IS NULL AND @contrasena IS NULL
            THROW 50142, 'Un usuario nuevo necesita contraseña.', 1;
        IF @contrasena IS NOT NULL AND (LEN(@contrasena) < 8 OR @contrasena NOT LIKE '%[0-9]%')
            THROW 50143, 'La contraseña debe tener al menos 8 caracteres y un número.', 1;
        IF NOT EXISTS (SELECT 1 FROM Empleado WHERE id_empleado = @id_empleado AND activo = 1)
            THROW 50144, 'El empleado no existe o está inactivo.', 1;
        IF NOT EXISTS (SELECT 1 FROM Rol WHERE id_rol = @id_rol)
            THROW 50145, 'El rol no existe.', 1;
        IF EXISTS (SELECT 1 FROM Usuario WHERE nombre_usuario = @nombre_usuario AND id_usuario <> ISNULL(@id_usuario, 0))
            THROW 50146, 'Ese nombre de usuario ya está en uso.', 1;
        IF EXISTS (SELECT 1 FROM Usuario WHERE id_empleado = @id_empleado AND id_usuario <> ISNULL(@id_usuario, 0))
            THROW 50147, 'Ese empleado ya tiene un usuario.', 1;
        IF @id_usuario = @id_admin AND (@activo = 0 OR @id_rol <> (SELECT id_rol FROM Rol WHERE nombre = 'Administrador'))
            THROW 50148, 'No puedes desactivarte ni quitarte el rol de Administrador a ti mismo.', 1;

        BEGIN TRANSACTION;
        IF @id_usuario IS NULL
        BEGIN
            INSERT INTO Usuario (id_empleado, nombre_usuario, hash_contrasena, activo)
            VALUES (@id_empleado, @nombre_usuario, HASHBYTES('SHA2_256', @contrasena), @activo);
            SET @id_usuario = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE Usuario
               SET id_empleado = @id_empleado, nombre_usuario = @nombre_usuario, activo = @activo,
                   hash_contrasena = CASE WHEN @contrasena IS NULL THEN hash_contrasena ELSE HASHBYTES('SHA2_256', @contrasena) END,
                   intentos_fallidos = CASE WHEN @activo = 1 THEN 0 ELSE intentos_fallidos END
             WHERE id_usuario = @id_usuario;
            IF @@ROWCOUNT = 0 THROW 50149, 'El usuario no existe.', 1;
            DELETE FROM UsuarioRol WHERE id_usuario = @id_usuario;
        END
        INSERT INTO UsuarioRol (id_usuario, id_rol) VALUES (@id_usuario, @id_rol);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* =========================== Procesos =========================== */

/* Proceso 1 (principal): registrar venta, maestro-detalle.
   @lineas: JSON [{"id_producto":1,"cantidad":1}, ...]
   Tablas: Venta, DetalleVenta, Producto, UnidadSerie, Garantia. */
CREATE OR ALTER PROCEDURE usp_RegistrarVenta
    @id_cliente          INT,
    @id_empleado         INT,
    @id_metodo_pago      INT,
    @lineas              NVARCHAR(MAX),
    @descuento_pct       DECIMAL(5,2) = 0,
    @id_usuario_autoriza INT          = NULL,
    @id_venta            INT          = NULL OUTPUT,
    @folio               VARCHAR(20)  = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @items TABLE (n INT IDENTITY(1,1), id_producto INT, cantidad INT);

    BEGIN TRY
        IF ISJSON(@lineas) = 0
            THROW 50001, 'El detalle de la venta no es válido.', 1;
        INSERT INTO @items (id_producto, cantidad)
        SELECT id_producto, SUM(cantidad)
        FROM OPENJSON(@lineas) WITH (id_producto INT '$.id_producto', cantidad INT '$.cantidad')
        GROUP BY id_producto;

        IF NOT EXISTS (SELECT 1 FROM @items)
            THROW 50001, 'La venta no tiene productos.', 1;
        IF EXISTS (SELECT 1 FROM @items WHERE cantidad IS NULL OR cantidad <= 0)
            THROW 50002, 'Todas las cantidades deben ser mayores a cero.', 1;
        IF NOT EXISTS (SELECT 1 FROM Cliente WHERE id_cliente = @id_cliente AND activo = 1)
            THROW 50003, 'El cliente no existe o está inactivo.', 1;
        IF NOT EXISTS (SELECT 1 FROM Empleado WHERE id_empleado = @id_empleado AND activo = 1)
            THROW 50004, 'El empleado no existe o está inactivo.', 1;
        IF EXISTS (SELECT 1 FROM @items i LEFT JOIN Producto p ON p.id_producto = i.id_producto
                   WHERE p.id_producto IS NULL OR p.activo = 0)
            THROW 50005, 'Hay productos inexistentes o inactivos en la venta.', 1;

        DECLARE @max_desc DECIMAL(5,2) =
            (SELECT CAST(valor AS DECIMAL(5,2)) FROM Configuracion WHERE clave = 'DESCUENTO_MAX_OPERADOR');
        IF @descuento_pct < 0 OR @descuento_pct > 50
            THROW 50006, 'El descuento debe estar entre 0 % y 50 %.', 1;
        IF @descuento_pct > @max_desc AND dbo.fn_UsuarioTieneRol(@id_usuario_autoriza, 'Supervisor') = 0
            THROW 50007, 'Un descuento mayor al permitido requiere autorización de un Supervisor.', 1;

        BEGIN TRANSACTION;

        -- Bloqueo de las filas de producto: dos cajas no venden la misma existencia
        IF EXISTS (SELECT 1 FROM @items i
                   JOIN Producto p WITH (UPDLOCK, HOLDLOCK) ON p.id_producto = i.id_producto
                   WHERE p.stock_actual < i.cantidad)
            THROW 50008, 'Stock insuficiente para uno o más productos.', 1;

        DECLARE @id_sucursal INT = (SELECT id_sucursal FROM Empleado WHERE id_empleado = @id_empleado);
        SET @folio = CONCAT('V-', RIGHT('00000' + CAST(
                        ISNULL((SELECT MAX(CAST(SUBSTRING(folio, 3, 10) AS INT)) FROM Venta WITH (UPDLOCK, HOLDLOCK)), 1000) + 1
                        AS VARCHAR), 5));

        INSERT INTO Venta (folio, id_cliente, id_empleado, id_sucursal, id_metodo_pago, subtotal, descuento, total)
        VALUES (@folio, @id_cliente, @id_empleado, @id_sucursal, @id_metodo_pago, 0, 0, 0);
        SET @id_venta = SCOPE_IDENTITY();

        DECLARE @n INT = 1, @max INT = (SELECT MAX(n) FROM @items), @p INT, @cant INT, @serie BIT,
                @k INT, @id_unidad INT, @precio DECIMAL(10,2), @meses INT, @id_det INT;
        WHILE @n <= @max
        BEGIN
            SELECT @p = i.id_producto, @cant = i.cantidad, @serie = p.requiere_serie, @precio = p.precio_venta,
                   @meses = CASE WHEN p.id_condicion = 2 THEN 6 ELSE 12 END
            FROM @items i JOIN Producto p ON p.id_producto = i.id_producto WHERE i.n = @n;

            IF @serie = 1
            BEGIN
                -- Una línea por pieza: cada una lleva su número de serie y su garantía
                SET @k = 1;
                WHILE @k <= @cant
                BEGIN
                    SET @id_unidad = NULL;
                    SELECT TOP 1 @id_unidad = id_unidad FROM UnidadSerie WITH (UPDLOCK, READPAST)
                    WHERE id_producto = @p AND estado = 'Disponible' ORDER BY id_unidad;
                    IF @id_unidad IS NULL
                        THROW 50009, 'No hay piezas con número de serie disponibles.', 1;

                    INSERT INTO DetalleVenta (id_venta, id_producto, id_unidad, cantidad, precio_unitario, subtotal)
                    VALUES (@id_venta, @p, @id_unidad, 1, @precio, @precio);
                    SET @id_det = SCOPE_IDENTITY();
                    UPDATE UnidadSerie SET estado = 'Vendida' WHERE id_unidad = @id_unidad;
                    INSERT INTO Garantia (id_detalle_venta, meses, fecha_inicio, fecha_fin)
                    VALUES (@id_det, @meses, CAST(GETDATE() AS DATE), DATEADD(MONTH, @meses, CAST(GETDATE() AS DATE)));
                    SET @k += 1;
                END
            END
            ELSE
                INSERT INTO DetalleVenta (id_venta, id_producto, cantidad, precio_unitario, subtotal)
                VALUES (@id_venta, @p, @cant, @precio, @precio * @cant);

            UPDATE Producto SET stock_actual = stock_actual - @cant WHERE id_producto = @p;
            SET @n += 1;
        END

        DECLARE @sub DECIMAL(12,2) = (SELECT SUM(subtotal) FROM DetalleVenta WHERE id_venta = @id_venta);
        UPDATE Venta
           SET subtotal = @sub,
               descuento = ROUND(@sub * @descuento_pct / 100, 2),
               total = @sub - ROUND(@sub * @descuento_pct / 100, 2)
         WHERE id_venta = @id_venta;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @id_venta = NULL; SET @folio = NULL;
        THROW;
    END CATCH
END
GO

/* Proceso 2 (secundario): registrar compra a proveedor, maestro-detalle.
   @lineas: JSON [{"id_producto":1,"cantidad":2,"costo_unitario":100,
                   "series":["SN1","SN2"]}, ...]
   "series" es obligatorio (una por pieza) si el producto requiere serie.
   Tablas: Compra, DetalleCompra, Producto, UnidadSerie, ProductoProveedor. */
CREATE OR ALTER PROCEDURE usp_RegistrarCompra
    @id_proveedor INT,
    @id_empleado  INT,
    @lineas       NVARCHAR(MAX),
    @id_compra    INT         = NULL OUTPUT,
    @folio        VARCHAR(20) = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @items TABLE (n INT IDENTITY(1,1), id_producto INT, cantidad INT, costo DECIMAL(10,2), series NVARCHAR(MAX));
    DECLARE @series TABLE (id_producto INT, numero_serie VARCHAR(50));

    BEGIN TRY
        IF ISJSON(@lineas) = 0
            THROW 50200, 'El detalle de la compra no es válido.', 1;
        INSERT INTO @items (id_producto, cantidad, costo, series)
        SELECT id_producto, cantidad, costo_unitario, series
        FROM OPENJSON(@lineas) WITH (id_producto INT, cantidad INT, costo_unitario DECIMAL(10,2),
                                     series NVARCHAR(MAX) '$.series' AS JSON);
        INSERT INTO @series (id_producto, numero_serie)
        SELECT i.id_producto, UPPER(LTRIM(RTRIM(s.value))) FROM @items i CROSS APPLY OPENJSON(i.series) s;

        IF NOT EXISTS (SELECT 1 FROM @items)
            THROW 50201, 'La compra no tiene productos.', 1;
        IF EXISTS (SELECT 1 FROM @items WHERE cantidad IS NULL OR cantidad <= 0 OR costo IS NULL OR costo <= 0)
            THROW 50202, 'Cantidades y costos deben ser mayores a cero.', 1;
        IF EXISTS (SELECT id_producto FROM @items GROUP BY id_producto HAVING COUNT(*) > 1)
            THROW 50203, 'Un producto aparece dos veces en la compra; súmalo en una sola línea.', 1;
        IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE id_proveedor = @id_proveedor AND activo = 1)
            THROW 50204, 'El proveedor no existe o está inactivo.', 1;
        IF NOT EXISTS (SELECT 1 FROM Empleado WHERE id_empleado = @id_empleado AND activo = 1)
            THROW 50205, 'El empleado no existe o está inactivo.', 1;
        IF EXISTS (SELECT 1 FROM @items i LEFT JOIN Producto p ON p.id_producto = i.id_producto WHERE p.id_producto IS NULL)
            THROW 50206, 'Hay productos inexistentes en la compra.', 1;
        -- Productos con serie: exactamente una serie por pieza, sin repetir
        IF EXISTS (SELECT 1 FROM @items i JOIN Producto p ON p.id_producto = i.id_producto
                   WHERE p.requiere_serie = 1
                     AND i.cantidad <> (SELECT COUNT(DISTINCT s.numero_serie) FROM @series s WHERE s.id_producto = i.id_producto))
            THROW 50207, 'Cada pieza de un producto con número de serie necesita su número de serie (sin repetir).', 1;
        IF EXISTS (SELECT 1 FROM @series s JOIN UnidadSerie u ON u.numero_serie = s.numero_serie)
            THROW 50208, 'Uno o más números de serie ya están registrados.', 1;

        BEGIN TRANSACTION;

        SET @folio = CONCAT('C-', RIGHT('0000' + CAST(
                        ISNULL((SELECT MAX(CAST(SUBSTRING(folio, 3, 10) AS INT)) FROM Compra WITH (UPDLOCK, HOLDLOCK)), 0) + 1
                        AS VARCHAR), 4));
        INSERT INTO Compra (folio, id_proveedor, id_empleado, total)
        VALUES (@folio, @id_proveedor, @id_empleado, 0);
        SET @id_compra = SCOPE_IDENTITY();

        INSERT INTO DetalleCompra (id_compra, id_producto, cantidad, costo_unitario, subtotal)
        SELECT @id_compra, id_producto, cantidad, costo, cantidad * costo FROM @items;

        INSERT INTO UnidadSerie (id_producto, id_detalle_compra, numero_serie)
        SELECT s.id_producto, dc.id_detalle_compra, s.numero_serie
        FROM @series s JOIN DetalleCompra dc ON dc.id_compra = @id_compra AND dc.id_producto = s.id_producto
        JOIN Producto p ON p.id_producto = s.id_producto AND p.requiere_serie = 1;

        -- Entrada al almacén y último costo
        UPDATE p SET stock_actual = p.stock_actual + i.cantidad, costo = i.costo
        FROM Producto p JOIN @items i ON i.id_producto = p.id_producto;

        -- Relación N:M producto–proveedor con el costo más reciente
        MERGE ProductoProveedor AS t
        USING (SELECT id_producto, costo FROM @items) AS s
           ON t.id_producto = s.id_producto AND t.id_proveedor = @id_proveedor
        WHEN MATCHED THEN UPDATE SET costo_proveedor = s.costo
        WHEN NOT MATCHED THEN INSERT (id_producto, id_proveedor, costo_proveedor) VALUES (s.id_producto, @id_proveedor, s.costo);

        UPDATE Compra SET total = (SELECT SUM(subtotal) FROM DetalleCompra WHERE id_compra = @id_compra)
        WHERE id_compra = @id_compra;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @id_compra = NULL; SET @folio = NULL;
        THROW;
    END CATCH
END
GO

/* Recepción de un equipo en el taller: OrdenServicio + línea de
   diagnóstico + primer registro en HistorialEstadoOrden. */
CREATE OR ALTER PROCEDURE usp_RegistrarOrdenServicio
    @id_cliente          INT,
    @id_empleado_tecnico INT,
    @equipo_descripcion  VARCHAR(150),
    @descripcion_falla   VARCHAR(500),
    @numero_serie_equipo VARCHAR(50) = NULL,
    @id_orden            INT         = NULL OUTPUT,
    @folio               VARCHAR(20) = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        IF LTRIM(@equipo_descripcion) = '' OR LTRIM(@descripcion_falla) = ''
            THROW 50300, 'Describe el equipo y la falla.', 1;
        IF NOT EXISTS (SELECT 1 FROM Cliente WHERE id_cliente = @id_cliente AND activo = 1)
            THROW 50301, 'El cliente no existe o está inactivo.', 1;
        IF dbo.fn_TecnicoHabilitado(@id_empleado_tecnico, 1) = 0
            THROW 50302, 'El técnico no está activo o no está habilitado para diagnósticos.', 1;

        BEGIN TRANSACTION;
        SET @folio = CONCAT('OS-', RIGHT('0000' + CAST(
                        ISNULL((SELECT MAX(CAST(SUBSTRING(folio, 4, 10) AS INT)) FROM OrdenServicio WITH (UPDLOCK, HOLDLOCK)), 3000) + 1
                        AS VARCHAR), 4));

        INSERT INTO OrdenServicio (folio, id_cliente, id_empleado_tecnico, id_sucursal, id_estado_orden,
                                   equipo_descripcion, numero_serie_equipo, descripcion_falla)
        SELECT @folio, @id_cliente, @id_empleado_tecnico, e.id_sucursal, 1,
               @equipo_descripcion, NULLIF(@numero_serie_equipo, ''), @descripcion_falla
        FROM Empleado e WHERE e.id_empleado = @id_empleado_tecnico;
        SET @id_orden = SCOPE_IDENTITY();

        INSERT INTO DetalleOrdenServicio (id_orden, id_tipo_servicio, cantidad, precio_unitario, subtotal)
        SELECT @id_orden, id_tipo_servicio, 1, precio_base, precio_base FROM TipoServicio WHERE id_tipo_servicio = 1;

        UPDATE OrdenServicio SET total = dbo.fn_TotalOrden(@id_orden) WHERE id_orden = @id_orden;

        INSERT INTO HistorialEstadoOrden (id_orden, id_estado_anterior, id_estado_nuevo, id_usuario)
        VALUES (@id_orden, NULL, 1, TRY_CAST(SESSION_CONTEXT(N'id_usuario') AS INT));

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @id_orden = NULL; SET @folio = NULL;
        THROW;
    END CATCH
END
GO

/* Agrega mano de obra (@id_tipo_servicio) o una refacción (@id_producto).
   La refacción sale del almacén; el técnico debe estar habilitado
   para el servicio. Recalcula el total de la orden. */
CREATE OR ALTER PROCEDURE usp_OrdenAgregarLinea
    @id_orden         INT,
    @id_tipo_servicio INT = NULL,
    @id_producto      INT = NULL,
    @cantidad         INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        IF (@id_tipo_servicio IS NULL AND @id_producto IS NULL) OR (@id_tipo_servicio IS NOT NULL AND @id_producto IS NOT NULL)
            THROW 50310, 'Indica un servicio o una refacción (solo uno).', 1;
        IF @cantidad <= 0
            THROW 50311, 'La cantidad debe ser mayor a cero.', 1;

        BEGIN TRANSACTION;
        DECLARE @tecnico INT, @final BIT;
        SELECT @tecnico = o.id_empleado_tecnico, @final = eo.es_final
        FROM OrdenServicio o WITH (UPDLOCK) JOIN EstadoOrden eo ON eo.id_estado_orden = o.id_estado_orden
        WHERE o.id_orden = @id_orden;
        IF @tecnico IS NULL THROW 50312, 'La orden no existe.', 1;
        IF @final = 1       THROW 50313, 'La orden ya está cerrada (Entregada o Cancelada).', 1;

        IF @id_tipo_servicio IS NOT NULL
        BEGIN
            IF dbo.fn_TecnicoHabilitado(@tecnico, @id_tipo_servicio) = 0
                THROW 50314, 'El técnico asignado no está habilitado para ese servicio.', 1;
            INSERT INTO DetalleOrdenServicio (id_orden, id_tipo_servicio, cantidad, precio_unitario, subtotal)
            SELECT @id_orden, @id_tipo_servicio, @cantidad, precio_base, precio_base * @cantidad
            FROM TipoServicio WHERE id_tipo_servicio = @id_tipo_servicio;
            IF @@ROWCOUNT = 0 THROW 50315, 'El servicio no existe.', 1;
        END
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM Producto WHERE id_producto = @id_producto AND requiere_serie = 1)
                THROW 50316, 'Las refacciones se controlan por cantidad; ese producto requiere número de serie.', 1;
            UPDATE Producto WITH (UPDLOCK) SET stock_actual = stock_actual - @cantidad
            WHERE id_producto = @id_producto AND activo = 1 AND stock_actual >= @cantidad;
            IF @@ROWCOUNT = 0 THROW 50317, 'La refacción no existe, está inactiva o no hay stock suficiente.', 1;
            INSERT INTO DetalleOrdenServicio (id_orden, id_producto, cantidad, precio_unitario, subtotal)
            SELECT @id_orden, @id_producto, @cantidad, precio_venta, precio_venta * @cantidad
            FROM Producto WHERE id_producto = @id_producto;
        END

        UPDATE OrdenServicio SET total = dbo.fn_TotalOrden(@id_orden) WHERE id_orden = @id_orden;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* Transiciones válidas: avanzar un paso (Recibida → En diagnóstico →
   En reparación → Lista → Entregada) o cancelar si no está cerrada.
   Al cancelar, las refacciones vuelven al almacén. El historial lo
   escribe el trigger trg_OrdenServicio_Estado. */
CREATE OR ALTER PROCEDURE usp_CambiarEstadoOrden
    @id_orden    INT,
    @id_estado   INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @actual INT;
        SELECT @actual = id_estado_orden FROM OrdenServicio WITH (UPDLOCK) WHERE id_orden = @id_orden;
        IF @actual IS NULL THROW 50320, 'La orden no existe.', 1;
        IF @actual IN (5, 6) THROW 50321, 'Una orden Entregada o Cancelada ya no puede cambiar de estado.', 1;
        IF NOT (@id_estado = @actual + 1 AND @id_estado <= 5) AND @id_estado <> 6
            THROW 50322, 'Transición no permitida: la orden solo avanza un paso a la vez o se cancela.', 1;

        IF @id_estado = 6
            UPDATE p SET stock_actual = p.stock_actual + d.cantidad
            FROM Producto p JOIN DetalleOrdenServicio d ON d.id_producto = p.id_producto
            WHERE d.id_orden = @id_orden;

        UPDATE OrdenServicio
           SET id_estado_orden = @id_estado,
               fecha_entrega = CASE WHEN @id_estado = 5 THEN SYSDATETIME() ELSE fecha_entrega END
         WHERE id_orden = @id_orden;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* Proceso 3 (cancelación): reintegra stock, libera piezas y elimina
   sus garantías. Solo ventas de los últimos 30 días. */
CREATE OR ALTER PROCEDURE usp_CancelarVenta
    @id_venta INT,
    @motivo   VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        IF LTRIM(ISNULL(@motivo, '')) = ''
            THROW 50023, 'Captura el motivo de la cancelación.', 1;

        BEGIN TRANSACTION;
        DECLARE @estado VARCHAR(12), @fecha DATETIME2;
        SELECT @estado = estado, @fecha = fecha FROM Venta WITH (UPDLOCK) WHERE id_venta = @id_venta;
        IF @estado IS NULL       THROW 50020, 'La venta no existe.', 1;
        IF @estado = 'Cancelada' THROW 50021, 'La venta ya estaba cancelada.', 1;
        IF DATEDIFF(DAY, @fecha, SYSDATETIME()) > 30
            THROW 50022, 'Solo se pueden cancelar ventas de los últimos 30 días.', 1;

        UPDATE p SET stock_actual = p.stock_actual + x.cant
        FROM Producto p
        JOIN (SELECT id_producto, SUM(cantidad) cant FROM DetalleVenta WHERE id_venta = @id_venta GROUP BY id_producto) x
          ON x.id_producto = p.id_producto;

        UPDATE u SET estado = 'Disponible'
        FROM UnidadSerie u JOIN DetalleVenta dv ON dv.id_unidad = u.id_unidad
        WHERE dv.id_venta = @id_venta;

        DELETE g FROM Garantia g JOIN DetalleVenta dv ON dv.id_detalle_venta = g.id_detalle_venta
        WHERE dv.id_venta = @id_venta;

        UPDATE Venta SET estado = 'Cancelada', fecha_cancelacion = SYSDATETIME(), motivo_cancelacion = @motivo
        WHERE id_venta = @id_venta;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* ========================= Administrativos ========================= */

/* Proceso 4 (ajuste): corrige el stock de un producto por conteo físico,
   merma o daño. Solo Supervisor/Administrador, máximo ±20 piezas por
   ajuste, nunca negativo. El trigger de auditoría guarda el cambio con
   el motivo. Productos con serie se ajustan dando de baja la pieza. */
CREATE OR ALTER PROCEDURE usp_AjustarStock
    @id_producto INT,
    @cantidad    INT,            -- positivo entra, negativo sale
    @motivo      VARCHAR(200),
    @id_usuario  INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        IF dbo.fn_UsuarioTieneRol(@id_usuario, 'Supervisor') = 0
            THROW 50400, 'Solo un Supervisor o Administrador puede ajustar inventario.', 1;
        IF @cantidad = 0 OR ABS(@cantidad) > 20
            THROW 50401, 'El ajuste debe ser distinto de cero y de máximo 20 piezas.', 1;
        IF LTRIM(ISNULL(@motivo, '')) = ''
            THROW 50402, 'Captura el motivo del ajuste.', 1;

        BEGIN TRANSACTION;
        DECLARE @stock INT, @serie BIT;
        SELECT @stock = stock_actual, @serie = requiere_serie FROM Producto WITH (UPDLOCK) WHERE id_producto = @id_producto AND activo = 1;
        IF @stock IS NULL           THROW 50403, 'El producto no existe o está inactivo.', 1;
        IF @serie = 1               THROW 50404, 'Los productos con número de serie se ajustan dando de baja la pieza.', 1;
        IF @stock + @cantidad < 0   THROW 50405, 'El ajuste dejaría el stock en negativo.', 1;

        EXEC sp_set_session_context N'motivo', @motivo;
        UPDATE Producto SET stock_actual = stock_actual + @cantidad WHERE id_producto = @id_producto;
        EXEC sp_set_session_context N'motivo', NULL;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC sp_set_session_context N'motivo', NULL;
        THROW;
    END CATCH
END
GO

/* Cambio de precio por porcentaje, a un producto o a toda una categoría
   (parámetros opcionales). Límite ±30 %; si algún precio quedara bajo el
   costo, CK_Producto_precio_vs_costo detiene todo y se revierte. */
CREATE OR ALTER PROCEDURE usp_AjustarPrecio
    @porcentaje   DECIMAL(5,2),
    @id_usuario   INT,
    @id_producto  INT = NULL,
    @id_categoria INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        IF dbo.fn_UsuarioTieneRol(@id_usuario, 'Supervisor') = 0
            THROW 50410, 'Solo un Supervisor o Administrador puede cambiar precios.', 1;
        IF @porcentaje = 0 OR ABS(@porcentaje) > 30
            THROW 50411, 'El cambio de precio debe ser distinto de cero y de máximo ±30 %.', 1;
        IF @id_producto IS NULL AND @id_categoria IS NULL
            THROW 50412, 'Indica un producto o una categoría.', 1;

        BEGIN TRANSACTION;
        EXEC sp_set_session_context N'motivo', 'Ajuste de precio por porcentaje';
        UPDATE Producto
           SET precio_venta = ROUND(precio_venta * (1 + @porcentaje / 100), 0)
         WHERE activo = 1
           AND (@id_producto IS NULL OR id_producto = @id_producto)
           AND (@id_categoria IS NULL OR id_categoria = @id_categoria);
        IF @@ROWCOUNT = 0 THROW 50413, 'Ningún producto activo coincide con el filtro.', 1;
        EXEC sp_set_session_context N'motivo', NULL;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC sp_set_session_context N'motivo', NULL;
        THROW;
    END CATCH
END
GO

/* ============================ Reportes ============================ */

CREATE OR ALTER PROCEDURE usp_ResumenPeriodo
    @desde DATE = NULL,
    @hasta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @desde = ISNULL(@desde, DATEADD(MONTH, -6, CAST(GETDATE() AS DATE)));
    SET @hasta = ISNULL(@hasta, CAST(GETDATE() AS DATE));
    IF @hasta < @desde THROW 50030, 'La fecha final no puede ser menor que la inicial.', 1;

    SELECT FORMAT(fecha, 'yyyy-MM') AS mes, COUNT(*) AS ventas,
           SUM(total) AS ingresos, CAST(AVG(total) AS DECIMAL(12,2)) AS ticket_promedio
    FROM Venta
    WHERE estado = 'Completada' AND fecha >= @desde AND fecha < DATEADD(DAY, 1, @hasta)
    GROUP BY FORMAT(fecha, 'yyyy-MM')
    ORDER BY mes;
END
GO

-- Actividad por empleado: ventas, importe vendido y órdenes de taller atendidas
CREATE OR ALTER PROCEDURE usp_ActividadEmpleados
    @desde       DATE = NULL,
    @hasta       DATE = NULL,
    @id_sucursal INT  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @desde = ISNULL(@desde, '2000-01-01');
    SET @hasta = ISNULL(@hasta, CAST(GETDATE() AS DATE));
    IF @hasta < @desde THROW 50030, 'La fecha final no puede ser menor que la inicial.', 1;

    SELECT CONCAT(e.nombre, ' ', e.apellidos) AS empleado, e.puesto, s.nombre AS sucursal,
           ISNULL(v.ventas, 0) AS ventas, ISNULL(v.vendido, 0) AS vendido, ISNULL(o.ordenes, 0) AS ordenes
    FROM Empleado e
    JOIN Sucursal s ON s.id_sucursal = e.id_sucursal
    LEFT JOIN (SELECT id_empleado, COUNT(*) AS ventas, SUM(total) AS vendido FROM Venta
               WHERE estado = 'Completada' AND fecha >= @desde AND fecha < DATEADD(DAY, 1, @hasta)
               GROUP BY id_empleado) v ON v.id_empleado = e.id_empleado
    LEFT JOIN (SELECT id_empleado_tecnico, COUNT(*) AS ordenes FROM OrdenServicio
               WHERE fecha_recepcion >= @desde AND fecha_recepcion < DATEADD(DAY, 1, @hasta)
               GROUP BY id_empleado_tecnico) o ON o.id_empleado_tecnico = e.id_empleado
    WHERE (@id_sucursal IS NULL OR e.id_sucursal = @id_sucursal)
    ORDER BY vendido DESC, ordenes DESC;
END
GO
