namespace TecnoStore.Forms
{
    partial class FrmAjusteStock
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.btnAplicarStock = new System.Windows.Forms.Button();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.nudPorcentaje = new System.Windows.Forms.NumericUpDown();
            this.btnAplicarPrecio = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorcentaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            this.cboProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboProducto.DisplayMember = "Texto"; this.cboProducto.ValueMember = "Id";
            this.cboProducto.Location = new System.Drawing.Point(14, 42); this.cboProducto.Name = "cboProducto"; this.cboProducto.Size = new System.Drawing.Size(490, 23);
            this.nudCantidad.Minimum = new decimal(new int[] { 20, 0, 0, -2147483648 }); this.nudCantidad.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(14, 90); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(80, 23);
            this.txtMotivo.Location = new System.Drawing.Point(112, 90); this.txtMotivo.MaxLength = 200; this.txtMotivo.Name = "txtMotivo"; this.txtMotivo.Size = new System.Drawing.Size(392, 23);
            this.btnAplicarStock.Location = new System.Drawing.Point(384, 150); this.btnAplicarStock.Name = "btnAplicarStock"; this.btnAplicarStock.Size = new System.Drawing.Size(120, 30); this.btnAplicarStock.Text = "Aplicar ajuste";
            this.btnAplicarStock.Click += new System.EventHandler(this.btnAplicarStock_Click);
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboCategoria.DisplayMember = "Texto"; this.cboCategoria.ValueMember = "Id";
            this.cboCategoria.Location = new System.Drawing.Point(14, 42); this.cboCategoria.Name = "cboCategoria"; this.cboCategoria.Size = new System.Drawing.Size(250, 23);
            this.nudPorcentaje.DecimalPlaces = 1; this.nudPorcentaje.Minimum = new decimal(new int[] { 30, 0, 0, -2147483648 }); this.nudPorcentaje.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            this.nudPorcentaje.Location = new System.Drawing.Point(278, 42); this.nudPorcentaje.Name = "nudPorcentaje"; this.nudPorcentaje.Size = new System.Drawing.Size(80, 23);
            this.btnAplicarPrecio.Location = new System.Drawing.Point(384, 80); this.btnAplicarPrecio.Name = "btnAplicarPrecio"; this.btnAplicarPrecio.Size = new System.Drawing.Size(120, 30); this.btnAplicarPrecio.Text = "Aplicar precios";
            this.btnAplicarPrecio.Click += new System.EventHandler(this.btnAplicarPrecio_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmAjusteStock
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 430);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmAjusteStock";
            this.Text = "Ajustes de inventario y precios";
            this.Load += new System.EventHandler(this.FrmAjusteStock_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorcentaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Button btnAplicarStock;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.NumericUpDown nudPorcentaje;
        private System.Windows.Forms.Button btnAplicarPrecio;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
