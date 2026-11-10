/* =============================================================
   TecnoStore · 05_Consultas.sql
   Consultas de la Unidad 2 (sección 7.2). Cada una indica qué
   requisito demuestra. Se pueden ejecutar sueltas en SSMS.

   Requisito                         Mín.  Consultas
   WHERE compuesto                    5    Q01–Q05
   LIKE / búsqueda por texto          3    Q06–Q08
   Rango de fechas                    3    Q09–Q11
   GROUP BY                           4    Q12–Q15
   HAVING                             2    Q16–Q17
   Funciones fecha/texto/conversión   4    Q18–Q21
   INNER JOIN                         5    Q22–Q26
   LEFT JOIN                          3    Q27–Q29
   5 o más tablas                     2    Q30–Q31
   Subconsulta normal                 2    Q32–Q33
   Subconsulta correlacionada         2    Q34–Q35
   Operadores SET (3 distintos)       4    Q36–Q39
   Paginación (ORDER BY + OFFSET)     —    Q40
   ============================================================= */
USE TecnoStore;
GO

/* ===================== WHERE compuesto ===================== */

-- Q01 · Productos activos con número de serie, precio entre 10 y 20 mil y con existencia
SELECT sku, nombre, precio_venta, stock_actual
FROM Producto
WHERE activo = 1 AND requiere_serie = 1
  AND precio_venta BETWEEN 10000 AND 20000 AND stock_actual > 0
ORDER BY precio_venta DESC;

-- Q02 · Ventas completadas con descuento o pagadas a meses sin intereses
SELECT folio, fecha, subtotal, descuento, total, id_metodo_pago
FROM Venta
WHERE estado = 'Completada' AND (descuento > 0 OR id_metodo_pago = 5)
ORDER BY fecha;

-- Q03 · Clientes activos registrados en 2026 sin teléfono o sin RFC
SELECT id_cliente, nombre, apellidos, correo, telefono, rfc
FROM Cliente
WHERE activo = 1 AND YEAR(fecha_registro) = 2026 AND (telefono IS NULL OR rfc IS NULL);

-- Q04 · Órdenes no entregadas con más de 100 pesos y sin número de serie del equipo
SELECT folio, equipo_descripcion, total, id_estado_orden
FROM OrdenServicio
WHERE id_estado_orden NOT IN (5, 6) AND total > 100 AND numero_serie_equipo IS NULL;

-- Q05 · Empleados activos que no son vendedores, contratados antes de 2024
SELECT nombre, apellidos, puesto, fecha_contratacion
FROM Empleado
WHERE activo = 1 AND puesto <> 'Vendedor' AND fecha_contratacion < '2024-01-01'
ORDER BY fecha_contratacion;

/* ===================== LIKE ===================== */

-- Q06 · Productos Apple de la línea iPhone (búsqueda por texto en nombre)
SELECT sku, nombre, precio_venta FROM Producto WHERE nombre LIKE 'iPhone%';

-- Q07 · Clientes con correo que contiene "ez" (apellidos tipo Pérez, Gómez...)
SELECT nombre, apellidos, correo FROM Cliente WHERE correo LIKE '%ez%';

-- Q08 · Fallas reportadas sobre batería o pantalla
SELECT folio, equipo_descripcion, descripcion_falla
FROM OrdenServicio
WHERE descripcion_falla LIKE '%bater%' OR descripcion_falla LIKE '%pantalla%';

/* ===================== Rangos de fecha ===================== */

-- Q09 · Ventas de julio de 2026 (rango semiabierto, aprovecha índices)
SELECT folio, fecha, total FROM Venta
WHERE fecha >= '2026-07-01' AND fecha < '2026-08-01'
ORDER BY fecha;

-- Q10 · Garantías que vencen en los próximos 90 días
SELECT g.id_garantia, g.fecha_inicio, g.fecha_fin, g.meses
FROM Garantia g
WHERE g.fecha_fin BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 90, CAST(GETDATE() AS DATE));

