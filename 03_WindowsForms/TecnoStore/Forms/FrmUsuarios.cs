using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 14 · Usuarios (solo Administrador): alta, cambio de rol, contraseña
    /// y bloqueo con usp_UsuarioGuardar. Solo se guarda el hash de la contraseña.
    /// </summary>
    public partial class FrmUsuarios : Form, IRefrescable
    {
        private int? idActual;
        private readonly Ficha ficha;

        public FrmUsuarios()
        {
            InitializeComponent();
            Ui.Grid(dgvUsuarios);
            Ui.Mostrar(dgvUsuarios, "empleado", "usuario", "rol", "ultimo_acceso");

            btnNuevo.Text = "Nuevo usuario";
            btnEditar.Text = "Abrir";
            Sencillo.Vaciar(this);
            Sencillo.Armar(this, dgvUsuarios, Sencillo.Barra(new Panel(), new Control[0], btnNuevo, btnEditar));
            Sencillo.Abrir(dgvUsuarios, Abrir, btnEditar);

            chkActivo.Text = "Puede entrar al sistema";
            lblContrasenaAyuda.AutoSize = false;
            var campos = new PanelCampos(2, new Campo("Empleado *", cboEmpleado, true),
                new Campo("Usuario *", txtUsuario), new Campo("Rol *", cboRol),
                new Campo("Contraseña", txtContrasena), new Campo("Repetir contraseña", txtConfirmar),
                new Campo(null, lblContrasenaAyuda, true, 24), new Campo(null, chkActivo, true));
            ficha = new Ficha(640, 500, campos, btnGuardar, btnCancelar);
            Disposed += (s, e) => ficha.Dispose();
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                cboEmpleado.DataSource = UsuarioRepository.Empleados();
                cboRol.DataSource = UsuarioRepository.Roles();
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        public void Refrescar()
        {
            try { dgvUsuarios.DataSource = UsuarioRepository.Listar(); }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Abrir()
        {
            var r = dgvUsuarios.CurrentRow;
            idActual = (int)r.Cells["id_usuario"].Value;
            cboEmpleado.SelectedValue = r.Cells["id_empleado"].Value;
            txtUsuario.Text = Convert.ToString(r.Cells["usuario"].Value);
            if (r.Cells["id_rol"].Value != DBNull.Value) cboRol.SelectedValue = r.Cells["id_rol"].Value;
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            MostrarFicha(Convert.ToString(r.Cells["empleado"].Value), "Déjala vacía para conservar la actual.");
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtUsuario.Clear(); chkActivo.Checked = true;
            MostrarFicha("Nuevo usuario", "Mínimo 8 caracteres y un número.");
        }

        private void MostrarFicha(string titulo, string ayuda)
        {
            txtContrasena.Clear(); txtConfirmar.Clear();
            lblContrasenaAyuda.Text = ayuda;
            Ui.SinErrores(errorProvider, cboEmpleado, txtUsuario, cboRol, txtContrasena, txtConfirmar);
            ficha.Abrir(this, titulo);
        }

        private void dgvUsuarios_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvUsuarios.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.ForeColor = Tema.Ceniza;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(cboEmpleado, errorProvider, "Selecciona el empleado")
                    & Ui.Requerido(txtUsuario, errorProvider, "Falta el usuario")
                    & Ui.Requerido(cboRol, errorProvider, "Selecciona el rol");
            if (idActual == null) ok &= Ui.Requerido(txtContrasena, errorProvider, "Falta la contraseña");
            bool iguales = txtContrasena.Text == txtConfirmar.Text;
            errorProvider.SetError(txtConfirmar, iguales ? "" : "Las contraseñas no coinciden");
            if (!ok || !iguales) return;

            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                UsuarioRepository.Guardar(idActual, (int)cboEmpleado.SelectedValue, txtUsuario.Text,
                                          (int)cboRol.SelectedValue, txtContrasena.Text, chkActivo.Checked);
                ficha.Listo();
                Ui.Aviso(nuevo ? "Usuario creado" : "Cambios guardados");
                cboEmpleado.DataSource = UsuarioRepository.Empleados();
                Refrescar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); }
            finally { btnGuardar.Enabled = true; }
        }
    }
}
