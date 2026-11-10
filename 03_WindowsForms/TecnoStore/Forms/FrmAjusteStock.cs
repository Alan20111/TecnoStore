using System;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Ajustes (solo Supervisor y Administrador):
    ///  - inventario con usp_AjustarStock (±20 piezas, con motivo, queda auditado)
    ///  - precios por porcentaje con usp_AjustarPrecio (±30 %, por categoría o producto)
    /// </summary>
    public partial class FrmAjusteStock : Form
    {
        public FrmAjusteStock()
        {
            InitializeComponent();
        }

        private void FrmAjusteStock_Load(object sender, EventArgs e)
        {
            try
            {
                cboProducto.DataSource = CatalogoRepository.Refacciones();
                var categorias = CatalogoRepository.Categorias();
                cboCategoria.DataSource = categorias;
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void btnAplicarStock_Click(object sender, EventArgs e)
        {
            if (!Ui.Requerido(cboProducto, errorProvider, "Selecciona un producto")
              | !Ui.Requerido(txtMotivo, errorProvider, "El motivo es obligatorio")) return;
            if (nudCantidad.Value == 0) { Ui.Info("La cantidad no puede ser cero."); return; }
            var p = (Opcion)cboProducto.SelectedItem;
            if (!Ui.Confirmar("¿Ajustar " + nudCantidad.Value.ToString("+0;-0") + " pieza(s) de\n" + p.Texto + "?")) return;
            try
            {
                ProductoRepository.AjustarStock(p.Id, (int)nudCantidad.Value, txtMotivo.Text.Trim());
                Ui.Info("Ajuste aplicado y registrado en la auditoría.");
                txtMotivo.Clear(); nudCantidad.Value = 0;
                cboProducto.DataSource = CatalogoRepository.Refacciones();
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
                Ui.Info("Precios actualizados; cada cambio quedó en la auditoría.");
                nudPorcentaje.Value = 0;
            }
            catch (Exception ex) { Ui.Error(ex, "No se cambiaron los precios (ROLLBACK)"); }
        }
    }
}
