// Vista previa de la app Windows Forms de TecnoStore.
// Mismas pantallas, textos y permisos que 03_WindowsForms, conectada a SQL Server.
'use strict';

const $app = document.getElementById('app');
const state = { token: null, user: null, cat: null, vista: 'inicio', ultimas: {} };

/* ---------- utilidades ---------- */
const esc = (v) => String(v ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
const money = (v, d = 2) => (v == null ? '' : Number(v).toLocaleString('es-MX', { style: 'currency', currency: 'MXN', minimumFractionDigits: d, maximumFractionDigits: d }));
const datetime = (v) => (v ? new Date(v).toLocaleString('es-MX', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', timeZone: 'UTC' }) : '');
const isoDia = (d) => d.toISOString().slice(0, 10);
const haceMeses = (n) => { const d = new Date(); d.setMonth(d.getMonth() - n); return isoDia(d); };
const h = (html) => { const t = document.createElement('template'); t.innerHTML = html.trim(); return t.content.firstElementChild; };
const options = (list, sel, empty) => (empty ? `<option value="">${esc(empty)}</option>` : '') +
  list.map((o) => `<option value="${o.id}" ${String(o.id) === String(sel) ? 'selected' : ''}>${esc(o.nombre)}</option>`).join('');

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

// Capacidades que SQL Server concede al rol (usp_PermisosApp); la interfaz solo muestra lo que se puede usar
const tiene = (cap) => !!state.user?.caps?.includes(cap);
const ES_COSTO = /costo|margen|utilidad|valor_inventario/i;

/* ---------- piezas (equivalen a Utils/Sencillo.cs) ---------- */
// Aviso breve que no interrumpe
function aviso(texto) {
  document.querySelector('.aviso')?.remove();
  const el = h(`<div class="aviso" role="status">${esc(texto)}</div>`);
  document.body.appendChild(el);
  setTimeout(() => el.remove(), 2600);
}

// Ventana de una sola tarea: título, contenido y botones grandes. botones: [texto, clase, acción]
// (la acción devuelve true para cerrar). Esc cierra.
function ficha(titulo, cuerpo, botones, { ancha = false } = {}) {
  const el = h(`<div class="overlay"><div class="ficha ${ancha ? 'ancha' : ''}" role="dialog" aria-label="${esc(titulo)}"><h2>${esc(titulo)}</h2><div class="cuerpo"></div><div class="pie"></div></div></div>`);
  if (typeof cuerpo === 'string') el.querySelector('.cuerpo').innerHTML = cuerpo; else el.querySelector('.cuerpo').append(cuerpo);
  const pie = el.querySelector('.pie');
  const cerrar = () => { el.remove(); document.removeEventListener('keydown', tecla, true); };
  const tecla = (e) => { if (e.key === 'Escape' && el === [...document.querySelectorAll('.overlay')].pop()) { e.stopPropagation(); cerrar(); el.alCerrar?.(); } };
  document.addEventListener('keydown', tecla, true);
  const peligro = botones.filter((b) => b[1] === 'peligro'), resto = botones.filter((b) => b[1] !== 'peligro');
  const poner = ([t, clase, accion]) => {
    const b = h(`<button class="${clase || ''}">${esc(t)}</button>`);
    b.onclick = async () => { b.disabled = true; try { if (!accion || await accion()) cerrar(); } catch (e) { error(e); } finally { b.disabled = false; } };
    pie.append(b);
  };
  peligro.forEach(poner); pie.append(h('<span class="spacer"></span>')); resto.forEach(poner);
  document.body.append(el);
  (el.querySelector('.cuerpo input:not([disabled]), .cuerpo select:not([disabled])') || el.querySelector('.primary') || pie.lastElementChild)?.focus();
  el.cerrar = cerrar;
  return el;
}
const error = (e) => ficha('No se pudo', `<div class="mensaje">${esc(e.message || e)}</div>`, [['Entendido', 'primary']]);
const informar = (t) => ficha('TecnoStore', `<div class="mensaje">${esc(t)}</div>`, [['Entendido', 'primary']]);
const confirmar = (t, boton = 'Sí') => new Promise((ok) => {
  const el = ficha('Confirmar', `<div class="mensaje">${esc(t)}</div>`, [['No', '', () => { ok(false); return true; }], [boton, 'primary', () => { ok(true); return true; }]]);
  el.alCerrar = () => ok(false);
});
const preguntar = (titulo, etiqueta, boton) => new Promise((ok) => {
  const el = ficha(titulo, `<div class="field"><span>${esc(etiqueta)}</span><input maxlength="200"></div>`, [['Regresar', '', () => { ok(null); return true; }], [boton, 'primary', () => {
    const i = el.querySelector('input'); if (!i.value.trim()) { i.classList.add('invalid'); return false; } ok(i.value.trim()); return true; }]]);
  el.alCerrar = () => ok(null);
});

// Campos: { k, t, tipo: text|number|select|check|password|textarea, req, opciones, ancho, oculto }
function campos(lista, valores = {}, { una = false, lectura = false } = {}) {
  const el = h(`<div class="campos ${una ? 'una' : ''}"></div>`);
  for (const f of lista) {
    if (f.oculto) continue;
    const at = `name="${f.k}" ${lectura ? 'disabled' : ''} maxlength="${f.max || 200}"`;
    const v = valores[f.k] ?? f.defecto ?? '';
    el.append(h(f.tipo === 'check' ? `<label class="check ${f.ancho ? 'ancho' : ''}"><input type="checkbox" ${at} ${v ? 'checked' : ''}> ${esc(f.t)}</label>`
      : f.tipo === 'texto' ? `<div class="ayuda ${f.ancho ? 'ancho' : ''}">${esc(f.t)}</div>`
      : `<div class="field ${f.ancho ? 'ancho' : ''}" data-campo="${f.k}"><span>${esc(f.t)}</span>${f.tipo === 'select' ? `<select ${at}>${options(f.opciones, v, f.vacio)}</select>`
        : f.tipo === 'textarea' ? `<textarea ${at}>${esc(v)}</textarea>`
        : `<input ${at} type="${f.tipo || 'text'}" ${f.tipo === 'number' ? 'step="any"' : ''} value="${esc(v)}" ${f.min != null ? `min="${f.min}" max="${f.maxn}"` : ''}>`}</div>`));
  }
  el.leer = () => {
    const d = {}; let ok = true;
    for (const f of lista) {
      const c = el.querySelector(`[name="${f.k}"]`); if (!c) continue;
      d[f.k] = f.tipo === 'check' ? c.checked : c.value.trim();
      const falta = f.req && !d[f.k]; c.classList.toggle('invalid', !!falta); if (falta) ok = false;
    }
    return ok ? d : null;
  };
  el.c = (k) => el.querySelector(`[name="${k}"]`);
  return el;
}

// Selector de vistas o filtros con botones grandes
function seg(opciones, activo, alElegir) {
  const el = h(`<div class="seg">${opciones.map((o, i) => `<button class="${i === activo ? 'on' : ''}">${esc(o)}</button>`).join('')}</div>`);
  el.activo = activo;
  [...el.children].forEach((b, i) => b.onclick = () => { el.activo = i; [...el.children].forEach((x, k) => x.classList.toggle('on', k === i)); alElegir?.(i); });
  return el;
}

// Tabla: pocas columnas; las de costo desaparecen si el rol no puede verlas
function grid(cols, rows, { onSelect, onOpen, rowClass, cls = '' } = {}) {
  if (!tiene('ver_costos')) cols = cols.filter((x) => !ES_COSTO.test(x.k));
  const el = h(`<div class="grid ${cls}"></div>`);
  if (!rows.length) { el.innerHTML = '<div class="empty">No hay nada que mostrar</div>'; return el; }
  el.innerHTML = `<table><thead><tr>${cols.map((c) => `<th class="${c.num ? 'num' : ''}">${esc(c.t)}</th>`).join('')}</tr></thead>
    <tbody>${rows.map((r, i) => `<tr data-i="${i}" class="${rowClass ? rowClass(r) : ''}">${cols.map((c) =>
      `<td class="${c.num ? 'num' : ''}">${c.f ? c.f(r[c.k], r) : esc(r[c.k])}</td>`).join('')}</tr>`).join('')}</tbody></table>`;
  const elegir = (i) => {
    el.querySelectorAll('tr.sel').forEach((x) => x.classList.remove('sel'));
    el.querySelector(`tr[data-i="${i}"]`).classList.add('sel');
    el.sel = rows[i]; onSelect?.(rows[i]);
  };
  el.querySelectorAll('tbody tr').forEach((tr) => {
    tr.onclick = () => elegir(+tr.dataset.i);
    if (onOpen) tr.ondblclick = () => onOpen(rows[+tr.dataset.i]);
  });
  if (onOpen || onSelect) { el.sel = rows[0]; el.querySelector('tbody tr').classList.add('sel'); }
  return el;
}
const $$ = () => (v) => money(v);

// Pantalla de lista: buscador que busca solo, filtros y botones; doble clic abre
function lista(c, { pista, filtros = [], botones = [], abrir, textoAbrir = 'Abrir', datos, cols, rowClass }) {
  c.innerHTML = '<div class="barra"></div><div class="card tabla"></div>';
  const barra = c.firstElementChild, zona = c.lastElementChild;
  let t, actual;
  const buscar = pista ? h(`<input class="buscar" placeholder="${esc(pista)}">`) : null;
  const cargar = async () => {
    try { actual = grid(cols, await datos(buscar?.value || ''), { onOpen: abrir, rowClass }); zona.replaceChildren(actual); } catch (e) { error(e); }
  };
  if (buscar) { barra.append(buscar); buscar.oninput = () => { clearTimeout(t); t = setTimeout(cargar, 300); }; }
  filtros.forEach((f) => barra.append(f));
  barra.append(h('<span class="spacer"></span>'));
  if (abrir) { const b = h(`<button>${esc(textoAbrir)}</button>`); b.onclick = () => actual?.sel && abrir(actual.sel); barra.append(b); }
  botones.forEach(([texto, accion, clase]) => { const b = h(`<button class="${clase || ''}">${esc(texto)}</button>`); b.onclick = accion; barra.append(b); });
  return { cargar, barra, cargado: cargar() };
}
const tarjeta = (titulo, contenido) => { const el = h(`<div class="card">${titulo ? `<h3>${esc(titulo)}</h3>` : ''}</div>`); el.append(contenido); return el; };
const dosZonas = (izq, der) => { const el = h('<div class="dos"></div>'); el.append(izq, der); return el; };

/* ---------- FrmLogin ---------- */
function renderLogin() {
  $app.innerHTML = '';
  const el = h(`<div class="login"><form autocomplete="off"><h1>TecnoStore</h1>
    <div class="field"><span>Usuario</span><input name="usuario" required></div>
    <div class="field"><span>Contraseña</span><input name="contrasena" type="password" required></div>
    <div class="error"></div><button class="primary">Entrar</button>
    <div class="ayuda">Para probar: <code>admin</code>, <code>csupervisor</code>, <code>loperador</code> o <code>fconsulta</code><br>Contraseña: 1234</div></form></div>`);
  const form = el.querySelector('form');
  el.querySelectorAll('code').forEach((x) => x.onclick = () => { form.usuario.value = x.textContent; form.contrasena.value = '1234'; });
  form.onsubmit = async (ev) => {
    ev.preventDefault();
    const btn = form.querySelector('button'); btn.disabled = true;
    try {
      const r = await api('/api/login', { usuario: form.usuario.value.trim(), contrasena: form.contrasena.value });
      state.token = r.token; state.user = r.user; state.vista = 'inicio'; state.ultimas = {};
      state.cat = await api('/api/catalogos');
      renderMain();
    } catch (e) { el.querySelector('.error').textContent = e.message; form.contrasena.value = ''; } finally { btn.disabled = false; }
  };
  $app.append(el); form.usuario.focus();
}

/* ---------- FrmPrincipal ---------- */
// Un módulo = un enfoque: [clave, título, vistas: [clave, título, pantalla]]
const MODULOS = [
  ['inicio', 'Inicio', [['inicio', 'Inicio', renderInicio]]],
  ['ventas', 'Ventas', [['venta', 'Vender', renderVenta], ['ventas', 'Ventas hechas', renderVentas]]],
  ['taller', 'Taller', [['ordenes', 'Órdenes', renderOrdenes]]],
  ['clientes', 'Clientes', [['clientes', 'Clientes', renderClientes]]],
  ['inventario', 'Inventario', [['productos', 'Productos', renderProductos], ['compras', 'Compras', renderCompras], ['ajustes', 'Ajustes', renderAjustes], ['proveedores', 'Proveedores', renderProveedores]]],
  ['reportes', 'Reportes', [['dashboard', 'Resumen', renderDashboard], ['reportes', 'Reportes', renderReportes], ['auditoria', 'Auditoría', renderAuditoria]]],
  ['config', 'Configuración', [['usuarios', 'Usuarios', renderUsuarios], ['categorias', 'Categorías', (c) => renderCatalogo(c, 'categorias')], ['marcas', 'Marcas', (c) => renderCatalogo(c, 'marcas')], ['tipos', 'Servicios', (c) => renderCatalogo(c, 'tipos')]]],
];
const modulosDelRol = () => MODULOS.map(([k, t, v]) => [k, t, v.filter((x) => state.user.modulos.includes(x[0]))]).filter((m) => m[2].length);
const ICONOS = {
  inicio: '<path d="M3 10.5 12 3l9 7.5V21h-6v-6H9v6H3z"/>',
  ventas: '<path d="M6 7h12l1 13H5z"/><path d="M9 7a3 3 0 0 1 6 0"/>',
  taller: '<path d="M14.7 6.3a4 4 0 0 0-5.4 5.2L3 17.8V21h3.2l6.3-6.3a4 4 0 0 0 5.2-5.4l-2.6 2.6-2.4-.6-.6-2.4z"/>',
  clientes: '<circle cx="9" cy="8" r="3.5"/><path d="M2.5 20a6.5 6.5 0 0 1 13 0"/><path d="M16 4.5a3.5 3.5 0 0 1 0 7M18.5 20a6.5 6.5 0 0 0-3-5.5"/>',
  inventario: '<path d="M21 8 12 3 3 8l9 5 9-5z"/><path d="M3 8v8l9 5 9-5V8"/><path d="M12 13v8"/>',
  reportes: '<path d="M4 20V10M10 20V4M16 20v-7M22 20H2"/>',
  config: '<path d="M4 6h10M18 6h2M4 12h4M12 12h8M4 18h12"/><circle cx="16" cy="6" r="2"/><circle cx="10" cy="12" r="2"/><circle cx="18" cy="18" r="2"/>',
  compras: '<path d="M3 4h2l2.4 11.2a2 2 0 0 0 2 1.6h7.7a2 2 0 0 0 2-1.5L21 8H6"/><circle cx="10" cy="20" r="1.3"/><circle cx="17" cy="20" r="1.3"/>',
};
const icono = (k) => `<svg class="ic" viewBox="0 0 24 24" aria-hidden="true">${ICONOS[k] || ''}</svg>`;
const iniciales = (n) => (n || '?').split(' ').filter(Boolean).slice(0, 2).map((x) => x[0]).join('').toUpperCase();

function renderMain() {
  const u = state.user, mods = modulosDelRol();
  $app.innerHTML = '';
  const el = h(`<div class="app"><aside class="lateral"><div class="marca">TecnoStore</div>
      ${mods.map(([k, t], i) => `<button data-m="${k}">${icono(k)}<span>${esc(t)}</span><kbd>F${i + 1}</kbd></button>`).join('')}</aside>
    <div class="principal"><header class="encabezado"><h2></h2><span class="spacer"></span><div class="avatar">${esc(iniciales(u.nombre))}</div>
      <div class="quien"><b>${esc(u.nombre)}</b><small>${esc(u.rol)} · ${esc(u.sucursal)}</small></div><button data-salir>Salir</button></header>
      <div class="vistas"></div><div class="content"></div></div></div>`);
  el.querySelectorAll('[data-m]').forEach((b) => b.onclick = () => abrirModulo(b.dataset.m));
  el.querySelector('[data-salir]').onclick = async () => { if (await confirmar('¿Cerrar la sesión?', 'Salir')) { state.token = state.user = null; document.onkeydown = null; renderLogin(); } };
  document.onkeydown = (ev) => {
    if (document.querySelector('.overlay')) return;
    const n = /^F(\d)$/.exec(ev.key);
    if (n && mods[n[1] - 1]) { ev.preventDefault(); abrirModulo(mods[n[1] - 1][0]); }
    else if (ev.key === 'Escape' && state.vista !== 'inicio' && !document.activeElement?.value) abrir('inicio');
  };
  $app.append(el);
  abrir(u.modulos.includes(state.vista) ? state.vista : 'inicio');
}
const abrirModulo = (k) => { const m = modulosDelRol().find((x) => x[0] === k); if (m) abrir(state.ultimas[k] || m[2][0][0]); };

async function abrir(vista, parametro) {
  const m = modulosDelRol().find((x) => x[2].some((v) => v[0] === vista));
  if (!m) return;
  state.vista = vista; state.ultimas[m[0]] = vista;
  document.querySelectorAll('.lateral [data-m]').forEach((b) => b.classList.toggle('active', b.dataset.m === m[0]));
  document.querySelector('.encabezado h2').textContent = m[1];
  const barra = document.querySelector('.vistas');
  barra.replaceChildren(...(m[2].length > 1 ? [seg(m[2].map((v) => v[1]), m[2].findIndex((v) => v[0] === vista), (i) => abrir(m[2][i][0]))] : []));
  barra.style.display = m[2].length > 1 ? '' : 'none';
  const viejo = document.querySelector('.content'), c = h('<div class="content"></div>');
  viejo.replaceWith(c);
  try { await m[2].find((v) => v[0] === vista)[2](c, parametro); } catch (e) { error(e); }
}

/* ---------- FrmInicio: lo de todos los días en botones grandes ---------- */
async function renderInicio(c) {
  const k = await api('/api/inicio'), ver = tiene('ver_reportes');
  const hora = new Date().getHours();
  const acciones = [
    tiene('registrar_venta') && ['ventas', 'Nueva venta', 'cobrar a un cliente', () => abrir('venta'), 'primary'],
    tiene('gestionar_ordenes') && ['taller', 'Recibir equipo', 'entra al taller', () => abrir('ordenes', 'recibir')],
    tiene('registrar_compra') && ['compras', 'Llegó mercancía', 'compra a proveedor', () => abrir('compras')],
    !tiene('registrar_venta') && ver && ['reportes', 'Ver reportes', 'ventas e inventario', () => abrir('reportes'), 'primary'],
    ['inventario', 'Buscar producto', 'precio y existencias', () => abrir('productos')],
    !tiene('registrar_compra') && ['clientes', 'Clientes', 'datos y compras', () => abrir('clientes')],
  ].filter(Boolean);
  const numeros = [
    ver && ['Ingresos', money(k.ingresos, 0), 'últimos 90 días', () => abrir('dashboard')],
    ver && ['Ventas', k.ventas, 'últimos 90 días', () => abrir('ventas')],
    ['En taller', k.ordenes_abiertas, 'equipos por atender', () => abrir('ordenes')],
    !ver && ['Listos', k.ordenes_listas, 'para entregar', () => abrir('ordenes')],
    ['Por resurtir', k.alertas_stock, 'productos', () => abrir('productos', 'alertas')],
  ].filter(Boolean);
  c.innerHTML = `<h1 class="saludo">${hora < 12 ? 'Buenos días' : hora < 19 ? 'Buenas tardes' : 'Buenas noches'}, ${esc(state.user.nombre.split(' ')[0])}</h1>
    <div class="fila" style="flex:1.2">${acciones.map(([i, t, n, , cl]) => `<button class="grande ${cl || ''}">${icono(i)}<b>${esc(t)}</b><small>${esc(n)}</small></button>`).join('')}</div>
    <div class="fila">${numeros.map(([t, v, n]) => `<button class="grande"><span class="et">${esc(t)}</span><span class="cifra">${esc(v)}</span><small>${esc(n)}</small></button>`).join('')}</div>`;
  const [f1, f2] = c.querySelectorAll('.fila');
  [...f1.children].forEach((b, i) => b.onclick = acciones[i][3]);
  [...f2.children].forEach((b, i) => b.onclick = numeros[i][3]);
}

/* ---------- FrmVenta: buscar, tocar, cobrar ---------- */
async function renderVenta(c) {
  state.cat = await api('/api/catalogos');
  const lineas = []; let sel = null;
  c.innerHTML = `<div class="vender"><div><input class="buscar" placeholder="Escribe o escanea el producto…"><div class="card tabla"></div></div>
    <div class="card cuenta"><div class="field"><span>Cliente</span><select name="cliente">${options(state.cat.clientes, '', 'Elige un cliente')}</select></div>
      <div class="lineas" style="flex:1;min-height:90px;overflow:auto"></div>
      <div class="row"><button class="mini" data-menos>−</button><button class="mini" data-mas>+</button><span class="spacer"></span><button class="peligro" data-vaciar>Vaciar</button></div>
      <div class="total">$0.00</div><button class="primary cobrar">COBRAR</button></div></div>`;
  const $ = (s) => c.querySelector(s), buscar = $('.buscar');
  const total = () => lineas.reduce((a, l) => a + l.precio_venta * l.cantidad, 0);
  const filtrados = () => { const p = buscar.value.trim().toLowerCase().split(/\s+/).filter(Boolean); return state.cat.productos.filter((x) => p.every((w) => (x.nombre + ' ' + x.sku).toLowerCase().includes(w))); };
  const pintar = () => {
    const g = grid([{ t: 'Producto', k: 'nombre' }, { t: 'Pzas.', k: 'cantidad', num: 1 }, { t: 'Importe', k: 'importe', num: 1, f: $$() }],
      lineas.map((l) => ({ ...l, importe: l.precio_venta * l.cantidad })), { onSelect: (r) => sel = r.id });
    $('.lineas').replaceChildren(g); sel = lineas.some((l) => l.id === sel) ? sel : lineas[0]?.id;
    g.querySelectorAll('tbody tr').forEach((tr, i) => tr.classList.toggle('sel', lineas[i].id === sel));
    $('.total').textContent = money(total());
    ['[data-menos]', '[data-mas]', '[data-vaciar]', '.cobrar'].forEach((s) => $(s).disabled = !lineas.length);
  };
  const agregar = (p) => {
    const l = lineas.find((x) => x.id === p.id);
    if ((l?.cantidad || 0) + 1 > p.stock_actual) return aviso(`Solo hay ${p.stock_actual} pieza(s) de ${p.nombre}`);
    if (l) l.cantidad++; else lineas.push({ ...p, cantidad: 1 });
    sel = p.id; pintar();
  };
  const productos = () => {
    const rows = filtrados();
    const g = grid([{ t: 'Producto', k: 'nombre' }, { t: 'Precio', k: 'precio_venta', num: 1, f: $$() }, { t: 'Quedan', k: 'stock_actual', num: 1 }], rows);
    g.querySelectorAll('tbody tr').forEach((tr) => tr.onclick = () => agregar(rows[+tr.dataset.i]));
    $('.tabla').replaceChildren(g);
  };
  buscar.oninput = productos;
  buscar.onkeydown = (e) => {
    if (e.key !== 'Enter' || !buscar.value.trim()) return;
    const p = state.cat.productos.find((x) => x.sku.toLowerCase() === buscar.value.trim().toLowerCase()) || filtrados()[0];
    if (p) { agregar(p); buscar.value = ''; productos(); }
  };
  $('[data-mas]').onclick = () => { const p = state.cat.productos.find((x) => x.id === sel); if (p) agregar(p); };
  $('[data-menos]').onclick = () => { const i = lineas.findIndex((x) => x.id === sel); if (i < 0) return; if (lineas[i].cantidad > 1) lineas[i].cantidad--; else lineas.splice(i, 1); pintar(); };
  $('[data-vaciar]').onclick = async () => { if (await confirmar('¿Vaciar la venta?', 'Vaciar')) { lineas.length = 0; pintar(); } };
  $('.cobrar').onclick = () => {
    const cli = $('[name=cliente]');
    cli.classList.toggle('invalid', !cli.value);
    if (!cli.value) return cli.focus();
    cobrar(total(), async (metodo, desc) => {
      const r = await api('/api/ventas', { id_cliente: cli.value, id_metodo_pago: metodo, descuento_pct: desc, lineas: lineas.map((l) => ({ id_producto: l.id, cantidad: l.cantidad })) });
      aviso(`Venta ${r.folio} registrada`);
      abrir('venta');
      return true;
    });
  };
  productos(); pintar(); buscar.focus();
}

// FrmCobrar: total grande, forma de pago y descuento; es también la confirmación
function cobrar(subtotal, guardar) {
  const cuerpo = h('<div><div class="total" style="text-align:center"></div><div class="ayuda" style="text-align:center;min-height:24px"></div></div>');
  const pago = seg(state.cat.metodos.map((m) => m.nombre), 0), otro = h('<input type="number" min="0" max="50" value="0" style="width:110px;display:none">');
  const desc = seg(['Sin descuento', '5 %', '10 %', 'Otro'], 0, (i) => { otro.style.display = i === 3 ? '' : 'none'; pintar(); });
  const pct = () => [0, 5, 10, +otro.value || 0][desc.activo];
  const fila = h('<div class="row"></div>'); fila.append(desc, otro); otro.oninput = () => pintar();
  cuerpo.append(h('<div class="field"><span style="margin:12px 0 6px">Forma de pago</span></div>'), pago, h('<div class="field"><span style="margin:18px 0 6px">Descuento</span></div>'), fila,
    h(`<div class="ayuda" style="margin-top:8px">${tiene('cancelar_venta') ? 'Puedes autorizar hasta 50 %.' : `Más de ${esc(state.cat.descuentoMax)} % lo autoriza un Supervisor.`}</div>`));
  const el = ficha('Cobrar', cuerpo, [['Regresar', ''], ['Cobrar', 'primary', () => guardar(state.cat.metodos[pago.activo].id, pct())]], { ancha: true });
  const boton = el.querySelector('.pie .primary'); boton.style.cssText = 'flex:2;min-height:64px;font-size:19px';
  function pintar() {
    const d = Math.round(subtotal * pct()) / 100;
    cuerpo.children[0].textContent = money(subtotal - d);
    cuerpo.children[1].textContent = d ? `Antes ${money(subtotal)} · descuento ${money(d)}` : '';
    boton.textContent = 'Cobrar ' + money(subtotal - d);
  }
  pintar(); boton.focus();
}

/* ---------- FrmHistorialVentas ---------- */
function renderVentas(c) {
  const estado = seg(['Todas', 'Completadas', 'Canceladas'], 0, () => L.cargar());
  const fechas = h(`<div class="barra" style="display:none"><span class="suave">Desde</span><input type="date" value="${haceMeses(6)}"><span class="suave">hasta</span><input type="date" value="${isoDia(new Date())}"></div>`);
  const [desde, hasta] = fechas.querySelectorAll('input');
  const L = lista(c, {
    pista: 'Folio o cliente…', filtros: [estado], textoAbrir: 'Ver venta', abrir: ver,
    botones: [['Fechas', () => { fechas.style.display = fechas.style.display ? '' : 'none'; }]],
    datos: (texto) => api('/api/ventas?' + new URLSearchParams({ texto, desde: desde.value, hasta: hasta.value, estado: ['', 'Completada', 'Cancelada'][estado.activo] })),
    cols: [{ t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Cliente', k: 'cliente' }, { t: 'Total', k: 'total', num: 1, f: $$() },
      { t: 'Estado', k: 'estado', f: (v) => `<span class="${v === 'Cancelada' ? 'rojo' : 'suave'}">${esc(v)}</span>` }],
  });
  L.barra.after(fechas); desde.onchange = hasta.onchange = L.cargar;
  async function ver(r) {
    const det = await api('/api/ventas/detalle?id=' + r.id_venta);
    const cuerpo = h(`<div><div class="ayuda" style="margin-bottom:12px">${esc(r.cliente)} · ${datetime(r.fecha)}<br>Atendió ${esc(r.vendedor)} · ${esc(r.metodo_pago)}${r.descuento > 0 ? ' · Descuento ' + money(r.descuento) : ''}</div></div>`);
    cuerpo.append(grid([{ t: 'Producto', k: 'nombre' }, { t: 'No. de serie', k: 'numero_serie' }, { t: 'Pzas.', k: 'cantidad', num: 1 }, { t: 'Importe', k: 'subtotal', num: 1, f: $$() }, { t: 'Garantía', k: 'garantia' }], det));
    ficha(`${r.folio}   ${money(r.total)}${r.estado === 'Cancelada' ? '   (cancelada)' : ''}`, cuerpo, [
      ...(tiene('cancelar_venta') && r.estado !== 'Cancelada' ? [['Cancelar venta', 'peligro', async () => {
        const motivo = await preguntar('Cancelar ' + r.folio, '¿Por qué se cancela?', 'Continuar');
        if (!motivo || !await confirmar(`¿Cancelar la venta ${r.folio} por ${money(r.total)}?\nLos productos regresan al almacén.`, 'Cancelar venta')) return false;
        await api('/api/ventas/cancelar', { id_venta: r.id_venta, motivo });
        aviso(`Venta ${r.folio} cancelada`); L.cargar(); return true;
      }]] : []), ['Cerrar', '']], { ancha: true });
  }
  return L.cargado;
}

/* ---------- FrmOrdenes ---------- */
async function renderOrdenes(c, parametro) {
  const puede = tiene('gestionar_ordenes');
  const ver = seg(['Abiertas', 'Todas'], 0, () => L.cargar());
  const L = lista(c, {
    filtros: [ver], abrir, botones: puede ? [['Recibir equipo', recibir, 'primary']] : [],
    datos: () => api('/api/ordenes?abiertas=' + (ver.activo === 0 ? '1' : '')),
    cols: [{ t: 'Folio', k: 'folio' }, { t: 'Equipo', k: 'equipo_descripcion' }, { t: 'Cliente', k: 'cliente' },
      { t: 'Estado', k: 'estado', f: (v) => `<span class="${v === 'Cancelada' ? 'rojo' : v === 'Lista' ? 'fuerte' : ''}">${esc(v)}</span>` }, { t: 'Días', k: 'dias_en_taller', num: 1 }],
  });
  function recibir() {
    const f = campos([{ k: 'id_cliente', t: 'Cliente *', tipo: 'select', opciones: state.cat.clientes, vacio: 'Elige un cliente', req: 1, ancho: 1 },
      { k: 'equipo', t: 'Equipo *', req: 1, max: 150 }, { k: 'serie', t: 'No. de serie', max: 50 }, { k: 'falla', t: '¿Qué le pasa? *', tipo: 'textarea', req: 1, ancho: 1, max: 500 },
      { k: 'id_tecnico', t: 'Técnico *', tipo: 'select', opciones: state.cat.tecnicos, req: 1, ancho: 1 }]);
    ficha('Recibir equipo', f, [['Cancelar', ''], ['Recibir equipo', 'primary', async () => {
      const d = f.leer(); if (!d) return false;
      const r = await api('/api/ordenes', d); aviso(`Orden ${r.folio} registrada`); L.cargar(); return true;
    }]]);
  }
  async function abrir(o) {
    const estados = state.cat.estados, id = estados.find((e) => e.nombre === o.estado).id, cerrada = id >= 5;
    const [det, hist, cat] = await Promise.all([api('/api/ordenes/detalle?id=' + o.id_orden), api('/api/ordenes/historial?id=' + o.id_orden), api('/api/catalogos')]);
    state.cat = cat;
    const zona = h(`<div><div class="ayuda">${esc(o.equipo_descripcion)} · ${esc(o.cliente)}<br>${esc(o.estado)} · ${money(o.total)}</div></div>`);
    const tabla = h('<div class="card" style="margin-top:14px;min-height:260px"></div>');
    const pintar = (i) => tabla.replaceChildren(i ? grid([{ t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Pasó a', k: 'a_estado' }], hist)
      : grid([{ t: 'Concepto', k: 'concepto' }, { t: 'Pzas.', k: 'cantidad', num: 1 }, { t: 'Importe', k: 'subtotal', num: 1, f: $$() }], det));
    const s = seg(['Trabajos', 'Historial'], 0, pintar); s.style.marginTop = '14px';
    zona.append(s, tabla); pintar(0);
    const f = campos([{ k: 'servicio', t: 'Servicio', tipo: 'select', opciones: cat.servicios }, { k: 'refaccion', t: 'Refacción', tipo: 'select', opciones: cat.refacciones }, { k: 'cantidad', t: 'Piezas', tipo: 'number', defecto: 1 }], {}, { una: true, lectura: cerrada });
    const linea = (texto, antes, datos) => { const b = h(`<button ${cerrada ? 'disabled' : ''}>${texto}</button>`); b.onclick = () => seguir(async () => api('/api/ordenes/linea', { id_orden: o.id_orden, ...datos() })); f.querySelector(`[data-campo=${antes}]`).after(b); };
    linea('Agregar servicio', 'servicio', () => ({ servicio: f.c('servicio').value }));
    linea('Agregar refacción', 'cantidad', () => ({ refaccion: f.c('refaccion').value, cantidad: f.c('cantidad').value }));
    const seguir = async (accion, terminar) => {
      try {
        await accion(); el.cerrar(); await L.cargar();
        if (!terminar) { const n = (await api('/api/ordenes?abiertas=')).find((x) => x.id_orden === o.id_orden); abrir(n); }
      } catch (e) { error(e); }
    };
    const cambiar = (e) => seguir(() => api('/api/ordenes/estado', { id_orden: o.id_orden, id_estado: e }).then(() => e >= 5 && aviso(e === 5 ? 'Equipo entregado' : 'Orden cancelada')), e >= 5);
    const el = ficha(o.folio, puede ? dosZonas(f, zona) : zona, [
      ...(puede && !cerrada ? [['Cancelar orden', 'peligro', async () => { if (await confirmar('¿Cancelar la orden? Las refacciones regresan al almacén.', 'Cancelar orden')) await cambiar(6); return false; }]] : []),
      ['Cerrar', ''],
      ...(puede && !cerrada ? [[id + 1 === 5 ? 'Entregar al cliente' : 'Pasar a ' + estados.find((e) => e.id === id + 1).nombre, 'primary', async () => { await cambiar(id + 1); return false; }]] : [])], { ancha: true });
  }
  await L.cargado;
  if (parametro === 'recibir' && puede) recibir();
}

/* ---------- FrmClientes ---------- */
function renderClientes(c) {
  const puede = tiene('guardar_cliente');
  const L = lista(c, {
    pista: 'Buscar cliente…', abrir: (r) => abrir(r), botones: puede ? [['Nuevo cliente', () => abrir(null), 'primary']] : [],
    datos: (texto) => api('/api/clientes?' + new URLSearchParams({ texto })), rowClass: (r) => (r.activo ? '' : 'apagado'),
    cols: [{ t: 'Nombre', k: 'nombre' }, { t: 'Apellidos', k: 'apellidos' }, { t: 'Teléfono', k: 'telefono' }, { t: 'Total comprado', k: 'total_comprado', num: 1, f: $$() }, { t: 'Nivel', k: 'nivel' }],
  });
  async function abrir(r) {
    const f = campos([{ k: 'nombre', t: 'Nombre *', req: 1, max: 60 }, { k: 'apellidos', t: 'Apellidos *', req: 1, max: 80 }, { k: 'correo', t: 'Correo *', req: 1, max: 100 },
      { k: 'telefono', t: 'Teléfono', max: 20 }, { k: 'rfc', t: 'RFC', max: 13 }, { k: 'activo', t: 'Cliente activo', tipo: 'check', defecto: true, oculto: !r }], r || {}, { una: true, lectura: !puede });
    const compras = r ? await api('/api/clientes/compras?id=' + r.id_cliente) : null;
    ficha(r ? `${r.nombre} ${r.apellidos}` : 'Nuevo cliente', r ? dosZonas(f, tarjeta('Sus compras', grid([{ t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Total', k: 'total', num: 1, f: $$() }, { t: 'Estado', k: 'estado' }], compras))) : f,
      [[puede ? 'Cancelar' : 'Cerrar', ''], ...(puede ? [['Guardar', 'primary', async () => {
        const d = f.leer(); if (!d) return false;
        await api('/api/clientes', { ...d, id: r?.id_cliente, activo: r ? d.activo : true });
        aviso(r ? 'Cambios guardados' : 'Cliente registrado'); L.cargar(); return true;
      }]] : [])], { ancha: !!r });
  }
  return L.cargado;
}

/* ---------- FrmProductos ---------- */
function renderProductos(c, parametro) {
  const puede = tiene('guardar_producto'), costos = tiene('ver_costos');
  const cat = h(`<select>${options(state.cat.categorias, '', 'Todas las categorías')}</select>`), al = seg(['Todos', 'Por resurtir'], parametro === 'alertas' ? 1 : 0, () => L.cargar());
  const L = lista(c, {
    pista: 'Buscar producto…', filtros: [cat, al], abrir: (r) => abrir(r), botones: puede ? [['Nuevo producto', () => abrir(null), 'primary']] : [],
    datos: (texto) => api('/api/productos?' + new URLSearchParams({ texto, categoria: cat.value, alertas: al.activo ? '1' : '' })),
    cols: [{ t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Precio', k: 'precio_venta', num: 1, f: $$() }, { t: 'Stock', k: 'stock_actual', num: 1 },
      { t: 'Estado', k: 'alerta_stock', f: (v) => `<span class="${v === 'Agotado' ? 'rojo' : v === 'OK' ? 'suave' : 'fuerte'}">${esc(v)}</span>` }],
  });
  cat.onchange = L.cargar;
  async function abrir(r) {
    const p = r ? await api('/api/producto?id=' + r.id_producto) : {};
    const f = campos([{ k: 'nombre', t: 'Nombre *', req: 1, ancho: 1, max: 120 }, { k: 'sku', t: 'SKU *', req: 1, max: 30 }, { k: 'id_categoria', t: 'Categoría', tipo: 'select', opciones: state.cat.categorias },
      { k: 'id_marca', t: 'Marca', tipo: 'select', opciones: state.cat.marcas }, { k: 'id_condicion', t: 'Condición', tipo: 'select', opciones: state.cat.condiciones },
      { k: 'precio_venta', t: 'Precio de venta *', tipo: 'number', req: 1 }, { k: 'costo', t: 'Costo', tipo: 'number', oculto: !costos }, { k: 'stock_minimo', t: 'Avisar cuando queden', tipo: 'number', defecto: 0 },
      { k: 'stock_actual', t: `En almacén: ${p.stock_actual} piezas`, tipo: 'texto', oculto: !r },
      { k: 'requiere_serie', t: 'Cada pieza lleva número de serie', tipo: 'check' }, { k: 'activo', t: 'Producto a la venta', tipo: 'check', defecto: true }], p, { lectura: !puede });
    ficha(r ? p.nombre : 'Nuevo producto', f, [
      ...(r && tiene('eliminar_producto') ? [['Eliminar', 'peligro', async () => {
        if (!await confirmar(`¿Eliminar el producto ${p.nombre}?\n\nSi ya tiene ventas, compras u órdenes, solo se desactivará para conservar el historial.`, 'Eliminar')) return false;
        await api('/api/productos/eliminar', { id: p.id_producto }); aviso('Producto eliminado'); L.cargar(); return true;
      }]] : []), [puede ? 'Cancelar' : 'Cerrar', ''], ...(puede ? [['Guardar', 'primary', async () => {
        const d = f.leer(); if (!d) return false;
        await api('/api/productos', { ...d, id: r?.id_producto }); aviso(r ? 'Cambios guardados' : 'Producto registrado'); L.cargar(); return true;
      }]] : [])]);
  }
  return L.cargado;
}

/* ---------- FrmProveedores ---------- */
function renderProveedores(c) {
  const puede = tiene('guardar_proveedor');
  const L = lista(c, {
    pista: 'Buscar proveedor…', abrir: (r) => abrir(r), botones: puede ? [['Nuevo proveedor', () => abrir(null), 'primary']] : [],
    datos: (texto) => api('/api/proveedores?' + new URLSearchParams({ texto })), rowClass: (r) => (r.activo ? '' : 'apagado'),
    cols: [{ t: 'Proveedor', k: 'razon_social' }, { t: 'Teléfono', k: 'telefono' }, { t: 'Correo', k: 'correo' }, { t: 'Productos', k: 'productos', num: 1 }],
  });
  async function abrir(r) {
    const f = campos([{ k: 'razon_social', t: 'Razón social *', req: 1, max: 120 }, { k: 'rfc', t: 'RFC *', req: 1, max: 13 }, { k: 'correo', t: 'Correo', max: 100 }, { k: 'telefono', t: 'Teléfono', max: 20 },
      { k: 'activo', t: 'Proveedor activo', tipo: 'check', defecto: true, oculto: !r }], r || {}, { una: true, lectura: !puede });
    const surte = r ? await api('/api/proveedores/productos?id=' + r.id_proveedor) : null;
    ficha(r ? r.razon_social : 'Nuevo proveedor', r ? dosZonas(f, tarjeta('Productos que surte', grid([{ t: 'Producto', k: 'nombre' }, { t: 'Costo', k: 'costo_proveedor', num: 1, f: $$() }, { t: 'Stock', k: 'stock_actual', num: 1 }], surte))) : f,
      [[puede ? 'Cancelar' : 'Cerrar', ''], ...(puede ? [['Guardar', 'primary', async () => {
        const d = f.leer(); if (!d) return false;
        await api('/api/proveedores', { ...d, id: r?.id_proveedor, activo: r ? d.activo : true }); aviso(r ? 'Cambios guardados' : 'Proveedor registrado'); L.cargar(); return true;
      }]] : [])], { ancha: !!r });
  }
  return L.cargado;
}

/* ---------- FrmCompra ---------- */
async function renderCompras(c) {
  const d = await api('/api/compras'), lineas = []; let sel = null;
  c.innerHTML = '<div class="dos" style="flex:1;grid-template-columns:400px 1fr"></div>';
  const f = campos([{ k: 'proveedor', t: 'Proveedor *', tipo: 'select', opciones: d.proveedores, vacio: 'Elige un proveedor', req: 1 }, { k: 'producto', t: 'Producto', tipo: 'select', opciones: d.productos },
    { k: 'cant', t: 'Piezas', tipo: 'number', defecto: 1 }, { k: 'costo', t: 'Costo de cada pieza', tipo: 'number' }, { k: 'series', t: 'Números de serie (uno por renglón)', tipo: 'textarea', max: 2000 }], {}, { una: true });
  const b = h('<button>Agregar a la compra</button>'); f.append(b);
  const der = h(`<div class="card" style="display:flex;flex-direction:column"><h3>Esta compra</h3><div class="lineas" style="flex:1"></div><div class="total">$0.00</div>
    <div class="row"><button data-ant>Compras anteriores</button><span class="spacer"></span><button data-quitar>Quitar</button><button class="primary" data-guardar>Guardar compra</button></div></div>`);
  c.firstElementChild.append(tarjeta('Qué llegó', f), der);
  const prod = () => d.productos.find((p) => p.id === +f.c('producto').value), total = () => lineas.reduce((a, l) => a + l.cantidad * l.costo_unitario, 0);
  const cambio = () => { const p = prod(); f.c('costo').value = p.costo; f.c('series').value = ''; f.querySelector('[data-campo=series]').style.display = p.requiere_serie ? '' : 'none'; };
  const pintar = () => {
    const g = grid([{ t: 'Producto', k: 'nombre' }, { t: 'Pzas.', k: 'cantidad', num: 1 }, { t: 'Importe', k: 'importe', num: 1, f: $$() }], lineas.map((l) => ({ ...l, importe: l.cantidad * l.costo_unitario })), { onSelect: (r) => sel = r.id_producto });
    sel = lineas[0]?.id_producto; der.querySelector('.lineas').replaceChildren(g); der.querySelector('.total').textContent = money(total());
    der.querySelector('[data-quitar]').disabled = der.querySelector('[data-guardar]').disabled = !lineas.length;
  };
  f.c('producto').onchange = cambio;
  b.onclick = () => {
    const p = prod(), cantidad = parseInt(f.c('cant').value, 10), costo = +f.c('costo').value;
    if (!(cantidad > 0) || !(costo > 0)) return informar('Cantidad y costo deben ser mayores a cero.');
    if (lineas.some((l) => l.id_producto === p.id)) return informar('Ese producto ya está en la compra; quítalo y vuelve a agregarlo con la cantidad total.');
    const series = f.c('series').value.split('\n').map((x) => x.trim().toUpperCase()).filter(Boolean);
    if (p.requiere_serie && series.length !== cantidad) return informar(`Captura ${cantidad} número(s) de serie, uno por renglón (llevas ${series.length}).`);
    if (new Set(series).size !== series.length) return informar('Hay números de serie repetidos.');
    lineas.push({ id_producto: p.id, nombre: p.nombre, cantidad, costo_unitario: costo, ...(p.requiere_serie ? { series } : {}) });
    f.c('cant').value = 1; f.c('series').value = ''; pintar();
  };
  der.querySelector('[data-quitar]').onclick = () => { lineas.splice(lineas.findIndex((l) => l.id_producto === sel), 1); pintar(); };
  der.querySelector('[data-ant]').onclick = () => ficha('Compras anteriores', grid([{ t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Proveedor', k: 'proveedor' }, { t: 'Total', k: 'total', num: 1, f: $$() }], d.recientes), [['Cerrar', '']], { ancha: true });
  der.querySelector('[data-guardar]').onclick = async () => {
    const pr = f.c('proveedor'); pr.classList.toggle('invalid', !pr.value); if (!pr.value) return pr.focus();
    if (!await confirmar(`¿Guardar la compra por ${money(total())}?\nEl inventario sube al guardar.`, 'Guardar compra')) return;
    try { const r = await api('/api/compras', { id_proveedor: pr.value, lineas: lineas.map(({ nombre, ...l }) => l) }); aviso(`Compra ${r.folio} guardada`); abrir('compras'); } catch (e) { error(e); }
  };
  cambio(); pintar();
}

/* ---------- FrmAjusteStock ---------- */
async function renderAjustes(c) {
  const d = await api('/api/ajustes');
  c.innerHTML = '<div class="mitades"></div>';
  if (tiene('ajustar_stock')) {
    const f = campos([{ k: 'p', t: 'Producto', tipo: 'select', opciones: d.productos }, { k: 'cant', t: 'Piezas (+ entran, − salen)', tipo: 'number', defecto: 0, min: -20, maxn: 20 }, { k: 'motivo', t: 'Motivo *', req: 1 }], {}, { una: true });
    const b = h('<button class="primary">Ajustar inventario</button>'); f.append(b);
    b.onclick = async () => {
      const x = f.leer(), n = parseInt(f.c('cant').value, 10); if (!n) return informar('La cantidad no puede ser cero.'); if (!x) return;
      if (!await confirmar(`¿Ajustar ${n > 0 ? '+' : ''}${n} pieza(s) de\n${f.c('p').selectedOptions[0].textContent}?`)) return;
      try { await api('/api/ajustes/stock', { id_producto: x.p, cantidad: n, motivo: x.motivo }); aviso('Inventario ajustado'); abrir('ajustes'); } catch (e) { error(e); }
    };
    c.firstElementChild.append(tarjeta('Corregir el inventario', f));
  }
  if (tiene('ajustar_precio')) {
    const f = campos([{ k: 'cat', t: 'Categoría', tipo: 'select', opciones: d.categorias }, { k: 'pct', t: 'Porcentaje (+ sube, − baja)', tipo: 'number', defecto: 0, min: -30, maxn: 30 }], {}, { una: true });
    const b = h('<button>Cambiar precios</button>'); f.append(b);
    b.onclick = async () => {
      const n = +f.c('pct').value; if (!n) return informar('El porcentaje no puede ser cero.');
      if (!await confirmar(`¿Cambiar ${n > 0 ? '+' : ''}${n} % el precio de todos los productos activos de ${f.c('cat').selectedOptions[0].textContent}?`)) return;
      try { await api('/api/ajustes/precio', { porcentaje: n, id_categoria: f.c('cat').value }); aviso('Precios actualizados'); f.c('pct').value = 0; } catch (e) { error(e); }
    };
    c.firstElementChild.append(tarjeta('Cambiar precios', f));
  }
}

/* ---------- FrmDashboard (Resumen) ---------- */
const miles = (v) => (v ? '$' + Math.round(v / 1000) + 'K' : '0');
function columnas(datos) {
  const W = 560, H = 230, iz = 48, ab = 26, ar = 22, max = Math.max(1, ...datos.map((d) => d.v)) * 1.12, paso = (W - iz - 10) / datos.length, ancho = paso * 0.55;
  const y = (v) => ar + (H - ar - ab) * (1 - v / max);
  return `<svg viewBox="0 0 ${W} ${H}" class="grafica">${[0, 0.5, 1].map((f) => `<line x1="${iz}" x2="${W - 6}" y1="${y(max * f)}" y2="${y(max * f)}" class="rejilla"/><text x="${iz - 8}" y="${y(max * f) + 4}" text-anchor="end">${miles(max * f)}</text>`).join('')}
    ${datos.map((d, i) => { const x = iz + paso * i + (paso - ancho) / 2, yy = y(d.v); return `<rect x="${x}" y="${yy}" width="${ancho}" height="${H - ab - yy}" rx="6" class="${i === datos.length - 1 ? 'actual' : ''}"><title>${esc(d.t)}: ${money(d.v, 0)}</title></rect><text x="${x + ancho / 2}" y="${yy - 6}" text-anchor="middle">${d.v ? miles(d.v) : ''}</text><text x="${x + ancho / 2}" y="${H - 8}" text-anchor="middle">${esc(d.t)}</text>`; }).join('')}</svg>`;
}
async function renderDashboard(c) {
  const d = await api('/api/dashboard'), k = d.kpi;
  const vs = (a, b) => (!b ? (a ? 'sin datos del periodo anterior' : 'sin movimientos') : `${a >= b ? '▲' : '▼'} ${Math.abs((a - b) * 100 / b).toFixed(1)} % vs. 90 días previos`);
  const meses = [];
  for (let i = 5; i >= 0; i--) {
    const f = new Date(); f.setDate(1); f.setMonth(f.getMonth() - i);
    const t = f.toLocaleDateString('es-MX', { month: 'short' }).replace('.', '');
    meses.push({ t: t[0].toUpperCase() + t.slice(1), v: d.meses.find((m) => m.mes === f.toISOString().slice(0, 7))?.ingresos || 0 });
  }
  const max = Math.max(1, ...d.categorias.map((x) => x.ingresos));
  c.innerHTML = `<div class="kpis">${[['Ingresos · 90 días', money(k.ingresos_mes, 0), vs(k.ingresos_mes, k.ingresos_ant)], ['Ventas · 90 días', k.ventas_mes, vs(k.ventas_mes, k.ventas_ant)],
      ['Equipos en taller', k.ordenes_abiertas, `${k.ordenes_listas} listos para entregar`], ['Productos por resurtir', k.alertas_stock, k.agotados ? `${k.agotados} agotado(s)` : 'ninguno agotado']]
      .map(([t, v, n]) => `<div class="card kpi"><span>${esc(t)}</span><b>${esc(v)}</b><small>${esc(n)}</small></div>`).join('')}</div>
    <div class="mitades" style="align-items:stretch"><div class="card"><h3>Ingresos por mes</h3>${columnas(meses)}</div>
      <div class="card"><h3>Ventas por categoría · 90 días</h3><div class="barrasH">${d.categorias.map((x) => `<div class="bh"><span>${esc(x.categoria)}</span><div><i style="width:${x.ingresos / max * 100}%"></i></div><b>${miles(x.ingresos)}</b></div>`).join('')}</div></div></div>
    <div class="mitades" data-tablas></div>`;
  c.querySelector('[data-tablas]').append(
    tarjeta('Por resurtir', grid([{ t: 'Producto', k: 'producto' }, { t: 'Stock', k: 'stock', num: 1, f: (v) => `<span class="${v ? '' : 'rojo'}">${v}</span>` }, { t: 'Mínimo', k: 'minimo', num: 1 }], d.alertas, { cls: 'compacta' })),
    tarjeta('Equipos en el taller', grid([{ t: 'Equipo', k: 'equipo' }, { t: 'Estado', k: 'estado' }, { t: 'Días', k: 'dias', num: 1 }], d.taller, { cls: 'compacta' })));
}

/* ---------- FrmReportes ---------- */
async function renderReportes(c) {
  let TABS = [['resumen', 'Por mes', [{ t: 'Mes', k: 'mes' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: $$() }, { t: 'Ticket promedio', k: 'ticket_promedio', num: 1, f: $$() }]],
    ['top', 'Más vendidos', [{ t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Unidades', k: 'unidades_vendidas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: $$() }, { t: 'Utilidad', k: 'utilidad_bruta', num: 1, f: $$() }]],
    ['categorias', 'Por categoría', [{ t: 'Categoría', k: 'categoria' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Unidades', k: 'unidades', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: $$() }]],
    ['empleados', 'Por empleado', [{ t: 'Empleado', k: 'empleado' }, { t: 'Puesto', k: 'puesto' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Vendido', k: 'vendido', num: 1, f: $$() }, { t: 'Órdenes', k: 'ordenes', num: 1 }]],
    ['stock', 'Por resurtir', [{ t: 'Producto', k: 'nombre' }, { t: 'Stock', k: 'stock_actual', num: 1 }, { t: 'Mínimo', k: 'stock_minimo', num: 1 }, { t: 'Faltan', k: 'faltante', num: 1 }, { t: 'Proveedor sugerido', k: 'proveedor_sugerido' }]]];
  if (!tiene('ver_costos')) TABS = TABS.filter((t) => t[0] !== 'empleados');
  c.innerHTML = `<div class="barra" data-a></div><div class="barra" data-b></div><div class="barra" data-f style="display:none"><span class="suave">Desde</span><input type="date"><span class="suave">hasta</span><input type="date"></div><div class="card tabla"></div>`;
  const [desde, hasta] = c.querySelectorAll('[data-f] input'); let data;
  const s = seg(TABS.map((t) => t[1]), 0, () => pintar()), kpi = h('<b style="font-size:18px;font-weight:600"></b>');
  const pintar = () => { const [k, , cols] = TABS[s.activo]; c.querySelector('.tabla').replaceChildren(grid(cols, data[k])); };
  const cargar = async () => {
    try { data = await api('/api/reportes?' + new URLSearchParams({ desde: desde.value, hasta: hasta.value })); kpi.textContent = `${data.resumen.reduce((a, r) => a + r.ventas, 0)} ventas · ${money(data.resumen.reduce((a, r) => a + r.ingresos, 0), 0)}`; pintar(); } catch (e) { error(e); }
  };
  const periodo = (i) => {
    c.querySelector('[data-f]').style.display = i === 3 ? '' : 'none'; if (i === 3) return;
    const hoy = new Date(); desde.value = i === 0 ? isoDia(new Date(hoy.getFullYear(), hoy.getMonth(), 1)) : haceMeses(i === 1 ? 3 : 6); hasta.value = isoDia(hoy); return cargar();
  };
  const exp = h('<button>Exportar a Excel</button>');
  exp.onclick = () => { const [k, n, cols] = TABS[s.activo]; const csv = [cols.map((x) => x.t).join(','), ...data[k].map((r) => cols.map((x) => `"${String(r[x.k] ?? '').replace(/"/g, '""')}"`).join(','))].join('\n'); const a = document.createElement('a'); a.href = URL.createObjectURL(new Blob(['﻿' + csv], { type: 'text/csv' })); a.download = n + '.csv'; a.click(); };
  c.querySelector('[data-a]').append(s, h('<span class="spacer"></span>'), exp);
  c.querySelector('[data-b]').append(seg(['Este mes', '3 meses', '6 meses', 'Otro'], 2, periodo), kpi);
  desde.onchange = hasta.onchange = cargar;
  await periodo(2);
}

/* ---------- FrmAuditoria ---------- */
async function renderAuditoria(c) {
  const rows = await api('/api/auditoria');
  c.innerHTML = '<div class="barra"></div><div class="card tabla"></div>';
  const p = seg(['Hoy', '7 días', '30 días', 'Todo'], 2, () => pintar()), t = seg(['Todo', 'Productos', 'Ventas'], 0, () => pintar());
  const pintar = () => {
    const lim = new Date(); lim.setHours(0, 0, 0, 0); lim.setDate(lim.getDate() - [0, 7, 30, 3650][p.activo]);
    c.querySelector('.tabla').replaceChildren(grid([{ t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Usuario', k: 'usuario_app' }, { t: 'Qué cambió', k: 'tabla' }, { t: 'Antes', k: 'valor_anterior' }, { t: 'Después', k: 'valor_nuevo' }, { t: 'Motivo', k: 'motivo' }],
      rows.filter((r) => new Date(r.fecha) >= lim && (!t.activo || r.tabla === ['', 'Producto', 'Venta'][t.activo]))));
  };
  c.firstElementChild.append(p, t); pintar();
}

/* ---------- FrmUsuarios ---------- */
async function renderUsuarios(c) {
  let d = await api('/api/usuarios');
  const L = lista(c, {
    abrir: (r) => abrir(r), botones: [['Nuevo usuario', () => abrir(null), 'primary']], datos: async () => { d = await api('/api/usuarios'); return d.usuarios; }, rowClass: (r) => (r.activo ? '' : 'apagado'),
    cols: [{ t: 'Empleado', k: 'empleado' }, { t: 'Usuario', k: 'usuario' }, { t: 'Rol', k: 'rol' }, { t: 'Último acceso', k: 'ultimo_acceso', f: datetime }],
  });
  function abrir(r) {
    const f = campos([{ k: 'id_empleado', t: 'Empleado *', tipo: 'select', opciones: d.empleados, req: 1, ancho: 1 }, { k: 'usuario', t: 'Usuario *', req: 1, max: 40 }, { k: 'id_rol', t: 'Rol *', tipo: 'select', opciones: d.roles, req: 1 },
      { k: 'contrasena', t: 'Contraseña', tipo: 'password', req: !r, max: 100 }, { k: 'repetir', t: 'Repetir contraseña', tipo: 'password', max: 100 },
      { k: 'x', t: r ? 'Déjala vacía para conservar la actual.' : 'Mínimo 8 caracteres y un número.', tipo: 'texto', ancho: 1 }, { k: 'activo', t: 'Puede entrar al sistema', tipo: 'check', defecto: true, ancho: 1 }], r ? { ...r, contrasena: '' } : {});
    ficha(r ? r.empleado : 'Nuevo usuario', f, [['Cancelar', ''], ['Guardar', 'primary', async () => {
      const x = f.leer(); if (!x) return false;
      if (x.contrasena !== x.repetir) { f.c('repetir').classList.add('invalid'); return false; }
      await api('/api/usuarios', { ...x, id: r?.id_usuario }); aviso(r ? 'Cambios guardados' : 'Usuario creado'); L.cargar(); return true;
    }]]);
  }
  return L.cargado;
}

/* ---------- FrmCatalogo ---------- */
function renderCatalogo(c, tipo) {
  const cfg = { categorias: ['Descripción', 'Nueva categoría'], marcas: ['País de origen', 'Nueva marca'], tipos: ['Precio base', 'Nuevo servicio'] }[tipo], precio = tipo === 'tipos';
  const L = lista(c, {
    pista: 'Buscar…', abrir: (r) => abrir(r), botones: [[cfg[1], () => abrir(null), 'primary']], datos: (texto) => api('/api/catalogo?' + new URLSearchParams({ tipo, texto })),
    cols: [{ t: 'Nombre', k: 'nombre' }, { t: cfg[0], k: 'campo2', num: precio, f: precio ? $$() : undefined }],
  });
  function abrir(r) {
    const f = campos([{ k: 'nombre', t: 'Nombre *', req: 1, max: 80 }, { k: 'campo2', t: cfg[0], tipo: precio ? 'number' : 'text' }], r || {}, { una: true });
    ficha(r ? r.nombre : cfg[1], f, [...(r ? [['Eliminar', 'peligro', async () => {
      if (!await confirmar(`¿Eliminar "${r.nombre}"?\nSi está en uso no se podrá eliminar.`, 'Eliminar')) return false;
      await api('/api/catalogo/eliminar', { tipo, id: r.id }); aviso('Registro eliminado'); L.cargar(); return true;
    }]] : []), ['Cancelar', ''], ['Guardar', 'primary', async () => {
      const d = f.leer(); if (!d) return false;
      await api('/api/catalogo', { tipo, id: r?.id, ...d }); aviso(r ? 'Cambios guardados' : 'Registro agregado'); L.cargar(); return true;
    }]]);
  }
  return L.cargado;
}

// Captura automática para la documentación: ?usuario=admin&modulo=productos
// entra con un usuario demo y abre esa pantalla (solo usuarios demo con su contraseña conocida).
async function inicio() {
  const p = new URLSearchParams(location.search);
  if (p.has('usuario') || p.has('captura')) document.documentElement.classList.add('sin-animacion');
  if (!['admin', 'csupervisor', 'loperador', 'fconsulta'].includes(p.get('usuario'))) return renderLogin();
  try {
    const r = await api('/api/login', { usuario: p.get('usuario'), contrasena: '1234' });
    state.token = r.token; state.user = r.user;
    state.cat = await api('/api/catalogos');
    state.vista = p.get('modulo') || 'inicio';
    renderMain();
  } catch (e) { renderLogin(); }
}
inicio();
