using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TecnoStore.Models;

namespace TecnoStore.Data
{
    // Cada repositorio agrupa el SQL de un módulo. Los formularios no
    // escriben SQL: llaman a estos métodos.

    public static class AuthRepository
    {
        /// <summary>usp_Login: valida la contraseña (hash) y devuelve el perfil.</summary>
        public static UsuarioSesion Login(string usuario, string contrasena)
        {
            var t = Db.ProcedimientoTabla("usp_Login",
                Db.P("@nombre_usuario", SqlDbType.VarChar, 40, usuario),
                Db.P("@contrasena", SqlDbType.VarChar, 100, contrasena));
            if (t.Rows.Count == 0) throw new InvalidOperationException("El empleado está inactivo.");
            var r = t.Rows[0];
            return new UsuarioSesion
            {
                IdUsuario = (int)r["id_usuario"],
                IdEmpleado = (int)r["id_empleado"],
                NombreUsuario = (string)r["nombre_usuario"],
                Nombre = (string)r["nombre"],
                Puesto = (string)r["puesto"],
                Sucursal = (string)r["sucursal"],
                Rol = (string)r["rol"],
            };
        }
    }

    public static class CatalogoRepository
    {
        private static List<Opcion> Opciones(string sql)
        {
            return Db.Consulta(sql).Rows.Cast<DataRow>()
                .Select(r => new Opcion { Id = (int)r[0], Texto = (string)r[1] }).ToList();
        }

        public static List<Opcion> Categorias() => Opciones("SELECT id_categoria, nombre FROM Categoria ORDER BY nombre");
        public static List<Opcion> Marcas() => Opciones("SELECT id_marca, nombre FROM Marca ORDER BY nombre");
        public static List<Opcion> Condiciones() => Opciones("SELECT id_condicion, nombre FROM Condicion ORDER BY id_condicion");
        public static List<Opcion> MetodosPago() => Opciones("SELECT id_metodo_pago, nombre FROM MetodoPago ORDER BY id_metodo_pago");
        public static List<Opcion> EstadosOrden() => Opciones("SELECT id_estado_orden, nombre FROM EstadoOrden ORDER BY id_estado_orden");
        public static List<Opcion> TiposServicio() => Opciones("SELECT id_tipo_servicio, CONCAT(nombre, ' (', FORMAT(precio_base, 'C', 'es-MX'), ')') FROM TipoServicio ORDER BY nombre");
        public static List<Opcion> ClientesActivos() => Opciones("SELECT id_cliente, CONCAT(nombre, ' ', apellidos) FROM Cliente WHERE activo = 1 ORDER BY nombre, apellidos");
        public static List<Opcion> Tecnicos() => Opciones("SELECT id_empleado, CONCAT(nombre, ' ', apellidos) FROM Empleado WHERE puesto = 'Técnico' AND activo = 1 ORDER BY nombre");
        public static List<Opcion> Refacciones() => Opciones("SELECT id_producto, CONCAT(sku, ' — ', nombre, ' (stock ', stock_actual, ')') FROM Producto WHERE requiere_serie = 0 AND activo = 1 AND stock_actual > 0 ORDER BY nombre");
    }

    public static class ProductoRepository
    {
        /// <summary>usp_ProductoBuscar con parámetros opcionales.</summary>
        public static DataTable Buscar(string texto, int? idCategoria, bool soloAlertas)
        {
            return Db.ProcedimientoTabla("usp_ProductoBuscar",
                Db.P("@texto", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()),
                Db.P("@id_categoria", SqlDbType.Int, idCategoria),
                Db.P("@solo_alertas", SqlDbType.Bit, soloAlertas));
        }

        public static DataRow Obtener(int id)
        {
            var t = Db.Consulta("SELECT * FROM Producto WHERE id_producto = @id", Db.P("@id", SqlDbType.Int, id));
            return t.Rows.Count == 0 ? null : t.Rows[0];
        }

