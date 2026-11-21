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
        // Un solo acento: escala del verde azulado y de la tinta (sin arcoíris)
        private static readonly Color[] Paleta =
        {
            Color.FromArgb(1, 106, 113), Color.FromArgb(46, 138, 144), Color.FromArgb(94, 167, 172), Color.FromArgb(144, 195, 198),
            Color.FromArgb(192, 220, 222), Color.FromArgb(39, 37, 30), Color.FromArgb(85, 83, 77), Color.FromArgb(130, 128, 122),
            Color.FromArgb(174, 172, 166), Color.FromArgb(214, 211, 205),
        };
        private static readonly Color ColumnaClara = Color.FromArgb(201, 223, 225);

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
            serie.Color = ColumnaClara;
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
            var c = new Chart { BackColor = Tema.Tarjeta, Palette = ChartColorPalette.None, PaletteCustomColors = Paleta };
            var a = new ChartArea("principal") { BackColor = Tema.Tarjeta };
            a.AxisX.MajorGrid.Enabled = false;
            a.AxisX.LineColor = Tema.Borde;
            a.AxisX.MajorTickMark.Enabled = false;
            a.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.5F);
            a.AxisX.LabelStyle.ForeColor = Tema.TextoSuave;
            a.AxisX.Interval = 1;
            a.AxisY.MajorGrid.LineColor = Color.FromArgb(239, 236, 230);
            a.AxisY.LineColor = Tema.Tarjeta;
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
                Func<decimal, string> dinero = v => v.ToString("C0", Mx), entero = v => v.ToString("N0", Mx);
                Kpi(0, ingresos, dinero, Variacion(ingresos, ingresosAnt));
                Kpi(1, ventas, entero, Variacion(ventas, ventasAnt));
                Kpi(2, ventas == 0 ? 0 : ingresos / ventas, dinero, "por venta completada", Tema.TextoSuave);
                Kpi(3, Convert.ToInt32(k["unidades_mes"]), entero, "piezas en 90 días", Tema.TextoSuave);
                Kpi(4, (int)k["ordenes_abiertas"], entero, k["ordenes_listas"] + " listas para entregar", Tema.TextoSuave);
                int alertas = (int)k["alertas_stock"], agotados = (int)k["agotados"];
                Kpi(5, alertas, entero, agotados > 0 ? agotados + " agotado(s)" : "ninguno agotado", agotados > 0 ? Tema.Rojo : Tema.Verde);
                Kpi(6, Convert.ToInt32(k["clientes_activos"]), entero, "+" + k["clientes_nuevos"] + " nuevos en 90 días", Tema.TextoSuave);
                Kpi(7, (decimal)k["valor_inventario"], dinero, k["garantias_vigentes"] + " garantías vigentes", Tema.TextoSuave);

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
                if (pts.Count > 0) pts[pts.Count - 1].Color = Tema.Navegacion; // mes actual
                // Las columnas crecen desde cero (eje fijo para que no "salte")
                var finales = pts.Select(x => x.YValues[0]).ToArray();
                chtIngresos.ChartAreas[0].AxisY.Maximum = Math.Max(1, finales.Max() * 1.18);
                Animacion.Ejecutar(650, Animacion.Salida, p =>
                {
                    for (int i = 0; i < pts.Count; i++) pts[i].YValues[0] = finales[i] * p;
                    chtIngresos.Invalidate();
                });

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

        private void Kpi(int i, decimal valor, Func<decimal, string> formato, Tuple<string, Color> nota) => Kpi(i, valor, formato, nota.Item1, nota.Item2);

        /// <summary>El número cuenta desde 0 hasta su valor (como los widgets de Apple).</summary>
        private void Kpi(int i, decimal valor, Func<decimal, string> formato, string nota, Color color)
        {
            kpiNota[i].Text = nota;
            kpiNota[i].ForeColor = color;
            var etiqueta = kpiValor[i];
            Animacion.Ejecutar(700, Animacion.Salida, p => etiqueta.Text = formato(Math.Round(valor * (decimal)p, 0)));
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
