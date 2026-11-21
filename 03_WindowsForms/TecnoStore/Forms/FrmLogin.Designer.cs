namespace TecnoStore.Forms
{
    partial class FrmLogin
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblServidor = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(24, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "TecnoStore";
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSubtitulo.Location = new System.Drawing.Point(27, 52);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Text = "Sistema Integral de Gestión";
            //
            // lblServidor
            //
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(27, 88);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Text = "Servidor (localhost, .\\SQLEXPRESS…):";
            //
            // txtServidor
            //
            this.txtServidor.Location = new System.Drawing.Point(30, 106);
            this.txtServidor.MaxLength = 128;
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(280, 23);
            this.txtServidor.TabIndex = 0;
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(27, 140);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Text = "Usuario:";
            //
            // txtUsuario
            //
            this.txtUsuario.Location = new System.Drawing.Point(30, 158);
            this.txtUsuario.MaxLength = 40;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(280, 23);
            this.txtUsuario.TabIndex = 1;
            //
            // lblContrasena
            //
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(27, 192);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Text = "Contraseña:";
            //
            // txtContrasena
            //
            this.txtContrasena.Location = new System.Drawing.Point(30, 210);
            this.txtContrasena.MaxLength = 100;
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Size = new System.Drawing.Size(280, 23);
            this.txtContrasena.TabIndex = 2;
            this.txtContrasena.UseSystemPasswordChar = true;
            //
            // btnIngresar
            //
            this.btnIngresar.Location = new System.Drawing.Point(130, 252);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(88, 28);
            this.btnIngresar.TabIndex = 3;
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.UseVisualStyleBackColor = true;
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);
            //
            // btnSalir
            //
            this.btnSalir.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSalir.Location = new System.Drawing.Point(222, 252);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(88, 28);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblAyuda
            //
            this.lblAyuda.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAyuda.Location = new System.Drawing.Point(27, 296);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(290, 48);
            this.lblAyuda.Text = "Usuarios demo: admin, csupervisor, loperador, fconsulta\r\nContraseña: 1234";
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // FrmLogin
            //
            this.AcceptButton = this.btnIngresar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSalir;
            this.ClientSize = new System.Drawing.Size(344, 352);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblServidor);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblAyuda);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iniciar sesión — TecnoStore";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
