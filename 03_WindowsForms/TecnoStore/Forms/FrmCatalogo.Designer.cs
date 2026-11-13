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
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblConteo = new System.Windows.Forms.Label();
            this.lblPermiso = new System.Windows.Forms.Label();
            this.dgvRegistros = new System.Windows.Forms.DataGridView();
            this.pnlDerecho = new System.Windows.Forms.Panel();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblCampo2 = new System.Windows.Forms.Label();
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
            this.pnlDerecho.SuspendLayout();
            this.grpDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblBuscar);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.btnBuscar);
            this.pnlFiltros.Controls.Add(this.lblConteo);
            this.pnlFiltros.Controls.Add(this.lblPermiso);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(900, 56);
            this.lblBuscar.AutoSize = true; this.lblBuscar.Location = new System.Drawing.Point(10, 6); this.lblBuscar.Text = "Buscar por nombre:";
            this.txtBuscar.Location = new System.Drawing.Point(12, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(220, 23);
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            this.btnBuscar.Location = new System.Drawing.Point(242, 22); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(90, 27); this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lblConteo.AutoSize = true; this.lblConteo.ForeColor = System.Drawing.SystemColors.GrayText; this.lblConteo.Location = new System.Drawing.Point(344, 28); this.lblConteo.Name = "lblConteo";
            this.lblPermiso.AutoSize = true; this.lblPermiso.ForeColor = System.Drawing.Color.DarkGoldenrod; this.lblPermiso.Location = new System.Drawing.Point(460, 28); this.lblPermiso.Name = "lblPermiso";
            //
            // dgvRegistros
            //
            this.dgvRegistros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRegistros.Name = "dgvRegistros";
            this.dgvRegistros.SelectionChanged += new System.EventHandler(this.dgvRegistros_SelectionChanged);
            //
            // pnlDerecho
            //
            this.pnlDerecho.Controls.Add(this.grpDatos);
            this.pnlDerecho.Controls.Add(this.btnNuevo);
            this.pnlDerecho.Controls.Add(this.btnEditar);
            this.pnlDerecho.Controls.Add(this.btnEliminar);
            this.pnlDerecho.Controls.Add(this.btnGuardar);
            this.pnlDerecho.Controls.Add(this.btnCancelar);
            this.pnlDerecho.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDerecho.Name = "pnlDerecho";
            this.pnlDerecho.Size = new System.Drawing.Size(340, 500);
            //
            // grpDatos
            //
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.lblCampo2);
            this.grpDatos.Controls.Add(this.txtCampo2);
            this.grpDatos.Controls.Add(this.nudPrecio);
            this.grpDatos.Location = new System.Drawing.Point(10, 6);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(318, 140);
            this.grpDatos.Text = "Datos";
            this.lblNombre.AutoSize = true; this.lblNombre.Location = new System.Drawing.Point(12, 24); this.lblNombre.Text = "Nombre: *";
            this.txtNombre.Location = new System.Drawing.Point(14, 42); this.txtNombre.MaxLength = 80; this.txtNombre.Name = "txtNombre"; this.txtNombre.Size = new System.Drawing.Size(286, 23);
            this.lblCampo2.AutoSize = true; this.lblCampo2.Location = new System.Drawing.Point(12, 74); this.lblCampo2.Name = "lblCampo2"; this.lblCampo2.Text = "Descripción:";
            this.txtCampo2.Location = new System.Drawing.Point(14, 92); this.txtCampo2.Name = "txtCampo2"; this.txtCampo2.Size = new System.Drawing.Size(286, 23);
            this.nudPrecio.DecimalPlaces = 2; this.nudPrecio.Maximum = new decimal(new int[] { 99999, 0, 0, 0 }); this.nudPrecio.ThousandsSeparator = true;
            this.nudPrecio.Location = new System.Drawing.Point(14, 92); this.nudPrecio.Name = "nudPrecio"; this.nudPrecio.Size = new System.Drawing.Size(120, 23);
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 156); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 156); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            this.btnEliminar.Location = new System.Drawing.Point(228, 156); this.btnEliminar.Name = "btnEliminar"; this.btnEliminar.Size = new System.Drawing.Size(100, 28); this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnGuardar.Location = new System.Drawing.Point(119, 190); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 190); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmCatalogo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 556);
            this.Controls.Add(this.dgvRegistros);
            this.Controls.Add(this.pnlDerecho);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCatalogo";
            this.Text = "Catálogo";
            this.Load += new System.EventHandler(this.FrmCatalogo_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).EndInit();
            this.pnlDerecho.ResumeLayout(false);
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.Label lblPermiso;
        private System.Windows.Forms.DataGridView dgvRegistros;
        private System.Windows.Forms.Panel pnlDerecho;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblCampo2;
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
