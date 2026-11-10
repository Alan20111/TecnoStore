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
  venta: ['Nueva venta', renderVenta],
  ventas: ['Historial de ventas', renderVentas],
  ordenes: ['Órdenes de servicio', renderOrdenes],
  reportes: ['Reportes', renderReportes],
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
    const tecla = { F2: 'productos', F3: 'clientes', F5: 'venta', F6: 'ordenes' }[ev.key];
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
    ['Catálogos', item('productos', 'F2') + item('clientes', 'F3')],
    ['Procesos', item('venta', 'F5') + item('ordenes', 'F6')],
    ['Consultas', item('ventas') + item('reportes')],
    ['Administración', item('auditoria')],
    ['Ayuda', `<button data-accion="atajos">Atajos de teclado</button><button data-accion="acerca">Acerca de TecnoStore</button>`],
  ];
  return grupos.filter(([, html]) => html)
    .map(([t, html]) => `<div class="menu"><span class="top">${t}</span><div class="dropdown">${html}</div></div>`).join('');
}

function cerrarMenus() { document.querySelectorAll('.menu.open').forEach((m) => m.classList.remove('open')); }

const acercaDe = () => messageBox('TecnoStore — Sistema Integral de Gestión\nProyecto final de Taller de Bases de Datos\n\nVista previa conectada a SQL Server. La aplicación real es la solución de Windows Forms en 03_WindowsForms.', { title: 'Acerca de' });
const atajos = () => messageBox('F2  Productos\nF3  Clientes\nF5  Nueva venta\nF6  Órdenes de servicio\nEnter  Buscar en los filtros\nEsc  Cerrar menús', { title: 'Atajos de teclado' });

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
    if (await messageBox(`¿Cancelar la venta ${sel.folio} por ${money(sel.total)}?\nSe reintegrará el stock.`, { icon: 'question', buttons: ['Sí', 'No'] }) !== 'Sí') return;
    try { await api('/api/ventas/cancelar', { id_venta: sel.id_venta }); await messageBox(`Venta ${sel.folio} cancelada.`, { icon: 'ok' }); await cargar(); }
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
    { t: 'Fecha', k: 'fecha', f: datetime }, { t: 'Usuario SQL', k: 'usuario' }, { t: 'Tabla', k: 'tabla' }, { t: 'Operación', k: 'operacion' },
    { t: 'Id', k: 'id_registro', num: 1 }, { t: 'Valor anterior', k: 'valor_anterior' }, { t: 'Valor nuevo', k: 'valor_nuevo' },
  ], rows));
}

renderLogin();
