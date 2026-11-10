namespace TecnoStore.Forms
{
    partial class FrmVenta
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
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblMetodo = new System.Windows.Forms.Label();
            this.cboMetodo = new System.Windows.Forms.ComboBox();
            this.lblDescuentoPct = new System.Windows.Forms.Label();
            this.nudDescuento = new System.Windows.Forms.NumericUpDown();
            this.lblVendedor = new System.Windows.Forms.Label();
            this.txtVendedor = new System.Windows.Forms.TextBox();
            this.lblReglaDescuento = new System.Windows.Forms.Label();
            this.grpAgregar = new System.Windows.Forms.GroupBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.pnlTotales = new System.Windows.Forms.Panel();
            this.lblSubtotalTxt = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblDescuentoTxt = new System.Windows.Forms.Label();
            this.lblDescuento = new System.Windows.Forms.Label();
            this.lblTotalTxt = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDescuento)).BeginInit();
            this.grpAgregar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.pnlTotales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // grpEncabezado
            //
            this.grpEncabezado.Controls.Add(this.lblCliente);
            this.grpEncabezado.Controls.Add(this.cboCliente);
            this.grpEncabezado.Controls.Add(this.lblMetodo);
            this.grpEncabezado.Controls.Add(this.cboMetodo);
            this.grpEncabezado.Controls.Add(this.lblDescuentoPct);
            this.grpEncabezado.Controls.Add(this.nudDescuento);
            this.grpEncabezado.Controls.Add(this.lblVendedor);
            this.grpEncabezado.Controls.Add(this.txtVendedor);
            this.grpEncabezado.Controls.Add(this.lblReglaDescuento);
            this.grpEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEncabezado.Name = "grpEncabezado";
            this.grpEncabezado.Size = new System.Drawing.Size(1000, 96);
            this.grpEncabezado.Text = "Encabezado";
            this.lblCliente.AutoSize = true; this.lblCliente.Location = new System.Drawing.Point(12, 22); this.lblCliente.Text = "Cliente: *";
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCliente.DisplayMember = "Texto"; this.cboCliente.ValueMember = "Id";
            this.cboCliente.Location = new System.Drawing.Point(14, 40); this.cboCliente.Name = "cboCliente"; this.cboCliente.Size = new System.Drawing.Size(260, 23);
            this.lblMetodo.AutoSize = true; this.lblMetodo.Location = new System.Drawing.Point(286, 22); this.lblMetodo.Text = "Método de pago:";
            this.cboMetodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetodo.DisplayMember = "Texto"; this.cboMetodo.ValueMember = "Id";
            this.cboMetodo.Location = new System.Drawing.Point(288, 40); this.cboMetodo.Name = "cboMetodo"; this.cboMetodo.Size = new System.Drawing.Size(180, 23);
            this.lblDescuentoPct.AutoSize = true; this.lblDescuentoPct.Location = new System.Drawing.Point(480, 22); this.lblDescuentoPct.Text = "Descuento %:";
            this.nudDescuento.DecimalPlaces = 0; this.nudDescuento.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this.nudDescuento.Location = new System.Drawing.Point(482, 40); this.nudDescuento.Name = "nudDescuento"; this.nudDescuento.Size = new System.Drawing.Size(80, 23);
            this.nudDescuento.ValueChanged += new System.EventHandler(this.nudDescuento_ValueChanged);
            this.lblVendedor.AutoSize = true; this.lblVendedor.Location = new System.Drawing.Point(574, 22); this.lblVendedor.Text = "Vendedor:";
            this.txtVendedor.Location = new System.Drawing.Point(576, 40); this.txtVendedor.Name = "txtVendedor"; this.txtVendedor.ReadOnly = true; this.txtVendedor.Size = new System.Drawing.Size(200, 23);
            this.lblReglaDescuento.AutoSize = true; this.lblReglaDescuento.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblReglaDescuento.Location = new System.Drawing.Point(12, 70); this.lblReglaDescuento.Name = "lblReglaDescuento";
            //
            // grpAgregar
            //
            this.grpAgregar.Controls.Add(this.lblProducto);
            this.grpAgregar.Controls.Add(this.cboProducto);
            this.grpAgregar.Controls.Add(this.lblCantidad);
            this.grpAgregar.Controls.Add(this.nudCantidad);
            this.grpAgregar.Controls.Add(this.btnAgregar);
            this.grpAgregar.Controls.Add(this.btnQuitar);
            this.grpAgregar.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpAgregar.Name = "grpAgregar";
            this.grpAgregar.Size = new System.Drawing.Size(1000, 76);
            this.grpAgregar.Text = "Agregar producto";
            this.lblProducto.AutoSize = true; this.lblProducto.Location = new System.Drawing.Point(12, 22); this.lblProducto.Text = "Producto:";
            this.cboProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProducto.Location = new System.Drawing.Point(14, 40); this.cboProducto.Name = "cboProducto"; this.cboProducto.Size = new System.Drawing.Size(454, 23);
            this.lblCantidad.AutoSize = true; this.lblCantidad.Location = new System.Drawing.Point(480, 22); this.lblCantidad.Text = "Cantidad:";
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCantidad.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(482, 40); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(80, 23);
            this.btnAgregar.Location = new System.Drawing.Point(574, 38); this.btnAgregar.Name = "btnAgregar"; this.btnAgregar.Size = new System.Drawing.Size(100, 27); this.btnAgregar.Text = "Agregar ↓";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            this.btnQuitar.Location = new System.Drawing.Point(680, 38); this.btnQuitar.Name = "btnQuitar"; this.btnQuitar.Size = new System.Drawing.Size(100, 27); this.btnQuitar.Text = "Quitar línea";
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            //
            // dgvDetalle
            //
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            //
            // pnlTotales
            //
            this.pnlTotales.Controls.Add(this.lblSubtotalTxt);
            this.pnlTotales.Controls.Add(this.lblSubtotal);
            this.pnlTotales.Controls.Add(this.lblDescuentoTxt);
            this.pnlTotales.Controls.Add(this.lblDescuento);
            this.pnlTotales.Controls.Add(this.lblTotalTxt);
            this.pnlTotales.Controls.Add(this.lblTotal);
            this.pnlTotales.Controls.Add(this.btnLimpiar);
            this.pnlTotales.Controls.Add(this.btnGuardar);
            this.pnlTotales.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTotales.Name = "pnlTotales";
            this.pnlTotales.Size = new System.Drawing.Size(1000, 90);
            this.lblSubtotalTxt.AutoSize = true; this.lblSubtotalTxt.Location = new System.Drawing.Point(14, 10); this.lblSubtotalTxt.Text = "Subtotal:";
            this.lblSubtotal.AutoSize = true; this.lblSubtotal.Location = new System.Drawing.Point(100, 10); this.lblSubtotal.Name = "lblSubtotal";
            this.lblDescuentoTxt.AutoSize = true; this.lblDescuentoTxt.Location = new System.Drawing.Point(14, 30); this.lblDescuentoTxt.Text = "Descuento:";
            this.lblDescuento.AutoSize = true; this.lblDescuento.Location = new System.Drawing.Point(100, 30); this.lblDescuento.Name = "lblDescuento";
            this.lblTotalTxt.AutoSize = true; this.lblTotalTxt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalTxt.Location = new System.Drawing.Point(12, 52); this.lblTotalTxt.Text = "Total:";
            this.lblTotal.AutoSize = true; this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(98, 52); this.lblTotal.Name = "lblTotal";
            this.btnLimpiar.Location = new System.Drawing.Point(560, 30); this.btnLimpiar.Name = "btnLimpiar"; this.btnLimpiar.Size = new System.Drawing.Size(100, 32); this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.Location = new System.Drawing.Point(668, 30); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(130, 32); this.btnGuardar.Text = "Guardar venta";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmVenta
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 620);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.pnlTotales);
            this.Controls.Add(this.grpAgregar);
            this.Controls.Add(this.grpEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmVenta";
            this.Text = "Nueva venta";
            this.Load += new System.EventHandler(this.FrmVenta_Load);
            this.grpEncabezado.ResumeLayout(false);
            this.grpEncabezado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDescuento)).EndInit();
            this.grpAgregar.ResumeLayout(false);
            this.grpAgregar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.pnlTotales.ResumeLayout(false);
            this.pnlTotales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpEncabezado;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblMetodo;
        private System.Windows.Forms.ComboBox cboMetodo;
        private System.Windows.Forms.Label lblDescuentoPct;
        private System.Windows.Forms.NumericUpDown nudDescuento;
        private System.Windows.Forms.Label lblVendedor;
        private System.Windows.Forms.TextBox txtVendedor;
        private System.Windows.Forms.Label lblReglaDescuento;
        private System.Windows.Forms.GroupBox grpAgregar;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Panel pnlTotales;
        private System.Windows.Forms.Label lblSubtotalTxt;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblDescuentoTxt;
        private System.Windows.Forms.Label lblDescuento;
        private System.Windows.Forms.Label lblTotalTxt;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
