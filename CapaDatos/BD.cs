using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace CapaDatos
{
    public class BD
    {
        static readonly string connstring = @"server=127.0.0.1;uid=root;pwd=*****;database=requerimientos";
        static readonly MySqlConnection conn = new MySqlConnection(connstring);
        private void Connect() {
            conn.Open();
        }

        private void Close() {
            conn.Close();
        }

        private string PasswordHash(string password)
        {
           return BitConverter.ToString(
               SHA1CryptoServiceProvider.Create().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(password))
               ).Replace("-", "").ToLower();
        }

        public int Login(String user, String password)
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM usuario WHERE UsuarioNombre=@user AND NOT UsuarioCuentaBloqueada";
            cmd.Parameters.AddWithValue("@user", user);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                var idUsuario = reader.GetInt16("idUsuario");
                var hashPwd = reader.GetString("UsuarioPassword");
                var intentos = reader.GetInt16("UsuarioIntentosLoginFallido") + 1;
                reader.Close();
                
                if (hashPwd == PasswordHash(password))
                {
                    //Actualizo la cantidad de intentos fallidos a cero
                    cmd.CommandText = "UPDATE usuario SET UsuarioIntentosLoginFallido = 0 WHERE idUsuario = @idUsuario";
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.ExecuteNonQuery();
                    Close();
                    return idUsuario;
                } else {
                    //Password incorrecto, actualizo la cantidad de intentos
                    var updateIntentosCmd = conn.CreateCommand();
                    updateIntentosCmd.CommandText = "UPDATE usuario SET UsuarioIntentosLoginFallido = @intentos WHERE idUsuario = @idUsuario";
                    updateIntentosCmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    updateIntentosCmd.Parameters.AddWithValue("@intentos", intentos);
                    updateIntentosCmd.ExecuteNonQuery();

                    if (intentos >= 3)
                    {
                        //Bloqueo la cuenta
                        var blockUserCmd = conn.CreateCommand();
                        blockUserCmd.CommandText = "UPDATE usuario SET UsuarioCuentaBloqueada = true WHERE idUsuario = @idUsuario";
                        blockUserCmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        blockUserCmd.ExecuteNonQuery();
                        Close();
                        throw new TooManyAttemptsException();
                    }

                    Close();
                    throw new LoginException();
                }
            } else {
                // Usuario incorrecto
                Close();
                throw new LoginException();
            }
        }

        public object[] GetRequerimientos()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TipoRequerimientoDescripcion, PrioridadNombre, RequerimienetoDescripcion, PrioridadPlazo " +
                "FROM Requerimiento " +
                "INNER JOIN Prioridad, TipoRequerimiento " +
                "WHERE Requerimiento.Prioridad_idPrioridad = Prioridad.idPrioridad " +
                "AND Requerimiento.TipoRequerimiento_idTipoRequerimiento = TipoRequerimiento.idTipoRequerimiento";
            var reader = cmd.ExecuteReader();
            reader.Read();
            object[] results = new object[10];
            reader.GetValues(results);
            reader.Close();
            return results;
        }

        public object[] GetUsuarios()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idUsuario, UsuarioNombre FROM Usuario";
            var reader = cmd.ExecuteReader();
            List<object> results = new List<object>();
            while (reader.Read())
            {
                results.Add(new { Text = reader.GetString(1), Value = reader.GetInt32(0) });
            }
            reader.Close();
            Close();
            return results.ToArray();
        }

        public object[] GetTipoRequerimiento()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idTipoRequerimiento, TipoRequerimientoDescripcion FROM TipoRequerimiento";
            var reader = cmd.ExecuteReader();
            List<object> results = new List<object>();
            while (reader.Read())
            {
                results.Add(new { Text = reader.GetString(1), Value = reader.GetInt32(0) } );
            }
            reader.Close();
            Close();
            return results.ToArray();
        }

        public object[] GetPrioridad()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idPrioridad, PrioridadNombre FROM Prioridad";
            var reader = cmd.ExecuteReader();
            List<object> results = new List<object>();
            while (reader.Read())
            {
                results.Add(new { Text = reader.GetString(1), Value = reader.GetInt32(0) });
            }
            reader.Close();
            Close();
            return results.ToArray();
        }
    }

    public class LoginException: System.Exception { }

    public class TooManyAttemptsException: System.Exception { }
}
