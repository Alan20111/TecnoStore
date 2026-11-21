using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>Panel blanco con esquinas redondeadas y borde suave (tarjetas del Dashboard).</summary>
    public class Tarjeta : Panel
    {
        [DefaultValue(12)]
        public int Radio { get; set; } = 12;

        public Tarjeta()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            Padding = new Padding(16, 12, 16, 12);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent?.BackColor ?? Tema.Fondo);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = Redondo(r, Radio))
            using (var fondo = new SolidBrush(BackColor))
            using (var borde = new Pen(Tema.Borde))
            {
                e.Graphics.FillPath(fondo, path);
                e.Graphics.DrawPath(borde, path);
            }
        }

        private static GraphicsPath Redondo(Rectangle r, int radio)
        {
            int d = radio * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
