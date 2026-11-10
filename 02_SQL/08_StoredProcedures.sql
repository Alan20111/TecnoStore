/* =============================================================
   TecnoStore · 08_StoredProcedures.sql
   Avance: catálogo (búsqueda), 2 procesos transaccionales y
   1 reporte. Faltan los demás para llegar a 10–12.
   ============================================================= */
USE TecnoStore;
GO

/* ---------- Catálogo: búsqueda con parámetros opcionales ---------- */
CREATE OR ALTER PROCEDURE usp_ProductoBuscar
    @texto        VARCHAR(60) = NULL,
    @id_categoria INT         = NULL,
    @solo_alertas BIT         = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_producto, sku, nombre, categoria, marca, condicion,
           precio_venta, stock_actual, stock_minimo, alerta_stock,
           dbo.fn_MargenProducto(id_producto) AS margen_pct
    FROM vw_ProductosActivos
    WHERE (@texto IS NULL OR nombre LIKE '%' + @texto + '%' OR sku LIKE '%' + @texto + '%')
      AND (@id_categoria IS NULL OR id_categoria = @id_categoria)
      AND (@solo_alertas = 0 OR alerta_stock <> 'OK')
    ORDER BY categoria, nombre;
END
GO

/* ---------- Proceso principal: registrar venta (maestro-detalle) ----------
   @lineas: JSON [{"id_producto":1,"cantidad":1}, ...]
   - Valida cliente/empleado activos, existencia, stock y límite de descuento.
   - Productos con número de serie: toma la primera UnidadSerie Disponible
     por pieza y la marca Vendida; genera su garantía.
   - Todo o nada: cualquier error hace ROLLBACK completo.               */
CREATE OR ALTER PROCEDURE usp_RegistrarVenta
    @id_cliente      INT,
    @id_empleado     INT,
    @id_metodo_pago  INT,
    @lineas          NVARCHAR(MAX),
    @descuento_pct   DECIMAL(5,2) = 0,
    @id_usuario_autoriza INT      = NULL,
    @id_venta        INT          = NULL OUTPUT,
    @folio           VARCHAR(20)  = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @items TABLE (n INT IDENTITY(1,1), id_producto INT, cantidad INT);
    INSERT INTO @items (id_producto, cantidad)
    SELECT id_producto, cantidad
    FROM OPENJSON(@lineas) WITH (id_producto INT '$.id_producto', cantidad INT '$.cantidad');

    BEGIN TRY
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

        -- Límite de descuento sin autorización de Supervisor/Administrador
        DECLARE @max_desc DECIMAL(5,2) =
            (SELECT CAST(valor AS DECIMAL(5,2)) FROM Configuracion WHERE clave = 'DESCUENTO_MAX_OPERADOR');
        IF @descuento_pct < 0 OR @descuento_pct > 50
            THROW 50006, 'El descuento debe estar entre 0 % y 50 %.', 1;
        IF @descuento_pct > @max_desc AND NOT EXISTS (
            SELECT 1 FROM UsuarioRol ur JOIN Rol r ON r.id_rol = ur.id_rol
            WHERE ur.id_usuario = @id_usuario_autoriza AND r.nombre IN ('Supervisor','Administrador'))
            THROW 50007, 'Un descuento mayor al permitido requiere autorización de un Supervisor.', 1;

        BEGIN TRANSACTION;

        -- Stock suficiente, con bloqueo para que dos cajas no vendan la misma pieza
        IF EXISTS (SELECT 1 FROM (SELECT id_producto, SUM(cantidad) cant FROM @items GROUP BY id_producto) i
                   JOIN Producto p WITH (UPDLOCK, HOLDLOCK) ON p.id_producto = i.id_producto
                   WHERE p.stock_actual < i.cant)
            THROW 50008, 'Stock insuficiente para uno o más productos.', 1;

        DECLARE @id_sucursal INT = (SELECT id_sucursal FROM Empleado WHERE id_empleado = @id_empleado);
        SET @folio = CONCAT('V-', RIGHT('00000' + CAST(
                        ISNULL((SELECT MAX(CAST(SUBSTRING(folio, 3, 10) AS INT)) FROM Venta WITH (UPDLOCK, HOLDLOCK)), 1000) + 1
                        AS VARCHAR), 5));

        INSERT INTO Venta (folio, id_cliente, id_empleado, id_sucursal, id_metodo_pago, subtotal, descuento, total, estado)
        VALUES (@folio, @id_cliente, @id_empleado, @id_sucursal, @id_metodo_pago, 0, 0, 0, 'Completada');
        SET @id_venta = SCOPE_IDENTITY();

        -- Una línea por pieza para productos con serie; una sola línea para los demás
        DECLARE @n INT = 1, @max INT = (SELECT MAX(n) FROM @items), @p INT, @cant INT, @serie BIT,
                @k INT, @id_unidad INT, @precio DECIMAL(10,2), @meses INT, @id_det INT;
        WHILE @n <= @max
        BEGIN
            SELECT @p = i.id_producto, @cant = i.cantidad, @serie = p.requiere_serie, @precio = p.precio_venta,
                   @meses = CASE WHEN p.id_condicion = 2 THEN 6 ELSE 12 END
            FROM @items i JOIN Producto p ON p.id_producto = i.id_producto WHERE i.n = @n;

            IF @serie = 1
            BEGIN
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

/* ---------- Cancelación: reintegra stock y libera piezas ---------- */
CREATE OR ALTER PROCEDURE usp_CancelarVenta
    @id_venta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @estado VARCHAR(12), @fecha DATETIME2;
        SELECT @estado = estado, @fecha = fecha FROM Venta WITH (UPDLOCK) WHERE id_venta = @id_venta;
        IF @estado IS NULL          THROW 50020, 'La venta no existe.', 1;
        IF @estado = 'Cancelada'    THROW 50021, 'La venta ya estaba cancelada.', 1;
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

        UPDATE Venta SET estado = 'Cancelada' WHERE id_venta = @id_venta;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* ---------- Reporte: resumen por periodo (fechas opcionales) ---------- */
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
