/* =============================================================
   TecnoStore · 04_DatosDemo.sql
   Datos de demostración. Los catálogos y maestros se insertan
   a mano; compras, ventas y órdenes de servicio se generan con
   un generador pseudoaleatorio con semilla fija, así que cada
   ejecución desde cero produce exactamente los mismos datos.

   Casos útiles incluidos a propósito:
   - clientes 26-30 sin compras
   - producto USB-KNG-128 sin ventas, IPH-12-64 inactivo
   - empleada Valeria Cruz inactiva
   - refacciones por debajo del stock mínimo (alertas)
   - ventas canceladas y órdenes canceladas
   - movimientos repartidos de abril a noviembre de 2026

   Usuarios de prueba (contraseña de todos: Tecno123!):
   admin, csupervisor, loperador, fconsulta, smendez
   ============================================================= */
USE TecnoStore;
GO
SET NOCOUNT ON;

/* ---------- Catálogos ---------- */
INSERT INTO Categoria (nombre, descripcion) VALUES
 ('Celulares','Smartphones nuevos y reacondicionados'),
 ('Laptops','Computadoras portátiles'),
 ('Computadoras de escritorio','Equipos de escritorio y all-in-one'),
 ('Tabletas','Tabletas y iPad'),
 ('Relojes inteligentes','Smartwatches'),
 ('Audífonos','Audífonos inalámbricos y alámbricos'),
 ('Accesorios','Cargadores, cables, fundas, periféricos'),
 ('Refacciones','Piezas para el taller'),
 ('Monitores','Pantallas externas'),
 ('Almacenamiento','Discos SSD y memorias');

INSERT INTO Marca (nombre, pais_origen) VALUES
 ('Apple','Estados Unidos'),('Samsung','Corea del Sur'),('Xiaomi','China'),
 ('Lenovo','China'),('HP','Estados Unidos'),('Dell','Estados Unidos'),
 ('Sony','Japón'),('Logitech','Suiza'),('Anker','China'),('Kingston','Estados Unidos');

INSERT INTO Condicion (nombre) VALUES
 ('Nuevo'),('Reacondicionado'),('Open box'),('Usado'),('Exhibición');

INSERT INTO MetodoPago (nombre) VALUES
 ('Efectivo'),('Tarjeta de débito'),('Tarjeta de crédito'),
 ('Transferencia'),('Meses sin intereses'),('Vale de tienda');

INSERT INTO TipoServicio (nombre, precio_base) VALUES
 ('Diagnóstico',250),('Limpieza interna',450),('Cambio de batería',600),
 ('Cambio de pantalla',900),('Formateo e instalación',500),('Respaldo de información',350),
 ('Reparación de placa',1800),('Cambio de teclado',700),('Cambio de puerto de carga',550),
 ('Actualización de SSD/RAM',400);

INSERT INTO EstadoOrden (nombre, es_final) VALUES
 ('Recibida',0),('En diagnóstico',0),('En reparación',0),('Lista',0),('Entregada',1),('Cancelada',1);

/* ---------- Maestros ---------- */
INSERT INTO Sucursal (nombre, direccion, telefono) VALUES
 ('TecnoStore Centro','Av. Juárez 120, Centro, CDMX','5551000001'),
 ('TecnoStore Polanco','Av. Presidente Masaryk 340, Polanco, CDMX','5551000002'),
 ('TecnoStore Satélite','Circuito Centro Comercial 2251, Naucalpan, Edo. Méx.','5551000003');

INSERT INTO Empleado (id_sucursal, nombre, apellidos, puesto, fecha_contratacion, activo) VALUES
 (1,'Ana','López García','Gerente','2021-02-15',1),
 (1,'Carlos','Ramírez Soto','Supervisor','2022-01-10',1),
 (1,'Luis','Hernández Ríos','Vendedor','2023-03-01',1),
 (2,'María','Torres Peña','Vendedor','2023-06-12',1),
 (3,'Jorge','Castillo Mora','Vendedor','2024-01-08',1),
 (1,'Sofía','Méndez Lara','Técnico','2022-08-22',1),
 (2,'Diego','Flores Ibarra','Técnico','2023-09-04',1),
 (2,'Paola','Ruiz Campos','Vendedor','2024-05-20',1),
 (1,'Ricardo','Vega Núñez','Almacenista','2022-11-30',1),
 (1,'Fernanda','Ortiz Salas','Analista','2025-02-03',1),
 (3,'Héctor','Juárez Pineda','Técnico','2024-07-15',1),
 (3,'Valeria','Cruz Delgado','Vendedor','2023-10-01',0);

