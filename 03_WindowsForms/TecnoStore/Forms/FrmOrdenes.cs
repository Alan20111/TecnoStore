using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 10 · Proceso secundario (maestro-detalle): órdenes del taller.
    /// Alta: usp_RegistrarOrdenServicio · Líneas: usp_OrdenAgregarLinea ·
    /// Estados: usp_CambiarEstadoOrden. La orden solo avanza un paso a la vez,
    /// así que basta un botón: "Pasar a …".
    /// </summary>
    public partial class FrmOrdenes : Form, IRefrescable
    {
        private const int Entregada = 5, Cancelada = 6;
        private readonly bool puede = Db.Sesion.Puede(Capacidad.GestionarOrdenes);
        private readonly Segmentos segVer = new Segmentos("Abiertas", "Todas");
        private readonly Segmentos segDetalle = new Segmentos("Trabajos", "Historial");
        private readonly Ficha fichaNueva, fichaOrden;
        private readonly Button btnSiguiente, btnCancelarOrden;
        private readonly Label lblDatos = new Label { Dock = DockStyle.Top, Height = 60, ForeColor = Tema.TextoSuave, AutoEllipsis = true };
        private readonly PanelCampos acciones;
        private int idOrden, idEstado;

        public FrmOrdenes()
        {
            InitializeComponent();
            Ui.Grid(dgvOrdenes);
            Ui.Grid(dgvDetalle);
            Ui.Grid(dgvHistorial);
            Ui.Mostrar(dgvOrdenes, "folio", "equipo_descripcion", "cliente", "estado", "dias_en_taller");
            Ui.Mostrar(dgvDetalle, "concepto", "cantidad", "subtotal=Importe");
            Ui.Mostrar(dgvHistorial, "fecha", "a_estado=Pasó a");

            var btnRecibir = Sencillo.Boton("Recibir equipo", Recibir, "btnRecibir");
            var btnAbrir = Sencillo.Boton("Abrir", null, "btnAbrir");
            Sencillo.Vaciar(this);
            segVer.Elegido += i => Refrescar();
            Sencillo.Barra(pnlArriba, new Control[] { segVer }, puede ? new[] { btnRecibir, btnAbrir } : new[] { btnAbrir });
            Sencillo.Armar(this, dgvOrdenes, pnlArriba);
            Sencillo.Abrir(dgvOrdenes, Abrir, btnAbrir);

            // Ficha 1: recibir un equipo
            btnRegistrar.Text = "Recibir equipo";
            fichaNueva = new Ficha(640, 560, new PanelCampos(2, new Campo("Cliente *", cboCliente, true),
                new Campo("Equipo *", txtEquipo), new Campo("No. de serie", txtSerie),
                new Campo("¿Qué le pasa? *", txtFalla, true, 96), new Campo("Técnico *", cboTecnico, true)),
                btnRegistrar, new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel });

            // Ficha 2: una orden (lo que se le ha hecho y el siguiente paso)
            btnSiguiente = Sencillo.Boton("Pasar a En diagnóstico", () => CambiarEstado(idEstado + 1), "btnSiguiente");
            btnCancelarOrden = Sencillo.Boton("Cancelar orden", () =>
            {
                if (Ui.Confirmar("¿Cancelar la orden? Las refacciones regresan al almacén.")) CambiarEstado(Cancelada);
            });
            btnAgregarServicio.Text = "Agregar servicio";
            btnAgregarRefaccion.Text = "Agregar refacción";
            acciones = new PanelCampos(1, new Campo("Servicio", cboServicio), new Campo(null, btnAgregarServicio),
                new Campo("Refacción", cboRefaccion), new Campo("Piezas", nudCantidad), new Campo(null, btnAgregarRefaccion));
            dgvHistorial.Visible = false;
            segDetalle.Elegido += i => { dgvDetalle.Visible = i == 0; dgvHistorial.Visible = i == 1; };
            var tablas = new Panel();
            tablas.Controls.Add(dgvDetalle); tablas.Controls.Add(dgvHistorial);
            var zona = new Panel();
            zona.Controls.Add(Sencillo.EnTarjeta(tablas));
            zona.Controls.Add(Sencillo.Barra(new Panel(), new Control[] { segDetalle }));
            zona.Controls.Add(lblDatos);
            fichaOrden = new Ficha(1000, 640, puede ? Sencillo.DosZonas(acciones, 320, zona) : zona, puede ? btnSiguiente : null,
                new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel }, puede ? btnCancelarOrden : null);
            Disposed += (s, e) => { fichaNueva.Dispose(); fichaOrden.Dispose(); };
        }

        private void FrmOrdenes_Load(object sender, EventArgs e)
        {
            try
            {
                cboTecnico.DataSource = CatalogoRepository.Tecnicos();
                cboServicio.DataSource = CatalogoRepository.TiposServicio();
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        public void Refrescar()
        {
            try { dgvOrdenes.DataSource = OrdenRepository.Buscar(segVer.Activo == 0); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void dgvOrdenes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var celda = dgvOrdenes.Rows[e.RowIndex].Cells["estado"];
            string estado = Convert.ToString(celda.Value);
            celda.Style.ForeColor = estado == "Cancelada" ? Tema.Rojo : estado == "Lista" ? Tema.Verde : Tema.Navegacion;
        }

        /// <summary>Abre la ficha para recibir un equipo (también desde el Inicio).</summary>
        public void Recibir()
        {
            if (!puede) return;
            try { cboCliente.DataSource = CatalogoRepository.ClientesActivos(); } catch (Exception ex) { Ui.Error(ex); return; }
            cboCliente.SelectedIndex = -1;
            txtEquipo.Clear(); txtFalla.Clear(); txtSerie.Clear();
            Ui.SinErrores(errorProvider, cboCliente, cboTecnico, txtEquipo, txtFalla);
            fichaNueva.Abrir(this, "Recibir equipo");
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(cboCliente, errorProvider, "Selecciona el cliente")
                    & Ui.Requerido(cboTecnico, errorProvider, "Selecciona el técnico")
                    & Ui.Requerido(txtEquipo, errorProvider, "Escribe qué equipo es")
                    & Ui.Requerido(txtFalla, errorProvider, "Escribe qué le pasa");
            if (!ok) return;
            btnRegistrar.Enabled = false;
            try
            {
                string folio = OrdenRepository.Registrar((int)cboCliente.SelectedValue, (int)cboTecnico.SelectedValue,
                                                         txtEquipo.Text.Trim(), txtFalla.Text.Trim(), txtSerie.Text);
                fichaNueva.Listo();
                Ui.Aviso("Orden " + folio + " registrada");
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se registró la orden"); }
            finally { btnRegistrar.Enabled = true; }
        }

        private void Abrir()
        {
            idOrden = (int)dgvOrdenes.CurrentRow.Cells["id_orden"].Value;
            string folio = Convert.ToString(dgvOrdenes.CurrentRow.Cells["folio"].Value);
            try
            {
                cboRefaccion.DataSource = CatalogoRepository.Refacciones();
                LeerOrden();
            }
            catch (Exception ex) { Ui.Error(ex); return; }
            fichaOrden.Abrir(this, folio);
        }

        private void LeerOrden()
        {
            dgvDetalle.DataSource = OrdenRepository.Detalle(idOrden);
            dgvHistorial.DataSource = OrdenRepository.Historial(idOrden);
            string estado = null, texto = null;
            foreach (DataGridViewRow r in dgvOrdenes.Rows)
                if ((int)r.Cells["id_orden"].Value == idOrden)
                {
                    estado = Convert.ToString(r.Cells["estado"].Value);
                    texto = r.Cells["equipo_descripcion"].Value + "   ·   " + r.Cells["cliente"].Value +
                            "\n" + estado + "   ·   " + Convert.ToDecimal(r.Cells["total"].Value).ToString("C");
                }
            if (estado == null) return;   // salió de la lista de abiertas
            lblDatos.Text = texto;
            var estados = CatalogoRepository.EstadosOrden();
            idEstado = estados.Find(x => x.Texto == estado).Id;
            bool cerrada = idEstado >= Entregada;
            acciones.Enabled = !cerrada;
            btnSiguiente.Visible = btnCancelarOrden.Visible = !cerrada;
            if (!cerrada) btnSiguiente.Text = idEstado + 1 == Entregada ? "Entregar al cliente" : "Pasar a " + estados.Find(x => x.Id == idEstado + 1).Texto;
        }

        private void CambiarEstado(int nuevo)
        {
            try
            {
                OrdenRepository.CambiarEstado(idOrden, nuevo);
                Refrescar();
                if (nuevo >= Entregada) { fichaOrden.Listo(); Ui.Aviso(nuevo == Entregada ? "Equipo entregado" : "Orden cancelada"); }
                else LeerOrden();
            }
            catch (Exception ex) { Ui.Error(ex, "No se cambió el estado"); }
        }

        private void AgregarLinea(int? servicio, int? refaccion, int cantidad)
        {
            try
            {
                OrdenRepository.AgregarLinea(idOrden, servicio, refaccion, cantidad);
                if (refaccion != null) { cboRefaccion.DataSource = CatalogoRepository.Refacciones(); nudCantidad.Value = 1; }   // stock actualizado
                Refrescar();
                LeerOrden();
            }
            catch (Exception ex) { Ui.Error(ex, "No se agregó"); }
        }

        private void btnAgregarServicio_Click(object sender, EventArgs e)
        {
            if (cboServicio.SelectedValue != null) AgregarLinea((int)cboServicio.SelectedValue, null, 1);
        }

        private void btnAgregarRefaccion_Click(object sender, EventArgs e)
        {
            if (cboRefaccion.SelectedValue != null) AgregarLinea(null, (int)cboRefaccion.SelectedValue, (int)nudCantidad.Value);
        }
    }
}
