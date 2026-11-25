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
            this.pnlScroll.SuspendLayout();
            this.tlpDashboard.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlScroll
            //
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.Controls.Add(this.tlpDashboard);
            this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroll.Name = "pnlScroll";
            //
            // tlpDashboard: fila 0 indicadores, fila 1 gráficas, fila 2 lo que requiere atención
            //
            this.tlpDashboard.ColumnCount = 4;
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpDashboard.Name = "tlpDashboard";
            this.tlpDashboard.RowCount = 3;
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tlpDashboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 268F));
            this.tlpDashboard.Size = new System.Drawing.Size(1000, 670);
            //
            // FrmDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.ClientSize = new System.Drawing.Size(1020, 700);
            this.Controls.Add(this.pnlScroll);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "FrmDashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.FrmDashboard_Load);
            this.pnlScroll.ResumeLayout(false);
            this.tlpDashboard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.TableLayoutPanel tlpDashboard;
    }
}
