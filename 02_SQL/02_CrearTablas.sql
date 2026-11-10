/* =============================================================
   TecnoStore · 02_CrearTablas.sql
   27 tablas según "Modelo de datos y diccionario".
   Convenciones: tablas PascalCase singular, columnas snake_case,
   PK id_<tabla> INT IDENTITY, constraints con nombre
   (PK_, FK_, UQ_, CK_, DF_), fechas DATETIME2/DATE, importes DECIMAL.
   ============================================================= */
USE TecnoStore;
GO

/* ---------- 1. Catálogos (6) ---------- */
CREATE TABLE Categoria (
    id_categoria INT IDENTITY(1,1) CONSTRAINT PK_Categoria PRIMARY KEY,
    nombre       VARCHAR(60)  NOT NULL CONSTRAINT UQ_Categoria_nombre UNIQUE,
    descripcion  VARCHAR(200) NULL
);

CREATE TABLE Marca (
    id_marca    INT IDENTITY(1,1) CONSTRAINT PK_Marca PRIMARY KEY,
    nombre      VARCHAR(60) NOT NULL CONSTRAINT UQ_Marca_nombre UNIQUE,
    pais_origen VARCHAR(60) NULL
);

CREATE TABLE Condicion (
    id_condicion INT IDENTITY(1,1) CONSTRAINT PK_Condicion PRIMARY KEY,
    nombre       VARCHAR(40) NOT NULL CONSTRAINT UQ_Condicion_nombre UNIQUE
);

CREATE TABLE MetodoPago (
    id_metodo_pago INT IDENTITY(1,1) CONSTRAINT PK_MetodoPago PRIMARY KEY,
    nombre         VARCHAR(40) NOT NULL CONSTRAINT UQ_MetodoPago_nombre UNIQUE
);

CREATE TABLE TipoServicio (
    id_tipo_servicio INT IDENTITY(1,1) CONSTRAINT PK_TipoServicio PRIMARY KEY,
    nombre           VARCHAR(80)   NOT NULL CONSTRAINT UQ_TipoServicio_nombre UNIQUE,
    precio_base      DECIMAL(10,2) NOT NULL CONSTRAINT CK_TipoServicio_precio_base CHECK (precio_base >= 0)
);

CREATE TABLE EstadoOrden (
    id_estado_orden INT IDENTITY(1,1) CONSTRAINT PK_EstadoOrden PRIMARY KEY,
    nombre          VARCHAR(40) NOT NULL CONSTRAINT UQ_EstadoOrden_nombre UNIQUE,
    es_final        BIT NOT NULL CONSTRAINT DF_EstadoOrden_es_final DEFAULT 0
);

/* ---------- 2. Entidades maestras (5) ---------- */
CREATE TABLE Sucursal (
    id_sucursal INT IDENTITY(1,1) CONSTRAINT PK_Sucursal PRIMARY KEY,
    nombre      VARCHAR(80)  NOT NULL CONSTRAINT UQ_Sucursal_nombre UNIQUE,
    direccion   VARCHAR(200) NOT NULL,
    telefono    VARCHAR(20)  NULL
);

CREATE TABLE Cliente (
    id_cliente     INT IDENTITY(1,1) CONSTRAINT PK_Cliente PRIMARY KEY,
    nombre         VARCHAR(60)  NOT NULL,
    apellidos      VARCHAR(80)  NOT NULL,
    correo         VARCHAR(100) NOT NULL CONSTRAINT UQ_Cliente_correo UNIQUE,
    telefono       VARCHAR(20)  NULL,
    fecha_registro DATETIME2    NOT NULL CONSTRAINT DF_Cliente_fecha_registro DEFAULT SYSDATETIME(),
    activo         BIT          NOT NULL CONSTRAINT DF_Cliente_activo DEFAULT 1
);

CREATE TABLE Empleado (
    id_empleado        INT IDENTITY(1,1) CONSTRAINT PK_Empleado PRIMARY KEY,
    id_sucursal        INT         NOT NULL CONSTRAINT FK_Empleado_Sucursal REFERENCES Sucursal(id_sucursal),
    nombre             VARCHAR(60) NOT NULL,
    apellidos          VARCHAR(80) NOT NULL,
    puesto             VARCHAR(40) NOT NULL,
    fecha_contratacion DATE        NOT NULL CONSTRAINT CK_Empleado_fecha_contratacion CHECK (fecha_contratacion <= CAST(GETDATE() AS DATE)),
    activo             BIT         NOT NULL CONSTRAINT DF_Empleado_activo DEFAULT 1
);

