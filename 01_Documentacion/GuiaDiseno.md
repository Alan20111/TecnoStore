# Guía de diseño · TecnoStore

Cómo se ve y cómo se usa TecnoStore, y por qué. Aplica igual a la **app de Windows** (`03_WindowsForms/TecnoStore/Utils/`) y a la **vista previa web** (`preview/public/`).

Dos ideas mandan:

1. **Sencillo**: pocos módulos, botones grandes y una sola tarea por pantalla.
2. **Monocromático**: blanco, gris y negro; el rojo aparece solo en errores y en acciones que eliminan o cancelan.

---

## 1. Retroalimentación: qué estaba mal y qué se cambió

Se compararon las pantallas con dos programas propios que la gente usa a diario (un punto de venta de mostrador y el expediente de un consultorio). Lo que hacen bien: cada pestaña tiene un enfoque, los botones son grandes, la búsqueda ocurre sola, la ventana pide una cosa a la vez y lo que un rol no usa no aparece.

| Antes | Problema | Ahora |
|---|---|---|
| 15 módulos en el menú | Hay que leer mucho para encontrar algo | **7 módulos** (Inicio, Ventas, Taller, Clientes, Inventario, Reportes, Configuración), con F1 a F7. El Operador ve 5 |
| Se abría en un tablero de gráficas | Útil para el dueño, no para quien atiende | **Inicio** con 4 botones grandes según el rol (Nueva venta, Recibir equipo…) y números que llevan a su pantalla |
| Panel de captura siempre junto a la tabla | Dos tareas compitiendo; media pantalla en gris | La tabla ocupa todo; **doble clic abre una ficha** con campos grandes, Guardar y Cancelar |
| Nuevo / Editar / Guardar / Cancelar siempre visibles | Cuatro botones, casi todos apagados | **Nuevo** y **Abrir**; los demás viven en la ficha |
| Botón Buscar | Un paso de más | **Busca al escribir** |
| Venta: tres zonas con listas desplegables | Muchos clics por producto | Se **escribe o escanea, se toca y se cobra**. Forma de pago y descuento se piden al final, con botones |
| Taller: lista de estados posibles | La mayoría los rechazaba la base (solo se avanza un paso) | Un botón: **Pasar a En reparación**, **Entregar al cliente** |
| Números de serie siempre visibles en Compras | Confunden cuando no aplican | Aparecen **solo si el producto los lleva** |
| Mensaje "Cambios guardados" que hay que cerrar | Un clic sin valor | **Aviso** que se va solo |
| Botones de 28 px, letra de 9 pt | Difícil de leer y de atinar | Botones de **48 px o más**, letra de **11 a 13 pt**, filas de **46 px** |
| Servidor en el inicio de sesión | Casi nunca cambia | Se muestra solo al tocar **Servidor (cambiar)** o si falla la conexión |

---

## 2. Estilo

Basado en el lenguaje de **shadcn/ui** (familia de Vercel, Linear y Radix).

| Token | Valor | Uso |
|---|---|---|
| Lienzo | `#f5f5f5` | Fondo; campos y botones secundarios sobre blanco |
| Papel | `#ffffff` | Tarjetas, tablas y ventanas |
| Superficie | `#fafafa` | Barra lateral; texto sobre negro |
| Tinta | `#0a0a0a` | Texto y **acción principal** |
| Tinta suave | `#171717` | Módulo activo y avisos |
| Gris medio | `#737373` | Texto secundario; anillo de foco |
| Línea fina | `#e5e5e5` | Bordes de 1 px |
| Rojo | `#e7000b` | **Solo** errores, eliminar, cancelar y agotado |
| Radios | 24 tarjetas · 18 botones, campos y selectores | Nada con esquina recta |
| Sombra | `0 0 0 1px rgba(23,23,23,.05), 0 1px 3px rgba(0,0,0,.1), 0 1px 2px -1px rgba(0,0,0,.1)` | Solo tarjetas |
| Tipografía | Geist → Inter → Segoe UI | 400 texto · 500 botones · 600 títulos, con espaciado −0.025 a −0.05 em en lo grande (web) |

**Jerarquía de botones.** Negro: lo que se vino a hacer (uno por pantalla). Gris o blanco con línea fina: lo demás. Letra roja: eliminar o cancelar, siempre a la izquierda, lejos de Guardar.

