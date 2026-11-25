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
    /// Tercer proceso maestro-detalle · Compra a proveedor: a la izquierda qué
    /// llegó, a la derecha la lista y el total. usp_RegistrarCompra guarda todo
    /// (encabezado, detalle, stock y números de serie) en una transacción.
    /// </summary>
    public partial class FrmCompra : Form, IRefrescable
    {
        private readonly BindingList<LineaCompra> lineas = new BindingList<LineaCompra>();
        private readonly PanelCampos campos;
        private readonly Ficha fichaRecientes;

        public FrmCompra()
        {
            InitializeComponent();
            Ui.Grid(dgvDetalle);
            Ui.Grid(dgvRecientes);
            Ui.Mostrar(dgvRecientes, "folio", "fecha", "proveedor", "total");
            Ui.Mostrar(dgvDetalle, "Producto", "Cantidad", "Importe");
            dgvDetalle.DataSource = lineas;
            lineas.ListChanged += (s, e) => Totales();

            Sencillo.Vaciar(this);
            btnAgregar.Text = "Agregar a la compra";
            btnQuitar.Text = "Quitar";
            btnGuardar.Text = "Guardar compra";
            campos = new PanelCampos(1, new Campo("Proveedor *", cboProveedor), new Campo("Producto", cboProducto),
                new Campo("Piezas", nudCantidad), new Campo("Costo de cada pieza", nudCosto),
                new Campo("Números de serie (uno por renglón)", txtSeries, false, 110), new Campo(null, btnAgregar));
            var izquierda = Sencillo.EnTarjeta(campos, "Qué llegó");
            campos.AutoScroll = true;

            lblTotal.AutoSize = false; lblTotal.Dock = DockStyle.Top; lblTotal.Height = 62;
            lblTotal.Font = Tema.FuenteCifra; lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight; lblTotal.ForeColor = Tema.Navegacion;
            var btnRecientes = Sencillo.Boton("Compras anteriores", VerRecientes);
            var pie = Sencillo.Barra(new Panel(), new Control[] { btnRecientes }, btnGuardar, btnQuitar);
            btnRecientes.Width = 200;
            pie.Dock = DockStyle.Bottom; pie.BackColor = Tema.Tarjeta;
            var derecha = Sencillo.EnTarjeta(dgvDetalle, "Esta compra");
            var abajo = new Panel { Dock = DockStyle.Bottom, Height = 140, BackColor = Tema.Tarjeta };
            abajo.Controls.Add(pie); abajo.Controls.Add(lblTotal);
            derecha.Controls.Add(abajo);
            dgvDetalle.BringToFront();   // que la tabla ocupe lo que sobra
            var zonas = Sencillo.DosZonas(izquierda, 400, derecha);
            zonas.Dock = DockStyle.Fill;
            Controls.Add(zonas);

            fichaRecientes = new Ficha(820, 560, Sencillo.EnTarjeta(dgvRecientes), null, new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel });
            Disposed += (s, e) => fichaRecientes.Dispose();
        }

        private void FrmCompra_Load(object sender, EventArgs e) { Refrescar(); Totales(); }

        public void Refrescar()
        {
            if (lineas.Count > 0) return;   // no perder lo que ya se capturó
            try
            {
                cboProveedor.DataSource = ProveedorRepository.Activos();
                cboProveedor.SelectedIndex = -1;
                cboProducto.DataSource = CompraRepository.Productos();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Totales()
        {
            lblTotal.Text = lineas.Sum(l => l.Importe).ToString("C");
            btnGuardar.Enabled = btnQuitar.Enabled = lineas.Count > 0;
        }

        private void VerRecientes()
        {
            try { dgvRecientes.DataSource = CompraRepository.Recientes(); } catch (Exception ex) { Ui.Error(ex); return; }
            fichaRecientes.Abrir(this, "Compras anteriores");
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboProducto.SelectedItem is ProductoCompra p)) return;
            nudCosto.Value = Math.Max(nudCosto.Minimum, Math.Min(nudCosto.Maximum, p.Costo));
            if (!p.RequiereSerie) txtSeries.Clear();
            campos?.Mostrar(txtSeries, p.RequiereSerie);   // solo se piden cuando hacen falta
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboProveedor, errorProvider, "Selecciona el proveedor")) return;
            if (!Ui.Confirmar("¿Guardar la compra por " + lblTotal.Text + "?\nEl inventario sube al guardar.", "Confirmar compra")) return;
            btnGuardar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                string folio = CompraRepository.Registrar((int)cboProveedor.SelectedValue, lineas);
                lineas.Clear();
                Ui.Aviso("Compra " + folio + " guardada");
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex, "La compra no se guardó"); }
            finally { Cursor = Cursors.Default; Totales(); }
        }
    }
}
