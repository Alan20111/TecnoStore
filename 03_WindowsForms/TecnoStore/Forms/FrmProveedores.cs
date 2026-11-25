using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 7 · Proveedores: lista con buscador; la ficha muestra sus datos y
    /// los productos que surte (tabla N:M ProductoProveedor). usp_ProveedorGuardar.
    /// </summary>
    public partial class FrmProveedores : Form, IRefrescable
    {
        private int? idActual;
        private readonly Ficha ficha;
        private readonly PanelCampos campos;
        private readonly Panel zonaProductos;

        public FrmProveedores()
        {
            InitializeComponent();
            bool puede = Db.Sesion.Puede(Capacidad.GuardarProveedor);
            Ui.Grid(dgvProveedores);
            Ui.Grid(dgvProductos);
            Ui.Mostrar(dgvProveedores, "razon_social", "telefono", "correo", "productos=Productos");
            Ui.Mostrar(dgvProductos, "nombre=Producto", "costo_proveedor", "stock_actual");

            btnNuevo.Text = "Nuevo proveedor";
            btnEditar.Text = "Abrir";
            btnCancelar.Text = puede ? "Cancelar" : "Cerrar";
            Sencillo.Vaciar(this);
            Sencillo.Pista(txtBuscar, "Buscar proveedor…");
            Sencillo.BuscarAlEscribir(txtBuscar, Buscar);
            Sencillo.Barra(pnlFiltros, new Control[] { txtBuscar }, puede ? new[] { btnNuevo, btnEditar } : new[] { btnEditar });
            Sencillo.Armar(this, dgvProveedores, pnlFiltros);
            Sencillo.Abrir(dgvProveedores, Abrir, btnEditar);

            chkActivo.Text = "Proveedor activo";
            campos = new PanelCampos(1, new Campo("Razón social *", txtRazonSocial), new Campo("RFC *", txtRfc),
                new Campo("Correo", txtCorreo), new Campo("Teléfono", txtTelefono), new Campo(null, chkActivo)) { Enabled = puede };
            zonaProductos = Sencillo.EnTarjeta(dgvProductos, "Productos que surte");
            ficha = new Ficha(980, 580, Sencillo.DosZonas(campos, 360, zonaProductos), puede ? btnGuardar : null, btnCancelar);
            Disposed += (s, e) => ficha.Dispose();
        }

        private void FrmProveedores_Load(object sender, EventArgs e) => Buscar();
        public void Refrescar() => Buscar();

        private void Buscar()
        {
            try { dgvProveedores.DataSource = ProveedorRepository.Buscar(txtBuscar.Text); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            var r = dgvProveedores.CurrentRow;
            idActual = (int)r.Cells["id_proveedor"].Value;
            txtRazonSocial.Text = Convert.ToString(r.Cells["razon_social"].Value);
            txtRfc.Text = Convert.ToString(r.Cells["rfc"].Value);
            txtCorreo.Text = Convert.ToString(r.Cells["correo"].Value);
            txtTelefono.Text = Convert.ToString(r.Cells["telefono"].Value);
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            try { dgvProductos.DataSource = ProveedorRepository.ProductosQueSurte(idActual.Value); }
            catch (Exception ex) { Ui.Error(ex); return; }
            MostrarFicha(txtRazonSocial.Text);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtRazonSocial.Clear(); txtRfc.Clear(); txtCorreo.Clear(); txtTelefono.Clear(); chkActivo.Checked = true;
            MostrarFicha("Nuevo proveedor");
        }

        private void MostrarFicha(string titulo)
        {
            Ui.SinErrores(errorProvider, txtRazonSocial, txtRfc);
            campos.Mostrar(chkActivo, idActual != null);
            zonaProductos.Visible = idActual != null;
            ficha.Abrir(this, titulo);
        }

        private void dgvProveedores_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvProveedores.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.ForeColor = Tema.Ceniza;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtRazonSocial, errorProvider, "Falta la razón social")
                    & Ui.Requerido(txtRfc, errorProvider, "Falta el RFC");
            if (ok && txtRfc.Text.Trim().Length < 12) { errorProvider.SetError(txtRfc, "El RFC tiene 12 o 13 caracteres"); ok = false; }
            if (!ok) return;
            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                ProveedorRepository.Guardar(idActual, txtRazonSocial.Text, txtRfc.Text, txtCorreo.Text.Trim(), txtTelefono.Text.Trim(), chkActivo.Checked);
                ficha.Listo();
                Ui.Aviso(nuevo ? "Proveedor registrado" : "Cambios guardados");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); }
            finally { btnGuardar.Enabled = true; }
        }
    }
}
