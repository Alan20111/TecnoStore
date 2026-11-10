using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TecnoStore.Models;

namespace TecnoStore.Data
{
    /// <summary>
    /// Punto único de acceso a SQL Server: centraliza la cadena de conexión,
    /// abre conexiones con using y siempre usa parámetros (nunca concatena
    /// texto del usuario dentro del SQL).
    /// </summary>
    public static class Db
    {
        /// <summary>Usuario que inició sesión (null antes del login).</summary>
        public static UsuarioSesion Sesion { get; set; }

        private static bool ModoRoles =>
            string.Equals(ConfigurationManager.AppSettings["ModoConexion"], "Roles", StringComparison.OrdinalIgnoreCase);

        private static string CadenaConexion()
        {
            string nombre;
            if (!ModoRoles) nombre = "Unica";
            else nombre = Sesion == null ? "Login" : Sesion.Rol;

            var cs = ConfigurationManager.ConnectionStrings[nombre];
            if (cs == null)
                throw new InvalidOperationException("Falta la cadena de conexión '" + nombre + "' en App.config.");
            return cs.ConnectionString;
        }

        /// <summary>
        /// Abre una conexión y le dice a SQL Server qué usuario de la app está
        /// trabajando (SESSION_CONTEXT). Los triggers de auditoría y el
        /// historial de órdenes lo leen de ahí.
        /// </summary>
        public static SqlConnection Abrir()
        {
            var cn = new SqlConnection(CadenaConexion());
            cn.Open();
            if (Sesion != null)
            {
                using (var cmd = new SqlCommand("sp_set_session_context", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@key", "id_usuario");
                    cmd.Parameters.AddWithValue("@value", Sesion.IdUsuario);
                    cmd.ExecuteNonQuery();
                }
            }
            return cn;
        }

        /// <summary>Consulta SQL parametrizada (vistas, funciones, SELECT).</summary>
        public static DataTable Consulta(string sql, params SqlParameter[] parametros)
        {
            using (var cn = Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                var tabla = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(tabla);
                return tabla;
            }
        }

        /// <summary>Ejecuta un procedimiento que devuelve un resultado.</summary>
        public static DataTable ProcedimientoTabla(string nombre, params SqlParameter[] parametros)
        {
            using (var cn = Abrir())
            using (var cmd = new SqlCommand(nombre, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parametros);
                var tabla = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(tabla);
                return tabla;
            }
        }

        /// <summary>Ejecuta un procedimiento sin resultado; los OUTPUT quedan en los parámetros.</summary>
        public static void Procedimiento(string nombre, params SqlParameter[] parametros)
        {
            using (var cn = Abrir())
            using (var cmd = new SqlCommand(nombre, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parametros);
                cmd.ExecuteNonQuery();
            }
        }

        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            using (var cn = Abrir())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                return cmd.ExecuteScalar();
            }
        }

        // ----- Atajos para crear parámetros con tipo -----
        public static SqlParameter P(string nombre, SqlDbType tipo, object valor)
        {
            return new SqlParameter(nombre, tipo) { Value = valor ?? DBNull.Value };
        }

        public static SqlParameter P(string nombre, SqlDbType tipo, int tamano, object valor)
        {
            return new SqlParameter(nombre, tipo, tamano) { Value = valor ?? DBNull.Value };
        }

        public static SqlParameter Salida(string nombre, SqlDbType tipo, int tamano = 0)
        {
            var p = tamano > 0 ? new SqlParameter(nombre, tipo, tamano) : new SqlParameter(nombre, tipo);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        public static SqlParameter Decimal(string nombre, decimal valor, byte precision = 12, byte escala = 2)
        {
            return new SqlParameter(nombre, SqlDbType.Decimal) { Precision = precision, Scale = escala, Value = valor };
        }
    }
}
