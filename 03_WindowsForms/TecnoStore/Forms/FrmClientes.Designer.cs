namespace TecnoStore.Forms
{
    partial class FrmClientes
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
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtApellidos = new System.Windows.Forms.TextBox();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtRfc = new System.Windows.Forms.TextBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 56);
            this.txtBuscar.Location = new System.Drawing.Point(12, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(260, 23);
            //
            // dgvClientes
            //
            this.dgvClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvClientes_RowPrePaint);
            this.dgvHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistorial.Name = "dgvHistorial";
            this.txtNombre.Location = new System.Drawing.Point(14, 42); this.txtNombre.MaxLength = 60; this.txtNombre.Name = "txtNombre"; this.txtNombre.Size = new System.Drawing.Size(286, 23);
            this.txtApellidos.Location = new System.Drawing.Point(14, 90); this.txtApellidos.MaxLength = 80; this.txtApellidos.Name = "txtApellidos"; this.txtApellidos.Size = new System.Drawing.Size(286, 23);
            this.txtCorreo.Location = new System.Drawing.Point(14, 138); this.txtCorreo.MaxLength = 100; this.txtCorreo.Name = "txtCorreo"; this.txtCorreo.Size = new System.Drawing.Size(286, 23);
            this.txtTelefono.Location = new System.Drawing.Point(14, 186); this.txtTelefono.MaxLength = 20; this.txtTelefono.Name = "txtTelefono"; this.txtTelefono.Size = new System.Drawing.Size(138, 23);
            this.txtRfc.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRfc.Location = new System.Drawing.Point(162, 186); this.txtRfc.MaxLength = 13; this.txtRfc.Name = "txtRfc"; this.txtRfc.Size = new System.Drawing.Size(138, 23);
            this.chkActivo.AutoSize = true; this.chkActivo.Checked = true; this.chkActivo.Location = new System.Drawing.Point(14, 222); this.chkActivo.Name = "chkActivo"; this.chkActivo.Text = "Activo (desmarcar = baja lógica)";
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 316); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 316); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnGuardar.Location = new System.Drawing.Point(119, 350); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 350); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.errorProvider.ContainerControl = this;
            //
            // FrmClientes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvClientes);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmClientes";
            this.Text = "Clientes";
            this.Load += new System.EventHandler(this.FrmClientes_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtApellidos;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtRfc;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
