#!/usr/bin/env bash
# Ejecuta sqlcmd dentro del contenedor de SQL Server.
#   scripts/sql.sh -i /scripts/01_CrearBase.sql      (archivo de 02_SQL)
#   scripts/sql.sh -d TecnoStore -Q "SELECT ..."     (consulta suelta)
exec docker exec tecnostore-sql /opt/mssql-tools18/bin/sqlcmd \
  -C -S localhost -U sa -P 'TecnoStore#2026' -b -f 65001 "$@"
