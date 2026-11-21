using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;

namespace TecnoStore.Utils
{
    /// <summary>Mensajes, validación visual y formato de DataGridView.</summary>
    public static class Ui
    {
        public static void Error(Exception ex, string titulo = "Error")
        {
            // Los THROW de los procedimientos llegan como SqlException con el mensaje de negocio.
            // Reglas de negocio (THROW 50000+): su mensaje ya está escrito para el usuario.
            // Lo demás se traduce a lenguaje humano; el detalle técnico queda al final, como diagnóstico.
            string msg;
            if (ex is SqlException sql)
            {
                msg = sql.Number >= 50000 ? sql.Message
                    : sql.Number == 229 || sql.Number == 230 ? "Tu perfil no tiene permiso para esta operación."
                    : sql.Number == 547 ? "No se puede completar porque hay información relacionada que lo impide."
                    : sql.Number == 2627 || sql.Number == 2601 ? "Ya existe un registro con esos datos."
                    : sql.Number == -2 ? "SQL Server tardó demasiado en responder. Intenta de nuevo."
                    : sql.Number == 53 || sql.Number == 2 || sql.Number == -1 ? "No hay conexión con el servidor de base de datos."
                    : "No se pudo completar la operación.";
                if (sql.Number < 50000) msg += "\n\nDetalle para soporte: error " + sql.Number;
            }
            else msg = ex.Message;
            MessageBox.Show(msg, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Info(string msg, string titulo = "TecnoStore") =>
            MessageBox.Show(msg, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static bool Confirmar(string msg, string titulo = "Confirmar") =>
            MessageBox.Show(msg, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;

        /// <summary>Marca en rojo el control si está vacío. Devuelve true si es válido.</summary>
        public static bool Requerido(Control c, ErrorProvider ep, string mensaje)
        {
            bool vacio = c is ComboBox cb ? cb.SelectedItem == null : string.IsNullOrWhiteSpace(c.Text);
            ep.SetError(c, vacio ? mensaje : "");
            c.BackColor = vacio ? Color.MistyRose : SystemColors.Window;
            return !vacio;
        }

        /// <summary>Formato estándar para los DataGridView de solo lectura.</summary>
        public static void Grid(DataGridView g)
        {
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            g.RowHeadersWidth = 24;
            g.BackgroundColor = SystemColors.Window;
            g.DataBindingComplete += (s, e) => Formatear(g);
        }

        /// <summary>
        /// Divulgación progresiva: la tabla muestra solo estas columnas, en este
        /// orden; el resto queda en el panel de detalle de la pantalla. Los
        /// encabezados opcionales se escriben "columna=Encabezado".
        /// </summary>
        public static void Mostrar(DataGridView g, params string[] columnas)
        {
            g.Tag = columnas;
            if (g.Columns.Count > 0) Formatear(g);
        }

        // Encabezados en español natural (sin nombres de columnas de la base)
        private static readonly System.Collections.Generic.Dictionary<string, string> Encabezados =
            new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["sku"] = "SKU", ["nombre"] = "Nombre", ["precio_venta"] = "Precio", ["stock_actual"] = "Stock", ["stock_minimo"] = "Mínimo",
            ["alerta_stock"] = "Estado", ["margen_pct"] = "Margen %", ["total_comprado"] = "Total comprado", ["nivel"] = "Nivel",
            ["razon_social"] = "Proveedor", ["rfc"] = "RFC", ["telefono"] = "Teléfono", ["correo"] = "Correo",
            ["equipo_descripcion"] = "Equipo", ["dias_en_taller"] = "Días", ["numero_serie"] = "No. de serie",
            ["garantia_hasta"] = "Garantía hasta", ["costo_proveedor"] = "Costo", ["usuario_app"] = "Usuario",
            ["valor_anterior"] = "Antes", ["valor_nuevo"] = "Después", ["ultimo_acceso"] = "Último acceso",
            ["unidades_vendidas"] = "Unidades", ["utilidad_bruta"] = "Utilidad", ["stock"] = "Stock", ["minimo"] = "Mínimo",
        };

        /// <summary>Encabezados legibles y formato de moneda/fecha según el nombre de columna.</summary>
        public static void Formatear(DataGridView g)
        {
            var mostrar = g.Tag as string[];
            var claves = mostrar?.Select(x => x.Split('=')[0].ToLowerInvariant()).ToList();
            foreach (DataGridViewColumn col in g.Columns)
            {
                string n = col.DataPropertyName.ToLowerInvariant();
                bool oculta = n.StartsWith("id_") || n == "id" || n == "idproducto" || n == "es_final"
                              || (claves != null && !claves.Contains(n))
                              || (!(Db.Sesion?.Puede(Capacidad.VerCostos) ?? true) && EsCosto(n));   // la base no le deja ver costos
                if (oculta) { col.Visible = false; continue; }
                col.Visible = true;
                int i = claves?.IndexOf(n) ?? -1;
                if (i >= 0) col.DisplayIndex = Math.Min(i, g.Columns.Count - 1);
                string propio = i >= 0 && mostrar[i].Contains("=") ? mostrar[i].Split('=')[1] : null;
                string conocido;
                col.HeaderText = propio ?? (Encabezados.TryGetValue(n, out conocido) ? conocido : Titulo(col.DataPropertyName));
                if (n.Contains("precio") || n.Contains("total") || n.Contains("subtotal") || n.Contains("costo") ||
                    n.Contains("ingresos") || n.Contains("vendido") || n.Contains("utilidad") || n.Contains("descuento") ||
                    n.Contains("ticket") || n == "importe")
                {
                    col.DefaultCellStyle.Format = "C2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (n.Contains("fecha") || n == "registro" || n.Contains("hasta"))
                    col.DefaultCellStyle.Format = n.Contains("fecha") && n != "fecha_fin" ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";
            }
        }

        /// <summary>Columnas que revelan costos o márgenes (solo con la capacidad ver_costos).</summary>
        public static bool EsCosto(string columna)
        {
            string n = columna.ToLowerInvariant();
            return n.Contains("costo") || n.Contains("margen") || n.Contains("utilidad") || n == "valor_inventario";
        }

        private static string Titulo(string columna)
        {
            string t = columna.Replace('_', ' ');
            return t.Length == 0 ? t : char.ToUpper(t[0]) + t.Substring(1);
        }
    }
}