INSERT INTO Proveedor (razon_social, rfc, correo, telefono) VALUES
 ('Distribuidora Ingram Micro México','IMM9501018A1','ventas@ingram.example.mx','5552000001'),
 ('CVA Comercializadora','CVA0203045B2','pedidos@cva.example.mx','5552000002'),
 ('CT Internacional del Noroeste','CTI8806127C3','compras@ct.example.mx','5552000003'),
 ('Exel del Norte','EXN9109308D4','ventas@exel.example.mx','5552000004'),
 ('PCH Mayoreo','PCH0011229E5','contacto@pch.example.mx','5552000005'),
 ('Grupo Daisytek','GDA9704151F6','ventas@daisytek.example.mx','5552000006'),
 ('Tecnosinergia','TSI1205178G7','pedidos@tecnosinergia.example.mx','5552000007'),
 ('Refacciones Móviles del Centro','RMC1508266H8','ventas@refmoviles.example.mx','5552000008'),
 ('Importadora Pacífico Tech','IPT1702139J9','import@pacificotech.example.mx','5552000009'),
 ('Mayoreo Digital Bajío','MDB1904057K1','ventas@mdbajio.example.mx','5552000010');

INSERT INTO Cliente (nombre, apellidos, correo, telefono, fecha_registro) VALUES
 ('Alejandro','Gómez Ruiz','alejandro.gomez@correo.mx','5530000001','2026-01-05'),
 ('Beatriz','Navarro León','beatriz.navarro@correo.mx','5530000002','2026-01-12'),
 ('César','Domínguez Paz','cesar.dominguez@correo.mx','5530000003','2026-01-20'),
 ('Daniela','Herrera Villa','daniela.herrera@correo.mx','5530000004','2026-02-02'),
 ('Eduardo','Sánchez Mejía','eduardo.sanchez@correo.mx',NULL,'2026-02-09'),
 ('Fabiola','Ríos Cárdenas','fabiola.rios@correo.mx','5530000006','2026-02-14'),
 ('Gerardo','Medina Ochoa','gerardo.medina@correo.mx','5530000007','2026-02-21'),
 ('Hilda','Aguilar Fuentes','hilda.aguilar@correo.mx','5530000008','2026-03-01'),
 ('Iván','Morales Rangel','ivan.morales@correo.mx','5530000009','2026-03-07'),
 ('Jimena','Castro Bravo','jimena.castro@correo.mx','5530000010','2026-03-15'),
 ('Kevin','Romero Lugo','kevin.romero@correo.mx',NULL,'2026-03-22'),
 ('Laura','Jiménez Arce','laura.jimenez@correo.mx','5530000012','2026-03-29'),
 ('Mario','Vargas Peralta','mario.vargas@correo.mx','5530000013','2026-04-04'),
 ('Natalia','Reyes Solís','natalia.reyes@correo.mx','5530000014','2026-04-11'),
 ('Óscar','Guzmán Tapia','oscar.guzman@correo.mx','5530000015','2026-04-18'),
 ('Patricia','Mendoza Cano','patricia.mendoza@correo.mx','5530000016','2026-04-25'),
 ('Quetzal','Ortega Nava','quetzal.ortega@correo.mx',NULL,'2026-05-02'),
 ('Rocío','Silva Ponce','rocio.silva@correo.mx','5530000018','2026-05-09'),
 ('Samuel','Delgado Rosas','samuel.delgado@correo.mx','5530000019','2026-05-16'),
 ('Tania','Cortés Ávila','tania.cortes@correo.mx','5530000020','2026-05-23'),
 ('Ulises','Peña Rivas','ulises.pena@correo.mx','5530000021','2026-05-30'),
 ('Verónica','Luna Maldonado','veronica.luna@correo.mx','5530000022','2026-06-06'),
 ('Wilfrido','Ibarra Soto','wilfrido.ibarra@correo.mx','5530000023','2026-06-13'),
 ('Ximena','Campos Toledo','ximena.campos@correo.mx','5530000024','2026-06-20'),
 ('Yahir','Rosales Vera','yahir.rosales@correo.mx','5530000025','2026-06-27'),
 ('Zaira','Montes Olvera','zaira.montes@correo.mx','5530000026','2026-07-04'),
 ('Adrián','Salazar Gil','adrian.salazar@correo.mx','5530000027','2026-08-01'),
 ('Brenda','Figueroa Luna','brenda.figueroa@correo.mx',NULL,'2026-09-12'),
 ('Cristian','Esquivel Mora','cristian.esquivel@correo.mx','5530000029','2026-10-03'),
 ('Diana','Valencia Ruiz','diana.valencia@correo.mx','5530000030','2026-10-20');

