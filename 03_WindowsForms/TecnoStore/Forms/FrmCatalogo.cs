using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulos 3, 4 y 5 · Catálogos (Categorías, Marcas, Tipos de servicio): una
    /// misma pantalla sirve para los tres. INSERT, UPDATE y DELETE parametrizados;
    /// lo que está en uso no se puede eliminar.
    /// </summary>
    public partial class FrmCatalogo : Form, IRefrescable
    {
        public TipoCatalogo Tipo { get; }
        private int? idActual;
        private readonly Ficha ficha;

        // Constructor sin parámetros para el Diseñador de Visual Studio
        public FrmCatalogo() : this(TipoCatalogo.Categoria) { }

        public FrmCatalogo(TipoCatalogo tipo)
        {
            InitializeComponent();
            Tipo = tipo;
            bool puede = Db.Sesion.Puede(Capacidad.EditarCatalogos), esPrecio = tipo == TipoCatalogo.TipoServicio;
            Ui.Grid(dgvRegistros);
            Ui.Mostrar(dgvRegistros, "nombre", "campo2");
            dgvRegistros.DataBindingComplete += (s, e) =>   // encabezado y formato del segundo campo
            {
                if (!dgvRegistros.Columns.Contains("campo2")) return;
                var col = dgvRegistros.Columns["campo2"];
                col.HeaderText = CatalogoCrudRepository.Campo2(Tipo);
                if (esPrecio) { col.DefaultCellStyle.Format = "C2"; col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
            };
            Text = CatalogoCrudRepository.Titulo(tipo);
            txtCampo2.MaxLength = tipo == TipoCatalogo.Categoria ? 200 : 60;

            string uno = tipo == TipoCatalogo.Categoria ? "Nueva categoría" : tipo == TipoCatalogo.Marca ? "Nueva marca" : "Nuevo servicio";
            btnNuevo.Text = uno;
            btnEditar.Text = "Abrir";
            btnCancelar.Text = puede ? "Cancelar" : "Cerrar";
            Sencillo.Vaciar(this);
            Sencillo.Pista(txtBuscar, "Buscar…");
            Sencillo.BuscarAlEscribir(txtBuscar, Buscar);
            Sencillo.Barra(pnlFiltros, new Control[] { txtBuscar }, puede ? new[] { btnNuevo, btnEditar } : new[] { btnEditar });
            Sencillo.Armar(this, dgvRegistros, pnlFiltros);
            Sencillo.Abrir(dgvRegistros, Abrir, btnEditar);

            var campos = new PanelCampos(1, new Campo("Nombre *", txtNombre),
                new Campo(CatalogoCrudRepository.Campo2(tipo), esPrecio ? (Control)nudPrecio : txtCampo2)) { Enabled = puede };
            ficha = new Ficha(560, 350, campos, puede ? btnGuardar : null, btnCancelar, puede ? btnEliminar : null);
            Disposed += (s, e) => ficha.Dispose();
        }

        private void FrmCatalogo_Load(object sender, EventArgs e) => Buscar();
        public void Refrescar() => Buscar();

        private void Buscar()
        {
            try { dgvRegistros.DataSource = CatalogoCrudRepository.Listar(Tipo, txtBuscar.Text); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            var r = dgvRegistros.CurrentRow;
            idActual = (int)r.Cells["id"].Value;
            txtNombre.Text = Convert.ToString(r.Cells["nombre"].Value);
            if (Tipo == TipoCatalogo.TipoServicio) nudPrecio.Value = Convert.ToDecimal(r.Cells["campo2"].Value);
            else txtCampo2.Text = Convert.ToString(r.Cells["campo2"].Value);
            MostrarFicha(txtNombre.Text);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtNombre.Clear(); txtCampo2.Clear(); nudPrecio.Value = 0;
            MostrarFicha(btnNuevo.Text);
        }

        private void MostrarFicha(string titulo)
        {
            Ui.SinErrores(errorProvider, txtNombre);
            btnEliminar.Visible = idActual != null;
            ficha.Abrir(this, titulo);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(txtNombre, errorProvider, "Falta el nombre")) return;
            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                CatalogoCrudRepository.Guardar(Tipo, idActual, txtNombre.Text, txtCampo2.Text, nudPrecio.Value);
                ficha.Listo();
                Ui.Aviso(nuevo ? "Registro agregado" : "Cambios guardados");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); }
            finally { btnGuardar.Enabled = true; }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == null) return;
            if (!Ui.Confirmar("¿Eliminar \"" + txtNombre.Text + "\"?\nSi está en uso no se podrá eliminar.")) return;
            try
            {
                CatalogoCrudRepository.Eliminar(Tipo, idActual.Value);
                ficha.Listo();
                Ui.Aviso("Registro eliminado");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se eliminó"); }
        }
    }
}
