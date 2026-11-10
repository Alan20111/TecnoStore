using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using TecnoStore.Forms;

namespace TecnoStore
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Moneda y fechas en formato mexicano sin importar la configuración de la PC
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK) return;
                }
                using (var principal = new FrmPrincipal())
                {
                    Application.Run(principal);
                    if (!principal.CerrarSesion) return;   // "Cerrar sesión" vuelve al login
                }
            }
        }
    }
}
