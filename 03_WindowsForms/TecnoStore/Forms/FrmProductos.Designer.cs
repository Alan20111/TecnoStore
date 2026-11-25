namespace TecnoStore.Forms
{
    partial class FrmProductos
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
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.cboFiltroCategoria = new System.Windows.Forms.ComboBox();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.cboCondicion = new System.Windows.Forms.ComboBox();
            this.nudPrecio = new System.Windows.Forms.NumericUpDown();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.nudStockMinimo = new System.Windows.Forms.NumericUpDown();
            this.chkSerie = new System.Windows.Forms.CheckBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.lblStock = new System.Windows.Forms.Label();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.cboFiltroCategoria);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 56);
            this.txtBuscar.Location = new System.Drawing.Point(12, 24);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(220, 23);
            this.cboFiltroCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFiltroCategoria.Location = new System.Drawing.Point(246, 24);
            this.cboFiltroCategoria.Name = "cboFiltroCategoria";
            this.cboFiltroCategoria.Size = new System.Drawing.Size(190, 23);
            //
            // dgvProductos
            //
            this.dgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvProductos_RowPrePaint);
            this.txtSku.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSku.Location = new System.Drawing.Point(14, 42); this.txtSku.MaxLength = 30; this.txtSku.Name = "txtSku"; this.txtSku.Size = new System.Drawing.Size(140, 23);
            this.txtNombre.Location = new System.Drawing.Point(14, 90); this.txtNombre.MaxLength = 120; this.txtNombre.Name = "txtNombre"; this.txtNombre.Size = new System.Drawing.Size(286, 23);
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategoria.DisplayMember = "Texto"; this.cboCategoria.ValueMember = "Id";
            this.cboCategoria.Location = new System.Drawing.Point(14, 138); this.cboCategoria.Name = "cboCategoria"; this.cboCategoria.Size = new System.Drawing.Size(286, 23);
            this.cboMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca.DisplayMember = "Texto"; this.cboMarca.ValueMember = "Id";
            this.cboMarca.Location = new System.Drawing.Point(14, 186); this.cboMarca.Name = "cboMarca"; this.cboMarca.Size = new System.Drawing.Size(138, 23);
            this.cboCondicion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCondicion.DisplayMember = "Texto"; this.cboCondicion.ValueMember = "Id";
            this.cboCondicion.Location = new System.Drawing.Point(162, 186); this.cboCondicion.Name = "cboCondicion"; this.cboCondicion.Size = new System.Drawing.Size(138, 23);
            this.nudPrecio.DecimalPlaces = 2; this.nudPrecio.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudPrecio.ThousandsSeparator = true;
            this.nudPrecio.Location = new System.Drawing.Point(14, 234); this.nudPrecio.Name = "nudPrecio"; this.nudPrecio.Size = new System.Drawing.Size(138, 23);
            this.nudCosto.DecimalPlaces = 2; this.nudCosto.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudCosto.ThousandsSeparator = true;
            this.nudCosto.Location = new System.Drawing.Point(162, 234); this.nudCosto.Name = "nudCosto"; this.nudCosto.Size = new System.Drawing.Size(138, 23);
            this.nudStockMinimo.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudStockMinimo.Location = new System.Drawing.Point(14, 282); this.nudStockMinimo.Name = "nudStockMinimo"; this.nudStockMinimo.Size = new System.Drawing.Size(80, 23);
            this.chkSerie.AutoSize = true; this.chkSerie.Location = new System.Drawing.Point(14, 316); this.chkSerie.Name = "chkSerie"; this.chkSerie.Text = "Se controla por número de serie";
            this.chkActivo.AutoSize = true; this.chkActivo.Checked = true; this.chkActivo.Location = new System.Drawing.Point(14, 342); this.chkActivo.Name = "chkActivo"; this.chkActivo.Text = "Activo";
            this.lblStock.ForeColor = System.Drawing.SystemColors.GrayText; this.lblStock.Location = new System.Drawing.Point(12, 372); this.lblStock.Name = "lblStock"; this.lblStock.Size = new System.Drawing.Size(290, 36);
            //
            // botones
            //
            this.btnNuevo.Location = new System.Drawing.Point(10, 436); this.btnNuevo.Name = "btnNuevo"; this.btnNuevo.Size = new System.Drawing.Size(100, 28); this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            this.btnEditar.Location = new System.Drawing.Point(119, 436); this.btnEditar.Name = "btnEditar"; this.btnEditar.Size = new System.Drawing.Size(100, 28); this.btnEditar.Text = "Editar";
            this.btnEliminar.Location = new System.Drawing.Point(228, 436); this.btnEliminar.Name = "btnEliminar"; this.btnEliminar.Size = new System.Drawing.Size(100, 28); this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnGuardar.Location = new System.Drawing.Point(119, 470); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 470); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.errorProvider.ContainerControl = this;
            //
            // FrmProductos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvProductos);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmProductos";
            this.Text = "Productos";
            this.Load += new System.EventHandler(this.FrmProductos_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.ComboBox cboFiltroCategoria;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.ComboBox cboCondicion;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.NumericUpDown nudStockMinimo;
        private System.Windows.Forms.CheckBox chkSerie;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
