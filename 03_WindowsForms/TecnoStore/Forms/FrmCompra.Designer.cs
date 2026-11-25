namespace TecnoStore.Forms
{
    partial class FrmCompra
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
            this.cboProveedor = new System.Windows.Forms.ComboBox();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.txtSeries = new System.Windows.Forms.TextBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.dgvRecientes = new System.Windows.Forms.DataGridView();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            this.cboProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboProveedor.DisplayMember = "Texto"; this.cboProveedor.ValueMember = "Id";
            this.cboProveedor.Location = new System.Drawing.Point(14, 40); this.cboProveedor.Name = "cboProveedor"; this.cboProveedor.Size = new System.Drawing.Size(320, 23);
            this.cboProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProducto.Location = new System.Drawing.Point(14, 40); this.cboProducto.Name = "cboProducto"; this.cboProducto.Size = new System.Drawing.Size(400, 23);
            this.cboProducto.SelectedIndexChanged += new System.EventHandler(this.cboProducto_SelectedIndexChanged);
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCantidad.Maximum = new decimal(new int[] { 500, 0, 0, 0 }); this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(14, 90); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(80, 23);
            this.nudCosto.DecimalPlaces = 2; this.nudCosto.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCosto.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudCosto.ThousandsSeparator = true;
            this.nudCosto.Location = new System.Drawing.Point(108, 90); this.nudCosto.Name = "nudCosto"; this.nudCosto.Size = new System.Drawing.Size(120, 23);
            this.txtSeries.Location = new System.Drawing.Point(428, 40); this.txtSeries.Multiline = true; this.txtSeries.Name = "txtSeries";
            this.txtSeries.ScrollBars = System.Windows.Forms.ScrollBars.Vertical; this.txtSeries.Size = new System.Drawing.Size(240, 76);
            this.btnAgregar.Location = new System.Drawing.Point(680, 40); this.btnAgregar.Name = "btnAgregar"; this.btnAgregar.Size = new System.Drawing.Size(110, 27); this.btnAgregar.Text = "Agregar ↓";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            this.btnQuitar.Location = new System.Drawing.Point(680, 74); this.btnQuitar.Name = "btnQuitar"; this.btnQuitar.Size = new System.Drawing.Size(110, 27); this.btnQuitar.Text = "Quitar línea";
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            //
            // dgvDetalle
            //
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            this.lblTotal.AutoSize = true; this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(80, 14); this.lblTotal.Name = "lblTotal";
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.Location = new System.Drawing.Point(668, 10); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(130, 32); this.btnGuardar.Text = "Guardar compra";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.dgvRecientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecientes.Name = "dgvRecientes";
            this.errorProvider.ContainerControl = this;
            //
            // FrmCompra
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 680);
            this.Controls.Add(this.dgvDetalle);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCompra";
            this.Text = "Compras a proveedor";
            this.Load += new System.EventHandler(this.FrmCompra_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cboProveedor;
        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.TextBox txtSeries;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.DataGridView dgvRecientes;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