        /// <summary>usp_ProductoGuardar: alta si id es null, si no edición. Devuelve el id.</summary>
        public static int Guardar(int? id, string sku, string nombre, int idCategoria, int idMarca, int idCondicion,
                                  decimal precio, decimal costo, bool requiereSerie, int stockMinimo, bool activo)
        {
            var pid = Db.P("@id_producto", SqlDbType.Int, id);
            pid.Direction = ParameterDirection.InputOutput;
            Db.Procedimiento("usp_ProductoGuardar", pid,
                Db.P("@sku", SqlDbType.VarChar, 30, sku),
                Db.P("@nombre", SqlDbType.VarChar, 120, nombre),
                Db.P("@id_categoria", SqlDbType.Int, idCategoria),
                Db.P("@id_marca", SqlDbType.Int, idMarca),
                Db.P("@id_condicion", SqlDbType.Int, idCondicion),
                Db.Decimal("@precio_venta", precio, 10, 2),
                Db.Decimal("@costo", costo, 10, 2),
                Db.P("@requiere_serie", SqlDbType.Bit, requiereSerie),
                Db.P("@stock_minimo", SqlDbType.Int, stockMinimo),
                Db.P("@activo", SqlDbType.Bit, activo));
            return (int)pid.Value;
        }

        /// <summary>DELETE: el trigger trg_Producto_BajaLogica lo convierte en baja si tiene historial.</summary>
        public static void Eliminar(int id)
        {
            Db.Consulta("DELETE FROM Producto WHERE id_producto = @id", Db.P("@id", SqlDbType.Int, id));
        }

        public static List<ProductoVenta> ParaVenta()
        {
            return Db.Consulta(@"SELECT id_producto, sku, nombre, precio_venta, stock_actual, requiere_serie
                                 FROM vw_ProductosActivos WHERE stock_actual > 0 ORDER BY nombre")
                .Rows.Cast<DataRow>().Select(r => new ProductoVenta
                {
                    Id = (int)r["id_producto"], Sku = (string)r["sku"], Nombre = (string)r["nombre"],
                    Precio = (decimal)r["precio_venta"], Stock = (int)r["stock_actual"], RequiereSerie = (bool)r["requiere_serie"],
                }).ToList();
        }

        public static void AjustarStock(int idProducto, int cantidad, string motivo)
        {
            Db.Procedimiento("usp_AjustarStock",
                Db.P("@id_producto", SqlDbType.Int, idProducto),
                Db.P("@cantidad", SqlDbType.Int, cantidad),
                Db.P("@motivo", SqlDbType.VarChar, 200, motivo),
                Db.P("@id_usuario", SqlDbType.Int, Db.Sesion.IdUsuario));
        }
    }

    public static class ClienteRepository
    {
        /// <summary>Usa las funciones fn_TotalCompradoCliente y fn_ClasificacionCliente.</summary>
        public static DataTable Buscar(string texto)
        {
            return Db.Consulta(@"
                SELECT c.id_cliente, c.nombre, c.apellidos, c.correo, c.telefono, c.rfc, c.activo,
                       CAST(c.fecha_registro AS DATE) AS registro,
                       dbo.fn_TotalCompradoCliente(c.id_cliente) AS total_comprado,
                       dbo.fn_ClasificacionCliente(c.id_cliente) AS nivel
                FROM Cliente c
                WHERE @t IS NULL OR c.nombre LIKE '%' + @t + '%' OR c.apellidos LIKE '%' + @t + '%' OR c.correo LIKE '%' + @t + '%'
                ORDER BY c.nombre, c.apellidos",
                Db.P("@t", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()));
        }

        public static DataTable Historial(int idCliente)
        {
            return Db.Consulta(@"SELECT folio, fecha, sucursal, lineas, total, estado
                                 FROM vw_VentaCompleta WHERE id_cliente = @id ORDER BY fecha DESC",
                Db.P("@id", SqlDbType.Int, idCliente));
        }

        public static int Guardar(int? id, string nombre, string apellidos, string correo, string telefono, string rfc, bool activo)
        {
            var pid = Db.P("@id_cliente", SqlDbType.Int, id);
            pid.Direction = ParameterDirection.InputOutput;
            Db.Procedimiento("usp_ClienteGuardar", pid,
                Db.P("@nombre", SqlDbType.VarChar, 60, nombre),
                Db.P("@apellidos", SqlDbType.VarChar, 80, apellidos),
                Db.P("@correo", SqlDbType.VarChar, 100, correo),
                Db.P("@telefono", SqlDbType.VarChar, 20, telefono),
                Db.P("@rfc", SqlDbType.VarChar, 13, string.IsNullOrWhiteSpace(rfc) ? null : rfc.Trim()),
                Db.P("@activo", SqlDbType.Bit, activo));
            return (int)pid.Value;
        }
    }

