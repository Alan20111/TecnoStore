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
            this.grpStock = new System.Windows.Forms.GroupBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.btnAplicarStock = new System.Windows.Forms.Button();
            this.grpPrecio = new System.Windows.Forms.GroupBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.lblPorcentaje = new System.Windows.Forms.Label();
            this.nudPorcentaje = new System.Windows.Forms.NumericUpDown();
            this.btnAplicarPrecio = new System.Windows.Forms.Button();
            this.lblNota = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.grpPrecio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorcentaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // grpStock
            //
            this.grpStock.Controls.Add(this.lblProducto);
            this.grpStock.Controls.Add(this.cboProducto);
            this.grpStock.Controls.Add(this.lblCantidad);
            this.grpStock.Controls.Add(this.nudCantidad);
            this.grpStock.Controls.Add(this.lblMotivo);
            this.grpStock.Controls.Add(this.txtMotivo);
            this.grpStock.Controls.Add(this.btnAplicarStock);
            this.grpStock.Location = new System.Drawing.Point(16, 16);
            this.grpStock.Name = "grpStock";
            this.grpStock.Size = new System.Drawing.Size(520, 200);
            this.grpStock.Text = "Ajuste de inventario (productos sin número de serie)";
            this.lblProducto.AutoSize = true; this.lblProducto.Location = new System.Drawing.Point(12, 24); this.lblProducto.Text = "Producto:";
            this.cboProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboProducto.DisplayMember = "Texto"; this.cboProducto.ValueMember = "Id";
            this.cboProducto.Location = new System.Drawing.Point(14, 42); this.cboProducto.Name = "cboProducto"; this.cboProducto.Size = new System.Drawing.Size(490, 23);
            this.lblCantidad.AutoSize = true; this.lblCantidad.Location = new System.Drawing.Point(12, 72); this.lblCantidad.Text = "Cantidad (+ entra, − sale):";
            this.nudCantidad.Minimum = new decimal(new int[] { 20, 0, 0, -2147483648 }); this.nudCantidad.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(14, 90); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(80, 23);
            this.lblMotivo.AutoSize = true; this.lblMotivo.Location = new System.Drawing.Point(110, 72); this.lblMotivo.Text = "Motivo: *";
            this.txtMotivo.Location = new System.Drawing.Point(112, 90); this.txtMotivo.MaxLength = 200; this.txtMotivo.Name = "txtMotivo"; this.txtMotivo.Size = new System.Drawing.Size(392, 23);
            this.btnAplicarStock.Location = new System.Drawing.Point(384, 150); this.btnAplicarStock.Name = "btnAplicarStock"; this.btnAplicarStock.Size = new System.Drawing.Size(120, 30); this.btnAplicarStock.Text = "Aplicar ajuste";
            this.btnAplicarStock.Click += new System.EventHandler(this.btnAplicarStock_Click);
            //
            // grpPrecio
            //
            this.grpPrecio.Controls.Add(this.lblCategoria);
            this.grpPrecio.Controls.Add(this.cboCategoria);
            this.grpPrecio.Controls.Add(this.lblPorcentaje);
            this.grpPrecio.Controls.Add(this.nudPorcentaje);
            this.grpPrecio.Controls.Add(this.btnAplicarPrecio);
            this.grpPrecio.Location = new System.Drawing.Point(16, 230);
            this.grpPrecio.Name = "grpPrecio";
            this.grpPrecio.Size = new System.Drawing.Size(520, 130);
            this.grpPrecio.Text = "Cambio de precios por categoría";
            this.lblCategoria.AutoSize = true; this.lblCategoria.Location = new System.Drawing.Point(12, 24); this.lblCategoria.Text = "Categoría:";
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboCategoria.DisplayMember = "Texto"; this.cboCategoria.ValueMember = "Id";
            this.cboCategoria.Location = new System.Drawing.Point(14, 42); this.cboCategoria.Name = "cboCategoria"; this.cboCategoria.Size = new System.Drawing.Size(250, 23);
            this.lblPorcentaje.AutoSize = true; this.lblPorcentaje.Location = new System.Drawing.Point(276, 24); this.lblPorcentaje.Text = "Porcentaje (±30):";
            this.nudPorcentaje.DecimalPlaces = 1; this.nudPorcentaje.Minimum = new decimal(new int[] { 30, 0, 0, -2147483648 }); this.nudPorcentaje.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            this.nudPorcentaje.Location = new System.Drawing.Point(278, 42); this.nudPorcentaje.Name = "nudPorcentaje"; this.nudPorcentaje.Size = new System.Drawing.Size(80, 23);
            this.btnAplicarPrecio.Location = new System.Drawing.Point(384, 80); this.btnAplicarPrecio.Name = "btnAplicarPrecio"; this.btnAplicarPrecio.Size = new System.Drawing.Size(120, 30); this.btnAplicarPrecio.Text = "Aplicar precios";
            this.btnAplicarPrecio.Click += new System.EventHandler(this.btnAplicarPrecio_Click);
            //
            // lblNota
            //
            this.lblNota.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNota.Location = new System.Drawing.Point(16, 372);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(520, 40);
            this.lblNota.Text = "Las reglas (permisos, límites, precio nunca menor al costo) las valida SQL Server. Si un precio quedara bajo el costo, no se cambia ninguno.";
            this.errorProvider.ContainerControl = this;
            //
            // FrmAjusteStock
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 430);
            this.Controls.Add(this.grpStock);
            this.Controls.Add(this.grpPrecio);
            this.Controls.Add(this.lblNota);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmAjusteStock";
            this.Text = "Ajustes de inventario y precios";
            this.Load += new System.EventHandler(this.FrmAjusteStock_Load);
            this.grpStock.ResumeLayout(false);
            this.grpStock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.grpPrecio.ResumeLayout(false);
            this.grpPrecio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorcentaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpStock;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Button btnAplicarStock;
        private System.Windows.Forms.GroupBox grpPrecio;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.Label lblPorcentaje;
        private System.Windows.Forms.NumericUpDown nudPorcentaje;
        private System.Windows.Forms.Button btnAplicarPrecio;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
