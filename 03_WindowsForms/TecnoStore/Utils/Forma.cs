using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Formas tipo Apple: rectángulos con esquina continua ("squircle") y
    /// ajustes de ventana de Windows 11 (esquinas redondeadas del sistema y
    /// barra de título oscura).
    /// </summary>
    public static class Forma
    {
        /// <summary>
        /// Rectángulo con esquinas continuas ("squircle"), como CALayerCornerCurve.continuous
        /// de Apple. Usa la geometría de Figma para "corner smoothing" (ξ = 0.6 coincide
        /// con iOS): cada esquina es curva de transición → arco de 90·(1−ξ)° → curva de
        /// transición, así la curvatura crece sin el salto que tiene un arco circular.
        /// </summary>
        public static GraphicsPath Continua(RectangleF r, float radio, float suavizado = 0.6f)
        {
            var path = new GraphicsPath();
            float maxP = Math.Min(r.Width, r.Height) / 2f;
            radio = Math.Max(0, Math.Min(radio, maxP));
            if (radio < 0.5f) { path.AddRectangle(r); return path; }

            // Si no cabe la esquina completa se reduce el suavizado (igual que Figma)
            float xi = Math.Max(0, Math.Min(suavizado, maxP / radio - 1));
            float p = (1 + xi) * radio;
            double arco = 90 * (1 - xi);
            float L = (float)(Math.Sin(Rad(arco / 2)) * radio * Math.Sqrt(2));
            double alfa = (90 - arco) / 2;
            float p3p4 = (float)(radio * Math.Tan(Rad(alfa / 2)));
            double beta = 45 * xi;
            float c = (float)(p3p4 * Math.Cos(Rad(beta)));
            float d = (float)(c * Math.Tan(Rad(beta)));
            float b = (p - L - c - d) / 3f;
            float a = 2 * b;
            float k = (float)(4.0 / 3.0 * Math.Tan(Rad(arco / 4)) * radio);   // Bézier que aproxima el arco

            // Esquinas en sentido horario: e1 = dirección de avance, e2 = hacia el siguiente lado
            var esquinas = new[]
            {
                new { V = new PointF(r.Right, r.Top),    E1 = new PointF(1, 0),  E2 = new PointF(0, 1) },
                new { V = new PointF(r.Right, r.Bottom), E1 = new PointF(0, 1),  E2 = new PointF(-1, 0) },
                new { V = new PointF(r.Left, r.Bottom),  E1 = new PointF(-1, 0), E2 = new PointF(0, -1) },
                new { V = new PointF(r.Left, r.Top),     E1 = new PointF(0, -1), E2 = new PointF(1, 0) },
            };
            path.StartFigure();
            PointF? anterior = null;
            foreach (var q in esquinas)
            {
                Func<float, float, PointF> P = (u, v) => new PointF(q.V.X + u * q.E1.X + v * q.E2.X, q.V.Y + u * q.E1.Y + v * q.E2.Y);
                PointF p0 = P(-p, 0);
                if (anterior.HasValue) path.AddLine(anterior.Value, p0);
                PointF c1 = P(-p + a, 0), c2 = P(-p + a + b, 0), p1 = P(-p + a + b + c, d);
                PointF p2 = P(-p + a + b + c + L, d + L);
                PointF c3 = P(-p + a + b + c + L + d, d + L + c), c4 = P(-p + a + b + c + L + d, d + L + b + c), p3 = P(0, p);
                path.AddBezier(p0, c1, c2, p1);
                // Arco central: tangentes = dirección de llegada a p1 y de salida de p2
                PointF t1 = Unit(new PointF(p1.X - c2.X, p1.Y - c2.Y)), t2 = Unit(new PointF(c3.X - p2.X, c3.Y - p2.Y));
                path.AddBezier(p1, new PointF(p1.X + k * t1.X, p1.Y + k * t1.Y), new PointF(p2.X - k * t2.X, p2.Y - k * t2.Y), p2);
                path.AddBezier(p2, c3, c4, p3);
                anterior = p3;
            }
            path.AddLine(anterior.Value, P0(r, p));
            path.CloseFigure();
            return path;
        }

        private static PointF P0(RectangleF r, float p) => new PointF(r.Right - p, r.Top);
        private static double Rad(double grados) => grados * Math.PI / 180;
        private static PointF Unit(PointF v)
        {
            float n = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return n < 1e-4f ? new PointF(0, 0) : new PointF(v.X / n, v.Y / n);
        }

        public static GraphicsPath Continua(Rectangle r, float radio) =>
            Continua(new RectangleF(r.X, r.Y, r.Width, r.Height), radio);

        // ---------------- Ventana (Windows 11) ----------------
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int atributo, ref int valor, int tamano);

        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;   // 2 = redondeadas
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        /// <summary>
        /// En Windows 11 redondea la ventana y pinta la barra de título del
        /// color de la marca. En Windows 10 la llamada simplemente no hace nada.
        /// </summary>
        public static void VentanaModerna(Form f, Color titulo, Color textoTitulo)
        {
            f.HandleCreated += (s, e) =>
            {
                try
                {
                    int esquinas = 2;
                    DwmSetWindowAttribute(f.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref esquinas, sizeof(int));
                    int fondo = ColorTranslator.ToWin32(titulo);
                    DwmSetWindowAttribute(f.Handle, DWMWA_CAPTION_COLOR, ref fondo, sizeof(int));
                    int texto = ColorTranslator.ToWin32(textoTitulo);
                    DwmSetWindowAttribute(f.Handle, DWMWA_TEXT_COLOR, ref texto, sizeof(int));
                    int borde = ColorTranslator.ToWin32(titulo);
                    DwmSetWindowAttribute(f.Handle, DWMWA_BORDER_COLOR, ref borde, sizeof(int));
                }
                catch (DllNotFoundException) { /* sin DWM: se queda la ventana clásica */ }
                catch (EntryPointNotFoundException) { }
            };
        }
    }
}
