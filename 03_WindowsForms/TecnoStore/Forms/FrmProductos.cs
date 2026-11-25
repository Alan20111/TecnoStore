using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 8 · Productos: buscador, categoría y "por resurtir"; doble clic abre
    /// la ficha. Búsqueda: usp_ProductoBuscar · Guardar: usp_ProductoGuardar ·
    /// Eliminar: DELETE (el trigger lo convierte en baja lógica si hay historial).
    /// </summary>
    public partial class FrmProductos : Form, IRefrescable
    {
        private int? idActual;          // null = registro nuevo
        private readonly Ficha ficha;
        private readonly PanelCampos campos;
        private readonly Segmentos segAlertas = new Segmentos("Todos", "Por resurtir");

        public FrmProductos()
        {
            InitializeComponent();
            var s = Db.Sesion;
            bool puede = s.Puede(Capacidad.GuardarProducto);
            Ui.Grid(dgvProductos);
            // Lo esencial a la vista; marca, condición y mínimo están en la ficha
            Ui.Mostrar(dgvProductos, "nombre=Producto", "categoria=Categoría", "precio_venta", "stock_actual", "alerta_stock");

            btnNuevo.Text = "Nuevo producto";
            btnEditar.Text = "Abrir";
            btnCancelar.Text = puede ? "Cancelar" : "Cerrar";
            Sencillo.Vaciar(this);
            Sencillo.Pista(txtBuscar, "Buscar producto…");
            Sencillo.BuscarAlEscribir(txtBuscar, Buscar);
            cboFiltroCategoria.SelectedIndexChanged += (o, e) => { if (Visible) Buscar(); };
            segAlertas.Elegido += i => Buscar();
            Sencillo.Barra(pnlFiltros, new Control[] { txtBuscar, cboFiltroCategoria, segAlertas }, puede ? new[] { btnNuevo, btnEditar } : new[] { btnEditar });
            Sencillo.Armar(this, dgvProductos, pnlFiltros);
            Sencillo.Abrir(dgvProductos, Abrir, btnEditar);

            chkSerie.Text = "Cada pieza lleva número de serie";
            chkActivo.Text = "Producto a la venta";
            lblStock.AutoSize = false; lblStock.ForeColor = Tema.TextoSuave;
            campos = new PanelCampos(2,
                new Campo("Nombre *", txtNombre, true),
                new Campo("SKU *", txtSku), new Campo("Categoría", cboCategoria),
                new Campo("Marca", cboMarca), new Campo("Condición", cboCondicion),
                new Campo("Precio de venta *", nudPrecio), new Campo("Costo", nudCosto),
                new Campo("Avisar cuando queden", nudStockMinimo), new Campo("En almacén", lblStock, false, 34),
                new Campo(null, chkSerie), new Campo(null, chkActivo)) { Enabled = puede };
            // Lo que la base no le deja ver, no aparece
            campos.Mostrar(nudCosto, s.Puede(Capacidad.VerCostos));
            ficha = new Ficha(720, 664, campos, puede ? btnGuardar : null, btnCancelar,
                              s.Puede(Capacidad.EliminarProducto) ? btnEliminar : null);
            Disposed += (o, e) => ficha.Dispose();
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            try
            {
                var categorias = CatalogoRepository.Categorias();
                cboFiltroCategoria.Items.Add(new Opcion { Id = 0, Texto = "Todas las categorías" });
                cboFiltroCategoria.Items.AddRange(categorias.ToArray());
                cboFiltroCategoria.SelectedIndex = 0;
                cboCategoria.DataSource = categorias;
                cboMarca.DataSource = CatalogoRepository.Marcas();
                cboCondicion.DataSource = CatalogoRepository.Condiciones();
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        public void Refrescar() => Buscar();

        /// <summary>Desde el Inicio: solo lo que hay que resurtir.</summary>
        public void VerPorResurtir() { segAlertas.Activo = 1; Buscar(); }

        private void Buscar()
        {
            try
            {
                var cat = (Opcion)cboFiltroCategoria.SelectedItem;
                dgvProductos.DataSource = ProductoRepository.Buscar(txtBuscar.Text, cat == null || cat.Id == 0 ? (int?)null : cat.Id, segAlertas.Activo == 1);
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            try
            {
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
                lblStock.Text = fila["stock_actual"] + " piezas";
            }
            catch (Exception ex) { Ui.Error(ex); return; }
            MostrarFicha(txtNombre.Text);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtSku.Clear(); txtNombre.Clear();
            nudPrecio.Value = 0; nudCosto.Value = 0; nudStockMinimo.Value = 0;
            chkSerie.Checked = false; chkActivo.Checked = true;
            MostrarFicha("Nuevo producto");
        }

        private void MostrarFicha(string titulo)
        {
            Ui.SinErrores(errorProvider, txtSku, txtNombre, nudPrecio, nudCosto);
            campos.Mostrar(lblStock, idActual != null);
            btnEliminar.Visible = idActual != null;
            ficha.Abrir(this, titulo);
        }

        private bool Validar()
        {
            bool ok = Ui.Requerido(txtSku, errorProvider, "Falta el SKU")
                    & Ui.Requerido(txtNombre, errorProvider, "Falta el nombre");
            errorProvider.SetError(nudPrecio, nudPrecio.Value <= 0 ? "El precio debe ser mayor a cero" : "");
            errorProvider.SetError(nudCosto, nudCosto.Value > nudPrecio.Value ? "El costo no puede superar al precio" : "");
            return ok && nudPrecio.Value > 0 && nudCosto.Value <= nudPrecio.Value;
        }

        private void dgvProductos_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvProductos.Rows[e.RowIndex];
            string alerta = Convert.ToString(row.Cells["alerta_stock"].Value);
            row.Cells["alerta_stock"].Style.ForeColor = alerta == "Agotado" ? Tema.Rojo : alerta == "Stock bajo" ? Tema.Ambar : Tema.Verde;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;
            btnGuardar.Enabled = false;
            try
            {
                ProductoRepository.Guardar(idActual, txtSku.Text, txtNombre.Text.Trim(),
                    (int)cboCategoria.SelectedValue, (int)cboMarca.SelectedValue, (int)cboCondicion.SelectedValue,
                    nudPrecio.Value, nudCosto.Value, chkSerie.Checked, (int)nudStockMinimo.Value, chkActivo.Checked);
                ficha.Listo();
                Ui.Aviso(idActual == null ? "Producto registrado" : "Cambios guardados");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); }
            finally { btnGuardar.Enabled = true; }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idActual == null) return;
            if (!Ui.Confirmar("¿Eliminar el producto " + txtNombre.Text + "?\n\nSi ya tiene ventas, compras u órdenes, solo se desactivará para conservar el historial.")) return;
            try
            {
                ProductoRepository.Eliminar(idActual.Value);
                ficha.Listo();
                Ui.Aviso("Producto eliminado");
                Buscar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }
    }
}
