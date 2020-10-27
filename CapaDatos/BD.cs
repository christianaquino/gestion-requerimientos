using System;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CapaDatos
{
    public class BD
    {
        static readonly string connstring = @"server=127.0.0.1;uid=root;pwd=****;database=requerimientos";
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

        public void Login(String user, String password)
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
            reader.Close();
            Close();
        }

        public object[] getRequerimientos()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM requerimieneto";
            var reader = cmd.ExecuteReader();
            reader.Read();
            object[] results = new object[10];
            reader.GetValues(results);
            reader.Close();
            return results;
        }
    }

    public class LoginException: System.Exception { }

    public class TooManyAttemptsException: System.Exception { }
}
