// Vista previa de la app Windows Forms de TecnoStore.
// Cada pantalla corresponde a un formulario/UserControl que se hará en C#.
'use strict';

const $app = document.getElementById('app');
const state = { token: null, user: null, modulo: 'inicio', cat: null };

/* ---------- utilidades ---------- */
const esc = (v) => String(v ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
const money = (v) => (v == null ? '' : Number(v).toLocaleString('es-MX', { style: 'currency', currency: 'MXN' }));
const date = (v) => (v ? new Date(v).toLocaleDateString('es-MX', { day: '2-digit', month: '2-digit', year: 'numeric', timeZone: 'UTC' }) : '');
const datetime = (v) => (v ? new Date(v).toLocaleString('es-MX', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', timeZone: 'UTC' }) : '');
const h = (html) => { const t = document.createElement('template'); t.innerHTML = html.trim(); return t.content.firstElementChild; };

async function api(path, body) {
  const res = await fetch(path, {
    method: body ? 'POST' : 'GET',
    headers: { 'Content-Type': 'application/json', 'x-token': state.token || '' },
    body: body ? JSON.stringify(body) : undefined,
  });
  const data = await res.json();
  if (!res.ok) throw new Error(data.error || 'Error');
  return data;
}

// MessageBox.Show(...)
function messageBox(text, { title = 'TecnoStore', icon = 'info', buttons = ['Aceptar'] } = {}) {
  return new Promise((resolve) => {
    const glyph = { info: 'i', error: '✕', question: '?', ok: '✓' }[icon];
    const el = h(`<div class="overlay"><div class="msgbox">
      <div class="titlebar"><span class="title" style="padding-left:12px">${esc(title)}</span><div class="caption"><span class="close">✕</span></div></div>
      <div class="msg"><div class="ico ${icon}">${glyph}</div><div>${esc(text).replace(/\n/g, '<br>')}</div></div>
      <div class="buttons">${buttons.map((b, i) => `<button data-i="${i}" ${i === 0 ? 'class="primary"' : ''}>${esc(b)}</button>`).reverse().join('')}</div>
    </div></div>`);
    const close = (v) => { el.remove(); resolve(v); };
    el.querySelectorAll('button').forEach((b) => b.onclick = () => close(buttons[+b.dataset.i]));
    el.querySelector('.close').onclick = () => close(null);
    document.body.appendChild(el);
    (el.querySelector('.buttons button.primary') || el.querySelector('button')).focus();
  });
}
const showError = (e) => messageBox(e.message || e, { icon: 'error', title: 'Error' });

// Capacidades que SQL Server concede al rol (usp_PermisosApp); la interfaz solo muestra lo que se puede usar
const tiene = (cap) => !!state.user?.caps?.includes(cap);
const ES_COSTO = /costo|margen|utilidad|valor_inventario/i;
const vacia = '<div class="empty">No hay resultados. Prueba con otra búsqueda o quita filtros.</div>';

// DataGridView: pocas columnas; las de costo desaparecen si el rol no puede verlas
function grid(cols, rows, { onSelect, rowClass, cls = '' } = {}) {
  if (!tiene('ver_costos')) cols = cols.filter((x) => !ES_COSTO.test(x.k));
  const el = h(`<div class="grid ${cls}"></div>`);
  if (!rows.length) { el.innerHTML = vacia; return el; }
  el.innerHTML = `<table><thead><tr><th class="rowheader"></th>${cols.map((c) => `<th>${esc(c.t)}</th>`).join('')}</tr></thead>
    <tbody>${rows.map((r, i) => `<tr data-i="${i}" class="${rowClass ? rowClass(r) : ''}"><td class="rowheader"></td>${cols.map((c) => {
      const v = c.f ? c.f(r[c.k], r) : esc(r[c.k]);
      return `<td class="${c.num ? 'num' : ''}">${v}</td>`;
    }).join('')}</tr>`).join('')}</tbody></table>`;
  el.querySelectorAll('tbody tr').forEach((tr) => tr.onclick = () => {
    el.querySelectorAll('tr.sel').forEach((x) => x.classList.remove('sel'));
    tr.classList.add('sel');
    onSelect?.(rows[+tr.dataset.i]);
  });
  return el;
}
const M = (k) => (v) => money(v);
const pillEstado = (v) => `<span class="pill ${v === 'Cancelada' ? 'roja' : 'verde'}">${esc(v)}</span>`;
const N = (v) => esc(v);
const options = (list, sel, empty) => (empty ? `<option value="">${esc(empty)}</option>` : '') +
  list.map((o) => `<option value="${o.id}" ${String(o.id) === String(sel) ? 'selected' : ''}>${esc(o.nombre)}</option>`).join('');

function titlebar(title) {
  return `<div class="titlebar"><div class="icon">T</div><div class="title">${esc(title)}</div>
    <div class="caption"><span data-win="min" title="Minimizar">─</span><span data-win="max" title="Maximizar">☐</span><span data-win="close" class="close" title="Cerrar">✕</span></div></div>`;
}

// Botones ─ ☐ ✕ de la ventana: minimizar deja solo la barra de título,
// maximizar ocupa toda la pantalla y cerrar ejecuta la acción de salida.
function ventana(el, alCerrar) {
  el.querySelector('[data-win=min]').onclick = () => el.classList.toggle('min');
  el.querySelector('[data-win=max]').onclick = () => el.classList.toggle('max');
  el.querySelector('.titlebar .title').ondblclick = () => el.classList.toggle('max');
  el.querySelector('[data-win=close]').onclick = alCerrar;
}

const salirApp = () => messageBox('En la aplicación de Windows Forms este botón cierra el programa.\nEn la vista previa no hay nada que cerrar.', { title: 'Salir' });

/* ---------- FrmLogin ---------- */
function renderLogin() {
  $app.innerHTML = '';
  const el = h(`<div class="form dialog">
    ${titlebar('Iniciar sesión — TecnoStore')}
    <form class="login" autocomplete="off">
      <div class="logo"><div class="icon">T</div><div><b>TecnoStore</b><span style="color:#5f5f5f">Sistema Integral de Gestión</span></div></div>
      <div class="field"><span>Servidor (localhost, .\\SQLEXPRESS…):</span><input value="localhost" disabled title="En la vista previa siempre es el SQL Server de Docker"></div>
      <div class="field"><span>Usuario:</span><input name="usuario" required></div>
      <div class="field"><span>Contraseña:</span><input name="contrasena" type="password" required></div>
      <div class="error-label"></div>
      <div class="row"><span class="spacer"></span><button type="submit" class="primary">Ingresar</button><button type="button" data-salir-app>Salir</button></div>
      <div class="hint">Usuarios demo (contraseña <code data-p>1234</code>):<br>
        <code data-u>admin</code> Administrador · <code data-u>csupervisor</code> Supervisor<br>
        <code data-u>loperador</code> Operador · <code data-u>fconsulta</code> Consulta</div>
    </form></div>`);
  const form = el.querySelector('form');
  el.querySelectorAll('[data-u]').forEach((c) => c.onclick = () => { form.usuario.value = c.textContent; form.contrasena.value = '1234'; });
  el.querySelector('[data-p]').onclick = () => { form.contrasena.value = '1234'; };
  el.querySelector('[data-salir-app]').onclick = salirApp;
  ventana(el, salirApp);
  form.onsubmit = async (ev) => {
    ev.preventDefault();
    const btn = form.querySelector('button[type=submit]');
    btn.disabled = true;
    try {
      const r = await api('/api/login', { usuario: form.usuario.value.trim(), contrasena: form.contrasena.value });
      state.token = r.token; state.user = r.user; state.modulo = 'inicio';
      state.cat = await api('/api/catalogos');
      renderMain();
    } catch (e) {
      el.querySelector('.error-label').textContent = e.message;
      form.contrasena.classList.add('invalid');
    } finally { btn.disabled = false; }
  };
  $app.appendChild(el);
  form.usuario.focus();
}

/* ---------- FrmPrincipal: panel con barra lateral ---------- */
// [clave, título, grupo, atajo, render] — mismo orden y grupos que FrmPrincipal en C#
const MODULOS_LISTA = [
  ['dashboard', 'Dashboard', 'General', null, renderDashboard],
  ['venta', 'Nueva venta', 'Ventas', 'F5', renderVenta],
  ['ventas', 'Historial de ventas', 'Ventas', null, renderVentas],
  ['clientes', 'Clientes', 'Ventas', 'F3', renderClientes],
  ['ordenes', 'Órdenes de servicio', 'Taller', 'F6', renderOrdenes],
  ['productos', 'Productos', 'Inventario', 'F2', renderProductos],
  ['compras', 'Compras a proveedor', 'Inventario', 'F7', renderCompras],
  ['proveedores', 'Proveedores', 'Inventario', 'F4', renderProveedores],
  ['ajustes', 'Ajustes de inventario', 'Inventario', null, renderAjustes],
  ['categorias', 'Categorías', 'Catálogos', null, (c) => renderCatalogo(c, 'categorias')],
  ['marcas', 'Marcas', 'Catálogos', null, (c) => renderCatalogo(c, 'marcas')],
  ['tipos', 'Tipos de servicio', 'Catálogos', null, (c) => renderCatalogo(c, 'tipos')],
  ['reportes', 'Reportes', 'Análisis', null, renderReportes],
  ['usuarios', 'Usuarios y roles', 'Administración', null, renderUsuarios],
  ['auditoria', 'Auditoría', 'Administración', null, renderAuditoria],
];
const ICONOS = {
  dashboard: '<rect x="3" y="3" width="7" height="9" rx="2"/><rect x="14" y="3" width="7" height="5" rx="2"/><rect x="14" y="12" width="7" height="9" rx="2"/><rect x="3" y="16" width="7" height="5" rx="2"/>',
  venta: '<path d="M6 7h12l-1 13H7z"/><path d="M9 7a3 3 0 0 1 6 0"/>',
  ventas: '<circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/>',
  clientes: '<circle cx="9" cy="8" r="3.5"/><path d="M2.5 20a6.5 6.5 0 0 1 13 0"/><path d="M16 4.5a3.5 3.5 0 0 1 0 7M18.5 20a6.5 6.5 0 0 0-3-5.5"/>',
  ordenes: '<path d="M14.7 6.3a4 4 0 0 0-5.4 5.2L3 17.8V21h3.2l6.3-6.3a4 4 0 0 0 5.2-5.4l-2.6 2.6-2.4-.6-.6-2.4z"/>',
  productos: '<path d="M21 8 12 3 3 8l9 5 9-5z"/><path d="M3 8v8l9 5 9-5V8"/><path d="M12 13v8"/>',
  compras: '<path d="M3 4h2l2.4 11.2a2 2 0 0 0 2 1.6h7.7a2 2 0 0 0 2-1.5L21 8H6"/><circle cx="10" cy="20" r="1.3"/><circle cx="17" cy="20" r="1.3"/>',
  proveedores: '<path d="M3 17V7h11v10"/><path d="M14 10h4l3 3v4h-7"/><circle cx="7" cy="18" r="2"/><circle cx="17" cy="18" r="2"/>',
  ajustes: '<path d="M4 6h10M18 6h2M4 12h4M12 12h8M4 18h12M20 18h0"/><circle cx="16" cy="6" r="2"/><circle cx="10" cy="12" r="2"/><circle cx="18" cy="18" r="2"/>',
  categorias: '<rect x="3" y="3" width="7" height="7" rx="2"/><rect x="14" y="3" width="7" height="7" rx="2"/><rect x="3" y="14" width="7" height="7" rx="2"/><rect x="14" y="14" width="7" height="7" rx="2"/>',
  marcas: '<path d="M20.6 13.4 13.4 20.6a2 2 0 0 1-2.8 0L3 13V3h10l7.6 7.6a2 2 0 0 1 0 2.8z"/><circle cx="7.5" cy="7.5" r="1.5"/>',
  tipos: '<path d="M9 4h6M10 4v4l-5 9a2 2 0 0 0 1.7 3h10.6a2 2 0 0 0 1.7-3l-5-9V4"/>',
  reportes: '<path d="M4 20V10M10 20V4M16 20v-7M22 20H2"/>',
  usuarios: '<circle cx="12" cy="8" r="4"/><path d="M4 21a8 8 0 0 1 16 0"/>',
  auditoria: '<path d="M12 3 4 6v6c0 4.5 3.4 8.3 8 9 4.6-.7 8-4.5 8-9V6z"/><path d="m9 12 2 2 4-4"/>',
};
const icono = (k) => `<svg viewBox="0 0 24 24" aria-hidden="true">${ICONOS[k] || ''}</svg>`;

// Material: "vidrio" (Liquid Glass aproximado) u "opaco" (iOS 16); se recuerda en este navegador
function leerMaterial() { try { return localStorage.getItem('material') || 'vidrio'; } catch (e) { return 'vidrio'; } }
function ponerMaterial(m) {
  document.documentElement.dataset.material = m;
  try { localStorage.setItem('material', m); } catch (e) { /* sin almacenamiento */ }
}
ponerMaterial(leerMaterial());

// Control segmentado con "pulgar" que se desliza (segmented control de Apple)
function segmentado(contenedor, selector, activo) {
  let pulgar = contenedor.querySelector('.pulgar');
  if (!pulgar) { pulgar = document.createElement('span'); pulgar.className = 'pulgar'; contenedor.prepend(pulgar); }
  const b = contenedor.querySelector(selector + '.' + activo) || contenedor.querySelector(selector);
  if (!b) return;
  pulgar.style.left = b.offsetLeft + 'px';
  pulgar.style.width = b.offsetWidth + 'px';
}

// Números que cuentan hasta su valor (como los widgets de Apple)
function contar(el, final, formato, ms = 700) {
  if (matchMedia('(prefers-reduced-motion: reduce)').matches || document.documentElement.classList.contains('sin-animacion')) { el.textContent = formato(final); return; }
  const t0 = performance.now();
  const paso = (t) => {
    const p = Math.min(1, (t - t0) / ms), e = 1 - Math.pow(1 - p, 3);
    el.textContent = formato(final * e);
    if (p < 1) requestAnimationFrame(paso);
  };
  requestAnimationFrame(paso);
}

const MODULOS = Object.fromEntries(MODULOS_LISTA.map(([k, t, g, a, r]) => [k, [t, r, g, a]]));
const iniciales = (n) => (n || '?').split(' ').filter(Boolean).slice(0, 2).map((x) => x[0]).join('').toUpperCase();

function renderMain() {
  const u = state.user;
  $app.innerHTML = '';
  let grupo = null, nav = '';
  for (const [k, t, g] of MODULOS_LISTA) {
    if (!u.modulos.includes(k)) continue;
    if (g !== grupo) { grupo = g; nav += `<div class="grupo">${esc(g)}</div>`; }
    nav += `<button data-m="${k}">${icono(k)}<span>${esc(t)}</span></button>`;
  }
  const el = h(`<div class="form app">
    ${titlebar('TecnoStore — Panel de administración')}
    <div class="body">
      <aside class="lateral">
        <div class="marca"><div class="sello">T</div><div><b>TecnoStore</b><small>Panel de administración</small></div></div>
        <nav><span class="indicador"></span>${nav}</nav>
      </aside>
      <div class="principal">
        <header class="encabezado">
          <h2 class="titulo"></h2><span class="atajo"></span><span class="spacer"></span>
          <div class="segmentado material" title="Material de la interfaz"><button data-mat="vidrio">Vidrio líquido</button><button data-mat="opaco">Opaco</button></div>
          <div class="avatar">${esc(iniciales(u.nombre))}</div>
          <div class="quien"><b>${esc(u.nombre)}</b><small>${esc(u.rol)} · ${esc(u.sucursal)}</small></div>
          <button data-salir>Cerrar sesión</button>
        </header>
        <div class="content"></div>
      </div>
    </div></div>`);
  const cerrarSesion = async () => {
    if (await messageBox('¿Deseas cerrar la sesión?', { icon: 'question', buttons: ['Sí', 'No'] }) === 'Sí') { state.token = null; renderLogin(); }
  };
  el.querySelectorAll('[data-m]').forEach((b) => b.onclick = () => abrir(b.dataset.m));
  el.querySelector('[data-salir]').onclick = cerrarSesion;
  ventana(el, cerrarSesion);
  const seg = el.querySelector('.material');
  const pintarMaterial = () => {
    const m = leerMaterial();
    seg.querySelectorAll('button').forEach((b) => b.classList.toggle('on', b.dataset.mat === m));
    segmentado(seg, 'button', 'on');
  };
  seg.querySelectorAll('button').forEach((b) => b.onclick = () => { ponerMaterial(b.dataset.mat); pintarMaterial(); });
  requestAnimationFrame(pintarMaterial);
  document.onkeydown = (ev) => {
    const tecla = { F2: 'productos', F3: 'clientes', F4: 'proveedores', F5: 'venta', F6: 'ordenes', F7: 'compras' }[ev.key];
    if (tecla && state.user?.modulos.includes(tecla) && !document.querySelector('.overlay')) { ev.preventDefault(); abrir(tecla); }
  };
  $app.appendChild(el);
  abrir(u.modulos.includes(state.modulo) ? state.modulo : u.modulos[0]);
}

const acercaDe = () => messageBox('TecnoStore — Panel de administración\nProyecto final de Taller de Bases de Datos\n\nVista previa conectada a SQL Server. La aplicación real es la solución de Windows Forms en 03_WindowsForms.', { title: 'Acerca de' });

// La pastilla verde azulado se desliza hasta el módulo activo
function moverIndicador() {
  const ind = document.querySelector('.lateral .indicador'), b = document.querySelector('.lateral [data-m].active');
  if (!ind || !b) return;
  ind.style.top = b.offsetTop + 'px';
  ind.style.height = b.offsetHeight + 'px';
}

async function abrir(m) {
  state.modulo = m;
  document.querySelectorAll('.lateral [data-m]').forEach((b) => b.classList.toggle('active', b.dataset.m === m));
  moverIndicador();
  document.querySelector('.encabezado .titulo').textContent = MODULOS[m][0];
  document.querySelector('.encabezado .atajo').textContent = MODULOS[m][3] ? 'Atajo: ' + MODULOS[m][3] : '';
  const content = document.querySelector('.content');
  content.innerHTML = '<div class="sub">Cargando…</div>';
  content.scrollTop = 0;
  try { await MODULOS[m][1](content); } catch (e) { content.innerHTML = ''; showError(e); }
}

/* ---------- FrmDashboard ---------- */
// Una sola pantalla (Stephen Few): 4 indicadores, 2 gráficas de barras y 2 listas cortas.
// El detalle vive en Reportes.
const mx = (v, d = 0) => Number(v).toLocaleString('es-MX', { style: 'currency', currency: 'MXN', maximumFractionDigits: d, minimumFractionDigits: d });
const miles = (v) => (v ? '$' + Math.round(v / 1000) + 'K' : '0');

function variacion(actual, anterior) {
  if (!anterior) return [actual ? 'sin datos del periodo anterior' : 'sin movimientos', ''];
  const pct = (actual - anterior) * 100 / anterior;
  return [`${pct >= 0 ? '▲' : '▼'} ${Math.abs(pct).toFixed(1)} % vs. 90 días previos`, pct >= 0 ? 'sube' : 'baja'];
}

// Gráfica de columnas en SVG (equivale al Chart de tipo Column en C#)
function columnas(datos) {
  const W = 560, H = 230, iz = 52, ab = 26, ar = 22, max = Math.max(1, ...datos.map((d) => d.v)) * 1.12;
  const paso = (W - iz - 10) / datos.length, ancho = paso * 0.55;
  const y = (v) => ar + (H - ar - ab) * (1 - v / max);
  const lineas = [0, 0.5, 1].map((f) => {
    const v = max * f, yy = y(v);
    return `<line x1="${iz}" x2="${W - 6}" y1="${yy}" y2="${yy}" class="rejilla"/><text x="${iz - 8}" y="${yy + 4}" class="eje" text-anchor="end">${miles(v)}</text>`;
  }).join('');
  const barras = datos.map((d, i) => {
    const x = iz + paso * i + (paso - ancho) / 2, yy = y(d.v);
    return `<rect x="${x}" y="${yy}" width="${ancho}" height="${H - ab - yy}" rx="4" class="barra${i === datos.length - 1 ? ' actual' : ''}"><title>${esc(d.t)}: ${esc(mx(d.v))}</title></rect>
      <text x="${x + ancho / 2}" y="${yy - 6}" class="valor" text-anchor="middle">${d.v ? miles(d.v) : ''}</text>
      <text x="${x + ancho / 2}" y="${H - 8}" class="eje" text-anchor="middle">${esc(d.t)}</text>`;
  }).join('');
  return `<svg viewBox="0 0 ${W} ${H}" class="grafica">${lineas}${barras}</svg>`;
}

// Barras horizontales: comparar categorías es más fácil que en una dona (Chart tipo Bar en C#)
function barrasH(datos) {
  if (!datos.length) return vacia;
  const max = Math.max(1, ...datos.map((d) => d.v));
  return `<div class="barrasH">${datos.map((d) => `<div class="bh" title="${esc(d.t)}: ${esc(mx(d.v))}"><span>${esc(d.t)}</span>
    <div><i style="width:${(d.v / max) * 100}%"></i></div><b>${esc(miles(d.v))}</b></div>`).join('')}</div>`;
}

async function renderDashboard(c) {
  const d = await api('/api/dashboard');
  const k = d.kpi, u = state.user;
  const hora = new Date().getHours();
  const saludo = hora < 12 ? 'Buenos días' : hora < 19 ? 'Buenas tardes' : 'Buenas noches';
  const fecha = new Date().toLocaleDateString('es-MX', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
  const [vi, ci] = variacion(k.ingresos_mes, k.ingresos_ant), [vv, cv] = variacion(k.ventas_mes, k.ventas_ant);
  // Lo más importante arriba a la izquierda
  const tarjetas = [
    ['Ingresos · 90 días', k.ingresos_mes, vi, ci, true],
    ['Ventas · 90 días', k.ventas_mes, vv, cv],
    ['Equipos en taller', k.ordenes_abiertas, `${k.ordenes_listas} listos para entregar`, ''],
    ['Productos por resurtir', k.alertas_stock, k.agotados ? `${k.agotados} agotado(s)` : 'ninguno agotado', k.agotados ? 'baja' : 'sube'],
  ];
  const meses = [];
  for (let i = 5; i >= 0; i--) {
    const f = new Date(); f.setDate(1); f.setMonth(f.getMonth() - i);
    const clave = f.toISOString().slice(0, 7);
    const fila = d.meses.find((m) => m.mes === clave);
    const t = f.toLocaleDateString('es-MX', { month: 'short' }).replace('.', '');
    meses.push({ t: t[0].toUpperCase() + t.slice(1), v: fila ? fila.ingresos : 0 });
  }
  c.innerHTML = `<div class="dash">
    <div class="dash-saludo"><div><h1>${saludo}, ${esc(u.nombre.split(' ')[0])}</h1>
      <div class="sub">${esc(fecha[0].toUpperCase() + fecha.slice(1))} · ${esc(u.sucursal)}</div></div>
      <span class="spacer"></span><span class="sub">Actualizado a las ${new Date().toLocaleTimeString('es-MX', { hour: '2-digit', minute: '2-digit' })}</span>
      <button data-actualizar>Actualizar</button></div>
    <div class="kpis">${tarjetas.map(([t, , n, cls], i) => `<div class="card kpi"><span>${esc(t)}</span><b data-kpi="${i}">—</b><small class="${cls}">${esc(n)}</small></div>`).join('')}</div>
    <div class="fila2">
      <div class="card"><h3>Ingresos por mes</h3>${columnas(meses)}</div>
      <div class="card"><h3>Ventas por categoría · 90 días</h3>${barrasH(d.categorias.map((x) => ({ t: x.categoria, v: x.ingresos })))}</div>
    </div>
    <div class="fila2">
      <div class="card"><h3>Requiere atención · productos por resurtir</h3><div data-g="alertas"></div></div>
      <div class="card"><h3>Requiere atención · equipos en el taller</h3><div data-g="taller"></div></div>
    </div></div>`;
  const poner = (k2, el) => c.querySelector(`[data-g=${k2}]`).replaceWith(el);
  poner('alertas', grid([{ t: 'Producto', k: 'producto' }, { t: 'Stock', k: 'stock', num: 1, f: (v) => `<span class="${v === 0 ? 'texto-rojo' : ''}">${v}</span>` }, { t: 'Mínimo', k: 'minimo', num: 1 }], d.alertas, { cls: 'compacta' }));
  poner('taller', grid([{ t: 'Folio', k: 'folio' }, { t: 'Equipo', k: 'equipo' }, { t: 'Estado', k: 'estado', f: (v) => `<span class="pill azul">${esc(v)}</span>` }, { t: 'Días', k: 'dias', num: 1 }], d.taller, { cls: 'compacta' }));
  c.querySelector('[data-actualizar]').onclick = () => renderDashboard(c).catch(showError);
  c.querySelectorAll('[data-kpi]').forEach((el) => {
    const [, valor, , , dinero] = tarjetas[+el.dataset.kpi];
    contar(el, Number(valor) || 0, dinero ? (v) => mx(v) : (v) => Math.round(v).toLocaleString('es-MX'));
  });
}

/* ---------- FrmProductos ---------- */
async function renderProductos(c) {
  c.innerHTML = `<h1>Productos</h1><div class="sub">Busca por nombre o SKU. Los que necesitan resurtirse aparecen resaltados.</div>
    <fieldset class="groupbox"><legend>Filtros</legend><div class="row">
      <div class="field"><span>Buscar (nombre o SKU):</span><input name="texto" style="width:220px"></div>
      <div class="field"><span>Categoría:</span><select name="categoria">${options(state.cat.categorias, '', '(Todas)')}</select></div>
      <label style="margin-top:14px"><input type="checkbox" name="alertas"> Solo con alerta de stock</label>
      <span class="spacer"></span>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button><button data-limpiar style="margin-top:14px">Limpiar</button>
    </div></fieldset><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="sub count"></div>`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  const cargar = async () => {
    const qs = new URLSearchParams({ texto: f('texto').value, categoria: f('categoria').value, alertas: f('alertas').checked ? '1' : '' });
    const rows = await api('/api/productos?' + qs);
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' },
      { t: 'Precio', k: 'precio_venta', num: 1, f: M() }, { t: 'Stock', k: 'stock_actual', num: 1 },
      { t: 'Alerta', k: 'alerta_stock', f: (v) => (v === 'OK' || !v ? '' : `<span class="pill ${v === 'Agotado' ? 'roja' : 'ambar'}">${esc(v)}</span>`) },
    ], rows, { rowClass: (r) => ({ Agotado: 'danger', 'Stock bajo': 'warn' }[r.alerta_stock] || '') }));
    c.querySelector('.count').textContent = `${rows.length} registro(s)`;
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  c.querySelector('[data-limpiar]').onclick = () => { f('texto').value = ''; f('categoria').value = ''; f('alertas').checked = false; cargar().catch(showError); };
  f('texto').onkeydown = (e) => { if (e.key === 'Enter') cargar().catch(showError); };
  await cargar();
}

/* ---------- FrmClientes ---------- */
async function renderClientes(c) {
  c.innerHTML = `<h1>Clientes</h1><div class="sub">El nivel depende de cuánto ha comprado cada cliente.</div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Nombre, apellidos o correo:</span><input name="texto" style="width:260px"></div>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button>
    </div></fieldset><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="sub count"></div>`;
  const cargar = async () => {
    const rows = await api('/api/clientes?' + new URLSearchParams({ texto: c.querySelector('[name=texto]').value }));
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Cliente', k: 'cliente' }, { t: 'Correo', k: 'correo' },
      { t: 'Compras', k: 'compras', num: 1 }, { t: 'Total comprado', k: 'total_comprado', num: 1, f: M() },
      { t: 'Nivel', k: 'nivel', f: (v) => (v === 'Sin compras' ? '<span style="color:#888">Sin compras</span>' : `<span class="badge ${esc(v)}">${esc(v)}</span>`) },
    ], rows, { rowClass: (r) => (r.activo ? '' : 'danger') }));
    c.querySelector('.count').textContent = `${rows.length} cliente(s)`;
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  c.querySelector('[name=texto]').onkeydown = (e) => { if (e.key === 'Enter') cargar().catch(showError); };
  await cargar();
}

