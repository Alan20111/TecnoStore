using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Último paso de la venta: total grande, forma de pago con un toque y
    /// descuento. Es también la confirmación antes de guardar.
    /// </summary>
    public class FrmCobrar : Form
    {
        private readonly List<Opcion> metodos;
        private readonly Segmentos segMetodo, segDescuento = new Segmentos("Sin descuento", "5 %", "10 %", "Otro");
        private readonly NumericUpDown nudOtro = new NumericUpDown { Maximum = 50, TextAlign = HorizontalAlignment.Center };
        private readonly Label lblTotal = new Label { TextAlign = ContentAlignment.MiddleCenter, Font = Tema.FuenteTotal, ForeColor = Tema.Navegacion };
        private readonly Label lblNota = new Label { TextAlign = ContentAlignment.MiddleCenter, ForeColor = Tema.TextoSuave };
        private readonly Button btnCobrar;
        private readonly decimal subtotal;

        public int IdMetodo => metodos[segMetodo.Activo].Id;
        public decimal Descuento => segDescuento.Activo == 0 ? 0 : segDescuento.Activo == 1 ? 5 : segDescuento.Activo == 2 ? 10 : nudOtro.Value;

        public FrmCobrar(decimal subtotal)
        {
            this.subtotal = subtotal;
            metodos = CatalogoRepository.MetodosPago();
            segMetodo = new Segmentos(metodos.Select(m => m.Texto).ToArray());
            AutoScaleMode = AutoScaleMode.None;
            Text = "Cobrar";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            int w = Math.Max(640, Math.Max(segMetodo.AnchoIdeal, segDescuento.AnchoIdeal + 130) + 64);
            ClientSize = new Size(w, 480);

            Poner(lblTotal, 0, 24, w, 70);
            Poner(lblNota, 0, 94, w, 26);
            Poner(Titulo("Forma de pago"), 32, 136, w - 64, 28);
            Poner(segMetodo, 32, 166, segMetodo.AnchoIdeal, Tema.AltoBoton);
            Poner(Titulo("Descuento"), 32, 236, w - 64, 28);
            Poner(segDescuento, 32, 266, segDescuento.AnchoIdeal, Tema.AltoBoton);
            Poner(nudOtro, 44 + segDescuento.AnchoIdeal, 274, 100, 34);
            nudOtro.Visible = false;
            var regla = new Label { ForeColor = Tema.TextoSuave, Text = Db.Sesion.EsSupervisorOMas ? "Puedes autorizar hasta 50 %." : "Más de 10 % lo autoriza un Supervisor." };
            Poner(regla, 32, 320, w - 64, 26);

            btnCobrar = new Button { Name = "btnCobrar", Text = "Cobrar", DialogResult = DialogResult.OK };
            var regresar = new Button { Text = "Regresar", DialogResult = DialogResult.Cancel };
            Poner(regresar, 32, 388, 160, 64);
            Poner(btnCobrar, 208, 388, w - 240, 64);
            AcceptButton = btnCobrar; CancelButton = regresar;
            segDescuento.Elegido += i => { nudOtro.Visible = i == 3; if (i == 3) nudOtro.Focus(); Total(); };
            nudOtro.ValueChanged += (s, e) => Total();
            BackColor = Tema.Tarjeta;
            Tema.Aplicar(this);
            BackColor = Tema.Tarjeta;
            lblTotal.Font = Tema.FuenteTotal;
            btnCobrar.Font = Tema.FuenteSubtitulo;
            Forma.VentanaModerna(this, Tema.Carbon, Tema.Texto);
            Shown += (s, e) => btnCobrar.Focus();
            Total();
        }

        private static Label Titulo(string t) => new Label { Text = t, ForeColor = Tema.TextoSuave, TextAlign = ContentAlignment.MiddleLeft };
        private void Poner(Control c, int x, int y, int w, int h) { c.SetBounds(x, y, w, h); Controls.Add(c); }

        private void Total()
        {
            decimal d = Math.Round(subtotal * Descuento / 100m, 2);
            lblTotal.Text = (subtotal - d).ToString("C");
            lblNota.Text = d > 0 ? "Antes " + subtotal.ToString("C") + "   ·   descuento " + d.ToString("C") : "";
            btnCobrar.Text = "Cobrar " + lblTotal.Text;
        }
    }
}
