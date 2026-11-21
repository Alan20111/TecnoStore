using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 1 · Acceso: el usuario elige el servidor de SQL Server y se
    /// identifica con usp_Login. El último servidor usado se recuerda en
    /// %LOCALAPPDATA%\TecnoStore\servidor.txt.
    /// </summary>
    public partial class FrmLogin : Form
    {
        private static readonly string ArchivoServidor = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TecnoStore", "servidor.txt");

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            string guardado = null;
            try { if (File.Exists(ArchivoServidor)) guardado = File.ReadAllText(ArchivoServidor).Trim(); } catch { /* sin servidor guardado */ }
            txtServidor.Text = string.IsNullOrEmpty(guardado) ? Db.ServidorConfigurado() : guardado;
            txtUsuario.Select();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            bool ok = Ui.Requerido(txtServidor, errorProvider, "Captura el servidor (por ejemplo localhost o .\\SQLEXPRESS)")
                    & Ui.Requerido(txtUsuario, errorProvider, "Captura el usuario")
                    & Ui.Requerido(txtContrasena, errorProvider, "Captura la contraseña");
            if (!ok) return;

            btnIngresar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                Db.Servidor = txtServidor.Text.Trim();
                Db.Sesion = null;   // el login usa la conexión "Login"
                Db.Sesion = AuthRepository.Login(txtUsuario.Text.Trim(), txtContrasena.Text);
                GuardarServidor();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex) when (ex.Number < 50000)
            {
                // Errores de conexión: servidor equivocado, base no creada o sin permiso
                string detalle =
                    ex.Number == 4060 ? "La base de datos TecnoStore no existe en ese servidor. Ejecuten primero los scripts de 02_SQL." :
                    ex.Number == 18456 ? "SQL Server rechazó el inicio de sesión de Windows o del login configurado." :
                    "No se pudo conectar con el servidor \"" + txtServidor.Text.Trim() + "\".\n\n" +
                    "Revisen el nombre (localhost, .\\SQLEXPRESS o NOMBRE-PC\\INSTANCIA) y que el servicio de SQL Server esté iniciado.";
                MessageBox.Show(detalle + "\n\nDetalle técnico: " + ex.Message, "No se pudo conectar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtServidor.Focus();
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

        private void GuardarServidor()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArchivoServidor));
                File.WriteAllText(ArchivoServidor, txtServidor.Text.Trim());
            }
            catch { /* recordarlo es opcional */ }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