-- Cliente inactivo (sin movimientos)
UPDATE Cliente SET activo = 0 WHERE id_cliente = 30;

-- stock_actual arranca en 0: lo llenan las compras y lo bajan las ventas
INSERT INTO Producto (sku, nombre, id_categoria, id_marca, id_condicion, precio_venta, costo, requiere_serie, stock_minimo) VALUES
 ('IPH-15-128','iPhone 15 128 GB',1,1,1,17999,14500,1,2),
 ('IPH-14-128-R','iPhone 14 128 GB reacondicionado',1,1,2,11499,8200,1,1),
 ('SGS-24-256','Galaxy S24 256 GB',1,2,1,16999,13200,1,2),
 ('SGA-55-128','Galaxy A55 128 GB',1,2,1,7999,5900,1,2),
 ('XRN-13-256','Redmi Note 13 256 GB',1,3,1,5499,3900,1,2),
 ('MBA-M3-13','MacBook Air 13" M3',2,1,1,24999,20500,1,1),
 ('MBP-M3-14','MacBook Pro 14" M3',2,1,1,37999,31500,1,1),
 ('MBA-M1-R','MacBook Air M1 reacondicionada',2,1,2,13999,9800,1,1),
 ('LEN-TP-E14','Lenovo ThinkPad E14',2,4,1,18499,14800,1,1),
 ('HP-PAV-15','HP Pavilion 15',2,5,1,14999,11900,1,1),
 ('DEL-INS-14','Dell Inspiron 14 (open box)',2,6,3,12999,10100,1,1),
 ('IMC-M3-24','iMac 24" M3',3,1,1,32999,27500,1,1),
 ('MMN-M2','Mac mini M2',3,1,1,12999,10400,1,1),
 ('HP-AIO-24','HP All-in-One 24',3,5,1,15999,12800,1,1),
 ('IPD-10-64','iPad 10a generación 64 GB',4,1,1,8499,6600,1,2),
 ('SGT-S9FE','Galaxy Tab S9 FE',4,2,1,9999,7700,1,1),
 ('APW-S9-41','Apple Watch Series 9 41 mm',5,1,1,8999,7000,1,1),
 ('SGW-6-40','Galaxy Watch6 40 mm',5,2,1,5999,4500,1,1),
 ('APP-PRO2','AirPods Pro 2',6,1,1,4999,3800,0,5),
 ('SNY-WH1000','Sony WH-1000XM5',6,7,1,6999,5200,0,3),
 ('XBD-5','Redmi Buds 5',6,3,1,699,420,0,10),
 ('CRG-USBC-20','Cargador USB-C 20 W',7,1,1,449,260,0,15),
 ('ANK-PB-10K','Power bank Anker 10000 mAh',7,9,1,599,350,0,8),
 ('LOG-MX3S','Mouse Logitech MX Master 3S',7,8,1,2199,1600,0,4),
 ('LOG-K380','Teclado Logitech K380',7,8,1,899,600,0,5),
 ('CBL-USBC-1M','Cable USB-C a USB-C 1 m',7,9,1,249,110,0,20),
 ('FND-IPH15','Funda de silicón iPhone 15',7,1,1,399,180,0,10),
 ('REF-BAT-IPH14','Batería iPhone 14 (refacción)',8,1,1,1299,750,0,5),
 ('REF-PAN-IPH13','Pantalla iPhone 13 (refacción)',8,1,1,2899,1900,0,3),
 ('REF-TEC-MBA','Teclado MacBook Air (refacción)',8,1,1,1999,1300,0,2),
 ('MON-DEL-27','Monitor Dell 27" 4K',9,6,1,7499,5800,1,1),
 ('SSD-KNG-1TB','SSD Kingston NV2 1 TB',10,10,1,1199,820,0,6),
 ('USB-KNG-128','Memoria USB Kingston 128 GB',10,10,1,249,140,0,10),
 ('IPH-12-64','iPhone 12 64 GB (descontinuado)',1,1,1,9999,7800,1,1);

