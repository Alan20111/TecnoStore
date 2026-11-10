/* =============================================================
   TecnoStore · 06_Vistas.sql
   2 operativas, 2 relacionales, 2 analíticas.
   ============================================================= */
USE TecnoStore;
GO

/* ---------- Operativas ---------- */
CREATE OR ALTER VIEW vw_ProductosActivos AS
SELECT p.id_producto, p.sku, p.nombre, p.id_categoria, c.nombre AS categoria, m.nombre AS marca,
       cd.nombre AS condicion, p.precio_venta, p.costo, p.stock_actual, p.stock_minimo,
       p.requiere_serie,
       CASE WHEN p.stock_actual = 0 THEN 'Agotado'
            WHEN p.stock_actual <= p.stock_minimo THEN 'Stock bajo'
            ELSE 'OK' END AS alerta_stock
FROM Producto p
JOIN Categoria c  ON c.id_categoria = p.id_categoria
JOIN Marca m      ON m.id_marca = p.id_marca
JOIN Condicion cd ON cd.id_condicion = p.id_condicion
WHERE p.activo = 1;
GO

CREATE OR ALTER VIEW vw_StockBajo AS
SELECT p.id_producto, p.sku, p.nombre, c.nombre AS categoria, p.stock_actual, p.stock_minimo,
       p.stock_minimo - p.stock_actual AS faltante,
       pv.razon_social AS proveedor_sugerido, pp.costo_proveedor
FROM Producto p
JOIN Categoria c ON c.id_categoria = p.id_categoria
LEFT JOIN ProductoProveedor pp ON pp.id_producto = p.id_producto
     AND pp.costo_proveedor = (SELECT MIN(x.costo_proveedor) FROM ProductoProveedor x WHERE x.id_producto = p.id_producto)
LEFT JOIN Proveedor pv ON pv.id_proveedor = pp.id_proveedor
WHERE p.activo = 1 AND p.stock_actual <= p.stock_minimo;
GO

/* ---------- Relacionales ---------- */
CREATE OR ALTER VIEW vw_VentaCompleta AS
SELECT v.id_venta, v.folio, v.fecha, v.estado,
       v.id_cliente, CONCAT(cl.nombre, ' ', cl.apellidos) AS cliente,
       CONCAT(e.nombre, ' ', e.apellidos) AS vendedor,
       s.nombre AS sucursal, mp.nombre AS metodo_pago,
       (SELECT COUNT(*) FROM DetalleVenta dv WHERE dv.id_venta = v.id_venta) AS lineas,
       v.subtotal, v.descuento, v.total
FROM Venta v
JOIN Cliente cl    ON cl.id_cliente = v.id_cliente
JOIN Empleado e    ON e.id_empleado = v.id_empleado
JOIN Sucursal s    ON s.id_sucursal = v.id_sucursal
JOIN MetodoPago mp ON mp.id_metodo_pago = v.id_metodo_pago;
GO

CREATE OR ALTER VIEW vw_OrdenServicioCompleta AS
SELECT o.id_orden, o.folio, o.fecha_recepcion, o.fecha_entrega,
       CONCAT(cl.nombre, ' ', cl.apellidos) AS cliente, cl.telefono,
       CONCAT(e.nombre, ' ', e.apellidos) AS tecnico,
       eo.nombre AS estado, eo.es_final,
       o.equipo_descripcion, o.numero_serie_equipo, o.descripcion_falla, o.total,
       DATEDIFF(DAY, o.fecha_recepcion, ISNULL(o.fecha_entrega, SYSDATETIME())) AS dias_en_taller
FROM OrdenServicio o
JOIN Cliente cl     ON cl.id_cliente = o.id_cliente
JOIN Empleado e     ON e.id_empleado = o.id_empleado_tecnico
JOIN EstadoOrden eo ON eo.id_estado_orden = o.id_estado_orden;
GO

/* ---------- Analíticas ---------- */
CREATE OR ALTER VIEW vw_ResumenMensual AS
SELECT FORMAT(v.fecha, 'yyyy-MM') AS mes,
       COUNT(*) AS ventas,
       SUM(v.total) AS ingresos,
       AVG(v.total) AS ticket_promedio,
       SUM(v.descuento) AS descuentos
FROM Venta v
WHERE v.estado = 'Completada'
GROUP BY FORMAT(v.fecha, 'yyyy-MM');
GO

CREATE OR ALTER VIEW vw_TopProductos AS
SELECT p.id_producto, p.sku, p.nombre, c.nombre AS categoria,
       SUM(dv.cantidad) AS unidades_vendidas,
       SUM(dv.subtotal) AS ingresos,
       SUM(dv.subtotal - dv.cantidad * p.costo) AS utilidad_bruta
FROM DetalleVenta dv
JOIN Venta v     ON v.id_venta = dv.id_venta AND v.estado = 'Completada'
JOIN Producto p  ON p.id_producto = dv.id_producto
JOIN Categoria c ON c.id_categoria = p.id_categoria
GROUP BY p.id_producto, p.sku, p.nombre, c.nombre;
GO
