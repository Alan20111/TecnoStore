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
            this.pnlBuscar = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.tarProductos = new TecnoStore.Utils.Tarjeta();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.pnlHueco = new System.Windows.Forms.Panel();
            this.tarCuenta = new TecnoStore.Utils.Tarjeta();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.pnlCliente = new System.Windows.Forms.Panel();
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.pnlCobro = new System.Windows.Forms.Panel();
            this.btnMenos = new System.Windows.Forms.Button();
            this.btnMas = new System.Windows.Forms.Button();
            this.btnVaciar = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlBuscar.SuspendLayout();
            this.tarProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.tarCuenta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.pnlCliente.SuspendLayout();
            this.pnlCobro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlBuscar
            //
            this.pnlBuscar.Controls.Add(this.txtBuscar);
            this.pnlBuscar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBuscar.Name = "pnlBuscar";
            this.pnlBuscar.Size = new System.Drawing.Size(560, 60);
            this.txtBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscar.Location = new System.Drawing.Point(0, 8);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(560, 31);
            this.txtBuscar.TabIndex = 0;
            //
            // tarProductos
            //
            this.tarProductos.Controls.Add(this.dgvProductos);
            this.tarProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tarProductos.Name = "tarProductos";
            this.tarProductos.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
            this.dgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.TabIndex = 1;
            //
            // pnlHueco
            //
            this.pnlHueco.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlHueco.Name = "pnlHueco";
            this.pnlHueco.Size = new System.Drawing.Size(18, 620);
            //
            // tarCuenta
            //
            this.tarCuenta.Controls.Add(this.dgvDetalle);
            this.tarCuenta.Controls.Add(this.pnlCliente);
            this.tarCuenta.Controls.Add(this.pnlCobro);
            this.tarCuenta.Dock = System.Windows.Forms.DockStyle.Right;
            this.tarCuenta.Name = "tarCuenta";
            this.tarCuenta.Padding = new System.Windows.Forms.Padding(16, 12, 16, 16);
            this.tarCuenta.Size = new System.Drawing.Size(430, 620);
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.TabIndex = 3;
            //
            // pnlCliente
            //
            this.pnlCliente.Controls.Add(this.lblCliente);
            this.pnlCliente.Controls.Add(this.cboCliente);
            this.pnlCliente.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCliente.Name = "pnlCliente";
            this.pnlCliente.Size = new System.Drawing.Size(398, 84);
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(0, 2);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Text = "Cliente";
            this.cboCliente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCliente.DisplayMember = "Texto";
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCliente.Location = new System.Drawing.Point(2, 28);
            this.cboCliente.Name = "cboCliente";
            this.cboCliente.Size = new System.Drawing.Size(372, 31);
            this.cboCliente.TabIndex = 2;
            this.cboCliente.ValueMember = "Id";
            //
            // pnlCobro
            //
            this.pnlCobro.Controls.Add(this.btnMenos);
            this.pnlCobro.Controls.Add(this.btnMas);
            this.pnlCobro.Controls.Add(this.btnVaciar);
            this.pnlCobro.Controls.Add(this.lblTotal);
            this.pnlCobro.Controls.Add(this.btnCobrar);
            this.pnlCobro.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCobro.Name = "pnlCobro";
            this.pnlCobro.Size = new System.Drawing.Size(398, 222);
            this.btnMenos.Location = new System.Drawing.Point(0, 8);
            this.btnMenos.Name = "btnMenos";
            this.btnMenos.Size = new System.Drawing.Size(72, 48);
            this.btnMenos.Text = "−";
            this.btnMenos.Click += new System.EventHandler(this.btnMenos_Click);
            this.btnMas.Location = new System.Drawing.Point(80, 8);
            this.btnMas.Name = "btnMas";
            this.btnMas.Size = new System.Drawing.Size(72, 48);
            this.btnMas.Text = "+";
            this.btnMas.Click += new System.EventHandler(this.btnMas_Click);
            this.btnVaciar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVaciar.Location = new System.Drawing.Point(288, 8);
            this.btnVaciar.Name = "btnVaciar";
            this.btnVaciar.Size = new System.Drawing.Size(110, 48);
            this.btnVaciar.Text = "Vaciar";
            this.btnVaciar.Click += new System.EventHandler(this.btnVaciar_Click);
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.lblTotal.Location = new System.Drawing.Point(0, 62);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(398, 74);
            this.lblTotal.Text = "$0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCobrar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCobrar.Location = new System.Drawing.Point(0, 142);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(398, 76);
            this.btnCobrar.TabIndex = 4;
            this.btnCobrar.Text = "COBRAR";
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmVenta
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1010, 620);
            this.Controls.Add(this.tarProductos);
            this.Controls.Add(this.pnlBuscar);
            this.Controls.Add(this.pnlHueco);
            this.Controls.Add(this.tarCuenta);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.Name = "FrmVenta";
            this.Text = "Vender";
            this.Load += new System.EventHandler(this.FrmVenta_Load);
            this.pnlBuscar.ResumeLayout(false);
            this.pnlBuscar.PerformLayout();
            this.tarProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.tarCuenta.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.pnlCliente.ResumeLayout(false);
            this.pnlCliente.PerformLayout();
            this.pnlCobro.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private TecnoStore.Utils.Tarjeta tarProductos;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Panel pnlHueco;
        private TecnoStore.Utils.Tarjeta tarCuenta;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Panel pnlCliente;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Panel pnlCobro;
        private System.Windows.Forms.Button btnMenos;
        private System.Windows.Forms.Button btnMas;
        private System.Windows.Forms.Button btnVaciar;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
