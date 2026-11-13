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
            this.grpEncabezado = new System.Windows.Forms.GroupBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cboProveedor = new System.Windows.Forms.ComboBox();
            this.lblRecibe = new System.Windows.Forms.Label();
            this.txtRecibe = new System.Windows.Forms.TextBox();
            this.grpAgregar = new System.Windows.Forms.GroupBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblCosto = new System.Windows.Forms.Label();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.lblSeries = new System.Windows.Forms.Label();
            this.txtSeries = new System.Windows.Forms.TextBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.pnlTotales = new System.Windows.Forms.Panel();
            this.lblTotalTxt = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.grpRecientes = new System.Windows.Forms.GroupBox();
            this.dgvRecientes = new System.Windows.Forms.DataGridView();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpEncabezado.SuspendLayout();
            this.grpAgregar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.pnlTotales.SuspendLayout();
            this.grpRecientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // grpEncabezado
            //
            this.grpEncabezado.Controls.Add(this.lblProveedor);
            this.grpEncabezado.Controls.Add(this.cboProveedor);
            this.grpEncabezado.Controls.Add(this.lblRecibe);
            this.grpEncabezado.Controls.Add(this.txtRecibe);
            this.grpEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEncabezado.Name = "grpEncabezado";
            this.grpEncabezado.Size = new System.Drawing.Size(1000, 74);
            this.grpEncabezado.Text = "Encabezado";
            this.lblProveedor.AutoSize = true; this.lblProveedor.Location = new System.Drawing.Point(12, 22); this.lblProveedor.Text = "Proveedor: *";
            this.cboProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboProveedor.DisplayMember = "Texto"; this.cboProveedor.ValueMember = "Id";
            this.cboProveedor.Location = new System.Drawing.Point(14, 40); this.cboProveedor.Name = "cboProveedor"; this.cboProveedor.Size = new System.Drawing.Size(320, 23);
            this.lblRecibe.AutoSize = true; this.lblRecibe.Location = new System.Drawing.Point(346, 22); this.lblRecibe.Text = "Recibe:";
            this.txtRecibe.Location = new System.Drawing.Point(348, 40); this.txtRecibe.Name = "txtRecibe"; this.txtRecibe.ReadOnly = true; this.txtRecibe.Size = new System.Drawing.Size(220, 23);
            //
            // grpAgregar
            //
            this.grpAgregar.Controls.Add(this.lblProducto);
            this.grpAgregar.Controls.Add(this.cboProducto);
            this.grpAgregar.Controls.Add(this.lblCantidad);
            this.grpAgregar.Controls.Add(this.nudCantidad);
            this.grpAgregar.Controls.Add(this.lblCosto);
            this.grpAgregar.Controls.Add(this.nudCosto);
            this.grpAgregar.Controls.Add(this.lblSeries);
            this.grpAgregar.Controls.Add(this.txtSeries);
            this.grpAgregar.Controls.Add(this.btnAgregar);
            this.grpAgregar.Controls.Add(this.btnQuitar);
            this.grpAgregar.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpAgregar.Name = "grpAgregar";
            this.grpAgregar.Size = new System.Drawing.Size(1000, 130);
            this.grpAgregar.Text = "Agregar producto";
            this.lblProducto.AutoSize = true; this.lblProducto.Location = new System.Drawing.Point(12, 22); this.lblProducto.Text = "Producto:";
            this.cboProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProducto.Location = new System.Drawing.Point(14, 40); this.cboProducto.Name = "cboProducto"; this.cboProducto.Size = new System.Drawing.Size(400, 23);
            this.cboProducto.SelectedIndexChanged += new System.EventHandler(this.cboProducto_SelectedIndexChanged);
            this.lblCantidad.AutoSize = true; this.lblCantidad.Location = new System.Drawing.Point(12, 72); this.lblCantidad.Text = "Cantidad:";
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCantidad.Maximum = new decimal(new int[] { 500, 0, 0, 0 }); this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(14, 90); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(80, 23);
            this.lblCosto.AutoSize = true; this.lblCosto.Location = new System.Drawing.Point(106, 72); this.lblCosto.Text = "Costo unitario:";
            this.nudCosto.DecimalPlaces = 2; this.nudCosto.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCosto.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudCosto.ThousandsSeparator = true;
            this.nudCosto.Location = new System.Drawing.Point(108, 90); this.nudCosto.Name = "nudCosto"; this.nudCosto.Size = new System.Drawing.Size(120, 23);
            this.lblSeries.AutoSize = true; this.lblSeries.Location = new System.Drawing.Point(426, 22); this.lblSeries.Name = "lblSeries"; this.lblSeries.Text = "Números de serie (uno por renglón):";
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
            //
            // pnlTotales
            //
            this.pnlTotales.Controls.Add(this.lblTotalTxt);
            this.pnlTotales.Controls.Add(this.lblTotal);
            this.pnlTotales.Controls.Add(this.btnLimpiar);
            this.pnlTotales.Controls.Add(this.btnGuardar);
            this.pnlTotales.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTotales.Name = "pnlTotales";
            this.pnlTotales.Size = new System.Drawing.Size(1000, 52);
            this.lblTotalTxt.AutoSize = true; this.lblTotalTxt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalTxt.Location = new System.Drawing.Point(12, 14); this.lblTotalTxt.Text = "Total:";
            this.lblTotal.AutoSize = true; this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(80, 14); this.lblTotal.Name = "lblTotal";
            this.btnLimpiar.Location = new System.Drawing.Point(560, 10); this.btnLimpiar.Name = "btnLimpiar"; this.btnLimpiar.Size = new System.Drawing.Size(100, 32); this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.Location = new System.Drawing.Point(668, 10); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(130, 32); this.btnGuardar.Text = "Guardar compra";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // grpRecientes
            //
            this.grpRecientes.Controls.Add(this.dgvRecientes);
            this.grpRecientes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpRecientes.Name = "grpRecientes";
            this.grpRecientes.Size = new System.Drawing.Size(1000, 170);
            this.grpRecientes.Text = "Compras recientes";
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
            this.Controls.Add(this.pnlTotales);
            this.Controls.Add(this.grpRecientes);
            this.Controls.Add(this.grpAgregar);
            this.Controls.Add(this.grpEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCompra";
            this.Text = "Compras a proveedor";
            this.Load += new System.EventHandler(this.FrmCompra_Load);
            this.grpEncabezado.ResumeLayout(false);
            this.grpEncabezado.PerformLayout();
            this.grpAgregar.ResumeLayout(false);
            this.grpAgregar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.pnlTotales.ResumeLayout(false);
            this.pnlTotales.PerformLayout();
            this.grpRecientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpEncabezado;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cboProveedor;
        private System.Windows.Forms.Label lblRecibe;
        private System.Windows.Forms.TextBox txtRecibe;
        private System.Windows.Forms.GroupBox grpAgregar;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblCosto;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.Label lblSeries;
        private System.Windows.Forms.TextBox txtSeries;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Panel pnlTotales;
        private System.Windows.Forms.Label lblTotalTxt;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.GroupBox grpRecientes;
        private System.Windows.Forms.DataGridView dgvRecientes;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