UPDATE Producto SET activo = 0 WHERE sku = 'IPH-12-64';

-- N:M Producto–Proveedor: cada producto con 1 o 2 proveedores
INSERT INTO ProductoProveedor (id_producto, id_proveedor, costo_proveedor)
SELECT id_producto, (id_producto % 10) + 1, costo FROM Producto
UNION ALL
SELECT id_producto, ((id_producto + 3) % 10) + 1, ROUND(costo * 1.04, 2) FROM Producto WHERE id_producto % 2 = 0;

/* ---------- Seguridad de la aplicación ---------- */
INSERT INTO Rol (nombre) VALUES ('Administrador'),('Supervisor'),('Operador'),('Consulta');

INSERT INTO Usuario (id_empleado, nombre_usuario, hash_contrasena) VALUES
 (1,'admin',      HASHBYTES('SHA2_256','Tecno123!')),
 (2,'csupervisor',HASHBYTES('SHA2_256','Tecno123!')),
 (3,'loperador',  HASHBYTES('SHA2_256','Tecno123!')),
 (10,'fconsulta', HASHBYTES('SHA2_256','Tecno123!')),
 (6,'smendez',    HASHBYTES('SHA2_256','Tecno123!'));

INSERT INTO UsuarioRol (id_usuario, id_rol) VALUES (1,1),(2,2),(3,3),(4,4),(5,3);

INSERT INTO Configuracion (clave, valor, descripcion) VALUES
 ('IVA','0.16','Tasa de IVA incluida en precios'),
 ('DESCUENTO_MAX_OPERADOR','10','% máximo de descuento sin autorización de Supervisor'),
 ('MESES_GARANTIA_NUEVO','12','Meses de garantía para equipo nuevo'),
 ('MESES_GARANTIA_REACOND','6','Meses de garantía para equipo reacondicionado'),
 ('PREFIJO_VENTA','V-','Prefijo de folio de venta'),
 ('PREFIJO_ORDEN','OS-','Prefijo de folio de orden de servicio');
GO

/* =============================================================
   Movimientos generados (semilla fija)
   ============================================================= */
SET NOCOUNT ON;
DECLARE @seed BIGINT = 20260409;
DECLARE @r INT;

/* ---------- Compras: 12 encabezados, cada producto en una compra ---------- */
DECLARE @c INT = 1, @id_compra INT, @fecha DATETIME2, @id_det INT, @p INT, @qty INT, @n INT;
WHILE @c <= 12
BEGIN
    SET @fecha = DATEADD(HOUR, 10, DATEADD(DAY, (@c - 1) * 5, CAST('2026-04-01' AS DATETIME2)));
    INSERT INTO Compra (folio, fecha, id_proveedor, id_empleado, total, estado)
    VALUES (CONCAT('C-', RIGHT('0000' + CAST(@c AS VARCHAR), 4)), @fecha, ((@c - 1) % 10) + 1, 9, 0, 'Recibida');
    SET @id_compra = SCOPE_IDENTITY();

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT id_producto FROM Producto WHERE id_producto % 12 = @c - 1 AND activo = 1;
    OPEN cur; FETCH NEXT FROM cur INTO @p;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @qty = CASE WHEN (SELECT requiere_serie FROM Producto WHERE id_producto = @p) = 1 THEN 5
                        WHEN (SELECT id_categoria FROM Producto WHERE id_producto = @p) = 8 THEN 4
                        ELSE 30 END;
        INSERT INTO DetalleCompra (id_compra, id_producto, cantidad, costo_unitario, subtotal)
        SELECT @id_compra, @p, @qty, costo, costo * @qty FROM Producto WHERE id_producto = @p;
        SET @id_det = SCOPE_IDENTITY();

        IF (SELECT requiere_serie FROM Producto WHERE id_producto = @p) = 1
        BEGIN
            SET @n = 1;
            WHILE @n <= @qty
            BEGIN
                INSERT INTO UnidadSerie (id_producto, id_detalle_compra, numero_serie, estado, fecha_ingreso)
                SELECT @p, @id_det, CONCAT(sku, '-', RIGHT('000' + CAST(@n AS VARCHAR), 3)), 'Disponible', @fecha
                FROM Producto WHERE id_producto = @p;
                SET @n += 1;
            END
        END

        UPDATE Producto SET stock_actual = stock_actual + @qty WHERE id_producto = @p;
        FETCH NEXT FROM cur INTO @p;
    END
    CLOSE cur; DEALLOCATE cur;

    UPDATE Compra SET total = (SELECT SUM(subtotal) FROM DetalleCompra WHERE id_compra = @id_compra)
    WHERE id_compra = @id_compra;
    SET @c += 1;
