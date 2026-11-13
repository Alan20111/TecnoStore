using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TecnoStore.Models;

namespace TecnoStore.Data
{
    /// <summary>Catálogos que se administran con la misma pantalla (FrmCatalogo).</summary>
    public enum TipoCatalogo { Categoria, Marca, TipoServicio }

    /// <summary>
    /// CRUD de catálogos con DML parametrizado (sección 7.1): INSERT, UPDATE con
    /// validación de filas afectadas y DELETE controlado por dependencias (FK).
    /// Los nombres de tabla están fijos en el código; el usuario solo aporta valores.
    /// </summary>
    public static class CatalogoCrudRepository
    {
        public static string Titulo(TipoCatalogo t) =>
            t == TipoCatalogo.Categoria ? "Categorías" : t == TipoCatalogo.Marca ? "Marcas" : "Tipos de servicio";

        /// <summary>Etiqueta del segundo campo del catálogo.</summary>
        public static string Campo2(TipoCatalogo t) =>
            t == TipoCatalogo.Categoria ? "Descripción" : t == TipoCatalogo.Marca ? "País de origen" : "Precio base";

        public static DataTable Listar(TipoCatalogo t, string texto)
        {
            string sql;
            switch (t)
            {
                case TipoCatalogo.Categoria:
                    sql = @"SELECT c.id_categoria AS id, c.nombre, c.descripcion AS campo2,
                                   (SELECT COUNT(*) FROM Producto p WHERE p.id_categoria = c.id_categoria) AS productos
                            FROM Categoria c WHERE @t IS NULL OR c.nombre LIKE '%' + @t + '%' ORDER BY c.nombre";
                    break;
                case TipoCatalogo.Marca:
                    sql = @"SELECT m.id_marca AS id, m.nombre, m.pais_origen AS campo2,
                                   (SELECT COUNT(*) FROM Producto p WHERE p.id_marca = m.id_marca) AS productos
                            FROM Marca m WHERE @t IS NULL OR m.nombre LIKE '%' + @t + '%' ORDER BY m.nombre";
                    break;
                default:
                    sql = @"SELECT ts.id_tipo_servicio AS id, ts.nombre, ts.precio_base AS campo2,
                                   (SELECT COUNT(*) FROM DetalleOrdenServicio d WHERE d.id_tipo_servicio = ts.id_tipo_servicio) AS usos,
                                   (SELECT COUNT(*) FROM EmpleadoTipoServicio e WHERE e.id_tipo_servicio = ts.id_tipo_servicio) AS tecnicos
                            FROM TipoServicio ts WHERE @t IS NULL OR ts.nombre LIKE '%' + @t + '%' ORDER BY ts.nombre";
                    break;
            }
            return Db.Consulta(sql, Db.P("@t", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()));
        }

        public static void Guardar(TipoCatalogo t, int? id, string nombre, string campo2, decimal precio)
        {
            string sql;
            var ps = new List<SqlParameter>
            {
                Db.P("@id", SqlDbType.Int, id),
                Db.P("@nombre", SqlDbType.VarChar, 80, nombre.Trim()),
            };
            switch (t)
            {
                case TipoCatalogo.Categoria:
                    sql = id == null ? "INSERT INTO Categoria (nombre, descripcion) VALUES (@nombre, @c2)"
                                     : "UPDATE Categoria SET nombre = @nombre, descripcion = @c2 WHERE id_categoria = @id";
                    ps.Add(Db.P("@c2", SqlDbType.VarChar, 200, string.IsNullOrWhiteSpace(campo2) ? null : campo2.Trim()));
                    break;
                case TipoCatalogo.Marca:
                    sql = id == null ? "INSERT INTO Marca (nombre, pais_origen) VALUES (@nombre, @c2)"
                                     : "UPDATE Marca SET nombre = @nombre, pais_origen = @c2 WHERE id_marca = @id";
                    ps.Add(Db.P("@c2", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(campo2) ? null : campo2.Trim()));
                    break;
                default:
                    sql = id == null ? "INSERT INTO TipoServicio (nombre, precio_base) VALUES (@nombre, @precio)"
                                     : "UPDATE TipoServicio SET nombre = @nombre, precio_base = @precio WHERE id_tipo_servicio = @id";
                    ps.Add(Db.Decimal("@precio", precio, 10, 2));
                    break;
            }
            try
            {
                using (var cn = Db.Abrir())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddRange(ps.ToArray());
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("El registro ya no existe; recarga la lista.");
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Ya existe un registro con el nombre \"" + nombre.Trim() + "\".");
            }
        }

