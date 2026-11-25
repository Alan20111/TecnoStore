namespace TecnoStore.Forms
{
    partial class FrmHistorialVentas
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
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnCancelarVenta = new System.Windows.Forms.Button();
            this.dgvVentas = new System.Windows.Forms.DataGridView();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.btnCancelarVenta);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 58);
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(12, 24); this.dtpDesde.Name = "dtpDesde"; this.dtpDesde.Size = new System.Drawing.Size(110, 23);
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(134, 24); this.dtpHasta.Name = "dtpHasta"; this.dtpHasta.Size = new System.Drawing.Size(110, 23);
            this.txtBuscar.Location = new System.Drawing.Point(388, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(180, 23);
            this.btnCancelarVenta.Location = new System.Drawing.Point(690, 22); this.btnCancelarVenta.Name = "btnCancelarVenta"; this.btnCancelarVenta.Size = new System.Drawing.Size(130, 27); this.btnCancelarVenta.Text = "Cancelar venta…";
            this.btnCancelarVenta.Click += new System.EventHandler(this.btnCancelarVenta_Click);
            //
            // dgvVentas
            //
            this.dgvVentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvVentas_RowPrePaint);
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            //
            // FrmHistorialVentas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvVentas);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmHistorialVentas";
            this.Text = "Historial de ventas";
            this.Load += new System.EventHandler(this.FrmHistorialVentas_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnCancelarVenta;
        private System.Windows.Forms.DataGridView dgvVentas;
        private System.Windows.Forms.DataGridView dgvDetalle;
    }
}
