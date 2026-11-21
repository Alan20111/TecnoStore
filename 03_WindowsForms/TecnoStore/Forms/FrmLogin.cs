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
            catch (SqlException ex) when (ex.Number == 4060)
            {
                // Sí hay servidor, pero no se pudo abrir la base TecnoStore
                ManejarBaseFaltante(ex);
            }
            catch (SqlException ex) when (ex.Number < 50000)
            {
                // Errores de conexión: servidor equivocado, servicio apagado o login rechazado
                string detalle =
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

        /// <summary>
        /// Error 4060: o la base no existe en este servidor, o existe y el usuario
        /// de Windows no tiene acceso. Solo en el primer caso se ofrece crearla.
        /// </summary>
        private void ManejarBaseFaltante(SqlException ex)
        {
            string servidor = txtServidor.Text.Trim();
            bool existe;
            try { existe = InstaladorBase.Existe(servidor); }
            catch (Exception ex2) { Ui.Error(ex2, "No se pudo revisar el servidor"); return; }

            if (existe)
            {
                MessageBox.Show(
                    "La base TecnoStore existe en \"" + servidor + "\", pero tu usuario de Windows (" + Environment.UserDomainName + "\\" +
                    Environment.UserName + ") no tiene acceso a ella.\n\n" +
                    "Ábranla en SSMS con el usuario que la creó y den acceso a este usuario, o creen la base con este usuario.\n\n" +
                    "Detalle técnico: " + ex.Message, "Sin acceso a la base", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r = MessageBox.Show(
                "No existe la base de datos TecnoStore en el servidor \"" + servidor + "\".\n\n" +
                "Si ya ejecutaron los scripts en SSMS, probablemente fue en otra instancia (por ejemplo .\\SQLEXPRESS en lugar de localhost): " +
                "escriban esa en Servidor.\n\n" +
                "¿Quieren crearla ahora en \"" + servidor + "\" con los datos demo? (ejecuta los scripts de 02_SQL; tarda unos segundos)",
                "Base de datos no encontrada", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (r != DialogResult.Yes) { txtServidor.Focus(); return; }

            string textoOriginal = Text;
            try
            {
                Cursor = Cursors.WaitCursor;
                var resultado = InstaladorBase.Instalar(servidor, paso => { Text = "Creando base: " + paso + "…"; Application.DoEvents(); });
                string aviso = resultado.SeguridadOk ? "" :
                    "\n\nAviso: no se pudieron crear los logins y roles de 07_Seguridad.sql (su usuario de Windows necesita permiso para crear logins). " +
                    "La app funciona igual con la conexión única.\nDetalle: " + resultado.AvisoSeguridad;
                MessageBox.Show("Base TecnoStore creada en \"" + servidor + "\". Ya pueden iniciar sesión (contraseña de prueba 1234)." + aviso,
                                "Base creada", MessageBoxButtons.OK, resultado.SeguridadOk ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                btnIngresar.Enabled = true;
                btnIngresar.PerformClick();
            }
            catch (Exception ex3) { Ui.Error(ex3, "No se pudo crear la base"); }
            finally { Text = textoOriginal; Cursor = Cursors.Default; }
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
