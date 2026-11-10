/* =============================================================
   TecnoStore · 11_Transacciones.sql
   Demostración de los 4 procesos transaccionales (sección 9):
   cada uno con una ejecución exitosa y una fallida que termina en
   ROLLBACK total. Ejecutar sobre una base recién creada.

   Proceso                      SP                    Tablas que modifica
   1 Principal: venta           usp_RegistrarVenta    Venta, DetalleVenta, Producto, UnidadSerie, Garantia
   2 Secundario: compra         usp_RegistrarCompra   Compra, DetalleCompra, Producto, UnidadSerie, ProductoProveedor
   3 Cancelación                usp_CancelarVenta     Venta, Producto, UnidadSerie, Garantia
   4 Ajuste                     usp_AjustarStock      Producto, Auditoria (trigger)
   Más una transacción escrita a mano (sin SP) al final: baja de pieza dañada.

   Por qué hace falta la transacción (ejemplo del proceso 1): si el
   detalle se guardara pero fallara el descuento de stock, quedaría una
   venta cobrada con inventario falso; si se marcara la pieza como
   Vendida y fallara la garantía, el cliente no tendría cobertura.
   Con BEGIN TRAN / COMMIT / ROLLBACK o pasa todo o no pasa nada.

   Aislamiento: los SP usan el nivel por defecto READ COMMITTED (evita
   lecturas sucias) y UPDLOCK/HOLDLOCK en las filas de stock y folios,
   para que dos cajas al mismo tiempo no vendan la misma pieza ni
   generen el mismo folio (evita la lectura no repetible y el fantasma
   justo en las filas que importan, sin bloquear toda la tabla como
   haría SERIALIZABLE).
   ============================================================= */
USE TecnoStore;
GO
SET NOCOUNT ON;
EXEC sp_set_session_context N'id_usuario', 2;   -- csupervisor
GO

/* ===================== Proceso 1: venta ===================== */
PRINT '=== Proceso 1 · venta correcta';
DECLARE @id INT, @folio VARCHAR(20);
DECLARE @stock_antes INT = (SELECT stock_actual FROM Producto WHERE id_producto = 3);
EXEC usp_RegistrarVenta @id_cliente = 1, @id_empleado = 3, @id_metodo_pago = 2,
     @lineas = N'[{"id_producto":3,"cantidad":1},{"id_producto":22,"cantidad":2}]',
     @id_venta = @id OUTPUT, @folio = @folio OUTPUT;
SELECT @folio AS folio, total, lineas FROM vw_VentaCompleta WHERE id_venta = @id;
SELECT @stock_antes AS stock_antes, stock_actual AS stock_despues FROM Producto WHERE id_producto = 3;
SELECT dv.id_detalle_venta, us.numero_serie, us.estado, g.fecha_fin AS garantia_hasta
FROM DetalleVenta dv JOIN UnidadSerie us ON us.id_unidad = dv.id_unidad
LEFT JOIN Garantia g ON g.id_detalle_venta = dv.id_detalle_venta
WHERE dv.id_venta = @id;
GO

PRINT '=== Proceso 1 · venta fallida: la segunda línea no tiene stock';
DECLARE @ventas INT = (SELECT COUNT(*) FROM Venta), @detalles INT = (SELECT COUNT(*) FROM DetalleVenta),
        @stock22 INT = (SELECT stock_actual FROM Producto WHERE id_producto = 22);
BEGIN TRY
    EXEC usp_RegistrarVenta 1, 3, 2, N'[{"id_producto":22,"cantidad":1},{"id_producto":29,"cantidad":500}]';
END TRY
BEGIN CATCH
    PRINT CONCAT('Error capturado: ', ERROR_MESSAGE());
END CATCH
SELECT CASE WHEN COUNT(*) = @ventas THEN 'OK: sin encabezado nuevo' ELSE 'FALLO' END AS venta FROM Venta;
SELECT CASE WHEN COUNT(*) = @detalles THEN 'OK: sin detalle parcial' ELSE 'FALLO' END AS detalle FROM DetalleVenta;
SELECT CASE WHEN stock_actual = @stock22 THEN 'OK: stock intacto' ELSE 'FALLO' END AS stock FROM Producto WHERE id_producto = 22;
GO

/* ===================== Proceso 2: compra ===================== */
PRINT '=== Proceso 2 · compra correcta (con números de serie)';
DECLARE @id INT, @folio VARCHAR(20);
EXEC usp_RegistrarCompra @id_proveedor = 3, @id_empleado = 9,
     @lineas = N'[{"id_producto":1,"cantidad":2,"costo_unitario":14400,"series":["IPH15-NEW-001","IPH15-NEW-002"]},
                  {"id_producto":28,"cantidad":10,"costo_unitario":740}]',
     @id_compra = @id OUTPUT, @folio = @folio OUTPUT;
SELECT @folio AS folio, total FROM Compra WHERE id_compra = @id;
SELECT numero_serie, estado FROM UnidadSerie WHERE numero_serie LIKE 'IPH15-NEW-%';
GO

PRINT '=== Proceso 2 · compra fallida: número de serie repetido';
DECLARE @compras INT = (SELECT COUNT(*) FROM Compra), @stock28 INT = (SELECT stock_actual FROM Producto WHERE id_producto = 28);
BEGIN TRY
    EXEC usp_RegistrarCompra 3, 9,
         N'[{"id_producto":28,"cantidad":5,"costo_unitario":740},
            {"id_producto":1,"cantidad":1,"costo_unitario":14400,"series":["IPH15-NEW-001"]}]';