END

-- Equipos reacondicionados: cada unidad pasó por el taller antes de venderse
INSERT INTO Reacondicionado (id_unidad, id_empleado_tecnico, grado, costo_reparacion, fecha_ingreso, fecha_liberacion, observaciones)
SELECT u.id_unidad,
       CASE u.id_unidad % 3 WHEN 0 THEN 6 WHEN 1 THEN 7 ELSE 11 END,
       CASE u.id_unidad % 3 WHEN 0 THEN 'A' WHEN 1 THEN 'B' ELSE 'C' END,
       CASE u.id_unidad % 3 WHEN 0 THEN 350 WHEN 1 THEN 800 ELSE 1250 END,
       CAST(u.fecha_ingreso AS DATE),
       DATEADD(DAY, 4, CAST(u.fecha_ingreso AS DATE)),
       CASE u.id_unidad % 3 WHEN 0 THEN 'Batería al 92 %, sin detalles' WHEN 1 THEN 'Rayones leves en carcasa'
            ELSE 'Cambio de batería y limpieza' END
FROM UnidadSerie u JOIN Producto p ON p.id_producto = u.id_producto
WHERE p.id_condicion = 2;

/* ---------- Ventas: 45 encabezados, 1 a 4 líneas cada una ---------- */
DECLARE @v INT = 1, @id_venta INT, @lineas INT, @l INT, @id_emp INT, @id_unidad INT, @precio DECIMAL(10,2), @sub DECIMAL(12,2);
DECLARE @vendedores TABLE (i INT IDENTITY(0,1), id_empleado INT);
INSERT INTO @vendedores (id_empleado) VALUES (3),(4),(5),(8),(2),(3),(4);

WHILE @v <= 45
BEGIN
    SET @seed = (@seed * 1103515245 + 12345) % 2147483648; SET @r = (@seed / 65536) % 1000;
    SET @fecha = DATEADD(MINUTE, 600 + (@r % 480), DATEADD(DAY, (@v - 1) * 4, CAST('2026-05-02' AS DATETIME2)));
    SELECT @id_emp = id_empleado FROM @vendedores WHERE i = @r % 7;

    SET @seed = (@seed * 1103515245 + 12345) % 2147483648; SET @r = (@seed / 65536) % 1000;
    INSERT INTO Venta (folio, fecha, id_cliente, id_empleado, id_sucursal, id_metodo_pago, subtotal, descuento, total, estado)
    SELECT CONCAT('V-', RIGHT('00000' + CAST(1000 + @v AS VARCHAR), 5)), @fecha,
           (@r % 25) + 1, @id_emp, e.id_sucursal, (@r % 6) + 1, 0, 0, 0, 'Completada'
    FROM Empleado e WHERE e.id_empleado = @id_emp;
    SET @id_venta = SCOPE_IDENTITY();

    SET @seed = (@seed * 1103515245 + 12345) % 2147483648;
    SET @lineas = ((@seed / 65536) % 4) + 1;
    SET @l = 1;
    WHILE @l <= @lineas
    BEGIN
        SET @seed = (@seed * 1103515245 + 12345) % 2147483648; SET @r = (@seed / 65536) % 1000;
        SET @p = (@r % 32) + 1;   -- productos 1..32 (33 nunca se vende, 34 inactivo)
        SET @id_unidad = NULL;

        IF (SELECT requiere_serie FROM Producto WHERE id_producto = @p) = 1
        BEGIN
            SELECT TOP 1 @id_unidad = id_unidad FROM UnidadSerie
            WHERE id_producto = @p AND estado = 'Disponible' ORDER BY id_unidad;
            IF @id_unidad IS NULL SET @p = 22;  -- sin piezas: se vende un cargador
            SET @qty = 1;
        END
        IF @id_unidad IS NULL
        BEGIN
            SET @qty = (@r % 3) + 1;
            IF (SELECT stock_actual FROM Producto WHERE id_producto = @p) < @qty SET @p = 26;
        END

        SELECT @precio = precio_venta FROM Producto WHERE id_producto = @p;
        INSERT INTO DetalleVenta (id_venta, id_producto, id_unidad, cantidad, precio_unitario, subtotal)
        VALUES (@id_venta, @p, @id_unidad, @qty, @precio, @precio * @qty);

        IF @id_unidad IS NOT NULL UPDATE UnidadSerie SET estado = 'Vendida' WHERE id_unidad = @id_unidad;
        UPDATE Producto SET stock_actual = stock_actual - @qty WHERE id_producto = @p;
        SET @l += 1;
    END

    SELECT @sub = SUM(subtotal) FROM DetalleVenta WHERE id_venta = @id_venta;
    UPDATE Venta
       SET subtotal = @sub,
           descuento = CASE WHEN @v % 7 = 0 THEN ROUND(@sub * 0.05, 2) ELSE 0 END,
           total = @sub - CASE WHEN @v % 7 = 0 THEN ROUND(@sub * 0.05, 2) ELSE 0 END
     WHERE id_venta = @id_venta;
    SET @v += 1;
