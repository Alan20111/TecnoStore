#!/usr/bin/env bash
# Crea la base TecnoStore desde cero en el contenedor de Docker (Mac).
# El orden es el mismo que se usa en SSMS (ver README.md).
set -e
cd "$(dirname "$0")/.."
for f in 01_CrearBase 02_CrearTablas 03_Alter_Constraints 04_DatosDemo 06_Vistas \
         08_StoredProcedures 09_Functions 10_Triggers 07_Seguridad; do
  echo "== $f"
  scripts/sql.sh -i "/scripts/$f.sql" | grep -v -E "Changed database|rolled back" || true
done
echo "Base TecnoStore lista."