    public static class VentaRepository
    {
        /// <summary>usp_RegistrarVenta (transacción completa). Devuelve el folio.</summary>
        public static string Registrar(int idCliente, int idMetodoPago, decimal descuentoPct, IEnumerable<LineaVenta> lineas)
        {
            var folio = Db.Salida("@folio", SqlDbType.VarChar, 20);
            Db.Procedimiento("usp_RegistrarVenta",
                Db.P("@id_cliente", SqlDbType.Int, idCliente),
                Db.P("@id_empleado", SqlDbType.Int, Db.Sesion.IdEmpleado),
                Db.P("@id_metodo_pago", SqlDbType.Int, idMetodoPago),
                Db.P("@lineas", SqlDbType.NVarChar, -1, Json.Lineas(lineas)),
                Db.Decimal("@descuento_pct", descuentoPct, 5, 2),
                // Un supervisor o administrador se autoriza a sí mismo
                Db.P("@id_usuario_autoriza", SqlDbType.Int, Db.Sesion.EsSupervisorOMas ? (object)Db.Sesion.IdUsuario : null),
                Db.Salida("@id_venta", SqlDbType.Int),
                folio);
            return (string)folio.Value;
        }

        /// <summary>Historial sobre la vista vw_VentaCompleta con filtros combinados.</summary>
        public static DataTable Buscar(DateTime desde, DateTime hasta, string estado, string texto)
        {
            return Db.Consulta(@"
                SELECT id_venta, folio, fecha, cliente, vendedor, sucursal, metodo_pago, lineas, subtotal, descuento, total, estado
                FROM vw_VentaCompleta
                WHERE fecha >= @desde AND fecha < DATEADD(DAY, 1, @hasta)
                  AND (@estado IS NULL OR estado = @estado)
                  AND (@t IS NULL OR folio LIKE '%' + @t + '%' OR cliente LIKE '%' + @t + '%')
                ORDER BY fecha DESC",
                Db.P("@desde", SqlDbType.Date, desde.Date),
                Db.P("@hasta", SqlDbType.Date, hasta.Date),
                Db.P("@estado", SqlDbType.VarChar, 12, string.IsNullOrEmpty(estado) ? null : estado),
                Db.P("@t", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()));
        }

        public static DataTable Detalle(int idVenta)
        {
            return Db.Consulta(@"
                SELECT p.sku, p.nombre, us.numero_serie, dv.cantidad, dv.precio_unitario, dv.subtotal,
                       g.fecha_fin AS garantia_hasta
                FROM DetalleVenta dv
                JOIN Producto p ON p.id_producto = dv.id_producto
                LEFT JOIN UnidadSerie us ON us.id_unidad = dv.id_unidad
                LEFT JOIN Garantia g ON g.id_detalle_venta = dv.id_detalle_venta
                WHERE dv.id_venta = @id ORDER BY dv.id_detalle_venta",
                Db.P("@id", SqlDbType.Int, idVenta));
        }

        public static void Cancelar(int idVenta, string motivo)
        {
            Db.Procedimiento("usp_CancelarVenta",
                Db.P("@id_venta", SqlDbType.Int, idVenta),
                Db.P("@motivo", SqlDbType.VarChar, 200, motivo));
        }
    }

    public static class OrdenRepository
    {
        public static DataTable Buscar(bool soloAbiertas)
        {
            return Db.Consulta(@"
                SELECT id_orden, folio, fecha_recepcion, cliente, equipo_descripcion, descripcion_falla, tecnico, estado, es_final,
                       dbo.fn_TotalOrden(id_orden) AS total, dias_en_taller
                FROM vw_OrdenServicioCompleta
                WHERE @abiertas = 0 OR es_final = 0
                ORDER BY fecha_recepcion DESC",
                Db.P("@abiertas", SqlDbType.Bit, soloAbiertas));
        }

        public static DataTable Detalle(int idOrden)
        {
            return Db.Consulta(@"
                SELECT ISNULL(ts.nombre, p.nombre) AS concepto,
                       CASE WHEN d.id_tipo_servicio IS NULL THEN 'Refacción' ELSE 'Mano de obra' END AS tipo,
                       d.cantidad, d.precio_unitario, d.subtotal
                FROM DetalleOrdenServicio d
                LEFT JOIN TipoServicio ts ON ts.id_tipo_servicio = d.id_tipo_servicio
                LEFT JOIN Producto p ON p.id_producto = d.id_producto
                WHERE d.id_orden = @id ORDER BY d.id_detalle_orden",
                Db.P("@id", SqlDbType.Int, idOrden));
        }

