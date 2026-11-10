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
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblFiltroCategoria = new System.Windows.Forms.Label();
            this.cboFiltroCategoria = new System.Windows.Forms.ComboBox();
            this.chkAlertas = new System.Windows.Forms.CheckBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblConteo = new System.Windows.Forms.Label();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.pnlDerecho = new System.Windows.Forms.Panel();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblSku = new System.Windows.Forms.Label();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.lblMarca = new System.Windows.Forms.Label();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.lblCondicion = new System.Windows.Forms.Label();
            this.cboCondicion = new System.Windows.Forms.ComboBox();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.nudPrecio = new System.Windows.Forms.NumericUpDown();
            this.lblCosto = new System.Windows.Forms.Label();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.lblStockMinimo = new System.Windows.Forms.Label();
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
            this.pnlDerecho.SuspendLayout();
            this.grpDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblBuscar);
            this.pnlFiltros.Controls.Add(this.txtBuscar);
            this.pnlFiltros.Controls.Add(this.lblFiltroCategoria);
            this.pnlFiltros.Controls.Add(this.cboFiltroCategoria);
            this.pnlFiltros.Controls.Add(this.chkAlertas);
            this.pnlFiltros.Controls.Add(this.btnBuscar);
            this.pnlFiltros.Controls.Add(this.lblConteo);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1100, 56);
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new System.Drawing.Point(10, 6);
            this.lblBuscar.Text = "Buscar (nombre o SKU):";
            this.txtBuscar.Location = new System.Drawing.Point(12, 24);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(220, 23);
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            this.lblFiltroCategoria.AutoSize = true;
            this.lblFiltroCategoria.Location = new System.Drawing.Point(244, 6);
            this.lblFiltroCategoria.Text = "Categoría:";
            this.cboFiltroCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFiltroCategoria.Location = new System.Drawing.Point(246, 24);
            this.cboFiltroCategoria.Name = "cboFiltroCategoria";
            this.cboFiltroCategoria.Size = new System.Drawing.Size(190, 23);
            this.chkAlertas.AutoSize = true;
            this.chkAlertas.Location = new System.Drawing.Point(450, 26);
            this.chkAlertas.Name = "chkAlertas";
            this.chkAlertas.Text = "Solo con alerta de stock";
            this.btnBuscar.Location = new System.Drawing.Point(620, 22);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 27);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.lblConteo.AutoSize = true;
            this.lblConteo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblConteo.Location = new System.Drawing.Point(722, 28);
            this.lblConteo.Name = "lblConteo";
            this.lblConteo.Text = "";
            //
            // dgvProductos
            //
            this.dgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.SelectionChanged += new System.EventHandler(this.dgvProductos_SelectionChanged);
            this.dgvProductos.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvProductos_RowPrePaint);
            //
            // pnlDerecho
            //
            this.pnlDerecho.Controls.Add(this.grpDatos);
            this.pnlDerecho.Controls.Add(this.btnNuevo);
            this.pnlDerecho.Controls.Add(this.btnEditar);
            this.pnlDerecho.Controls.Add(this.btnGuardar);
            this.pnlDerecho.Controls.Add(this.btnCancelar);
            this.pnlDerecho.Controls.Add(this.btnEliminar);
            this.pnlDerecho.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDerecho.Name = "pnlDerecho";
            this.pnlDerecho.Padding = new System.Windows.Forms.Padding(8);
            this.pnlDerecho.Size = new System.Drawing.Size(340, 600);
            //
            // grpDatos
            //
            this.grpDatos.Controls.Add(this.lblSku);
            this.grpDatos.Controls.Add(this.txtSku);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.lblCategoria);
            this.grpDatos.Controls.Add(this.cboCategoria);
            this.grpDatos.Controls.Add(this.lblMarca);
            this.grpDatos.Controls.Add(this.cboMarca);
            this.grpDatos.Controls.Add(this.lblCondicion);
            this.grpDatos.Controls.Add(this.cboCondicion);
            this.grpDatos.Controls.Add(this.lblPrecio);
            this.grpDatos.Controls.Add(this.nudPrecio);
            this.grpDatos.Controls.Add(this.lblCosto);
            this.grpDatos.Controls.Add(this.nudCosto);
            this.grpDatos.Controls.Add(this.lblStockMinimo);
            this.grpDatos.Controls.Add(this.nudStockMinimo);
            this.grpDatos.Controls.Add(this.chkSerie);
            this.grpDatos.Controls.Add(this.chkActivo);
            this.grpDatos.Controls.Add(this.lblStock);
            this.grpDatos.Location = new System.Drawing.Point(10, 6);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(318, 420);
            this.grpDatos.Text = "Datos del producto";
            this.lblSku.AutoSize = true; this.lblSku.Location = new System.Drawing.Point(12, 24); this.lblSku.Text = "SKU: *";
            this.txtSku.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSku.Location = new System.Drawing.Point(14, 42); this.txtSku.MaxLength = 30; this.txtSku.Name = "txtSku"; this.txtSku.Size = new System.Drawing.Size(140, 23);
            this.lblNombre.AutoSize = true; this.lblNombre.Location = new System.Drawing.Point(12, 72); this.lblNombre.Text = "Nombre: *";
            this.txtNombre.Location = new System.Drawing.Point(14, 90); this.txtNombre.MaxLength = 120; this.txtNombre.Name = "txtNombre"; this.txtNombre.Size = new System.Drawing.Size(286, 23);
            this.lblCategoria.AutoSize = true; this.lblCategoria.Location = new System.Drawing.Point(12, 120); this.lblCategoria.Text = "Categoría:";
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategoria.DisplayMember = "Texto"; this.cboCategoria.ValueMember = "Id";
            this.cboCategoria.Location = new System.Drawing.Point(14, 138); this.cboCategoria.Name = "cboCategoria"; this.cboCategoria.Size = new System.Drawing.Size(286, 23);
            this.lblMarca.AutoSize = true; this.lblMarca.Location = new System.Drawing.Point(12, 168); this.lblMarca.Text = "Marca:";
            this.cboMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca.DisplayMember = "Texto"; this.cboMarca.ValueMember = "Id";
            this.cboMarca.Location = new System.Drawing.Point(14, 186); this.cboMarca.Name = "cboMarca"; this.cboMarca.Size = new System.Drawing.Size(138, 23);
            this.lblCondicion.AutoSize = true; this.lblCondicion.Location = new System.Drawing.Point(160, 168); this.lblCondicion.Text = "Condición:";
            this.cboCondicion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCondicion.DisplayMember = "Texto"; this.cboCondicion.ValueMember = "Id";
            this.cboCondicion.Location = new System.Drawing.Point(162, 186); this.cboCondicion.Name = "cboCondicion"; this.cboCondicion.Size = new System.Drawing.Size(138, 23);
            this.lblPrecio.AutoSize = true; this.lblPrecio.Location = new System.Drawing.Point(12, 216); this.lblPrecio.Text = "Precio de venta: *";
            this.nudPrecio.DecimalPlaces = 2; this.nudPrecio.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudPrecio.ThousandsSeparator = true;
            this.nudPrecio.Location = new System.Drawing.Point(14, 234); this.nudPrecio.Name = "nudPrecio"; this.nudPrecio.Size = new System.Drawing.Size(138, 23);
            this.lblCosto.AutoSize = true; this.lblCosto.Location = new System.Drawing.Point(160, 216); this.lblCosto.Text = "Costo:";
            this.nudCosto.DecimalPlaces = 2; this.nudCosto.Maximum = new decimal(new int[] { 999999, 0, 0, 0 }); this.nudCosto.ThousandsSeparator = true;
            this.nudCosto.Location = new System.Drawing.Point(162, 234); this.nudCosto.Name = "nudCosto"; this.nudCosto.Size = new System.Drawing.Size(138, 23);
            this.lblStockMinimo.AutoSize = true; this.lblStockMinimo.Location = new System.Drawing.Point(12, 264); this.lblStockMinimo.Text = "Stock mínimo:";
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
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            this.btnEliminar.Location = new System.Drawing.Point(228, 436); this.btnEliminar.Name = "btnEliminar"; this.btnEliminar.Size = new System.Drawing.Size(100, 28); this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnGuardar.Location = new System.Drawing.Point(119, 470); this.btnGuardar.Name = "btnGuardar"; this.btnGuardar.Size = new System.Drawing.Size(100, 28); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(228, 470); this.btnCancelar.Name = "btnCancelar"; this.btnCancelar.Size = new System.Drawing.Size(100, 28); this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.errorProvider.ContainerControl = this;
            //
            // FrmProductos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvProductos);
            this.Controls.Add(this.pnlDerecho);
            this.Controls.Add(this.pnlFiltros);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmProductos";
            this.Text = "Productos";
            this.Load += new System.EventHandler(this.FrmProductos_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.pnlDerecho.ResumeLayout(false);
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblFiltroCategoria;
        private System.Windows.Forms.ComboBox cboFiltroCategoria;
        private System.Windows.Forms.CheckBox chkAlertas;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Panel pnlDerecho;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.Label lblCondicion;
        private System.Windows.Forms.ComboBox cboCondicion;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.Label lblCosto;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.Label lblStockMinimo;
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
