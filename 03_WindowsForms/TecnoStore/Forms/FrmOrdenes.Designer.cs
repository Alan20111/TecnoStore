namespace TecnoStore.Forms
{
    partial class FrmOrdenes
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
            this.pnlArriba = new System.Windows.Forms.Panel();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.cboTecnico = new System.Windows.Forms.ComboBox();
            this.txtEquipo = new System.Windows.Forms.TextBox();
            this.txtSerie = new System.Windows.Forms.TextBox();
            this.txtFalla = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.cboServicio = new System.Windows.Forms.ComboBox();
            this.btnAgregarServicio = new System.Windows.Forms.Button();
            this.cboRefaccion = new System.Windows.Forms.ComboBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregarRefaccion = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlArriba.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlArriba
            //
            this.pnlArriba.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlArriba.Name = "pnlArriba";
            this.pnlArriba.Size = new System.Drawing.Size(1100, 34);
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvOrdenes_RowPrePaint);
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboCliente.DisplayMember = "Texto"; this.cboCliente.ValueMember = "Id";
            this.cboCliente.Location = new System.Drawing.Point(14, 40); this.cboCliente.Name = "cboCliente"; this.cboCliente.Size = new System.Drawing.Size(292, 23);
            this.cboTecnico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboTecnico.DisplayMember = "Texto"; this.cboTecnico.ValueMember = "Id";
            this.cboTecnico.Location = new System.Drawing.Point(14, 86); this.cboTecnico.Name = "cboTecnico"; this.cboTecnico.Size = new System.Drawing.Size(292, 23);
            this.txtEquipo.Location = new System.Drawing.Point(14, 132); this.txtEquipo.MaxLength = 150; this.txtEquipo.Name = "txtEquipo"; this.txtEquipo.Size = new System.Drawing.Size(180, 23);
            this.txtSerie.Location = new System.Drawing.Point(202, 132); this.txtSerie.MaxLength = 50; this.txtSerie.Name = "txtSerie"; this.txtSerie.Size = new System.Drawing.Size(104, 23);
            this.txtFalla.Location = new System.Drawing.Point(14, 178); this.txtFalla.MaxLength = 500; this.txtFalla.Multiline = true; this.txtFalla.Name = "txtFalla"; this.txtFalla.Size = new System.Drawing.Size(292, 70);
            this.btnRegistrar.Location = new System.Drawing.Point(186, 258); this.btnRegistrar.Name = "btnRegistrar"; this.btnRegistrar.Size = new System.Drawing.Size(120, 28); this.btnRegistrar.Text = "Registrar orden";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            this.cboServicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboServicio.DisplayMember = "Texto"; this.cboServicio.ValueMember = "Id";
            this.cboServicio.Location = new System.Drawing.Point(14, 40); this.cboServicio.Name = "cboServicio"; this.cboServicio.Size = new System.Drawing.Size(210, 23);
            this.btnAgregarServicio.Location = new System.Drawing.Point(230, 39); this.btnAgregarServicio.Name = "btnAgregarServicio"; this.btnAgregarServicio.Size = new System.Drawing.Size(76, 25); this.btnAgregarServicio.Text = "Agregar";
            this.btnAgregarServicio.Click += new System.EventHandler(this.btnAgregarServicio_Click);
            this.cboRefaccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboRefaccion.DisplayMember = "Texto"; this.cboRefaccion.ValueMember = "Id";
            this.cboRefaccion.Location = new System.Drawing.Point(14, 88); this.cboRefaccion.Name = "cboRefaccion"; this.cboRefaccion.Size = new System.Drawing.Size(160, 23);
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(180, 88); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(44, 23);
            this.btnAgregarRefaccion.Location = new System.Drawing.Point(230, 87); this.btnAgregarRefaccion.Name = "btnAgregarRefaccion"; this.btnAgregarRefaccion.Size = new System.Drawing.Size(76, 25); this.btnAgregarRefaccion.Text = "Agregar";
            this.btnAgregarRefaccion.Click += new System.EventHandler(this.btnAgregarRefaccion_Click);
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistorial.Name = "dgvHistorial";
            this.errorProvider.ContainerControl = this;
            //
            // FrmOrdenes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvOrdenes);
            this.Controls.Add(this.pnlArriba);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmOrdenes";
            this.Text = "Órdenes de servicio";
            this.Load += new System.EventHandler(this.FrmOrdenes_Load);
            this.pnlArriba.ResumeLayout(false);
            this.pnlArriba.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlArriba;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.ComboBox cboTecnico;
        private System.Windows.Forms.TextBox txtEquipo;
        private System.Windows.Forms.TextBox txtSerie;
        private System.Windows.Forms.TextBox txtFalla;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.ComboBox cboServicio;
        private System.Windows.Forms.Button btnAgregarServicio;
        private System.Windows.Forms.ComboBox cboRefaccion;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregarRefaccion;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
