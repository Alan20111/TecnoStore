/* =============================================================
   TecnoStore · 01_CrearBase.sql
   Crea la base desde cero. Ejecutar primero.
   ============================================================= */
USE master;
GO

IF DB_ID(N'TecnoStore') IS NOT NULL
BEGIN
    ALTER DATABASE TecnoStore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TecnoStore;
END
GO

CREATE DATABASE TecnoStore COLLATE Modern_Spanish_CI_AS;
GO

USE TecnoStore;
GO
