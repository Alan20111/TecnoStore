using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Barra de desplazamiento vertical delgada y superpuesta para DataGridView,
    /// como las "overlay scrollbars" de macOS: una cápsula de 6 px que no ocupa
    /// ancho, aparece al desplazarse o al pasar el cursor, crece a 9 px bajo el
    /// cursor y se oculta tras 1.2 s sin actividad. La rueda del mouse desplaza
    /// 3 filas; Shift + rueda desplaza en horizontal.
    /// Si la tabla no permite desplazarse por código, regresa a la barra nativa.
    /// </summary>
    public class BarraDelgada : Control
    {
        private const int Ancho = 14, Delgada = 6, Gruesa = 9, Minimo = 28;
        private readonly DataGridView grid;
        private readonly Timer ocultar = new Timer { Interval = 1200 };
        private bool sobre, arrastrando;
        private int yArrastre, filaArrastre;
        private Rectangle pulgar;

        public static void Instalar(DataGridView g)
        {
            if (g.Controls.OfType<BarraDelgada>().Any()) return;
            g.Controls.Add(new BarraDelgada(g));
        }

        private BarraDelgada(DataGridView g)
        {
            grid = g;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            Visible = false;
            Cursor = Cursors.Default;
            grid.ScrollBars = ScrollBars.Horizontal;   // la vertical la dibuja esta barra

            grid.MouseWheel += Rueda;
            grid.Scroll += (s, e) => Actualizar(true);
            grid.Resize += (s, e) => Actualizar(false);
            grid.RowsAdded += (s, e) => Actualizar(false);
            grid.RowsRemoved += (s, e) => Actualizar(false);
            grid.DataBindingComplete += (s, e) => Actualizar(false);
            grid.SelectionChanged += (s, e) => Actualizar(true);
            grid.MouseEnter += (s, e) => Actualizar(true);
            ocultar.Tick += (s, e) => { if (!sobre && !arrastrando) { Visible = false; ocultar.Stop(); } };
        }

        private int Total => grid.Rows.GetRowCount(DataGridViewElementStates.Visible);
        private int Visibles => Math.Max(1, grid.DisplayedRowCount(false));
        private int Primera => Math.Max(0, grid.FirstDisplayedScrollingRowIndex);

        private void Rueda(object sender, MouseEventArgs e)
        {
            if (e is HandledMouseEventArgs h) h.Handled = true;   // evita el desplazamiento nativo
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                try { grid.HorizontalScrollingOffset = Math.Max(0, grid.HorizontalScrollingOffset - e.Delta / 2); } catch (InvalidOperationException) { }
                return;
            }
            IrA(Primera - Math.Sign(e.Delta) * 3);
        }

        private void IrA(int fila)
        {
            int max = Math.Max(0, Total - Visibles);
            fila = Math.Max(0, Math.Min(max, fila));
            if (Total == 0) return;
            try { grid.FirstDisplayedScrollingRowIndex = fila; }
            catch (InvalidOperationException)
            {
                // Esta tabla no se deja desplazar sin barra: se vuelve a la nativa
                grid.MouseWheel -= Rueda;
                grid.ScrollBars = ScrollBars.Both;
                Dispose();
                return;
            }
            Actualizar(true);
        }

        private void Actualizar(bool mostrar)
        {
            if (IsDisposed) return;
            int total = Total, visibles = Visibles;
            int arriba = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight + 2 : 2;
            var hBar = grid.Controls.OfType<HScrollBar>().FirstOrDefault();
            int abajo = (hBar != null && hBar.Visible ? hBar.Height : 0) + 2;
            Bounds = new Rectangle(grid.ClientSize.Width - Ancho, arriba, Ancho, Math.Max(0, grid.ClientSize.Height - arriba - abajo));

            if (total <= visibles || Height < Minimo) { Visible = false; return; }
            int largo = Math.Max(Minimo, Height * visibles / total);
            int y = (Height - largo) * Primera / Math.Max(1, total - visibles);
            pulgar = new Rectangle(0, y, Width, largo);

            // Solo la cápsula es parte del control: lo demás deja ver y tocar la tabla
            int ancho = sobre || arrastrando ? Gruesa : Delgada;
            using (var forma = Forma.Continua(new RectangleF(Width - ancho - 3, y, ancho, largo), ancho / 2f, 0))
                Region = new Region(forma);
            if (mostrar) { Visible = true; BringToFront(); ocultar.Stop(); ocultar.Start(); }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int ancho = sobre || arrastrando ? Gruesa : Delgada;
            int alfa = arrastrando ? 150 : sobre ? 120 : 85;
            using (var forma = Forma.Continua(new RectangleF(Width - ancho - 3, pulgar.Y, ancho, pulgar.Height), ancho / 2f, 0))
            using (var b = new SolidBrush(Color.FromArgb(alfa, Tema.Texto)))
            {
                e.Graphics.Clear(grid.DefaultCellStyle.BackColor);
                e.Graphics.FillPath(b, forma);
            }
        }

        protected override void OnMouseEnter(EventArgs e) { sobre = true; Actualizar(true); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { sobre = false; Actualizar(true); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            arrastrando = true; yArrastre = e.Y; filaArrastre = Primera;
            Capture = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (arrastrando)
            {
                int recorrido = Math.Max(1, Height - pulgar.Height);
                int filas = (e.Y - yArrastre) * Math.Max(0, Total - Visibles) / recorrido;
                IrA(filaArrastre + filas);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            arrastrando = false; Capture = false; Actualizar(true);
            base.OnMouseUp(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) ocultar.Dispose();
            base.Dispose(disposing);
        }
    }
}