CREATE TABLE Proveedor (
    id_proveedor INT IDENTITY(1,1) CONSTRAINT PK_Proveedor PRIMARY KEY,
    razon_social VARCHAR(120) NOT NULL,
    rfc          VARCHAR(13)  NOT NULL CONSTRAINT UQ_Proveedor_rfc UNIQUE,
    correo       VARCHAR(100) NULL,
    telefono     VARCHAR(20)  NULL,
    activo       BIT          NOT NULL CONSTRAINT DF_Proveedor_activo DEFAULT 1
);

CREATE TABLE Producto (
    id_producto    INT IDENTITY(1,1) CONSTRAINT PK_Producto PRIMARY KEY,
    sku            VARCHAR(30)   NOT NULL CONSTRAINT UQ_Producto_sku UNIQUE,
    nombre         VARCHAR(120)  NOT NULL,
    id_categoria   INT           NOT NULL CONSTRAINT FK_Producto_Categoria REFERENCES Categoria(id_categoria),
    id_marca       INT           NOT NULL CONSTRAINT FK_Producto_Marca     REFERENCES Marca(id_marca),
    id_condicion   INT           NOT NULL CONSTRAINT FK_Producto_Condicion REFERENCES Condicion(id_condicion),
    precio_venta   DECIMAL(10,2) NOT NULL CONSTRAINT CK_Producto_precio_venta CHECK (precio_venta > 0),
    costo          DECIMAL(10,2) NOT NULL CONSTRAINT CK_Producto_costo CHECK (costo >= 0),
    requiere_serie BIT           NOT NULL CONSTRAINT DF_Producto_requiere_serie DEFAULT 0,
    stock_actual   INT           NOT NULL CONSTRAINT DF_Producto_stock_actual DEFAULT 0
                                          CONSTRAINT CK_Producto_stock_actual CHECK (stock_actual >= 0),
    stock_minimo   INT           NOT NULL CONSTRAINT DF_Producto_stock_minimo DEFAULT 0
                                          CONSTRAINT CK_Producto_stock_minimo CHECK (stock_minimo >= 0),
    activo         BIT           NOT NULL CONSTRAINT DF_Producto_activo DEFAULT 1
);

/* ---------- 3. Seguridad y relaciones N:M (4) ---------- */
CREATE TABLE Usuario (
    id_usuario      INT IDENTITY(1,1) CONSTRAINT PK_Usuario PRIMARY KEY,
    id_empleado     INT            NOT NULL CONSTRAINT UQ_Usuario_id_empleado UNIQUE
                                            CONSTRAINT FK_Usuario_Empleado REFERENCES Empleado(id_empleado),
    nombre_usuario  VARCHAR(40)    NOT NULL CONSTRAINT UQ_Usuario_nombre_usuario UNIQUE,
    hash_contrasena VARBINARY(64)  NOT NULL,   -- SHA2_256, nunca texto plano
    activo          BIT            NOT NULL CONSTRAINT DF_Usuario_activo DEFAULT 1
);

CREATE TABLE Rol (
    id_rol INT IDENTITY(1,1) CONSTRAINT PK_Rol PRIMARY KEY,
    nombre VARCHAR(40) NOT NULL CONSTRAINT UQ_Rol_nombre UNIQUE
);

CREATE TABLE UsuarioRol (
    id_usuario INT NOT NULL CONSTRAINT FK_UsuarioRol_Usuario REFERENCES Usuario(id_usuario),
    id_rol     INT NOT NULL CONSTRAINT FK_UsuarioRol_Rol     REFERENCES Rol(id_rol),
    CONSTRAINT PK_UsuarioRol PRIMARY KEY (id_usuario, id_rol)
);

CREATE TABLE ProductoProveedor (
    id_producto     INT           NOT NULL CONSTRAINT FK_ProductoProveedor_Producto  REFERENCES Producto(id_producto),
    id_proveedor    INT           NOT NULL CONSTRAINT FK_ProductoProveedor_Proveedor REFERENCES Proveedor(id_proveedor),
    costo_proveedor DECIMAL(10,2) NOT NULL CONSTRAINT CK_ProductoProveedor_costo CHECK (costo_proveedor > 0),
    CONSTRAINT PK_ProductoProveedor PRIMARY KEY (id_producto, id_proveedor)
);

