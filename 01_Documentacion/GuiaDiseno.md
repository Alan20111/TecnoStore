# Guía de diseño · TecnoStore

Sistema visual del panel de administración. Combina tres referencias y documenta cómo se implementó en la **vista previa web** (`preview/public/style.css`, `app.js`) y en la **app de Windows Forms** (`03_WindowsForms/TecnoStore/Utils/`).

Las etiquetas indican el origen de cada dato:

- **[Apple]**: lo publica Apple (Human Interface Guidelines, sesiones de WWDC o documentación de API).
- **[Comunidad]**: es una aproximación de terceros con fuente.
- **[Nuestro]**: es una decisión o calibración propia. Apple no publica radios de desenfoque ni opacidades exactas, así que todos esos valores son de esta categoría.

---

## 1. De dónde sale cada parte

| Referencia | Qué aporta | Regla que respetamos |
|---|---|---|
| **Perplexity** | Lienzo pergamino cálido `#faf8f5`, tarjetas `#fdfbfa`, tinta `#27251e`, grafito `#72706b`, línea de lápiz `#d1d1cd`, tarjetas de radio 16 | El verde azulado `#016a71` es el **único color de navegación y selección**: pastilla del menú, filas seleccionadas, anillo de foco e interruptores |
| **Brex** | Tipografía compacta con tracking negativo, radio 12 en botones y campos, barra oscura `#15191e` (la barra de título) | El naranja `#ff5900` es el **único color de la acción principal** (Guardar, Buscar, Ingresar…), una por zona |
| **Apple** | Materiales de vidrio, esquinas continuas, animaciones con resorte, presión visible, scrollbars delgados superpuestos, controles segmentados, interruptores y alertas tipo hoja | El vidrio solo va en la capa de navegación; en el contenido nunca |

Los dos acentos no compiten porque cada uno tiene un solo papel: el verde azulado indica *dónde estás* y el naranja indica *qué hacer*. Las gráficas usan escalas de esos mismos tonos, nunca un arcoíris.

---

## 2. Investigación: cómo diseña Apple

### 2.1 Liquid Glass (WWDC 2025, iOS/macOS 26)

