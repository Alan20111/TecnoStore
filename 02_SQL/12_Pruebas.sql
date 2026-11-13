/* =============================================================
   TecnoStore · 12_Pruebas.sql
   32 casos de prueba automáticos (sección 17). Cada caso guarda
   entrada, resultado esperado, resultado obtenido y si pasó.
   Ejecutar sobre una base recién creada (01 → 10, 07); modifica datos.

   Tipo                   Casos
   Operación correcta     P01–P10, P27, P29
   Validación             P11–P17, P28, P30, P32
   Transacción fallida    P18–P20
   Seguridad              P21–P22, P31
   Trigger / auditoría    P23–P26
   ============================================================= */
USE TecnoStore;
GO
SET NOCOUNT ON;
IF OBJECT_ID('tempdb..#Prueba') IS NOT NULL DROP TABLE #Prueba;
CREATE TABLE #Prueba (
    caso VARCHAR(4), tipo VARCHAR(20), descripcion VARCHAR(120),
    entrada VARCHAR(200), esperado VARCHAR(200), obtenido VARCHAR(400), paso BIT
);
EXEC sp_set_session_context N'id_usuario', 2;  -- csupervisor
GO

/* ======================= Operaciones correctas ======================= */
DECLARE @id INT;
BEGIN TRY
    EXEC usp_ClienteGuardar @id_cliente = @id OUTPUT, @nombre = 'Mariana', @apellidos = 'Prueba López',
         @correo = 'Mariana.Prueba@correo.mx', @telefono = '5539998877';
    INSERT INTO #Prueba SELECT 'P01','Correcta','Alta de cliente','Mariana Prueba, correo con mayúsculas',
        'Se crea; correo en minúsculas', CONCAT('id=', @id, ' correo=', correo), IIF(correo = 'mariana.prueba@correo.mx', 1, 0)
    FROM Cliente WHERE id_cliente = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P01','Correcta','Alta de cliente','','Se crea',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT = (SELECT id_cliente FROM Cliente WHERE correo = 'mariana.prueba@correo.mx');
BEGIN TRY
    EXEC usp_ClienteGuardar @id_cliente = @id, @nombre = 'Mariana', @apellidos = 'Prueba López',
         @correo = 'mariana.prueba@correo.mx', @telefono = '5500000000', @rfc = 'PULM900101AB1';
    INSERT INTO #Prueba SELECT 'P02','Correcta','Edición de cliente','Nuevo teléfono y RFC',
        'Se actualiza', CONCAT(telefono, ' ', rfc), IIF(telefono = '5500000000' AND rfc = 'PULM900101AB1', 1, 0)
    FROM Cliente WHERE id_cliente = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P02','Correcta','Edición de cliente','','Se actualiza',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT;
BEGIN TRY
    EXEC usp_ProductoGuardar @id_producto = @id OUTPUT, @sku = 'hub-usbc-7', @nombre = 'Hub USB-C 7 en 1',
         @id_categoria = 7, @id_marca = 9, @id_condicion = 1, @precio_venta = 899, @costo = 520, @stock_minimo = 5;
    INSERT INTO #Prueba SELECT 'P03','Correcta','Alta de producto','SKU en minúsculas',
        'Se crea con SKU en mayúsculas y stock 0', CONCAT(sku, ' stock=', stock_actual), IIF(sku = 'HUB-USBC-7' AND stock_actual = 0, 1, 0)
    FROM Producto WHERE id_producto = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P03','Correcta','Alta de producto','','Se crea',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT, @folio VARCHAR(20), @stock INT = (SELECT stock_actual FROM Producto WHERE id_producto = 22);
BEGIN TRY
    EXEC usp_RegistrarVenta 2, 3, 1, N'[{"id_producto":22,"cantidad":3}]', 0, NULL, @id OUTPUT, @folio OUTPUT;
    INSERT INTO #Prueba SELECT 'P04','Correcta','Venta de 3 cargadores','cliente 2, producto 22 x3',
        'Folio nuevo, total 1347, stock -3', CONCAT(@folio, ' total=', v.total, ' stock=', p.stock_actual),
        IIF(v.total = 1347 AND p.stock_actual = @stock - 3, 1, 0)
    FROM Venta v CROSS JOIN Producto p WHERE v.id_venta = @id AND p.id_producto = 22;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P04','Correcta','Venta','','Se registra',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT, @folio VARCHAR(20);
