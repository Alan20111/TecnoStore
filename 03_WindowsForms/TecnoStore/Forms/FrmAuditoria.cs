using System;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Auditoría (solo Supervisor y Administrador). Es de solo lectura:
    /// SQL Server la protege con DENY y con trg_Auditoria_SoloLectura.
    /// </summary>
    public partial class FrmAuditoria : Form, IRefrescable
    {
        private readonly Segmentos segPeriodo = new Segmentos("Hoy", "7 días", "30 días", "Todo");
        private readonly Segmentos segTabla = new Segmentos("Todo", "Productos", "Ventas");

        public FrmAuditoria()
        {
            InitializeComponent();
            Ui.Grid(dgvAuditoria);
            Ui.Mostrar(dgvAuditoria, "fecha", "usuario_app", "tabla=Qué cambió", "valor_anterior", "valor_nuevo", "motivo");
            segPeriodo.Activo = 2;
            segPeriodo.Elegido += i => Refrescar();
            segTabla.Elegido += i => Refrescar();
            Sencillo.Vaciar(this);
            Sencillo.Barra(pnlFiltros, new Control[] { segPeriodo, segTabla });
            Sencillo.Armar(this, dgvAuditoria, pnlFiltros);
        }

        private void FrmAuditoria_Load(object sender, EventArgs e) => Refrescar();

        public void Refrescar()
        {
            try
            {
                int dias = new[] { 0, 7, 30, 3650 }[segPeriodo.Activo];
                string tabla = new[] { null, "Producto", "Venta" }[segTabla.Activo];
                dgvAuditoria.DataSource = AuditoriaRepository.Buscar(DateTime.Today.AddDays(-dias), DateTime.Today, tabla);
            }
            catch (Exception ex) { Ui.Error(ex); }
        }
    }
}
