using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Tercer proceso maestro-detalle · Compra a proveedor (entrada de mercancía).
    /// Se guarda con usp_RegistrarCompra: encabezado, detalle, entrada al stock,
    /// piezas con número de serie y relación ProductoProveedor en una transacción.
    /// </summary>
    public partial class FrmCompra : Form
    {
        private readonly BindingList<LineaCompra> lineas = new BindingList<LineaCompra>();

        public FrmCompra()
        {
            InitializeComponent();
            Ui.Grid(dgvDetalle);
            Ui.Grid(dgvRecientes);
            dgvDetalle.DataSource = lineas;
            lineas.ListChanged += (s, e) =>
            {
                lblTotal.Text = lineas.Sum(l => l.Importe).ToString("C");
                btnGuardar.Enabled = lineas.Count > 0;
            };
        }

        private void FrmCompra_Load(object sender, EventArgs e)
        {
            try
            {
                cboProveedor.DataSource = ProveedorRepository.Activos();
                cboProveedor.SelectedIndex = -1;
                cboProducto.DataSource = CompraRepository.Productos();
                txtRecibe.Text = Db.Sesion.Nombre;
                dgvRecientes.DataSource = CompraRepository.Recientes();
                lblTotal.Text = 0m.ToString("C");
                btnGuardar.Enabled = false;
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboProducto.SelectedItem is ProductoCompra p)) return;
            nudCosto.Value = Math.Min(nudCosto.Maximum, p.Costo);
            txtSeries.Enabled = p.RequiereSerie;
            lblSeries.Text = p.RequiereSerie ? "Números de serie (uno por renglón): *" : "Números de serie (no aplica):";
            if (!p.RequiereSerie) txtSeries.Clear();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!(cboProducto.SelectedItem is ProductoCompra p)) return;
            if (lineas.Any(l => l.IdProducto == p.Id)) { Ui.Info("Ese producto ya está en la compra; quítalo y vuelve a agregarlo con la cantidad total."); return; }
            int cantidad = (int)nudCantidad.Value;
            var series = txtSeries.Lines.Select(x => x.Trim().ToUpperInvariant()).Where(x => x.Length > 0).ToList();
            if (p.RequiereSerie)
            {
                if (series.Count != cantidad) { Ui.Info("Captura " + cantidad + " número(s) de serie, uno por renglón (llevas " + series.Count + ")."); return; }
                if (series.Distinct().Count() != series.Count) { Ui.Info("Hay números de serie repetidos."); return; }
            }
            lineas.Add(new LineaCompra
            {
                IdProducto = p.Id, Sku = p.Sku, Producto = p.Nombre, Cantidad = cantidad, Costo = nudCosto.Value,
                Series = string.Join(", ", series), ListaSeries = series,
            });
            nudCantidad.Value = 1; txtSeries.Clear();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow?.DataBoundItem is LineaCompra l) lineas.Remove(l);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (lineas.Count > 0 && !Ui.Confirmar("¿Descartar la compra en captura?")) return;
            lineas.Clear(); cboProveedor.SelectedIndex = -1;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboProveedor, errorProvider, "Selecciona el proveedor")) return;
            if (!Ui.Confirmar("¿Registrar la compra por " + lblTotal.Text + "?\nEl stock entra al guardar.", "Confirmar compra")) return;
            btnGuardar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                string folio = CompraRepository.Registrar((int)cboProveedor.SelectedValue, lineas);
                Ui.Info("Compra " + folio + " registrada; el inventario ya se actualizó.", "Compra registrada");
                lineas.Clear(); cboProveedor.SelectedIndex = -1;
                cboProducto.DataSource = CompraRepository.Productos();
                dgvRecientes.DataSource = CompraRepository.Recientes();
            }
            catch (Exception ex) { Ui.Error(ex, "La compra no se guardó (ROLLBACK)"); btnGuardar.Enabled = true; }
            finally { Cursor = Cursors.Default; }
        }
    }
}
