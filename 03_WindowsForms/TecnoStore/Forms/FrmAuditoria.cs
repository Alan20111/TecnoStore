using System;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Auditoría (solo Supervisor y Administrador). Es de solo lectura:
    /// SQL Server la protege con DENY y con trg_Auditoria_SoloLectura.
    /// </summary>
    public partial class FrmAuditoria : Form
    {
        public FrmAuditoria()
        {
            InitializeComponent();
            Ui.Grid(dgvAuditoria);
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            cboTabla.SelectedIndex = 0;
        }

        private void FrmAuditoria_Load(object sender, EventArgs e) => Buscar();
        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();

        private void Buscar()
        {
            try
            {
                string tabla = cboTabla.SelectedIndex <= 0 ? null : cboTabla.Text;
                dgvAuditoria.DataSource = AuditoriaRepository.Buscar(dtpDesde.Value, dtpHasta.Value, tabla);
                lblConteo.Text = dgvAuditoria.Rows.Count + " movimiento(s)";
            }
            catch (Exception ex) { Ui.Error(ex); }
        }
    }
}