**Lo que adaptamos a propósito.** El estilo original es compacto (botones de 36 px, letra de 14 px). Aquí se usan tamaños mayores porque es un programa de mostrador. El anillo de foco es gris medio y no línea fina, para que se distinga al usar el teclado. Los estados "todo bien" no llevan color: solo destaca lo que necesita atención.

**Se conserva del diseño anterior:** esquina continua (ξ = 0.6, `Forma.Continua`), barra de desplazamiento delgada, animaciones cortas (150 a 260 ms) que se apagan si Windows o el navegador piden reducir movimiento.

---

## 3. Menos información por pantalla y permisos por rol

Cada pantalla tenía demasiada información. Estos son los principios que aplicamos y de dónde salen.

### 3.1 Principios

| Principio | Fuente | Cómo lo aplicamos |
|---|---|---|
| **Divulgación progresiva**: mostrar primero lo que casi todos usan y dejar lo avanzado a un clic | [NN/g · Progressive Disclosure](https://www.nngroup.com/articles/progressive-disclosure/) | Buscador más 1 o 2 filtros; las fechas aparecen con el botón **Fechas** u **Otro**. El detalle y la edición se abren en una **ficha**, no al lado de la tabla |
| **Ley de Hick**: más opciones hacen más lenta la decisión | [Laws of UX · Hick](https://lawsofux.com/hicks-law/) | De 5 a 7 módulos en lugar de 15; **un botón principal** (negro) por pantalla |
| **Ley de Miller**: la memoria de trabajo es limitada | [Laws of UX · Miller](https://lawsofux.com/millers-law/) | Tablas de **4 a 6 columnas**; listas del Resumen de **5 filas** |
| **Estética minimalista** y **lenguaje del usuario** (heurísticas 2 y 8) | [NN/g · 10 heurísticas](https://www.nngroup.com/articles/ten-usability-heuristics/) | Sin nombres de procedimientos, tablas ni triggers en la interfaz: "Se registra sola cada vez que cambia un precio" en vez de "trg_Producto_Auditoria" |
| **Tablas para buscar y comparar**, no para leerlo todo | [NN/g · Data Tables](https://www.nngroup.com/articles/data-tables/) | Las columnas que identifican (folio, nombre) van primero; la cifra clave (total, stock) al final, alineada a la derecha |
| **Dashboard de un vistazo** en una sola pantalla, sin pastel ni dona | Stephen Few, *Information Dashboard Design* | Resumen con 4 indicadores, 2 gráficas de barras y 2 listas cortas. El **Inicio** es todavía más simple: botones y números |
| **Estados vacíos** que dicen qué hacer | [NN/g · Empty States](https://www.nngroup.com/articles/empty-state-interface-design/) | "No hay nada que mostrar" en el centro de la tabla |
| **Confirmar solo lo riesgoso** | [NN/g · Confirmation Dialogs](https://www.nngroup.com/articles/confirmation-dialog/) | Se confirma comprar, cancelar, eliminar y ajustar. En la venta, la ventana **Cobrar** es la confirmación. Lo que sí se hizo se dice con un aviso que se va solo, sin botón que cerrar |
| **Autorizar en el servidor y negar por defecto** | [OWASP · Authorization Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authorization_Cheat_Sheet.html) | La interfaz oculta, pero quien decide es SQL Server (ver 3.2) |

**Ocultar o deshabilitar.** [Nuestro] Si el rol **nunca** podrá usar algo, se oculta (menú, botón, panel o columna). Si es un bloqueo **temporal** (una orden ya entregada, una venta sin líneas), se deshabilita y se dice por qué.

### 3.2 Permisos que salen de la base de datos

La app no tiene una lista de roles escrita a mano. Al iniciar sesión pregunta a SQL Server qué puede hacer el usuario:

1. [`usp_PermisosApp`](../02_SQL/07_Seguridad.sql) recibe el usuario de la app y toma su rol (`UsuarioRol`).
2. Se pone en el lugar del usuario de base de datos de ese rol (`EXECUTE AS USER = 'tecno_operador'`, etc.) y pregunta con `HAS_PERMS_BY_NAME` si tiene cada permiso. Después hace `REVERT`.
3. Devuelve 15 capacidades (`ver_costos`, `registrar_venta`, `cancelar_venta`…) con 1 o 0.

Así, si alguien cambia un `GRANT` o un `DENY` en `07_Seguridad.sql`, la interfaz cambia sola. Las pruebas P33 y P34 de `12_Pruebas.sql` comprueban la matriz del Operador y del Supervisor.

| Qué se oculta | Capacidad | Admin | Supervisor | Operador | Consulta |
|---|---|:-:|:-:|:-:|:-:|
| Módulo Reportes (Resumen y Reportes) | `ver_reportes` | ✓ | ✓ | — | ✓ |
| Costos, márgenes, utilidad y actividad por empleado | `ver_costos` | ✓ | ✓ | — | ✓ |
| Nueva venta | `registrar_venta` | ✓ | ✓ | ✓ | — |
| Botón Cancelar venta | `cancelar_venta` | ✓ | ✓ | — | — |
| Recibir equipo y avanzar órdenes | `gestionar_ordenes` | ✓ | ✓ | ✓ | — |
| Compras | `registrar_compra` | ✓ | ✓ | — | — |
| Ajustes (inventario / precios) | `ajustar_stock` / `ajustar_precio` | ✓ | ✓ | — | — |
| Editar productos | `guardar_producto` | ✓ | ✓ | — | — |
| Eliminar productos; módulo Configuración (categorías, marcas, servicios) | `eliminar_producto` / `editar_catalogos` | ✓ | — | — | — |
| Editar clientes / proveedores | `guardar_cliente` / `guardar_proveedor` | ✓ | ✓ | ✓ / — | — |
| Proveedores (muestra costos) | `guardar_proveedor` o `ver_costos` | ✓ | ✓ | — | ✓ |
| Auditoría | `ver_auditoria` | ✓ | ✓ | — | — |
| Usuarios | `admin_usuarios` | ✓ | — | — | — |

Ocultar no basta. Si alguien llama la operación de todos modos, SQL Server la rechaza por el `DENY` o la falta de `GRANT`, y el servidor de la vista previa valida la misma capacidad antes de ejecutarla. Sin `ver_costos`, los costos **no salen del servidor**: no basta con esconder la columna.

**En el código:** `AuthRepository.CargarPermisos` y `UsuarioSesion.Puede()` (WinForms; si la base aún no tiene el procedimiento, usa una copia de la misma matriz), `Ui.Mostrar(grid, columnas…)` para elegir columnas, `FrmPrincipal` para módulos y vistas, `modulosDe()`, `exige()` y `sinCostos()` en `preview/server.js`, y `tiene()` en `preview/public/app.js`.

---

## 4. Dónde está cada cosa en el código

| Pieza | Windows Forms | Vista previa |
|---|---|---|
| Colores, letras y tamaños | `Utils/Tema.cs` | `:root` en `style.css` |
| Módulos y vistas por rol | `Forms/FrmPrincipal.cs` | `MODULOS` en `app.js`, `MODULO_REQUIERE` en `server.js` |
| Inicio con botones grandes | `Forms/FrmInicio.cs`, `BotonGrande` | `renderInicio()` |
| Vender y cobrar | `Forms/FrmVenta.cs`, `Forms/FrmCobrar.cs` | `renderVenta()`, `cobrar()` |
| Ficha (una tarea, botones grandes) | `Ficha` en `Utils/Sencillo.cs` | `ficha()` |
| Campos con caja redondeada | `PanelCampos`, `Caja` | `campos()` |
| Selector de vistas y filtros | `Segmentos` | `seg()` |
| Lista con buscador | `Sencillo.Barra`, `BuscarAlEscribir`, `Abrir` | `lista()` |
| Aviso que se va solo | `Ui.Aviso` | `aviso()` |
| Menú lateral | `Utils/MenuLateral.cs` | `.lateral` |

Los formularios siguen abriéndose en el diseñador de Visual Studio: los controles y eventos están en su `.Designer.cs` y `Sencillo` los acomoda al ejecutar.

## 5. Limitaciones honestas

- La app se probó corriendo en **Mono sobre Linux** (pantallas, guardar, cobrar y avanzar órdenes, con los logins reales de SQL Server). **No se ha probado en Windows**; ahí cambian la letra y los íconos. Las gráficas del Resumen no se pudieron probar en Mono.
- La letra Geist no viaja con el programa: se usa si la PC la tiene instalada; si no, Segoe UI.
- Los mensajes de confirmación de la app usan el cuadro estándar de Windows (el proyecto pide `MessageBox` en operaciones críticas).
