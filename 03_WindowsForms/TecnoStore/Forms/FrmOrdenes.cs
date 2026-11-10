using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 10 · Proceso secundario (maestro-detalle): órdenes de servicio del taller.
    /// Alta: usp_RegistrarOrdenServicio · Líneas: usp_OrdenAgregarLinea ·
    /// Estados: usp_CambiarEstadoOrden (el trigger guarda el historial).
    /// </summary>
    public partial class FrmOrdenes : Form
    {
        public FrmOrdenes()
        {
            InitializeComponent();
            Ui.Grid(dgvOrdenes);
            Ui.Grid(dgvDetalle);
            Ui.Grid(dgvHistorial);
        }

        private int? IdSeleccionada => dgvOrdenes.CurrentRow == null ? (int?)null : (int)dgvOrdenes.CurrentRow.Cells["id_orden"].Value;

        private void FrmOrdenes_Load(object sender, EventArgs e)
        {
            try
            {
                cboCliente.DataSource = CatalogoRepository.ClientesActivos();
                cboCliente.SelectedIndex = -1;
                cboTecnico.DataSource = CatalogoRepository.Tecnicos();
                cboServicio.DataSource = CatalogoRepository.TiposServicio();
                cboRefaccion.DataSource = CatalogoRepository.Refacciones();
                cboEstado.DataSource = CatalogoRepository.EstadosOrden();
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Buscar()
        {
            int? sel = IdSeleccionada;
            dgvOrdenes.DataSource = OrdenRepository.Buscar(chkAbiertas.Checked);
            if (sel != null)
                foreach (DataGridViewRow r in dgvOrdenes.Rows)
                    if ((int)r.Cells["id_orden"].Value == sel) { dgvOrdenes.CurrentCell = r.Cells["folio"]; break; }
        }

        private void chkAbiertas_CheckedChanged(object sender, EventArgs e) { try { Buscar(); } catch (Exception ex) { Ui.Error(ex); } }

        private void dgvOrdenes_SelectionChanged(object sender, EventArgs e)
        {
            int? id = IdSeleccionada;
            if (id == null) return;
            try
            {
                dgvDetalle.DataSource = OrdenRepository.Detalle(id.Value);
                dgvHistorial.DataSource = OrdenRepository.Historial(id.Value);
                bool cerrada = (bool)dgvOrdenes.CurrentRow.Cells["es_final"].Value;
                grpAcciones.Enabled = !cerrada && Db.Sesion.PuedeOperar;
                grpAcciones.Text = "Orden " + dgvOrdenes.CurrentRow.Cells["folio"].Value + (cerrada ? " (cerrada)" : "");
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void dgvOrdenes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvOrdenes.Rows[e.RowIndex];
            string estado = Convert.ToString(row.Cells["estado"].Value);
            row.DefaultCellStyle.BackColor = estado == "Cancelada" ? Color.MistyRose : estado == "Lista" ? Color.Honeydew : SystemColors.Window;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(cboCliente, errorProvider, "Selecciona el cliente")
                    & Ui.Requerido(cboTecnico, errorProvider, "Selecciona el técnico")
                    & Ui.Requerido(txtEquipo, errorProvider, "Describe el equipo")
                    & Ui.Requerido(txtFalla, errorProvider, "Describe la falla");
            if (!ok) return;
            btnRegistrar.Enabled = false;
            try
            {
                string folio = OrdenRepository.Registrar((int)cboCliente.SelectedValue, (int)cboTecnico.SelectedValue,
                                                         txtEquipo.Text.Trim(), txtFalla.Text.Trim(), txtSerie.Text);
                Ui.Info("Orden " + folio + " registrada. Se agregó el diagnóstico.");
                txtEquipo.Clear(); txtFalla.Clear(); txtSerie.Clear(); cboCliente.SelectedIndex = -1;
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se registró la orden"); }
            finally { btnRegistrar.Enabled = true; }
        }

        private void btnAgregarServicio_Click(object sender, EventArgs e)
        {
            if (IdSeleccionada == null || cboServicio.SelectedValue == null) return;
            try { OrdenRepository.AgregarLinea(IdSeleccionada.Value, (int)cboServicio.SelectedValue, null, 1); Buscar(); dgvOrdenes_SelectionChanged(null, null); }
            catch (Exception ex) { Ui.Error(ex, "No se agregó"); }
        }

        private void btnAgregarRefaccion_Click(object sender, EventArgs e)
        {
            if (IdSeleccionada == null || cboRefaccion.SelectedValue == null) return;
            try
            {
                OrdenRepository.AgregarLinea(IdSeleccionada.Value, null, (int)cboRefaccion.SelectedValue, (int)nudCantidad.Value);
                cboRefaccion.DataSource = CatalogoRepository.Refacciones();   // stock actualizado
                Buscar(); dgvOrdenes_SelectionChanged(null, null);
            }
            catch (Exception ex) { Ui.Error(ex, "No se agregó"); }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (IdSeleccionada == null) return;
            var estado = (Opcion)cboEstado.SelectedItem;
            if (estado.Texto == "Cancelada" && !Ui.Confirmar("¿Cancelar la orden? Las refacciones regresan al almacén.")) return;
            try { OrdenRepository.CambiarEstado(IdSeleccionada.Value, estado.Id); Buscar(); dgvOrdenes_SelectionChanged(null, null); }
            catch (Exception ex) { Ui.Error(ex, "No se cambió el estado"); }
        }
    }
}