/* ---------- 4. Compras e inventario por pieza ---------- */
CREATE TABLE Compra (
    id_compra    INT IDENTITY(1,1) CONSTRAINT PK_Compra PRIMARY KEY,
    folio        VARCHAR(20)   NOT NULL CONSTRAINT UQ_Compra_folio UNIQUE,
    fecha        DATETIME2     NOT NULL CONSTRAINT DF_Compra_fecha DEFAULT SYSDATETIME(),
    id_proveedor INT           NOT NULL CONSTRAINT FK_Compra_Proveedor REFERENCES Proveedor(id_proveedor),
    id_empleado  INT           NOT NULL CONSTRAINT FK_Compra_Empleado  REFERENCES Empleado(id_empleado),
    total        DECIMAL(12,2) NOT NULL CONSTRAINT CK_Compra_total CHECK (total >= 0),
    estado       VARCHAR(12)   NOT NULL CONSTRAINT CK_Compra_estado CHECK (estado IN ('Recibida','Cancelada'))
);

CREATE TABLE DetalleCompra (
    id_detalle_compra INT IDENTITY(1,1) CONSTRAINT PK_DetalleCompra PRIMARY KEY,
    id_compra         INT           NOT NULL CONSTRAINT FK_DetalleCompra_Compra   REFERENCES Compra(id_compra),
    id_producto       INT           NOT NULL CONSTRAINT FK_DetalleCompra_Producto REFERENCES Producto(id_producto),
    cantidad          INT           NOT NULL CONSTRAINT CK_DetalleCompra_cantidad CHECK (cantidad > 0),
    costo_unitario    DECIMAL(10,2) NOT NULL CONSTRAINT CK_DetalleCompra_costo CHECK (costo_unitario > 0),
    subtotal          DECIMAL(12,2) NOT NULL
);

CREATE TABLE UnidadSerie (
    id_unidad         INT IDENTITY(1,1) CONSTRAINT PK_UnidadSerie PRIMARY KEY,
    id_producto       INT         NOT NULL CONSTRAINT FK_UnidadSerie_Producto REFERENCES Producto(id_producto),
    id_detalle_compra INT         NULL     CONSTRAINT FK_UnidadSerie_DetalleCompra REFERENCES DetalleCompra(id_detalle_compra),
    numero_serie      VARCHAR(50) NOT NULL CONSTRAINT UQ_UnidadSerie_numero_serie UNIQUE,
    estado            VARCHAR(20) NOT NULL CONSTRAINT CK_UnidadSerie_estado CHECK (estado IN ('Disponible','Vendida','En reparación','Baja')),
    fecha_ingreso     DATETIME2   NOT NULL CONSTRAINT DF_UnidadSerie_fecha_ingreso DEFAULT SYSDATETIME()
);

CREATE TABLE Reacondicionado (
    id_reacondicionado  INT IDENTITY(1,1) CONSTRAINT PK_Reacondicionado PRIMARY KEY,
    id_unidad           INT           NOT NULL CONSTRAINT UQ_Reacondicionado_id_unidad UNIQUE
                                               CONSTRAINT FK_Reacondicionado_UnidadSerie REFERENCES UnidadSerie(id_unidad),
    id_empleado_tecnico INT           NOT NULL CONSTRAINT FK_Reacondicionado_Empleado REFERENCES Empleado(id_empleado),
    grado               CHAR(1)       NOT NULL CONSTRAINT CK_Reacondicionado_grado CHECK (grado IN ('A','B','C')),
    costo_reparacion    DECIMAL(10,2) NOT NULL CONSTRAINT DF_Reacondicionado_costo DEFAULT 0
                                               CONSTRAINT CK_Reacondicionado_costo CHECK (costo_reparacion >= 0),
    fecha_ingreso       DATE          NOT NULL CONSTRAINT DF_Reacondicionado_fecha_ingreso DEFAULT GETDATE(),
    fecha_liberacion    DATE          NULL,
    observaciones       VARCHAR(300)  NULL,
    CONSTRAINT CK_Reacondicionado_fechas CHECK (fecha_liberacion IS NULL OR fecha_liberacion >= fecha_ingreso)
);

