using System;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Ajustes (solo Supervisor y Administrador), una tarjeta por tarea:
    ///  - inventario con usp_AjustarStock (±20 piezas, con motivo, queda auditado)
    ///  - precios por porcentaje con usp_AjustarPrecio (±30 %, por categoría)
    /// </summary>
    public partial class FrmAjusteStock : Form, IRefrescable
    {
        public FrmAjusteStock()
        {
            InitializeComponent();
            btnAplicarStock.Text = "Ajustar inventario";
            btnAplicarPrecio.Text = "Cambiar precios";
            Sencillo.Vaciar(this);
            var columnas = new TableLayoutPanel { Dock = DockStyle.Top, Height = 420, ColumnCount = 2, RowCount = 1 };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            int col = 0;
            // Cada tarjeta aparece solo si la base le concede ese ajuste
            if (Db.Sesion.Puede(Capacidad.AjustarStock))
                columnas.Controls.Add(Sencillo.EnTarjeta(new PanelCampos(1, new Campo("Producto", cboProducto),
                    new Campo("Piezas (+ entran, − salen)", nudCantidad), new Campo("Motivo *", txtMotivo), new Campo(null, btnAplicarStock)), "Corregir el inventario"), col++, 0);
            if (Db.Sesion.Puede(Capacidad.AjustarPrecio))
                columnas.Controls.Add(Sencillo.EnTarjeta(new PanelCampos(1, new Campo("Categoría", cboCategoria),
                    new Campo("Porcentaje (+ sube, − baja)", nudPorcentaje), new Campo(null, btnAplicarPrecio)), "Cambiar precios"), col, 0);
            Controls.Add(columnas);
        }

        private void FrmAjusteStock_Load(object sender, EventArgs e)
        {
            try { cboCategoria.DataSource = CatalogoRepository.Categorias(); Refrescar(); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        public void Refrescar()
        {
            try { cboProducto.DataSource = CatalogoRepository.Refacciones(); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnAplicarStock_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboProducto, errorProvider, "Selecciona un producto")
              | !Ui.Requerido(txtMotivo, errorProvider, "Escribe el motivo")) return;
            if (nudCantidad.Value == 0) { Ui.Info("La cantidad no puede ser cero."); return; }
            var p = (Opcion)cboProducto.SelectedItem;
            if (!Ui.Confirmar("¿Ajustar " + nudCantidad.Value.ToString("+0;-0") + " pieza(s) de\n" + p.Texto + "?")) return;
            try
            {
                ProductoRepository.AjustarStock(p.Id, (int)nudCantidad.Value, txtMotivo.Text.Trim());
                Ui.Aviso("Inventario ajustado");
                txtMotivo.Clear(); nudCantidad.Value = 0;
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se aplicó el ajuste"); }
        }

        private void btnAplicarPrecio_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboCategoria, errorProvider, "Selecciona una categoría")) return;
            if (nudPorcentaje.Value == 0) { Ui.Info("El porcentaje no puede ser cero."); return; }
            var c = (Opcion)cboCategoria.SelectedItem;
            if (!Ui.Confirmar("¿Cambiar " + nudPorcentaje.Value.ToString("+0.##;-0.##") + " % el precio de todos los productos activos de " + c.Texto + "?")) return;
            try
            {
                Db.Procedimiento("usp_AjustarPrecio",
                    Db.Decimal("@porcentaje", nudPorcentaje.Value, 5, 2),
                    Db.P("@id_usuario", System.Data.SqlDbType.Int, Db.Sesion.IdUsuario),
                    Db.P("@id_categoria", System.Data.SqlDbType.Int, c.Id));
                Ui.Aviso("Precios actualizados");
                nudPorcentaje.Value = 0;
            }
            catch (Exception ex) { Ui.Error(ex, "No se cambiaron los precios"); }
        }
    }
}