/* ---------- FrmVenta (maestro-detalle) ---------- */
async function renderVenta(c) {
  state.cat = await api('/api/catalogos');
  const lineas = [];
  const prod = (id) => state.cat.productos.find((p) => p.id === +id);
  c.innerHTML = `<h1>Nueva venta</h1><div class="sub">Elige cliente y productos, y guarda. Si algo no cuadra (stock, descuento) no se guarda nada.</div>
    <fieldset class="groupbox"><legend>Encabezado</legend><div class="row">
      <div class="field"><span>Cliente: *</span><select name="cliente" style="width:240px">${options(state.cat.clientes, '', '(Selecciona)')}</select></div>
      <div class="field"><span>Método de pago: *</span><select name="metodo">${options(state.cat.metodos, 1)}</select></div>
      <div class="field"><span>Descuento %:</span><input name="desc" type="number" min="0" max="50" step="1" value="0" style="width:70px"></div>
      <div class="field"><span>Vendedor:</span><input value="${esc(state.user.nombre)}" disabled style="width:180px"></div>
    </div><div class="sub" style="margin-top:6px">${tiene('cancelar_venta') ? 'Puedes autorizar descuentos de hasta 50 %.' : `Descuentos mayores a ${esc(state.cat.descuentoMax)} % los autoriza un Supervisor.`}</div></fieldset>
    <fieldset class="groupbox"><legend>Agregar producto</legend><div class="row">
      <div class="field"><span>Producto:</span><select name="producto" style="width:380px">${state.cat.productos.map((p) =>
        `<option value="${p.id}">${esc(p.sku)} — ${esc(p.nombre)} (${money(p.precio_venta)} · stock ${p.stock_actual})</option>`).join('')}</select></div>
      <div class="field"><span>Cantidad:</span><input name="cant" type="number" min="1" value="1" style="width:70px"></div>
      <button data-agregar style="margin-top:14px">Agregar ↓</button><button data-quitar style="margin-top:14px">Quitar línea</button>
    </div></fieldset>
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>
    <div class="row"><div class="totals"></div><span class="spacer"></span>
      <button data-limpiar>Limpiar</button><button class="primary" data-guardar>Guardar venta</button></div>`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  let sel = null;
  const pintar = () => {
    sel = null;
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Producto', k: 'nombre' }, { t: 'Cantidad', k: 'cantidad', num: 1 },
      { t: 'Precio', k: 'precio_venta', num: 1, f: M() }, { t: 'Importe', k: 'importe', num: 1, f: M() },
    ], lineas.map((l) => ({ ...l, importe: l.precio_venta * l.cantidad })), { onSelect: (r) => sel = r }));
    const sub = lineas.reduce((a, l) => a + l.precio_venta * l.cantidad, 0);
    const desc = Math.round(sub * (+f('desc').value || 0)) / 100;
    c.querySelector('.totals').innerHTML = `<span>Subtotal:</span><span>${money(sub)}</span><span>Descuento:</span><span>${money(desc)}</span>
      <span><b>Total:</b></span><b>${money(sub - desc)}</b>`;
  };
  f('desc').oninput = pintar;
  c.querySelector('[data-agregar]').onclick = () => {
    const p = prod(f('producto').value), cant = parseInt(f('cant').value, 10);
    if (!(cant > 0)) { f('cant').classList.add('invalid'); return messageBox('La cantidad debe ser mayor a cero.', { icon: 'error' }); }
    f('cant').classList.remove('invalid');
    const ya = lineas.find((l) => l.id === p.id);
    if (ya) ya.cantidad += cant; else lineas.push({ ...p, cantidad: cant });
    pintar();
  };
  c.querySelector('[data-quitar]').onclick = () => { if (sel) { lineas.splice(lineas.findIndex((l) => l.id === sel.id), 1); pintar(); } };
  c.querySelector('[data-limpiar]').onclick = () => renderVenta(c).catch(showError);
  const guardar = c.querySelector('[data-guardar]');
  guardar.onclick = async () => {
    f('cliente').classList.toggle('invalid', !f('cliente').value);
    if (!f('cliente').value) return messageBox('Selecciona un cliente.', { icon: 'error' });
    if (!lineas.length) return messageBox('Agrega al menos un producto.', { icon: 'error' });
    const total = c.querySelector('.totals b:last-child').textContent;
    if (await messageBox(`¿Registrar la venta por ${total}?`, { icon: 'question', buttons: ['Sí', 'No'], title: 'Confirmar venta' }) !== 'Sí') return;
    guardar.disabled = true;   // evita doble ejecución
    try {
      const r = await api('/api/ventas', {
        id_cliente: f('cliente').value, id_metodo_pago: f('metodo').value, descuento_pct: f('desc').value,
        lineas: lineas.map((l) => ({ id_producto: l.id, cantidad: l.cantidad })),
      });
      await messageBox(`Venta ${r.folio} registrada correctamente.`, { icon: 'ok', title: 'Venta registrada' });
      await renderVenta(c);
    } catch (e) {
      guardar.disabled = false;
      showError(new Error(`La venta no se guardó:\n\n${e.message}`));
    }
  };
  pintar();
}