-- Q11 · Órdenes recibidas en los últimos 60 días
SELECT folio, fecha_recepcion, equipo_descripcion
FROM OrdenServicio
WHERE fecha_recepcion >= DATEADD(DAY, -60, SYSDATETIME());

/* ===================== GROUP BY ===================== */

-- Q12 · Ingresos por método de pago
SELECT mp.nombre AS metodo, COUNT(*) AS ventas, SUM(v.total) AS ingresos
FROM Venta v JOIN MetodoPago mp ON mp.id_metodo_pago = v.id_metodo_pago
WHERE v.estado = 'Completada'
GROUP BY mp.nombre ORDER BY ingresos DESC;

-- Q13 · Productos y valor de inventario por categoría
SELECT c.nombre AS categoria, COUNT(*) AS productos, SUM(p.stock_actual) AS piezas,
       SUM(p.stock_actual * p.costo) AS valor_inventario
FROM Producto p JOIN Categoria c ON c.id_categoria = p.id_categoria
GROUP BY c.nombre ORDER BY valor_inventario DESC;

-- Q14 · Órdenes de servicio por estado
SELECT eo.nombre AS estado, COUNT(o.id_orden) AS ordenes, SUM(o.total) AS importe
FROM EstadoOrden eo LEFT JOIN OrdenServicio o ON o.id_estado_orden = eo.id_estado_orden
GROUP BY eo.id_estado_orden, eo.nombre ORDER BY eo.id_estado_orden;

-- Q15 · Unidades con número de serie por estado
SELECT estado, COUNT(*) AS unidades FROM UnidadSerie GROUP BY estado;

/* ===================== HAVING ===================== */

-- Q16 · Clientes con 3 o más compras completadas
SELECT id_cliente, COUNT(*) AS compras, SUM(total) AS gastado
FROM Venta WHERE estado = 'Completada'
GROUP BY id_cliente HAVING COUNT(*) >= 3
ORDER BY gastado DESC;

-- Q17 · Categorías cuyo ticket promedio por línea supera 5,000
SELECT c.nombre, AVG(dv.subtotal) AS promedio_linea
FROM DetalleVenta dv
JOIN Producto p ON p.id_producto = dv.id_producto
JOIN Categoria c ON c.id_categoria = p.id_categoria
GROUP BY c.nombre HAVING AVG(dv.subtotal) > 5000;

/* ===================== Funciones de fecha, texto y conversión ===================== */

-- Q18 · Antigüedad de empleados (DATEDIFF, DATENAME)
SELECT CONCAT(nombre, ' ', apellidos) AS empleado, fecha_contratacion,
       DATEDIFF(MONTH, fecha_contratacion, GETDATE()) / 12 AS anios,
       DATENAME(WEEKDAY, fecha_contratacion) AS dia_ingreso
FROM Empleado;

-- Q19 · Nombre en formato "APELLIDOS, Nombre" e iniciales (UPPER, LEFT, CONCAT)
SELECT UPPER(apellidos) + ', ' + nombre AS cliente,
       LEFT(nombre, 1) + LEFT(apellidos, 1) AS iniciales,
       LEN(correo) AS largo_correo
FROM Cliente;

-- Q20 · Folio numérico y total en texto con formato de moneda (CAST, SUBSTRING, FORMAT)
SELECT folio, CAST(SUBSTRING(folio, 3, 10) AS INT) AS numero,
       FORMAT(total, 'C', 'es-MX') AS total_texto,
       CONVERT(VARCHAR(10), fecha, 103) AS fecha_dd_mm_aaaa
FROM Venta;

-- Q21 · Ventas por día de la semana (DATEPART, DATENAME)
SELECT DATEPART(WEEKDAY, fecha) AS n, DATENAME(WEEKDAY, fecha) AS dia, COUNT(*) AS ventas
FROM Venta GROUP BY DATEPART(WEEKDAY, fecha), DATENAME(WEEKDAY, fecha) ORDER BY n;

