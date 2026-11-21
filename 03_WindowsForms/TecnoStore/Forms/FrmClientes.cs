using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 6 · Clientes: CRUD + historial de compras.
    /// Total y nivel se calculan en SQL Server con fn_TotalCompradoCliente y fn_ClasificacionCliente.
    /// Bajas lógicas: un cliente con historial se desactiva, no se borra.
    /// </summary>
    public partial class FrmClientes : Form
    {
        private int? idActual;
        private bool editando;

        public FrmClientes()
        {
            InitializeComponent();
            Ui.Grid(dgvClientes);
            Ui.Grid(dgvHistorial);
            Ui.Mostrar(dgvClientes, "nombre", "apellidos", "correo", "total_comprado", "nivel");
            Ui.Mostrar(dgvHistorial, "folio", "fecha", "total", "estado");
            btnNuevo.Visible = btnEditar.Visible = btnGuardar.Visible = btnCancelar.Visible = Db.Sesion.Puede(Capacidad.GuardarCliente);
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            try { Buscar(); Estado(false); } catch (Exception ex) { Ui.Error(ex); }
        }

        private void Buscar()
        {
            dgvClientes.DataSource = ClienteRepository.Buscar(txtBuscar.Text);
            lblConteo.Text = dgvClientes.Rows.Count + " cliente(s)";
        }

        private void Estado(bool edicion)
        {
            editando = edicion;
            bool puede = Db.Sesion.Puede(Capacidad.GuardarCliente);
            grpDatos.Enabled = edicion;
            btnNuevo.Enabled = !edicion && puede;
            btnEditar.Enabled = !edicion && puede && idActual != null;
            btnGuardar.Enabled = edicion;
            btnCancelar.Enabled = edicion;
            dgvClientes.Enabled = !edicion;
            grpDatos.Text = !edicion ? "Datos del cliente (lectura)" : idActual == null ? "Nuevo cliente" : "Editando cliente";
        }

        private void Limpiar()
        {
            idActual = null;
            txtNombre.Clear(); txtApellidos.Clear(); txtCorreo.Clear(); txtTelefono.Clear(); txtRfc.Clear();
            chkActivo.Checked = true;
            foreach (Control c in grpDatos.Controls) { errorProvider.SetError(c, ""); if (c is TextBox) c.BackColor = SystemColors.Window; }
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (editando || dgvClientes.CurrentRow == null) return;
            var r = dgvClientes.CurrentRow;
            idActual = (int)r.Cells["id_cliente"].Value;
            txtNombre.Text = Convert.ToString(r.Cells["nombre"].Value);
            txtApellidos.Text = Convert.ToString(r.Cells["apellidos"].Value);
            txtCorreo.Text = Convert.ToString(r.Cells["correo"].Value);
            txtTelefono.Text = Convert.ToString(r.Cells["telefono"].Value);
            txtRfc.Text = Convert.ToString(r.Cells["rfc"].Value);
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            try
            {
                dgvHistorial.DataSource = ClienteRepository.Historial(idActual.Value);
                grpHistorial.Text = "Historial de compras (" + dgvHistorial.Rows.Count + ")";
            }
            catch (Exception ex) { Ui.Error(ex); }
            Estado(false);
        }

        private void dgvClientes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvClientes.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }

        private void btnBuscar_Click(object sender, EventArgs e) { try { Buscar(); } catch (Exception ex) { Ui.Error(ex); } }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) btnBuscar.PerformClick(); }
        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); dgvHistorial.DataSource = null; Estado(true); txtNombre.Focus(); }
        private void btnEditar_Click(object sender, EventArgs e) { Estado(true); txtNombre.Focus(); }
        private void btnCancelar_Click(object sender, EventArgs e) { Estado(false); Limpiar(); dgvClientes_SelectionChanged(null, null); }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtNombre, errorProvider, "Obligatorio")
                    & Ui.Requerido(txtApellidos, errorProvider, "Obligatorio")
                    & Ui.Requerido(txtCorreo, errorProvider, "Obligatorio");
            if (ok && !txtCorreo.Text.Contains("@")) { errorProvider.SetError(txtCorreo, "Correo no válido"); ok = false; }
            if (!ok) return;

            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                ClienteRepository.Guardar(idActual, txtNombre.Text.Trim(), txtApellidos.Text.Trim(), txtCorreo.Text,
                                          txtTelefono.Text.Trim(), txtRfc.Text, chkActivo.Checked);
                Ui.Info(nuevo ? "Cliente registrado." : "Cambios guardados.");
                Estado(false);
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); btnGuardar.Enabled = true; }
        }
    }
}
