namespace TecnoStore.Forms
{
    partial class FrmPrincipal
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
            this.pnlLateral = new System.Windows.Forms.Panel();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.pnlMarca = new System.Windows.Forms.Panel();
            this.lblMarca = new System.Windows.Forms.Label();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblIniciales = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblRol = new System.Windows.Forms.Label();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.pnlLateral.SuspendLayout();
            this.pnlMarca.SuspendLayout();
            this.pnlEncabezado.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlLateral (barra lateral oscura)
            //
            this.pnlLateral.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.pnlLateral.Controls.Add(this.pnlMenu);
            this.pnlLateral.Controls.Add(this.pnlMarca);
            this.pnlLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLateral.Name = "pnlLateral";
            this.pnlLateral.Size = new System.Drawing.Size(248, 800);
            //
            // pnlMenu (los botones se agregan según el perfil)
            //
            this.pnlMenu.AutoScroll = true;
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(240, 720);
            //
            // pnlMarca
            //
            this.pnlMarca.Controls.Add(this.lblMarca);
            this.pnlMarca.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMarca.Name = "pnlMarca";
            this.pnlMarca.Size = new System.Drawing.Size(240, 74);
            this.lblMarca.AutoSize = true;
            this.lblMarca.Font = new System.Drawing.Font("Segoe UI Semibold", 17F);
            this.lblMarca.ForeColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.lblMarca.Location = new System.Drawing.Point(20, 20);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Text = "TecnoStore";
            //
            // pnlEncabezado
            //
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Controls.Add(this.lblIniciales);
            this.pnlEncabezado.Controls.Add(this.lblUsuario);
            this.pnlEncabezado.Controls.Add(this.lblRol);
            this.pnlEncabezado.Controls.Add(this.btnCerrarSesion);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(1040, 74);
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.lblTitulo.Location = new System.Drawing.Point(22, 16);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "Inicio";
            this.lblIniciales.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.lblIniciales.Location = new System.Drawing.Point(706, 14);
            this.lblIniciales.Name = "lblIniciales";
            this.lblIniciales.Size = new System.Drawing.Size(36, 36);
            this.lblIniciales.Paint += new System.Windows.Forms.PaintEventHandler(this.lblIniciales_Paint);
            this.lblUsuario.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lblUsuario.Location = new System.Drawing.Point(750, 13);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Text = "Usuario";
            this.lblRol.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.lblRol.AutoSize = true;
            this.lblRol.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblRol.ForeColor = System.Drawing.Color.FromArgb(115, 115, 115);
            this.lblRol.Location = new System.Drawing.Point(751, 33);
            this.lblRol.Name = "lblRol";
            this.lblRol.Text = "Rol";
            this.btnCerrarSesion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnCerrarSesion.Location = new System.Drawing.Point(916, 16);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(110, 32);
            this.btnCerrarSesion.Text = "Salir";
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            //
            // pnlContenido (aquí se abre cada pantalla)
            //
            this.pnlContenido.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Padding = new System.Windows.Forms.Padding(24, 18, 24, 20);
            //
            // FrmPrincipal
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlEncabezado);
            this.Controls.Add(this.pnlLateral);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1100, 680);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TecnoStore";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.pnlLateral.ResumeLayout(false);
            this.pnlMarca.ResumeLayout(false);
            this.pnlMarca.PerformLayout();
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlLateral;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Panel pnlMarca;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblIniciales;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Panel pnlContenido;
    }
}
