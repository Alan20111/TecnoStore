using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TecnoStore.Data
{
    /// <summary>
    /// Crea la base TecnoStore en el servidor elegido ejecutando los mismos
    /// scripts de 02_SQL (van incrustados en el .exe), en el orden del README.
    /// Igual que en SSMS, cada script se divide en lotes por las líneas "GO".
    /// </summary>
    public static class InstaladorBase
    {
        // 07_Seguridad va al final: da permisos sobre procedimientos ya creados
        private static readonly string[] Orden =
        {
            "01_CrearBase", "02_CrearTablas", "03_Alter_Constraints", "04_DatosDemo", "06_Vistas",
            "08_StoredProcedures", "09_Functions", "10_Triggers", "07_Seguridad",
        };

        public class Resultado
        {
            public bool SeguridadOk { get; set; } = true;
            public string AvisoSeguridad { get; set; }
        }

        /// <summary>
        /// ¿Existe la base TecnoStore en el servidor? Se consulta desde master,
        /// así que funciona aunque el usuario no tenga acceso a la base.
        /// </summary>
        public static bool Existe(string servidor)
        {
            var cs = new SqlConnectionStringBuilder
            {
                DataSource = servidor, InitialCatalog = "master", IntegratedSecurity = true, ConnectTimeout = 10,
            }.ConnectionString;
            using (var cn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT DB_ID(N'TecnoStore')", cn))
            {
                cn.Open();
                return cmd.ExecuteScalar() != DBNull.Value;
            }
        }

        /// <summary>
        /// Ejecuta los scripts. Solo debe llamarse si la base NO existe:
        /// 01_CrearBase.sql borra la base si la encuentra. Si falla 07_Seguridad (por ejemplo, el usuario de
        /// Windows no puede crear logins) la base queda lista igual en modo de
        /// conexión única y se informa el aviso.
        /// </summary>
        public static Resultado Instalar(string servidor, Action<string> progreso)
        {
            if (Existe(servidor))
                throw new InvalidOperationException("La base TecnoStore ya existe en " + servidor + "; no se volvió a crear para no borrar sus datos.");
            var resultado = new Resultado();
            var cs = new SqlConnectionStringBuilder
            {
                DataSource = servidor,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                ConnectTimeout = 10,
            }.ConnectionString;

            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                foreach (var nombre in Orden)
                {
                    progreso?.Invoke(nombre);
                    try
                    {
                        foreach (var lote in Lotes(Leer(nombre)))
                        {
                            using (var cmd = new SqlCommand(lote, cn) { CommandTimeout = 300 })
                                cmd.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex) when (nombre == "07_Seguridad")
                    {
                        resultado.SeguridadOk = false;
                        resultado.AvisoSeguridad = ex.Message;
                    }
                    catch (SqlException ex)
                    {
                        throw new InvalidOperationException("Falló el script " + nombre + ".sql: " + ex.Message, ex);
                    }
                }
            }
            SqlConnection.ClearAllPools();   // las conexiones viejas apuntaban a una base que no existía
            return resultado;
        }

        private static string Leer(string nombre)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream("Scripts." + nombre + ".sql"))
            {
                if (s == null) throw new InvalidOperationException("El .exe no trae el script " + nombre + ".sql.");
                using (var r = new StreamReader(s, System.Text.Encoding.UTF8, true))
                    return r.ReadToEnd();
            }
        }

        /// <summary>Divide un script en lotes por las líneas que solo dicen GO.</summary>
        public static IEnumerable<string> Lotes(string script)
        {
            foreach (var lote in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                if (lote.Trim().Length > 0) yield return lote;
        }
    }
}
