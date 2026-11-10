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
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnCancelarVenta = new System.Windows.Forms.Button();
            this.lblConteo = new System.Windows.Forms.Label();
            this.dgvVentas = new System.Windows.Forms.DataGridView();
            this.grpDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            this.grpDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblDesde);
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.lblHasta);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.lblEstado);
            this.pnlFiltros.Controls.Add(this.cboEstado);
            this.pnlFiltros.Controls.Add(this.lblBuscar);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.btnBuscar);
            this.pnlFiltros.Controls.Add(this.btnCancelarVenta);
            this.pnlFiltros.Controls.Add(this.lblConteo);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 58);
            this.lblDesde.AutoSize = true; this.lblDesde.Location = new System.Drawing.Point(10, 6); this.lblDesde.Text = "Desde:";
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(12, 24); this.dtpDesde.Name = "dtpDesde"; this.dtpDesde.Size = new System.Drawing.Size(110, 23);
            this.lblHasta.AutoSize = true; this.lblHasta.Location = new System.Drawing.Point(132, 6); this.lblHasta.Text = "Hasta:";
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(134, 24); this.dtpHasta.Name = "dtpHasta"; this.dtpHasta.Size = new System.Drawing.Size(110, 23);
            this.lblEstado.AutoSize = true; this.lblEstado.Location = new System.Drawing.Point(254, 6); this.lblEstado.Text = "Estado:";
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Items.AddRange(new object[] { "(Todos)", "Completada", "Cancelada" });
            this.cboEstado.Location = new System.Drawing.Point(256, 24); this.cboEstado.Name = "cboEstado"; this.cboEstado.Size = new System.Drawing.Size(120, 23);
            this.lblBuscar.AutoSize = true; this.lblBuscar.Location = new System.Drawing.Point(386, 6); this.lblBuscar.Text = "Folio o cliente:";
            this.txtBuscar.Location = new System.Drawing.Point(388, 24); this.txtBuscar.Name = "txtBuscar"; this.txtBuscar.Size = new System.Drawing.Size(180, 23);
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            this.btnBuscar.Location = new System.Drawing.Point(578, 22); this.btnBuscar.Name = "btnBuscar"; this.btnBuscar.Size = new System.Drawing.Size(90, 27); this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.btnCancelarVenta.Location = new System.Drawing.Point(690, 22); this.btnCancelarVenta.Name = "btnCancelarVenta"; this.btnCancelarVenta.Size = new System.Drawing.Size(130, 27); this.btnCancelarVenta.Text = "Cancelar venta…";
            this.btnCancelarVenta.Click += new System.EventHandler(this.btnCancelarVenta_Click);
            this.lblConteo.AutoSize = true; this.lblConteo.ForeColor = System.Drawing.SystemColors.GrayText; this.lblConteo.Location = new System.Drawing.Point(832, 28); this.lblConteo.Name = "lblConteo";
            //
            // dgvVentas
            //
            this.dgvVentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.SelectionChanged += new System.EventHandler(this.dgvVentas_SelectionChanged);
            this.dgvVentas.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvVentas_RowPrePaint);
            //
            // grpDetalle
            //
            this.grpDetalle.Controls.Add(this.dgvDetalle);
            this.grpDetalle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpDetalle.Name = "grpDetalle";
            this.grpDetalle.Size = new System.Drawing.Size(1100, 200);
            this.grpDetalle.Text = "Detalle de la venta seleccionada";
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            //
            // FrmHistorialVentas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvVentas);
            this.Controls.Add(this.grpDetalle);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmHistorialVentas";
            this.Text = "Historial de ventas";
            this.Load += new System.EventHandler(this.FrmHistorialVentas_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            this.grpDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnCancelarVenta;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.DataGridView dgvVentas;
        private System.Windows.Forms.GroupBox grpDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
    }
}
