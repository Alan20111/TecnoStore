using System;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;

namespace TecnoStore.Forms
{
    /// <summary>Módulo 2 · Menú principal (MDI): muestra solo las opciones del perfil.</summary>
    public partial class FrmPrincipal : Form
    {
        public bool CerrarSesion { get; private set; }

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            var s = Db.Sesion;
            Text = "TecnoStore — Sistema Integral de Gestión  [" + s.Rol + "]";
            lblUsuario.Text = "Usuario: " + s.NombreUsuario + " (" + s.Nombre + ")";
            lblRol.Text = "Rol: " + s.Rol;
            lblSucursal.Text = s.Sucursal;

            // Matriz de permisos aplicada al menú (SQL Server la vuelve a validar)
            mnuNuevaVenta.Visible = s.PuedeOperar;
            mnuOrdenes.Visible = s.PuedeOperar;
            mnuReportes.Visible = s.Rol != "Operador";
            mnuAuditoria.Visible = s.EsSupervisorOMas;
            mnuAjusteStock.Visible = s.EsSupervisorOMas;
            mnuAdministracion.Visible = s.EsSupervisorOMas;
        }

        /// <summary>Abre el formulario hijo o lo trae al frente si ya estaba abierto.</summary>
        private void Abrir<T>() where T : Form, new()
        {
            var abierto = MdiChildren.OfType<T>().FirstOrDefault();
            if (abierto != null) { abierto.Activate(); return; }
            var f = new T { MdiParent = this, WindowState = FormWindowState.Maximized };
            f.Show();
        }

        private void mnuProductos_Click(object sender, EventArgs e) => Abrir<FrmProductos>();
        private void mnuClientes_Click(object sender, EventArgs e) => Abrir<FrmClientes>();
        private void mnuNuevaVenta_Click(object sender, EventArgs e) => Abrir<FrmVenta>();
        private void mnuHistorial_Click(object sender, EventArgs e) => Abrir<FrmHistorialVentas>();
        private void mnuOrdenes_Click(object sender, EventArgs e) => Abrir<FrmOrdenes>();
        private void mnuReportes_Click(object sender, EventArgs e) => Abrir<FrmReportes>();
        private void mnuAuditoria_Click(object sender, EventArgs e) => Abrir<FrmAuditoria>();
        private void mnuAjusteStock_Click(object sender, EventArgs e) => Abrir<FrmAjusteStock>();

        private void mnuCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Deseas cerrar la sesión?", "Cerrar sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            CerrarSesion = true;
            Db.Sesion = null;
            Close();
        }

        private void mnuSalir_Click(object sender, EventArgs e) => Close();
    }
}
