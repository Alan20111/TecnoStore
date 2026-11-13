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
      <div class="buttons">${buttons.map((b, i) => `<button data-i="${i}" ${i === 0 ? 'class="primary"' : ''}>${esc(b)}</button>`).join('')}</div>
    </div></div>`);
    const close = (v) => { el.remove(); resolve(v); };
    el.querySelectorAll('button').forEach((b) => b.onclick = () => close(buttons[+b.dataset.i]));
    el.querySelector('.close').onclick = () => close(null);
    document.body.appendChild(el);
    el.querySelector('button').focus();
  });
}
const showError = (e) => messageBox(e.message || e, { icon: 'error', title: 'Error' });

// DataGridView
function grid(cols, rows, { onSelect, rowClass, cls = '' } = {}) {
  const el = h(`<div class="grid ${cls}"></div>`);
  if (!rows.length) { el.innerHTML = '<div class="empty">Sin registros</div>'; return el; }
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
      <div class="field"><span>Usuario:</span><input name="usuario" required></div>
      <div class="field"><span>Contraseña:</span><input name="contrasena" type="password" required></div>
      <div class="error-label"></div>
      <div class="row"><span class="spacer"></span><button type="submit" class="primary">Ingresar</button><button type="button" data-salir-app>Salir</button></div>
      <div class="hint">Usuarios demo (contraseña <code data-p>Tecno123!</code>):<br>
        <code data-u>admin</code> Administrador · <code data-u>csupervisor</code> Supervisor<br>
        <code data-u>loperador</code> Operador · <code data-u>fconsulta</code> Consulta</div>
    </form></div>`);
  const form = el.querySelector('form');
  el.querySelectorAll('[data-u]').forEach((c) => c.onclick = () => { form.usuario.value = c.textContent; form.contrasena.value = 'Tecno123!'; });
  el.querySelector('[data-p]').onclick = () => { form.contrasena.value = 'Tecno123!'; };
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

/* ---------- FrmPrincipal ---------- */
const MODULOS = {
  inicio: ['Inicio', renderInicio],
  productos: ['Productos', renderProductos],
  clientes: ['Clientes', renderClientes],
  proveedores: ['Proveedores', renderProveedores],
  categorias: ['Categorías', (c) => renderCatalogo(c, 'categorias')],
  marcas: ['Marcas', (c) => renderCatalogo(c, 'marcas')],
  tipos: ['Tipos de servicio', (c) => renderCatalogo(c, 'tipos')],
  venta: ['Nueva venta', renderVenta],
  ordenes: ['Órdenes de servicio', renderOrdenes],
  compras: ['Compras a proveedor', renderCompras],
  ventas: ['Historial de ventas', renderVentas],
  reportes: ['Reportes', renderReportes],
  usuarios: ['Usuarios y roles', renderUsuarios],
  ajustes: ['Ajustes de inventario', renderAjustes],
  auditoria: ['Auditoría', renderAuditoria],
};