BEGIN TRY
    EXEC usp_RegistrarCompra 5, 9, N'[{"id_producto":17,"cantidad":2,"costo_unitario":6900,"series":["APW-T-01","APW-T-02"]}]', @id OUTPUT, @folio OUTPUT;
    INSERT INTO #Prueba SELECT 'P05','Correcta','Compra con números de serie','2 Apple Watch',
        '2 piezas Disponibles y costo actualizado', CONCAT(@folio, ' piezas=', COUNT(*)),
        IIF(COUNT(*) = 2 AND (SELECT costo FROM Producto WHERE id_producto = 17) = 6900, 1, 0)
    FROM UnidadSerie WHERE numero_serie LIKE 'APW-T-0_' AND estado = 'Disponible';
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P05','Correcta','Compra','','Se registra',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT, @folio VARCHAR(20);
BEGIN TRY
    EXEC usp_RegistrarOrdenServicio 4, 6, 'MacBook Air 2022', 'No enciende', 'C02TEST123', @id OUTPUT, @folio OUTPUT;
    INSERT INTO #Prueba SELECT 'P06','Correcta','Recepción de equipo en taller','cliente 4, técnica Sofía',
        'Orden Recibida, total 250 (diagnóstico), 1 historial', CONCAT(@folio, ' total=', total, ' hist=',
        (SELECT COUNT(*) FROM HistorialEstadoOrden h WHERE h.id_orden = @id)),
        IIF(total = 250 AND id_estado_orden = 1 AND (SELECT COUNT(*) FROM HistorialEstadoOrden h WHERE h.id_orden = @id) = 1, 1, 0)
    FROM OrdenServicio WHERE id_orden = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P06','Correcta','Orden de servicio','','Se registra',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT = (SELECT id_orden FROM OrdenServicio WHERE numero_serie_equipo = 'C02TEST123');
DECLARE @stock INT = (SELECT stock_actual FROM Producto WHERE id_producto = 30);
BEGIN TRY
    EXEC usp_OrdenAgregarLinea @id_orden = @id, @id_tipo_servicio = 8;
    EXEC usp_OrdenAgregarLinea @id_orden = @id, @id_producto = 30;
    INSERT INTO #Prueba SELECT 'P07','Correcta','Agregar mano de obra y refacción','cambio de teclado + teclado MBA',
        'Total 250+700+1999=2949, refacción -1', CONCAT('total=', total, ' stock=', (SELECT stock_actual FROM Producto WHERE id_producto = 30)),
        IIF(total = 2949 AND (SELECT stock_actual FROM Producto WHERE id_producto = 30) = @stock - 1, 1, 0)
    FROM OrdenServicio WHERE id_orden = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P07','Correcta','Agregar líneas','','Se agregan',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT = (SELECT id_orden FROM OrdenServicio WHERE numero_serie_equipo = 'C02TEST123');
BEGIN TRY
    EXEC usp_CambiarEstadoOrden @id, 2;
    INSERT INTO #Prueba SELECT 'P08','Correcta','Avanzar orden a En diagnóstico','Recibida -> En diagnóstico',
        'Estado 2 y historial con usuario 2', CONCAT('estado=', o.id_estado_orden, ' usuario=', h.id_usuario),
        IIF(o.id_estado_orden = 2 AND h.id_usuario = 2, 1, 0)
    FROM OrdenServicio o CROSS APPLY (SELECT TOP 1 id_usuario FROM HistorialEstadoOrden WHERE id_orden = o.id_orden ORDER BY id_historial DESC) h
    WHERE o.id_orden = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P08','Correcta','Avanzar orden','','Avanza',ERROR_MESSAGE(),0); END CATCH
GO

BEGIN TRY
    DECLARE @r TABLE (mes VARCHAR(7), ventas INT, ingresos DECIMAL(12,2), ticket DECIMAL(12,2));
    INSERT INTO @r EXEC usp_ResumenPeriodo '2026-05-01', '2026-10-31';
    INSERT INTO #Prueba SELECT 'P09','Correcta','Reporte resumen por periodo','mayo–octubre 2026',
        '6 meses con ventas', CONCAT(COUNT(*), ' meses'), IIF(COUNT(*) = 6, 1, 0) FROM @r;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P09','Correcta','Reporte','','6 meses',ERROR_MESSAGE(),0); END CATCH
