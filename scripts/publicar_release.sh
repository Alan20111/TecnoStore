#!/usr/bin/env bash
# Compila la app de Windows Forms en Release y la publica como un release
# nuevo en GitHub (v1.0 -> v1.1 -> v1.2 ...), con el .exe y un zip que
# incluye el .exe.config y LEEME.txt.
#
#   scripts/publicar_release.sh "Qué cambió"         (siguiente versión)
#   scripts/publicar_release.sh "Qué cambió" v2.0    (versión explícita)
#
# Requiere: el commit ya subido a GitHub (push), gh con sesión iniciada y el
# SDK de .NET (en Mac: ~/.dotnet/dotnet, compila con EnableWindowsTargeting).
set -euo pipefail
cd "$(dirname "$0")/.."

REPO="Alan20111/TecnoStore"
CAMBIOS="${1:?Indica qué cambió, por ejemplo: scripts/publicar_release.sh \"Nueva pantalla de X\"}"
DOTNET="${DOTNET:-$(command -v dotnet || echo "$HOME/.dotnet/dotnet")}"

if [ -n "$(git status --porcelain)" ]; then
  echo "Hay cambios sin commit; haz commit y push antes de publicar." >&2; exit 1
fi
git fetch -q origin
if [ "$(git rev-parse HEAD)" != "$(git rev-parse origin/main)" ]; then
  echo "El commit actual no está en origin/main; haz push antes de publicar." >&2; exit 1
fi

# Versión: la indicada, o la última + 0.1
if [ -n "${2:-}" ]; then
  VERSION="$2"
else
  ULTIMA=$(gh release list -R "$REPO" --limit 1 --json tagName --jq '.[0].tagName' 2>/dev/null || true)
  ULTIMA="${ULTIMA:-v0.9}"
  MAYOR=$(echo "${ULTIMA#v}" | cut -d. -f1); MENOR=$(echo "${ULTIMA#v}" | cut -d. -f2)
  VERSION="v${MAYOR}.$((MENOR + 1))"
fi

echo "== Compilando $VERSION"
rm -rf 03_WindowsForms/TecnoStore/bin/Release
"$DOTNET" build 03_WindowsForms/TecnoStore.sln -c Release -p:EnableWindowsTargeting=true -nologo -v q

SALIDA="$(mktemp -d)"
mkdir -p "$SALIDA/TecnoStore"
cp 03_WindowsForms/TecnoStore/bin/Release/net48/TecnoStore.exe \
   03_WindowsForms/TecnoStore/bin/Release/net48/TecnoStore.exe.config "$SALIDA/TecnoStore/"
cat > "$SALIDA/TecnoStore/LEEME.txt" <<EOF
TecnoStore $VERSION - Sistema Integral de Gestion (Windows Forms, .NET Framework 4.8)

Requisitos
  - Windows 10 u 11 (trae .NET Framework 4.8).
  - SQL Server con la base TecnoStore creada: ejecutar en SSMS los scripts de
    la carpeta 02_SQL del repositorio, en el orden que indica el README.

Servidor
  - En la pantalla de inicio de sesion escriban su servidor de SQL Server:
    localhost, .\SQLEXPRESS o NOMBRE-PC\INSTANCIA. Se recuerda para la
    proxima vez. Usa autenticacion de Windows.
  - TecnoStore.exe.config es opcional: sirve para cambiar el servidor por
    defecto o usar un login de SQL Server por rol (ModoConexion=Roles).

Usuarios demo (contrasena 1234)
  admin (Administrador), csupervisor (Supervisor),
  loperador (Operador), fconsulta (Consulta)

Codigo fuente y documentacion: https://github.com/$REPO
EOF
ZIP="TecnoStore-$VERSION-win.zip"
(cd "$SALIDA" && zip -qr "$ZIP" TecnoStore && cp TecnoStore/TecnoStore.exe .)

cat > "$SALIDA/notas.md" <<EOF
## Cambios

$CAMBIOS

## Descarga

- **$ZIP** (recomendado): \`TecnoStore.exe\`, su configuración \`TecnoStore.exe.config\` y \`LEEME.txt\`.
- **TecnoStore.exe**: solo el ejecutable; funciona sin el \`.config\`.

## Antes de abrirlo

1. Crear la base en SQL Server con los scripts de \`02_SQL\` en el orden del [README](https://github.com/$REPO#en-windows-paso-a-paso).
2. En la pantalla de inicio de sesión escribir su servidor (\`localhost\`, \`.\\SQLEXPRESS\` o \`NOMBRE-PC\\INSTANCIA\`).
3. Entrar con \`admin\` y contraseña \`1234\` (también \`csupervisor\`, \`loperador\`, \`fconsulta\`).

> Compilado con el SDK de .NET desde macOS (commit $(git rev-parse --short HEAD)).
EOF

echo "== Publicando $VERSION"
gh release create "$VERSION" "$SALIDA/$ZIP" "$SALIDA/TecnoStore.exe" -R "$REPO" \
  --target "$(git rev-parse HEAD)" --title "TecnoStore $VERSION" --notes-file "$SALIDA/notas.md"
rm -rf "$SALIDA"
