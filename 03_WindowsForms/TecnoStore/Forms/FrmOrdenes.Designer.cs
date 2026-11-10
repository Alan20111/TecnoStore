namespace TecnoStore.Forms
{
    partial class FrmOrdenes
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
            this.pnlArriba = new System.Windows.Forms.Panel();
            this.chkAbiertas = new System.Windows.Forms.CheckBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.pnlDerecho = new System.Windows.Forms.Panel();
            this.grpNueva = new System.Windows.Forms.GroupBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.cboTecnico = new System.Windows.Forms.ComboBox();
            this.lblEquipo = new System.Windows.Forms.Label();
            this.txtEquipo = new System.Windows.Forms.TextBox();
            this.lblSerie = new System.Windows.Forms.Label();
            this.txtSerie = new System.Windows.Forms.TextBox();
            this.lblFalla = new System.Windows.Forms.Label();
            this.txtFalla = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.lblServicio = new System.Windows.Forms.Label();
            this.cboServicio = new System.Windows.Forms.ComboBox();
            this.btnAgregarServicio = new System.Windows.Forms.Button();
            this.lblRefaccion = new System.Windows.Forms.Label();
            this.cboRefaccion = new System.Windows.Forms.ComboBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregarRefaccion = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.btnCambiarEstado = new System.Windows.Forms.Button();
            this.tabAbajo = new System.Windows.Forms.TabControl();
            this.tabDetalle = new System.Windows.Forms.TabPage();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.tabHistorial = new System.Windows.Forms.TabPage();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlArriba.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.pnlDerecho.SuspendLayout();
            this.grpNueva.SuspendLayout();
            this.grpAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.tabAbajo.SuspendLayout();
            this.tabDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.tabHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // pnlArriba
            //
            this.pnlArriba.Controls.Add(this.chkAbiertas);
            this.pnlArriba.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlArriba.Name = "pnlArriba";
            this.pnlArriba.Size = new System.Drawing.Size(1100, 34);
            this.chkAbiertas.AutoSize = true; this.chkAbiertas.Checked = true; this.chkAbiertas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAbiertas.Location = new System.Drawing.Point(12, 8); this.chkAbiertas.Name = "chkAbiertas"; this.chkAbiertas.Text = "Solo órdenes abiertas";
            this.chkAbiertas.CheckedChanged += new System.EventHandler(this.chkAbiertas_CheckedChanged);
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.SelectionChanged += new System.EventHandler(this.dgvOrdenes_SelectionChanged);
            this.dgvOrdenes.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvOrdenes_RowPrePaint);
            //
            // pnlDerecho
            //
            this.pnlDerecho.Controls.Add(this.grpAcciones);
            this.pnlDerecho.Controls.Add(this.grpNueva);
            this.pnlDerecho.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlDerecho.Name = "pnlDerecho";
            this.pnlDerecho.Size = new System.Drawing.Size(340, 620);
            //
            // grpNueva
            //
            this.grpNueva.Controls.Add(this.lblCliente);
            this.grpNueva.Controls.Add(this.cboCliente);
            this.grpNueva.Controls.Add(this.lblTecnico);
            this.grpNueva.Controls.Add(this.cboTecnico);
            this.grpNueva.Controls.Add(this.lblEquipo);
            this.grpNueva.Controls.Add(this.txtEquipo);
            this.grpNueva.Controls.Add(this.lblSerie);
            this.grpNueva.Controls.Add(this.txtSerie);
            this.grpNueva.Controls.Add(this.lblFalla);
            this.grpNueva.Controls.Add(this.txtFalla);
            this.grpNueva.Controls.Add(this.btnRegistrar);
            this.grpNueva.Location = new System.Drawing.Point(8, 4);
            this.grpNueva.Name = "grpNueva";
            this.grpNueva.Size = new System.Drawing.Size(322, 300);
            this.grpNueva.Text = "Recibir equipo (nueva orden)";
            this.lblCliente.AutoSize = true; this.lblCliente.Location = new System.Drawing.Point(12, 22); this.lblCliente.Text = "Cliente: *";
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboCliente.DisplayMember = "Texto"; this.cboCliente.ValueMember = "Id";
            this.cboCliente.Location = new System.Drawing.Point(14, 40); this.cboCliente.Name = "cboCliente"; this.cboCliente.Size = new System.Drawing.Size(292, 23);
            this.lblTecnico.AutoSize = true; this.lblTecnico.Location = new System.Drawing.Point(12, 68); this.lblTecnico.Text = "Técnico: *";
            this.cboTecnico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboTecnico.DisplayMember = "Texto"; this.cboTecnico.ValueMember = "Id";
            this.cboTecnico.Location = new System.Drawing.Point(14, 86); this.cboTecnico.Name = "cboTecnico"; this.cboTecnico.Size = new System.Drawing.Size(292, 23);
            this.lblEquipo.AutoSize = true; this.lblEquipo.Location = new System.Drawing.Point(12, 114); this.lblEquipo.Text = "Equipo: *";
            this.txtEquipo.Location = new System.Drawing.Point(14, 132); this.txtEquipo.MaxLength = 150; this.txtEquipo.Name = "txtEquipo"; this.txtEquipo.Size = new System.Drawing.Size(180, 23);
            this.lblSerie.AutoSize = true; this.lblSerie.Location = new System.Drawing.Point(200, 114); this.lblSerie.Text = "No. de serie:";
            this.txtSerie.Location = new System.Drawing.Point(202, 132); this.txtSerie.MaxLength = 50; this.txtSerie.Name = "txtSerie"; this.txtSerie.Size = new System.Drawing.Size(104, 23);
            this.lblFalla.AutoSize = true; this.lblFalla.Location = new System.Drawing.Point(12, 160); this.lblFalla.Text = "Falla reportada: *";
            this.txtFalla.Location = new System.Drawing.Point(14, 178); this.txtFalla.MaxLength = 500; this.txtFalla.Multiline = true; this.txtFalla.Name = "txtFalla"; this.txtFalla.Size = new System.Drawing.Size(292, 70);
            this.btnRegistrar.Location = new System.Drawing.Point(186, 258); this.btnRegistrar.Name = "btnRegistrar"; this.btnRegistrar.Size = new System.Drawing.Size(120, 28); this.btnRegistrar.Text = "Registrar orden";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // grpAcciones
            //
            this.grpAcciones.Controls.Add(this.lblServicio);
            this.grpAcciones.Controls.Add(this.cboServicio);
            this.grpAcciones.Controls.Add(this.btnAgregarServicio);
            this.grpAcciones.Controls.Add(this.lblRefaccion);
            this.grpAcciones.Controls.Add(this.cboRefaccion);
            this.grpAcciones.Controls.Add(this.nudCantidad);
            this.grpAcciones.Controls.Add(this.btnAgregarRefaccion);
            this.grpAcciones.Controls.Add(this.lblEstado);
            this.grpAcciones.Controls.Add(this.cboEstado);
            this.grpAcciones.Controls.Add(this.btnCambiarEstado);
            this.grpAcciones.Enabled = false;
            this.grpAcciones.Location = new System.Drawing.Point(8, 310);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(322, 220);
            this.grpAcciones.Text = "Orden seleccionada";
            this.lblServicio.AutoSize = true; this.lblServicio.Location = new System.Drawing.Point(12, 22); this.lblServicio.Text = "Mano de obra:";
            this.cboServicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboServicio.DisplayMember = "Texto"; this.cboServicio.ValueMember = "Id";
            this.cboServicio.Location = new System.Drawing.Point(14, 40); this.cboServicio.Name = "cboServicio"; this.cboServicio.Size = new System.Drawing.Size(210, 23);
            this.btnAgregarServicio.Location = new System.Drawing.Point(230, 39); this.btnAgregarServicio.Name = "btnAgregarServicio"; this.btnAgregarServicio.Size = new System.Drawing.Size(76, 25); this.btnAgregarServicio.Text = "Agregar";
            this.btnAgregarServicio.Click += new System.EventHandler(this.btnAgregarServicio_Click);
            this.lblRefaccion.AutoSize = true; this.lblRefaccion.Location = new System.Drawing.Point(12, 70); this.lblRefaccion.Text = "Refacción y cantidad:";
            this.cboRefaccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboRefaccion.DisplayMember = "Texto"; this.cboRefaccion.ValueMember = "Id";
            this.cboRefaccion.Location = new System.Drawing.Point(14, 88); this.cboRefaccion.Name = "cboRefaccion"; this.cboRefaccion.Size = new System.Drawing.Size(160, 23);
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 }); this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Location = new System.Drawing.Point(180, 88); this.nudCantidad.Name = "nudCantidad"; this.nudCantidad.Size = new System.Drawing.Size(44, 23);
            this.btnAgregarRefaccion.Location = new System.Drawing.Point(230, 87); this.btnAgregarRefaccion.Name = "btnAgregarRefaccion"; this.btnAgregarRefaccion.Size = new System.Drawing.Size(76, 25); this.btnAgregarRefaccion.Text = "Agregar";
            this.btnAgregarRefaccion.Click += new System.EventHandler(this.btnAgregarRefaccion_Click);
            this.lblEstado.AutoSize = true; this.lblEstado.Location = new System.Drawing.Point(12, 124); this.lblEstado.Text = "Cambiar a estado:";
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cboEstado.DisplayMember = "Texto"; this.cboEstado.ValueMember = "Id";
            this.cboEstado.Location = new System.Drawing.Point(14, 142); this.cboEstado.Name = "cboEstado"; this.cboEstado.Size = new System.Drawing.Size(210, 23);
            this.btnCambiarEstado.Location = new System.Drawing.Point(230, 141); this.btnCambiarEstado.Name = "btnCambiarEstado"; this.btnCambiarEstado.Size = new System.Drawing.Size(76, 25); this.btnCambiarEstado.Text = "Aplicar";
            this.btnCambiarEstado.Click += new System.EventHandler(this.btnCambiarEstado_Click);
            //
            // tabAbajo
            //
            this.tabAbajo.Controls.Add(this.tabDetalle);
            this.tabAbajo.Controls.Add(this.tabHistorial);
            this.tabAbajo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabAbajo.Name = "tabAbajo";
            this.tabAbajo.Size = new System.Drawing.Size(760, 210);
            this.tabDetalle.Controls.Add(this.dgvDetalle);
            this.tabDetalle.Name = "tabDetalle";
            this.tabDetalle.Text = "Detalle (mano de obra y refacciones)";
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Name = "dgvDetalle";
            this.tabHistorial.Controls.Add(this.dgvHistorial);
            this.tabHistorial.Name = "tabHistorial";
            this.tabHistorial.Text = "Historial de estados";
            this.dgvHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistorial.Name = "dgvHistorial";
            this.errorProvider.ContainerControl = this;
            //
            // FrmOrdenes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 656);
            this.Controls.Add(this.dgvOrdenes);
            this.Controls.Add(this.tabAbajo);
            this.Controls.Add(this.pnlDerecho);
            this.Controls.Add(this.pnlArriba);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmOrdenes";
            this.Text = "Órdenes de servicio";
            this.Load += new System.EventHandler(this.FrmOrdenes_Load);
            this.pnlArriba.ResumeLayout(false);
            this.pnlArriba.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.pnlDerecho.ResumeLayout(false);
            this.grpNueva.ResumeLayout(false);
            this.grpNueva.PerformLayout();
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.tabAbajo.ResumeLayout(false);
            this.tabDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.tabHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlArriba;
        private System.Windows.Forms.CheckBox chkAbiertas;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.Panel pnlDerecho;
        private System.Windows.Forms.GroupBox grpNueva;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.ComboBox cboTecnico;
        private System.Windows.Forms.Label lblEquipo;
        private System.Windows.Forms.TextBox txtEquipo;
        private System.Windows.Forms.Label lblSerie;
        private System.Windows.Forms.TextBox txtSerie;
        private System.Windows.Forms.Label lblFalla;
        private System.Windows.Forms.TextBox txtFalla;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.Label lblServicio;
        private System.Windows.Forms.ComboBox cboServicio;
        private System.Windows.Forms.Button btnAgregarServicio;
        private System.Windows.Forms.Label lblRefaccion;
        private System.Windows.Forms.ComboBox cboRefaccion;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregarRefaccion;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Button btnCambiarEstado;
        private System.Windows.Forms.TabControl tabAbajo;
        private System.Windows.Forms.TabPage tabDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.TabPage tabHistorial;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
