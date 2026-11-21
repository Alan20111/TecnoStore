using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 8 · Productos: CRUD con filtros, existencias y alertas.
    /// Búsqueda: usp_ProductoBuscar · Guardar: usp_ProductoGuardar ·
    /// Eliminar: DELETE (el trigger lo convierte en baja lógica si hay historial).
    /// </summary>
    public partial class FrmProductos : Form
    {
        private int? idActual;          // null = registro nuevo
        private bool editando;

        public FrmProductos()
        {
            InitializeComponent();
            Ui.Grid(dgvProductos);
            // Lo esencial a la vista; marca, condición, mínimo y margen en el panel de la derecha
            Ui.Mostrar(dgvProductos, "sku", "nombre=Producto", "categoria=Categoría", "precio_venta", "stock_actual", "alerta_stock");
            var s = Db.Sesion;
            // Lo que la base no le permite, ni se ve
            btnNuevo.Visible = btnEditar.Visible = btnGuardar.Visible = btnCancelar.Visible = s.Puede(Capacidad.GuardarProducto);
            btnEliminar.Visible = s.Puede(Capacidad.EliminarProducto);
            lblCosto.Visible = nudCosto.Visible = s.Puede(Capacidad.VerCostos);
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            try
            {
                var categorias = CatalogoRepository.Categorias();
                cboFiltroCategoria.Items.Add(new Opcion { Id = 0, Texto = "(Todas)" });
                cboFiltroCategoria.Items.AddRange(categorias.ToArray());
                cboFiltroCategoria.SelectedIndex = 0;
                cboCategoria.DataSource = categorias;
                cboMarca.DataSource = CatalogoRepository.Marcas();
                cboCondicion.DataSource = CatalogoRepository.Condiciones();
                Buscar();
                Estado(false);
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Buscar()
        {
            var cat = (Opcion)cboFiltroCategoria.SelectedItem;
            dgvProductos.DataSource = ProductoRepository.Buscar(txtBuscar.Text, cat == null || cat.Id == 0 ? (int?)null : cat.Id, chkAlertas.Checked);
            lblConteo.Text = dgvProductos.Rows.Count + " producto(s)";
        }

        /// <summary>Estados de la pantalla: lectura o edición (nuevo/existente).</summary>
        private void Estado(bool edicion)
        {
            editando = edicion;
            bool puedeEditar = Db.Sesion.Puede(Capacidad.GuardarProducto);
            grpDatos.Enabled = edicion;
            btnNuevo.Enabled = !edicion && puedeEditar;
            btnEditar.Enabled = !edicion && puedeEditar && dgvProductos.CurrentRow != null;
            btnGuardar.Enabled = edicion;
            btnCancelar.Enabled = edicion;
            btnEliminar.Enabled = !edicion && Db.Sesion.Puede(Capacidad.EliminarProducto) && dgvProductos.CurrentRow != null;
            dgvProductos.Enabled = !edicion;
            grpDatos.Text = !edicion ? "Datos del producto (lectura)" : idActual == null ? "Nuevo producto" : "Editando producto";
        }

        private void Limpiar()
        {
            idActual = null;
            txtSku.Clear(); txtNombre.Clear();
            nudPrecio.Value = 0; nudCosto.Value = 0; nudStockMinimo.Value = 0;
            chkSerie.Checked = false; chkActivo.Checked = true;
            lblStock.Text = "Stock actual: 0 (entra por compras)";
            foreach (Control c in grpDatos.Controls) { errorProvider.SetError(c, ""); if (c is TextBox) c.BackColor = SystemColors.Window; }
        }

        private void MostrarSeleccionado()
        {
            if (editando || dgvProductos.CurrentRow == null) return;
            var fila = ProductoRepository.Obtener((int)dgvProductos.CurrentRow.Cells["id_producto"].Value);
            if (fila == null) return;
            idActual = (int)fila["id_producto"];
            txtSku.Text = (string)fila["sku"];
            txtNombre.Text = (string)fila["nombre"];
            cboCategoria.SelectedValue = fila["id_categoria"];
            cboMarca.SelectedValue = fila["id_marca"];
            cboCondicion.SelectedValue = fila["id_condicion"];
            nudPrecio.Value = (decimal)fila["precio_venta"];
            nudCosto.Value = (decimal)fila["costo"];
            nudStockMinimo.Value = (int)fila["stock_minimo"];
            chkSerie.Checked = (bool)fila["requiere_serie"];
            chkActivo.Checked = (bool)fila["activo"];
            lblStock.Text = "Stock actual: " + fila["stock_actual"] + "   ·   Mínimo: " + fila["stock_minimo"] +
                            (Db.Sesion.Puede(Capacidad.VerCostos) && nudPrecio.Value > 0
                                ? "   ·   Margen: " + ((nudPrecio.Value - nudCosto.Value) * 100 / nudPrecio.Value).ToString("0.0") + " %" : "");
            Estado(false);
        }

        private bool Validar()
        {
            bool ok = Ui.Requerido(txtSku, errorProvider, "El SKU es obligatorio")
                    & Ui.Requerido(txtNombre, errorProvider, "El nombre es obligatorio");
            errorProvider.SetError(nudPrecio, nudPrecio.Value <= 0 ? "El precio debe ser mayor a cero" : "");
            errorProvider.SetError(nudCosto, nudCosto.Value > nudPrecio.Value ? "El costo no puede superar al precio" : "");
            return ok && nudPrecio.Value > 0 && nudCosto.Value <= nudPrecio.Value;
        }

        private void btnBuscar_Click(object sender, EventArgs e) { try { Buscar(); } catch (Exception ex) { Ui.Error(ex); } }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) btnBuscar.PerformClick(); }
        private void dgvProductos_SelectionChanged(object sender, EventArgs e) { try { MostrarSeleccionado(); } catch (Exception ex) { Ui.Error(ex); } }

        private void dgvProductos_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvProductos.Rows[e.RowIndex];
            string alerta = Convert.ToString(row.Cells["alerta_stock"].Value);
            row.DefaultCellStyle.BackColor = alerta == "Agotado" ? Color.MistyRose : alerta == "Stock bajo" ? Color.LemonChiffon : SystemColors.Window;
        }

        private void btnNuevo_Click(object sender, EventArgs e) { Limpiar(); Estado(true); txtSku.Focus(); }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idActual == null) return;
            Estado(true);
            txtNombre.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Estado(false);
            Limpiar();
            MostrarSeleccionado();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;
            btnGuardar.Enabled = false;
            try
            {
                int id = ProductoRepository.Guardar(idActual, txtSku.Text, txtNombre.Text.Trim(),
                    (int)cboCategoria.SelectedValue, (int)cboMarca.SelectedValue, (int)cboCondicion.SelectedValue,
                    nudPrecio.Value, nudCosto.Value, chkSerie.Checked, (int)nudStockMinimo.Value, chkActivo.Checked);
                Ui.Info(idActual == null ? "Producto registrado." : "Cambios guardados.");
                Estado(false);
                Buscar();
                Seleccionar(id);
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); btnGuardar.Enabled = true; }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == null) return;
            if (!Ui.Confirmar("¿Eliminar el producto " + txtSku.Text + "?\n\nSi ya tiene ventas, compras u órdenes, solo se desactivará para conservar el historial.")) return;
            try
            {
                ProductoRepository.Eliminar(idActual.Value);
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Seleccionar(int id)
        {
            foreach (DataGridViewRow r in dgvProductos.Rows)
                if ((int)r.Cells["id_producto"].Value == id) { r.Selected = true; dgvProductos.CurrentCell = r.Cells.Cast<DataGridViewCell>().First(c => c.Visible); return; }
        }
    }
}
