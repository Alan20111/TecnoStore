using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>Mensajes, validación visual y formato de DataGridView.</summary>
    public static class Ui
    {
        public static void Error(Exception ex, string titulo = "Error")
        {
            // Los THROW de los procedimientos llegan como SqlException con el mensaje de negocio.
            string msg = ex is SqlException sql && sql.Number >= 50000 ? sql.Message
                       : ex is SqlException sql2 && sql2.Number == 229 ? "Tu perfil no tiene permiso para esta operación.\n\n" + sql2.Message
                       : ex.Message;
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

        /// <summary>Encabezados legibles y formato de moneda/fecha según el nombre de columna.</summary>
        public static void Formatear(DataGridView g)
        {
            foreach (DataGridViewColumn col in g.Columns)
            {
                string n = col.DataPropertyName.ToLowerInvariant();
                if (n.StartsWith("id_") || n == "id" || n == "idproducto" || n == "es_final") { col.Visible = false; continue; }
                col.HeaderText = Titulo(col.DataPropertyName);
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

        private static string Titulo(string columna)
        {
            string t = columna.Replace('_', ' ');
            return t.Length == 0 ? t : char.ToUpper(t[0]) + t.Substring(1);
        }
    }
}
