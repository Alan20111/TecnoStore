using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TecnoStore.Data;
using TecnoStore.Models;
using TecnoStore.Utils;

namespace TecnoStore.Forms
{
    /// <summary>
    /// Módulo 2 · Ventana principal. Pocos módulos grandes a la izquierda
    /// (F1, F2, F3…) y, si el módulo tiene varias vistas, botones grandes arriba.
    /// Cada módulo y cada vista aparece solo si SQL Server le concede al rol la
    /// capacidad que la respalda (usp_PermisosApp).
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        public bool CerrarSesion { get; private set; }

        private class Vista
        {
            public string Clave, Titulo;
            public Func<Form> Crear;
            public bool Visible;
        }

        private class Modulo
        {
            public string Clave, Titulo;
            public char Icono;
            public List<Vista> Vistas = new List<Vista>();
            public string Ultima;   // la vista en la que se quedó
        }

        private readonly List<Modulo> modulos = new List<Modulo>();
        private readonly Dictionary<string, Form> abiertos = new Dictionary<string, Form>();
        private readonly Panel pnlVistas = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Tema.Fondo, Visible = false };
        private readonly Label lblAviso = new Label { AutoSize = false, Visible = false, TextAlign = ContentAlignment.MiddleCenter, BackColor = Tema.Navegacion, ForeColor = Tema.Lateral, Font = Tema.FuenteSubtitulo };
        private readonly Timer relojAviso = new Timer { Interval = 2600 };
        private MenuLateral menu;
        private Segmentos segVistas;
        private Modulo actual;

        public FrmPrincipal()
        {
            InitializeComponent();
            Animacion.Fundido(this);
            Forma.VentanaModerna(this, Tema.Carbon, Tema.Texto);
            var s = Db.Sesion;
            bool ajusta = s.Puede(Capacidad.AjustarStock) || s.Puede(Capacidad.AjustarPrecio);

            // Un módulo = un enfoque. Lo que el rol nunca va a usar no aparece.
            Nuevo("inicio", "Inicio", '', V("inicio", "Inicio", () => new FrmInicio(Abrir)));
            Nuevo("ventas", "Ventas", '',
                V("venta", "Vender", () => new FrmVenta(), s.Puede(Capacidad.RegistrarVenta)),
                V("historial", "Ventas hechas", () => new FrmHistorialVentas()));
            Nuevo("taller", "Taller", '', V("ordenes", "Órdenes", () => new FrmOrdenes()));
            Nuevo("clientes", "Clientes", '', V("clientes", "Clientes", () => new FrmClientes()));
            Nuevo("inventario", "Inventario", '',
                V("productos", "Productos", () => new FrmProductos()),
                V("compras", "Compras", () => new FrmCompra(), s.Puede(Capacidad.RegistrarCompra)),
                V("ajustes", "Ajustes", () => new FrmAjusteStock(), ajusta),
                V("proveedores", "Proveedores", () => new FrmProveedores(), s.Puede(Capacidad.GuardarProveedor) || s.Puede(Capacidad.VerCostos)));
            Nuevo("reportes", "Reportes", '',
                V("dashboard", "Resumen", () => new FrmDashboard(), s.Puede(Capacidad.VerReportes)),
                V("reportes", "Reportes", () => new FrmReportes(), s.Puede(Capacidad.VerReportes)),
                V("auditoria", "Auditoría", () => new FrmAuditoria(), s.Puede(Capacidad.VerAuditoria)));
            Nuevo("ajustesapp", "Configuración", '',
                V("usuarios", "Usuarios", () => new FrmUsuarios(), s.Puede(Capacidad.AdminUsuarios)),
                V("categorias", "Categorías", () => new FrmCatalogo(TipoCatalogo.Categoria), s.Puede(Capacidad.EditarCatalogos)),
                V("marcas", "Marcas", () => new FrmCatalogo(TipoCatalogo.Marca), s.Puede(Capacidad.EditarCatalogos)),
                V("tipos", "Servicios", () => new FrmCatalogo(TipoCatalogo.TipoServicio), s.Puede(Capacidad.EditarCatalogos)));
            modulos.RemoveAll(m => m.Vistas.Count == 0);
        }

        private static Vista V(string clave, string titulo, Func<Form> crear, bool visible = true) =>
            new Vista { Clave = clave, Titulo = titulo, Crear = crear, Visible = visible };

        private void Nuevo(string clave, string titulo, char icono, params Vista[] vistas) =>
            modulos.Add(new Modulo { Clave = clave, Titulo = titulo, Icono = icono, Vistas = vistas.Where(v => v.Visible).ToList() });

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            var s = Db.Sesion;
            Text = "TecnoStore";
            lblUsuario.Text = s.Nombre;
            lblRol.Text = s.Rol + " · " + s.Sucursal;
            lblUsuario.Font = Tema.FuenteNegrita;
            lblTitulo.Font = Tema.FuenteTitulo;
            lblIniciales.Text = Iniciales(s.Nombre);
            Tema.Boton(btnCerrarSesion, false);
            Encabezado();
            pnlEncabezado.Resize += (o, ev) => Encabezado();
            pnlEncabezado.Paint += (o, ev) =>   // línea de lápiz bajo el encabezado
            {
                using (var linea = new Pen(Tema.LineaSuave)) ev.Graphics.DrawLine(linea, 0, pnlEncabezado.Height - 1, pnlEncabezado.Width, pnlEncabezado.Height - 1);
            };

            // Orden de acople: contenido (llena), vistas (arriba), encabezado, barra lateral
            Controls.Add(pnlVistas);
            Controls.SetChildIndex(pnlVistas, 1);
            Controls.Add(lblAviso);
            lblAviso.BringToFront();
            Ui.MostrarAviso = Avisar;
            relojAviso.Tick += (o, ev) => { relojAviso.Stop(); lblAviso.Visible = false; };
            Disposed += (o, ev) => { Ui.MostrarAviso = null; relojAviso.Dispose(); };

            menu = new MenuLateral { Dock = DockStyle.Fill };
            for (int i = 0; i < modulos.Count; i++)
                menu.Agregar(modulos[i].Clave, modulos[i].Titulo, "F" + (i + 1), modulos[i].Icono);
            menu.Seleccionado += AbrirModulo;
            pnlMenu.Controls.Clear();
            pnlMenu.Controls.Add(menu);
            AbrirModulo("inicio");
        }

        private void Encabezado()
        {
            int d = pnlEncabezado.ClientSize.Width - 20;
            btnCerrarSesion.SetBounds(d - 100, 14, 100, Tema.AltoBoton - 4);
            int ancho = Math.Max(lblUsuario.PreferredWidth, lblRol.PreferredWidth);
            lblUsuario.Location = new Point(d - 120 - ancho, 14);
            lblRol.Location = new Point(d - 120 - ancho, 37);
            lblIniciales.SetBounds(d - 120 - ancho - 52, 16, 40, 40);
        }

        /// <summary>Aviso breve abajo: dice que sí se hizo y se va solo.</summary>
        private void Avisar(string texto)
        {
            int w = Math.Min(ClientSize.Width - 80, TextRenderer.MeasureText(texto, lblAviso.Font).Width + 64);
            lblAviso.Text = texto;
            lblAviso.SetBounds((ClientSize.Width + pnlLateral.Width - w) / 2, ClientSize.Height - 96, w, 56);
            using (var p = Forma.Continua(new RectangleF(0, 0, w, 56), 18)) lblAviso.Region = new Region(p);
            lblAviso.Visible = true;
            lblAviso.BringToFront();
            relojAviso.Stop(); relojAviso.Start();
        }

        private void AbrirModulo(string clave)
        {
            var m = modulos.Find(x => x.Clave == clave);
            if (m != null) Abrir(m.Ultima ?? m.Vistas[0].Clave);
        }

        /// <summary>
        /// Muestra una vista (la crea la primera vez; después solo la actualiza).
        /// Entra deslizándose 14 px hacia arriba con salida suave.
        /// </summary>
        private Form Abrir(string clave)
        {
            var m = modulos.Find(x => x.Vistas.Any(v => v.Clave == clave));
            if (m == null) return null;   // su rol no la tiene
            var vista = m.Vistas.Find(v => v.Clave == clave);

            Form f;
            bool nueva = !abiertos.TryGetValue(clave, out f) || f.IsDisposed;
            if (nueva)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    f = vista.Crear();
                    f.TopLevel = false;
                    f.FormBorderStyle = FormBorderStyle.None;
                    Tema.Aplicar(f);
                    pnlContenido.Controls.Add(f);
                    abiertos[clave] = f;
                }
                catch (Exception ex) { Ui.Error(ex); return null; }
                finally { Cursor = Cursors.Default; }
            }
            m.Ultima = clave;
            Vistas(m, vista);
            lblTitulo.Text = m.Titulo;
            menu.Activar(m.Clave);

            var destino = pnlContenido.DisplayRectangle;
            f.Dock = DockStyle.None;
            f.Bounds = new Rectangle(destino.X, destino.Y + 14, destino.Width, destino.Height);
            f.Show();
            f.BringToFront();
            // Las demás se ocultan: no reciben Tab ni teclas
            foreach (var otra in abiertos.Values) if (otra != f && !otra.IsDisposed) otra.Visible = false;
            if (!nueva) (f as IRefrescable)?.Refrescar();
            Animacion.Ejecutar(240, Animacion.Salida,
                p => { if (!f.IsDisposed) f.Top = destino.Y + (int)Math.Round(14 * (1 - p)); },
                () => { if (!f.IsDisposed) f.Dock = DockStyle.Fill; });
            return f;
        }

        /// <summary>Los botones de arriba: solo si el módulo tiene más de una vista para este rol.</summary>
        private void Vistas(Modulo m, Vista vista)
        {
            if (actual != m)
            {
                actual = m;
                pnlVistas.Controls.Clear();
                segVistas?.Dispose();
                segVistas = null;
                if (m.Vistas.Count > 1)
                {
                    segVistas = new Segmentos(m.Vistas.Select(v => v.Titulo).ToArray()) { Location = new Point(24, 18) };
                    segVistas.Elegido += i => Abrir(m.Vistas[i].Clave);
                    pnlVistas.Controls.Add(segVistas);
                }
                pnlVistas.Visible = segVistas != null;
            }
            if (segVistas != null) segVistas.Activo = m.Vistas.IndexOf(vista);
        }

        // F1, F2, F3… abren los módulos en el orden del menú; Esc regresa al Inicio
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int n = keyData - Keys.F1;
            if (n >= 0 && n < modulos.Count) { AbrirModulo(modulos[n].Clave); return true; }
            if (keyData == Keys.Escape && actual != null && actual.Clave != "inicio"
                && !(CampoConTexto(ActiveControl))) { AbrirModulo("inicio"); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // En un buscador con texto, Esc primero lo limpia
        private static bool CampoConTexto(Control c)
        {
            while (c is ContainerControl cc && cc.ActiveControl != null) c = cc.ActiveControl;
            return c is TextBox t && t.Text != "" || c is ComboBox cb && cb.DroppedDown;
        }

        private static string Iniciales(string nombre)
        {
            var partes = (nombre ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return partes.Length == 0 ? "?" : (partes[0][0].ToString() + (partes.Length > 1 ? partes[1][0].ToString() : "")).ToUpperInvariant();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            if (!Ui.Confirmar("¿Cerrar la sesión?", "Salir")) return;
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
