using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Estilo de panel de administración de MacStore. Se aplica en tiempo de
    /// ejecución a cualquier formulario, así los .Designer.cs siguen siendo
    /// editables en Visual Studio y el estilo vive en un solo lugar.
    /// </summary>
    public static class Tema
    {
        public static readonly Color Fondo = Color.FromArgb(245, 245, 247);
        public static readonly Color Tarjeta = Color.White;
        public static readonly Color Texto = Color.FromArgb(29, 29, 31);
        public static readonly Color TextoSuave = Color.FromArgb(110, 110, 115);
        public static readonly Color Borde = Color.FromArgb(229, 229, 234);
        public static readonly Color Acento = Color.FromArgb(0, 113, 227);
        public static readonly Color AcentoHover = Color.FromArgb(0, 98, 204);
        public static readonly Color Lateral = Color.FromArgb(29, 29, 31);
        public static readonly Color LateralHover = Color.FromArgb(52, 52, 56);
        public static readonly Color LateralActivo = Color.FromArgb(0, 113, 227);
        public static readonly Color Verde = Color.FromArgb(52, 199, 89);
        public static readonly Color Naranja = Color.FromArgb(255, 149, 0);
        public static readonly Color Rojo = Color.FromArgb(255, 59, 48);
        public static readonly Color Seleccion = Color.FromArgb(232, 240, 254);

        public static readonly Font Fuente = new Font("Segoe UI", 9.75F);
        public static readonly Font FuenteNegrita = new Font("Segoe UI Semibold", 9.75F);
        public static readonly Font FuenteTitulo = new Font("Segoe UI Semibold", 18F);
        public static readonly Font FuenteKpi = new Font("Segoe UI Semibold", 20F);

        // Botones que llevan la acción principal de su pantalla
        private static readonly string[] Primarios =
            { "btnGuardar", "btnIngresar", "btnBuscar", "btnRegistrar", "btnGenerar", "btnAplicar", "btnAgregar" };

        public static void Aplicar(Control raiz)
        {
            if (raiz is Form f)
            {
                f.BackColor = Fondo;
                f.Font = Fuente;
                f.ForeColor = Texto;
            }
            foreach (Control c in raiz.Controls) Estilo(c);
        }

        private static void Estilo(Control c)
        {
            switch (c)
            {
                case Button b:
                    Boton(b, Primarios.Any(p => b.Name.StartsWith(p, StringComparison.Ordinal)));
                    break;
                case DataGridView g:
                    Tabla(g);
                    break;
                case TextBox t:
                    t.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case GroupBox gb:
                    gb.ForeColor = TextoSuave;
                    gb.Font = FuenteNegrita;
                    gb.BackColor = Tarjeta;
                    foreach (Control h in gb.Controls) { h.Font = Fuente; if (!(h is Button)) h.ForeColor = Texto; }
                    break;
                case Panel p when p.Name.StartsWith("pnlFiltros", StringComparison.Ordinal) || p.Name.StartsWith("pnlArriba", StringComparison.Ordinal):
                    p.BackColor = Tarjeta;
                    p.Padding = new Padding(6);
                    break;
                case Label l when l.Name == "lblConteo":
                    l.ForeColor = TextoSuave;
                    break;
                case TabControl tc:
                    tc.Font = FuenteNegrita;
                    break;
            }
            foreach (Control h in c.Controls) Estilo(h);
        }

        public static void Boton(Button b, bool primario)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.Cursor = Cursors.Hand;
            b.Font = primario ? FuenteNegrita : Fuente;
            b.FlatAppearance.BorderSize = primario ? 0 : 1;
            b.FlatAppearance.BorderColor = Borde;
            b.BackColor = primario ? Acento : Tarjeta;
            b.ForeColor = primario ? Color.White : Texto;
            b.FlatAppearance.MouseOverBackColor = primario ? AcentoHover : Fondo;
            b.UseVisualStyleBackColor = false;
            if (b.Height < 30) b.Height = 30;
            b.EnabledChanged -= BotonHabilitado;
            b.EnabledChanged += BotonHabilitado;
            BotonHabilitado(b, EventArgs.Empty);
        }

        // Un botón deshabilitado se ve gris aunque sea primario
        private static void BotonHabilitado(object sender, EventArgs e)
        {
            var b = (Button)sender;
            bool primario = b.Font.Bold || b.Font.Name.Contains("Semibold");
            if (!b.Enabled) { b.BackColor = Fondo; b.ForeColor = TextoSuave; }
            else { b.BackColor = primario ? Acento : Tarjeta; b.ForeColor = primario ? Color.White : Texto; }
        }

        public static void Tabla(DataGridView g)
        {
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Tarjeta;
            g.GridColor = Borde;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoSuave;
            g.ColumnHeadersDefaultCellStyle.Font = FuenteNegrita;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 36;
            g.RowHeadersVisible = false;
            g.RowTemplate.Height = 30;
            g.DefaultCellStyle.Font = Fuente;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.DefaultCellStyle.SelectionBackColor = Seleccion;
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 252);
        }
    }
}