GO

DECLARE @id INT = (SELECT MAX(id_venta) FROM Venta), @stock INT = (SELECT stock_actual FROM Producto WHERE id_producto = 22);
BEGIN TRY
    EXEC usp_CancelarVenta @id, 'Prueba P10';
    INSERT INTO #Prueba SELECT 'P10','Correcta','Cancelar venta reciente','venta de P04',
        'Cancelada y stock +3', CONCAT(v.estado, ' stock=', p.stock_actual), IIF(v.estado = 'Cancelada' AND p.stock_actual = @stock + 3, 1, 0)
    FROM Venta v CROSS JOIN Producto p WHERE v.id_venta = @id AND p.id_producto = 22;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P10','Correcta','Cancelar venta','','Se cancela',ERROR_MESSAGE(),0); END CATCH
GO

/* ============================ Validaciones ============================ */
-- Plantilla: se ejecuta algo que DEBE fallar; pasa si el mensaje es el esperado.
BEGIN TRY
    EXEC usp_ClienteGuardar @nombre = 'Otro', @apellidos = 'Cliente', @correo = 'alejandro.gomez@correo.mx';
    INSERT INTO #Prueba VALUES ('P11','Validación','Correo duplicado','alejandro.gomez@correo.mx','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P11','Validación','Correo duplicado','alejandro.gomez@correo.mx','Ya existe un cliente con ese correo',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50122, 1, 0));
END CATCH
GO
BEGIN TRY
    EXEC usp_ProductoGuardar @sku = 'IPH-15-128', @nombre = 'Duplicado', @id_categoria = 1, @id_marca = 1, @id_condicion = 1, @precio_venta = 100, @costo = 50;
    INSERT INTO #Prueba VALUES ('P12','Validación','SKU duplicado','IPH-15-128','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P12','Validación','SKU duplicado','IPH-15-128','Ya existe otro producto con ese SKU',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50112, 1, 0));
END CATCH
GO
BEGIN TRY
    UPDATE Producto SET precio_venta = 10 WHERE id_producto = 1;
    INSERT INTO #Prueba VALUES ('P13','Validación','Precio menor al costo (directo en tabla)','precio 10, costo 14500','CHECK lo rechaza','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P13','Validación','Precio menor al costo (directo en tabla)','precio 10, costo 14500','CK_Producto_precio_vs_costo',LEFT(ERROR_MESSAGE(),400),IIF(ERROR_MESSAGE() LIKE '%CK_Producto_precio_vs_costo%', 1, 0));
END CATCH
GO
BEGIN TRY
    EXEC usp_RegistrarVenta 2, 3, 1, N'[{"id_producto":22,"cantidad":0}]';
    INSERT INTO #Prueba VALUES ('P14','Validación','Cantidad cero','cantidad 0','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P14','Validación','Cantidad cero','cantidad 0','Cantidades mayores a cero',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50002, 1, 0));
END CATCH
GO
BEGIN TRY
    EXEC usp_RegistrarVenta 2, 3, 1, N'[{"id_producto":22,"cantidad":1}]', 25, 3;  -- autoriza el operador
    INSERT INTO #Prueba VALUES ('P15','Validación','Descuento 25 % autorizado por operador','descuento 25, usuario 3','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P15','Validación','Descuento 25 % autorizado por operador','descuento 25, usuario 3','Requiere Supervisor',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50007, 1, 0));
END CATCH
GO
DECLARE @id INT = (SELECT id_orden FROM OrdenServicio WHERE numero_serie_equipo = 'C02TEST123');
BEGIN TRY
    EXEC usp_CambiarEstadoOrden @id, 5;   -- de En diagnóstico directo a Entregada
    INSERT INTO #Prueba VALUES ('P16','Validación','Saltar estados de la orden','En diagnóstico -> Entregada','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P16','Validación','Saltar estados de la orden','En diagnóstico -> Entregada','Transición no permitida',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50322, 1, 0));
