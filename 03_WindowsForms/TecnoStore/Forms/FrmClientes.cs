using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 6 · Clientes: se busca al escribir; doble clic abre la ficha con
    /// sus datos y sus compras. Total y nivel se calculan en SQL Server con
    /// fn_TotalCompradoCliente y fn_ClasificacionCliente. Baja lógica: no se borra.
    /// </summary>
    public partial class FrmClientes : Form, IRefrescable
    {
        private int? idActual;
        private readonly Ficha ficha;
        private readonly PanelCampos campos;
        private readonly Panel zonaCompras;

        public FrmClientes()
        {
            InitializeComponent();
            bool puede = Db.Sesion.Puede(Capacidad.GuardarCliente);
            Ui.Grid(dgvClientes);
            Ui.Grid(dgvHistorial);
            Ui.Mostrar(dgvClientes, "nombre", "apellidos", "telefono", "total_comprado", "nivel");
            Ui.Mostrar(dgvHistorial, "folio", "fecha", "total", "estado");

            btnNuevo.Text = "Nuevo cliente";
            btnEditar.Text = "Abrir";
            btnCancelar.Text = puede ? "Cancelar" : "Cerrar";
            Sencillo.Vaciar(this);
            Sencillo.Pista(txtBuscar, "Buscar cliente…");
            Sencillo.BuscarAlEscribir(txtBuscar, Buscar);
            Sencillo.Barra(pnlFiltros, new Control[] { txtBuscar }, puede ? new[] { btnNuevo, btnEditar } : new[] { btnEditar });
            Sencillo.Armar(this, dgvClientes, pnlFiltros);
            Sencillo.Abrir(dgvClientes, Abrir, btnEditar);

            campos = new PanelCampos(1,
                new Campo("Nombre *", txtNombre), new Campo("Apellidos *", txtApellidos), new Campo("Correo *", txtCorreo),
                new Campo("Teléfono", txtTelefono), new Campo("RFC", txtRfc), new Campo(null, chkActivo)) { Enabled = puede };
            chkActivo.Text = "Cliente activo";
            zonaCompras = Sencillo.EnTarjeta(dgvHistorial, "Sus compras");
            ficha = new Ficha(980, 640, Sencillo.DosZonas(campos, 360, zonaCompras), puede ? btnGuardar : null, btnCancelar);
            Disposed += (s, e) => ficha.Dispose();
        }

        private void FrmClientes_Load(object sender, EventArgs e) => Buscar();
        public void Refrescar() => Buscar();

        private void Buscar()
        {
            try { dgvClientes.DataSource = ClienteRepository.Buscar(txtBuscar.Text); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            var r = dgvClientes.CurrentRow;
            idActual = (int)r.Cells["id_cliente"].Value;
            txtNombre.Text = Convert.ToString(r.Cells["nombre"].Value);
            txtApellidos.Text = Convert.ToString(r.Cells["apellidos"].Value);
            txtCorreo.Text = Convert.ToString(r.Cells["correo"].Value);
            txtTelefono.Text = Convert.ToString(r.Cells["telefono"].Value);
            txtRfc.Text = Convert.ToString(r.Cells["rfc"].Value);
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            try { dgvHistorial.DataSource = ClienteRepository.Historial(idActual.Value); }
            catch (Exception ex) { Ui.Error(ex); return; }
            MostrarFicha(txtNombre.Text + " " + txtApellidos.Text);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtNombre.Clear(); txtApellidos.Clear(); txtCorreo.Clear(); txtTelefono.Clear(); txtRfc.Clear();
            chkActivo.Checked = true;
            MostrarFicha("Nuevo cliente");
        }

        private void MostrarFicha(string titulo)
        {
            Ui.SinErrores(errorProvider, txtNombre, txtApellidos, txtCorreo);
            campos.Mostrar(chkActivo, idActual != null);
            zonaCompras.Visible = idActual != null;   // al dar de alta no hay compras
            ficha.Abrir(this, titulo);
        }

        private void dgvClientes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvClientes.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.ForeColor = Tema.Ceniza;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtNombre, errorProvider, "Falta el nombre")
                    & Ui.Requerido(txtApellidos, errorProvider, "Faltan los apellidos")
                    & Ui.Requerido(txtCorreo, errorProvider, "Falta el correo");
            if (ok && !txtCorreo.Text.Contains("@")) { errorProvider.SetError(txtCorreo, "Ese correo no es válido"); ok = false; }
            if (!ok) return;

            btnGuardar.Enabled = false;   // evita doble ejecución
            try
            {
                bool nuevo = idActual == null;
                ClienteRepository.Guardar(idActual, txtNombre.Text.Trim(), txtApellidos.Text.Trim(), txtCorreo.Text,
                                          txtTelefono.Text.Trim(), txtRfc.Text, chkActivo.Checked);
                ficha.Listo();
                Ui.Aviso(nuevo ? "Cliente registrado" : "Cambios guardados");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); }
            finally { btnGuardar.Enabled = true; }
        }
    }
}
