using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulos 3, 4 y 5 · Catálogos (Categorías, Marcas, Tipos de servicio):
    /// alta, edición, búsqueda y eliminación controlada. Una misma pantalla
    /// sirve para los tres; cambia el catálogo que recibe al abrirse.
    /// Solo el Administrador modifica; los demás perfiles consultan.
    /// </summary>
    public partial class FrmCatalogo : Form
    {
        public TipoCatalogo Tipo { get; }
        private int? idActual;
        private bool editando;

        // Constructor sin parámetros para el Diseñador de Visual Studio
        public FrmCatalogo() : this(TipoCatalogo.Categoria) { }

        public FrmCatalogo(TipoCatalogo tipo)
        {
            InitializeComponent();
            Tipo = tipo;
            Ui.Grid(dgvRegistros);
            // Después del formato general: encabezado y formato del segundo campo
            dgvRegistros.DataBindingComplete += (s, e) =>
            {
                if (!dgvRegistros.Columns.Contains("campo2")) return;
                var col = dgvRegistros.Columns["campo2"];
                col.HeaderText = CatalogoCrudRepository.Campo2(Tipo);
                if (Tipo == TipoCatalogo.TipoServicio) { col.DefaultCellStyle.Format = "C2"; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
            };
            Text = CatalogoCrudRepository.Titulo(tipo);
            lblCampo2.Text = CatalogoCrudRepository.Campo2(tipo) + ":";
            bool esPrecio = tipo == TipoCatalogo.TipoServicio;
            txtCampo2.Visible = !esPrecio;
            nudPrecio.Visible = esPrecio;
            txtCampo2.MaxLength = tipo == TipoCatalogo.Categoria ? 200 : 60;
        }

        private void FrmCatalogo_Load(object sender, EventArgs e)
        {
            bool puede = Db.Sesion.Puede(Capacidad.EditarCatalogos);
            lblPermiso.Text = puede ? "" : "Solo lectura";
            btnNuevo.Visible = btnEditar.Visible = btnEliminar.Visible = btnGuardar.Visible = btnCancelar.Visible = puede;
            grpDatos.Visible = puede;
            pnlDerecho.Visible = puede;   // sin permiso, la lista ocupa todo el ancho
            Buscar();
            Estado(false);
        }

        private void Buscar()
        {
            try
            {
                dgvRegistros.DataSource = CatalogoCrudRepository.Listar(Tipo, txtBuscar.Text);
                lblConteo.Text = dgvRegistros.Rows.Count + " registro(s)";
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Estado(bool edicion)
        {
            editando = edicion;
            bool admin = Db.Sesion.Puede(Capacidad.EditarCatalogos);
            grpDatos.Enabled = edicion;
            btnNuevo.Enabled = !edicion && admin;
            btnEditar.Enabled = !edicion && admin && idActual != null;
            btnEliminar.Enabled = !edicion && admin && idActual != null;
            btnGuardar.Enabled = edicion;
            btnCancelar.Enabled = edicion;
            dgvRegistros.Enabled = !edicion;
            grpDatos.Text = !edicion ? "Datos (lectura)" : idActual == null ? "Nuevo registro" : "Editando registro";
        }

        private void dgvRegistros_SelectionChanged(object sender, EventArgs e)
        {
            if (editando || dgvRegistros.CurrentRow == null) return;
            var r = dgvRegistros.CurrentRow;
            idActual = (int)r.Cells["id"].Value;
            txtNombre.Text = Convert.ToString(r.Cells["nombre"].Value);
            if (Tipo == TipoCatalogo.TipoServicio) nudPrecio.Value = Convert.ToDecimal(r.Cells["campo2"].Value);
            else txtCampo2.Text = Convert.ToString(r.Cells["campo2"].Value);
            Estado(false);
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) Buscar(); }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtNombre.Clear(); txtCampo2.Clear(); nudPrecio.Value = 0;
            Estado(true);
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e) { Estado(true); txtNombre.Focus(); }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear(); txtNombre.BackColor = SystemColors.Window;
            Estado(false);
            dgvRegistros_SelectionChanged(null, null);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(txtNombre, errorProvider, "El nombre es obligatorio")) return;
            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                CatalogoCrudRepository.Guardar(Tipo, idActual, txtNombre.Text, txtCampo2.Text, nudPrecio.Value);
                Ui.Info(nuevo ? "Registro agregado." : "Cambios guardados.");
                Estado(false);
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); btnGuardar.Enabled = true; }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == null) return;
            if (!Ui.Confirmar("¿Eliminar \"" + txtNombre.Text + "\"?\nSi está en uso, SQL Server no lo permitirá.")) return;
            try
            {
                CatalogoCrudRepository.Eliminar(Tipo, idActual.Value);
                idActual = null;
                Buscar();
                Estado(false);
            }
            catch (Exception ex) { Ui.Error(ex, "No se eliminó"); }
        }
    }
}