        public static void Eliminar(TipoCatalogo t, int id)
        {
            string sql = t == TipoCatalogo.Categoria ? "DELETE FROM Categoria WHERE id_categoria = @id"
                       : t == TipoCatalogo.Marca ? "DELETE FROM Marca WHERE id_marca = @id"
                       : "DELETE FROM TipoServicio WHERE id_tipo_servicio = @id";
            try
            {
                using (var cn = Db.Abrir())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add(Db.P("@id", SqlDbType.Int, id));
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("El registro ya no existe; recarga la lista.");
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // La FK protege la integridad: no se borra algo que está en uso
                throw new InvalidOperationException("No se puede eliminar: hay productos, órdenes o técnicos que lo usan.");
            }
        }
    }

    public static class ProveedorRepository
    {
        public static DataTable Buscar(string texto)
        {
            return Db.Consulta(@"
                SELECT pv.id_proveedor, pv.razon_social, pv.rfc, pv.correo, pv.telefono, pv.activo,
                       (SELECT COUNT(*) FROM ProductoProveedor pp WHERE pp.id_proveedor = pv.id_proveedor) AS productos,
                       (SELECT COUNT(*) FROM Compra c WHERE c.id_proveedor = pv.id_proveedor) AS compras,
                       (SELECT ISNULL(SUM(total), 0) FROM Compra c WHERE c.id_proveedor = pv.id_proveedor AND c.estado = 'Recibida') AS total_comprado
                FROM Proveedor pv
                WHERE @t IS NULL OR pv.razon_social LIKE '%' + @t + '%' OR pv.rfc LIKE '%' + @t + '%'
                ORDER BY pv.razon_social",
                Db.P("@t", SqlDbType.VarChar, 60, string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()));
        }

        /// <summary>Relación N:M ProductoProveedor.</summary>
        public static DataTable ProductosQueSurte(int idProveedor)
        {
            return Db.Consulta(@"
                SELECT p.sku, p.nombre, pp.costo_proveedor, p.costo AS costo_actual, p.stock_actual
                FROM ProductoProveedor pp JOIN Producto p ON p.id_producto = pp.id_producto
                WHERE pp.id_proveedor = @id ORDER BY p.nombre",
                Db.P("@id", SqlDbType.Int, idProveedor));
        }

        public static void Guardar(int? id, string razonSocial, string rfc, string correo, string telefono, bool activo)
        {
            Db.Procedimiento("usp_ProveedorGuardar",
                Db.P("@id_proveedor", SqlDbType.Int, id),
                Db.P("@razon_social", SqlDbType.VarChar, 120, razonSocial.Trim()),
                Db.P("@rfc", SqlDbType.VarChar, 13, rfc),
                Db.P("@correo", SqlDbType.VarChar, 100, correo),
                Db.P("@telefono", SqlDbType.VarChar, 20, telefono),
                Db.P("@activo", SqlDbType.Bit, activo));
        }

        public static List<Opcion> Activos()
        {
            return Db.Consulta("SELECT id_proveedor, razon_social FROM Proveedor WHERE activo = 1 ORDER BY razon_social")
                .Rows.Cast<DataRow>().Select(r => new Opcion { Id = (int)r[0], Texto = (string)r[1] }).ToList();
        }
    }

    public class LineaCompra
    {
        public int IdProducto { get; set; }
        public string Sku { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal Importe => Cantidad * Costo;
        public string Series { get; set; }
        internal List<string> ListaSeries { get; set; } = new List<string>();
    }

    public class ProductoCompra
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public bool RequiereSerie { get; set; }
        public override string ToString() => Sku + " — " + Nombre + (RequiereSerie ? "  [con número de serie]" : "");
    }

    public static class CompraRepository
    {
        public static List<ProductoCompra> Productos()
        {
            return Db.Consulta("SELECT id_producto, sku, nombre, costo, requiere_serie FROM Producto WHERE activo = 1 ORDER BY nombre")
                .Rows.Cast<DataRow>().Select(r => new ProductoCompra
                {
                    Id = (int)r[0], Sku = (string)r[1], Nombre = (string)r[2], Costo = (decimal)r[3], RequiereSerie = (bool)r[4],
                }).ToList();
        }

        /// <summary>usp_RegistrarCompra (transacción). Devuelve el folio.</summary>
        public static string Registrar(int idProveedor, IEnumerable<LineaCompra> lineas)
        {
            string json = "[" + string.Join(",", lineas.Select(l =>
                "{\"id_producto\":" + l.IdProducto +
                ",\"cantidad\":" + l.Cantidad +
                ",\"costo_unitario\":" + l.Costo.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                (l.ListaSeries.Count > 0 ? ",\"series\":[" + string.Join(",", l.ListaSeries.Select(Json.Texto)) + "]" : "") +
                "}")) + "]";
            var folio = Db.Salida("@folio", SqlDbType.VarChar, 20);
            Db.Procedimiento("usp_RegistrarCompra",
                Db.P("@id_proveedor", SqlDbType.Int, idProveedor),
                Db.P("@id_empleado", SqlDbType.Int, Db.Sesion.IdEmpleado),
                Db.P("@lineas", SqlDbType.NVarChar, -1, json),
                Db.Salida("@id_compra", SqlDbType.Int),
                folio);
            return (string)folio.Value;
        }

        public static DataTable Recientes()
        {
            return Db.Consulta(@"
                SELECT TOP 50 c.folio, c.fecha, pv.razon_social AS proveedor, CONCAT(e.nombre, ' ', e.apellidos) AS recibio,
                       (SELECT COUNT(*) FROM DetalleCompra d WHERE d.id_compra = c.id_compra) AS lineas, c.total, c.estado
                FROM Compra c JOIN Proveedor pv ON pv.id_proveedor = c.id_proveedor JOIN Empleado e ON e.id_empleado = c.id_empleado
                ORDER BY c.fecha DESC");
        }
    }

    public static class UsuarioRepository
    {
        public static DataTable Listar()
        {
            return Db.Consulta(@"
                SELECT u.id_usuario, u.id_empleado, ur.id_rol, u.nombre_usuario AS usuario,
                       CONCAT(e.nombre, ' ', e.apellidos) AS empleado, e.puesto, r.nombre AS rol,
                       u.activo, u.ultimo_acceso, u.intentos_fallidos
                FROM Usuario u
                JOIN Empleado e ON e.id_empleado = u.id_empleado
                LEFT JOIN UsuarioRol ur ON ur.id_usuario = u.id_usuario
                LEFT JOIN Rol r ON r.id_rol = ur.id_rol
                ORDER BY u.nombre_usuario");
        }

        public static List<Opcion> Roles() =>
            Db.Consulta("SELECT id_rol, nombre FROM Rol ORDER BY id_rol").Rows.Cast<DataRow>()
              .Select(r => new Opcion { Id = (int)r[0], Texto = (string)r[1] }).ToList();

        /// <summary>Empleados activos; los que ya tienen usuario se marcan.</summary>
        public static List<Opcion> Empleados() =>
            Db.Consulta(@"SELECT e.id_empleado, CONCAT(e.nombre, ' ', e.apellidos, ' — ', e.puesto,
                                 CASE WHEN u.id_usuario IS NULL THEN '' ELSE ' (ya tiene usuario)' END)
                          FROM Empleado e LEFT JOIN Usuario u ON u.id_empleado = e.id_empleado
                          WHERE e.activo = 1 ORDER BY e.nombre").Rows.Cast<DataRow>()
              .Select(r => new Opcion { Id = (int)r[0], Texto = (string)r[1] }).ToList();

        public static void Guardar(int? id, int idEmpleado, string usuario, int idRol, string contrasena, bool activo)
        {
            Db.Procedimiento("usp_UsuarioGuardar",
                Db.P("@id_usuario", SqlDbType.Int, id),
                Db.P("@id_empleado", SqlDbType.Int, idEmpleado),
                Db.P("@nombre_usuario", SqlDbType.VarChar, 40, usuario),
                Db.P("@id_rol", SqlDbType.Int, idRol),
                Db.P("@contrasena", SqlDbType.VarChar, 100, string.IsNullOrEmpty(contrasena) ? null : contrasena),
                Db.P("@activo", SqlDbType.Bit, activo),
                Db.P("@id_admin", SqlDbType.Int, Db.Sesion.IdUsuario));
        }
    }
}
