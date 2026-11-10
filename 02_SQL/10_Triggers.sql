/* =============================================================
   TecnoStore · 10_Triggers.sql
   Todos manejan operaciones de varias filas (inserted/deleted
   como conjuntos, sin variables escalares).
   ============================================================= */
USE TecnoStore;
GO

/* Auditoría: cambios de precio y de stock en Producto.
   Está en la base (no en la app) para que cualquier cambio, venga
   de un SP, de la app o de SSMS, quede registrado. */
CREATE OR ALTER TRIGGER trg_Producto_Auditoria
ON Producto
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(precio_venta)
        INSERT INTO Auditoria (usuario, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
        SELECT ORIGINAL_LOGIN(), 'Producto', 'UPDATE', i.id_producto,
               CONCAT('precio_venta=', d.precio_venta), CONCAT('precio_venta=', i.precio_venta)
        FROM inserted i JOIN deleted d ON d.id_producto = i.id_producto
        WHERE i.precio_venta <> d.precio_venta;

    IF UPDATE(stock_actual)
        INSERT INTO Auditoria (usuario, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
        SELECT ORIGINAL_LOGIN(), 'Producto', 'UPDATE', i.id_producto,
               CONCAT('stock_actual=', d.stock_actual), CONCAT('stock_actual=', i.stock_actual)
        FROM inserted i JOIN deleted d ON d.id_producto = i.id_producto
        WHERE i.stock_actual <> d.stock_actual;
END
GO

/* Auditoría: cambios de estado de ventas (cancelaciones). */
CREATE OR ALTER TRIGGER trg_Venta_Auditoria
ON Venta
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Auditoria (usuario, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
    SELECT ORIGINAL_LOGIN(), 'Venta', 'UPDATE', i.id_venta,
           CONCAT('estado=', d.estado, '; total=', d.total), CONCAT('estado=', i.estado, '; total=', i.total)
    FROM inserted i JOIN deleted d ON d.id_venta = i.id_venta
    WHERE i.estado <> d.estado OR i.total <> d.total AND d.total <> 0;
END
GO

/* Regla + automatización: una orden en estado final (Entregada o
   Cancelada) ya no cambia de estado; cada cambio válido se guarda
   en HistorialEstadoOrden. */
CREATE OR ALTER TRIGGER trg_OrdenServicio_Estado
ON OrdenServicio
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(id_estado_orden) RETURN;

    IF EXISTS (SELECT 1 FROM inserted i
               JOIN deleted d      ON d.id_orden = i.id_orden
               JOIN EstadoOrden eo ON eo.id_estado_orden = d.id_estado_orden
               WHERE eo.es_final = 1 AND i.id_estado_orden <> d.id_estado_orden)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50040, 'Una orden Entregada o Cancelada ya no puede cambiar de estado.', 1;
    END

    INSERT INTO HistorialEstadoOrden (id_orden, id_estado_anterior, id_estado_nuevo, id_usuario)
    SELECT i.id_orden, d.id_estado_orden, i.id_estado_orden,
           TRY_CAST(SESSION_CONTEXT(N'id_usuario') AS INT)
    FROM inserted i JOIN deleted d ON d.id_orden = i.id_orden
    WHERE i.id_estado_orden <> d.id_estado_orden;
END
GO

/* Libre justificado: el historial de auditoría no se edita ni se borra. */
CREATE OR ALTER TRIGGER trg_Auditoria_SoloLectura
ON Auditoria
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    THROW 50041, 'La auditoría es de solo lectura.', 1;
END
GO