END CATCH
GO
DECLARE @id INT, @f VARCHAR(20);
EXEC usp_RegistrarOrdenServicio 5, 11, 'iPhone 12', 'No prende', NULL, @id OUTPUT, @f OUTPUT;
BEGIN TRY
    EXEC usp_OrdenAgregarLinea @id_orden = @id, @id_tipo_servicio = 7;  -- Héctor no hace placa
    INSERT INTO #Prueba VALUES ('P17','Validación','Técnico no habilitado (N:M)','Héctor + reparación de placa','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P17','Validación','Técnico no habilitado (N:M)','Héctor + reparación de placa','No habilitado',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50314, 1, 0));
END CATCH
GO

/* ======================== Transacciones fallidas ======================== */
DECLARE @v INT = (SELECT COUNT(*) FROM Venta), @d INT = (SELECT COUNT(*) FROM DetalleVenta), @s INT = (SELECT stock_actual FROM Producto WHERE id_producto = 22);
BEGIN TRY
    EXEC usp_RegistrarVenta 2, 3, 1, N'[{"id_producto":22,"cantidad":1},{"id_producto":29,"cantidad":999}]';
END TRY BEGIN CATCH END CATCH
INSERT INTO #Prueba SELECT 'P18','Transacción','Venta con una línea sin stock','cargador x1 + pantalla x999',
    'ROLLBACK: sin venta, sin detalle, stock igual',
    CONCAT('ventas=', (SELECT COUNT(*) FROM Venta) - @v, ' detalles=', (SELECT COUNT(*) FROM DetalleVenta) - @d, ' stock=', stock_actual),
    IIF((SELECT COUNT(*) FROM Venta) = @v AND (SELECT COUNT(*) FROM DetalleVenta) = @d AND stock_actual = @s, 1, 0)
FROM Producto WHERE id_producto = 22;
GO
DECLARE @c INT = (SELECT COUNT(*) FROM Compra), @s INT = (SELECT stock_actual FROM Producto WHERE id_producto = 26);
BEGIN TRY
    EXEC usp_RegistrarCompra 5, 9, N'[{"id_producto":26,"cantidad":10,"costo_unitario":100},{"id_producto":17,"cantidad":1,"costo_unitario":6900,"series":["APW-T-01"]}]';
END TRY BEGIN CATCH END CATCH
INSERT INTO #Prueba SELECT 'P19','Transacción','Compra con número de serie repetido','cables x10 + serie APW-T-01 (ya existe)',
    'ROLLBACK: sin compra y cables sin entrar', CONCAT('compras=', (SELECT COUNT(*) FROM Compra) - @c, ' stock=', stock_actual),
    IIF((SELECT COUNT(*) FROM Compra) = @c AND stock_actual = @s, 1, 0)
FROM Producto WHERE id_producto = 26;
GO
DECLARE @id INT = (SELECT id_orden FROM OrdenServicio WHERE numero_serie_equipo = 'C02TEST123');
DECLARE @l INT = (SELECT COUNT(*) FROM DetalleOrdenServicio WHERE id_orden = @id), @t DECIMAL(12,2) = (SELECT total FROM OrdenServicio WHERE id_orden = @id);
BEGIN TRY
    EXEC usp_OrdenAgregarLinea @id_orden = @id, @id_producto = 29, @cantidad = 999;
END TRY BEGIN CATCH END CATCH
INSERT INTO #Prueba SELECT 'P20','Transacción','Refacción sin stock en orden','pantalla x999',
    'ROLLBACK: sin línea nueva y total igual', CONCAT('lineas=', COUNT(*) - @l, ' total=', (SELECT total FROM OrdenServicio WHERE id_orden = @id)),
    IIF(COUNT(*) = @l AND (SELECT total FROM OrdenServicio WHERE id_orden = @id) = @t, 1, 0)
FROM DetalleOrdenServicio WHERE id_orden = @id;
GO

/* ============================== Seguridad ============================== */
EXECUTE AS USER = 'tecno_consulta';
BEGIN TRY
    EXEC usp_RegistrarVenta 2, 3, 1, N'[{"id_producto":22,"cantidad":1}]';
    REVERT;
    INSERT INTO #Prueba VALUES ('P21','Seguridad','Rol consulta intenta vender','EXECUTE AS tecno_consulta','Permiso denegado','Se guardó',0);
