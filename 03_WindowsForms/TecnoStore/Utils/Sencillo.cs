using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>Una pantalla que vuelve a leer sus datos cada vez que se abre.</summary>
    public interface IRefrescable { void Refrescar(); }

    /// <summary>Un campo de captura: su etiqueta y su control.</summary>
    public class Campo
    {
        public string Texto;
        public Control Control;
        /// <summary>true = ocupa todo el ancho aunque haya dos columnas.</summary>
        public bool Ancho;
        public int Alto;

        public Campo(string texto, Control control, bool ancho = false, int alto = 0)
        {
            Texto = texto; Control = control; Ancho = ancho; Alto = alto;
        }
    }

    /// <summary>
    /// Caja redondeada de 46 px para un campo de captura. Se le pinta un anillo
    /// con el foco y una línea roja si el campo tiene error; tocar el borde enfoca el campo.
    /// </summary>
    public class Caja : Panel
    {
        public const int Alto = 46;
        private readonly Control campo;

        public Caja(Control campo)
        {
            this.campo = campo;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            Height = Alto;
            Cursor = Cursors.IBeam;
            campo.Anchor = AnchorStyles.None;
            campo.Dock = DockStyle.None;
            Controls.Add(campo);
            campo.Enter += (s, e) => Invalidate();
            campo.Leave += (s, e) => Invalidate();
            campo.BackColorChanged += (s, e) => Invalidate();
            campo.EnabledChanged += (s, e) => Invalidate();
            Click += (s, e) => campo.Focus();
        }

        /// <summary>La caja de un campo, si ya tiene.</summary>
        public static Control De(Control c) => c.Parent as Caja ?? c;

        /// <summary>true si el campo se ve mejor dentro de una caja.</summary>
        public static bool Admite(Control c) => c is TextBox || c is ComboBox || c is NumericUpDown;

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            bool multi = campo is TextBox t && t.Multiline;
            // A la derecha queda lugar para el ícono de error
            int w = Math.Max(20, Width - 14 - (campo is ComboBox ? 8 : 28));
            int h = multi ? Height - 16 : campo.Height;
            campo.SetBounds(14, multi ? 8 : (Height - h) / 2, w, h);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Parent?.BackColor ?? Tema.Fondo);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            bool foco = campo.ContainsFocus;
            using (var p = Forma.Continua(new RectangleF(1, 1, Width - 3, Height - 3), Tema.RadioCampo))
            using (var b = new SolidBrush(campo.BackColor))
            {
                g.FillPath(b, p);
                // Sin borde en reposo si ya contrasta con su fondo; anillo al enfocar; rojo si hay error
                bool error = campo.BackColor.ToArgb() == Tema.ErrorFondo.ToArgb(), gris = campo.BackColor.ToArgb() == Tema.Fondo.ToArgb();
                if (error || foco || !gris)
                    using (var l = new Pen(error ? Tema.Rojo : foco ? Tema.Anillo : Tema.Linea, error || foco ? 1.6f : 1f)) g.DrawPath(l, p);
            }
        }
    }

    /// <summary>
    /// Acomoda campos grandes con su etiqueta arriba, en una o dos columnas.
    /// Los campos ocultos no dejan hueco.
    /// </summary>
    public class PanelCampos : Panel
    {
        private readonly List<Campo> campos = new List<Campo>();
        private readonly Dictionary<Campo, Label> etiquetas = new Dictionary<Campo, Label>();
        private readonly HashSet<Control> ocultos = new HashSet<Control>();
        private readonly int columnas;
        private const int AltoEtiqueta = 24, Separacion = 14, Entre = 18;

        public PanelCampos(int columnas, params Campo[] lista)
        {
            this.columnas = Math.Max(1, columnas);
            BackColor = Tema.Fondo;
            foreach (var c in lista)
            {
                campos.Add(c);
                if (c.Texto != null && !(c.Control is CheckBox))
                {
                    var l = new Label { Text = c.Texto, AutoSize = false, Height = AltoEtiqueta, ForeColor = Tema.TextoSuave, Font = Tema.Fuente, TextAlign = ContentAlignment.BottomLeft };
                    etiquetas[c] = l;
                    Controls.Add(l);
                }
                Controls.Add(Caja.Admite(c.Control) ? new Caja(c.Control) : c.Control);
            }
        }

        public void Mostrar(Control control, bool visible)
        {
            if (visible) ocultos.Remove(control); else ocultos.Add(control);
            Acomodar();
        }

        /// <summary>Cambia la etiqueta de un campo ("Contraseña *", "Precio base"…).</summary>
        public void Etiqueta(Control control, string texto)
        {
            var c = campos.Find(x => x.Control == control);
            if (c != null && etiquetas.ContainsKey(c)) etiquetas[c].Text = texto;
        }

        /// <summary>Alto que necesita el contenido.</summary>
        public int AltoNecesario { get; private set; }

        protected override void OnLayout(LayoutEventArgs e) { base.OnLayout(e); Acomodar(); }

        private void Acomodar()
        {
            int ancho = Math.Max(120, ClientSize.Width - Padding.Horizontal - 4);
            int anchoCol = (ancho - Entre * (columnas - 1)) / columnas;
            int y = Padding.Top + AutoScrollPosition.Y, col = 0, altoFila = 0;
            foreach (var c in campos)
            {
                bool oculto = ocultos.Contains(c.Control);
                var caja = Caja.De(c.Control);
                caja.Visible = !oculto;
                Label l;
                if (etiquetas.TryGetValue(c, out l)) l.Visible = !oculto;
                if (oculto) continue;
                bool ancha = c.Ancho || columnas == 1;
                if (ancha && col > 0) { y += altoFila; col = 0; altoFila = 0; }

                int x = Padding.Left + col * (anchoCol + Entre);
                int w = ancha ? ancho : anchoCol, yc = y;
                if (l != null) { l.SetBounds(x, y, w, AltoEtiqueta); yc += AltoEtiqueta + 2; }
                int alto = c.Alto > 0 ? c.Alto : caja is Caja ? Caja.Alto : c.Control is Button ? Tema.AltoBoton : c.Control is CheckBox ? 36 : c.Control.Height;
                caja.SetBounds(x, yc, w, alto);
                int total = yc - y + alto + Separacion;
                altoFila = Math.Max(altoFila, total);

                if (ancha || ++col >= columnas) { y += altoFila; col = 0; altoFila = 0; }
            }
            AltoNecesario = y + altoFila + Padding.Bottom;
        }
    }

    /// <summary>
    /// Barra de botones grandes para cambiar de vista o de filtro. La pastilla
    /// se desliza hasta el elegido.
    /// </summary>
    public class Segmentos : Control
    {
        private readonly List<string> textos = new List<string>();
        private int activo, sobre = -1;
        private float pastilla = -1;
        private Timer anim;
        private const int AnchoMinimo = 96;
        public event Action<int> Elegido;

        public Segmentos(params string[] opciones)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            Height = Tema.AltoBoton;
            Font = Tema.FuenteBoton;
            Cursor = Cursors.Hand;
            TabStop = true;
            Poner(opciones);
        }

        public void Poner(params string[] opciones)
        {
            textos.Clear(); textos.AddRange(opciones);
            activo = 0; pastilla = -1;
            Width = AnchoIdeal;
            Invalidate();
        }

        public int AnchoIdeal => 8 + textos.Sum(t => Math.Max(AnchoMinimo, TextRenderer.MeasureText(t, Font).Width + 34));

        public int Activo
        {
            get { return activo; }
            set
            {
                if (value < 0 || value >= textos.Count || value == activo) return;
                activo = value;
                float desde = pastilla, hasta = Area(activo).X;
                anim?.Stop();
                if (desde < 0) { pastilla = hasta; Invalidate(); return; }
                anim = Animacion.Ejecutar(220, Animacion.Salida, p => { if (!IsDisposed) { pastilla = (float)Animacion.Entre(desde, hasta, p); Invalidate(); } });
            }
        }

        private RectangleF Area(int i)
        {
            float x = 4;
            for (int k = 0; k < textos.Count; k++)
            {
                float w = Math.Max(AnchoMinimo, TextRenderer.MeasureText(textos[k], Font).Width + 34);
                if (k == i) return new RectangleF(x, 4, w, Height - 9);
                x += w;
            }
            return RectangleF.Empty;
        }

        private int En(Point p)
        {
            for (int i = 0; i < textos.Count; i++) if (Area(i).Contains(p.X, p.Y)) return i;
            return -1;
        }

        private void Elegir(int i)
        {
            if (i < 0 || i >= textos.Count || i == activo) return;
            Activo = i;
            Elegido?.Invoke(i);
        }

        protected override void OnMouseMove(MouseEventArgs e) { int i = En(e.Location); if (i != sobre) { sobre = i; Invalidate(); } base.OnMouseMove(e); }
        protected override void OnMouseLeave(EventArgs e) { sobre = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e) { Elegir(En(e.Location)); base.OnMouseClick(e); }
        protected override bool IsInputKey(Keys k) => k == Keys.Left || k == Keys.Right || base.IsInputKey(k);
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) Elegir(activo - 1);
            else if (e.KeyCode == Keys.Right) Elegir(activo + 1);
            base.OnKeyDown(e);
        }
        private bool Teclado { get; set; }   // el foco llegó con Tab
        protected override void OnGotFocus(EventArgs e) { Teclado = MouseButtons == MouseButtons.None; Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Tema.Fondo);
            if (textos.Count == 0) return;
            if (pastilla < 0) pastilla = Area(activo).X;
            float fin = Area(textos.Count - 1).Right;
            using (var p = Forma.Continua(new RectangleF(0.5f, 0.5f, fin + 3, Height - 2), Tema.RadioBoton))
            using (var b = new SolidBrush(Parent != null && Parent.BackColor.ToArgb() == Tema.Tarjeta.ToArgb() ? Tema.Fondo : Color.FromArgb(234, 234, 234)))
            {
                g.FillPath(b, p);
                if (Focused && Teclado) using (var l = new Pen(Tema.Anillo, 1.6f)) g.DrawPath(l, p);
            }
            var a = Area(activo);
            using (var p = Forma.Continua(new RectangleF(pastilla, a.Y, a.Width, a.Height), Tema.RadioBoton - 4))
            using (var b = new SolidBrush(Tema.Tarjeta))
            using (var l = new Pen(Tema.Linea))
            { g.FillPath(b, p); g.DrawPath(l, p); }
            for (int i = 0; i < textos.Count; i++)
                TextRenderer.DrawText(g, textos[i], Font, Rectangle.Round(Area(i)),
                    i == activo || i == sobre ? Tema.Texto : Tema.TextoSuave,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }

        protected override void Dispose(bool disposing) { if (disposing) anim?.Dispose(); base.Dispose(disposing); }
    }

    /// <summary>Botón grande del Inicio: ícono, nombre y, si es un número, la cifra.</summary>
    public class BotonGrande : Button
    {
        private readonly char icono;
        private readonly string titulo, nota;
        private readonly bool primario;
        private double hover;
        private bool presionado;
        private string cifra;
        private Timer anim;

        public BotonGrande(char icono, string titulo, string nota, bool primario = false)
        {
            this.icono = icono; this.titulo = titulo; this.nota = nota; this.primario = primario;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Text = titulo;             // lo que lee un lector de pantalla
            Dock = DockStyle.Fill; Margin = new Padding(8);
            MouseEnter += (s, e) => Pasar(1);
            MouseLeave += (s, e) => { presionado = false; Pasar(0); };
            MouseDown += (s, e) => { presionado = true; Invalidate(); };
            MouseUp += (s, e) => { presionado = false; Invalidate(); };
        }

        /// <summary>Cifra del botón (si se pone, el número manda y el título va debajo).</summary>
        public string Cifra { get { return cifra; } set { cifra = value; Invalidate(); } }

        private void Pasar(double destino)
        {
            anim?.Stop();
            double desde = hover;
            anim = Animacion.Ejecutar(150, Animacion.Salida, p => { if (!IsDisposed) { hover = Animacion.Entre(desde, destino, p); Invalidate(); } });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Tema.Fondo);
            Color fondo = primario ? Animacion.Mezclar(Tema.Accion, Tema.AccionHover, hover)
                                   : Animacion.Mezclar(Tema.Tarjeta, Tema.Lateral, hover);
            if (presionado) fondo = Animacion.Mezclar(fondo, Color.Black, 0.06);
            Color fuerte = primario ? Tema.Lateral : Tema.Texto, suave = primario ? Tema.Ceniza : Tema.TextoSuave;
            float d = presionado ? 1.5f : 0.5f;
            using (var p = Forma.Continua(new RectangleF(d, d, Width - 1 - 2 * d, Height - 1 - 2 * d), Tema.RadioTarjeta))
            using (var b = new SolidBrush(fondo))
            using (var l = new Pen(primario ? fondo : Focused ? Tema.Anillo : Tema.Linea, Focused && !primario ? 1.6f : 1f))
            { g.FillPath(b, p); g.DrawPath(l, p); }

            const TextFormatFlags centro = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
            var fIcono = Tema.FuenteIconos(26F);
            bool conIcono = cifra == null && fIcono != null;
            if (cifra != null)
            {
                // Cifra: etiqueta chica arriba, número grande, nota abajo
                int y0 = Math.Max(6, (Height - 108) / 2);
                TextRenderer.DrawText(g, titulo.ToUpper(), Tema.FuentePequena, new Rectangle(10, y0, Width - 20, 24), suave, centro);
                TextRenderer.DrawText(g, cifra, Tema.FuenteCifra, new Rectangle(8, y0 + 24, Width - 16, 58), fuerte, centro);
                if (!string.IsNullOrEmpty(nota)) TextRenderer.DrawText(g, nota, Tema.Fuente, new Rectangle(10, y0 + 82, Width - 20, 26), suave, centro);
                return;
            }
            int hArriba = conIcono ? 54 : 0, hNota = string.IsNullOrEmpty(nota) ? 0 : 24;
            int y = Math.Max(6, (Height - (hArriba + 34 + hNota)) / 2);
            if (conIcono)
                TextRenderer.DrawText(g, icono.ToString(), fIcono, new Rectangle(8, y, Width - 16, hArriba), fuerte, centro);
            TextRenderer.DrawText(g, titulo, Tema.FuenteSubtitulo, new Rectangle(10, y + hArriba, Width - 20, 34), fuerte, centro);
            if (hNota > 0)
                TextRenderer.DrawText(g, nota, Tema.Fuente, new Rectangle(10, y + hArriba + 34, Width - 20, hNota), suave, centro);
        }

        protected override void Dispose(bool disposing) { if (disposing) anim?.Dispose(); base.Dispose(disposing); }
    }

    /// <summary>
    /// Ventana de una sola tarea: título, contenido y botones grandes abajo
    /// (la acción principal a la derecha, la de peligro a la izquierda).
    /// Esc cierra; Enter acepta. Se reutiliza: cerrarla solo la oculta.
    /// </summary>
    public class Ficha : Form
    {
        private readonly Label titulo;
        private readonly Panel cuerpo;

        public Ficha(int ancho, int alto, Control contenido, Button principal, Button cerrar, Button peligro = null)
        {
            AutoScaleMode = AutoScaleMode.None;
            Font = Tema.Fuente;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            // En pantallas chicas se recorta para que los botones siempre queden a la vista
            var area = Screen.PrimaryScreen.WorkingArea;
            ClientSize = new Size(Math.Min(ancho, area.Width - 40), Math.Min(alto, area.Height - 60));

            titulo = new Label { Dock = DockStyle.Top, Height = 64, Font = Tema.FuenteTitulo, ForeColor = Tema.Texto, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 8, 24, 0), AutoEllipsis = true };
            cuerpo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 4, 24, 8), AutoScroll = true };
            contenido.Dock = DockStyle.Fill;
            cuerpo.Controls.Add(contenido);

            var pie = new Panel { Dock = DockStyle.Bottom, Height = 88 };
            pie.Paint += (s, e) => { using (var l = new Pen(Tema.LineaSuave)) e.Graphics.DrawLine(l, 0, 0, pie.Width, 0); };
            var botones = new[] { principal, cerrar, peligro }.Where(b => b != null).ToArray();
            foreach (var b in botones) { b.Anchor = AnchorStyles.Top | AnchorStyles.Left; pie.Controls.Add(b); }
            pie.Layout += (s, e) =>
            {
                int x = pie.ClientSize.Width - 24;
                foreach (var b in botones)
                {
                    int w = Math.Max(b == principal ? 190 : 140, TextRenderer.MeasureText(b.Text, Tema.FuenteBoton).Width + 48);
                    if (b != peligro) x -= w;
                    b.SetBounds(b == peligro ? 24 : x, 18, w, Tema.AltoBoton + 4);
                    if (b != peligro) x -= 12;
                }
            };
            if (principal != null) { principal.TabIndex = 0; }
            Controls.Add(cuerpo); Controls.Add(pie); Controls.Add(titulo);
            AcceptButton = principal;
            CancelButton = cerrar;
            Blanco(this);
            Tema.Aplicar(this);
            Blanco(this);
            titulo.Font = Tema.FuenteTitulo;
            if (principal != null) Tema.Boton(principal, true);
            if (peligro != null) Tema.Peligro(peligro);
            Forma.VentanaModerna(this, Tema.Carbon, Tema.Texto);
        }

        /// <summary>La ventana es papel blanco: sus paneles también (los campos quedan en gris).</summary>
        internal static void Blanco(Control raiz)
        {
            if (raiz is Form || raiz.GetType() == typeof(Panel) || raiz is PanelCampos) raiz.BackColor = Tema.Tarjeta;
            foreach (Control c in raiz.Controls) Blanco(c);
        }

        public string Titulo { set { titulo.Text = value; Text = value; } }

        /// <summary>La muestra sobre la ventana principal. true si terminó con la acción principal.</summary>
        public bool Abrir(Control quien, string tituloNuevo)
        {
            Titulo = tituloNuevo;
            DialogResult = DialogResult.None;
            return ShowDialog(quien.TopLevelControl as Form) == DialogResult.OK;
        }

        /// <summary>Cierra la ficha: la tarea quedó hecha.</summary>
        public void Listo() { DialogResult = DialogResult.OK; }
    }

    /// <summary>Piezas comunes de las pantallas: barra con buscador grande, filas que se abren, etc.</summary>
    public static class Sencillo
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, string l);

        /// <summary>Texto gris de ayuda dentro de una caja vacía ("Buscar cliente…").</summary>
        public static void Pista(TextBox t, string texto)
        {
            void Poner() { try { SendMessage(t.Handle, 0x1501, (IntPtr)1, texto); } catch (EntryPointNotFoundException) { } catch (DllNotFoundException) { } }
            t.HandleCreated += (s, e) => Poner();
            if (t.IsHandleCreated) Poner();
        }

        /// <summary>Busca sola 300 ms después de dejar de escribir; no hace falta el botón Buscar.</summary>
        public static void BuscarAlEscribir(TextBox t, Action buscar)
        {
            var reloj = new Timer { Interval = 300 };
            reloj.Tick += (s, e) => { reloj.Stop(); if (!t.IsDisposed) buscar(); };
            t.TextChanged += (s, e) => { reloj.Stop(); reloj.Start(); };
            t.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; reloj.Stop(); buscar(); }
                else if (e.KeyCode == Keys.Escape && t.Text != "") { e.SuppressKeyPress = true; t.Clear(); }
            };
            t.Disposed += (s, e) => reloj.Dispose();
        }

        /// <summary>
        /// Deja el panel de arriba con un solo renglón: a la izquierda lo que se
        /// busca o filtra, a la derecha los botones. Lo que no se pasa, se oculta.
        /// </summary>
        public static Panel Barra(Panel panel, Control[] izquierda, params Button[] derecha)
        {
            panel.SuspendLayout();
            panel.Controls.Clear();
            panel.Dock = DockStyle.Top;
            panel.Height = 72;
            panel.Padding = new Padding(0);
            panel.BackColor = Tema.Fondo;
            panel.Name = "barra";   // que el tema no la pinte como tarjeta
            int x = 0;
            izquierda = izquierda.Select(c => Caja.Admite(c) ? (Control)new Caja(c) { Width = c is ComboBox ? 230 : 380 } : c).ToArray();
            foreach (var c in izquierda)
            {
                c.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                c.Margin = Padding.Empty;
                panel.Controls.Add(c);
            }
            foreach (var b in derecha)
            {
                b.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                panel.Controls.Add(b);
            }
            void Acomodar()
            {
                int d = panel.ClientSize.Width;
                foreach (var b in derecha)
                {
                    int w = Math.Max(130, TextRenderer.MeasureText(b.Text, Tema.FuenteBoton).Width + 44);
                    d -= w;
                    b.SetBounds(d, 4, w, Tema.AltoBoton + 4);
                    d -= 10;
                }
                // El buscador se lleva lo que sobre (entre 170 y 380 px); lo demás no se encoge
                int fijos = izquierda.Where(c => !(c is Caja && c.Controls[0] is TextBox)).Sum(c => c.Width + 14);
                x = 0;
                foreach (var c in izquierda)
                {
                    if (c is Caja && c.Controls[0] is TextBox) c.Width = Math.Max(170, Math.Min(380, d - fijos - 14));
                    else if (c is DateTimePicker) c.Width = 150;
                    c.Location = new Point(x, Math.Max(0, (60 - c.Height) / 2));
                    x += c.Width + 14;
                }
            }
            panel.Layout += (s, e) => Acomodar();
            panel.ResumeLayout();
            return panel;
        }

        /// <summary>Doble clic o Enter abren el renglón; el botón se activa solo si hay uno elegido.</summary>
        public static void Abrir(DataGridView g, Action abrir, Button boton = null)
        {
            g.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) abrir(); };
            g.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && g.CurrentRow != null) { e.SuppressKeyPress = true; abrir(); } };
            if (boton != null)
            {
                boton.Click += (s, e) => { if (g.CurrentRow != null) abrir(); };
                g.SelectionChanged += (s, e) => boton.Enabled = g.CurrentRow != null;
            }
        }

        /// <summary>Quita los controles del formulario para volver a armarlo, sin escalado automático.</summary>
        public static void Vaciar(Form f)
        {
            f.SuspendLayout();
            f.AutoScaleMode = AutoScaleMode.None;
            f.Controls.Clear();
            f.Padding = Padding.Empty;
            f.Font = Tema.Fuente;
            f.ResumeLayout(false);
        }

        /// <summary>Arma el formulario: las barras arriba y una tarjeta ocupando todo lo demás.</summary>
        public static void Armar(Form f, Control centro, params Control[] barras)
        {
            f.Controls.Add(EnTarjeta(centro));
            foreach (var b in barras.Reverse()) f.Controls.Add(b);
        }

        public static Tarjeta EnTarjeta(Control c, string titulo = null)
        {
            var t = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(12, 10, 12, 12) };
            c.Dock = DockStyle.Fill;
            t.Controls.Add(c);
            if (c is PanelCampos || c.GetType() == typeof(Panel)) c.BackColor = Tema.Tarjeta;
            if (c is PanelCampos) { t.Padding = new Padding(22, 14, 14, 14); }
            if (titulo != null)
                t.Controls.Add(new Label { Text = titulo, Dock = DockStyle.Top, Height = 44, Font = Tema.FuenteSubtitulo, ForeColor = Tema.Texto, TextAlign = ContentAlignment.MiddleLeft });
            return t;
        }

        /// <summary>Dos zonas lado a lado: a la izquierda los datos, a la derecha lo relacionado.</summary>
        public static Panel DosZonas(Control izquierda, int anchoIzquierda, Control derecha)
        {
            var p = new Panel();
            derecha.Dock = DockStyle.Fill;
            izquierda.Dock = DockStyle.Left;
            izquierda.Width = anchoIzquierda;
            p.Controls.Add(derecha);
            p.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 18 });
            p.Controls.Add(izquierda);
            return p;
        }

        /// <summary>Pide un texto corto en una ventana grande. null si se cancela o queda vacío.</summary>
        public static string Preguntar(Control quien, string titulo, string etiqueta, string boton)
        {
            var t = new TextBox { MaxLength = 200 };
            var ok = Boton(boton, null, "btnGuardar");
            var no = new Button { Text = "Regresar", DialogResult = DialogResult.Cancel };
            using (var f = new Ficha(560, 290, new PanelCampos(1, new Campo(etiqueta, t)), ok, no))
            {
                ok.Click += (s, e) => { if (t.Text.Trim() == "") { t.BackColor = Tema.ErrorFondo; t.Focus(); } else f.Listo(); };
                f.Shown += (s, e) => t.Focus();
                return f.Abrir(quien, titulo) ? t.Text.Trim() : null;
            }
        }

        public static Button Boton(string texto, Action click, string nombre = null)
        {
            var b = new Button { Text = texto, Name = nombre ?? "btn", Height = Tema.AltoBoton };
            if (click != null) b.Click += (s, e) => click();
            return b;
        }
    }
}
