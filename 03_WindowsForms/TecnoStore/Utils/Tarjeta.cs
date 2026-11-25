using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>Panel blanco con esquinas redondeadas y borde suave (tarjetas del Dashboard).</summary>
    public class Tarjeta : Panel
    {
        [DefaultValue(24)]
        public int Radio { get; set; } = Tema.RadioTarjeta;

        public Tarjeta()
        {
            DoubleBuffered = true;
            BackColor = Tema.Tarjeta;
            Padding = new Padding(16, 12, 16, 12);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Parent?.BackColor ?? Tema.Fondo);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 2.5f);
            // Única sombra del sistema: 1 px suave bajo la tarjeta (0 1px 2px rgba(0,0,0,.08))
            using (var sombra = Forma.Continua(new RectangleF(r.X, r.Y + 1.5f, r.Width, r.Height), Radio))
            using (var bs = new SolidBrush(Color.FromArgb(14, 0, 0, 0)))
                g.FillPath(bs, sombra);
            using (var path = Forma.Continua(r, Radio))
            using (var fondo = new SolidBrush(BackColor))
            using (var borde = new Pen(Tema.LineaSuave))
            {
                g.FillPath(fondo, path);
                g.DrawPath(borde, path);
            }
        }
    }
}