END TRY BEGIN CATCH
    DECLARE @m VARCHAR(400) = ERROR_MESSAGE(), @n INT = ERROR_NUMBER();
    REVERT;
    INSERT INTO #Prueba VALUES ('P21','Seguridad','Rol consulta intenta vender','EXECUTE AS tecno_consulta','Permiso EXECUTE denegado',@m,IIF(@n = 229, 1, 0));
END CATCH
GO
EXECUTE AS USER = 'tecno_operador';
BEGIN TRY
    DECLARE @x INT = (SELECT COUNT(*) FROM Auditoria);
    REVERT;
    INSERT INTO #Prueba VALUES ('P22','Seguridad','Rol operador lee la auditoría','EXECUTE AS tecno_operador','Permiso denegado','Pudo leer',0);
END TRY BEGIN CATCH
    DECLARE @m VARCHAR(400) = ERROR_MESSAGE(), @n INT = ERROR_NUMBER();
    REVERT;
    INSERT INTO #Prueba VALUES ('P22','Seguridad','Rol operador lee la auditoría','EXECUTE AS tecno_operador','Permiso SELECT denegado',@m,IIF(@n = 229, 1, 0));
END CATCH
GO

/* =========================== Triggers / auditoría =========================== */
DECLARE @a INT = (SELECT ISNULL(MAX(id_auditoria), 0) FROM Auditoria);
BEGIN TRY
    EXEC usp_AjustarPrecio @porcentaje = 10, @id_usuario = 2, @id_categoria = 6;   -- audífonos
    INSERT INTO #Prueba SELECT 'P23','Trigger','Cambio de precio de una categoría queda auditado','+10 % a Audífonos (3 productos)',
        '3 filas en Auditoria con usuario_app csupervisor', CONCAT(COUNT(*), ' filas, usuario_app=', MAX(usuario_app)),
        IIF(COUNT(*) = 3 AND MAX(usuario_app) = 'csupervisor', 1, 0)
    FROM Auditoria WHERE id_auditoria > @a AND valor_nuevo LIKE 'precio_venta=%';
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P23','Trigger','Auditoría de precio','','3 filas',ERROR_MESSAGE(),0); END CATCH
GO
BEGIN TRY
    UPDATE OrdenServicio SET id_estado_orden = 3 WHERE folio = 'OS-3001';   -- ya Entregada
    INSERT INTO #Prueba VALUES ('P24','Trigger','Cambiar una orden Entregada (directo en tabla)','OS-3001 -> En reparación','Rechazo del trigger','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P24','Trigger','Cambiar una orden Entregada (directo en tabla)','OS-3001 -> En reparación','trg_OrdenServicio_Estado lo impide',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50040, 1, 0));
END CATCH
GO
BEGIN TRY
    DELETE FROM Producto WHERE sku IN ('IPH-15-128', 'HUB-USBC-7');
    INSERT INTO #Prueba SELECT 'P25','Trigger','DELETE de producto con historial y sin historial','IPH-15-128 y HUB-USBC-7',
        'IPH-15-128 queda inactivo; HUB-USBC-7 se borra',
        CONCAT('IPH activo=', (SELECT activo FROM Producto WHERE sku = 'IPH-15-128'), ' HUB existe=', (SELECT COUNT(*) FROM Producto WHERE sku = 'HUB-USBC-7')),
        IIF((SELECT activo FROM Producto WHERE sku = 'IPH-15-128') = 0 AND NOT EXISTS (SELECT 1 FROM Producto WHERE sku = 'HUB-USBC-7'), 1, 0);
    UPDATE Producto SET activo = 1 WHERE sku = 'IPH-15-128';
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P25','Trigger','Baja lógica','','Inactivo',ERROR_MESSAGE(),0); END CATCH
GO
BEGIN TRY
    DELETE FROM Auditoria;
    INSERT INTO #Prueba VALUES ('P26','Trigger','Borrar la auditoría (como sa)','DELETE FROM Auditoria','Rechazo','Se borró',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P26','Trigger','Borrar la auditoría (como sa)','DELETE FROM Auditoria','Solo lectura',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50041, 1, 0));
END CATCH
GO

