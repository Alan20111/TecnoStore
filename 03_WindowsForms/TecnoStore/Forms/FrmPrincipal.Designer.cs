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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuArchivo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCatalogos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProductos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClientes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProcesos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNuevaVenta = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOrdenes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConsultas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistorial = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReportes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdministracion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAjusteStock = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAuditoria = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRol = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSucursal = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuArchivo, this.mnuCatalogos, this.mnuProcesos, this.mnuConsultas, this.mnuAdministracion });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1184, 24);
            //
            // mnuArchivo
            //
            this.mnuArchivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuCerrarSesion, this.mnuSalir });
            this.mnuArchivo.Name = "mnuArchivo";
            this.mnuArchivo.Text = "&Archivo";
            this.mnuCerrarSesion.Name = "mnuCerrarSesion";
            this.mnuCerrarSesion.Text = "&Cerrar sesión";
            this.mnuCerrarSesion.Click += new System.EventHandler(this.mnuCerrarSesion_Click);
            this.mnuSalir.Name = "mnuSalir";
            this.mnuSalir.Text = "&Salir";
            this.mnuSalir.Click += new System.EventHandler(this.mnuSalir_Click);
            //
            // mnuCatalogos
            //
            this.mnuCatalogos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuProductos, this.mnuClientes });
            this.mnuCatalogos.Name = "mnuCatalogos";
            this.mnuCatalogos.Text = "&Catálogos";
            this.mnuProductos.Name = "mnuProductos";
            this.mnuProductos.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuProductos.Text = "&Productos";
            this.mnuProductos.Click += new System.EventHandler(this.mnuProductos_Click);
            this.mnuClientes.Name = "mnuClientes";
            this.mnuClientes.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.mnuClientes.Text = "C&lientes";
            this.mnuClientes.Click += new System.EventHandler(this.mnuClientes_Click);
            //
            // mnuProcesos
            //
            this.mnuProcesos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuNuevaVenta, this.mnuOrdenes });
            this.mnuProcesos.Name = "mnuProcesos";
            this.mnuProcesos.Text = "&Procesos";
            this.mnuNuevaVenta.Name = "mnuNuevaVenta";
            this.mnuNuevaVenta.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuNuevaVenta.Text = "&Nueva venta";
            this.mnuNuevaVenta.Click += new System.EventHandler(this.mnuNuevaVenta_Click);
            this.mnuOrdenes.Name = "mnuOrdenes";
            this.mnuOrdenes.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.mnuOrdenes.Text = "Órdenes de &servicio";
            this.mnuOrdenes.Click += new System.EventHandler(this.mnuOrdenes_Click);
            //
            // mnuConsultas
            //
            this.mnuConsultas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuHistorial, this.mnuReportes });
            this.mnuConsultas.Name = "mnuConsultas";
            this.mnuConsultas.Text = "C&onsultas";
            this.mnuHistorial.Name = "mnuHistorial";
            this.mnuHistorial.Text = "&Historial de ventas";
            this.mnuHistorial.Click += new System.EventHandler(this.mnuHistorial_Click);
            this.mnuReportes.Name = "mnuReportes";
            this.mnuReportes.Text = "&Reportes";
            this.mnuReportes.Click += new System.EventHandler(this.mnuReportes_Click);
            //
            // mnuAdministracion
            //
            this.mnuAdministracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuAjusteStock, this.mnuAuditoria });
            this.mnuAdministracion.Name = "mnuAdministracion";
            this.mnuAdministracion.Text = "A&dministración";
            this.mnuAjusteStock.Name = "mnuAjusteStock";
            this.mnuAjusteStock.Text = "&Ajuste de inventario";
            this.mnuAjusteStock.Click += new System.EventHandler(this.mnuAjusteStock_Click);
            this.mnuAuditoria.Name = "mnuAuditoria";
            this.mnuAuditoria.Text = "A&uditoría";
            this.mnuAuditoria.Click += new System.EventHandler(this.mnuAuditoria_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblUsuario, this.lblRol, this.lblSucursal });
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1184, 22);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblUsuario.Text = "Usuario";
            this.lblRol.Name = "lblRol";
            this.lblRol.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblRol.Text = "Rol";
            this.lblSucursal.Name = "lblSucursal";
            this.lblSucursal.Text = "Sucursal";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TecnoStore";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuArchivo;
        private System.Windows.Forms.ToolStripMenuItem mnuCerrarSesion;
        private System.Windows.Forms.ToolStripMenuItem mnuSalir;
        private System.Windows.Forms.ToolStripMenuItem mnuCatalogos;
        private System.Windows.Forms.ToolStripMenuItem mnuProductos;
        private System.Windows.Forms.ToolStripMenuItem mnuClientes;
        private System.Windows.Forms.ToolStripMenuItem mnuProcesos;
        private System.Windows.Forms.ToolStripMenuItem mnuNuevaVenta;
        private System.Windows.Forms.ToolStripMenuItem mnuOrdenes;
        private System.Windows.Forms.ToolStripMenuItem mnuConsultas;
        private System.Windows.Forms.ToolStripMenuItem mnuHistorial;
        private System.Windows.Forms.ToolStripMenuItem mnuReportes;
        private System.Windows.Forms.ToolStripMenuItem mnuAdministracion;
        private System.Windows.Forms.ToolStripMenuItem mnuAjusteStock;
        private System.Windows.Forms.ToolStripMenuItem mnuAuditoria;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuario;
        private System.Windows.Forms.ToolStripStatusLabel lblRol;
        private System.Windows.Forms.ToolStripStatusLabel lblSucursal;
    }
}
