#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Genera la documentación y las evidencias del proyecto (Mac):

  01_Documentacion/  DocumentoTecnico.pdf, ManualUsuario.pdf, DER.png, DER.pdf,
                     DiccionarioDatos.pdf, MatrizPermisos.pdf, CasosPrueba.pdf
                     fuentes/  (los HTML de los que salen los PDF)
  04_Evidencias/     salidas_sql/  (salida real de los scripts en SQL Server)
                     capturas_vista_previa/  (pantallas de la vista previa)

Todo sale de la base real: el diccionario y la matriz se leen de los
catálogos del sistema de SQL Server y los casos de prueba de ejecutar
12_Pruebas.sql. Requisitos: Docker con SQL Server arriba, la vista previa
corriendo en http://localhost:5173 y Google Chrome instalado.

Uso:  python3 scripts/generar_documentacion.py
"""
import datetime
import html
import json
import os
import re
import subprocess
import sys
import urllib.request

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from doc_textos import COLUMNAS, REGLAS, TABLAS  # noqa: E402

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DOC = os.path.join(RAIZ, '01_Documentacion')
FUENTES = os.path.join(DOC, 'fuentes')
EVID = os.path.join(RAIZ, '04_Evidencias')
SALIDAS = os.path.join(EVID, 'salidas_sql')
CAPTURAS = os.path.join(EVID, 'capturas_vista_previa')
CHROME = '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome'
PREVIEW = 'http://localhost:5173'
HOY = datetime.date.today().strftime('%d/%m/%Y')
e = html.escape

for d in (DOC, FUENTES, EVID, SALIDAS, CAPTURAS):
    os.makedirs(d, exist_ok=True)


# ----------------------------------------------------------------- SQL Server
def sqlcmd(*args):
    r = subprocess.run([os.path.join(RAIZ, 'scripts', 'sql.sh'), *args], capture_output=True, text=True)
    return r.stdout + r.stderr


def consulta(q):
    """Ejecuta un SELECT ... FOR JSON y devuelve la lista de filas."""
    salida = sqlcmd('-d', 'TecnoStore', '-h', '-1', '-y', '8000', '-Q', 'SET NOCOUNT ON; ' + q + ' FOR JSON PATH, INCLUDE_NULL_VALUES')
    texto = ''.join(l.strip() for l in salida.splitlines())
    return json.loads(texto) if texto else []


def ejecutar_script(nombre, destino=None, *extra):
    salida = sqlcmd('-W', *extra, '-i', '/scripts/' + nombre + '.sql')
    salida = '\n'.join(l for l in salida.splitlines() if 'Changed database context' not in l)
    if destino:
        with open(os.path.join(SALIDAS, destino), 'w', encoding='utf-8') as f:
            f.write(salida)
    return salida


def crear_base(destino=None):
    r = subprocess.run([os.path.join(RAIZ, 'scripts', 'crear_base.sh')], capture_output=True, text=True)
    if r.returncode != 0:
        sys.exit('No se pudo crear la base:\n' + r.stdout + r.stderr)
    if destino:
        with open(os.path.join(SALIDAS, destino), 'w', encoding='utf-8') as f:
            f.write(r.stdout)


# ------------------------------------------------------------------- Chrome
def chrome(*args):
    subprocess.run([CHROME, '--headless=new', '--disable-gpu', '--hide-scrollbars', '--no-pdf-header-footer',
                    '--virtual-time-budget=8000', *args], capture_output=True)


def a_pdf(html_path, pdf_path):
    chrome('--print-to-pdf=' + pdf_path, 'file://' + html_path)
    print('  ' + os.path.relpath(pdf_path, RAIZ))


def captura(url, destino, ancho=1440, alto=900):
    chrome('--window-size=%d,%d' % (ancho, alto), '--screenshot=' + destino, url)


# ----------------------------------------------------------------- HTML base
CSS = """
@page { size: A4; margin: 18mm 16mm; }
body { font: 10.5pt/1.45 "Segoe UI", -apple-system, "Helvetica Neue", Arial, sans-serif; color: #1a1a1a; }
h1 { font-size: 20pt; margin: 0 0 6pt; color: #0b3b6f; }
h2 { font-size: 14pt; margin: 18pt 0 6pt; color: #0b3b6f; border-bottom: 1.5pt solid #0b3b6f; padding-bottom: 2pt; break-after: avoid; }
h3 { font-size: 11.5pt; margin: 12pt 0 4pt; break-after: avoid; }
p { margin: 4pt 0 6pt; }
table { border-collapse: collapse; width: 100%; margin: 4pt 0 10pt; font-size: 8.8pt; }
th, td { border: .6pt solid #b9c2cc; padding: 3pt 5pt; text-align: left; vertical-align: top; }
th { background: #e8eef5; }
tr { break-inside: avoid; }
code { font: 8.6pt Menlo, Consolas, monospace; background: #f2f4f7; padding: 0 2pt; }
.small { font-size: 8.5pt; color: #555; }
.portada { height: 245mm; display: flex; flex-direction: column; justify-content: center; text-align: center; break-after: page; }
.portada h1 { font-size: 30pt; }
.portada .sub { font-size: 14pt; color: #444; }
.portada table { width: 70%; margin: 24pt auto; font-size: 11pt; }
.salto { break-before: page; }
figure { margin: 8pt 0 12pt; text-align: center; break-inside: avoid; }
figure img { max-width: 100%; border: .6pt solid #b9c2cc; }
figcaption { font-size: 8.5pt; color: #555; margin-top: 3pt; }
.ok { color: #107c10; font-weight: 600; } .no { color: #c50f1f; font-weight: 600; }
.nota { background: #fff4ce; border: .6pt solid #e6c35c; padding: 5pt 8pt; margin: 6pt 0; font-size: 9pt; }
.indice li { margin: 1pt 0; }
"""


def pagina(titulo, cuerpo):
    return ('<!doctype html><html lang="es"><head><meta charset="utf-8"><title>%s</title><style>%s</style></head>'
            '<body>%s</body></html>') % (e(titulo), CSS, cuerpo)


def tabla(encabezados, filas):
    t = '<table><tr>' + ''.join('<th>%s</th>' % e(h) for h in encabezados) + '</tr>'
    for f in filas:
        t += '<tr>' + ''.join('<td>%s</td>' % c for c in f) + '</tr>'
    return t + '</table>'


def guardar(nombre, contenido):
    ruta = os.path.join(FUENTES, nombre)
    with open(ruta, 'w', encoding='utf-8') as f:
        f.write(contenido)
    return ruta


# ======================================================== 1. Base y metadatos
print('1. Creando la base desde cero y leyendo metadatos…')
crear_base('01_crear_base_y_seguridad.txt')

COLS = consulta("""
SELECT t.name AS tabla, c.column_id AS n, c.name AS col, ty.name AS tipo, c.max_length, c.precision, c.scale,
       c.is_nullable AS nulo, c.is_identity AS identidad, dc.definition AS defecto,
       CASE WHEN EXISTS (SELECT 1 FROM sys.index_columns ic JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                         WHERE i.is_primary_key = 1 AND ic.object_id = t.object_id AND ic.column_id = c.column_id) THEN 1 ELSE 0 END AS pk,
       CASE WHEN EXISTS (SELECT 1 FROM sys.index_columns ic JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                         WHERE i.is_unique_constraint = 1 AND ic.object_id = t.object_id AND ic.column_id = c.column_id) THEN 1 ELSE 0 END AS uq,
       (SELECT TOP 1 OBJECT_NAME(fkc.referenced_object_id) FROM sys.foreign_key_columns fkc
         WHERE fkc.parent_object_id = t.object_id AND fkc.parent_column_id = c.column_id) AS fk
FROM sys.tables t JOIN sys.columns c ON c.object_id = t.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = t.object_id AND dc.parent_column_id = c.column_id
ORDER BY t.name, c.column_id""")
CHECKS = consulta("""SELECT OBJECT_NAME(parent_object_id) AS tabla, name, definition, parent_column_id AS col
                     FROM sys.check_constraints ORDER BY tabla, name""")
FKS = consulta("""SELECT fk.name, OBJECT_NAME(fk.parent_object_id) AS hija, COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS col_hija,
                         OBJECT_NAME(fk.referenced_object_id) AS padre, COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS col_padre,
                         c.is_nullable AS nulo
                  FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                  JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
                  ORDER BY hija, fk.name""")
UQS = consulta("""SELECT OBJECT_NAME(parent_object_id) AS tabla, name FROM sys.key_constraints WHERE type = 'UQ' ORDER BY tabla, name""")
DFS = consulta("""SELECT OBJECT_NAME(parent_object_id) AS tabla, name, definition FROM sys.default_constraints ORDER BY tabla, name""")
OBJETOS = consulta("""SELECT name, type_desc AS tipo FROM sys.objects WHERE type IN ('V','P','FN','IF','TR') ORDER BY type_desc, name""")
PERMISOS = consulta("""
SELECT pr.name AS principal, pe.permission_name AS permiso, pe.state_desc AS estado,
       CASE pe.class WHEN 3 THEN 'SCHEMA::' + SCHEMA_NAME(pe.major_id) ELSE OBJECT_NAME(pe.major_id) END AS objeto
FROM sys.database_permissions pe JOIN sys.database_principals pr ON pr.principal_id = pe.grantee_principal_id
WHERE pr.name LIKE 'rol[_]%' OR pr.name LIKE 'tecno[_]%' ORDER BY pr.name, objeto, permiso""")
CONTEO = consulta("""SELECT
  (SELECT COUNT(*) FROM sys.tables) AS tablas, (SELECT COUNT(*) FROM sys.foreign_keys) AS fks,
  (SELECT COUNT(*) FROM sys.check_constraints) AS checks, (SELECT COUNT(*) FROM sys.key_constraints WHERE type = 'UQ') AS uniques,
  (SELECT COUNT(*) FROM sys.default_constraints) AS defaults, (SELECT COUNT(*) FROM sys.views) AS vistas,
  (SELECT COUNT(*) FROM sys.procedures) AS procs, (SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF')) AS funcs,
  (SELECT COUNT(*) FROM sys.triggers) AS triggers,
  (SELECT COUNT(*) FROM sys.database_principals WHERE type = 'R' AND is_fixed_role = 0 AND name <> 'public') AS roles""")[0]
FILAS = {r['tabla']: r['filas'] for r in consulta(
    """SELECT t.name AS tabla, SUM(p.rows) AS filas FROM sys.tables t JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
       GROUP BY t.name""")}
EJEMPLOS = {}
for t in sorted({c['tabla'] for c in COLS}):
    fila = consulta('SELECT TOP 1 * FROM dbo.[%s] ORDER BY 1 DESC' % t) if t != 'Configuracion' else consulta('SELECT TOP 1 * FROM Configuracion')
    EJEMPLOS[t] = fila[0] if fila else {}

# ======================================================== 2. Evidencias SQL
print('2. Ejecutando consultas, transacciones y pruebas…')
ejecutar_script('05_Consultas', '05_Consultas.txt')
ejecutar_script('11_Transacciones', '11_Transacciones.txt')
crear_base()
salida_pruebas = ejecutar_script('12_Pruebas', '12_Pruebas.txt', '-s', '|')
PRUEBAS = []
for linea in salida_pruebas.splitlines():
    partes = linea.split('|')
    if len(partes) == 7 and re.match(r'^P\d\d$', partes[0]):
        PRUEBAS.append(dict(zip(['caso', 'tipo', 'descripcion', 'entrada', 'esperado', 'obtenido', 'resultado'], partes)))
crear_base()   # queda limpia para las capturas
print('   %d casos, %d pasan' % (len(PRUEBAS), sum(p['resultado'] == 'PASA' for p in PRUEBAS)))

# ======================================================== 3. Capturas
print('3. Capturas de la vista previa…')
try:
    urllib.request.urlopen(PREVIEW, timeout=3)
    hay_preview = True
except Exception:
    hay_preview = False
    print('   (la vista previa no está corriendo; se omiten capturas nuevas)')

PANTALLAS = [
    ('01_login', None, None, 'Inicio de sesión'),
    ('02_inicio_admin', 'admin', 'inicio', 'Inicio del Administrador: indicadores y alertas'),
    ('03_productos', 'admin', 'productos', 'Productos: búsqueda, alertas de stock y margen'),
    ('04_clientes', 'admin', 'clientes', 'Clientes: total comprado y nivel calculados con funciones'),
    ('05_proveedores', 'admin', 'proveedores', 'Proveedores y productos que surte (N:M)'),
    ('06_categorias', 'admin', 'categorias', 'Catálogo de categorías'),
    ('07_marcas', 'admin', 'marcas', 'Catálogo de marcas'),
    ('08_tipos_servicio', 'admin', 'tipos', 'Catálogo de tipos de servicio'),
    ('09_nueva_venta', 'admin', 'venta', 'Nueva venta (maestro-detalle)'),
    ('10_ordenes', 'admin', 'ordenes', 'Órdenes de servicio del taller'),
    ('11_compras', 'admin', 'compras', 'Compras a proveedor (maestro-detalle)'),
    ('12_historial', 'admin', 'ventas', 'Historial de ventas con filtros y cancelación'),
    ('13_reportes', 'admin', 'reportes', 'Reportes'),
    ('14_usuarios', 'admin', 'usuarios', 'Usuarios y roles'),
    ('15_ajustes', 'admin', 'ajustes', 'Ajustes de inventario y precios'),
    ('16_auditoria', 'admin', 'auditoria', 'Auditoría'),
    ('17_inicio_operador', 'loperador', 'inicio', 'Menú del Operador: sin reportes, compras ni auditoría'),
    ('18_inicio_consulta', 'fconsulta', 'inicio', 'Menú de Consulta: solo lectura'),
]
if hay_preview:
    for archivo, usuario, modulo, _ in PANTALLAS:
        url = PREVIEW + ('/?usuario=%s&modulo=%s' % (usuario, modulo) if usuario else '/')
        captura(url, os.path.join(CAPTURAS, archivo + '.png'))
    print('   %d capturas' % len(PANTALLAS))


def figura(archivo, pie, ancho='100%'):
    ruta = os.path.join(CAPTURAS, archivo + '.png')
    if not os.path.exists(ruta):
        return ''
    return ('<figure><img src="file://%s" style="width:%s"><figcaption>%s · vista previa del diseño (las capturas finales '
            'deben tomarse de la aplicación Windows Forms)</figcaption></figure>') % (ruta, ancho, e(pie))


# ======================================================== 4. DER
print('4. DER…')
CLAVES = {}
for c in COLS:
    if c['pk'] or c['fk']:
        CLAVES.setdefault(c['tabla'], []).append(c)


def tipo_mermaid(c):
    t = c['tipo'].upper()
    return t if t in ('INT', 'BIT', 'DATE') else t


lineas = ['erDiagram']
for t in sorted(CLAVES):
    lineas.append('  %s {' % t)
    for c in CLAVES[t]:
        marca = ','.join(x for x in ('PK' if c['pk'] else '', 'FK' if c['fk'] else '') if x)
        lineas.append('    %s %s %s' % (tipo_mermaid(c), c['col'], marca))
    lineas.append('  }')
for fk in FKS:
    card = '|o--o{' if fk['nulo'] else '||--o{'
    lineas.append('  %s %s %s : "%s"' % (fk['padre'], card, fk['hija'], fk['col_hija']))
mermaid = '\n'.join(lineas)
with open(os.path.join(DOC, 'DER.mmd'), 'w', encoding='utf-8') as f:
    f.write(mermaid + '\n')

der_html = guardar('DER.html', """<!doctype html><html><head><meta charset="utf-8"><title>DER TecnoStore</title>
<style>body{margin:0;background:#fff;font-family:Arial} h1{font:bold 30px Arial;margin:20px 30px 0;color:#0b3b6f}
p{margin:4px 30px 0;font:16px Arial;color:#444} .mermaid svg{width:100%%!important;max-width:none!important;height:auto}</style>
<script src="https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js"></script></head><body>
<h1>TecnoStore · Diagrama Entidad-Relación</h1><p>%d tablas · %d llaves foráneas · se muestran llaves primarias (PK) y foráneas (FK);
el resto de columnas está en el diccionario de datos. ||--o{ obligatoria · |o--o{ opcional (FK que admite NULL)</p>
<pre class="mermaid">%s</pre>
<script>mermaid.initialize({startOnLoad:true, theme:'default', er:{useMaxWidth:false, layoutDirection:'TB', minEntityWidth:90, fontSize:14}});</script>
</body></html>""" % (CONTEO['tablas'], CONTEO['fks'], e(mermaid)))
captura('file://' + der_html, os.path.join(DOC, 'DER.png'), 3600, 1500)
chrome('--print-to-pdf=' + os.path.join(DOC, 'DER.pdf'), '--no-pdf-header-footer', 'file://' + der_html)
print('  01_Documentacion/DER.png')

# ======================================================== 5. Diccionario
print('5. Diccionario de datos…')


def tipo_sql(c):
    t = c['tipo']
    if t in ('varchar', 'char', 'nvarchar', 'varbinary'):
        n = c['max_length'] if t != 'nvarchar' else c['max_length'] // 2
        return '%s(%s)' % (t.upper(), 'MAX' if n == -1 else n)
    if t == 'decimal':
        return 'DECIMAL(%d,%d)' % (c['precision'], c['scale'])
    return t.upper()


def descripcion(c):
    clave = c['tabla'] + '.' + c['col']
    if clave in COLUMNAS:
        return COLUMNAS[clave]
    col = c['col']
    if c['pk'] and c['identidad']:
        return 'Identificador único (autonumérico).'
    if c['fk']:
        return 'Referencia a %s.' % c['fk']
    if col == 'activo':
        return 'Baja lógica: 0 = inactivo, se conserva el historial.'
    if col == 'nombre':
        return 'Nombre.'
    if col in ('apellidos', 'telefono', 'correo', 'direccion', 'descripcion'):
        return col.capitalize().replace('Telefono', 'Teléfono').replace('Direccion', 'Dirección').replace('Descripcion', 'Descripción') + '.'
    if col.startswith('fecha'):
        return 'Fecha y hora del registro.' if col == 'fecha' else col.replace('_', ' ').capitalize() + '.'
    if col in ('cantidad',):
        return 'Unidades.'
    if col == 'subtotal':
        return 'cantidad × precio/costo unitario.'
    return col.replace('_', ' ').capitalize() + '.'


def restricciones(c):
    r = []
    if c['pk']:
        r.append('PK')
    if c['identidad']:
        r.append('IDENTITY')
    if c['fk']:
        r.append('FK → ' + c['fk'])
    if c['uq']:
        r.append('UNIQUE')
    for ck in CHECKS:
        if ck['tabla'] == c['tabla'] and (('[%s]' % c['col']) in ck['definition']):
            r.append(ck['name'])
    return ', '.join(r)


def ejemplo(c):
    v = EJEMPLOS.get(c['tabla'], {}).get(c['col'])
    if c['col'] == 'hash_contrasena':
        return '(hash de 32 bytes)'
    if v is None:
        return 'NULL'
    v = str(v)
    if re.match(r'^\d{4}-\d\d-\d\dT', v):
        v = v.replace('T', ' ')[:16]
    return v if len(v) < 40 else v[:37] + '…'


cuerpo = ('<h1>Diccionario de datos · TecnoStore</h1><p class="small">Generado de los catálogos de SQL Server el %s. '
          '%d tablas. Convenciones: tablas en PascalCase singular, columnas en snake_case, PK <code>id_&lt;tabla&gt;</code>, '
          'constraints con nombre (PK_, FK_, UQ_, CK_, DF_), fechas DATETIME2/DATE e importes DECIMAL. '
          'El ejemplo es un valor real de la base demo.</p>') % (HOY, CONTEO['tablas'])
for t in sorted({c['tabla'] for c in COLS}, key=lambda x: (list(TABLAS).index(x) if x in TABLAS else 99, x)):
    grupo, desc = TABLAS.get(t, ('', ''))
    cuerpo += '<h3>%s <span class="small">· %s · %d filas demo</span></h3><p>%s</p>' % (e(t), e(grupo), FILAS.get(t, 0), e(desc))
    cuerpo += tabla(['Columna', 'Tipo', 'Nulo', 'Default', 'Restricciones', 'Descripción', 'Ejemplo'], [
        ['<b>%s</b>' % e(c['col']), e(tipo_sql(c)), 'Sí' if c['nulo'] else 'No', e((c['defecto'] or '').strip('()')),
         e(restricciones(c)), e(descripcion(c)), e(ejemplo(c))]
        for c in COLS if c['tabla'] == t])
a_pdf(guardar('DiccionarioDatos.html', pagina('Diccionario de datos', cuerpo)), os.path.join(DOC, 'DiccionarioDatos.pdf'))

# ======================================================== 6. Matriz de permisos
print('6. Matriz de permisos…')
MATRIZ = [
    ('Catálogos (Categoría, Marca, Tipo de servicio…)', 'CRUD', 'R', 'R', 'R'),
    ('Proveedores', 'CRUD', 'R + guardar (SP)', 'R', 'R'),
    ('Productos', 'CRUD', 'R + guardar y ajustar (SP)', 'R', 'R'),
    ('Clientes', 'CRUD', 'R + guardar (SP)', 'R + guardar (SP)', 'R'),
    ('Proceso principal: venta', 'CRUD', 'Registra, cancela y autoriza descuentos', 'Registra (sin descuento > 10 %)', 'R'),
    ('Órdenes de servicio', 'CRUD', 'Registra y avanza', 'Registra y avanza', 'R'),
    ('Compras a proveedor', 'CRUD', 'Registra (SP)', '—', '—'),
    ('Ajustes de stock y precio', 'Sí', 'Sí (SP)', '—', '—'),
    ('Reportes', 'R', 'R', 'R limitado (sin utilidad ni desempeño de otros)', 'R'),
    ('Seguridad (usuarios y roles)', 'CRUD', '—', '—', '—'),
    ('Auditoría', 'R', 'R', '—', '—'),
]
cuerpo = ('<h1>Matriz de permisos · TecnoStore</h1><p class="small">Generada el %s. Roles de base de datos: '
          '<code>rol_administrador</code>, <code>rol_supervisor</code>, <code>rol_operador</code>, <code>rol_consulta</code> '
          '(script <code>07_Seguridad.sql</code>). Nadie tiene <code>db_owner</code>; la auditoría no la modifica nadie.</p>') % HOY
cuerpo += '<h2>Matriz por módulo</h2>' + tabla(['Objeto / módulo', 'Administrador', 'Supervisor', 'Operador', 'Consulta'],
                                               [[e(x) for x in fila] for fila in MATRIZ])
cuerpo += ('<h2>Usuarios de prueba</h2>' + tabla(['Login SQL Server', 'Rol de base de datos', 'Usuario de la app', 'Uso'], [
    ['tecno_admin', 'rol_administrador', 'admin', 'Administrador'],
    ['tecno_supervisor', 'rol_supervisor', 'csupervisor', 'Supervisor'],
    ['tecno_operador', 'rol_operador', 'loperador / smendez', 'Operador'],
    ['tecno_consulta', 'rol_consulta', 'fconsulta', 'Solo lectura'],
    ['tecno_login', '(ninguno)', '—', 'Solo puede ejecutar usp_Login para la pantalla de acceso'],
]))
cuerpo += ('<h2>GRANT, DENY y REVOKE</h2><p><b>GRANT</b> da un permiso; <b>DENY</b> lo niega y gana sobre cualquier GRANT '
           '(por ejemplo, el esquema completo es legible pero <code>Usuario</code> y <code>Auditoria</code> no); '
           '<b>REVOKE</b> quita un GRANT o DENY previo sin dejar negación (en el script se otorga por error '
           '<code>usp_AjustarStock</code> al operador y se revoca). La demostración con <code>EXECUTE AS USER</code> / '
           '<code>REVERT</code> está al final de <code>07_Seguridad.sql</code>; su salida real está en '
           '<code>04_Evidencias/salidas_sql/01_crear_base_y_seguridad.txt</code>.</p>')
cuerpo += '<h2>Permisos reales en SQL Server</h2><p class="small">Leídos de <code>sys.database_permissions</code>.</p>'
cuerpo += tabla(['Principal', 'Permiso', 'Estado', 'Objeto'],
                [[e(p['principal']), e(p['permiso']), ('<span class="no">DENY</span>' if p['estado'] == 'DENY' else e(p['estado'])), e(p['objeto'] or '')]
                 for p in PERMISOS])
a_pdf(guardar('MatrizPermisos.html', pagina('Matriz de permisos', cuerpo)), os.path.join(DOC, 'MatrizPermisos.pdf'))

# ======================================================== 7. Casos de prueba
print('7. Casos de prueba…')
pasan = sum(p['resultado'] == 'PASA' for p in PRUEBAS)
por_tipo = {}
for p in PRUEBAS:
    por_tipo[p['tipo']] = por_tipo.get(p['tipo'], 0) + 1
cuerpo = ('<h1>Casos de prueba · TecnoStore</h1><p class="small">Ejecutados el %s con <code>02_SQL/12_Pruebas.sql</code> sobre una base '
          'recién creada. Cada caso compara el resultado esperado contra lo que devolvió SQL Server. '
          'Evidencia: <code>04_Evidencias/salidas_sql/12_Pruebas.txt</code>.</p>') % HOY
cuerpo += '<p><b>Resultado: %d de %d casos pasan.</b></p>' % (pasan, len(PRUEBAS))
MINIMOS = {'Correcta': 8, 'Validación': 5, 'Transacción': 3, 'Seguridad': 2, 'Trigger': 2}
cuerpo += tabla(['Tipo', 'Mínimo pedido', 'Casos'], [[e(t), str(m), str(por_tipo.get(t, 0))] for t, m in MINIMOS.items()])
cuerpo += tabla(['Caso', 'Tipo', 'Descripción', 'Entrada', 'Esperado', 'Obtenido', 'Resultado'], [
    [e(p['caso']), e(p['tipo']), e(p['descripcion']), e(p['entrada']), e(p['esperado']), e(p['obtenido']),
     '<span class="%s">%s</span>' % ('ok' if p['resultado'] == 'PASA' else 'no', e(p['resultado']))] for p in PRUEBAS])
cuerpo += ('<h2>Pruebas de la aplicación Windows Forms</h2><p>Las mismas reglas se ven desde la aplicación; para la evidencia en Windows, '
           'repetir estos pasos y guardar captura en <code>04_Evidencias/capturas/</code>:</p>')
cuerpo += tabla(['#', 'Pantalla', 'Pasos', 'Resultado esperado'], [
    ['A1', 'Inicio de sesión', 'Entrar con <code>admin</code> y contraseña incorrecta 5 veces', 'Mensaje de error; al quinto intento el usuario queda bloqueado'],
    ['A2', 'Nueva venta', 'Vender más piezas de las que hay en stock', '"Stock insuficiente…"; la venta no se guarda (ROLLBACK)'],
    ['A3', 'Nueva venta', 'Como <code>loperador</code>, descuento de 15 %', '"Un descuento mayor al permitido requiere autorización de un Supervisor"'],
    ['A4', 'Productos', 'Editar un precio por debajo del costo', 'La app y SQL Server lo rechazan'],
    ['A5', 'Categorías', 'Eliminar "Celulares"', '"No se puede eliminar: hay productos… que lo usan"'],
    ['A6', 'Compras', 'Capturar un número de serie ya registrado', '"Uno o más números de serie ya están registrados"; no entra nada al stock'],
    ['A7', 'Órdenes', 'Cambiar una orden Entregada', '"Una orden Entregada o Cancelada ya no puede cambiar de estado"'],
    ['A8', 'Menú', 'Entrar como <code>fconsulta</code>', 'Solo ve consultas; no aparecen Nueva venta, Compras ni Auditoría'],
    ['A9', 'Ajustes → Auditoría', 'Ajustar −2 piezas con motivo y abrir Auditoría', 'Aparece el cambio con usuario de la app y motivo'],
])
a_pdf(guardar('CasosPrueba.html', pagina('Casos de prueba', cuerpo)), os.path.join(DOC, 'CasosPrueba.pdf'))

# ======================================================== 8. Documento técnico
print('8. Documento técnico…')
import doc_tecnico  # noqa: E402

datos = dict(HOY=HOY, CONTEO=CONTEO, FILAS=FILAS, OBJETOS=OBJETOS, FKS=FKS, CHECKS=CHECKS, UQS=UQS, DFS=DFS,
             PRUEBAS=PRUEBAS, REGLAS=REGLAS, TABLAS=TABLAS, MATRIZ=MATRIZ, DOC=DOC,
             figura=figura, tabla=tabla, e=e, pagina=pagina)
a_pdf(guardar('DocumentoTecnico.html', doc_tecnico.documento(datos)), os.path.join(DOC, 'DocumentoTecnico.pdf'))
a_pdf(guardar('ManualUsuario.html', doc_tecnico.manual(datos)), os.path.join(DOC, 'ManualUsuario.pdf'))

print('Listo.')