function renderMain() {
  const u = state.user;
  $app.innerHTML = '';
  const el = h(`<div class="form">
    ${titlebar(`TecnoStore — Sistema Integral de Gestión  [${u.rol}]`)}
    <div class="menustrip">${menuHtml(u)}</div>
    <div class="body">
      <div class="nav">
        <div class="brand"><b>TecnoStore</b><small>${esc(u.sucursal)}</small></div>
        ${u.modulos.map((m) => `<button data-m="${m}">${MODULOS[m][0]}</button>`).join('')}
        <div class="sep"></div>
        <button data-salir>Cerrar sesión</button>
      </div>
      <div class="content"></div>
    </div>
    <div class="statusstrip">
      <span>Usuario: <b>${esc(u.nombre_usuario)}</b> (${esc(u.nombre)})</span>
      <span>Rol: ${esc(u.rol)}</span>
      <span class="grow"></span>
      <span><i class="dot"></i>SQL Server · localhost · TecnoStore</span>
    </div></div>`);
  const cerrarSesion = async () => {
    if (await messageBox('¿Deseas cerrar la sesión?', { icon: 'question', buttons: ['Sí', 'No'] }) === 'Sí') { state.token = null; renderLogin(); }
  };
  el.querySelectorAll('[data-m]').forEach((b) => b.onclick = () => { cerrarMenus(); abrir(b.dataset.m); });
  el.querySelector('[data-salir]').onclick = cerrarSesion;
  ventana(el, cerrarSesion);

  // Menús desplegables: clic abre; con uno abierto, pasar el mouse cambia de menú
  const menus = el.querySelectorAll('.menustrip .menu');
  menus.forEach((m) => {
    m.querySelector('.top').onclick = (ev) => { ev.stopPropagation(); const abierto = m.classList.contains('open'); cerrarMenus(); if (!abierto) m.classList.add('open'); };
    m.onmouseenter = () => { if (el.querySelector('.menu.open') && !m.classList.contains('open')) { cerrarMenus(); m.classList.add('open'); } };
  });
  el.querySelectorAll('[data-accion]').forEach((b) => b.onclick = () => {
    cerrarMenus();
    ({ cerrar: cerrarSesion, salir: salirApp, acerca: acercaDe, atajos: atajos })[b.dataset.accion]();
  });
  document.onclick = cerrarMenus;
  document.onkeydown = (ev) => {
    if (ev.key === 'Escape') cerrarMenus();
    const tecla = { F2: 'productos', F3: 'clientes', F4: 'proveedores', F5: 'venta', F6: 'ordenes', F7: 'compras' }[ev.key];
    if (tecla && state.user?.modulos.includes(tecla) && !document.querySelector('.overlay')) { ev.preventDefault(); abrir(tecla); }
  };
  $app.appendChild(el);
  abrir(state.modulo);
}

function menuHtml(u) {
  const tiene = (m) => u.modulos.includes(m);
  const item = (m, atajo) => (tiene(m) ? `<button data-m="${m}">${MODULOS[m][0]}${atajo ? `<kbd>${atajo}</kbd>` : ''}</button>` : '');
  const grupos = [
    ['Archivo', `<button data-accion="cerrar">Cerrar sesión</button><button data-accion="salir">Salir</button>`],
    ['Catálogos', item('productos', 'F2') + item('clientes', 'F3') + item('proveedores', 'F4') + '<hr>' + item('categorias') + item('marcas') + item('tipos')],
    ['Procesos', item('venta', 'F5') + item('ordenes', 'F6') + item('compras', 'F7')],
    ['Consultas', item('ventas') + item('reportes')],
    ['Administración', item('usuarios') + item('ajustes') + item('auditoria')],
    ['Ayuda', `<button data-accion="atajos">Atajos de teclado</button><button data-accion="acerca">Acerca de TecnoStore</button>`],
  ];
  return grupos.filter(([, html]) => html)
    .map(([t, html]) => `<div class="menu"><span class="top">${t}</span><div class="dropdown">${html}</div></div>`).join('');
}

function cerrarMenus() { document.querySelectorAll('.menu.open').forEach((m) => m.classList.remove('open')); }

const acercaDe = () => messageBox('TecnoStore — Sistema Integral de Gestión\nProyecto final de Taller de Bases de Datos\n\nVista previa conectada a SQL Server. La aplicación real es la solución de Windows Forms en 03_WindowsForms.', { title: 'Acerca de' });
const atajos = () => messageBox('F2  Productos\nF3  Clientes\nF4  Proveedores\nF5  Nueva venta\nF6  Órdenes de servicio\nF7  Compras\nEnter  Buscar en los filtros\nEsc  Cerrar menús', { title: 'Atajos de teclado' });