/* ---------- FrmHistorialVentas ---------- */
async function renderVentas(c) {
  const puedeCancelar = tiene('cancelar_venta');
  c.innerHTML = `<h1>Historial de ventas</h1><div class="sub">Selecciona una venta para ver qué se vendió.</div>
    <fieldset class="groupbox"><legend>Buscar</legend><div class="row">
      <div class="field"><span>Folio o cliente:</span><input name="texto" style="width:220px"></div>
      <div class="field"><span>Estado:</span><select name="estado"><option value="">(Todos)</option><option>Completada</option><option>Cancelada</option></select></div>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button>
      <details class="mas"><summary>Más filtros</summary>
        <div class="field"><span>Desde:</span><input type="date" name="desde" value="2026-01-01"></div>
        <div class="field"><span>Hasta:</span><input type="date" name="hasta" value="${new Date().toISOString().slice(0, 10)}"></div></details>
      <span class="spacer"></span>
      ${puedeCancelar ? '<button data-cancelar style="margin-top:14px">Cancelar venta…</button>' : ''}
    </div></fieldset>
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>
    <fieldset class="groupbox"><legend>Detalle de la venta seleccionada</legend><div class="det"><div class="sub">Selecciona una venta.</div></div></fieldset>`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  let sel = null;
  const cargar = async () => {
    sel = null;
    const qs = new URLSearchParams({ desde: f('desde').value, hasta: f('hasta').value, estado: f('estado').value, texto: f('texto').value });
    const rows = await api('/api/ventas?' + qs);
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Cliente', k: 'cliente' },
      { t: 'Total', k: 'total', num: 1, f: M() }, { t: 'Estado', k: 'estado', f: pillEstado },
    ], rows, {
      rowClass: (r) => (r.estado === 'Cancelada' ? 'danger' : ''),
      onSelect: async (r) => {
        sel = r;
        const det = await api('/api/ventas/detalle?id=' + r.id_venta);
        c.querySelector('.det').replaceChildren(grid([
          { t: 'Producto', k: 'nombre' }, { t: 'No. de serie', k: 'numero_serie' }, { t: 'Cant.', k: 'cantidad', num: 1 },
          { t: 'Importe', k: 'subtotal', num: 1, f: M() }, { t: 'Garantía', k: 'garantia' },
        ], det, { cls: 'short' }));
      },
    }));
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  if (puedeCancelar) c.querySelector('[data-cancelar]').onclick = async () => {
    if (!sel) return messageBox('Selecciona una venta.', { icon: 'info' });
    if (sel.estado === 'Cancelada') return messageBox(`La venta ${sel.folio} ya está cancelada.`, { icon: 'info' });
    const motivo = await inputBox(`Motivo de la cancelación de ${sel.folio}:`, 'Cancelar venta');
    if (!motivo) return;
    if (await messageBox(`¿Cancelar la venta ${sel.folio} por ${money(sel.total)}?\nSe reintegrará el stock y se liberarán las piezas con número de serie.`, { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await api('/api/ventas/cancelar', { id_venta: sel.id_venta, motivo }); await messageBox(`Venta ${sel.folio} cancelada.`, { icon: 'ok' }); await cargar(); }
    catch (e) { showError(e); }
  };
  await cargar();
}

/* ---------- FrmOrdenesServicio ---------- */
async function renderOrdenes(c) {
  const estados = state.cat.estados;
  const gestiona = tiene('gestionar_ordenes');
  c.innerHTML = `<h1>Órdenes de servicio (taller)</h1><div class="sub">Las órdenes entregadas o canceladas ya no cambian de estado.</div>
    <div class="row"><label><input type="checkbox" name="abiertas" checked> Solo órdenes abiertas</label></div>
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>
    <div class="${gestiona ? 'two' : ''}">
      <fieldset class="groupbox"><legend>Historial de estados</legend><div class="hist"><div class="sub">Selecciona una orden.</div></div></fieldset>
      ${gestiona ? `<fieldset class="groupbox"><legend>Cambiar estado</legend>
        <div class="row"><div class="field"><span>Orden:</span><input name="folio" disabled style="width:110px"></div>
        <div class="field"><span>Nuevo estado:</span><select name="estado">${options(estados)}</select></div>
        <button class="primary" data-aplicar style="margin-top:14px">Aplicar</button></div>
      </fieldset>` : ''}</div>`;
  let sel = null;
  const cargar = async () => {
    const rows = await api('/api/ordenes?abiertas=' + (c.querySelector('[name=abiertas]').checked ? '1' : ''));
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Folio', k: 'folio' }, { t: 'Equipo', k: 'equipo_descripcion' }, { t: 'Cliente', k: 'cliente' },
      { t: 'Estado', k: 'estado', f: (v) => `<span class="pill ${v === 'Cancelada' ? 'roja' : v === 'Lista' ? 'verde' : 'azul'}">${esc(v)}</span>` },
      { t: 'Días', k: 'dias_en_taller', num: 1 },
    ], rows, {
      rowClass: (r) => (r.estado === 'Cancelada' ? 'danger' : ''),
      onSelect: async (r) => {
        sel = r;
        if (gestiona) {
          c.querySelector('[name=folio]').value = r.folio;
          const cur = estados.find((e) => e.nombre === r.estado);
          c.querySelector('[name=estado]').value = Math.min(cur.id + 1, 5);
        }
        const hist = await api('/api/ordenes/historial?id=' + r.id_orden);
        c.querySelector('.hist').replaceChildren(grid([
          { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'De', k: 'de_estado', f: (v) => esc(v || '(alta)') }, { t: 'A', k: 'a_estado' }, { t: 'Usuario', k: 'usuario' },
        ], hist, { cls: 'short' }));
      },
    }));
  };
  c.querySelector('[name=abiertas]').onchange = () => cargar().catch(showError);
  if (gestiona) c.querySelector('[data-aplicar]').onclick = async () => {
    if (!sel) return messageBox('Selecciona una orden.', { icon: 'info' });
    const nuevo = estados.find((e) => e.id === +c.querySelector('[name=estado]').value);
    try {
      await api('/api/ordenes/estado', { id_orden: sel.id_orden, id_estado: nuevo.id });
      await messageBox(`Orden ${sel.folio} → ${nuevo.nombre}.`, { icon: 'ok' });
      await cargar();
    } catch (e) { showError(e); }
  };
  await cargar();
}

