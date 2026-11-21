using System;
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
    /// actividad por empleado (SP), stock bajo (vista con LEFT JOIN) y
    /// ventas por categoría (función de tabla). Exporta a CSV.
    /// </summary>
    public partial class FrmReportes : Form
    {
        public FrmReportes()
        {
            InitializeComponent();
            foreach (var g in new[] { dgvResumen, dgvTop, dgvEmpleados, dgvStock, dgvCategorias }) Ui.Grid(g);
            Ui.Mostrar(dgvTop, "nombre=Producto", "categoria=Categoría", "unidades_vendidas", "ingresos", "utilidad_bruta");
            Ui.Mostrar(dgvStock, "nombre=Producto", "stock_actual", "stock_minimo", "faltante=Faltan", "proveedor_sugerido=Proveedor sugerido");
            // Desempeño por empleado y utilidad solo para quien la base deja ver costos
            if (!Db.Sesion.Puede(Capacidad.VerCostos)) tabReportes.TabPages.Remove(tabEmpleados);
            dtpDesde.Value = DateTime.Today.AddMonths(-6);
            dtpHasta.Value = DateTime.Today;
        }

        private void FrmReportes_Load(object sender, EventArgs e) => Generar();

        private void Generar()
        {
            if (dtpHasta.Value < dtpDesde.Value) { Ui.Info("La fecha final no puede ser menor que la inicial."); return; }
            Cursor = Cursors.WaitCursor;
            try
            {
                dgvResumen.DataSource = ReporteRepository.ResumenPeriodo(dtpDesde.Value, dtpHasta.Value);
                dgvEmpleados.DataSource = ReporteRepository.ActividadEmpleados(dtpDesde.Value, dtpHasta.Value);
                dgvCategorias.DataSource = ReporteRepository.VentasPorCategoria(dtpDesde.Value, dtpHasta.Value);
                dgvStock.DataSource = ReporteRepository.StockBajo();
                dgvTop.DataSource = ReporteRepository.TopProductos();
                var resumen = (DataTable)dgvResumen.DataSource;
                decimal ingresos = resumen.Rows.Cast<DataRow>().Sum(r => (decimal)r["ingresos"]);
                int ventas = resumen.Rows.Cast<DataRow>().Sum(r => (int)r["ventas"]);
                lblKpi.Text = "Periodo: " + ventas + " ventas · " + ingresos.ToString("C") + " · alertas de stock: " + dgvStock.Rows.Count;
            }
            catch (Exception ex) { Ui.Error(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void btnGenerar_Click(object sender, EventArgs e) => Generar();

        private void btnExportar_Click(object sender, EventArgs e)
        {
            var grid = tabReportes.SelectedTab.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid == null || grid.Rows.Count == 0) { Ui.Info("No hay datos para exportar."); return; }
            using (var dlg = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = tabReportes.SelectedTab.Text + ".csv" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(",", cols.Select(c => Csv(c.HeaderText))));
                foreach (DataGridViewRow r in grid.Rows)
                    sb.AppendLine(string.Join(",", cols.Select(c => Csv(Convert.ToString(r.Cells[c.Index].Value)))));
                File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));   // BOM para que Excel lea acentos
                Ui.Info("Reporte exportado.");
            }
        }

        private static string Csv(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
