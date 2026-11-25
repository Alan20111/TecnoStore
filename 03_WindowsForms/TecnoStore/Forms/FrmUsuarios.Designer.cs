namespace TecnoStore.Forms
{
    partial class FrmUsuarios
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
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.cboEmpleado = new System.Windows.Forms.ComboBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.txtConfirmar = new System.Windows.Forms.TextBox();
            this.lblContrasenaAyuda = new System.Windows.Forms.Label();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // dgvUsuarios
            //
            this.dgvUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvUsuarios_RowPrePaint);
            this.cboEmpleado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboEmpleado.DisplayMember = "Texto"; this.cboEmpleado.ValueMember = "Id";
            this.cboEmpleado.Location = new System.Drawing.Point(14, 42); this.cboEmpleado.Name = "cboEmpleado"; this.cboEmpleado.Size = new System.Drawing.Size(290, 23);
            this.txtUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtUsuario.Location = new System.Drawing.Point(14, 90); this.txtUsuario.MaxLength = 40; this.txtUsuario.Name = "txtUsuario"; this.txtUsuario.Size = new System.Drawing.Size(140, 23);
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboRol.DisplayMember = "Texto"; this.cboRol.ValueMember = "Id";
            this.cboRol.Location = new System.Drawing.Point(164, 90); this.cboRol.Name = "cboRol"; this.cboRol.Size = new System.Drawing.Size(140, 23);
            this.txtContrasena.Location = new System.Drawing.Point(14, 140); this.txtContrasena.MaxLength = 100; this.txtContrasena.Name = "txtContrasena"; this.txtContrasena.Size = new System.Drawing.Size(140, 23); this.txtContrasena.UseSystemPasswordChar = true;
            this.txtConfirmar.Location = new System.Drawing.Point(164, 140); this.txtConfirmar.MaxLength = 100; this.txtConfirmar.Name = "txtConfirmar"; this.txtConfirmar.Size = new System.Drawing.Size(140, 23); this.txtConfirmar.UseSystemPasswordChar = true;
            this.lblContrasenaAyuda.ForeColor = System.Drawing.SystemColors.GrayText; this.lblContrasenaAyuda.Location = new System.Drawing.Point(12, 168); this.lblContrasenaAyuda.Name = "lblContrasenaAyuda"; this.lblContrasenaAyuda.Size = new System.Drawing.Size(292, 34);
            this.chkActivo.AutoSize = true; this.chkActivo.Checked = true; this.chkActivo.Location = new System.Drawing.Point(14, 210); this.chkActivo.Name = "chkActivo"; this.chkActivo.Text = "Activo (desmarcar = bloquear)";
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 346); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 346); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnGuardar.Location = new System.Drawing.Point(119, 380); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 380); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.errorProvider.ContainerControl = this;
            //
            // FrmUsuarios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Controls.Add(this.dgvUsuarios);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmUsuarios";
            this.Text = "Usuarios y roles";
            this.Load += new System.EventHandler(this.FrmUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.ComboBox cboEmpleado;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.TextBox txtConfirmar;
        private System.Windows.Forms.Label lblContrasenaAyuda;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