/* ---------- FrmReportes ---------- */
async function renderReportes(c) {
  c.innerHTML = `<h1>Reportes</h1><div class="sub">Las fechas aplican al resumen y a las ventas por categoría.</div>
    <div class="row">
      <div class="field"><span>Desde:</span><input type="date" name="desde" value="2026-01-01"></div>
      <div class="field"><span>Hasta:</span><input type="date" name="hasta" value="${new Date().toISOString().slice(0, 10)}"></div>
      <button class="primary" data-gen style="margin-top:14px">Generar</button><span class="spacer"></span>
      <button data-csv style="margin-top:14px">Exportar CSV</button>
    </div>
    <div class="tabs"></div><div class="tabpage"></div>`;
  const TABS = [
    ['resumen', 'Resumen por periodo', [{ t: 'Mes', k: 'mes' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }, { t: 'Ticket promedio', k: 'ticket_promedio', num: 1, f: M() }]],
    ['top', 'Top 10 productos', [{ t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Unidades', k: 'unidades_vendidas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }, { t: 'Utilidad bruta', k: 'utilidad_bruta', num: 1, f: M() }]],
    ['empleados', 'Actividad por empleado', [{ t: 'Empleado', k: 'empleado' }, { t: 'Puesto', k: 'puesto' }, { t: 'Sucursal', k: 'sucursal' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Vendido', k: 'vendido', num: 1, f: M() }, { t: 'Órdenes taller', k: 'ordenes', num: 1 }]],
    ['stock', 'Stock bajo', [{ t: 'Producto', k: 'nombre' }, { t: 'Stock', k: 'stock_actual', num: 1 }, { t: 'Mínimo', k: 'stock_minimo', num: 1 }, { t: 'Faltante', k: 'faltante', num: 1 }, { t: 'Proveedor más barato', k: 'proveedor_sugerido' }]],
    ['categorias', 'Ventas por categoría', [{ t: 'Categoría', k: 'categoria' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Unidades', k: 'unidades', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }]],
  ];
  let data = null, tab = 'resumen';
  const pintar = () => {
    const tabsEl = c.querySelector('.tabs');
    if (!tabsEl.querySelector('[data-t]')) {   // se arman una vez para que el pulgar se deslice
      tabsEl.innerHTML = TABS.map(([k, t]) => `<button data-t="${k}">${t}</button>`).join('');
      tabsEl.querySelectorAll('[data-t]').forEach((b) => b.onclick = () => { tab = b.dataset.t; pintar(); });
    }
    tabsEl.querySelectorAll('[data-t]').forEach((b) => b.classList.toggle('active', b.dataset.t === tab));
    requestAnimationFrame(() => segmentado(tabsEl, 'button', 'active'));
    const [, , cols] = TABS.find((t) => t[0] === tab);
    const page = c.querySelector('.tabpage');
    page.replaceChildren();
    if (tab === 'resumen' || tab === 'categorias') {
      const key = tab === 'resumen' ? 'mes' : 'categoria';
      const max = Math.max(1, ...data[tab].map((r) => r.ingresos));
      page.appendChild(h(`<div class="chart">${data[tab].map((r) => `<div class="bar" title="${esc(money(r.ingresos))}"><i style="height:${(r.ingresos / max) * 100}%"></i><span>${esc(r[key])}</span></div>`).join('')}</div>`));
    }
    page.appendChild(grid(cols, data[tab], { rowClass: tab === 'stock' ? (r) => (r.stock_actual === 0 ? 'danger' : 'warn') : null }));
  };
  const cargar = async () => {
    data = await api('/api/reportes?' + new URLSearchParams({ desde: c.querySelector('[name=desde]').value, hasta: c.querySelector('[name=hasta]').value }));
    // Sin permiso para ver costos, la pestaña de empleados no existe (igual que en la app de C#)
    if (!data.empleados) { const i = TABS.findIndex((t) => t[0] === 'empleados'); if (i >= 0) TABS.splice(i, 1); }
    pintar();
  };
  c.querySelector('[data-gen]').onclick = () => cargar().catch(showError);
  c.querySelector('[data-csv]').onclick = () => {
    const [, nombre, cols] = TABS.find((t) => t[0] === tab);
    const csv = [cols.map((x) => x.t).join(','), ...data[tab].map((r) => cols.map((x) => `"${String(r[x.k] ?? '').replace(/"/g, '""')}"`).join(','))].join('\n');
    const a = document.createElement('a');
    a.href = URL.createObjectURL(new Blob(['﻿' + csv], { type: 'text/csv' }));
    a.download = `${nombre}.csv`; a.click();
  };
  await cargar();
}

/* ---------- FrmAuditoria ---------- */
async function renderAuditoria(c) {
  const rows = await api('/api/auditoria');
  c.innerHTML = `<h1>Auditoría</h1><div class="sub">Se registra sola cada vez que cambia un precio, el inventario o una venta. Nadie puede editarla ni borrarla.</div>
    ${rows.length ? '' : '<div class="banner">Aún no hay cambios auditados: registra o cancela una venta y vuelve aquí.</div>'}
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>`;
  c.querySelector('.slot').appendChild(grid([
    { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Usuario', k: 'usuario_app' }, { t: 'Qué cambió', k: 'tabla' },
    { t: 'Antes', k: 'valor_anterior' }, { t: 'Después', k: 'valor_nuevo' }, { t: 'Motivo', k: 'motivo' },
  ], rows));
}

// InputBox (Microsoft.VisualBasic.Interaction.InputBox en la app de C#)
function inputBox(text, title = 'TecnoStore') {
  return new Promise((resolve) => {
    const el = h(`<div class="overlay"><div class="msgbox">
      <div class="titlebar"><span class="title" style="padding-left:12px">${esc(title)}</span><div class="caption"><span class="close">✕</span></div></div>
      <div class="msg" style="flex-direction:column;align-items:stretch;gap:8px"><div>${esc(text)}</div><input maxlength="200"></div>
      <div class="buttons"><button class="primary" data-ok>Aceptar</button><button data-no>Cancelar</button></div>
    </div></div>`);
    const input = el.querySelector('input');
    const close = (v) => { el.remove(); resolve(v); };
    el.querySelector('[data-ok]').onclick = () => close(input.value.trim() || null);
    el.querySelector('[data-no]').onclick = () => close(null);
    el.querySelector('.close').onclick = () => close(null);
    input.onkeydown = (e) => { if (e.key === 'Enter') close(input.value.trim() || null); if (e.key === 'Escape') close(null); };
    document.body.appendChild(el);
    input.focus();
  });
}

// Pantalla de alta/edición con estados lectura → edición (patrón de los CRUD en C#)
function crud(c, { campos, puede, alGuardar, alEliminar }) {
  const panel = c.querySelector('.crud');
  panel.innerHTML = `<fieldset class="groupbox"><legend>Datos (lectura)</legend><div class="campos">${campos.map((f) =>
    f.tipo === 'check' ? `<label><input type="checkbox" name="${f.k}"> ${esc(f.t)}</label>`
      : f.tipo === 'select' ? `<div class="field"><span>${esc(f.t)}</span><select name="${f.k}">${options(f.opciones, '')}</select></div>`
      : `<div class="field"><span>${esc(f.t)}</span><input name="${f.k}" ${f.tipo === 'number' ? 'type="number" step="0.01" min="0"' : ''}
          ${f.tipo === 'password' ? 'type="password"' : ''} maxlength="${f.max || 120}" ${f.mayus ? 'style="text-transform:uppercase"' : ''}></div>`).join('')}
      <div class="sub ayuda"></div></div></fieldset>
    <div class="row"><button data-nuevo>Nuevo</button><button data-editar>Editar</button>${alEliminar ? '<button data-eliminar>Eliminar</button>' : ''}</div>
    <div class="row"><span class="spacer"></span><button class="primary" data-guardar>Guardar</button><button data-cancelar-ed>Cancelar</button></div>`;
  const f = (k) => panel.querySelector(`[name=${k}]`);
  const st = { id: null, edicion: false, fila: null };
  const pintar = () => {
    panel.querySelector('fieldset').disabled = !st.edicion;
    panel.querySelector('legend').textContent = !st.edicion ? 'Datos (lectura)' : st.id ? 'Editando' : 'Nuevo registro';
    panel.querySelector('[data-nuevo]').disabled = st.edicion || !puede;
    panel.querySelector('[data-editar]').disabled = st.edicion || !puede || !st.id;
    if (alEliminar) panel.querySelector('[data-eliminar]').disabled = st.edicion || !puede || !st.id;
    panel.querySelector('[data-guardar]').disabled = !st.edicion;
    panel.querySelector('[data-cancelar-ed]').disabled = !st.edicion;
    c.querySelector('.slot').style.pointerEvents = st.edicion ? 'none' : '';
    c.querySelector('.slot').style.opacity = st.edicion ? 0.6 : 1;
  };
  const llenar = (r) => campos.forEach((x) => {
    const el = f(x.k); el.classList.remove('invalid');
    if (x.tipo === 'check') el.checked = r ? !!r[x.k] : (x.defecto ?? true);
    else el.value = r ? (r[x.k] ?? '') : (x.defecto ?? '');
  });
  const api_ = {
    seleccionar(r) { if (st.edicion) return; st.id = r[campos.idKey || 'id']; st.fila = r; llenar(r); pintar(); },
    ayuda(t) { panel.querySelector('.ayuda').textContent = t || ''; },
    get id() { return st.id; },
  };
  panel.querySelector('[data-nuevo]').onclick = () => { st.id = null; llenar(null); st.edicion = true; pintar(); api_.onModo?.(true); panel.querySelector('input,select').focus(); };
  panel.querySelector('[data-editar]').onclick = () => { st.edicion = true; pintar(); api_.onModo?.(false); };
  panel.querySelector('[data-cancelar-ed]').onclick = () => { st.edicion = false; if (st.fila) llenar(st.fila); pintar(); };
  panel.querySelector('[data-guardar]').onclick = async () => {
    const datos = { id: st.id };
    let ok = true;
    campos.forEach((x) => {
      const el = f(x.k);
      datos[x.k] = x.tipo === 'check' ? el.checked : el.value.trim();
      const falta = x.req && !datos[x.k] && !(x.reqSoloNuevo && st.id);
      el.classList.toggle('invalid', !!falta);
      if (falta) ok = false;
    });
    if (!ok) return messageBox('Llena los campos obligatorios (*).', { icon: 'error' });
    const btn = panel.querySelector('[data-guardar]');
    btn.disabled = true;
    try {
      await alGuardar(datos);
      await messageBox(st.id ? 'Cambios guardados.' : 'Registro agregado.', { icon: 'ok' });
      st.edicion = false; pintar();
    } catch (e) { btn.disabled = false; showError(e); }
  };
  if (alEliminar) panel.querySelector('[data-eliminar]').onclick = async () => {
    if (await messageBox('¿Eliminar el registro seleccionado?\nSi está en uso no se podrá eliminar.', { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await alEliminar(st.id); st.id = null; llenar(null); pintar(); } catch (e) { showError(e); }
  };
  llenar(null); pintar();
  return api_;
}

/* ---------- FrmCatalogo (Categorías, Marcas, Tipos de servicio) ---------- */
async function renderCatalogo(c, tipo) {
  const cfg = {
    categorias: { titulo: 'Categorías', c2: 'Descripción', max: 200 },
    marcas: { titulo: 'Marcas', c2: 'País de origen', max: 60 },
    tipos: { titulo: 'Tipos de servicio', c2: 'Precio base', num: true },
  }[tipo];
  const admin = tiene('editar_catalogos');
  c.innerHTML = `<h1>${cfg.titulo}${admin ? '' : '<span class="solo-lectura">Solo lectura</span>'}</h1>
    <div class="sub">${admin ? 'Lo que ya está en uso no se puede eliminar.' : 'Tu perfil puede consultar este catálogo, no modificarlo.'}</div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Nombre:</span><input name="texto"></div><button data-buscar style="margin-top:14px">Buscar</button>
      <span class="sub count" style="margin:14px 0 0"></span></div></fieldset>
    <div class="split"><div class="slot" style="flex:1;display:flex;min-height:0"></div>${admin ? '<div class="crud"></div>' : ''}</div>`;
  const form = !admin ? { seleccionar() {} } : crud(c, {
    campos: [{ k: 'nombre', t: 'Nombre: *', req: true, max: 80 }, { k: 'campo2', t: cfg.c2 + ':', tipo: cfg.num ? 'number' : 'text', max: cfg.max }],
    puede: admin,
    alGuardar: async (d) => { await api('/api/catalogo', { tipo, id: d.id, nombre: d.nombre, campo2: d.campo2 }); await cargar(); },
    alEliminar: async (id) => { await api('/api/catalogo/eliminar', { tipo, id }); await cargar(); },
  });
  const cargar = async () => {
    const rows = await api('/api/catalogo?' + new URLSearchParams({ tipo, texto: c.querySelector('[name=texto]').value }));
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Nombre', k: 'nombre' }, { t: cfg.c2, k: 'campo2', num: cfg.num, f: cfg.num ? M() : N },
      { t: tipo === 'tipos' ? 'Usado en órdenes' : 'Productos', k: 'en_uso', num: 1 },
      ...(tipo === 'tipos' ? [{ t: 'Técnicos habilitados', k: 'tecnicos', num: 1 }] : []),
    ], rows, { onSelect: (r) => form.seleccionar(r) }));
    c.querySelector('.count').textContent = `${rows.length} registro(s)`;
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  c.querySelector('[name=texto]').onkeydown = (e) => { if (e.key === 'Enter') cargar().catch(showError); };
  await cargar();
}

/* ---------- FrmProveedores ---------- */
async function renderProveedores(c) {
  const puede = tiene('guardar_proveedor');
  c.innerHTML = `<h1>Proveedores${puede ? '' : '<span class="solo-lectura">Solo lectura</span>'}</h1><div class="sub">Selecciona un proveedor para ver los productos que surte.</div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Razón social o RFC:</span><input name="texto" style="width:240px"></div>
      <button ${puede ? '' : 'class="primary"'} data-buscar style="margin-top:14px">Buscar</button><span class="sub count" style="margin:14px 0 0"></span></div></fieldset>
    <div class="split"><div class="slot" style="flex:1;display:flex;min-height:0"></div>${puede ? '<div class="crud"></div>' : ''}</div>
    <fieldset class="groupbox"><legend>Productos que surte</legend><div class="det"><div class="sub">Selecciona un proveedor.</div></div></fieldset>`;
  const form = !puede ? { seleccionar() {} } : crud(c, {
    campos: Object.assign([
      { k: 'razon_social', t: 'Razón social: *', req: true, max: 120 },
      { k: 'rfc', t: 'RFC: *', req: true, max: 13, mayus: true },
      { k: 'correo', t: 'Correo:', max: 100 }, { k: 'telefono', t: 'Teléfono:', max: 20 },
      { k: 'activo', t: 'Activo', tipo: 'check' },
    ], { idKey: 'id_proveedor' }),
    puede,
    alGuardar: async (d) => { await api('/api/proveedores', d); await cargar(); },
  });
  const cargar = async () => {
    const rows = await api('/api/proveedores?' + new URLSearchParams({ texto: c.querySelector('[name=texto]').value }));
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Razón social', k: 'razon_social' }, { t: 'RFC', k: 'rfc' }, { t: 'Teléfono', k: 'telefono' },
      { t: 'Productos', k: 'productos', num: 1 },
    ], rows, {
      rowClass: (r) => (r.activo ? '' : 'danger'),
      onSelect: async (r) => {
        form.seleccionar(r);
        const det = await api('/api/proveedores/productos?id=' + r.id_proveedor);
        c.querySelector('.det').replaceChildren(grid([
          { t: 'Producto', k: 'nombre' }, { t: 'Costo del proveedor', k: 'costo_proveedor', num: 1, f: M() }, { t: 'Stock', k: 'stock_actual', num: 1 },
        ], det, { cls: 'short' }));
      },
    }));
    c.querySelector('.count').textContent = `${rows.length} proveedor(es)`;
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  await cargar();
}

/* ---------- FrmCompra (maestro-detalle) ---------- */
async function renderCompras(c) {
  const d = await api('/api/compras');
  const lineas = [];
  c.innerHTML = `<h1>Compras a proveedor</h1><div class="sub">El inventario sube en cuanto guardas la compra.</div>
    <fieldset class="groupbox"><legend>Encabezado</legend><div class="row">
      <div class="field"><span>Proveedor: *</span><select name="proveedor" style="width:320px">${options(d.proveedores, '', '(Selecciona)')}</select></div>
      <div class="field"><span>Recibe:</span><input value="${esc(state.user.nombre)}" disabled style="width:200px"></div></div></fieldset>
    <fieldset class="groupbox"><legend>Agregar producto</legend><div class="row" style="align-items:flex-start">
      <div class="field"><span>Producto:</span><select name="producto" style="width:360px">${d.productos.map((p) =>
        `<option value="${p.id}">${esc(p.sku)} — ${esc(p.nombre)}${p.requiere_serie ? '  [con número de serie]' : ''}</option>`).join('')}</select>
        <div class="row" style="margin-top:6px"><div class="field"><span>Cantidad:</span><input name="cant" type="number" min="1" value="1" style="width:70px"></div>
        <div class="field"><span>Costo unitario:</span><input name="costo" type="number" min="1" step="0.01" style="width:110px"></div></div></div>
      <div class="field"><span class="lbl-series">Números de serie (uno por renglón):</span><textarea name="series" rows="4" style="width:220px;font:inherit;border:1px solid #7a7a7a"></textarea></div>
      <div style="display:flex;flex-direction:column;gap:6px;margin-top:14px"><button data-agregar>Agregar ↓</button><button data-quitar>Quitar línea</button></div>
    </div></fieldset>
    <div class="slot" style="flex:1;display:flex;min-height:110px"></div>
    <div class="row"><b class="total" style="font-size:15px"></b><span class="spacer"></span><button data-limpiar>Limpiar</button><button class="primary" data-guardar>Guardar compra</button></div>
    <fieldset class="groupbox"><legend>Compras recientes</legend><div class="recientes"></div></fieldset>`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  const prod = () => d.productos.find((p) => p.id === +f('producto').value);
  let sel = null;
  const alCambiarProducto = () => {
    const p = prod();
    f('costo').value = p.costo;
    f('series').disabled = !p.requiere_serie;
    if (!p.requiere_serie) f('series').value = '';
    c.querySelector('.lbl-series').textContent = p.requiere_serie ? 'Números de serie (uno por renglón): *' : 'Números de serie (no aplica):';
  };
  const pintar = () => {
    sel = null;
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Producto', k: 'nombre' }, { t: 'Cantidad', k: 'cantidad', num: 1 },
      { t: 'Costo', k: 'costo_unitario', num: 1, f: M() }, { t: 'Importe', k: 'importe', num: 1, f: M() }, { t: 'Series', k: 'series_txt' },
    ], lineas.map((l) => ({ ...l, importe: l.cantidad * l.costo_unitario, series_txt: (l.series || []).join(', ') })), { onSelect: (r) => sel = r }));
    c.querySelector('.total').textContent = 'Total: ' + money(lineas.reduce((a, l) => a + l.cantidad * l.costo_unitario, 0));
  };
  const recientes = (rows) => c.querySelector('.recientes').replaceChildren(grid([
    { t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Proveedor', k: 'proveedor' }, { t: 'Total', k: 'total', num: 1, f: M() },
  ], rows, { cls: 'short' }));
  f('producto').onchange = alCambiarProducto;
  c.querySelector('[data-agregar]').onclick = () => {
    const p = prod(), cantidad = parseInt(f('cant').value, 10), costo = +f('costo').value;
    if (!(cantidad > 0) || !(costo > 0)) return messageBox('Cantidad y costo deben ser mayores a cero.', { icon: 'error' });
    if (lineas.some((l) => l.id_producto === p.id)) return messageBox('Ese producto ya está en la compra; quítalo y agrégalo con la cantidad total.', { icon: 'info' });
    const series = f('series').value.split('\n').map((x) => x.trim().toUpperCase()).filter(Boolean);
    if (p.requiere_serie && series.length !== cantidad) return messageBox(`Captura ${cantidad} número(s) de serie, uno por renglón (llevas ${series.length}).`, { icon: 'info' });
    if (new Set(series).size !== series.length) return messageBox('Hay números de serie repetidos.', { icon: 'info' });
    lineas.push({ id_producto: p.id, sku: p.sku, nombre: p.nombre, cantidad, costo_unitario: costo, ...(p.requiere_serie ? { series } : {}) });
    f('cant').value = 1; f('series').value = '';
    pintar();
  };
  c.querySelector('[data-quitar]').onclick = () => { if (sel) { lineas.splice(lineas.findIndex((l) => l.id_producto === sel.id_producto), 1); pintar(); } };
  c.querySelector('[data-limpiar]').onclick = () => renderCompras(c).catch(showError);
  const guardar = c.querySelector('[data-guardar]');
  guardar.onclick = async () => {
    f('proveedor').classList.toggle('invalid', !f('proveedor').value);
    if (!f('proveedor').value) return messageBox('Selecciona el proveedor.', { icon: 'error' });
    if (!lineas.length) return messageBox('Agrega al menos un producto.', { icon: 'error' });
    if (await messageBox(`¿Registrar la compra por ${c.querySelector('.total').textContent.replace('Total: ', '')}?\nEl stock entra al guardar.`, { icon: 'question', buttons: ['Sí', 'No'], title: 'Confirmar compra' }) !== 'Sí') return;
    guardar.disabled = true;
    try {
      const r = await api('/api/compras', { id_proveedor: f('proveedor').value, lineas: lineas.map(({ sku, nombre, ...l }) => l) });
      await messageBox(`Compra ${r.folio} registrada; el inventario ya se actualizó.`, { icon: 'ok', title: 'Compra registrada' });
      await renderCompras(c);
    } catch (e) {
      guardar.disabled = false;
      showError(new Error(`La compra no se guardó:\n\n${e.message}`));
    }
  };
  alCambiarProducto(); pintar(); recientes(d.recientes);
}

/* ---------- FrmUsuarios ---------- */
async function renderUsuarios(c) {
  const d = await api('/api/usuarios');
  c.innerHTML = `<h1>Usuarios y roles</h1><div class="sub">El rol decide qué pantallas y botones ve cada persona. La contraseña se guarda cifrada y nunca se muestra.</div>
    <div class="split"><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="crud"></div></div>`;
  const form = crud(c, {
    campos: Object.assign([
      { k: 'id_empleado', t: 'Empleado: *', tipo: 'select', opciones: d.empleados, req: true },
      { k: 'usuario', t: 'Usuario: *', req: true, max: 40 },
      { k: 'id_rol', t: 'Rol: *', tipo: 'select', opciones: d.roles, req: true },
      { k: 'contrasena', t: 'Contraseña:', tipo: 'password', req: true, reqSoloNuevo: true, max: 100 },
      { k: 'activo', t: 'Activo (desmarcar = bloquear)', tipo: 'check' },
    ], { idKey: 'id_usuario' }),
    puede: true,
    alGuardar: async (x) => { await api('/api/usuarios', x); await renderUsuarios(c); },
  });
  form.ayuda('Contraseña nueva: 8+ caracteres y un número. Al editar, déjala vacía para conservar la actual.');
  c.querySelector('.slot').replaceChildren(grid([
    { t: 'Usuario', k: 'usuario' }, { t: 'Empleado', k: 'empleado' }, { t: 'Rol', k: 'rol' },
    { t: 'Activo', k: 'activo', f: (v) => (v ? 'Sí' : 'Bloqueado') }, { t: 'Último acceso', k: 'ultimo_acceso', f: datetime },
  ], d.usuarios, { rowClass: (r) => (r.activo ? '' : 'danger'), onSelect: (r) => form.seleccionar({ ...r, contrasena: '' }) }));
}

/* ---------- FrmAjusteStock ---------- */
async function renderAjustes(c) {
  const d = await api('/api/ajustes');
  const stock = tiene('ajustar_stock'), precio = tiene('ajustar_precio');
  c.innerHTML = `<h1>Ajustes de inventario y precios</h1><div class="sub">Cada cambio queda en la auditoría con su motivo.</div>
    ${stock ? `<fieldset class="groupbox"><legend>Ajuste de inventario · máximo ±20 piezas</legend>
      <div class="row"><div class="field"><span>Producto (sin número de serie):</span><select name="producto" style="width:420px">${options(d.productos)}</select></div>
      <div class="field"><span>Cantidad (+ entra, − sale):</span><input name="cant" type="number" min="-20" max="20" value="0" style="width:90px"></div></div>
      <div class="row" style="margin-top:6px"><div class="field" style="flex:1"><span>Motivo: *</span><input name="motivo" maxlength="200"></div>
      <button class="primary" data-stock style="margin-top:14px">Aplicar ajuste</button></div></fieldset>` : ''}
    ${precio ? `<fieldset class="groupbox"><legend>Cambio de precios por categoría · máximo ±30 %</legend>
      <div class="row"><div class="field"><span>Categoría:</span><select name="categoria" style="width:260px">${options(d.categorias)}</select></div>
      <div class="field"><span>Porcentaje:</span><input name="pct" type="number" min="-30" max="30" step="0.5" value="0" style="width:90px"></div>
      <button ${stock ? '' : 'class="primary"'} data-precio style="margin-top:14px">Aplicar precios</button></div>
      <div class="sub" style="margin-top:6px">Si algún precio quedara por debajo de su costo, no se cambia ninguno.</div></fieldset>` : ''}`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  if (stock) c.querySelector('[data-stock]').onclick = async () => {
    const cant = parseInt(f('cant').value, 10);
    f('motivo').classList.toggle('invalid', !f('motivo').value.trim());
    if (!cant) return messageBox('La cantidad no puede ser cero.', { icon: 'error' });
    if (!f('motivo').value.trim()) return messageBox('El motivo es obligatorio.', { icon: 'error' });
    const nombre = f('producto').selectedOptions[0].textContent;
    if (await messageBox(`¿Ajustar ${cant > 0 ? '+' : ''}${cant} pieza(s) de\n${nombre}?`, { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await api('/api/ajustes/stock', { id_producto: f('producto').value, cantidad: cant, motivo: f('motivo').value.trim() });
      await messageBox('Ajuste aplicado y registrado en la auditoría.', { icon: 'ok' }); await renderAjustes(c); }
    catch (e) { showError(e); }
  };
  if (precio) c.querySelector('[data-precio]').onclick = async () => {
    const pct = +f('pct').value;
    if (!pct) return messageBox('El porcentaje no puede ser cero.', { icon: 'error' });
    const cat = f('categoria').selectedOptions[0].textContent;
    if (await messageBox(`¿Cambiar ${pct > 0 ? '+' : ''}${pct} % el precio de todos los productos activos de ${cat}?`, { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await api('/api/ajustes/precio', { porcentaje: pct, id_categoria: f('categoria').value });
      await messageBox('Precios actualizados; cada cambio quedó en la auditoría.', { icon: 'ok' }); f('pct').value = 0; }
    catch (e) { showError(new Error(`No se cambiaron los precios:\n\n${e.message}`)); }
  };
}

// Captura automática para la documentación: ?usuario=admin&modulo=productos
// entra con un usuario demo y abre esa pantalla (solo usuarios demo con su contraseña conocida).
async function inicio() {
  const p = new URLSearchParams(location.search);
  // Modo captura (?usuario=… o ?captura): sin animaciones, para que Chrome sin ventana fotografíe el estado final
  if (p.has('usuario') || p.has('captura')) document.documentElement.classList.add('sin-animacion');
  const demo = ['admin', 'csupervisor', 'loperador', 'fconsulta'];
  if (!demo.includes(p.get('usuario'))) return renderLogin();
  try {
    const r = await api('/api/login', { usuario: p.get('usuario'), contrasena: '1234' });
    state.token = r.token; state.user = r.user;
    state.cat = await api('/api/catalogos');
    state.modulo = r.user.modulos.includes(p.get('modulo')) ? p.get('modulo') : 'inicio';
    renderMain();
  } catch (e) { renderLogin(); }
}
inicio();
