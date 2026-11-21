using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 9 · Proceso principal (maestro-detalle): nueva venta.
    /// Se guarda con usp_RegistrarVenta en una sola transacción; si SQL Server
    /// rechaza algo (stock, descuento, cliente inactivo) no se guarda nada.
    /// </summary>
    public partial class FrmVenta : Form
    {
        private readonly BindingList<LineaVenta> lineas = new BindingList<LineaVenta>();

        public FrmVenta()
        {
            InitializeComponent();
            Ui.Grid(dgvDetalle);
            Ui.Mostrar(dgvDetalle, "Producto", "Cantidad", "Precio", "Importe");
            dgvDetalle.AutoGenerateColumns = true;
            dgvDetalle.DataSource = lineas;
            lineas.ListChanged += (s, e) => Totales();
        }

        private void FrmVenta_Load(object sender, EventArgs e)
        {
            try
            {
                cboCliente.DataSource = CatalogoRepository.ClientesActivos();
                cboCliente.SelectedIndex = -1;
                cboMetodo.DataSource = CatalogoRepository.MetodosPago();
                txtVendedor.Text = Db.Sesion.Nombre;
                CargarProductos();
                lblReglaDescuento.Text = Db.Sesion.EsSupervisorOMas
                    ? "Como " + Db.Sesion.Rol + " puedes autorizar descuentos de hasta 50 %."
                    : "Descuentos mayores a 10 % requieren a un Supervisor.";
                Totales();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void CargarProductos()
        {
            cboProducto.DataSource = ProductoRepository.ParaVenta();
        }

        private void Totales()
        {
            decimal subtotal = lineas.Sum(l => l.Importe);
            decimal descuento = Math.Round(subtotal * nudDescuento.Value / 100m, 2);
            lblSubtotal.Text = subtotal.ToString("C");
            lblDescuento.Text = descuento.ToString("C");
            lblTotal.Text = (subtotal - descuento).ToString("C");
            btnGuardar.Enabled = lineas.Count > 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var p = cboProducto.SelectedItem as ProductoVenta;
            if (p == null) return;
            int cantidad = (int)nudCantidad.Value;
            var existente = lineas.FirstOrDefault(l => l.IdProducto == p.Id);
            int total = cantidad + (existente == null ? 0 : existente.Cantidad);
            if (total > p.Stock)
            {
                // Validación rápida en la app; SQL Server vuelve a validar al guardar.
                Ui.Info("Solo hay " + p.Stock + " pieza(s) de " + p.Nombre + ".", "Stock insuficiente");
                return;
            }
            if (existente != null) { existente.Cantidad = total; lineas.ResetBindings(); }
            else lineas.Add(new LineaVenta { IdProducto = p.Id, Sku = p.Sku, Producto = p.Nombre, Cantidad = cantidad, Precio = p.Precio });
            nudCantidad.Value = 1;
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow?.DataBoundItem is LineaVenta l) lineas.Remove(l);
        }

        private void nudDescuento_ValueChanged(object sender, EventArgs e) => Totales();

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (lineas.Count > 0 && !Ui.Confirmar("¿Descartar la venta en captura?")) return;
            lineas.Clear();
            cboCliente.SelectedIndex = -1;
            nudDescuento.Value = 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboCliente, errorProvider, "Selecciona un cliente")) return;
            if (lineas.Count == 0) { Ui.Info("Agrega al menos un producto."); return; }
            if (!Ui.Confirmar("¿Registrar la venta por " + lblTotal.Text + "?", "Confirmar venta")) return;

            btnGuardar.Enabled = false;   // evita doble ejecución
            Cursor = Cursors.WaitCursor;
            try
            {
                string folio = VentaRepository.Registrar((int)cboCliente.SelectedValue, (int)cboMetodo.SelectedValue, nudDescuento.Value, lineas);
                Ui.Info("Venta " + folio + " registrada correctamente.", "Venta registrada");
                lineas.Clear();
                cboCliente.SelectedIndex = -1;
                nudDescuento.Value = 0;
                CargarProductos();   // refresca existencias
            }
            catch (Exception ex)
            {
                Ui.Error(ex, "La venta no se guardó (ROLLBACK)");
                btnGuardar.Enabled = true;
            }
            finally { Cursor = Cursors.Default; }
        }
    }
}