/* ===================== INNER JOIN ===================== */

-- Q22 · Productos con su categoría, marca y condición
SELECT p.sku, p.nombre, c.nombre AS categoria, m.nombre AS marca, cd.nombre AS condicion
FROM Producto p
INNER JOIN Categoria c  ON c.id_categoria = p.id_categoria
INNER JOIN Marca m      ON m.id_marca = p.id_marca
INNER JOIN Condicion cd ON cd.id_condicion = p.id_condicion;

-- Q23 · Empleados con su sucursal
SELECT e.nombre, e.apellidos, e.puesto, s.nombre AS sucursal
FROM Empleado e INNER JOIN Sucursal s ON s.id_sucursal = e.id_sucursal;

-- Q24 · Usuarios de la aplicación con su rol y empleado
SELECT u.nombre_usuario, r.nombre AS rol, e.nombre, e.puesto
FROM Usuario u
INNER JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario
INNER JOIN Rol r         ON r.id_rol = ur.id_rol
INNER JOIN Empleado e    ON e.id_empleado = u.id_empleado;

-- Q25 · Compras con su proveedor
SELECT co.folio, co.fecha, pr.razon_social, co.total
FROM Compra co INNER JOIN Proveedor pr ON pr.id_proveedor = co.id_proveedor;

-- Q26 · Piezas reacondicionadas con producto, número de serie y técnico
SELECT p.nombre, us.numero_serie, r.grado, r.costo_reparacion, e.nombre AS tecnico
FROM Reacondicionado r
INNER JOIN UnidadSerie us ON us.id_unidad = r.id_unidad
INNER JOIN Producto p     ON p.id_producto = us.id_producto
INNER JOIN Empleado e     ON e.id_empleado = r.id_empleado_tecnico;

/* ===================== LEFT JOIN ===================== */

-- Q27 · Clientes sin ninguna venta (LEFT JOIN + IS NULL)
SELECT c.id_cliente, c.nombre, c.apellidos, c.fecha_registro
FROM Cliente c LEFT JOIN Venta v ON v.id_cliente = c.id_cliente
WHERE v.id_venta IS NULL;

-- Q28 · Productos sin ventas
SELECT p.sku, p.nombre, p.stock_actual
FROM Producto p LEFT JOIN DetalleVenta dv ON dv.id_producto = p.id_producto
WHERE dv.id_detalle_venta IS NULL;

-- Q29 · Técnicos con número de órdenes (incluye los que tienen 0)
SELECT e.nombre, e.apellidos, COUNT(o.id_orden) AS ordenes
FROM Empleado e LEFT JOIN OrdenServicio o ON o.id_empleado_tecnico = e.id_empleado
WHERE e.puesto = 'Técnico'
GROUP BY e.id_empleado, e.nombre, e.apellidos;

/* ===================== 5 o más tablas ===================== */

-- Q30 · Detalle completo de ventas: 7 tablas
SELECT v.folio, v.fecha, CONCAT(cl.nombre,' ',cl.apellidos) AS cliente,
       CONCAT(e.nombre,' ',e.apellidos) AS vendedor, s.nombre AS sucursal,
       p.nombre AS producto, c.nombre AS categoria, dv.cantidad, dv.subtotal
FROM Venta v
JOIN Cliente cl      ON cl.id_cliente = v.id_cliente
JOIN Empleado e      ON e.id_empleado = v.id_empleado
JOIN Sucursal s      ON s.id_sucursal = v.id_sucursal
JOIN DetalleVenta dv ON dv.id_venta = v.id_venta
JOIN Producto p      ON p.id_producto = dv.id_producto
JOIN Categoria c     ON c.id_categoria = p.id_categoria
ORDER BY v.fecha;

