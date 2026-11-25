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
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnExportar = new System.Windows.Forms.Button();
            this.lblKpi = new System.Windows.Forms.Label();
            this.dgvResumen = new System.Windows.Forms.DataGridView();
            this.dgvTop = new System.Windows.Forms.DataGridView();
            this.dgvEmpleados = new System.Windows.Forms.DataGridView();
            this.dgvStock = new System.Windows.Forms.DataGridView();
            this.dgvCategorias = new System.Windows.Forms.DataGridView();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.btnExportar);
            this.pnlFiltros.Controls.Add(this.lblKpi);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 58);
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(12, 24); this.dtpDesde.Name = "dtpDesde"; this.dtpDesde.Size = new System.Drawing.Size(110, 23);
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(134, 24); this.dtpHasta.Name = "dtpHasta"; this.dtpHasta.Size = new System.Drawing.Size(110, 23);
            this.btnExportar.Location = new System.Drawing.Point(352, 22); this.btnExportar.Name = "btnExportar"; this.btnExportar.Size = new System.Drawing.Size(110, 27); this.btnExportar.Text = "Exportar CSV…";
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.lblKpi.AutoSize = true; this.lblKpi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblKpi.Location = new System.Drawing.Point(478, 28); this.lblKpi.Name = "lblKpi";
            this.dgvResumen.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvResumen.Name = "dgvResumen";
            this.dgvTop.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvTop.Name = "dgvTop";
            this.dgvEmpleados.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvEmpleados.Name = "dgvEmpleados";
            this.dgvStock.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvStock.Name = "dgvStock";
            this.dgvCategorias.Dock = System.Windows.Forms.DockStyle.Fill; this.dgvCategorias.Name = "dgvCategorias";
            //
            // FrmReportes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmReportes";
            this.Text = "Reportes";
            this.Load += new System.EventHandler(this.FrmReportes_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label lblKpi;
        private System.Windows.Forms.DataGridView dgvResumen;
        private System.Windows.Forms.DataGridView dgvTop;
        private System.Windows.Forms.DataGridView dgvEmpleados;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.DataGridView dgvCategorias;
    }
}
