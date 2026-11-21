using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Barra lateral dibujada a mano, al estilo de las sidebars de macOS:
    /// secciones con etiqueta pequeña, íconos de línea (fuente de íconos de
    /// Windows), resaltado suave al pasar el cursor y una "pastilla" verde
    /// azulado que se desliza con resorte hasta el módulo activo. Se desplaza
    /// con la rueda y muestra un indicador delgado solo mientras se usa.
    /// </summary>
    public class MenuLateral : Control
    {
        private class Item { public string Clave, Texto, Grupo; public char Icono; public RectangleF Area; }

        private readonly List<Item> items = new List<Item>();
        private readonly Font fuenteIconos;
        private string activo, sobre;
        private float pastillaY, pastillaDestino, desplazamiento, alto;
        private Timer anim, ocultarBarra;
        private bool mostrarBarra;
        private const int AltoItem = 34, AltoGrupo = 30, Margen = 12;

        public event Action<string> Seleccionado;

        public MenuLateral()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Tema.Lateral;
            Font = Tema.Fuente;
            Cursor = Cursors.Hand;
            // Segoe Fluent Icons (Windows 11) o Segoe MDL2 Assets (Windows 10)
            string iconos = FontFamily.Families.Any(f => f.Name == "Segoe Fluent Icons") ? "Segoe Fluent Icons"
                          : FontFamily.Families.Any(f => f.Name == "Segoe MDL2 Assets") ? "Segoe MDL2 Assets" : null;
            if (iconos != null) fuenteIconos = new Font(iconos, 11F);
            ocultarBarra = new Timer { Interval = 1200 };
            ocultarBarra.Tick += (s, e) => { mostrarBarra = false; ocultarBarra.Stop(); Invalidate(); };
        }

        public void Agregar(string clave, string texto, string grupo, char icono)
        {
            items.Add(new Item { Clave = clave, Texto = texto, Grupo = grupo, Icono = icono });
            Acomodar();
        }

        public void Limpiar() { items.Clear(); activo = null; Acomodar(); }

        private void Acomodar()
        {
            float y = 6; string grupo = null;
            foreach (var it in items)
            {
                if (it.Grupo != grupo) { grupo = it.Grupo; y += AltoGrupo; }
                it.Area = new RectangleF(Margen, y, Width - Margen * 2, AltoItem);
                y += AltoItem + 2;
            }
            alto = y + 12;
            Invalidate();
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); Acomodar(); Limitar(); }

        /// <summary>Marca el módulo y desliza la pastilla con un resorte ligero (bounce ≈ 0.15).</summary>
        public void Activar(string clave)
        {
            var it = items.FirstOrDefault(x => x.Clave == clave);
            if (it == null) return;
            bool primera = activo == null;
            activo = clave;
            pastillaDestino = it.Area.Y;
            // Que el elemento activo quede a la vista
            if (it.Area.Y - desplazamiento < 0) desplazamiento = it.Area.Y - 8;
            if (it.Area.Bottom - desplazamiento > Height) desplazamiento = it.Area.Bottom - Height + 8;
            Limitar();
            anim?.Stop();
            if (primera) { pastillaY = pastillaDestino; Invalidate(); return; }
            float desde = pastillaY;
            anim = Animacion.Ejecutar(420, Animacion.Resorte, p => { pastillaY = (float)Animacion.Entre(desde, pastillaDestino, p); Invalidate(); });
        }

        private void Limitar() => desplazamiento = Math.Max(0, Math.Min(desplazamiento, Math.Max(0, alto - Height)));

        private Item En(Point p) => items.FirstOrDefault(i => i.Area.Contains(p.X, p.Y + desplazamiento));

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var it = En(e.Location);
            string nuevo = it?.Clave;
            if (nuevo != sobre) { sobre = nuevo; Cursor = it != null ? Cursors.Hand : Cursors.Default; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { sobre = null; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var it = En(e.Location);
            if (it != null && e.Button == MouseButtons.Left) Seleccionado?.Invoke(it.Clave);
            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            desplazamiento -= e.Delta / 3f;
            Limitar();
            if (alto > Height) { mostrarBarra = true; ocultarBarra.Stop(); ocultarBarra.Start(); }
            Invalidate();
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(BackColor);
            g.TranslateTransform(0, -desplazamiento);

            // Pastilla del módulo activo (debajo del texto)
            if (activo != null)
            {
                var r = new RectangleF(Margen, pastillaY, Width - Margen * 2, AltoItem);
                using (var path = Forma.Continua(r, Tema.RadioNav))
                using (var b = new SolidBrush(Tema.Navegacion))
                    g.FillPath(b, path);
            }

            string grupo = null;
            using (var fuenteGrupo = new Font(Tema.Fuente.FontFamily, 8.25F))
            {
                foreach (var it in items)
                {
                    if (it.Grupo != grupo)
                    {
                        grupo = it.Grupo;
                        TextRenderer.DrawText(g, grupo, fuenteGrupo, new Point(Margen + 10, (int)it.Area.Y - 20), Tema.Ceniza);
                    }
                    bool esActivo = it.Clave == activo, esSobre = it.Clave == sobre;
                    if (esSobre && !esActivo)
                        using (var path = Forma.Continua(it.Area, Tema.RadioNav))
                        using (var b = new SolidBrush(Color.FromArgb(14, Tema.Texto)))
                            g.FillPath(b, path);

                    // El texto sigue a la pastilla cuando esta llega (no salta antes)
                    bool cubierto = esActivo && Math.Abs(pastillaY - it.Area.Y) < AltoItem / 2f;
                    Color color = cubierto ? Color.White : esSobre ? Tema.Texto : Tema.TextoSuave;
                    int x = (int)it.Area.X + 10;
                    if (fuenteIconos != null)
                    {
                        TextRenderer.DrawText(g, it.Icono.ToString(), fuenteIconos,
                            new Rectangle(x, (int)it.Area.Y, 20, AltoItem), color, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
                        x += 28;
                    }
                    TextRenderer.DrawText(g, it.Texto, cubierto ? Tema.FuenteNegrita : Tema.Fuente,
                        new Rectangle(x, (int)it.Area.Y, (int)it.Area.Right - x, AltoItem), color,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                }
            }

            g.ResetTransform();
            // Indicador de desplazamiento delgado, solo mientras se usa la rueda
            if (mostrarBarra && alto > Height)
            {
                float largo = Math.Max(30, Height * Height / alto);
                float y = (Height - largo) * desplazamiento / (alto - Height);
                using (var path = Forma.Continua(new RectangleF(Width - 7, y + 2, 4, largo - 4), 2, 0))
                using (var b = new SolidBrush(Color.FromArgb(90, Tema.Texto)))
                    g.FillPath(b, path);
            }
            // Línea de lápiz que separa la barra lateral del contenido
            using (var linea = new Pen(Tema.LineaSuave)) g.DrawLine(linea, Width - 1, 0, Width - 1, Height);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { anim?.Dispose(); ocultarBarra.Dispose(); fuenteIconos?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
