using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulos 11 y 12 · Historial con filtros múltiples (fechas, estado, texto)
    /// sobre vw_VentaCompleta, y cancelación controlada con usp_CancelarVenta.
    /// </summary>
    public partial class FrmHistorialVentas : Form
    {
        public FrmHistorialVentas()
        {
            InitializeComponent();
            Ui.Grid(dgvVentas);
            Ui.Grid(dgvDetalle);
            // Vendedor, sucursal, pago y desglose quedan en el detalle de abajo
            Ui.Mostrar(dgvVentas, "folio", "fecha", "cliente", "total", "estado");
            Ui.Mostrar(dgvDetalle, "nombre=Producto", "numero_serie", "cantidad", "subtotal=Importe", "garantia_hasta");
            dtpDesde.Value = DateTime.Today.AddMonths(-6);
            dtpHasta.Value = DateTime.Today;
            cboEstado.SelectedIndex = 0;
        }

        private void FrmHistorialVentas_Load(object sender, EventArgs e)
        {
            btnCancelarVenta.Visible = Db.Sesion.Puede(Capacidad.CancelarVenta);
            Buscar();
        }

        private void Buscar()
        {
            if (dtpHasta.Value < dtpDesde.Value) { Ui.Info("La fecha final no puede ser menor que la inicial."); return; }
            try
            {
                string estado = cboEstado.SelectedIndex <= 0 ? null : cboEstado.Text;
                dgvVentas.DataSource = VentaRepository.Buscar(dtpDesde.Value, dtpHasta.Value, estado, txtBuscar.Text);
                lblConteo.Text = dgvVentas.Rows.Count + " venta(s)";
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) Buscar(); }

        private void dgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null) return;
            try { dgvDetalle.DataSource = VentaRepository.Detalle((int)dgvVentas.CurrentRow.Cells["id_venta"].Value); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void dgvVentas_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvVentas.Rows[e.RowIndex];
            if (Convert.ToString(row.Cells["estado"].Value) == "Cancelada") row.DefaultCellStyle.BackColor = Color.MistyRose;
        }

        private void btnCancelarVenta_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null) { Ui.Info("Selecciona una venta."); return; }
            var r = dgvVentas.CurrentRow;
            string folio = (string)r.Cells["folio"].Value;
            if ((string)r.Cells["estado"].Value == "Cancelada") { Ui.Info("La venta " + folio + " ya está cancelada."); return; }

            string motivo = Microsoft.VisualBasic.Interaction.InputBox(
                "Motivo de la cancelación de " + folio + ":", "Cancelar venta", "");
            if (string.IsNullOrWhiteSpace(motivo)) return;
            if (!Ui.Confirmar("¿Cancelar la venta " + folio + " por " + Convert.ToDecimal(r.Cells["total"].Value).ToString("C") +
                              "?\nSe reintegrará el stock y se liberarán las piezas con número de serie.")) return;
            try
            {
                VentaRepository.Cancelar((int)r.Cells["id_venta"].Value, motivo.Trim());
                Ui.Info("Venta " + folio + " cancelada.");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se pudo cancelar"); }
        }
    }
}
