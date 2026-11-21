using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Sistema de diseño de TecnoStore (ver 01_Documentacion/GuiaDiseno.md):
    ///  - Perplexity: lienzo pergamino cálido, tinta, línea de lápiz y verde
    ///    azulado SOLO para navegación y selección.
    ///  - Brex: tipografía compacta y un solo naranja SOLO para la acción principal.
    ///  - Apple: esquinas continuas, animaciones con resorte, presión visible,
    ///    barras de desplazamiento delgadas superpuestas.
    /// Se aplica en tiempo de ejecución a cualquier formulario, así los
    /// .Designer.cs siguen siendo editables en Visual Studio.
    /// </summary>
    public static class Tema
    {
        // ---- Color ----
        public static readonly Color Fondo = Color.FromArgb(250, 248, 245);        // pergamino
        public static readonly Color Tarjeta = Color.FromArgb(253, 251, 250);      // papel
        public static readonly Color Lateral = Color.FromArgb(243, 240, 234);      // barra lateral
        public static readonly Color Linea = Color.FromArgb(209, 209, 205);        // línea de lápiz
        public static readonly Color LineaSuave = Color.FromArgb(231, 228, 222);
        public static readonly Color Ceniza = Color.FromArgb(146, 145, 139);
        public static readonly Color TextoSuave = Color.FromArgb(114, 112, 107);   // grafito
        public static readonly Color Texto = Color.FromArgb(39, 37, 30);           // tinta
        public static readonly Color Navegacion = Color.FromArgb(1, 106, 113);     // verde azulado
        public static readonly Color NavegacionSuave = Color.FromArgb(229, 240, 241);
        public static readonly Color Accion = Color.FromArgb(255, 89, 0);          // brasa
        public static readonly Color AccionHover = Color.FromArgb(232, 81, 0);
        public static readonly Color Carbon = Color.FromArgb(21, 25, 30);          // barra de título
        public static readonly Color Verde = Color.FromArgb(47, 158, 90);
        public static readonly Color Rojo = Color.FromArgb(214, 64, 47);
        public static readonly Color Ambar = Color.FromArgb(183, 121, 31);

        // Compatibilidad con pantallas anteriores
        public static Color Acento => Navegacion;
        public static Color Borde => LineaSuave;
        public static Color Seleccion => NavegacionSuave;
        public static Color LateralActivo => Navegacion;

        // ---- Tipografía: Segoe UI Variable en Windows 11, Segoe UI en Windows 10 ----
        private static readonly string Familia = Existe("Segoe UI Variable Text") ? "Segoe UI Variable Text" : "Segoe UI";
        private static readonly string FamiliaDisplay = Existe("Segoe UI Variable Display") ? "Segoe UI Variable Display" : "Segoe UI";
        private static readonly string FamiliaFuerte = Existe("Segoe UI Variable Display Semib") ? "Segoe UI Variable Display Semib"
                                                     : Existe("Segoe UI Semibold") ? "Segoe UI Semibold" : "Segoe UI";
        public static readonly Font Fuente = new Font(Familia, 9.75F);
        public static readonly Font FuentePequena = new Font(Familia, 8.75F);
        public static readonly Font FuenteNegrita = new Font(FamiliaFuerte, 9.75F);
        public static readonly Font FuenteTitulo = new Font(FamiliaFuerte, 18F);
        public static readonly Font FuenteKpi = new Font(FamiliaFuerte, 20F);
        public static readonly Font FuenteDisplay = new Font(FamiliaDisplay, 15F);

        private static bool Existe(string familia) =>
            FontFamily.Families.Any(f => string.Equals(f.Name, familia, StringComparison.OrdinalIgnoreCase));

        // ---- Forma ----
        public const int RadioTarjeta = 16, RadioCampo = 12, RadioBoton = 12, RadioNav = 10;

        // Botones con la acción principal de su pantalla (la brasa)
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
                    t.BackColor = Tarjeta;
                    t.Font = Fuente;
                    break;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.BackColor = Tarjeta;
                    cb.Font = Fuente;
                    break;
                case NumericUpDown n:
                    n.BorderStyle = BorderStyle.FixedSingle;
                    n.BackColor = Tarjeta;
                    break;
                case CheckBox ch:
                    ch.FlatStyle = FlatStyle.Flat;
                    ch.FlatAppearance.BorderColor = Linea;
                    ch.FlatAppearance.CheckedBackColor = Navegacion;
                    ch.Cursor = Cursors.Hand;
                    break;
                case GroupBox gb:
                    gb.ForeColor = TextoSuave;
                    gb.Font = FuenteNegrita;
                    gb.BackColor = Tarjeta;
                    foreach (Control h in gb.Controls) { if (!(h is Button)) { h.Font = Fuente; h.ForeColor = Texto; } }
                    break;
                case Panel p when p.Name.StartsWith("pnlFiltros", StringComparison.Ordinal) || p.Name.StartsWith("pnlArriba", StringComparison.Ordinal):
                    p.BackColor = Tarjeta;
                    p.Padding = new Padding(8);
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

        // ================= Botones dibujados (esquina continua + animación) =================
        private class EstadoBoton
        {
            public bool Primario;
            public double Hover;       // 0 → 1, se anima
            public bool Presionado;
            public bool Teclado;       // el foco llegó con Tab, no con el mouse
            public Timer Anim;
        }

        public static void Boton(Button b, bool primario)
        {
            if (b.Tag is EstadoBoton) return;
            var st = new EstadoBoton { Primario = primario };
            b.Tag = st;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.Transparent;
            b.FlatAppearance.MouseDownBackColor = Color.Transparent;
            b.Cursor = Cursors.Hand;
            b.Font = primario ? FuenteNegrita : Fuente;
            b.UseVisualStyleBackColor = false;
            if (b.Height < 32) b.Height = 32;

            // Transición de color al entrar/salir (no un salto brusco)
            void AnimarHover(double destino)
            {
                st.Anim?.Stop();
                double desde = st.Hover;
                st.Anim = Animacion.Ejecutar(160, Animacion.Salida, p => { st.Hover = Animacion.Entre(desde, destino, p); b.Invalidate(); });
            }
            b.MouseEnter += (s, e) => AnimarHover(1);
            b.MouseLeave += (s, e) => { st.Presionado = false; AnimarHover(0); };
            b.MouseDown += (s, e) => { st.Presionado = true; b.Invalidate(); };
            b.MouseUp += (s, e) => { st.Presionado = false; b.Invalidate(); };
            b.EnabledChanged += (s, e) => b.Invalidate();
            b.Enter += (s, e) => { st.Teclado = (Control.MouseButtons == MouseButtons.None); b.Invalidate(); };
            b.Leave += (s, e) => { st.Teclado = false; b.Invalidate(); };
            b.MouseDown += (s, e) => st.Teclado = false;
            b.Paint += PintarBoton;
        }

        private static void PintarBoton(object sender, PaintEventArgs e)
        {
            var b = (Button)sender;
            var st = (EstadoBoton)b.Tag;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(b.Parent?.BackColor ?? Fondo);

            Color fondo, texto, borde;
            if (!b.Enabled) { fondo = Color.FromArgb(242, 240, 236); texto = Ceniza; borde = LineaSuave; }
            else if (st.Primario)
            {
                fondo = Animacion.Mezclar(Accion, AccionHover, st.Hover);
                texto = Color.White; borde = fondo;
            }
            else
            {
                fondo = Animacion.Mezclar(Tarjeta, Color.FromArgb(245, 242, 237), st.Hover);
                texto = Texto; borde = Animacion.Mezclar(Linea, Color.FromArgb(189, 188, 182), st.Hover);
            }

            // Al presionar se "hunde" 1 px y se oscurece un poco (equivale a la escala 0.97 de iOS)
            float inset = st.Presionado && b.Enabled ? 1.5f : 0.5f;
            if (st.Presionado && b.Enabled) fondo = Animacion.Mezclar(fondo, Color.Black, st.Primario ? 0.08 : 0.04);
            var r = new RectangleF(inset, inset, b.Width - 1 - inset * 2, b.Height - 1 - inset * 2);
            using (var path = Forma.Continua(r, RadioBoton))
            using (var relleno = new SolidBrush(fondo))
            using (var linea = new Pen(borde))
            {
                g.FillPath(relleno, path);
                if (!st.Primario || !b.Enabled) g.DrawPath(linea, path);
                if (st.Primario && b.Enabled)   // brillo superior sutil
                    using (var brillo = new Pen(Color.FromArgb(55, 255, 255, 255)))
                        g.DrawLine(brillo, r.Left + RadioBoton, r.Top + 1, r.Right - RadioBoton, r.Top + 1);
            }
            if (b.Focused && b.FindForm()?.ActiveControl == b && (Control.ModifierKeys & Keys.Alt) == 0 && st.Teclado)   // anillo de foco (solo al navegar con teclado)
                using (var anillo = new Pen(Color.FromArgb(90, Navegacion), 2f))
                using (var p2 = Forma.Continua(new RectangleF(1, 1, b.Width - 3, b.Height - 3), RadioBoton + 1))
                    g.DrawPath(anillo, p2);

            TextRenderer.DrawText(g, b.Text, b.Font, Rectangle.Round(r), texto,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        // ================= Tablas =================
        public static void Tabla(DataGridView g)
        {
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Tarjeta;
            g.GridColor = Color.FromArgb(241, 238, 233);
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoSuave;
            g.ColumnHeadersDefaultCellStyle.Font = Fuente;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 38;
            g.RowHeadersVisible = false;
            g.RowTemplate.Height = 32;
            g.DefaultCellStyle.BackColor = Tarjeta;
            g.DefaultCellStyle.Font = Fuente;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.SelectionBackColor = NavegacionSuave;
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 247, 243);
            BarraDelgada.Instalar(g);
        }
    }
}
