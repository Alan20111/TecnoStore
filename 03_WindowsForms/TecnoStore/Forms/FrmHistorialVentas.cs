using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulos 11 y 12 · Historial con filtros (texto, estado y fechas) sobre
    /// vw_VentaCompleta. Al abrir una venta se ve qué se vendió y, si el perfil
    /// puede, se cancela con usp_CancelarVenta.
    /// </summary>
    public partial class FrmHistorialVentas : Form, IRefrescable
    {
        private readonly Segmentos segEstado = new Segmentos("Todas", "Completadas", "Canceladas");
        private readonly Panel barraFechas;
        private readonly Ficha ficha;
        private readonly Label lblDatos = new Label { Dock = DockStyle.Top, Height = 64, ForeColor = Tema.TextoSuave };

        public FrmHistorialVentas()
        {
            InitializeComponent();
            Ui.Grid(dgvVentas);
            Ui.Grid(dgvDetalle);
            Ui.Mostrar(dgvVentas, "folio", "fecha", "cliente", "total", "estado");
            Ui.Mostrar(dgvDetalle, "nombre=Producto", "numero_serie", "cantidad", "subtotal=Importe", "garantia_hasta");
            dtpDesde.Value = DateTime.Today.AddMonths(-6);
            dtpHasta.Value = DateTime.Today;

            var btnFechas = Sencillo.Boton("Fechas", () => barraFechas.Visible = !barraFechas.Visible);
            var btnVer = Sencillo.Boton("Ver venta", null, "btnVer");
            Sencillo.Vaciar(this);
            Sencillo.Pista(txtBuscar, "Folio o cliente…");
            Sencillo.BuscarAlEscribir(txtBuscar, Buscar);
            segEstado.Elegido += i => Buscar();
            Sencillo.Barra(pnlFiltros, new Control[] { txtBuscar, segEstado }, btnVer, btnFechas);
            // Divulgación progresiva: las fechas aparecen solo si se piden
            barraFechas = Sencillo.Barra(new Panel(), new Control[] { Etiqueta("Desde"), dtpDesde, Etiqueta("hasta"), dtpHasta });
            barraFechas.Visible = false;
            dtpDesde.ValueChanged += (s, e) => Buscar();
            dtpHasta.ValueChanged += (s, e) => Buscar();
            Sencillo.Armar(this, dgvVentas, pnlFiltros, barraFechas);
            Sencillo.Abrir(dgvVentas, Abrir, btnVer);

            var cuerpo = new Panel();
            cuerpo.Controls.Add(Sencillo.EnTarjeta(dgvDetalle));
            cuerpo.Controls.Add(lblDatos);
            var cerrar = new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel };
            btnCancelarVenta.Text = "Cancelar venta";
            ficha = new Ficha(920, 560, cuerpo, null, cerrar, Db.Sesion.Puede(Capacidad.CancelarVenta) ? btnCancelarVenta : null);
            Disposed += (s, e) => ficha.Dispose();
        }

        private static Label Etiqueta(string t) => new Label { Text = t, AutoSize = true, ForeColor = Tema.TextoSuave };

        private void FrmHistorialVentas_Load(object sender, EventArgs e) => Buscar();
        public void Refrescar() => Buscar();

        private void Buscar()
        {
            if (dtpHasta.Value.Date < dtpDesde.Value.Date) return;   // todavía está eligiendo el periodo
            try
            {
                string estado = segEstado.Activo == 1 ? "Completada" : segEstado.Activo == 2 ? "Cancelada" : null;
                dgvVentas.DataSource = VentaRepository.Buscar(dtpDesde.Value, dtpHasta.Value, estado, txtBuscar.Text);
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            var r = dgvVentas.CurrentRow;
            try { dgvDetalle.DataSource = VentaRepository.Detalle((int)r.Cells["id_venta"].Value); }
            catch (Exception ex) { Ui.Error(ex); return; }
            decimal desc = Convert.ToDecimal(r.Cells["descuento"].Value);
            lblDatos.Text = r.Cells["cliente"].Value + "   ·   " + Convert.ToDateTime(r.Cells["fecha"].Value).ToString("dd/MM/yyyy HH:mm") +
                            "\nAtendió " + r.Cells["vendedor"].Value + "   ·   " + r.Cells["metodo_pago"].Value +
                            (desc > 0 ? "   ·   Descuento " + desc.ToString("C") : "");
            bool cancelada = (string)r.Cells["estado"].Value == "Cancelada";
            btnCancelarVenta.Visible = !cancelada;
            ficha.Abrir(this, r.Cells["folio"].Value + "   " + Convert.ToDecimal(r.Cells["total"].Value).ToString("C") + (cancelada ? "   (cancelada)" : ""));
        }

        private void dgvVentas_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var celda = dgvVentas.Rows[e.RowIndex].Cells["estado"];
            celda.Style.ForeColor = Convert.ToString(celda.Value) == "Cancelada" ? Tema.Rojo : Tema.Verde;
        }

        private void btnCancelarVenta_Click(object sender, EventArgs e)
        {
            var r = dgvVentas.CurrentRow;
            string folio = (string)r.Cells["folio"].Value;
            string motivo = Sencillo.Preguntar(ficha, "Cancelar " + folio, "¿Por qué se cancela?", "Continuar");
            if (motivo == null) return;
            if (!Ui.Confirmar("¿Cancelar la venta " + folio + " por " + Convert.ToDecimal(r.Cells["total"].Value).ToString("C") +
                              "?\nLos productos regresan al almacén.")) return;
            try
            {
                VentaRepository.Cancelar((int)r.Cells["id_venta"].Value, motivo);
                ficha.Listo();
                Ui.Aviso("Venta " + folio + " cancelada");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se pudo cancelar"); }
        }
    }
}
