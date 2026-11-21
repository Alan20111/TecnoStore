using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 2 · Panel de administración: barra lateral con los módulos del
    /// perfil, encabezado con el usuario y área central donde se abre cada
    /// pantalla. Las pantallas se crean una vez y se reutilizan.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        public bool CerrarSesion { get; private set; }

        private class Modulo
        {
            public string Clave, Titulo, Grupo, Atajo;
            public Func<Form> Crear;
            public Func<bool> Visible;
        }

        private readonly List<Modulo> modulos = new List<Modulo>();
        private readonly Dictionary<string, Form> abiertos = new Dictionary<string, Form>();

        public FrmPrincipal()
        {
            InitializeComponent();
            Animacion.Fundido(this);
            Forma.VentanaModerna(this, Tema.Carbon, Color.FromArgb(201, 199, 193));
            var s = Db.Sesion;
            // Matriz de permisos aplicada al menú (SQL Server la vuelve a validar)
            Func<bool> todos = () => true;
            Agregar("dashboard", "Dashboard", "General", null, () => new FrmDashboard(), () => s.Rol != "Operador");
            Agregar("venta", "Nueva venta", "Ventas", "F5", () => new FrmVenta(), () => s.PuedeOperar);
            Agregar("historial", "Historial de ventas", "Ventas", null, () => new FrmHistorialVentas(), todos);
            Agregar("clientes", "Clientes", "Ventas", "F3", () => new FrmClientes(), todos);
            Agregar("ordenes", "Órdenes de servicio", "Taller", "F6", () => new FrmOrdenes(), () => s.PuedeOperar);
            Agregar("productos", "Productos", "Inventario", "F2", () => new FrmProductos(), todos);
            Agregar("compras", "Compras a proveedor", "Inventario", "F7", () => new FrmCompra(), () => s.EsSupervisorOMas);
            Agregar("proveedores", "Proveedores", "Inventario", "F4", () => new FrmProveedores(), todos);
            Agregar("ajustes", "Ajustes de inventario", "Inventario", null, () => new FrmAjusteStock(), () => s.EsSupervisorOMas);
            Agregar("categorias", "Categorías", "Catálogos", null, () => new FrmCatalogo(TipoCatalogo.Categoria), todos);
            Agregar("marcas", "Marcas", "Catálogos", null, () => new FrmCatalogo(TipoCatalogo.Marca), todos);
            Agregar("tipos", "Tipos de servicio", "Catálogos", null, () => new FrmCatalogo(TipoCatalogo.TipoServicio), todos);
            Agregar("reportes", "Reportes", "Análisis", null, () => new FrmReportes(), () => s.Rol != "Operador");
            Agregar("usuarios", "Usuarios y roles", "Administración", null, () => new FrmUsuarios(), () => s.EsAdministrador);
            Agregar("auditoria", "Auditoría", "Administración", null, () => new FrmAuditoria(), () => s.EsSupervisorOMas);
        }

        private void Agregar(string clave, string titulo, string grupo, string atajo, Func<Form> crear, Func<bool> visible)
        {
            modulos.Add(new Modulo { Clave = clave, Titulo = titulo, Grupo = grupo, Atajo = atajo, Crear = crear, Visible = visible });
        }

        private MenuLateral menu;

        // Íconos de la fuente de íconos de Windows (Segoe Fluent Icons / MDL2 Assets)
        private static readonly Dictionary<string, char> Iconos = new Dictionary<string, char>
        {
            ["dashboard"] = '\uE80F', ["venta"] = '\uE7BF', ["historial"] = '\uE81C', ["clientes"] = '\uE716',
            ["ordenes"] = '\uE90F', ["productos"] = '\uE7B8', ["compras"] = '\uE719', ["proveedores"] = '\uE804',
            ["ajustes"] = '\uE9E9', ["categorias"] = '\uE8A9', ["marcas"] = '\uE8EC', ["tipos"] = '\uE15E',
            ["reportes"] = '\uE9D2', ["usuarios"] = '\uE77B', ["auditoria"] = '\uE72E',
        };

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            var s = Db.Sesion;
            Text = "TecnoStore — Panel de administración";
            lblUsuario.Text = s.Nombre;
            lblRol.Text = s.Rol + " · " + s.Sucursal;
            lblIniciales.Text = Iniciales(s.Nombre);
            Tema.Boton(btnCerrarSesion, false);
            pnlEncabezado.Paint += (o, ev) =>   // línea de lápiz bajo el encabezado
            {
                using (var linea = new Pen(Tema.LineaSuave)) ev.Graphics.DrawLine(linea, 0, pnlEncabezado.Height - 1, pnlEncabezado.Width, pnlEncabezado.Height - 1);
            };
            ConstruirMenu();
            Abrir(s.Rol == "Operador" ? "venta" : "dashboard");
        }

        private void ConstruirMenu()
        {
            menu = new MenuLateral { Dock = DockStyle.Fill };
            foreach (var m in modulos.Where(x => x.Visible()))
                menu.Agregar(m.Clave, m.Titulo, m.Grupo, Iconos.TryGetValue(m.Clave, out var ic) ? ic : ' ');
            menu.Seleccionado += Abrir;
            pnlMenu.Controls.Clear();
            pnlMenu.Controls.Add(menu);
        }

        /// <summary>
        /// Muestra el módulo en el área central (lo crea la primera vez). Entra
        /// deslizándose 14 px hacia arriba con salida suave, como las vistas de iOS.
        /// </summary>
        private void Abrir(string clave)
        {
            var m = modulos.Find(x => x.Clave == clave);
            if (m == null || !m.Visible()) return;

            Form f;
            if (!abiertos.TryGetValue(clave, out f) || f.IsDisposed)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    f = m.Crear();
                    f.TopLevel = false;
                    f.FormBorderStyle = FormBorderStyle.None;
                    Tema.Aplicar(f);
                    pnlContenido.Controls.Add(f);
                    abiertos[clave] = f;
                }
                catch (Exception ex) { Ui.Error(ex); return; }
                finally { Cursor = Cursors.Default; }
            }
            lblTitulo.Text = m.Titulo;
            lblAtajo.Text = m.Atajo == null ? "" : "Atajo: " + m.Atajo;
            lblAtajo.Left = lblTitulo.Right + 10;
            menu.Activar(clave);

            var destino = pnlContenido.DisplayRectangle;
            f.Dock = DockStyle.None;
            f.Bounds = new Rectangle(destino.X, destino.Y + 14, destino.Width, destino.Height);
            f.Show();
            f.BringToFront();
            Animacion.Ejecutar(260, Animacion.Salida,
                p => { if (!f.IsDisposed) f.Top = destino.Y + (int)Math.Round(14 * (1 - p)); },
                () => { if (!f.IsDisposed) f.Dock = DockStyle.Fill; });
        }

        // Atajos de teclado (los mismos que tenía el menú)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            string clave = null;
            switch (keyData)
            {
                case Keys.F2: clave = "productos"; break;
                case Keys.F3: clave = "clientes"; break;
                case Keys.F4: clave = "proveedores"; break;
                case Keys.F5: clave = "venta"; break;
                case Keys.F6: clave = "ordenes"; break;
                case Keys.F7: clave = "compras"; break;
            }
            if (clave != null) { Abrir(clave); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static string Iniciales(string nombre)
        {
            var partes = (nombre ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return partes.Length == 0 ? "?" : (partes[0][0].ToString() + (partes.Length > 1 ? partes[1][0].ToString() : "")).ToUpperInvariant();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Deseas cerrar la sesión?", "Cerrar sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            CerrarSesion = true;
            Db.Sesion = null;
            Close();
        }

        // Círculo con las iniciales del usuario
        private void lblIniciales_Paint(object sender, PaintEventArgs e)
        {
            var l = (Label)sender;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(pnlEncabezado.BackColor);
            using (var b = new SolidBrush(Tema.Texto)) e.Graphics.FillEllipse(b, 0, 0, l.Width - 1, l.Height - 1);
            TextRenderer.DrawText(e.Graphics, l.Text, Tema.FuenteNegrita, l.ClientRectangle, Tema.Fondo,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