- Material que **curva y concentra la luz** (refracción y reflejos especulares) y se adapta al contenido que tiene detrás. Los controles forman una capa que flota sobre la app. [Apple] ([Newsroom](https://www.apple.com/newsroom/2025/06/apple-introduces-a-delightful-and-elegant-new-software-design/), [WWDC25-219](https://developer.apple.com/videos/play/wwdc2025/219/))
- **Va en la capa de navegación y controles** (barras, sidebars, toolbars, popovers), **no en el contenido**. No se apila vidrio sobre vidrio. [Apple] ([HIG Materials](https://developer.apple.com/design/human-interface-guidelines/materials))
- Hay dos variantes. *Regular* sirve para casi todo. *Clear* solo va sobre fotos o video y lleva una atenuación del 35 %. [Apple]
- **Accesibilidad**: con *Reducir transparencia* se esmerila más; con *Aumentar contraste* pasa a blanco o negro con borde marcado. [Apple]
- **Concentricidad**: radio interior = radio exterior − padding. Las cápsulas tienen radio = alto / 2. [Apple] ([WWDC25-356](https://developer.apple.com/videos/play/wwdc2025/356/))

**Cómo lo aplicamos:**
- **Vista previa:** la barra lateral, el encabezado y las alertas usan `backdrop-filter: blur(20px) saturate(180%)` sobre papel al 62 %, con un brillo interior de 1 px. Las tablas y tarjetas de contenido son opacas. Se respetan `prefers-reduced-transparency` y `prefers-contrast`.
- **Material opaco estilo iOS 16:** el selector **Vidrio líquido / Opaco** del encabezado cambia todo a materiales sólidos con las mismas formas. Se recuerda por navegador.
- **Windows Forms:** GDI+ no puede desenfocar lo que hay detrás de un control, así que la app usa el estilo **opaco**. En Windows 11 sí aprovecha el sistema: ventana con esquinas redondeadas y barra de título del color de la marca (`DwmSetWindowAttribute`, en `Utils/Forma.cs`).

### 2.2 Materiales anteriores (iOS 15/16)

Hay cuatro grosores: ultraThin, thin, regular y thick. Más grueso significa más opaco y con mejor contraste. El material se elige por su función, no por el color. [Apple] ([HIG Materials](https://developer.apple.com/design/human-interface-guidelines/materials)) Apple no publica radios de desenfoque, así que nuestros valores son calibración propia [Nuestro].

### 2.3 Esquinas continuas ("squircle")

- Desde iOS 7 Apple usa **curvatura continua**. Un `border-radius` circular tiene un salto donde la recta se vuelve arco; la esquina continua lo elimina. [Comunidad] ([Figma](https://www.figma.com/blog/desperately-seeking-squircles/))
- La API oficial es `RoundedCornerStyle.continuous` y `CALayerCornerCurve.continuous`. [Apple]
- Figma la modela con un suavizado **ξ = 0.6, que coincide con iOS**. Cada esquina consume `(1+ξ)·r` de cada lado y es: curva de transición → arco de `90·(1−ξ)°` → curva de transición. [Comunidad] ([figma-squircle](https://github.com/phamfoo/figma-squircle/blob/main/src/draw.ts))

**Cómo lo aplicamos:**
- **Vista previa:** se usa `corner-shape: squircle` donde el navegador lo soporta (Chrome/Edge 139+), con radios ~1.3× mayores. Los demás navegadores usan `border-radius`.
- **Windows Forms:** `Forma.Continua()` implementa la geometría exacta con ξ = 0.6: tres curvas Bézier por esquina, la central aproximando el arco. Está **verificada numéricamente**: los extremos del arco caen sobre el círculo de la esquina y las tangentes son continuas. La usan botones, tarjetas, la pastilla del menú y las barras de desplazamiento.

### 2.4 Tipografía

- SF Pro ajusta su espaciado (tracking) al tamaño: −0.006 em a 13 pt, −0.026 em a 17 pt, −0.023 em a 20 pt. Los pesos recomendados van de Regular a Bold; Ultralight, Thin y Light no se usan. [Apple] ([HIG Typography](https://developer.apple.com/design/human-interface-guidelines/typography))
- En la web, `-apple-system` da San Francisco en Mac. [Apple] ([WebKit](https://webkit.org/blog/3709/using-the-system-font-in-web-content/))
- En Windows, el equivalente es **Segoe UI Variable**, con tamaño óptico automático. [Comunidad] ([Microsoft Learn](https://learn.microsoft.com/en-us/windows/apps/design/signature-experiences/typography))

**Cómo lo aplicamos:**
- Pila de fuentes `-apple-system, "SF Pro Text", "Segoe UI Variable Text", "Segoe UI", Inter`.
- Tracking −0.01 em en el texto (Brex) y −0.022 a −0.025 em en títulos grandes.
- Pesos 400 y 500 en casi todo (Perplexity); 600 solo en títulos y números grandes.
- En WinForms se detecta si existe Segoe UI Variable (Windows 11) y, si no, se usa Segoe UI.

### 2.5 Movimiento

- Las animaciones por defecto son *springs* definidas por duración perceptual y rebote. El valor por defecto de SwiftUI es **0.5 s sin rebote**; 0.15 es un rebote ligero y por encima de 0.4 se siente exagerado. Las animaciones deben ser breves, interrumpibles y escasas en acciones frecuentes. [Apple] ([WWDC23 Animate with springs](https://developer.apple.com/videos/play/wwdc2023/10158/), [HIG Motion](https://developer.apple.com/design/human-interface-guidelines/motion))
- Con *Reducir movimiento*: fundidos en lugar de desplazamientos y nada de animar desenfoque. [Apple]

**Cómo lo aplicamos:**

| Animación | Vista previa | Windows Forms |
|---|---|---|
| Curvas | Springs convertidos a CSS `linear()`: sin rebote (`--spring`) para casi todo y rebote 0.15 (`--resorte`) para objetos "físicos" | `Animacion.Salida` (sin rebote) y `Animacion.Resorte` (rebote ~1.5 %), con un temporizador a ~60 fps |
| Pastilla del menú | Se desliza hasta el módulo activo | Igual, en `MenuLateral.Activar` |
| Cambio de módulo | Entra con fundido y 8 px hacia arriba, con escalonado | Entra deslizándose 14 px |
| Dashboard | Los números cuentan hasta su valor; las columnas crecen desde la base; la dona gira al aparecer | Números que cuentan y columnas que crecen |
| Botones | `scale(.97)` al presionar | Se "hunde" 1 px y se oscurece; el color cambia gradualmente al pasar el cursor |
| Alertas | Aparecen como hoja (escala 1.08 → 1 con resorte) sobre un fondo atenuado | Cuadros de mensaje de Windows |
| Errores | El campo inválido se sacude | Fondo rojo claro y marca de error |
| Ventanas | — | Aparecen con fundido |
| Reducir movimiento | `prefers-reduced-motion` desactiva todo | Si Windows tiene los efectos visuales apagados, todo salta al valor final |

### 2.6 Indicadores de desplazamiento

- El indicador es translúcido y suele aparecer al empezar a desplazarse. En macOS el estilo "overlay" depende de la preferencia del usuario. [Apple] ([HIG Scroll views](https://developer.apple.com/design/human-interface-guidelines/scroll-views))
- En la web: si se declara `scrollbar-width`, Chrome **ignora** `::-webkit-scrollbar`. [Comunidad] ([MDN](https://developer.mozilla.org/en-US/docs/Web/CSS/scrollbar-width))

**Cómo lo aplicamos:**
- **Vista previa:** en Chrome y Safari, el thumb es una cápsula translúcida que solo aparece cuando el cursor está sobre el área desplazable. En Firefox se usa `scrollbar-width: thin`.
- **Windows Forms:**
  - `BarraDelgada` es una cápsula de 6 px superpuesta a la tabla, sin ocupar ancho. Crece a 9 px bajo el cursor, se puede arrastrar y se oculta tras 1.2 s.
  - La rueda desplaza 3 filas y Shift + rueda desplaza en horizontal.
  - Si una tabla no permite desplazarse por código, la barra se retira sola y vuelve la nativa.
  - El menú lateral tiene su propio indicador delgado.

### 2.7 Controles

- **Control segmentado** (pestañas de Reportes y selector de material): un "pulgar" de papel se desliza bajo el segmento activo. [Apple] ([HIG Segmented controls](https://developer.apple.com/design/human-interface-guidelines/segmented-controls))
- **Interruptores**: en la vista previa, las casillas son interruptores estilo iOS en verde azulado. El estado no depende solo del color, también de la posición. [Apple] ([HIG Toggles](https://developer.apple.com/design/human-interface-guidelines/toggles))
- **Barra lateral** con selección en pastilla e íconos de línea: SVG en la web y Segoe Fluent Icons o MDL2 Assets en Windows. [Apple] ([HIG Sidebars](https://developer.apple.com/design/human-interface-guidelines/sidebars))
- **Alertas**: la acción más probable va a la derecha y es la que recibe el foco. [Apple] ([HIG Alerts](https://developer.apple.com/design/human-interface-guidelines/alerts))
- **Foco visible**: anillo verde azulado de 3–4 px solo al navegar con teclado. Botones de al menos 32–36 px de alto. [Apple] ([HIG Accessibility](https://developer.apple.com/design/human-interface-guidelines/accessibility))

---

## 3. Tokens

| Token | Valor | Uso |
|---|---|---|
| Pergamino | `#faf8f5` | Lienzo de toda la app |
| Papel | `#fdfbfa` | Tarjetas, tablas y campos |
| Lateral | `#f3f0ea` | Barra lateral |
| Línea de lápiz | `#d1d1cd` / suave `#e7e4de` | Bordes de 1 px |
| Tinta / Grafito / Ceniza | `#27251e` / `#72706b` / `#92918b` | Texto principal / secundario / ayuda |
| Verde azulado | `#016a71` | Solo navegación y selección |
| Brasa | `#ff5900` | Solo la acción principal |
| Carbón | `#15191e` | Barra de título |
| Radios | tarjeta 16 · campo 12 · botón 12 · menú 10 · etiqueta 6 · cápsula 9999 | Esquina continua con ξ = 0.6 |
| Sombra | `0 1px 2px rgba(0,0,0,.08)` | La única de las superficies; las flotantes (alertas) llevan una mayor |
| Vidrio | `blur(20px) saturate(180%)`, papel al 62 %, brillo `inset 0 1px 0 rgba(255,255,255,.55)` | Barra lateral, encabezado y alertas |
| Spring | 0.5 s perceptual → CSS 0.68 s `linear(…)` | Movimiento por defecto |
| Hover / presión | 0.18 s · `scale(.97)` | Interacción |
| Scrollbar | 6 px (9–10 px al pasar el cursor), cápsula, tinta al 33–50 % | Tablas y paneles |

---

## 4. Menos información por pantalla y permisos por rol

Cada pantalla tenía demasiada información. Estos son los principios que aplicamos y de dónde salen.

### 4.1 Principios

| Principio | Fuente | Cómo lo aplicamos |
|---|---|---|
| **Divulgación progresiva**: mostrar primero lo que casi todos usan y dejar lo avanzado a un clic | [NN/g · Progressive Disclosure](https://www.nngroup.com/articles/progressive-disclosure/) | Búsqueda más 2 o 3 filtros; el resto (fechas del historial) va en **Más filtros**. El detalle de un registro aparece al seleccionarlo, no en la tabla |
| **Ley de Hick**: más opciones hacen más lenta la decisión | [Laws of UX · Hick](https://lawsofux.com/hicks-law/) | Menú lateral solo con los módulos del rol; **un botón principal** (naranja) por pantalla |
| **Ley de Miller**: la memoria de trabajo es limitada | [Laws of UX · Miller](https://lawsofux.com/millers-law/) | Tablas de **5 a 7 columnas**; listas del Dashboard de **5 filas** |
| **Estética minimalista** y **lenguaje del usuario** (heurísticas 2 y 8) | [NN/g · 10 heurísticas](https://www.nngroup.com/articles/ten-usability-heuristics/) | Sin nombres de procedimientos, tablas ni triggers en la interfaz: "Se registra sola cada vez que cambia un precio" en vez de "trg_Producto_Auditoria" |
| **Tablas para buscar y comparar**, no para leerlo todo | [NN/g · Data Tables](https://www.nngroup.com/articles/data-tables/) | Las columnas que identifican (folio, nombre) van primero; la cifra clave (total, stock) al final, alineada a la derecha |
| **Dashboard de un vistazo** en una sola pantalla, sin pastel ni dona | Stephen Few, *Information Dashboard Design* | 4 indicadores, 2 gráficas de barras (categorías en barras horizontales) y 2 listas de "Requiere atención" |
| **Estados vacíos** que dicen qué hacer | [NN/g · Empty States](https://www.nngroup.com/articles/empty-state-interface-design/) | "No hay resultados. Prueba con otra búsqueda o quita filtros." |
| **Confirmar solo lo riesgoso** | [NN/g · Confirmation Dialogs](https://www.nngroup.com/articles/confirmation-dialog/) | Se confirma guardar una venta o compra, cancelar y ajustar; buscar o abrir nunca pide confirmación |
| **Autorizar en el servidor y negar por defecto** | [OWASP · Authorization Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authorization_Cheat_Sheet.html) | La interfaz oculta, pero quien decide es SQL Server (ver 4.2) |

**Ocultar o deshabilitar.** [Nuestro] Si el rol **nunca** podrá usar algo, se oculta (menú, botón, panel o columna). Si es un bloqueo **temporal** (una orden ya entregada, una venta sin líneas), se deshabilita y se dice por qué.

### 4.2 Permisos que salen de la base de datos

La app no tiene una lista de roles escrita a mano. Al iniciar sesión pregunta a SQL Server qué puede hacer el usuario:

1. [`usp_PermisosApp`](../02_SQL/07_Seguridad.sql) recibe el usuario de la app y toma su rol (`UsuarioRol`).
2. Se pone en el lugar del usuario de base de datos de ese rol (`EXECUTE AS USER = 'tecno_operador'`, etc.) y pregunta con `HAS_PERMS_BY_NAME` si tiene cada permiso. Después hace `REVERT`.
3. Devuelve 15 capacidades (`ver_costos`, `registrar_venta`, `cancelar_venta`…) con 1 o 0.

Así, si alguien cambia un `GRANT` o un `DENY` en `07_Seguridad.sql`, la interfaz cambia sola. Las pruebas P33 y P34 de `12_Pruebas.sql` comprueban la matriz del Operador y del Supervisor.

| Qué se oculta | Capacidad | Admin | Supervisor | Operador | Consulta |
|---|---|:-:|:-:|:-:|:-:|
| Dashboard y Reportes | `ver_reportes` | ✓ | ✓ | — | ✓ |
| Costos, márgenes, utilidad y actividad por empleado | `ver_costos` | ✓ | ✓ | — | ✓ |
| Nueva venta | `registrar_venta` | ✓ | ✓ | ✓ | — |
| Botón Cancelar venta | `cancelar_venta` | ✓ | ✓ | — | — |
| Panel para cambiar estado de órdenes | `gestionar_ordenes` | ✓ | ✓ | ✓ | — |
| Compras | `registrar_compra` | ✓ | ✓ | — | — |
| Ajustes (inventario / precios) | `ajustar_stock` / `ajustar_precio` | ✓ | ✓ | — | — |
| Editar productos | `guardar_producto` | ✓ | ✓ | — | — |
| Eliminar productos y editar catálogos | `eliminar_producto` / `editar_catalogos` | ✓ | — | — | — |
| Editar clientes / proveedores | `guardar_cliente` / `guardar_proveedor` | ✓ | ✓ | ✓ / — | — |
| Auditoría | `ver_auditoria` | ✓ | ✓ | — | — |
| Usuarios | `admin_usuarios` | ✓ | — | — | — |

Ocultar no basta. Si alguien llama la operación de todos modos, SQL Server la rechaza por el `DENY` o la falta de `GRANT`, y el servidor de la vista previa valida la misma capacidad antes de ejecutarla. Sin `ver_costos`, los costos **no salen del servidor**: no basta con esconder la columna.

**En el código:** `AuthRepository.CargarPermisos` y `UsuarioSesion.Puede()` (WinForms; si la base aún no tiene el procedimiento, usa una copia de la misma matriz), `Ui.Mostrar(grid, columnas…)` para elegir columnas, `modulosDe()`, `exige()` y `sinCostos()` en `preview/server.js`, y `tiene()` en `preview/public/app.js`.

---

## 5. Dónde está cada cosa en el código

| Pieza | Vista previa | Windows Forms |
|---|---|---|
| Colores, tipografía, radios | `:root` en `style.css` | `Utils/Tema.cs` |
| Vidrio y material opaco | `--vidrio-*`, `[data-material="opaco"]` | Opaco; `Forma.VentanaModerna` en Windows 11 |
| Esquina continua | `corner-shape: squircle` | `Forma.Continua()` |
| Animaciones | `--spring`, `--resorte`, `@keyframes`, `contar()` | `Utils/Animacion.cs` |
| Barra lateral con pastilla | `.lateral`, `.indicador`, `moverIndicador()` | `Utils/MenuLateral.cs` |
| Botones | `button`, `button.primary` | `Tema.Boton` (dibujado a mano) |
| Scrollbars delgados | `::-webkit-scrollbar` | `Utils/BarraDelgada.cs` |
| Tarjetas | `.card` | `Utils/Tarjeta.cs` |
| Control segmentado | `.segmentado`, `.tabs`, `segmentado()` | — (las pestañas de Reportes son `TabControl` nativo) |

---

## 6. Limitaciones honestas

- **Windows Forms no tiene desenfoque de fondo.** El vidrio líquido solo existe en la vista previa. La app de Windows usa la versión opaca del mismo sistema, que equivale al estilo iOS 16.
- `corner-shape: squircle` solo funciona en Chrome/Edge 139+; en otros navegadores se ven esquinas circulares.
- Lo dibujado a mano en WinForms (botones, menú, barras, tarjetas) compila sin errores pero **no se ha probado en una PC con Windows**. Si algo se ve mal, la barra delgada se puede desactivar quitando `BarraDelgada.Instalar(g)` en `Tema.Tabla`, y los botones volviendo a `FlatStyle.Flat` sin el evento `Paint`.