/* ---------- 5. Ventas ---------- */
CREATE TABLE Venta (
    id_venta       INT IDENTITY(1,1) CONSTRAINT PK_Venta PRIMARY KEY,
    folio          VARCHAR(20)   NOT NULL CONSTRAINT UQ_Venta_folio UNIQUE,
    fecha          DATETIME2     NOT NULL CONSTRAINT DF_Venta_fecha DEFAULT SYSDATETIME(),
    id_cliente     INT           NOT NULL CONSTRAINT FK_Venta_Cliente    REFERENCES Cliente(id_cliente),
    id_empleado    INT           NOT NULL CONSTRAINT FK_Venta_Empleado   REFERENCES Empleado(id_empleado),
    id_sucursal    INT           NOT NULL CONSTRAINT FK_Venta_Sucursal   REFERENCES Sucursal(id_sucursal),
    id_metodo_pago INT           NOT NULL CONSTRAINT FK_Venta_MetodoPago REFERENCES MetodoPago(id_metodo_pago),
    subtotal       DECIMAL(12,2) NOT NULL CONSTRAINT CK_Venta_subtotal CHECK (subtotal >= 0),
    descuento      DECIMAL(12,2) NOT NULL CONSTRAINT DF_Venta_descuento DEFAULT 0
                                          CONSTRAINT CK_Venta_descuento CHECK (descuento >= 0),
    total          DECIMAL(12,2) NOT NULL CONSTRAINT CK_Venta_total CHECK (total >= 0),
    estado         VARCHAR(12)   NOT NULL CONSTRAINT CK_Venta_estado CHECK (estado IN ('Completada','Cancelada'))
);

CREATE TABLE DetalleVenta (
    id_detalle_venta INT IDENTITY(1,1) CONSTRAINT PK_DetalleVenta PRIMARY KEY,
    id_venta         INT           NOT NULL CONSTRAINT FK_DetalleVenta_Venta       REFERENCES Venta(id_venta),
    id_producto      INT           NOT NULL CONSTRAINT FK_DetalleVenta_Producto    REFERENCES Producto(id_producto),
    id_unidad        INT           NULL     CONSTRAINT FK_DetalleVenta_UnidadSerie REFERENCES UnidadSerie(id_unidad),
    cantidad         INT           NOT NULL CONSTRAINT CK_DetalleVenta_cantidad CHECK (cantidad > 0),
    precio_unitario  DECIMAL(10,2) NOT NULL CONSTRAINT CK_DetalleVenta_precio CHECK (precio_unitario > 0),
    subtotal         DECIMAL(12,2) NOT NULL
);

CREATE TABLE Garantia (
    id_garantia      INT IDENTITY(1,1) CONSTRAINT PK_Garantia PRIMARY KEY,
    id_detalle_venta INT  NOT NULL CONSTRAINT UQ_Garantia_id_detalle_venta UNIQUE
                               CONSTRAINT FK_Garantia_DetalleVenta REFERENCES DetalleVenta(id_detalle_venta),
    meses            INT  NOT NULL CONSTRAINT CK_Garantia_meses CHECK (meses > 0),
    fecha_inicio     DATE NOT NULL,
    fecha_fin        DATE NOT NULL,
    CONSTRAINT CK_Garantia_fechas CHECK (fecha_fin > fecha_inicio)
);

/* ---------- 6. Taller ---------- */
CREATE TABLE OrdenServicio (
    id_orden             INT IDENTITY(1,1) CONSTRAINT PK_OrdenServicio PRIMARY KEY,
    folio                VARCHAR(20)   NOT NULL CONSTRAINT UQ_OrdenServicio_folio UNIQUE,
    fecha_recepcion      DATETIME2     NOT NULL CONSTRAINT DF_OrdenServicio_fecha_recepcion DEFAULT SYSDATETIME(),
    id_cliente           INT           NOT NULL CONSTRAINT FK_OrdenServicio_Cliente     REFERENCES Cliente(id_cliente),
    id_empleado_tecnico  INT           NOT NULL CONSTRAINT FK_OrdenServicio_Empleado    REFERENCES Empleado(id_empleado),
    id_estado_orden      INT           NOT NULL CONSTRAINT FK_OrdenServicio_EstadoOrden REFERENCES EstadoOrden(id_estado_orden),
    equipo_descripcion   VARCHAR(150)  NOT NULL,
    numero_serie_equipo  VARCHAR(50)   NULL,   -- equipos que no se compraron en la tienda
    descripcion_falla    VARCHAR(300)  NOT NULL,
    fecha_entrega        DATETIME2     NULL,   -- se llena al entregar
    total                DECIMAL(12,2) NOT NULL CONSTRAINT DF_OrdenServicio_total DEFAULT 0
                                                CONSTRAINT CK_OrdenServicio_total CHECK (total >= 0),
    CONSTRAINT CK_OrdenServicio_fechas CHECK (fecha_entrega IS NULL OR fecha_entrega >= fecha_recepcion)
);