        public static DataTable Historial(int idOrden)
        {
            return Db.Consulta(@"
                SELECT h.fecha, ea.nombre AS de_estado, en.nombre AS a_estado
                FROM HistorialEstadoOrden h
                LEFT JOIN EstadoOrden ea ON ea.id_estado_orden = h.id_estado_anterior
                JOIN EstadoOrden en ON en.id_estado_orden = h.id_estado_nuevo
                WHERE h.id_orden = @id ORDER BY h.fecha",
                Db.P("@id", SqlDbType.Int, idOrden));
        }

        public static string Registrar(int idCliente, int idTecnico, string equipo, string falla, string serie)
        {
            var folio = Db.Salida("@folio", SqlDbType.VarChar, 20);
            Db.Procedimiento("usp_RegistrarOrdenServicio",
                Db.P("@id_cliente", SqlDbType.Int, idCliente),
                Db.P("@id_empleado_tecnico", SqlDbType.Int, idTecnico),
                Db.P("@equipo_descripcion", SqlDbType.VarChar, 150, equipo),
                Db.P("@descripcion_falla", SqlDbType.VarChar, 500, falla),
                Db.P("@numero_serie_equipo", SqlDbType.VarChar, 50, string.IsNullOrWhiteSpace(serie) ? null : serie.Trim()),
                Db.Salida("@id_orden", SqlDbType.Int),
                folio);
            return (string)folio.Value;
        }

        public static void AgregarLinea(int idOrden, int? idTipoServicio, int? idProducto, int cantidad)
        {
            Db.Procedimiento("usp_OrdenAgregarLinea",
                Db.P("@id_orden", SqlDbType.Int, idOrden),
                Db.P("@id_tipo_servicio", SqlDbType.Int, idTipoServicio),
                Db.P("@id_producto", SqlDbType.Int, idProducto),
                Db.P("@cantidad", SqlDbType.Int, cantidad));
        }

        public static void CambiarEstado(int idOrden, int idEstado)
        {
            Db.Procedimiento("usp_CambiarEstadoOrden",
                Db.P("@id_orden", SqlDbType.Int, idOrden),
                Db.P("@id_estado", SqlDbType.Int, idEstado));
        }
    }

    public static class ReporteRepository
    {
        public static DataTable ResumenPeriodo(DateTime desde, DateTime hasta) =>
            Db.ProcedimientoTabla("usp_ResumenPeriodo", Db.P("@desde", SqlDbType.Date, desde.Date), Db.P("@hasta", SqlDbType.Date, hasta.Date));

        public static DataTable TopProductos() =>
            Db.Consulta("SELECT TOP 10 sku, nombre, categoria, unidades_vendidas, ingresos, utilidad_bruta FROM vw_TopProductos ORDER BY ingresos DESC");

        public static DataTable ActividadEmpleados(DateTime desde, DateTime hasta) =>
            Db.ProcedimientoTabla("usp_ActividadEmpleados", Db.P("@desde", SqlDbType.Date, desde.Date), Db.P("@hasta", SqlDbType.Date, hasta.Date));

        public static DataTable StockBajo() =>
            Db.Consulta("SELECT sku, nombre, categoria, stock_actual, stock_minimo, faltante, proveedor_sugerido, costo_proveedor FROM vw_StockBajo ORDER BY faltante DESC");

        public static DataTable VentasPorCategoria(DateTime desde, DateTime hasta) =>
            Db.Consulta("SELECT * FROM dbo.fn_VentasPorCategoria(@d, @h) ORDER BY ingresos DESC",
                Db.P("@d", SqlDbType.Date, desde.Date), Db.P("@h", SqlDbType.Date, hasta.Date));
    }

    public static class AuditoriaRepository
    {
        public static DataTable Buscar(DateTime desde, DateTime hasta, string tabla)
        {
            return Db.Consulta(@"
                SELECT TOP 500 fecha, usuario_app, usuario AS login_sql, tabla, operacion, id_registro, valor_anterior, valor_nuevo, motivo
                FROM Auditoria
                WHERE fecha >= @desde AND fecha < DATEADD(DAY, 1, @hasta) AND (@tabla IS NULL OR tabla = @tabla)
                ORDER BY id_auditoria DESC",
                Db.P("@desde", SqlDbType.Date, desde.Date), Db.P("@hasta", SqlDbType.Date, hasta.Date),
                Db.P("@tabla", SqlDbType.VarChar, 60, string.IsNullOrEmpty(tabla) ? null : tabla));
        }
    }
}
