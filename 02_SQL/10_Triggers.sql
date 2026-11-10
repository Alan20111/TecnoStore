/* =============================================================
   TecnoStore · 10_Triggers.sql
   5 triggers. Todos manejan operaciones de varias filas
   (inserted/deleted como conjuntos, sin variables escalares).
   usuario_app y motivo salen de SESSION_CONTEXT, que la app y los
   SP llenan; si el cambio se hace desde SSMS quedan en NULL y
   "usuario" guarda el login de SQL Server.
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
    DECLARE @usuario_app VARCHAR(40) = (SELECT nombre_usuario FROM Usuario
                                        WHERE id_usuario = TRY_CAST(SESSION_CONTEXT(N'id_usuario') AS INT));
    DECLARE @motivo VARCHAR(200) = CAST(SESSION_CONTEXT(N'motivo') AS VARCHAR(200));
    IF UPDATE(precio_venta)
        INSERT INTO Auditoria (usuario, usuario_app, motivo, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
        SELECT ORIGINAL_LOGIN(), @usuario_app, @motivo, 'Producto', 'UPDATE', i.id_producto,
               CONCAT('precio_venta=', d.precio_venta), CONCAT('precio_venta=', i.precio_venta)
        FROM inserted i JOIN deleted d ON d.id_producto = i.id_producto
        WHERE i.precio_venta <> d.precio_venta;

    IF UPDATE(stock_actual)
        INSERT INTO Auditoria (usuario, usuario_app, motivo, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
        SELECT ORIGINAL_LOGIN(), @usuario_app, @motivo, 'Producto', 'UPDATE', i.id_producto,
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
    INSERT INTO Auditoria (usuario, usuario_app, motivo, tabla, operacion, id_registro, valor_anterior, valor_nuevo)
    SELECT ORIGINAL_LOGIN(),
           (SELECT nombre_usuario FROM Usuario WHERE id_usuario = TRY_CAST(SESSION_CONTEXT(N'id_usuario') AS INT)),
           i.motivo_cancelacion, 'Venta', 'UPDATE', i.id_venta,
           CONCAT('estado=', d.estado, '; total=', d.total), CONCAT('estado=', i.estado, '; total=', i.total)
    FROM inserted i JOIN deleted d ON d.id_venta = i.id_venta
    WHERE i.estado <> d.estado OR (i.total <> d.total AND d.total <> 0);
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

/* Regla: no se borra información histórica. Un DELETE sobre un producto
   que ya tuvo movimientos se convierte en baja lógica (activo = 0); los
   que nunca se usaron sí se eliminan. Funciona igual desde la app o SSMS. */
CREATE OR ALTER TRIGGER trg_Producto_BajaLogica
ON Producto
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p SET activo = 0
    FROM Producto p JOIN deleted d ON d.id_producto = p.id_producto
    WHERE EXISTS (SELECT 1 FROM DetalleVenta x WHERE x.id_producto = d.id_producto)
       OR EXISTS (SELECT 1 FROM DetalleCompra x WHERE x.id_producto = d.id_producto)
       OR EXISTS (SELECT 1 FROM DetalleOrdenServicio x WHERE x.id_producto = d.id_producto)
       OR EXISTS (SELECT 1 FROM UnidadSerie x WHERE x.id_producto = d.id_producto);

    DELETE pp FROM ProductoProveedor pp JOIN deleted d ON d.id_producto = pp.id_producto
    WHERE NOT EXISTS (SELECT 1 FROM DetalleVenta x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM DetalleCompra x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM DetalleOrdenServicio x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM UnidadSerie x WHERE x.id_producto = d.id_producto);
    DELETE p FROM Producto p JOIN deleted d ON d.id_producto = p.id_producto
    WHERE NOT EXISTS (SELECT 1 FROM DetalleVenta x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM DetalleCompra x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM DetalleOrdenServicio x WHERE x.id_producto = d.id_producto)
      AND NOT EXISTS (SELECT 1 FROM UnidadSerie x WHERE x.id_producto = d.id_producto);
END
GO
