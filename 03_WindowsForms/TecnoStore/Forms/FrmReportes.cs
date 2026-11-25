using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 13 · Reportes (5): resumen por periodo (SP), top productos (vista),
    /// actividad por empleado (SP), stock bajo (vista con LEFT JOIN) y ventas por
    /// categoría (función de tabla). El periodo se elige con un botón o con fechas.
    /// </summary>
    public partial class FrmReportes : Form, IRefrescable
    {
        private readonly Segmentos segReporte = new Segmentos();
        private readonly Segmentos segPeriodo = new Segmentos("Este mes", "3 meses", "6 meses", "Otro");
        private readonly Panel barraFechas, zona = new Panel();
        private readonly List<KeyValuePair<string, DataGridView>> reportes = new List<KeyValuePair<string, DataGridView>>();
        private bool cambiando;

        public FrmReportes()
        {
            InitializeComponent();
            foreach (var g in new[] { dgvResumen, dgvTop, dgvEmpleados, dgvStock, dgvCategorias }) Ui.Grid(g);
            Ui.Mostrar(dgvTop, "nombre=Producto", "categoria=Categoría", "unidades_vendidas", "ingresos", "utilidad_bruta");
            Ui.Mostrar(dgvStock, "nombre=Producto", "stock_actual", "stock_minimo", "faltante=Faltan", "proveedor_sugerido=Proveedor sugerido");
            Agregar("Por mes", dgvResumen);
            Agregar("Más vendidos", dgvTop);
            Agregar("Por categoría", dgvCategorias);
            // Desempeño por empleado solo para quien la base deja ver costos
            if (Db.Sesion.Puede(Capacidad.VerCostos)) Agregar("Por empleado", dgvEmpleados);
            Agregar("Por resurtir", dgvStock);
            segReporte.Poner(reportes.Select(r => r.Key).ToArray());
            segReporte.Elegido += Ver;
            segPeriodo.Activo = 2;
            segPeriodo.Elegido += Periodo;

            Sencillo.Vaciar(this);
            btnExportar.Text = "Exportar a Excel";
            Sencillo.Barra(pnlFiltros, new Control[] { segReporte }, btnExportar);
            lblKpi.AutoSize = true; lblKpi.Font = Tema.FuenteSubtitulo;
            var barraPeriodo = Sencillo.Barra(new Panel(), new Control[] { segPeriodo, lblKpi });
            barraFechas = Sencillo.Barra(new Panel(), new Control[] { new Label { Text = "Desde", AutoSize = true }, dtpDesde, new Label { Text = "hasta", AutoSize = true }, dtpHasta });
            barraFechas.Visible = false;
            dtpDesde.ValueChanged += (s, e) => { if (!cambiando) Refrescar(); };
            dtpHasta.ValueChanged += (s, e) => { if (!cambiando) Refrescar(); };
            foreach (var r in reportes) { r.Value.Visible = false; zona.Controls.Add(r.Value); }
            Sencillo.Armar(this, zona, pnlFiltros, barraPeriodo, barraFechas);
            Ver(0);
        }

        private void Agregar(string nombre, DataGridView g) => reportes.Add(new KeyValuePair<string, DataGridView>(nombre, g));
        private void Ver(int i) { for (int k = 0; k < reportes.Count; k++) reportes[k].Value.Visible = k == i; }

        private void FrmReportes_Load(object sender, EventArgs e) => Periodo(segPeriodo.Activo);

        private void Periodo(int i)
        {
            barraFechas.Visible = i == 3;
            if (i == 3) return;
            cambiando = true;
            var hoy = DateTime.Today;
            dtpDesde.Value = i == 0 ? new DateTime(hoy.Year, hoy.Month, 1) : hoy.AddMonths(i == 1 ? -3 : -6);
            dtpHasta.Value = hoy;
            cambiando = false;
            Refrescar();
        }

        public void Refrescar()
        {
            if (dtpHasta.Value.Date < dtpDesde.Value.Date) return;
            Cursor = Cursors.WaitCursor;
            try
            {
                dgvResumen.DataSource = ReporteRepository.ResumenPeriodo(dtpDesde.Value, dtpHasta.Value);
                if (Db.Sesion.Puede(Capacidad.VerCostos)) dgvEmpleados.DataSource = ReporteRepository.ActividadEmpleados(dtpDesde.Value, dtpHasta.Value);
                dgvCategorias.DataSource = ReporteRepository.VentasPorCategoria(dtpDesde.Value, dtpHasta.Value);
                dgvStock.DataSource = ReporteRepository.StockBajo();
                dgvTop.DataSource = ReporteRepository.TopProductos();
                var resumen = (DataTable)dgvResumen.DataSource;
                decimal ingresos = resumen.Rows.Cast<DataRow>().Sum(r => (decimal)r["ingresos"]);
                int ventas = resumen.Rows.Cast<DataRow>().Sum(r => (int)r["ventas"]);
                lblKpi.Text = ventas + " ventas   ·   " + ingresos.ToString("C0");
            }
            catch (Exception ex) { Ui.Error(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            var actual = reportes[segReporte.Activo];
            var grid = actual.Value;
            if (grid.Rows.Count == 0) { Ui.Info("No hay datos para exportar."); return; }
            using (var dlg = new SaveFileDialog { Filter = "CSV para Excel (*.csv)|*.csv", FileName = actual.Key + ".csv" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Join(",", cols.Select(c => Csv(c.HeaderText))));
                    foreach (DataGridViewRow r in grid.Rows)
                        sb.AppendLine(string.Join(",", cols.Select(c => Csv(Convert.ToString(r.Cells[c.Index].Value)))));
                    File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));   // BOM para que Excel lea acentos
                    Ui.Aviso("Reporte exportado");
                }
                catch (Exception ex) { Ui.Error(ex, "No se pudo exportar"); }
            }
        }

        private static string Csv(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
