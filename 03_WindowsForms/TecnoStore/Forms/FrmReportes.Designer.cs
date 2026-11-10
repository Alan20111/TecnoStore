namespace TecnoStore.Forms
{
    partial class FrmReportes
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
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.lblKpi = new System.Windows.Forms.Label();
            this.tabReportes = new System.Windows.Forms.TabControl();
            this.tabResumen = new System.Windows.Forms.TabPage();
            this.dgvResumen = new System.Windows.Forms.DataGridView();
            this.tabTop = new System.Windows.Forms.TabPage();
            this.dgvTop = new System.Windows.Forms.DataGridView();
            this.tabEmpleados = new System.Windows.Forms.TabPage();
            this.dgvEmpleados = new System.Windows.Forms.DataGridView();
            this.tabStock = new System.Windows.Forms.TabPage();
            this.dgvStock = new System.Windows.Forms.DataGridView();
            this.tabCategorias = new System.Windows.Forms.TabPage();
            this.dgvCategorias = new System.Windows.Forms.DataGridView();
            this.pnlFiltros.SuspendLayout();
            this.tabReportes.SuspendLayout();
            this.tabResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).BeginInit();
            this.tabTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).BeginInit();
            this.tabEmpleados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).BeginInit();
            this.tabStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).BeginInit();
            this.tabCategorias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblDesde);
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.lblHasta);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.btnGenerar);
            this.pnlFiltros.Controls.Add(this.btnExportar);
            this.pnlFiltros.Controls.Add(this.lblKpi);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 58);
            this.lblDesde.AutoSize = true; this.lblDesde.Location = new System.Drawing.Point(10, 6); this.lblDesde.Text = "Desde:";
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(12, 24); this.dtpDesde.Name = "dtpDesde"; this.dtpDesde.Size = new System.Drawing.Size(110, 23);
            this.lblHasta.AutoSize = true; this.lblHasta.Location = new System.Drawing.Point(132, 6); this.lblHasta.Text = "Hasta:";
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(134, 24); this.dtpHasta.Name = "dtpHasta"; this.dtpHasta.Size = new System.Drawing.Size(110, 23);
            this.btnGenerar.Location = new System.Drawing.Point(256, 22); this.btnGenerar.Name = "btnGenerar"; this.btnGenerar.Size = new System.Drawing.Size(90, 27); this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            this.btnExportar.Location = new System.Drawing.Point(352, 22); this.btnExportar.Name = "btnExportar"; this.btnExportar.Size = new System.Drawing.Size(110, 27); this.btnExportar.Text = "Exportar CSV…";
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.lblKpi.AutoSize = true; this.lblKpi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblKpi.Location = new System.Drawing.Point(478, 28); this.lblKpi.Name = "lblKpi";
            //
            // tabReportes
            //
            this.tabReportes.Controls.Add(this.tabResumen);
            this.tabReportes.Controls.Add(this.tabTop);
            this.tabReportes.Controls.Add(this.tabEmpleados);
            this.tabReportes.Controls.Add(this.tabStock);
            this.tabReportes.Controls.Add(this.tabCategorias);
            this.tabReportes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabReportes.Name = "tabReportes";
            this.tabResumen.Controls.Add(this.dgvResumen); this.tabResumen.Name = "tabResumen"; this.tabResumen.Text = "Resumen por periodo";
            this.dgvResumen.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvResumen.Name = "dgvResumen";
            this.tabTop.Controls.Add(this.dgvTop); this.tabTop.Name = "tabTop"; this.tabTop.Text = "Top 10 productos";
            this.dgvTop.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvTop.Name = "dgvTop";
            this.tabEmpleados.Controls.Add(this.dgvEmpleados); this.tabEmpleados.Name = "tabEmpleados"; this.tabEmpleados.Text = "Actividad por empleado";
            this.dgvEmpleados.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvEmpleados.Name = "dgvEmpleados";
            this.tabStock.Controls.Add(this.dgvStock); this.tabStock.Name = "tabStock"; this.tabStock.Text = "Stock bajo";
            this.dgvStock.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvStock.Name = "dgvStock";
            this.tabCategorias.Controls.Add(this.dgvCategorias); this.tabCategorias.Name = "tabCategorias"; this.tabCategorias.Text = "Ventas por categoría";
            this.dgvCategorias.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvCategorias.Name = "dgvCategorias";
            //
            // FrmReportes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.tabReportes);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmReportes";
            this.Text = "Reportes";
            this.Load += new System.EventHandler(this.FrmReportes_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.tabReportes.ResumeLayout(false);
            this.tabResumen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).EndInit();
            this.tabTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).EndInit();
            this.tabEmpleados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).EndInit();
            this.tabStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).EndInit();
            this.tabCategorias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label lblKpi;
        private System.Windows.Forms.TabControl tabReportes;
        private System.Windows.Forms.TabPage tabResumen;
        private System.Windows.Forms.DataGridView dgvResumen;
        private System.Windows.Forms.TabPage tabTop;
        private System.Windows.Forms.DataGridView dgvTop;
        private System.Windows.Forms.TabPage tabEmpleados;
        private System.Windows.Forms.DataGridView dgvEmpleados;
        private System.Windows.Forms.TabPage tabStock;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.TabPage tabCategorias;
        private System.Windows.Forms.DataGridView dgvCategorias;
    }
}
