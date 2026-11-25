using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 9 · Proceso principal (maestro-detalle): vender. A la izquierda se
    /// busca y se toca el producto; a la derecha queda la cuenta y COBRAR.
    /// Se guarda con usp_RegistrarVenta en una sola transacción; si SQL Server
    /// rechaza algo (stock, descuento, cliente inactivo) no se guarda nada.
    /// </summary>
    public partial class FrmVenta : Form, IRefrescable
    {
        private readonly BindingList<LineaVenta> lineas = new BindingList<LineaVenta>();
        private List<ProductoVenta> productos = new List<ProductoVenta>();

        public FrmVenta()
        {
            InitializeComponent();
            Ui.Grid(dgvProductos);
            Ui.Grid(dgvDetalle);
            Ui.Mostrar(dgvProductos, "nombre=Producto", "precio", "stock");
            Ui.Mostrar(dgvDetalle, "producto", "cantidad=Pzas.", "importe");
            dgvDetalle.DataSource = lineas;
            lineas.ListChanged += (s, e) => Totales();
            EnCaja(txtBuscar, pnlBuscar, 4);
            EnCaja(cboCliente, pnlCliente, 24);
            Sencillo.Pista(txtBuscar, "Escribe o escanea el producto…");
            txtBuscar.TextChanged += (s, e) => Filtrar();
            txtBuscar.KeyDown += txtBuscar_KeyDown;
            dgvProductos.CellClick += (s, e) => { if (e.RowIndex >= 0) Agregar(); };
            dgvProductos.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Agregar(); } };
            lblTotal.Font = Tema.FuenteTotal;
            Tema.Peligro(btnVaciar);
        }

        private static void EnCaja(Control campo, Panel panel, int y)
        {
            panel.Controls.Remove(campo);
            var caja = new Caja(campo) { Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            caja.SetBounds(0, y, panel.ClientSize.Width, Caja.Alto);
            panel.Controls.Add(caja);
        }

        private void FrmVenta_Load(object sender, EventArgs e) { btnCobrar.Font = Tema.FuenteTitulo; lblTotal.Font = Tema.FuenteTotal; Refrescar(); Totales(); }

        public void Refrescar()
        {
            try
            {
                productos = ProductoRepository.ParaVenta();
                Filtrar();
                if (lineas.Count == 0) { cboCliente.DataSource = CatalogoRepository.ClientesActivos(); cboCliente.SelectedIndex = -1; }
                txtBuscar.Focus();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        /// <summary>Todas las palabras deben aparecer en el nombre o el SKU.</summary>
        private void Filtrar()
        {
            var palabras = txtBuscar.Text.Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            dgvProductos.DataSource = productos.Where(p => palabras.All(w => (p.Nombre + " " + p.Sku).ToLowerInvariant().Contains(w))).ToList();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Con el lector: el SKU exacto se agrega; escribiendo: el primero de la lista
                e.SuppressKeyPress = true;
                var exacto = productos.Find(p => string.Equals(p.Sku, txtBuscar.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                if (exacto != null) { Agregar(exacto); txtBuscar.Clear(); }
                else if (txtBuscar.Text.Trim() != "" && dgvProductos.Rows.Count > 0) { Agregar(); txtBuscar.Clear(); }
            }
            else if (e.KeyCode == Keys.Down && dgvProductos.Rows.Count > 0) { e.SuppressKeyPress = true; dgvProductos.Focus(); }
            else if (e.KeyCode == Keys.Escape && txtBuscar.Text != "") { e.SuppressKeyPress = true; txtBuscar.Clear(); }
        }

        private void Agregar() => Agregar(dgvProductos.CurrentRow?.DataBoundItem as ProductoVenta);

        private void Agregar(ProductoVenta p)
        {
            if (p == null) return;
            var linea = lineas.FirstOrDefault(l => l.IdProducto == p.Id);
            if ((linea?.Cantidad ?? 0) + 1 > p.Stock)
            {
                // Validación rápida en la app; SQL Server vuelve a validar al guardar.
                Ui.Aviso("Solo hay " + p.Stock + " pieza(s) de " + p.Nombre);
                return;
            }
            if (linea == null) lineas.Add(new LineaVenta { IdProducto = p.Id, Sku = p.Sku, Producto = p.Nombre, Cantidad = 1, Precio = p.Precio });
            else { linea.Cantidad++; lineas.ResetBindings(); }
        }

        private LineaVenta Elegida => dgvDetalle.CurrentRow?.DataBoundItem as LineaVenta;

        private void btnMas_Click(object sender, EventArgs e)
        {
            var l = Elegida;
            if (l != null) Agregar(productos.Find(p => p.Id == l.IdProducto));
        }

        private void btnMenos_Click(object sender, EventArgs e)
        {
            var l = Elegida;
            if (l == null) return;
            if (l.Cantidad > 1) { l.Cantidad--; lineas.ResetBindings(); } else lineas.Remove(l);
        }

        private void btnVaciar_Click(object sender, EventArgs e)
        {
            if (lineas.Count > 0 && !Ui.Confirmar("¿Vaciar la venta?")) return;
            lineas.Clear();
            cboCliente.SelectedIndex = -1;
            txtBuscar.Focus();
        }

        private void Totales()
        {
            lblTotal.Text = lineas.Sum(l => l.Importe).ToString("C");
            btnCobrar.Enabled = btnMas.Enabled = btnMenos.Enabled = btnVaciar.Enabled = lineas.Count > 0;
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (lineas.Count == 0) return;
            if (!Ui.Requerido(cboCliente, errorProvider, "Selecciona un cliente")) { cboCliente.Focus(); cboCliente.DroppedDown = true; return; }
            try
            {
                using (var cobro = new FrmCobrar(lineas.Sum(l => l.Importe)))
                {
                    if (cobro.ShowDialog(TopLevelControl as Form) != DialogResult.OK) return;
                    btnCobrar.Enabled = false;   // evita doble ejecución
                    Cursor = Cursors.WaitCursor;
                    string folio = VentaRepository.Registrar((int)cboCliente.SelectedValue, cobro.IdMetodo, cobro.Descuento, lineas);
                    lineas.Clear();
                    Ui.Aviso("Venta " + folio + " registrada");
                    Refrescar();   // existencias al día
                }
            }
            catch (Exception ex) { Ui.Error(ex, "La venta no se guardó"); }
            finally { Cursor = Cursors.Default; Totales(); }
        }
    }
}