/* ==================== Catálogos, proveedores y usuarios ==================== */
DECLARE @id INT;
BEGIN TRY
    EXEC usp_ProveedorGuardar @id_proveedor = @id OUTPUT, @razon_social = 'Mayorista Nuevo SA de CV', @rfc = 'mns2001019z9', @correo = 'Ventas@MNS.example.mx';
    INSERT INTO #Prueba SELECT 'P27','Correcta','Alta de proveedor','RFC y correo en minúsculas/mayúsculas',
        'RFC en mayúsculas, correo en minúsculas', CONCAT(rfc, ' ', correo), IIF(rfc = 'MNS2001019Z9' AND correo = 'ventas@mns.example.mx', 1, 0)
    FROM Proveedor WHERE id_proveedor = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P27','Correcta','Alta de proveedor','','Se crea',ERROR_MESSAGE(),0); END CATCH
GO
BEGIN TRY
    EXEC usp_ProveedorGuardar @razon_social = 'Duplicado', @rfc = 'IMM9501018A1';
    INSERT INTO #Prueba VALUES ('P28','Validación','RFC de proveedor duplicado','IMM9501018A1','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P28','Validación','RFC de proveedor duplicado','IMM9501018A1','Ya existe un proveedor con ese RFC',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50133, 1, 0));
END CATCH
GO
DECLARE @id INT;
BEGIN TRY
    EXEC usp_UsuarioGuardar @id_usuario = @id OUTPUT, @id_empleado = 4, @nombre_usuario = 'MTorres', @id_rol = 3,
         @contrasena = 'Tienda2026', @id_admin = 1;
    INSERT INTO #Prueba SELECT 'P29','Correcta','Alta de usuario con rol (Usuario + UsuarioRol)','mtorres, Operador',
        'Usuario en minúsculas, rol Operador y la contraseña sirve para entrar',
        CONCAT(u.nombre_usuario, ' rol=', r.nombre), IIF(u.nombre_usuario = 'mtorres' AND r.nombre = 'Operador'
            AND u.hash_contrasena = HASHBYTES('SHA2_256', 'Tienda2026'), 1, 0)
    FROM Usuario u JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario JOIN Rol r ON r.id_rol = ur.id_rol
    WHERE u.id_usuario = @id;
END TRY BEGIN CATCH INSERT INTO #Prueba VALUES ('P29','Correcta','Alta de usuario','','Se crea',ERROR_MESSAGE(),0); END CATCH
GO
BEGIN TRY
    EXEC usp_UsuarioGuardar @id_empleado = 5, @nombre_usuario = 'jcastillo', @id_rol = 3, @contrasena = 'corta', @id_admin = 1;
    INSERT INTO #Prueba VALUES ('P30','Validación','Contraseña débil','"corta"','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P30','Validación','Contraseña débil','"corta"','Mínimo 8 caracteres y un número',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50143, 1, 0));
END CATCH
GO
BEGIN TRY
    EXEC usp_UsuarioGuardar @id_empleado = 5, @nombre_usuario = 'jcastillo', @id_rol = 1, @contrasena = 'Tienda2026', @id_admin = 2;
    INSERT INTO #Prueba VALUES ('P31','Seguridad','Supervisor intenta crear un Administrador','id_admin = csupervisor','Rechazo','Se guardó',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P31','Seguridad','Supervisor intenta crear un Administrador','id_admin = csupervisor','Solo un Administrador',ERROR_MESSAGE(),IIF(ERROR_NUMBER() = 50140, 1, 0));
END CATCH
GO
BEGIN TRY
    DELETE FROM Categoria WHERE nombre = 'Celulares';
    INSERT INTO #Prueba VALUES ('P32','Validación','Borrar categoría con productos','DELETE Categoria Celulares','La FK lo impide','Se borró',0);
END TRY BEGIN CATCH
    INSERT INTO #Prueba VALUES ('P32','Validación','Borrar categoría con productos','DELETE Categoria Celulares','Conflicto con FK_Producto_Categoria',LEFT(ERROR_MESSAGE(),400),IIF(ERROR_NUMBER() = 547, 1, 0));
END CATCH
GO

EXEC sp_set_session_context N'id_usuario', NULL;
SELECT caso, tipo, descripcion, entrada, esperado, obtenido, IIF(paso = 1, 'PASA', 'FALLA') AS resultado FROM #Prueba ORDER BY caso;
SELECT SUM(CAST(paso AS INT)) AS pasan, COUNT(*) AS total FROM #Prueba;
GO