END TRY
BEGIN CATCH
    PRINT CONCAT('Error capturado: ', ERROR_MESSAGE());
END CATCH
SELECT CASE WHEN COUNT(*) = @compras THEN 'OK: sin compra nueva' ELSE 'FALLO' END AS compra FROM Compra;
SELECT CASE WHEN stock_actual = @stock28 THEN 'OK: la refacción no entró' ELSE 'FALLO' END AS stock FROM Producto WHERE id_producto = 28;
GO

/* ===================== Proceso 3: cancelación ===================== */
PRINT '=== Proceso 3 · cancelación correcta de la venta recién hecha';
DECLARE @id INT = (SELECT MAX(id_venta) FROM Venta);
DECLARE @stock_antes INT = (SELECT stock_actual FROM Producto WHERE id_producto = 3);
EXEC usp_CancelarVenta @id_venta = @id, @motivo = 'Prueba de cancelación';
SELECT folio, estado, fecha_cancelacion, motivo_cancelacion FROM Venta WHERE id_venta = @id;
SELECT @stock_antes AS stock_antes, stock_actual AS stock_despues FROM Producto WHERE id_producto = 3;
SELECT us.numero_serie, us.estado,
       (SELECT COUNT(*) FROM Garantia g WHERE g.id_detalle_venta = dv.id_detalle_venta) AS garantias
FROM DetalleVenta dv JOIN UnidadSerie us ON us.id_unidad = dv.id_unidad WHERE dv.id_venta = @id;
GO

PRINT '=== Proceso 3 · cancelación fallida: venta de hace más de 30 días';
BEGIN TRY
    EXEC usp_CancelarVenta @id_venta = 1, @motivo = 'Fuera de plazo';
END TRY
BEGIN CATCH
    PRINT CONCAT('Error capturado: ', ERROR_MESSAGE());
END CATCH
SELECT folio, estado FROM Venta WHERE id_venta = 1;
GO

/* ===================== Proceso 4: ajuste ===================== */
PRINT '=== Proceso 4 · ajuste correcto por conteo físico (queda en auditoría)';
DECLARE @antes INT = (SELECT stock_actual FROM Producto WHERE id_producto = 26);
EXEC usp_AjustarStock @id_producto = 26, @cantidad = -2, @motivo = 'Conteo físico: 2 cables dañados', @id_usuario = 2;
SELECT @antes AS stock_antes, stock_actual AS stock_despues FROM Producto WHERE id_producto = 26;
SELECT TOP 1 usuario_app, motivo, valor_anterior, valor_nuevo FROM Auditoria ORDER BY id_auditoria DESC;
GO

PRINT '=== Proceso 4 · ajuste fallido: el operador no tiene permiso';
BEGIN TRY
    EXEC usp_AjustarStock @id_producto = 26, @cantidad = 5, @motivo = 'Intento', @id_usuario = 3;
END TRY
BEGIN CATCH
    PRINT CONCAT('Error capturado: ', ERROR_MESSAGE());
END CATCH
GO

/* ===================== Transacción escrita a mano =====================
   Baja de una pieza dañada en bodega: la pieza pasa a "Baja", el stock
   baja en 1 y se deja constancia. Muestra todos los elementos de 9.1
   sin esconderlos dentro de un SP. */
PRINT '=== Transacción manual · baja de pieza';
DECLARE @numero_serie VARCHAR(50) = 'MMN-M2-008';
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @id_unidad INT, @id_producto INT, @estado VARCHAR(20);
    SELECT @id_unidad = id_unidad, @id_producto = id_producto, @estado = estado
    FROM UnidadSerie WITH (UPDLOCK) WHERE numero_serie = @numero_serie;

    IF @id_unidad IS NULL        THROW 51000, 'La pieza no existe.', 1;
    IF @estado <> 'Disponible'   THROW 51001, 'Solo se puede dar de baja una pieza Disponible.', 1;

    UPDATE UnidadSerie SET estado = 'Baja' WHERE id_unidad = @id_unidad;
    EXEC sp_set_session_context N'motivo', 'Baja de pieza dañada en bodega';
    UPDATE Producto SET stock_actual = stock_actual - 1 WHERE id_producto = @id_producto;
    EXEC sp_set_session_context N'motivo', NULL;

    COMMIT TRANSACTION;
    PRINT CONCAT('Pieza ', @numero_serie, ' dada de baja.');
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT CONCAT('ROLLBACK: ', ERROR_MESSAGE());
END CATCH
SELECT numero_serie, estado FROM UnidadSerie WHERE numero_serie = 'MMN-M2-008';
GO

PRINT '=== Transacción manual · repetirla falla (la pieza ya está en Baja) y no cambia nada';
DECLARE @stock INT = (SELECT stock_actual FROM Producto WHERE sku = 'MMN-M2');
BEGIN TRY
    BEGIN TRANSACTION;
    IF (SELECT estado FROM UnidadSerie WITH (UPDLOCK) WHERE numero_serie = 'MMN-M2-008') <> 'Disponible'
        THROW 51001, 'Solo se puede dar de baja una pieza Disponible.', 1;
    UPDATE Producto SET stock_actual = stock_actual - 1 WHERE sku = 'MMN-M2';
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT CONCAT('ROLLBACK: ', ERROR_MESSAGE());
END CATCH
SELECT CASE WHEN stock_actual = @stock THEN 'OK: stock intacto' ELSE 'FALLO' END AS resultado FROM Producto WHERE sku = 'MMN-M2';
GO
EXEC sp_set_session_context N'id_usuario', NULL;
GO
