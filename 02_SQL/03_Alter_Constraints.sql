/* =============================================================
   TecnoStore · 03_Alter_Constraints.sql
   Evolución del esquema posterior al diseño inicial (02).
   12 cambios con ALTER TABLE; cada uno documenta su motivo.
   Todos están escritos para conservar los datos existentes:
   columnas nuevas NULL o con DEFAULT, y cuando una columna debe
   ser NOT NULL se agrega como NULL, se rellena y luego se cierra.
   ============================================================= */
USE TecnoStore;
GO

/* Cambio 1 — Agregar columna.
   Motivo: los clientes que piden factura necesitan RFC; es opcional
   porque la mayoría compra como público en general. */
ALTER TABLE Cliente ADD rfc VARCHAR(13) NULL;
GO

/* Cambio 2 — Agregar columna NOT NULL con DEFAULT.
   Motivo: saber desde cuándo existe un producto en catálogo (reportes
   de antigüedad). El DEFAULT rellena las filas que ya existían. */
ALTER TABLE Producto ADD fecha_alta DATETIME2 NOT NULL
    CONSTRAINT DF_Producto_fecha_alta DEFAULT SYSDATETIME();
GO

/* Cambio 3 — Agregar columnas + constraint que las relaciona.
   Motivo: toda cancelación debe dejar fecha y motivo; una venta
   completada no puede tener datos de cancelación. */
ALTER TABLE Venta ADD motivo_cancelacion VARCHAR(200) NULL,
                      fecha_cancelacion  DATETIME2    NULL;
GO
ALTER TABLE Venta ADD CONSTRAINT CK_Venta_cancelacion CHECK (
       (estado = 'Cancelada'  AND fecha_cancelacion IS NOT NULL AND fecha_cancelacion >= fecha)
    OR (estado = 'Completada' AND fecha_cancelacion IS NULL AND motivo_cancelacion IS NULL));
GO

/* Cambio 4 — Agregar constraint CHECK entre dos columnas.
   Motivo: regla de negocio "no se vende por debajo del costo". */
ALTER TABLE Producto ADD CONSTRAINT CK_Producto_precio_vs_costo CHECK (precio_venta >= costo);
GO

/* Cambio 5 — Modificar longitud.
   Motivo: los técnicos necesitan más espacio para describir la falla;
   ampliar un VARCHAR no pierde información. */
ALTER TABLE OrdenServicio ALTER COLUMN descripcion_falla VARCHAR(500) NOT NULL;
GO

/* Cambio 6 — Agregar una relación nueva (FK) sobre una tabla con datos.
   Motivo: la orden debe saber en qué sucursal se recibió el equipo
   (el técnico puede cambiar de sucursal después).
   Pasos: columna NULL -> rellenar con la sucursal del técnico ->
   cerrar a NOT NULL -> agregar la FK. */
ALTER TABLE OrdenServicio ADD id_sucursal INT NULL;
GO
UPDATE o SET id_sucursal = e.id_sucursal
FROM OrdenServicio o JOIN Empleado e ON e.id_empleado = o.id_empleado_tecnico;
GO
ALTER TABLE OrdenServicio ALTER COLUMN id_sucursal INT NOT NULL;
ALTER TABLE OrdenServicio ADD CONSTRAINT FK_OrdenServicio_Sucursal
    FOREIGN KEY (id_sucursal) REFERENCES Sucursal(id_sucursal);
GO

/* Cambio 7 — Agregar valores DEFAULT a columnas existentes.
   Motivo: el estado inicial de cada movimiento es siempre el mismo;
   que lo ponga la base evita errores de captura. */
ALTER TABLE Venta       ADD CONSTRAINT DF_Venta_estado       DEFAULT 'Completada' FOR estado;
ALTER TABLE Compra      ADD CONSTRAINT DF_Compra_estado      DEFAULT 'Recibida'   FOR estado;
ALTER TABLE UnidadSerie ADD CONSTRAINT DF_UnidadSerie_estado DEFAULT 'Disponible' FOR estado;
GO

/* Cambio 8 — Retirar un constraint por su nombre y volver a crearlo.
   Motivo: el CHECK original solo pedía descuento >= 0; faltaba impedir
   un descuento mayor que el subtotal (dejaría el total negativo). */
ALTER TABLE Venta DROP CONSTRAINT CK_Venta_descuento;
ALTER TABLE Venta ADD  CONSTRAINT CK_Venta_descuento CHECK (descuento >= 0 AND descuento <= subtotal);
GO

/* Cambio 9 — Agregar columnas de bitácora de acceso.
   Motivo: registrar el último inicio de sesión y los intentos fallidos. */
ALTER TABLE Usuario ADD ultimo_acceso     DATETIME2 NULL,
                        intentos_fallidos INT NOT NULL
                            CONSTRAINT DF_Usuario_intentos_fallidos DEFAULT 0
                            CONSTRAINT CK_Usuario_intentos_fallidos CHECK (intentos_fallidos >= 0);
GO

/* Cambio 10 — Agregar columnas a la auditoría.
   Motivo: la app se conecta con un login de SQL Server por rol, así que
   "usuario" (login) no basta para saber qué persona hizo el cambio;
   usuario_app guarda el usuario de la aplicación y motivo la razón
   capturada en los ajustes. */
ALTER TABLE Auditoria ADD usuario_app VARCHAR(40)  NULL,
                          motivo      VARCHAR(200) NULL;
GO

/* Cambio 11 — Agregar UNIQUE compuesto.
   Motivo: un producto aparece una sola vez por compra; si se repite
   se suma la cantidad en la misma línea. */
ALTER TABLE DetalleCompra ADD CONSTRAINT UQ_DetalleCompra_compra_producto UNIQUE (id_compra, id_producto);
GO

/* Cambio 12 — Nueva tabla intermedia (tercera relación N:M) con sus FK
   agregadas por ALTER TABLE.
   Motivo: no todos los técnicos hacen todos los servicios (p. ej.
   reparación de placa); una orden solo puede asignarse a un técnico
   habilitado para el servicio. */
CREATE TABLE EmpleadoTipoServicio (
    id_empleado      INT  NOT NULL,
    id_tipo_servicio INT  NOT NULL,
    fecha_alta       DATE NOT NULL CONSTRAINT DF_EmpleadoTipoServicio_fecha_alta DEFAULT GETDATE(),
    CONSTRAINT PK_EmpleadoTipoServicio PRIMARY KEY (id_empleado, id_tipo_servicio)
);
GO
ALTER TABLE EmpleadoTipoServicio ADD CONSTRAINT FK_EmpleadoTipoServicio_Empleado
    FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado);
ALTER TABLE EmpleadoTipoServicio ADD CONSTRAINT FK_EmpleadoTipoServicio_TipoServicio
    FOREIGN KEY (id_tipo_servicio) REFERENCES TipoServicio(id_tipo_servicio);
GO