CREATE TABLE DetalleOrdenServicio (
    id_detalle_orden INT IDENTITY(1,1) CONSTRAINT PK_DetalleOrdenServicio PRIMARY KEY,
    id_orden         INT           NOT NULL CONSTRAINT FK_DetalleOrdenServicio_OrdenServicio REFERENCES OrdenServicio(id_orden),
    id_tipo_servicio INT           NULL     CONSTRAINT FK_DetalleOrdenServicio_TipoServicio  REFERENCES TipoServicio(id_tipo_servicio),
    id_producto      INT           NULL     CONSTRAINT FK_DetalleOrdenServicio_Producto      REFERENCES Producto(id_producto),
    cantidad         INT           NOT NULL CONSTRAINT CK_DetalleOrdenServicio_cantidad CHECK (cantidad > 0),
    precio_unitario  DECIMAL(10,2) NOT NULL CONSTRAINT CK_DetalleOrdenServicio_precio CHECK (precio_unitario >= 0),
    subtotal         DECIMAL(12,2) NOT NULL,
    -- Cada línea es mano de obra o refacción
    CONSTRAINT CK_DetalleOrdenServicio_linea CHECK (id_tipo_servicio IS NOT NULL OR id_producto IS NOT NULL)
);

/* ---------- 7. Auditoría y soporte (3) ---------- */
CREATE TABLE HistorialEstadoOrden (
    id_historial       INT IDENTITY(1,1) CONSTRAINT PK_HistorialEstadoOrden PRIMARY KEY,
    id_orden           INT       NOT NULL CONSTRAINT FK_HistorialEstadoOrden_OrdenServicio REFERENCES OrdenServicio(id_orden),
    id_estado_anterior INT       NULL     CONSTRAINT FK_HistorialEstadoOrden_EstadoAnterior REFERENCES EstadoOrden(id_estado_orden),
    id_estado_nuevo    INT       NOT NULL CONSTRAINT FK_HistorialEstadoOrden_EstadoNuevo    REFERENCES EstadoOrden(id_estado_orden),
    fecha              DATETIME2 NOT NULL CONSTRAINT DF_HistorialEstadoOrden_fecha DEFAULT SYSDATETIME(),
    id_usuario         INT       NULL     CONSTRAINT FK_HistorialEstadoOrden_Usuario REFERENCES Usuario(id_usuario)
);

CREATE TABLE Auditoria (
    id_auditoria   INT IDENTITY(1,1) CONSTRAINT PK_Auditoria PRIMARY KEY,
    fecha          DATETIME2     NOT NULL CONSTRAINT DF_Auditoria_fecha DEFAULT SYSDATETIME(),
    usuario        VARCHAR(128)  NOT NULL CONSTRAINT DF_Auditoria_usuario DEFAULT SUSER_SNAME(),
    tabla          VARCHAR(60)   NOT NULL,
    operacion      VARCHAR(10)   NOT NULL CONSTRAINT CK_Auditoria_operacion CHECK (operacion IN ('INSERT','UPDATE','DELETE')),
    id_registro    INT           NOT NULL,
    valor_anterior VARCHAR(500)  NULL,
    valor_nuevo    VARCHAR(500)  NULL
);

CREATE TABLE Configuracion (
    clave       VARCHAR(40)  NOT NULL CONSTRAINT PK_Configuracion PRIMARY KEY,
    valor       VARCHAR(100) NOT NULL,
    descripcion VARCHAR(200) NULL
);
GO