-- Q31 · Garantías vigentes con cliente, producto y número de serie: 6 tablas
SELECT CONCAT(cl.nombre,' ',cl.apellidos) AS cliente, p.nombre AS producto,
       us.numero_serie, g.fecha_fin, v.folio
FROM Garantia g
JOIN DetalleVenta dv ON dv.id_detalle_venta = g.id_detalle_venta
JOIN Venta v         ON v.id_venta = dv.id_venta
JOIN Cliente cl      ON cl.id_cliente = v.id_cliente
JOIN Producto p      ON p.id_producto = dv.id_producto
JOIN UnidadSerie us  ON us.id_unidad = dv.id_unidad
WHERE g.fecha_fin >= CAST(GETDATE() AS DATE);

/* ===================== Subconsultas normales ===================== */

-- Q32 · Productos con precio mayor al promedio
SELECT sku, nombre, precio_venta FROM Producto
WHERE precio_venta > (SELECT AVG(precio_venta) FROM Producto)
ORDER BY precio_venta DESC;

-- Q33 · Clientes que compraron alguna laptop (subconsulta con IN)
SELECT nombre, apellidos FROM Cliente
WHERE id_cliente IN (SELECT v.id_cliente FROM Venta v
                     JOIN DetalleVenta dv ON dv.id_venta = v.id_venta
                     JOIN Producto p ON p.id_producto = dv.id_producto
                     WHERE p.id_categoria = 2);

/* ===================== Subconsultas correlacionadas ===================== */

-- Q34 · Venta más alta de cada cliente (la subconsulta depende de la fila externa)
SELECT v.id_cliente, v.folio, v.total
FROM Venta v
WHERE v.total = (SELECT MAX(v2.total) FROM Venta v2 WHERE v2.id_cliente = v.id_cliente);

-- Q35 · Productos más caros que el promedio de su propia categoría
SELECT p.sku, p.nombre, p.precio_venta, p.id_categoria
FROM Producto p
WHERE p.precio_venta > (SELECT AVG(p2.precio_venta) FROM Producto p2 WHERE p2.id_categoria = p.id_categoria);

/* ===================== Operadores SET ===================== */

-- Q36 · UNION: directorio de contactos (clientes y proveedores) sin duplicados
SELECT CONCAT(nombre, ' ', apellidos) AS contacto, correo, 'Cliente' AS tipo FROM Cliente
UNION
SELECT razon_social, correo, 'Proveedor' FROM Proveedor WHERE correo IS NOT NULL
ORDER BY tipo, contacto;

-- Q37 · UNION ALL: movimientos de inventario (entradas y salidas), conservando repetidos
SELECT 'Entrada' AS tipo, co.fecha, dc.id_producto, dc.cantidad
FROM DetalleCompra dc JOIN Compra co ON co.id_compra = dc.id_compra
UNION ALL
SELECT 'Salida', v.fecha, dv.id_producto, -dv.cantidad
FROM DetalleVenta dv JOIN Venta v ON v.id_venta = dv.id_venta WHERE v.estado = 'Completada'
ORDER BY fecha;

-- Q38 · INTERSECT: clientes que compraron Y también llevaron un equipo al taller
SELECT id_cliente FROM Venta
INTERSECT
SELECT id_cliente FROM OrdenServicio;

-- Q39 · EXCEPT: productos que se compraron a proveedor pero nunca se han vendido
SELECT id_producto FROM DetalleCompra
EXCEPT
SELECT id_producto FROM DetalleVenta;

/* ===================== Paginación ===================== */

-- Q40 · Página 2 del historial de ventas, 10 por página (como lo hará la app)
DECLARE @pagina INT = 2, @tamano INT = 10;
SELECT folio, fecha, total
FROM Venta
ORDER BY fecha DESC
OFFSET (@pagina - 1) * @tamano ROWS FETCH NEXT @tamano ROWS ONLY;
GO
