namespace TecnoStore.Forms
{
    partial class FrmProveedores
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
            this.dgvProveedores = new System.Windows.Forms.DataGridView();
            this.grpProductos = new System.Windows.Forms.GroupBox();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.pnlDerecho = new System.Windows.Forms.Panel();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblRazonSocial = new System.Windows.Forms.Label();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.lblRfc = new System.Windows.Forms.Label();
            this.txtRfc = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProveedores)).BeginInit();
            this.grpProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.pnlDerecho.SuspendLayout();
            this.grpDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblBuscar);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.btnBuscar);
            this.pnlFiltros.Controls.Add(this.lblConteo);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 56);
            this.lblBuscar.AutoSize = true; this.lblBuscar.Location = new System.Drawing.Point(10, 6); this.lblBuscar.Text = "Razón social o RFC:";
            this.txtBuscar.Location = new System.Drawing.Point(12, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(260, 23);
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            this.btnBuscar.Location = new System.Drawing.Point(282, 22); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(90, 27); this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lblConteo.AutoSize = true; this.lblConteo.ForeColor = System.Drawing.SystemColors.GrayText; this.lblConteo.Location = new System.Drawing.Point(384, 28); this.lblConteo.Name = "lblConteo";
            //
            // dgvProveedores
            //
            this.dgvProveedores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProveedores.Name = "dgvProveedores";
            this.dgvProveedores.SelectionChanged += new System.EventHandler(this.dgvProveedores_SelectionChanged);
            this.dgvProveedores.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvProveedores_RowPrePaint);
            //
            // grpProductos
            //
            this.grpProductos.Controls.Add(this.dgvProductos);
            this.grpProductos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpProductos.Name = "grpProductos";
            this.grpProductos.Size = new System.Drawing.Size(760, 220);
            this.grpProductos.Text = "Productos que surte (ProductoProveedor)";
            this.dgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductos.Name = "dgvProductos";
            //
            // pnlDerecho
            //
            this.pnlDerecho.Controls.Add(this.grpDatos);
            this.pnlDerecho.Controls.Add(this.btnNuevo);
            this.pnlDerecho.Controls.Add(this.btnEditar);
            this.pnlDerecho.Controls.Add(this.btnGuardar);
            this.pnlDerecho.Controls.Add(this.btnCancelar);
            this.pnlDerecho.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDerecho.Name = "pnlDerecho";
            this.pnlDerecho.Size = new System.Drawing.Size(340, 600);
            //
            // grpDatos
            //
            this.grpDatos.Controls.Add(this.lblRazonSocial);
            this.grpDatos.Controls.Add(this.txtRazonSocial);
            this.grpDatos.Controls.Add(this.lblRfc);
            this.grpDatos.Controls.Add(this.txtRfc);
            this.grpDatos.Controls.Add(this.lblCorreo);
            this.grpDatos.Controls.Add(this.txtCorreo);
            this.grpDatos.Controls.Add(this.lblTelefono);
            this.grpDatos.Controls.Add(this.txtTelefono);
            this.grpDatos.Controls.Add(this.chkActivo);
            this.grpDatos.Location = new System.Drawing.Point(10, 6);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(318, 260);
            this.grpDatos.Text = "Datos del proveedor";
            this.lblRazonSocial.AutoSize = true; this.lblRazonSocial.Location = new System.Drawing.Point(12, 24); this.lblRazonSocial.Text = "Razón social: *";
            this.txtRazonSocial.Location = new System.Drawing.Point(14, 42); this.txtRazonSocial.MaxLength = 120; this.txtRazonSocial.Name = "txtRazonSocial"; this.txtRazonSocial.Size = new System.Drawing.Size(286, 23);
            this.lblRfc.AutoSize = true; this.lblRfc.Location = new System.Drawing.Point(12, 72); this.lblRfc.Text = "RFC: *";
            this.txtRfc.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRfc.Location = new System.Drawing.Point(14, 90); this.txtRfc.MaxLength = 13; this.txtRfc.Name = "txtRfc"; this.txtRfc.Size = new System.Drawing.Size(140, 23);
            this.lblCorreo.AutoSize = true; this.lblCorreo.Location = new System.Drawing.Point(12, 120); this.lblCorreo.Text = "Correo:";
            this.txtCorreo.Location = new System.Drawing.Point(14, 138); this.txtCorreo.MaxLength = 100; this.txtCorreo.Name = "txtCorreo"; this.txtCorreo.Size = new System.Drawing.Size(286, 23);
            this.lblTelefono.AutoSize = true; this.lblTelefono.Location = new System.Drawing.Point(12, 168); this.lblTelefono.Text = "Teléfono:";
            this.txtTelefono.Location = new System.Drawing.Point(14, 186); this.txtTelefono.MaxLength = 20; this.txtTelefono.Name = "txtTelefono"; this.txtTelefono.Size = new System.Drawing.Size(140, 23);
            this.chkActivo.AutoSize = true; this.chkActivo.Checked = true; this.chkActivo.Location = new System.Drawing.Point(14, 222); this.chkActivo.Name = "chkActivo"; this.chkActivo.Text = "Activo";
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 276); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 276); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            this.btnGuardar.Location = new System.Drawing.Point(119, 310); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 310); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmProveedores
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvProveedores);
            this.Controls.Add(this.grpProductos);
            this.Controls.Add(this.pnlDerecho);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmProveedores";
            this.Text = "Proveedores";
            this.Load += new System.EventHandler(this.FrmProveedores_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProveedores)).EndInit();
            this.grpProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.pnlDerecho.ResumeLayout(false);
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.DataGridView dgvProveedores;
        private System.Windows.Forms.GroupBox grpProductos;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Panel pnlDerecho;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblRazonSocial;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label lblRfc;
        private System.Windows.Forms.TextBox txtRfc;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