END

-- Dos ventas canceladas: se reintegra stock y se liberan las unidades
UPDATE Venta SET estado = 'Cancelada' WHERE folio IN ('V-01009','V-01027');
UPDATE p SET stock_actual = p.stock_actual + x.cant
FROM Producto p
JOIN (SELECT dv.id_producto, SUM(dv.cantidad) cant FROM DetalleVenta dv JOIN Venta v ON v.id_venta = dv.id_venta
      WHERE v.estado = 'Cancelada' GROUP BY dv.id_producto) x ON x.id_producto = p.id_producto;
UPDATE u SET estado = 'Disponible'
FROM UnidadSerie u JOIN DetalleVenta dv ON dv.id_unidad = u.id_unidad
JOIN Venta v ON v.id_venta = dv.id_venta WHERE v.estado = 'Cancelada';

-- Garantías: toda pieza con número de serie vendida (reacondicionado 6 meses, nuevo 12)
INSERT INTO Garantia (id_detalle_venta, meses, fecha_inicio, fecha_fin)
SELECT dv.id_detalle_venta, m.meses, CAST(v.fecha AS DATE), DATEADD(MONTH, m.meses, CAST(v.fecha AS DATE))
FROM DetalleVenta dv
JOIN Venta v    ON v.id_venta = dv.id_venta
JOIN Producto p ON p.id_producto = dv.id_producto
CROSS APPLY (SELECT CASE WHEN p.id_condicion = 2 THEN 6 ELSE 12 END AS meses) m
WHERE dv.id_unidad IS NOT NULL AND v.estado = 'Completada';

/* ---------- Órdenes de servicio: 24 con historial de estados ---------- */
DECLARE @o INT = 1, @id_orden INT, @destino INT, @e INT, @tec INT, @ts INT, @ref INT;
DECLARE @equipos TABLE (i INT IDENTITY(0,1), equipo VARCHAR(150), falla VARCHAR(300));
INSERT INTO @equipos (equipo, falla) VALUES
 ('iPhone 13 negro','Pantalla estrellada, touch funciona'),
 ('MacBook Air 2020','No enciende después de derrame de café'),
 ('Galaxy S22','Batería dura menos de 4 horas'),
 ('iPhone 14','No carga, puerto con pelusa'),
 ('Laptop HP Pavilion','Muy lenta, ventilador ruidoso'),
 ('iPad 9a gen','Pantalla no responde en una esquina'),
 ('MacBook Pro 2019','Teclas que no responden'),
 ('Redmi Note 11','Se reinicia solo'),
 ('iMac 2017','Disco duro lento, piden cambio a SSD'),
 ('Apple Watch S6','Batería se infla');

