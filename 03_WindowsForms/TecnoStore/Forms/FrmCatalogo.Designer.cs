namespace TecnoStore.Forms
{
    partial class FrmCatalogo
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
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.dgvRegistros = new System.Windows.Forms.DataGridView();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtCampo2 = new System.Windows.Forms.TextBox();
            this.nudPrecio = new System.Windows.Forms.NumericUpDown();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(900, 56);
            this.txtBuscar.Location = new System.Drawing.Point(12, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(220, 23);
            //
            // dgvRegistros
            //
            this.dgvRegistros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRegistros.Name = "dgvRegistros";
            this.txtNombre.Location = new System.Drawing.Point(14, 42); this.txtNombre.MaxLength = 80; this.txtNombre.Name = "txtNombre"; this.txtNombre.Size = new System.Drawing.Size(286, 23);
            this.txtCampo2.Location = new System.Drawing.Point(14, 92); this.txtCampo2.Name = "txtCampo2"; this.txtCampo2.Size = new System.Drawing.Size(286, 23);
            this.nudPrecio.DecimalPlaces = 2; this.nudPrecio.Maximum = new decimal(new int[] { 99999, 0, 0, 0 }); this.nudPrecio.ThousandsSeparator = true;
            this.nudPrecio.Location = new System.Drawing.Point(14, 92); this.nudPrecio.Name = "nudPrecio"; this.nudPrecio.Size = new System.Drawing.Size(120, 23);
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 156); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 156); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnEliminar.Location = new System.Drawing.Point(228, 156); this.btnEliminar.Name = "btnEliminar"; this.btnEliminar.Size = new System.Drawing.Size(100, 28); this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnGuardar.Location = new System.Drawing.Point(119, 190); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 190); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.errorProvider.ContainerControl = this;
            //
            // FrmCatalogo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 556);
            this.Controls.Add(this.dgvRegistros);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCatalogo";
            this.Text = "Catálogo";
            this.Load += new System.EventHandler(this.FrmCatalogo_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.DataGridView dgvRegistros;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtCampo2;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
