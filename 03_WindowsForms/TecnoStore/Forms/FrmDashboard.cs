using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Dashboard: lo que hay que saber de un vistazo, en una sola pantalla
    /// (Stephen Few; NN/g). Cuatro indicadores, dos gráficas de barras (sin
    /// pastel ni dona) y dos listas de "requiere atención" con 5 filas.
    /// El detalle vive en Reportes. Solo lectura; usa vistas y funciones existentes.
    /// </summary>
    public partial class FrmDashboard : Form, IRefrescable
    {
        private static readonly CultureInfo Mx = new CultureInfo("es-MX");
        private static readonly Color ColumnaClara = Color.FromArgb(212, 212, 212);

        private Chart chtIngresos, chtCategorias;
        private DataGridView dgvAlertas, dgvOrdenes;
        private readonly Label[] kpiValor = new Label[4];
        private readonly Label[] kpiNota = new Label[4];

        public FrmDashboard()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Tema.Fondo;
            Construir();
        }

        private void FrmDashboard_Load(object sender, EventArgs e) => Cargar();
        public void Refrescar() => Cargar();

        private void Construir()
        {
            // El indicador más importante arriba a la izquierda
            string[] titulos = { "Ingresos · 90 días", "Ventas · 90 días", "Equipos en taller", "Productos por resurtir" };
            for (int i = 0; i < 4; i++)
            {
                var t = new Tarjeta { Dock = DockStyle.Fill, Margin = new Padding(6) };
                var lt = new Label { Text = titulos[i], AutoSize = true, ForeColor = Tema.TextoSuave, Font = Tema.Fuente, Location = new Point(16, 12) };
                kpiValor[i] = new Label { Text = "—", AutoSize = true, ForeColor = Tema.Texto, Font = Tema.FuenteKpi, Location = new Point(13, 36) };
                kpiNota[i] = new Label { Text = "", AutoSize = true, ForeColor = Tema.TextoSuave, Font = Tema.FuentePequena, Location = new Point(16, 78) };
                t.Controls.AddRange(new Control[] { lt, kpiValor[i], kpiNota[i] });
                tlpDashboard.Controls.Add(t, i, 0);
            }

            chtIngresos = NuevaGrafica();
            chtIngresos.ChartAreas[0].AxisY.LabelStyle.Format = "$#,##0,K";
            var serie = chtIngresos.Series.Add("Ingresos");
            serie.ChartType = SeriesChartType.Column;
            serie.Color = ColumnaClara;
            serie["PointWidth"] = "0.55";
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "$#,##0,K";
            serie.Font = Tema.FuentePequena;
            serie.LabelForeColor = Tema.TextoSuave;
            AgregarAncho(Contenedor("Ingresos por mes", chtIngresos), 0, 1);

            // Categorías en barras horizontales: se comparan mejor que en una dona
            chtCategorias = NuevaGrafica();
            var area = chtCategorias.ChartAreas[0];
            area.AxisY.LabelStyle.Format = "$#,##0,K";
            area.AxisX.IsReversed = true;   // la mayor arriba
            var barras = chtCategorias.Series.Add("Categorías");
            barras.ChartType = SeriesChartType.Bar;
            barras.Color = Tema.Navegacion;
            barras["PointWidth"] = "0.6";
            barras.IsValueShownAsLabel = true;
            barras.LabelFormat = "$#,##0,K";
            barras.Font = Tema.FuentePequena;
            barras.LabelForeColor = Tema.TextoSuave;
            AgregarAncho(Contenedor("Ventas por categoría · 90 días", chtCategorias), 2, 1);

            dgvAlertas = NuevaTabla();
            dgvOrdenes = NuevaTabla();
            Ui.Mostrar(dgvAlertas, "producto", "stock", "minimo");
            Ui.Mostrar(dgvOrdenes, "equipo", "estado", "dias");
            AgregarAncho(Contenedor("Por resurtir", dgvAlertas), 0, 2);
            AgregarAncho(Contenedor("Equipos en el taller", dgvOrdenes), 2, 2);
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
            var t = new Tarjeta { Dock = DockStyle.Fill, Margin = new Padding(6), Padding = new Padding(14, 44, 14, 12) };
            t.Controls.Add(new Label { Text = titulo, AutoSize = true, Font = Tema.FuenteNegrita, ForeColor = Tema.Texto, Location = new Point(16, 13) });
            contenido.Dock = DockStyle.Fill;
            t.Controls.Add(contenido);
            return t;
        }

        private static Chart NuevaGrafica()
        {
            var c = new Chart { BackColor = Tema.Tarjeta };
            var a = new ChartArea("principal") { BackColor = Tema.Tarjeta };
            a.AxisX.MajorGrid.Enabled = false;
            a.AxisX.LineColor = Tema.LineaSuave;
            a.AxisX.MajorTickMark.Enabled = false;
            a.AxisX.LabelStyle.Font = Tema.FuentePequena;
            a.AxisX.LabelStyle.ForeColor = Tema.TextoSuave;
            a.AxisX.Interval = 1;
            a.AxisY.MajorGrid.LineColor = Color.FromArgb(245, 245, 245);
            a.AxisY.LineColor = Tema.Tarjeta;
            a.AxisY.MajorTickMark.Enabled = false;
            a.AxisY.LabelStyle.Font = Tema.FuentePequena;
            a.AxisY.LabelStyle.ForeColor = Tema.TextoSuave;
            c.ChartAreas.Add(a);
            return c;
        }

        private static DataGridView NuevaTabla()
        {
            var g = new DataGridView { Name = "compacta" };
            Ui.Grid(g);
            Tema.Tabla(g);
            return g;
        }

        private void Cargar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var k = DashboardRepository.Indicadores();
                decimal ingresos = (decimal)k["ingresos_mes"], ingresosAnt = (decimal)k["ingresos_ant"];
                int ventas = (int)k["ventas_mes"], ventasAnt = (int)k["ventas_ant"];
                Func<decimal, string> dinero = v => v.ToString("C0", Mx), entero = v => v.ToString("N0", Mx);
                Kpi(0, ingresos, dinero, Variacion(ingresos, ingresosAnt));
                Kpi(1, ventas, entero, Variacion(ventas, ventasAnt));
                Kpi(2, (int)k["ordenes_abiertas"], entero, k["ordenes_listas"] + " listos para entregar", Tema.TextoSuave);
                int agotados = (int)k["agotados"];
                Kpi(3, (int)k["alertas_stock"], entero, agotados > 0 ? agotados + " agotado(s)" : "ninguno agotado", agotados > 0 ? Tema.Rojo : Tema.Verde);

                // Ingresos por mes: meses sin ventas en 0; el actual en tinta
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
                pts[pts.Count - 1].Color = Tema.Navegacion;
                Crecer(chtIngresos);

                // Las 6 categorías que más venden
                var cat = chtCategorias.Series[0].Points;
                cat.Clear();
                foreach (DataRow r in DashboardRepository.VentasPorCategoria().Rows.Cast<DataRow>().Take(6))
                    cat.AddXY(r["categoria"], r["ingresos"]);
                Crecer(chtCategorias);

                dgvAlertas.DataSource = Primeras(DashboardRepository.AlertasStock(), 5);
                dgvOrdenes.DataSource = Primeras(DashboardRepository.OrdenesAbiertas(), 5);
            }
            catch (Exception ex) { Ui.Error(ex, "No se pudo cargar el dashboard"); }
            finally { Cursor = Cursors.Default; }
        }

        private static DataTable Primeras(DataTable t, int n)
        {
            var copia = t.Clone();
            foreach (DataRow r in t.Rows.Cast<DataRow>().Take(n)) copia.ImportRow(r);
            return copia;
        }

        /// <summary>Las barras crecen desde cero (eje fijo para que no "salte").</summary>
        private static void Crecer(Chart c)
        {
            var pts = c.Series[0].Points;
            if (pts.Count == 0) return;
            var finales = pts.Select(x => x.YValues[0]).ToArray();
            c.ChartAreas[0].AxisY.Maximum = Math.Max(1, finales.Max() * 1.2);
            Animacion.Ejecutar(650, Animacion.Salida, p =>
            {
                for (int i = 0; i < pts.Count; i++) pts[i].YValues[0] = finales[i] * p;
                c.Invalidate();
            });
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
    }
}
