using System;
using System.Data.SqlClient;
using System.Drawing;
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

        private readonly PanelCampos campos;
        private readonly Label lblServidorActual = new Label { AutoSize = false, ForeColor = Tema.Navegacion, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };

        public FrmLogin()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            Controls.Clear();
            ClientSize = new Size(440, 500);
            campos = new PanelCampos(1, new Campo("Usuario", txtUsuario), new Campo("Contraseña", txtContrasena), new Campo("Servidor", txtServidor)) { Padding = new Padding(0) };
            lblTitulo.SetBounds(36, 30, 360, 52);
            Acomodar(false);
            Controls.AddRange(new Control[] { lblTitulo, campos, btnIngresar, lblServidorActual, lblAyuda });
            campos.Mostrar(txtServidor, false);
            lblServidorActual.Click += (s, e) => MostrarServidor();
            campos.BackColor = Tema.Tarjeta;
            Tema.Aplicar(this);
            lblTitulo.Font = Tema.FuenteCifra;
            btnIngresar.Font = Tema.FuenteSubtitulo;
            btnIngresar.Text = "Entrar";
            lblAyuda.Text = "Para probar: admin, csupervisor, loperador o fconsulta\r\nContraseña: 1234";
            campos.BackColor = Tema.Tarjeta;
            BackColor = Tema.Tarjeta;
            Animacion.Fundido(this);
            Forma.VentanaModerna(this, Tema.Carbon, Tema.Texto);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            string guardado = null;
            try { if (File.Exists(ArchivoServidor)) guardado = File.ReadAllText(ArchivoServidor).Trim(); } catch { /* sin servidor guardado */ }
            txtServidor.Text = string.IsNullOrEmpty(guardado) ? Db.ServidorConfigurado() : guardado;
            lblServidorActual.Text = "Servidor: " + txtServidor.Text + "  (cambiar)";
            txtUsuario.Select();
        }

        private void Acomodar(bool conServidor)
        {
            int alto = conServidor ? 262 : 176;
            campos.SetBounds(38, 94, 368, alto);
            btnIngresar.SetBounds(38, 94 + alto + 4, 364, 56);
            lblServidorActual.SetBounds(38, 94 + alto + 72, 364, 28);
            lblAyuda.SetBounds(38, 94 + alto + (conServidor ? 76 : 112), 364, 66);
            lblAyuda.Font = Tema.FuentePequena;
            lblAyuda.ForeColor = Tema.TextoSuave;
            ClientSize = new Size(440, 94 + alto + (conServidor ? 160 : 196));
        }

        /// <summary>El servidor casi nunca cambia: se pide solo al tocarlo o si la conexión falla.</summary>
        private void MostrarServidor()
        {
            campos.Mostrar(txtServidor, true);
            Acomodar(true);
            lblServidorActual.Visible = false;
            txtServidor.Focus();
            txtServidor.SelectAll();
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
                AuthRepository.CargarPermisos(Db.Sesion);   // qué puede ver y hacer, según SQL Server
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
                MostrarServidor();
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
            if (r != DialogResult.Yes) { MostrarServidor(); return; }

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
