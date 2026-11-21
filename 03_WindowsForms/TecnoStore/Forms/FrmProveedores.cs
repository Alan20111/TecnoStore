using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 7 · Proveedores: CRUD con usp_ProveedorGuardar y sus relaciones
    /// (productos que surte por la tabla N:M ProductoProveedor y compras).
    /// Supervisor y Administrador modifican; los demás consultan.
    /// </summary>
    public partial class FrmProveedores : Form
    {
        private int? idActual;
        private bool editando;

        public FrmProveedores()
        {
            InitializeComponent();
            Ui.Grid(dgvProveedores);
            Ui.Grid(dgvProductos);
            Ui.Mostrar(dgvProveedores, "razon_social", "rfc", "telefono", "productos=Productos");
            Ui.Mostrar(dgvProductos, "nombre=Producto", "costo_proveedor", "stock_actual");
            btnNuevo.Visible = btnEditar.Visible = btnGuardar.Visible = btnCancelar.Visible = Db.Sesion.Puede(Capacidad.GuardarProveedor);
        }

        private void FrmProveedores_Load(object sender, EventArgs e) { Buscar(); Estado(false); }

        private void Buscar()
        {
            try
            {
                dgvProveedores.DataSource = ProveedorRepository.Buscar(txtBuscar.Text);
                lblConteo.Text = dgvProveedores.Rows.Count + " proveedor(es)";
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Estado(bool edicion)
        {
            editando = edicion;
            bool puede = Db.Sesion.Puede(Capacidad.GuardarProveedor);
            grpDatos.Enabled = edicion;
            btnNuevo.Enabled = !edicion && puede;
            btnEditar.Enabled = !edicion && puede && idActual != null;
            btnGuardar.Enabled = edicion;
            btnCancelar.Enabled = edicion;
            dgvProveedores.Enabled = !edicion;
            grpDatos.Text = !edicion ? "Datos del proveedor (lectura)" : idActual == null ? "Nuevo proveedor" : "Editando proveedor";
        }

        private void dgvProveedores_SelectionChanged(object sender, EventArgs e)
        {
            if (editando || dgvProveedores.CurrentRow == null) return;
            var r = dgvProveedores.CurrentRow;
            idActual = (int)r.Cells["id_proveedor"].Value;
            txtRazonSocial.Text = Convert.ToString(r.Cells["razon_social"].Value);
            txtRfc.Text = Convert.ToString(r.Cells["rfc"].Value);
            txtCorreo.Text = Convert.ToString(r.Cells["correo"].Value);
            txtTelefono.Text = Convert.ToString(r.Cells["telefono"].Value);
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            try
            {
                dgvProductos.DataSource = ProveedorRepository.ProductosQueSurte(idActual.Value);
                grpProductos.Text = "Productos que surte (" + dgvProductos.Rows.Count + ")";
            }
            catch (Exception ex) { Ui.Error(ex); }
            Estado(false);
        }

        private void dgvProveedores_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvProveedores.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) Buscar(); }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtRazonSocial.Clear(); txtRfc.Clear(); txtCorreo.Clear(); txtTelefono.Clear(); chkActivo.Checked = true;
            dgvProductos.DataSource = null;
            Estado(true);
            txtRazonSocial.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e) { Estado(true); txtRazonSocial.Focus(); }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            Estado(false);
            dgvProveedores_SelectionChanged(null, null);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtRazonSocial, errorProvider, "Obligatorio")
                    & Ui.Requerido(txtRfc, errorProvider, "Obligatorio");
            if (ok && txtRfc.Text.Trim().Length < 12) { errorProvider.SetError(txtRfc, "El RFC tiene 12 o 13 caracteres"); ok = false; }
            if (!ok) return;
            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                ProveedorRepository.Guardar(idActual, txtRazonSocial.Text, txtRfc.Text, txtCorreo.Text.Trim(), txtTelefono.Text.Trim(), chkActivo.Checked);
                Ui.Info(nuevo ? "Proveedor registrado." : "Cambios guardados.");
                Estado(false);
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); btnGuardar.Enabled = true; }
        }
    }
}
