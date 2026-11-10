/* =============================================================
   TecnoStore · 09_Functions.sql
   3 escalares + 1 de tabla. Se usan en SP, reportes y la app.
   ============================================================= */
USE TecnoStore;
GO

-- Total histórico comprado por un cliente (solo ventas completadas)
CREATE OR ALTER FUNCTION fn_TotalCompradoCliente (@id_cliente INT)
RETURNS DECIMAL(12,2)
AS
BEGIN
    RETURN ISNULL((SELECT SUM(total) FROM Venta
                   WHERE id_cliente = @id_cliente AND estado = 'Completada'), 0);
END
GO

-- Clasificación del cliente según su gasto histórico
CREATE OR ALTER FUNCTION fn_ClasificacionCliente (@id_cliente INT)
RETURNS VARCHAR(12)
AS
BEGIN
    DECLARE @total DECIMAL(12,2) = dbo.fn_TotalCompradoCliente(@id_cliente);
    RETURN CASE WHEN @total >= 60000 THEN 'Oro'
                WHEN @total >= 20000 THEN 'Plata'
                WHEN @total > 0      THEN 'Bronce'
                ELSE 'Sin compras' END;
END
GO

-- Margen porcentual de un producto sobre su precio de venta
CREATE OR ALTER FUNCTION fn_MargenProducto (@id_producto INT)
RETURNS DECIMAL(5,2)
AS
BEGIN
    RETURN (SELECT CAST((precio_venta - costo) * 100.0 / precio_venta AS DECIMAL(5,2))
            FROM Producto WHERE id_producto = @id_producto);
END
GO

-- Ventas por categoría en un rango de fechas (función de tabla)
CREATE OR ALTER FUNCTION fn_VentasPorCategoria (@desde DATE, @hasta DATE)
RETURNS TABLE
AS
RETURN
    SELECT c.nombre AS categoria,
           COUNT(DISTINCT v.id_venta) AS ventas,
           SUM(dv.cantidad) AS unidades,
           SUM(dv.subtotal) AS ingresos
    FROM DetalleVenta dv
    JOIN Venta v     ON v.id_venta = dv.id_venta
    JOIN Producto p  ON p.id_producto = dv.id_producto
    JOIN Categoria c ON c.id_categoria = p.id_categoria
    WHERE v.estado = 'Completada'
      AND v.fecha >= @desde AND v.fecha < DATEADD(DAY, 1, @hasta)
    GROUP BY c.nombre;
GO
