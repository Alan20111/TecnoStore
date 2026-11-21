using System;
using System.Drawing;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 14 · Administración de usuarios (solo Administrador): alta, cambio de
    /// rol, cambio de contraseña y bloqueo/desbloqueo con usp_UsuarioGuardar.
    /// Las contraseñas nunca se leen: solo se guarda su hash.
    /// </summary>
    public partial class FrmUsuarios : Form
    {
        private int? idActual;
        private bool editando;

        public FrmUsuarios()
        {
            InitializeComponent();
            Ui.Grid(dgvUsuarios);
            Ui.Mostrar(dgvUsuarios, "usuario", "empleado", "rol", "activo", "ultimo_acceso");
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                cboEmpleado.DataSource = UsuarioRepository.Empleados();
                cboRol.DataSource = UsuarioRepository.Roles();
                Cargar();
                Estado(false);
            }
            catch (Exception ex) { Ui.Error(ex); }
        }

        private void Cargar() => dgvUsuarios.DataSource = UsuarioRepository.Listar();

        private void Estado(bool edicion)
        {
            editando = edicion;
            grpDatos.Enabled = edicion;
            btnNuevo.Enabled = !edicion;
            btnEditar.Enabled = !edicion && idActual != null;
            btnGuardar.Enabled = edicion;
            btnCancelar.Enabled = edicion;
            dgvUsuarios.Enabled = !edicion;
            lblContrasenaAyuda.Text = idActual == null ? "Obligatoria: 8+ caracteres y un número." : "Déjala vacía para conservar la actual.";
            grpDatos.Text = !edicion ? "Usuario (lectura)" : idActual == null ? "Nuevo usuario" : "Editando usuario";
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (editando || dgvUsuarios.CurrentRow == null) return;
            var r = dgvUsuarios.CurrentRow;
            idActual = (int)r.Cells["id_usuario"].Value;
            cboEmpleado.SelectedValue = r.Cells["id_empleado"].Value;
            txtUsuario.Text = Convert.ToString(r.Cells["usuario"].Value);
            if (r.Cells["id_rol"].Value != DBNull.Value) cboRol.SelectedValue = r.Cells["id_rol"].Value;
            chkActivo.Checked = (bool)r.Cells["activo"].Value;
            txtContrasena.Clear(); txtConfirmar.Clear();
            Estado(false);
        }

        private void dgvUsuarios_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvUsuarios.Rows[e.RowIndex];
            if (!(bool)row.Cells["activo"].Value) row.DefaultCellStyle.BackColor = Color.MistyRose;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idActual = null;
            txtUsuario.Clear(); txtContrasena.Clear(); txtConfirmar.Clear(); chkActivo.Checked = true;
            Estado(true);
            cboEmpleado.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e) { Estado(true); cboRol.Focus(); }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            Estado(false);
            dgvUsuarios_SelectionChanged(null, null);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(cboEmpleado, errorProvider, "Selecciona el empleado")
                    & Ui.Requerido(txtUsuario, errorProvider, "Captura el usuario")
                    & Ui.Requerido(cboRol, errorProvider, "Selecciona el rol");
            if (idActual == null) ok &= Ui.Requerido(txtContrasena, errorProvider, "La contraseña es obligatoria");
            if (txtContrasena.Text != txtConfirmar.Text) { errorProvider.SetError(txtConfirmar, "Las contraseñas no coinciden"); ok = false; }
            if (!ok) return;

            btnGuardar.Enabled = false;
            try
            {
                bool nuevo = idActual == null;
                UsuarioRepository.Guardar(idActual, (int)cboEmpleado.SelectedValue, txtUsuario.Text,
                                          (int)cboRol.SelectedValue, txtContrasena.Text, chkActivo.Checked);
                Ui.Info(nuevo ? "Usuario creado." : "Cambios guardados.");
                txtContrasena.Clear(); txtConfirmar.Clear();
                Estado(false);
                cboEmpleado.DataSource = UsuarioRepository.Empleados();
                Cargar();
            }
            catch (Exception ex) { Ui.Error(ex, "No se guardó"); btnGuardar.Enabled = true; }
        }
    }
}
