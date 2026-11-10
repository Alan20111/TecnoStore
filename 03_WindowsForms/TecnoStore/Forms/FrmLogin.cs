using System;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>Módulo 1 · Acceso: identifica al usuario y su perfil con usp_Login.</summary>
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtUsuario, errorProvider, "Captura el usuario")
                    & Ui.Requerido(txtContrasena, errorProvider, "Captura la contraseña");
            if (!ok) return;

            btnIngresar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                Db.Sesion = null;   // el login usa la conexión "Login"
                Db.Sesion = AuthRepository.Login(txtUsuario.Text.Trim(), txtContrasena.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                txtContrasena.Clear();
                txtContrasena.Focus();
                Ui.Error(ex, "No se pudo iniciar sesión");
            }
            finally
            {
                btnIngresar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