WHILE @o <= 24
BEGIN
    SET @seed = (@seed * 1103515245 + 12345) % 2147483648; SET @r = (@seed / 65536) % 1000;
    SET @fecha = DATEADD(MINUTE, 600 + (@r % 420), DATEADD(DAY, (@o - 1) * 7, CAST('2026-05-05' AS DATETIME2)));
    SET @tec = CASE @r % 3 WHEN 0 THEN 6 WHEN 1 THEN 7 ELSE 11 END;
    -- Las órdenes viejas ya se entregaron; las recientes siguen en proceso
    SET @destino = CASE WHEN @o IN (5, 14) THEN 6
                        WHEN @o <= 18 THEN 5
                        ELSE ((@o - 19) % 4) + 1 END;

    INSERT INTO OrdenServicio (folio, fecha_recepcion, id_cliente, id_empleado_tecnico, id_estado_orden,
                               equipo_descripcion, numero_serie_equipo, descripcion_falla, fecha_entrega, total)
    SELECT CONCAT('OS-', RIGHT('0000' + CAST(3000 + @o AS VARCHAR), 4)), @fecha, ((@r * 7) % 25) + 1, @tec, @destino,
           eq.equipo, CASE WHEN @r % 4 = 0 THEN NULL ELSE CONCAT('SN', 100000 + @r * 37) END, eq.falla,
           CASE WHEN @destino = 5 THEN DATEADD(DAY, 3 + @r % 5, @fecha) END, 0
    FROM @equipos eq WHERE eq.i = @o % 10;
    SET @id_orden = SCOPE_IDENTITY();

    -- Detalle: diagnóstico + servicio principal + refacción a veces
    IF @destino <> 6 OR @o = 14
    BEGIN
        INSERT INTO DetalleOrdenServicio (id_orden, id_tipo_servicio, cantidad, precio_unitario, subtotal)
        SELECT @id_orden, 1, 1, precio_base, precio_base FROM TipoServicio WHERE id_tipo_servicio = 1;
    END
    IF @destino NOT IN (1, 6)
    BEGIN
        SET @ts = CASE @o % 10 WHEN 0 THEN 4 WHEN 1 THEN 7 WHEN 2 THEN 3 WHEN 3 THEN 9 WHEN 4 THEN 2
                               WHEN 5 THEN 4 WHEN 6 THEN 8 WHEN 7 THEN 5 WHEN 8 THEN 10 ELSE 3 END;
        INSERT INTO DetalleOrdenServicio (id_orden, id_tipo_servicio, cantidad, precio_unitario, subtotal)
        SELECT @id_orden, @ts, 1, precio_base, precio_base FROM TipoServicio WHERE id_tipo_servicio = @ts;

        SET @ref = CASE @o % 10 WHEN 0 THEN 29 WHEN 2 THEN 28 WHEN 6 THEN 30 WHEN 8 THEN 32 ELSE NULL END;
        IF @ref IS NOT NULL
        BEGIN
            INSERT INTO DetalleOrdenServicio (id_orden, id_producto, cantidad, precio_unitario, subtotal)
            SELECT @id_orden, @ref, 1, precio_venta, precio_venta FROM Producto WHERE id_producto = @ref;
        END
    END

    UPDATE OrdenServicio SET total = ISNULL((SELECT SUM(subtotal) FROM DetalleOrdenServicio WHERE id_orden = @id_orden), 0)
    WHERE id_orden = @id_orden;

    -- Historial: recorrido de estados hasta el destino
    INSERT INTO HistorialEstadoOrden (id_orden, id_estado_anterior, id_estado_nuevo, fecha, id_usuario)
    VALUES (@id_orden, NULL, 1, @fecha, 3);
    SET @e = 2;
    WHILE @e <= CASE WHEN @destino = 6 THEN 2 ELSE @destino END
    BEGIN
        INSERT INTO HistorialEstadoOrden (id_orden, id_estado_anterior, id_estado_nuevo, fecha, id_usuario)
        VALUES (@id_orden, @e - 1, @e, DATEADD(HOUR, 18 * (@e - 1), @fecha), 5);
        SET @e += 1;
    END
    IF @destino = 6
        INSERT INTO HistorialEstadoOrden (id_orden, id_estado_anterior, id_estado_nuevo, fecha, id_usuario)
        VALUES (@id_orden, 2, 6, DATEADD(HOUR, 40, @fecha), 2);

    SET @o += 1;
END
GO