async function abrir(m) {
  state.modulo = m;
  document.querySelectorAll('.nav [data-m]').forEach((b) => b.classList.toggle('active', b.dataset.m === m));
  const content = document.querySelector('.content');
  content.innerHTML = '<div class="sub">Cargando…</div>';
  try { await MODULOS[m][1](content); } catch (e) { content.innerHTML = ''; showError(e); }
}

/* ---------- Inicio ---------- */
async function renderInicio(c) {
  const d = await api('/api/inicio');
  c.innerHTML = `<h1>Bienvenido, ${esc(state.user.nombre)}</h1><div class="sub">${esc(state.user.puesto)} · ${esc(state.user.sucursal)}</div>
    <div class="kpis">
      <fieldset class="groupbox"><legend>Ventas (30 días)</legend><b>${d.kpi.ventas30}</b><small>operaciones completadas</small></fieldset>
      <fieldset class="groupbox"><legend>Ingresos (30 días)</legend><b>${money(d.kpi.ingresos30)}</b><small>total cobrado</small></fieldset>
      <fieldset class="groupbox"><legend>Alertas de stock</legend><b style="color:#c50f1f">${d.kpi.alertas}</b><small>productos en o bajo el mínimo</small></fieldset>
      <fieldset class="groupbox"><legend>Taller</legend><b>${d.kpi.ordenes_abiertas}</b><small>órdenes abiertas</small></fieldset>
    </div>
    <div class="two"><fieldset class="groupbox" id="g1"><legend>Productos por resurtir (vw_StockBajo)</legend></fieldset>
    <fieldset class="groupbox" id="g2"><legend>Órdenes abiertas (vw_OrdenServicioCompleta)</legend></fieldset></div>`;
  c.querySelector('#g1').appendChild(grid([
    { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Stock', k: 'stock_actual', num: 1 }, { t: 'Mínimo', k: 'stock_minimo', num: 1 },
  ], d.stock, { cls: 'mid', rowClass: (r) => (r.stock_actual === 0 ? 'danger' : 'warn') }));
  c.querySelector('#g2').appendChild(grid([
    { t: 'Folio', k: 'folio' }, { t: 'Cliente', k: 'cliente' }, { t: 'Estado', k: 'estado' }, { t: 'Días', k: 'dias_en_taller', num: 1 },
  ], d.ordenes, { cls: 'mid' }));
}

/* ---------- FrmProductos ---------- */
async function renderProductos(c) {
  c.innerHTML = `<h1>Productos</h1><div class="sub">Búsqueda con <b>usp_ProductoBuscar</b> (parámetros opcionales) · margen con <b>fn_MargenProducto</b></div>
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
      { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Marca', k: 'marca' },
      { t: 'Condición', k: 'condicion' }, { t: 'Precio', k: 'precio_venta', num: 1, f: M() },
      { t: 'Margen %', k: 'margen_pct', num: 1 }, { t: 'Stock', k: 'stock_actual', num: 1 }, { t: 'Mín.', k: 'stock_minimo', num: 1 },
      { t: 'Alerta', k: 'alerta_stock' },
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
  c.innerHTML = `<h1>Clientes</h1><div class="sub">Total y nivel calculados en SQL Server con <b>fn_TotalCompradoCliente</b> y <b>fn_ClasificacionCliente</b></div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Nombre, apellidos o correo:</span><input name="texto" style="width:260px"></div>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button>
    </div></fieldset><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="sub count"></div>`;
  const cargar = async () => {
    const rows = await api('/api/clientes?' + new URLSearchParams({ texto: c.querySelector('[name=texto]').value }));
    c.querySelector('.slot').replaceChildren(grid([
      { t: '#', k: 'id_cliente', num: 1 }, { t: 'Cliente', k: 'cliente' }, { t: 'Correo', k: 'correo' }, { t: 'Teléfono', k: 'telefono' },
      { t: 'Registro', k: 'registro', f: date }, { t: 'Compras', k: 'compras', num: 1 }, { t: 'Total comprado', k: 'total_comprado', num: 1, f: M() },
      { t: 'Nivel', k: 'nivel', f: (v) => (v === 'Sin compras' ? '<span style="color:#888">Sin compras</span>' : `<span class="badge ${esc(v)}">${esc(v)}</span>`) },
      { t: 'Activo', k: 'activo', f: (v) => (v ? 'Sí' : 'No') },
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
  c.innerHTML = `<h1>Nueva venta</h1><div class="sub">Se guarda con <b>usp_RegistrarVenta</b>: una sola transacción (encabezado, detalle, stock, piezas con serie y garantías). Cualquier error hace ROLLBACK.</div>
    <fieldset class="groupbox"><legend>Encabezado</legend><div class="row">
      <div class="field"><span>Cliente: *</span><select name="cliente" style="width:240px">${options(state.cat.clientes, '', '(Selecciona)')}</select></div>
      <div class="field"><span>Método de pago: *</span><select name="metodo">${options(state.cat.metodos, 1)}</select></div>
      <div class="field"><span>Descuento %:</span><input name="desc" type="number" min="0" max="50" step="1" value="0" style="width:70px"></div>
      <div class="field"><span>Vendedor:</span><input value="${esc(state.user.nombre)}" disabled style="width:180px"></div>
    </div><div class="sub" style="margin-top:6px">Descuentos mayores a ${esc(state.cat.descuentoMax)} % requieren rol Supervisor (regla validada en el SP).</div></fieldset>
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
      { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Cantidad', k: 'cantidad', num: 1 },
      { t: 'Precio', k: 'precio_venta', num: 1, f: M() }, { t: 'Importe', k: 'importe', num: 1, f: M() },
      { t: 'Serie', k: 'requiere_serie', f: (v) => (v ? 'Se asigna al guardar' : '—') },
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
      showError(new Error(`SQL Server rechazó la venta y no se guardó nada (ROLLBACK):\n\n${e.message}`));
    }
  };
  pintar();
}

