using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Dashboard del administrador: indicadores de los últimos 90 días, ingresos por mes,
    /// ventas por categoría, ventas recientes, más vendidos, alertas de stock
    /// y órdenes del taller. Solo lectura; usa vistas y funciones existentes.
    /// Las tarjetas y gráficas se arman en código (Construir) para mantener el
    /// diseño en un solo lugar.
    /// </summary>
    public partial class FrmDashboard : Form
    {
        private static readonly CultureInfo Mx = new CultureInfo("es-MX");
        private static readonly Color[] Paleta =
        {
            Color.FromArgb(0, 113, 227), Color.FromArgb(52, 199, 89), Color.FromArgb(255, 149, 0), Color.FromArgb(175, 82, 222),
            Color.FromArgb(255, 59, 48), Color.FromArgb(90, 200, 250), Color.FromArgb(255, 204, 0), Color.FromArgb(142, 142, 147),
            Color.FromArgb(48, 176, 199), Color.FromArgb(88, 86, 214),
        };

        private Chart chtIngresos, chtCategorias;
        private DataGridView dgvRecientes, dgvMasVendidos, dgvAlertas, dgvOrdenes;
        private readonly Label[] kpiValor = new Label[8];
        private readonly Label[] kpiNota = new Label[8];

        public FrmDashboard()
        {
            InitializeComponent();
            Construir();
        }

        private void FrmDashboard_Load(object sender, EventArgs e) => Cargar();
        private void btnActualizar_Click(object sender, EventArgs e) => Cargar();

        private void Construir()
        {
            string[] titulos =
            {
                "Ingresos · 90 días", "Ventas · 90 días", "Ticket promedio", "Unidades vendidas",
                "Órdenes en taller", "Alertas de stock", "Clientes activos", "Valor del inventario",
            };
            for (int i = 0; i < 8; i++)
            {
                var t = new Tarjeta { Dock = DockStyle.Fill, Margin = new Padding(6) };
                var lt = new Label { Text = titulos[i], AutoSize = true, ForeColor = Tema.TextoSuave, Font = Tema.Fuente, Location = new Point(16, 14) };
                kpiValor[i] = new Label { Text = "—", AutoSize = true, ForeColor = Tema.Texto, Font = Tema.FuenteKpi, Location = new Point(13, 36) };
                kpiNota[i] = new Label { Text = "", AutoSize = true, ForeColor = Tema.TextoSuave, Font = new Font("Segoe UI", 8.5F), Location = new Point(16, 78) };
                t.Controls.AddRange(new Control[] { lt, kpiValor[i], kpiNota[i] });
                tlpDashboard.Controls.Add(t, i % 4, 1 + i / 4);
            }

            chtIngresos = NuevaGrafica();
            var area = chtIngresos.ChartAreas[0];
            area.AxisY.LabelStyle.Format = "$#,##0,K";
            var serie = chtIngresos.Series.Add("Ingresos");
            serie.ChartType = SeriesChartType.Column;
            serie.Color = Tema.Acento;
            serie["PointWidth"] = "0.55";
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "$#,##0,K";
            serie.Font = new Font("Segoe UI", 8F);
            serie.LabelForeColor = Tema.TextoSuave;
            tlpDashboard.Controls.Add(Contenedor("Ingresos por mes · últimos 6 meses", chtIngresos), 0, 3);
            tlpDashboard.SetColumnSpan(tlpDashboard.GetControlFromPosition(0, 3), 2);

            chtCategorias = NuevaGrafica();
            var dona = chtCategorias.Series.Add("Categorías");
            dona.ChartType = SeriesChartType.Doughnut;
            dona["DoughnutRadius"] = "45";
            dona["PieLabelStyle"] = "Disabled";
            chtCategorias.Legends.Add(new Legend { Docking = Docking.Right, Font = new Font("Segoe UI", 8.5F), ForeColor = Tema.Texto, BackColor = Color.Transparent });
            tlpDashboard.Controls.Add(Contenedor("Ventas por categoría · últimos 90 días", chtCategorias), 2, 3);
            tlpDashboard.SetColumnSpan(tlpDashboard.GetControlFromPosition(2, 3), 2);

            dgvRecientes = NuevaTabla(); dgvMasVendidos = NuevaTabla(); dgvAlertas = NuevaTabla(); dgvOrdenes = NuevaTabla();
            AgregarAncho(Contenedor("Ventas recientes", dgvRecientes), 0, 4);
            AgregarAncho(Contenedor("Más vendidos · últimos 90 días", dgvMasVendidos), 2, 4);
            AgregarAncho(Contenedor("Productos por resurtir", dgvAlertas), 0, 5);
            AgregarAncho(Contenedor("Equipos en el taller", dgvOrdenes), 2, 5);
            dgvAlertas.RowPrePaint += (s, e) =>
            {
                var row = dgvAlertas.Rows[e.RowIndex];
                if (Convert.ToInt32(row.Cells["stock"].Value) == 0) row.DefaultCellStyle.ForeColor = Tema.Rojo;
            };
        }

        private void AgregarAncho(Control c, int col, int fila)
        {
            tlpDashboard.Controls.Add(c, col, fila);
            tlpDashboard.SetColumnSpan(c, 2);
        }

        private static Tarjeta Contenedor(string titulo, Control contenido)
        {
            var t = new Tarjeta { Dock = DockStyle.Fill, Margin = new Padding(6), Padding = new Padding(14, 40, 14, 12) };
            t.Controls.Add(new Label { Text = titulo, AutoSize = true, Font = Tema.FuenteNegrita, ForeColor = Tema.Texto, Location = new Point(16, 13) });
            contenido.Dock = DockStyle.Fill;
            t.Controls.Add(contenido);
            return t;
        }

        private static Chart NuevaGrafica()
        {
            var c = new Chart { BackColor = Color.White, Palette = ChartColorPalette.None, PaletteCustomColors = Paleta };
            var a = new ChartArea("principal") { BackColor = Color.White };
            a.AxisX.MajorGrid.Enabled = false;
            a.AxisX.LineColor = Tema.Borde;
            a.AxisX.MajorTickMark.Enabled = false;
            a.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.5F);
            a.AxisX.LabelStyle.ForeColor = Tema.TextoSuave;
            a.AxisX.Interval = 1;
            a.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 243);
            a.AxisY.LineColor = Color.White;
            a.AxisY.MajorTickMark.Enabled = false;
            a.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            a.AxisY.LabelStyle.ForeColor = Tema.TextoSuave;
            c.ChartAreas.Add(a);
            return c;
        }

        private static DataGridView NuevaTabla()
        {
            var g = new DataGridView();
            Ui.Grid(g);
            Tema.Tabla(g);
            g.ScrollBars = ScrollBars.Vertical;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            return g;
        }

        private void Cargar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var s = Db.Sesion;
                lblSaludo.Text = Saludo() + ", " + s.Nombre.Split(' ')[0];
                lblFecha.Text = Mx.TextInfo.ToTitleCase(DateTime.Today.ToString("dddd d 'de' MMMM 'de' yyyy", Mx)) + " · " + s.Sucursal;

                var k = DashboardRepository.Indicadores();
                decimal ingresos = (decimal)k["ingresos_mes"], ingresosAnt = (decimal)k["ingresos_ant"];
                int ventas = (int)k["ventas_mes"], ventasAnt = (int)k["ventas_ant"];
                Kpi(0, ingresos.ToString("C0", Mx), Variacion(ingresos, ingresosAnt));
                Kpi(1, ventas.ToString("N0", Mx), Variacion(ventas, ventasAnt));
                Kpi(2, (ventas == 0 ? 0 : ingresos / ventas).ToString("C0", Mx), "por venta completada", Tema.TextoSuave);
                Kpi(3, Convert.ToInt32(k["unidades_mes"]).ToString("N0", Mx), "piezas en 90 días", Tema.TextoSuave);
                Kpi(4, k["ordenes_abiertas"].ToString(), k["ordenes_listas"] + " listas para entregar", Tema.TextoSuave);
                int alertas = (int)k["alertas_stock"], agotados = (int)k["agotados"];
                Kpi(5, alertas.ToString(), agotados > 0 ? agotados + " agotado(s)" : "ninguno agotado", agotados > 0 ? Tema.Rojo : Tema.Verde);
                Kpi(6, Convert.ToInt32(k["clientes_activos"]).ToString("N0", Mx), "+" + k["clientes_nuevos"] + " nuevos en 90 días", Tema.TextoSuave);
                Kpi(7, ((decimal)k["valor_inventario"]).ToString("C0", Mx), k["garantias_vigentes"] + " garantías vigentes", Tema.TextoSuave);

                // Ingresos por mes: se completan los meses sin ventas con 0
                var meses = DashboardRepository.IngresosPorMes().Rows.Cast<DataRow>()
                    .ToDictionary(r => (string)r["mes"], r => (decimal)r["ingresos"]);
                var pts = chtIngresos.Series[0].Points;
                pts.Clear();
                for (int i = 5; i >= 0; i--)
                {
                    var m = DateTime.Today.AddMonths(-i);
                    decimal v;
                    meses.TryGetValue(m.ToString("yyyy-MM"), out v);
                    pts.AddXY(Mx.TextInfo.ToTitleCase(m.ToString("MMM", Mx).TrimEnd('.')), v);
                }
                if (pts.Count > 0) pts[pts.Count - 1].Color = Tema.Acento; // mes actual

                var dona = chtCategorias.Series[0].Points;
                dona.Clear();
                foreach (DataRow r in DashboardRepository.VentasPorCategoria().Rows)
                {
                    int idx = dona.AddXY(r["categoria"], r["ingresos"]);
                    dona[idx].LegendText = r["categoria"] + "  " + ((decimal)r["ingresos"]).ToString("C0", Mx);
                }

                dgvRecientes.DataSource = DashboardRepository.VentasRecientes();
                dgvMasVendidos.DataSource = DashboardRepository.MasVendidos();
                dgvAlertas.DataSource = DashboardRepository.AlertasStock();
                dgvOrdenes.DataSource = DashboardRepository.OrdenesAbiertas();
                lblActualizado.Text = "Actualizado a las " + DateTime.Now.ToString("HH:mm");
            }
            catch (Exception ex) { Ui.Error(ex, "No se pudo cargar el dashboard"); }
            finally { Cursor = Cursors.Default; }
        }

        private void Kpi(int i, string valor, Tuple<string, Color> nota) => Kpi(i, valor, nota.Item1, nota.Item2);

        private void Kpi(int i, string valor, string nota, Color color)
        {
            kpiValor[i].Text = valor;
            kpiNota[i].Text = nota;
            kpiNota[i].ForeColor = color;
        }

        private static Tuple<string, Color> Variacion(decimal actual, decimal anterior)
        {
            if (anterior == 0) return Tuple.Create(actual > 0 ? "sin datos del periodo anterior" : "sin movimientos", Tema.TextoSuave);
            decimal pct = (actual - anterior) * 100 / anterior;
            return Tuple.Create((pct >= 0 ? "▲ " : "▼ ") + Math.Abs(pct).ToString("0.#", Mx) + " % vs. 90 días previos", pct >= 0 ? Tema.Verde : Tema.Rojo);
        }

        private static string Saludo()
        {
            int h = DateTime.Now.Hour;
            return h < 12 ? "Buenos días" : h < 19 ? "Buenas tardes" : "Buenas noches";
        }
    }
}
