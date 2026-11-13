using System.Collections.Generic;
using System.Linq;

namespace TecnoStore.Models
{
    public class UsuarioSesion
    {
        public int IdUsuario { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public string Sucursal { get; set; }
        /// <summary>Administrador, Supervisor, Operador o Consulta.</summary>
        public string Rol { get; set; }

        public bool EsAdministrador => Rol == "Administrador";
        public bool EsSupervisorOMas => Rol == "Administrador" || Rol == "Supervisor";
        public bool PuedeOperar => Rol != "Consulta";
    }

    /// <summary>Elemento de un ComboBox: el usuario ve el texto, la app usa el Id.</summary>
    public class Opcion
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public override string ToString() => Texto;
    }

    public class ProductoVenta
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool RequiereSerie { get; set; }
        public override string ToString() => Sku + " — " + Nombre + "  (" + Precio.ToString("C") + ", stock " + Stock + ")";
    }

    public class LineaVenta
    {
        public int IdProducto { get; set; }
        public string Sku { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Importe => Cantidad * Precio;
    }

    public static class Json
    {
        /// <summary>Arma el JSON que esperan usp_RegistrarVenta: [{"id_producto":1,"cantidad":2}].</summary>
        public static string Lineas(IEnumerable<LineaVenta> lineas)
        {
            return "[" + string.Join(",", lineas.Select(l =>
                "{\"id_producto\":" + l.IdProducto + ",\"cantidad\":" + l.Cantidad + "}")) + "]";
        }

        /// <summary>Texto escapado como cadena JSON (para números de serie).</summary>
        public static string Texto(string s)
        {
            var sb = new System.Text.StringBuilder("\"");
            foreach (char c in s)
            {
                if (c == '"' || c == '\\') sb.Append('\\').Append(c);
                else if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4"));
                else sb.Append(c);
            }
            return sb.Append('"').ToString();
        }
    }
}