/* ---------- FrmHistorialVentas ---------- */
async function renderVentas(c) {
  const puedeCancelar = ['Administrador', 'Supervisor'].includes(state.user.rol);
  c.innerHTML = `<h1>Historial de ventas</h1><div class="sub">Consulta sobre <b>vw_VentaCompleta</b> · cancelación con <b>usp_CancelarVenta</b> (reintegra stock y libera piezas)</div>
    <fieldset class="groupbox"><legend>Filtros</legend><div class="row">
      <div class="field"><span>Desde:</span><input type="date" name="desde" value="2026-05-01"></div>
      <div class="field"><span>Hasta:</span><input type="date" name="hasta" value="${new Date().toISOString().slice(0, 10)}"></div>
      <div class="field"><span>Estado:</span><select name="estado"><option value="">(Todos)</option><option>Completada</option><option>Cancelada</option></select></div>
      <div class="field"><span>Folio o cliente:</span><input name="texto"></div>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button>
      <span class="spacer"></span>
      <button data-cancelar style="margin-top:14px" ${puedeCancelar ? '' : 'disabled title="Requiere rol Supervisor"'}>Cancelar venta…</button>
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
      { t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Cliente', k: 'cliente' }, { t: 'Vendedor', k: 'vendedor' },
      { t: 'Sucursal', k: 'sucursal' }, { t: 'Pago', k: 'metodo_pago' }, { t: 'Líneas', k: 'lineas', num: 1 },
      { t: 'Descuento', k: 'descuento', num: 1, f: M() }, { t: 'Total', k: 'total', num: 1, f: M() }, { t: 'Estado', k: 'estado' },
    ], rows, {
      rowClass: (r) => (r.estado === 'Cancelada' ? 'danger' : ''),
      onSelect: async (r) => {
        sel = r;
        const det = await api('/api/ventas/detalle?id=' + r.id_venta);
        c.querySelector('.det').replaceChildren(grid([
          { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'No. de serie', k: 'numero_serie' }, { t: 'Cant.', k: 'cantidad', num: 1 },
          { t: 'Precio', k: 'precio_unitario', num: 1, f: M() }, { t: 'Importe', k: 'subtotal', num: 1, f: M() }, { t: 'Garantía', k: 'garantia' },
        ], det, { cls: 'short' }));
      },
    }));
  };
  c.querySelector('[data-buscar]').onclick = () => cargar().catch(showError);
  c.querySelector('[data-cancelar]').onclick = async () => {
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
  c.innerHTML = `<h1>Órdenes de servicio (taller)</h1><div class="sub">Cada cambio de estado lo registra el trigger <b>trg_OrdenServicio_Estado</b>, que además bloquea órdenes Entregadas o Canceladas.</div>
    <div class="row"><label><input type="checkbox" name="abiertas" checked> Solo órdenes abiertas</label></div>
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>
    <div class="two">
      <fieldset class="groupbox"><legend>Historial de estados (HistorialEstadoOrden)</legend><div class="hist"><div class="sub">Selecciona una orden.</div></div></fieldset>
      <fieldset class="groupbox"><legend>Cambiar estado</legend>
        <div class="row"><div class="field"><span>Orden:</span><input name="folio" disabled style="width:110px"></div>
        <div class="field"><span>Nuevo estado:</span><select name="estado">${options(estados)}</select></div>
        <button class="primary" data-aplicar style="margin-top:14px">Aplicar</button></div>
        <div class="sub" style="margin-top:8px">Prueba: quita el filtro, elige una orden Entregada e intenta cambiarla; SQL Server la rechaza.</div>
      </fieldset></div>`;
  let sel = null;
  const cargar = async () => {
    const rows = await api('/api/ordenes?abiertas=' + (c.querySelector('[name=abiertas]').checked ? '1' : ''));
    c.querySelector('.slot').replaceChildren(grid([
      { t: 'Folio', k: 'folio' }, { t: 'Recepción', k: 'fecha_recepcion', f: date }, { t: 'Cliente', k: 'cliente' }, { t: 'Equipo', k: 'equipo_descripcion' },
      { t: 'Falla', k: 'descripcion_falla' }, { t: 'Técnico', k: 'tecnico' }, { t: 'Estado', k: 'estado' },
      { t: 'Días', k: 'dias_en_taller', num: 1 }, { t: 'Total', k: 'total', num: 1, f: M() },
    ], rows, {
      rowClass: (r) => (r.estado === 'Cancelada' ? 'danger' : ''),
      onSelect: async (r) => {
        sel = r;
        c.querySelector('[name=folio]').value = r.folio;
        const cur = estados.find((e) => e.nombre === r.estado);
        c.querySelector('[name=estado]').value = Math.min(cur.id + 1, 5);
        const hist = await api('/api/ordenes/historial?id=' + r.id_orden);
        c.querySelector('.hist').replaceChildren(grid([
          { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'De', k: 'de_estado', f: (v) => esc(v || '(alta)') }, { t: 'A', k: 'a_estado' }, { t: 'Usuario', k: 'usuario' },
        ], hist, { cls: 'short' }));
      },
    }));
  };
  c.querySelector('[name=abiertas]').onchange = () => cargar().catch(showError);
  c.querySelector('[data-aplicar]').onclick = async () => {
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
  c.innerHTML = `<h1>Reportes</h1><div class="sub">Rango de fechas aplica a "Resumen por periodo" (<b>usp_ResumenPeriodo</b>) y "Ventas por categoría" (<b>fn_VentasPorCategoria</b>).</div>
    <div class="row">
      <div class="field"><span>Desde:</span><input type="date" name="desde" value="2026-05-01"></div>
      <div class="field"><span>Hasta:</span><input type="date" name="hasta" value="${new Date().toISOString().slice(0, 10)}"></div>
      <button class="primary" data-gen style="margin-top:14px">Generar</button><span class="spacer"></span>
      <button data-csv style="margin-top:14px">Exportar CSV</button>
    </div>
    <div class="tabs"></div><div class="tabpage"></div>`;
  const TABS = [
    ['resumen', 'Resumen por periodo', [{ t: 'Mes', k: 'mes' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }, { t: 'Ticket promedio', k: 'ticket_promedio', num: 1, f: M() }]],
    ['top', 'Top 10 productos', [{ t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Unidades', k: 'unidades_vendidas', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }, { t: 'Utilidad bruta', k: 'utilidad_bruta', num: 1, f: M() }]],
    ['empleados', 'Actividad por empleado', [{ t: 'Empleado', k: 'empleado' }, { t: 'Puesto', k: 'puesto' }, { t: 'Sucursal', k: 'sucursal' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Vendido', k: 'vendido', num: 1, f: M() }, { t: 'Órdenes taller', k: 'ordenes', num: 1 }]],
    ['stock', 'Stock bajo', [{ t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Categoría', k: 'categoria' }, { t: 'Stock', k: 'stock_actual', num: 1 }, { t: 'Mínimo', k: 'stock_minimo', num: 1 }, { t: 'Faltante', k: 'faltante', num: 1 }, { t: 'Proveedor más barato', k: 'proveedor_sugerido' }]],
    ['categorias', 'Ventas por categoría', [{ t: 'Categoría', k: 'categoria' }, { t: 'Ventas', k: 'ventas', num: 1 }, { t: 'Unidades', k: 'unidades', num: 1 }, { t: 'Ingresos', k: 'ingresos', num: 1, f: M() }]],
  ];
  let data = null, tab = 'resumen';
  const pintar = () => {
    c.querySelector('.tabs').innerHTML = TABS.map(([k, t]) => `<button data-t="${k}" class="${k === tab ? 'active' : ''}">${t}</button>`).join('');
    c.querySelectorAll('[data-t]').forEach((b) => b.onclick = () => { tab = b.dataset.t; pintar(); });
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
  c.innerHTML = `<h1>Auditoría</h1><div class="sub">Generada por los triggers <b>trg_Producto_Auditoria</b> y <b>trg_Venta_Auditoria</b>. Solo lectura: <b>trg_Auditoria_SoloLectura</b> impide editarla o borrarla.</div>
    ${rows.length ? '' : '<div class="banner">Aún no hay cambios auditados: registra o cancela una venta y vuelve aquí.</div>'}
    <div class="slot" style="flex:1;display:flex;min-height:0"></div>`;
  c.querySelector('.slot').appendChild(grid([
    { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Usuario app', k: 'usuario_app' }, { t: 'Login SQL', k: 'usuario' }, { t: 'Tabla', k: 'tabla' },
    { t: 'Operación', k: 'operacion' }, { t: 'Id', k: 'id_registro', num: 1 }, { t: 'Valor anterior', k: 'valor_anterior' },
    { t: 'Valor nuevo', k: 'valor_nuevo' }, { t: 'Motivo', k: 'motivo' },
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
    if (await messageBox('¿Eliminar el registro seleccionado?\nSi está en uso, SQL Server no lo permitirá.', { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
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
  const admin = state.user.rol === 'Administrador';
  c.innerHTML = `<h1>${cfg.titulo}</h1><div class="sub">Catálogo con INSERT, UPDATE y DELETE parametrizados. Borrar algo en uso lo impide la llave foránea.
      ${admin ? '' : '<b>Solo lectura:</b> únicamente el Administrador modifica catálogos.'}</div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Nombre:</span><input name="texto"></div><button class="primary" data-buscar style="margin-top:14px">Buscar</button>
      <span class="sub count" style="margin:14px 0 0"></span></div></fieldset>
    <div class="split"><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="crud"></div></div>`;
  const form = crud(c, {
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
  const puede = ['Administrador', 'Supervisor'].includes(state.user.rol);
  c.innerHTML = `<h1>Proveedores</h1><div class="sub">Guardado con <b>usp_ProveedorGuardar</b> · productos que surte desde la tabla N:M <b>ProductoProveedor</b></div>
    <fieldset class="groupbox"><legend>Búsqueda</legend><div class="row">
      <div class="field"><span>Razón social o RFC:</span><input name="texto" style="width:240px"></div>
      <button class="primary" data-buscar style="margin-top:14px">Buscar</button><span class="sub count" style="margin:14px 0 0"></span></div></fieldset>
    <div class="split"><div class="slot" style="flex:1;display:flex;min-height:0"></div><div class="crud"></div></div>
    <fieldset class="groupbox"><legend>Productos que surte</legend><div class="det"><div class="sub">Selecciona un proveedor.</div></div></fieldset>`;
  const form = crud(c, {
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
      { t: 'Razón social', k: 'razon_social' }, { t: 'RFC', k: 'rfc' }, { t: 'Correo', k: 'correo' }, { t: 'Teléfono', k: 'telefono' },
      { t: 'Productos', k: 'productos', num: 1 }, { t: 'Compras', k: 'compras', num: 1 }, { t: 'Total comprado', k: 'total_comprado', num: 1, f: M() },
      { t: 'Activo', k: 'activo', f: (v) => (v ? 'Sí' : 'No') },
    ], rows, {
      rowClass: (r) => (r.activo ? '' : 'danger'),
      onSelect: async (r) => {
        form.seleccionar(r);
        const det = await api('/api/proveedores/productos?id=' + r.id_proveedor);
        c.querySelector('.det').replaceChildren(grid([
          { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Costo del proveedor', k: 'costo_proveedor', num: 1, f: M() },
          { t: 'Costo actual', k: 'costo_actual', num: 1, f: M() }, { t: 'Stock', k: 'stock_actual', num: 1 },
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
  c.innerHTML = `<h1>Compras a proveedor</h1><div class="sub">Se guarda con <b>usp_RegistrarCompra</b>: encabezado, detalle, entrada al stock, piezas con número de serie y relación ProductoProveedor en una transacción.</div>
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
      { t: 'SKU', k: 'sku' }, { t: 'Producto', k: 'nombre' }, { t: 'Cantidad', k: 'cantidad', num: 1 },
      { t: 'Costo', k: 'costo_unitario', num: 1, f: M() }, { t: 'Importe', k: 'importe', num: 1, f: M() }, { t: 'Series', k: 'series_txt' },
    ], lineas.map((l) => ({ ...l, importe: l.cantidad * l.costo_unitario, series_txt: (l.series || []).join(', ') })), { onSelect: (r) => sel = r }));
    c.querySelector('.total').textContent = 'Total: ' + money(lineas.reduce((a, l) => a + l.cantidad * l.costo_unitario, 0));
  };
  const recientes = (rows) => c.querySelector('.recientes').replaceChildren(grid([
    { t: 'Folio', k: 'folio' }, { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Proveedor', k: 'proveedor' }, { t: 'Recibió', k: 'recibio' },
    { t: 'Líneas', k: 'lineas', num: 1 }, { t: 'Total', k: 'total', num: 1, f: M() }, { t: 'Estado', k: 'estado' },
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
      showError(new Error(`SQL Server rechazó la compra y no se guardó nada (ROLLBACK):\n\n${e.message}`));
    }
  };
  alCambiarProducto(); pintar(); recientes(d.recientes);
}

/* ---------- FrmUsuarios ---------- */
async function renderUsuarios(c) {
  const d = await api('/api/usuarios');
  c.innerHTML = `<h1>Usuarios y roles</h1><div class="sub">Guardado con <b>usp_UsuarioGuardar</b> (Usuario + UsuarioRol en una transacción). La contraseña se guarda como hash SHA2_256 y nunca se muestra.</div>
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
    { t: 'Usuario', k: 'usuario' }, { t: 'Empleado', k: 'empleado' }, { t: 'Puesto', k: 'puesto' }, { t: 'Rol', k: 'rol' },
    { t: 'Activo', k: 'activo', f: (v) => (v ? 'Sí' : 'Bloqueado') }, { t: 'Último acceso', k: 'ultimo_acceso', f: datetime },
    { t: 'Intentos fallidos', k: 'intentos_fallidos', num: 1 },
  ], d.usuarios, { rowClass: (r) => (r.activo ? '' : 'danger'), onSelect: (r) => form.seleccionar({ ...r, contrasena: '' }) }));
}

/* ---------- FrmAjusteStock ---------- */
async function renderAjustes(c) {
  const d = await api('/api/ajustes');
  c.innerHTML = `<h1>Ajustes de inventario y precios</h1><div class="sub">Solo Supervisor y Administrador. Las reglas las valida SQL Server y cada cambio queda en la auditoría con su motivo.</div>
    <fieldset class="groupbox"><legend>Ajuste de inventario (usp_AjustarStock · máximo ±20 piezas)</legend>
      <div class="row"><div class="field"><span>Producto (sin número de serie):</span><select name="producto" style="width:420px">${options(d.productos)}</select></div>
      <div class="field"><span>Cantidad (+ entra, − sale):</span><input name="cant" type="number" min="-20" max="20" value="0" style="width:90px"></div></div>
      <div class="row" style="margin-top:6px"><div class="field" style="flex:1"><span>Motivo: *</span><input name="motivo" maxlength="200"></div>
      <button class="primary" data-stock style="margin-top:14px">Aplicar ajuste</button></div></fieldset>
    <fieldset class="groupbox"><legend>Cambio de precios por categoría (usp_AjustarPrecio · máximo ±30 %)</legend>
      <div class="row"><div class="field"><span>Categoría:</span><select name="categoria" style="width:260px">${options(d.categorias)}</select></div>
      <div class="field"><span>Porcentaje:</span><input name="pct" type="number" min="-30" max="30" step="0.5" value="0" style="width:90px"></div>
      <button class="primary" data-precio style="margin-top:14px">Aplicar precios</button></div>
      <div class="sub" style="margin-top:6px">Si algún precio quedara por debajo del costo, la restricción CK_Producto_precio_vs_costo detiene todo y no se cambia ninguno.</div></fieldset>`;
  const f = (n) => c.querySelector(`[name=${n}]`);
  c.querySelector('[data-stock]').onclick = async () => {
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
  c.querySelector('[data-precio]').onclick = async () => {
    const pct = +f('pct').value;
    if (!pct) return messageBox('El porcentaje no puede ser cero.', { icon: 'error' });
    const cat = f('categoria').selectedOptions[0].textContent;
    if (await messageBox(`¿Cambiar ${pct > 0 ? '+' : ''}${pct} % el precio de todos los productos activos de ${cat}?`, { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await api('/api/ajustes/precio', { porcentaje: pct, id_categoria: f('categoria').value });
      await messageBox('Precios actualizados; cada cambio quedó en la auditoría.', { icon: 'ok' }); f('pct').value = 0; }
    catch (e) { showError(new Error(`No se cambiaron los precios (ROLLBACK):\n\n${e.message}`)); }
  };
}

// Captura automática para la documentación: ?usuario=admin&modulo=productos
// entra con un usuario demo y abre esa pantalla (solo usuarios demo con su contraseña conocida).
async function inicio() {
  const p = new URLSearchParams(location.search);
  const demo = ['admin', 'csupervisor', 'loperador', 'fconsulta'];
  if (!demo.includes(p.get('usuario'))) return renderLogin();
  try {
    const r = await api('/api/login', { usuario: p.get('usuario'), contrasena: 'Tecno123!' });
    state.token = r.token; state.user = r.user;
    state.cat = await api('/api/catalogos');
    state.modulo = r.user.modulos.includes(p.get('modulo')) ? p.get('modulo') : 'inicio';
    renderMain();
  } catch (e) { renderLogin(); }
}
inicio();
