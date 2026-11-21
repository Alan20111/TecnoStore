using System;
using System.Drawing;
using System.Windows.Forms;

namespace TecnoStore.Utils
{
    /// <summary>
    /// Animaciones con temporizador (~60 fps) y curvas tipo Apple.
    /// Si Windows tiene desactivados los efectos visuales, todo salta al
    /// valor final (equivalente a "Reducir movimiento").
    /// </summary>
    public static class Animacion
    {
        public static bool Activas => SystemInformation.UIEffectsEnabled;

        /// <summary>Salida suave (ease-out cúbica): rápida al inicio, frena al final.</summary>
        public static double Salida(double t) => 1 - Math.Pow(1 - t, 3);

        /// <summary>
        /// Resorte con rebote ligero (bounce ≈ 0.15 en SwiftUI): rebasa el destino
        /// ~1.5 % y se asienta. Apple usa por defecto bounce 0 (Salida);
        /// este se reserva para objetos "físicos" como la pastilla de navegación.
        /// </summary>
        public static double Resorte(double t)
        {
            const double amortiguamiento = 14.0, frecuencia = 10.5;
            return 1 - Math.Exp(-amortiguamiento * t) * Math.Cos(frecuencia * t);
        }

        /// <summary>Ejecuta <paramref name="paso"/> con el progreso ya suavizado (0 → 1) durante <paramref name="ms"/> milisegundos.</summary>
        public static Timer Ejecutar(int ms, Func<double, double> curva, Action<double> paso, Action fin = null)
        {
            if (!Activas || ms <= 0) { paso(1); fin?.Invoke(); return null; }
            var inicio = DateTime.Now;
            var t = new Timer { Interval = 15 };
            t.Tick += (s, e) =>
            {
                double p = Math.Min(1.0, (DateTime.Now - inicio).TotalMilliseconds / ms);
                paso(p >= 1 ? 1 : curva(p));
                if (p >= 1) { t.Stop(); t.Dispose(); fin?.Invoke(); }
            };
            t.Start();
            return t;
        }

        public static int Entre(int a, int b, double p) => (int)Math.Round(a + (b - a) * p);
        public static double Entre(double a, double b, double p) => a + (b - a) * p;

        public static Color Mezclar(Color a, Color b, double p)
        {
            p = Math.Max(0, Math.Min(1, p));
            return Color.FromArgb(Entre(a.A, b.A, p), Entre(a.R, b.R, p), Entre(a.G, b.G, p), Entre(a.B, b.B, p));
        }

        /// <summary>Aparece la ventana con un fundido corto (solo ventanas de nivel superior).</summary>
        public static void Fundido(Form f, int ms = 220)
        {
            if (!Activas) return;
            f.Opacity = 0;
            f.Shown += (s, e) => Ejecutar(ms, Salida, p => { if (!f.IsDisposed) f.Opacity = p; });
        }
    }
}
