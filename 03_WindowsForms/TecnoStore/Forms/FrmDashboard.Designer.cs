namespace TecnoStore.Forms
{
    partial class FrmDashboard
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
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.tlpDashboard = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSaludo = new System.Windows.Forms.Panel();
            this.lblSaludo = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblActualizado = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.pnlScroll.SuspendLayout();
            this.tlpDashboard.SuspendLayout();
            this.pnlSaludo.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlScroll
            //
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.Controls.Add(this.tlpDashboard);
            this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroll.Name = "pnlScroll";
            //
            // tlpDashboard: fila 0 saludo, filas 1-2 indicadores, fila 3 gráficas, filas 4-5 tablas
            //
            this.tlpDashboard.ColumnCount = 4;
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.Controls.Add(this.pnlSaludo, 0, 0);
            this.tlpDashboard.SetColumnSpan(this.pnlSaludo, 4);
            this.tlpDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpDashboard.Name = "tlpDashboard";
            this.tlpDashboard.RowCount = 6;
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tlpDashboard.Size = new System.Drawing.Size(1000, 1214);
            //
            // pnlSaludo
            //
            this.pnlSaludo.Controls.Add(this.lblSaludo);
            this.pnlSaludo.Controls.Add(this.lblFecha);
            this.pnlSaludo.Controls.Add(this.lblActualizado);
            this.pnlSaludo.Controls.Add(this.btnActualizar);
            this.pnlSaludo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaludo.Name = "pnlSaludo";
            this.pnlSaludo.Size = new System.Drawing.Size(994, 64);
            this.lblSaludo.AutoSize = true;
            this.lblSaludo.Font = new System.Drawing.Font("Segoe UI Semibold", 18F);
            this.lblSaludo.ForeColor = System.Drawing.Color.FromArgb(29, 29, 31);
            this.lblSaludo.Location = new System.Drawing.Point(4, 4);
            this.lblSaludo.Name = "lblSaludo";
            this.lblSaludo.Text = "Buenos días";
            this.lblFecha.AutoSize = true;
            this.lblFecha.ForeColor = System.Drawing.Color.FromArgb(110, 110, 115);
            this.lblFecha.Location = new System.Drawing.Point(7, 42);
            this.lblFecha.Name = "lblFecha";
            this.lblActualizado.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblActualizado.AutoSize = true;
            this.lblActualizado.ForeColor = System.Drawing.Color.FromArgb(142, 142, 147);
            this.lblActualizado.Location = new System.Drawing.Point(740, 24);
            this.lblActualizado.Name = "lblActualizado";
            this.btnActualizar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnActualizar.Location = new System.Drawing.Point(880, 16);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(110, 32);
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // FrmDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 247);
            this.ClientSize = new System.Drawing.Size(1020, 700);
            this.Controls.Add(this.pnlScroll);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "FrmDashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.FrmDashboard_Load);
            this.pnlScroll.ResumeLayout(false);
            this.tlpDashboard.ResumeLayout(false);
            this.pnlSaludo.ResumeLayout(false);
            this.pnlSaludo.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.TableLayoutPanel tlpDashboard;
        private System.Windows.Forms.Panel pnlSaludo;
        private System.Windows.Forms.Label lblSaludo;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblActualizado;
        private System.Windows.Forms.Button btnActualizar;
    }
}
