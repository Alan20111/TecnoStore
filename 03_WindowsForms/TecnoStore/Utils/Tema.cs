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
    /// monocromático, al estilo de shadcn/ui. Lienzo gris claro, tarjetas blancas
    /// con línea fina y radio 24, botones y campos en píldora de radio 18, texto
    /// en tinta. El rojo es el único color y solo aparece en errores y en acciones
    /// que eliminan o cancelan. Encima, tamaños grandes: letras, botones y filas.
    /// Se aplica en tiempo de ejecución a cualquier formulario, así los
    /// .Designer.cs siguen siendo editables en Visual Studio.
    /// </summary>
    public static class Tema
    {
        // ---- Color (tres tonos de superficie, tinta y un solo rojo) ----
        public static readonly Color Fondo = Color.FromArgb(245, 245, 245);        // canvas
        public static readonly Color Tarjeta = Color.White;                        // paper
        public static readonly Color Lateral = Color.FromArgb(250, 250, 250);      // surface alt
        public static readonly Color Linea = Color.FromArgb(229, 229, 229);        // hairline
        public static readonly Color LineaSuave = Linea;
        public static readonly Color Anillo = Color.FromArgb(115, 115, 115);       // foco (más visible que la línea fina)
        public static readonly Color Ceniza = Color.FromArgb(163, 163, 163);       // solo lo deshabilitado
        public static readonly Color TextoSuave = Color.FromArgb(115, 115, 115);   // mid gray
        public static readonly Color Texto = Color.FromArgb(10, 10, 10);           // ink
        public static readonly Color Navegacion = Color.FromArgb(23, 23, 23);      // ink soft: dónde estás
        public static readonly Color NavegacionSuave = Color.FromArgb(245, 245, 245);
        public static readonly Color Accion = Color.FromArgb(10, 10, 10);          // botón principal
        public static readonly Color AccionHover = Color.FromArgb(38, 38, 38);
        public static readonly Color Carbon = Color.White;                         // barra de título
        public static readonly Color Verde = TextoSuave;                           // "todo bien" no necesita color
        public static readonly Color Ambar = Texto;                                // "ojo": tinta, más fuerte que el gris
        public static readonly Color Rojo = Color.FromArgb(231, 0, 11);            // ember: errores y eliminar
        public static readonly Color ErrorFondo = Color.FromArgb(255, 245, 245);

        // ---- Tipografía: Segoe UI Variable en Windows 11, Segoe UI en Windows 10 ----
        // (Geist o Inter si la PC las tiene instaladas, que es la letra del estilo)
        private static readonly string Familia = Existe("Geist") ? "Geist" : Existe("Inter") ? "Inter" : Existe("Segoe UI Variable Text") ? "Segoe UI Variable Text" : "Segoe UI";
        private static readonly string FamiliaDisplay = Existe("Geist") ? "Geist" : Existe("Inter") ? "Inter" : Existe("Segoe UI Variable Display") ? "Segoe UI Variable Display" : "Segoe UI";
        private static readonly string FamiliaFuerte = Existe("Geist SemiBold") ? "Geist SemiBold" : Existe("Inter SemiBold") ? "Inter SemiBold" : Existe("Segoe UI Variable Display Semib") ? "Segoe UI Variable Display Semib"
                                                     : Existe("Segoe UI Semibold") ? "Segoe UI Semibold" : "Segoe UI";
        // Letras grandes: se leen de lejos
        public static readonly Font Fuente = new Font(Familia, 11F);
        public static readonly Font FuentePequena = new Font(Familia, 9.5F);
        public static readonly Font FuenteNegrita = new Font(FamiliaFuerte, 11F);
        public static readonly Font FuenteBoton = new Font(FamiliaFuerte, 11.5F);
        public static readonly Font FuenteCampo = new Font(Familia, 13F);
        public static readonly Font FuenteSubtitulo = new Font(FamiliaFuerte, 14F);
        public static readonly Font FuenteTitulo = new Font(FamiliaFuerte, 20F);
        public static readonly Font FuenteKpi = new Font(FamiliaFuerte, 20F);
        public static readonly Font FuenteCifra = new Font(FamiliaFuerte, 28F);
        public static readonly Font FuenteTotal = new Font(FamiliaFuerte, 34F);
        public static readonly Font FuenteDisplay = new Font(FamiliaDisplay, 15F);

        // Fuente de íconos de Windows (Segoe Fluent Icons en 11, MDL2 Assets en 10); null si no hay
        private static readonly string FamiliaIconos = Existe("Segoe Fluent Icons") ? "Segoe Fluent Icons"
                                                     : Existe("Segoe MDL2 Assets") ? "Segoe MDL2 Assets" : null;
        private static readonly System.Collections.Generic.Dictionary<float, Font> iconos = new System.Collections.Generic.Dictionary<float, Font>();
        public static Font FuenteIconos(float tamano)
        {
            if (FamiliaIconos == null) return null;
            Font f;
            if (!iconos.TryGetValue(tamano, out f)) iconos[tamano] = f = new Font(FamiliaIconos, tamano);
            return f;
        }

        /// <summary>Alto de botones: cómodo para el dedo y el mouse (44 px como mínimo en Apple; aquí 48).</summary>
        public const int AltoBoton = 48;

        private static bool Existe(string familia) =>
            FontFamily.Families.Any(f => string.Equals(f.Name, familia, StringComparison.OrdinalIgnoreCase));

        // ---- Forma ----
        public const int RadioTarjeta = 24, RadioCampo = 18, RadioBoton = 18, RadioNav = 18;

        // Botones con la acción principal de su pantalla (tinta sobre blanco)
        private static readonly string[] Primarios =
            { "btnGuardar", "btnIngresar", "btnRegistrar", "btnAplicarStock", "btnNuevo", "btnCobrar", "btnRecibir", "btnSiguiente" };

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

        /// <summary>Un campo va en gris sobre una superficie blanca y en blanco sobre el lienzo gris.</summary>
        public static Color FondoCampo(Control c)
        {
            var p = c.Parent;
            while (p is Caja) p = p.Parent;
            return p != null && p.BackColor.ToArgb() == Tarjeta.ToArgb() ? Fondo : Tarjeta;
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
                    t.BorderStyle = t.Parent is Caja ? BorderStyle.None : BorderStyle.FixedSingle;
                    t.BackColor = FondoCampo(t);
                    t.Font = FuenteCampo;
                    break;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.BackColor = FondoCampo(cb);
                    cb.Font = FuenteCampo;
                    break;
                case NumericUpDown n:
                    n.BorderStyle = n.Parent is Caja ? BorderStyle.None : BorderStyle.FixedSingle;
                    n.BackColor = FondoCampo(n);
                    n.Font = FuenteCampo;
                    break;
                case DateTimePicker dt:
                    dt.Font = FuenteCampo;
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
            public bool Primario, Peligro;
            public double Hover;       // 0 → 1, se anima
            public bool Presionado;
            public bool Teclado;       // el foco llegó con Tab, no con el mouse
            public Timer Anim;
        }

        public static void Boton(Button b, bool primario)
        {
            if (b is BotonGrande) return;   // se dibuja solo
            if (b.Tag is EstadoBoton ya) { ya.Primario |= primario; b.Invalidate(); return; }
            var st = new EstadoBoton { Primario = primario };
            b.Tag = st;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.Transparent;
            b.FlatAppearance.MouseDownBackColor = Color.Transparent;
            b.Cursor = Cursors.Hand;
            b.Font = FuenteBoton;
            b.UseVisualStyleBackColor = false;
            if (b.Height < AltoBoton) b.Height = AltoBoton;

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

        /// <summary>Acción que deshace o elimina: letra roja, sin relleno.</summary>
        public static void Peligro(Button b)
        {
            Boton(b, false);
            ((EstadoBoton)b.Tag).Peligro = true;
            b.Invalidate();
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
            Color atras = b.Parent?.BackColor ?? Fondo;
            bool sobreBlanco = atras.ToArgb() == Tarjeta.ToArgb();
            if (!b.Enabled) { fondo = sobreBlanco ? Fondo : Lateral; texto = Ceniza; borde = sobreBlanco ? fondo : Linea; }
            else if (st.Primario)
            {
                fondo = Animacion.Mezclar(Accion, AccionHover, st.Hover);
                texto = Lateral; borde = fondo;
            }
            else
            {
                // Sobre una tarjeta: gris sin borde. Sobre el lienzo: papel con línea fina.
                fondo = sobreBlanco ? Animacion.Mezclar(Fondo, Linea, st.Hover) : Animacion.Mezclar(Tarjeta, Lateral, st.Hover);
                texto = st.Peligro ? Rojo : Texto;
                borde = sobreBlanco ? fondo : Animacion.Mezclar(Linea, Color.FromArgb(212, 212, 212), st.Hover);
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
            }
            if (b.Focused && b.FindForm()?.ActiveControl == b && (Control.ModifierKeys & Keys.Alt) == 0 && st.Teclado)   // anillo de foco (solo al navegar con teclado)
                using (var anillo = new Pen(Anillo, 2f))
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
            g.GridColor = Fondo;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoSuave;
            g.ColumnHeadersDefaultCellStyle.Font = Fuente;
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tarjeta;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = g.Name == "compacta" ? 36 : 44;
            g.RowHeadersVisible = false;
            g.RowTemplate.Height = g.Name == "compacta" ? 34 : 46;
            g.DefaultCellStyle.BackColor = Tarjeta;
            g.DefaultCellStyle.Font = Fuente;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            g.DefaultCellStyle.SelectionBackColor = NavegacionSuave;
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.AlternatingRowsDefaultCellStyle.BackColor = Tarjeta;
            BarraDelgada.Instalar(g);
        }
    }
}
