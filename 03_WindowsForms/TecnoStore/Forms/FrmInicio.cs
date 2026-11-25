using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Inicio: lo que cada rol hace todos los días, en botones grandes, y debajo
    /// los números que le importan (cada número es una puerta a su pantalla).
    /// Nada más: el detalle vive en cada módulo y en Reportes.
    /// </summary>
    public class FrmInicio : Form, IRefrescable
    {
        private static readonly CultureInfo Mx = new CultureInfo("es-MX");
        private readonly Func<string, Form> ir;
        private readonly Label lblSaludo = new Label { Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
        private BotonGrande ingresos, ventas, taller, listos, resurtir;

        public FrmInicio(Func<string, Form> ir)
        {
            this.ir = ir;
            var s = Db.Sesion;
            AutoScaleMode = AutoScaleMode.None;
            Text = "Inicio";

            var acciones = Fila();
            if (s.Puede(Capacidad.RegistrarVenta)) Agregar(acciones, '', "Nueva venta", "cobrar a un cliente", () => ir("venta"), true);
            if (s.Puede(Capacidad.GestionarOrdenes)) Agregar(acciones, '', "Recibir equipo", "entra al taller", () => (ir("ordenes") as FrmOrdenes)?.Recibir());
            if (s.Puede(Capacidad.RegistrarCompra)) Agregar(acciones, '', "Llegó mercancía", "compra a proveedor", () => ir("compras"));
            if (!s.Puede(Capacidad.RegistrarVenta) && s.Puede(Capacidad.VerReportes)) Agregar(acciones, '', "Ver reportes", "ventas e inventario", () => ir("reportes"), true);
            Agregar(acciones, '', "Buscar producto", "precio y existencias", () => ir("productos"));
            if (!s.Puede(Capacidad.RegistrarCompra)) Agregar(acciones, '', "Clientes", "datos y compras", () => ir("clientes"));

            var numeros = Fila();
            if (s.Puede(Capacidad.VerReportes))
            {
                ingresos = Agregar(numeros, ' ', "Ingresos", "últimos 90 días", () => ir("dashboard"));
                ventas = Agregar(numeros, ' ', "Ventas", "últimos 90 días", () => ir("historial"));
            }
            taller = Agregar(numeros, ' ', "En taller", "equipos por atender", () => ir("ordenes"));
            if (!s.Puede(Capacidad.VerReportes)) listos = Agregar(numeros, ' ', "Listos", "para entregar", () => ir("ordenes"));
            resurtir = Agregar(numeros, ' ', "Por resurtir", "productos", () => (ir("productos") as FrmProductos)?.VerPorResurtir());

            var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 54F));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 46F));
            t.Controls.Add(lblSaludo, 0, 0);
            t.Controls.Add(acciones, 0, 1);
            t.Controls.Add(numeros, 0, 2);
            Controls.Add(t);
            Load += (o, e) => Refrescar();
        }

        private static TableLayoutPanel Fila() => new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 0, Margin = Padding.Empty };

        private static BotonGrande Agregar(TableLayoutPanel fila, char icono, string titulo, string nota, Action accion, bool primario = false)
        {
            var b = new BotonGrande(icono, titulo, nota, primario);
            b.Click += (s, e) => accion();
            fila.ColumnCount++;
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fila.Controls.Add(b, fila.ColumnCount - 1, 0);
            return b;
        }

        public void Refrescar()
        {
            int h = DateTime.Now.Hour;
            lblSaludo.Font = Tema.FuenteTitulo;
            lblSaludo.Text = (h < 12 ? "Buenos días" : h < 19 ? "Buenas tardes" : "Buenas noches") + ", " + Db.Sesion.Nombre.Split(' ')[0];
            try
            {
                DataRow k = DashboardRepository.Inicio();
                taller.Cifra = k["ordenes_abiertas"].ToString();
                resurtir.Cifra = k["alertas_stock"].ToString();
                if (listos != null) listos.Cifra = k["ordenes_listas"].ToString();
                if (ingresos != null)
                {
                    ingresos.Cifra = ((decimal)k["ingresos"]).ToString("C0", Mx);
                    ventas.Cifra = k["ventas"].ToString();
                }
            }
            catch (Exception ex) { Ui.Error(ex); }
        }
    }
}
