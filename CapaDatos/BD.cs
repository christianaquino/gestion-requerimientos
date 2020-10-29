using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MySqlX.XDevAPI;

namespace CapaDatos
{
    public class BD
    {
        static readonly string connstring = @"server=127.0.0.1;uid=root;pwd=*****;database=requerimientos";
        static readonly MySqlConnection conn = new MySqlConnection(connstring);
        private void Connect() {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }

        private void Close() {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
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

        public DataTable GetRequerimientos(int? tipo, int? prioridad, bool? resuelto)
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TipoRequerimientoDescripcion, PrioridadNombre, RequerimienetoDescripcion, PrioridadPlazo " +
                "FROM Requerimiento " +
                "INNER JOIN Prioridad, TipoRequerimiento " +
                "WHERE Requerimiento.Prioridad_idPrioridad = Prioridad.idPrioridad " +
                "AND Requerimiento.TipoRequerimiento_idTipoRequerimiento = TipoRequerimiento.idTipoRequerimiento";
            
            if(tipo!=null)
            {
                cmd.CommandText += " AND TipoRequerimiento.idTipoRequerimiento = " + tipo;
            }

            if (prioridad != null)
            {
                cmd.CommandText += " AND Prioridad.idPrioridad = " + prioridad;
            }

            if(resuelto != null) {
                if(resuelto == true) {
                    cmd.CommandText += " AND RequerimienetoResuelto";
                } else
                {
                    cmd.CommandText += " AND NOT RequerimienetoResuelto";
                }
                
            }

            var reader = cmd.ExecuteReader();
            
            DataTable requerimientos = new DataTable();
            requerimientos.Columns.Add(new DataColumn("TipoRequerimiento"));
            requerimientos.Columns.Add(new DataColumn("Prioridad"));
            requerimientos.Columns.Add(new DataColumn("Descripcion"));
            requerimientos.Columns.Add(new DataColumn("Plazo"));

            //dataGridRequerimientos.DataSource = new BindingSource(negocio.GetRequerimientos(), null);
            while(reader.Read())
            {
                DataRow newRow = requerimientos.NewRow();
                newRow["TipoRequerimiento"] = reader.GetValue(0);
                newRow["Prioridad"] = reader.GetValue(1);
                newRow["Descripcion"] = reader.GetValue(2);
                newRow["Plazo"] = reader.GetValue(3);
                requerimientos.Rows.Add(newRow);
            }
           
            reader.Close();
            return requerimientos;
        }

        public void RegistrarRequerimiento(int tipoRequerimiento, int prioridad, int usuarioAsignado, int creadoPor, string descripcion)
        {
            Connect();
            var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Requerimiento " +
                "(RequerimienetoDescripcion, Prioridad_idPrioridad, TipoRequerimiento_idTipoRequerimiento, AsignadoA_idUsuario, CreadoPor_idUsuario) " +
                "VALUES (@descripcion, @prioridad, @tipoRequerimiento, @usuarioAsignado, @creadoPor)";
            insertCmd.Parameters.AddWithValue("@descripcion", descripcion);
            insertCmd.Parameters.AddWithValue("@prioridad", prioridad);
            insertCmd.Parameters.AddWithValue("@tipoRequerimiento", tipoRequerimiento);
            insertCmd.Parameters.AddWithValue("@usuarioAsignado", usuarioAsignado);
            insertCmd.Parameters.AddWithValue("@creadoPor", 5);
            insertCmd.ExecuteNonQuery();
            Close();
        }
        public Dictionary<int, string> GetUsuarios()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idUsuario, UsuarioNombre FROM Usuario ORDER BY UsuarioNombre ASC";
            var reader = cmd.ExecuteReader();
            Dictionary<int, string> results = ParseResult(reader);
            reader.Close();
            Close();
            return results;
        }

        public int getPlazo(int idPrioridad)
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PrioridadPlazo FROM Prioridad WHERE idPrioridad = " + idPrioridad;
            var reader = cmd.ExecuteReader();
            reader.Read();
            int plazo = reader.GetInt16("PrioridadPlazo");
            reader.Close();
            Close();
            return plazo; 
        }

        public Dictionary<int, string> GetTipoRequerimiento()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idTipoRequerimiento, TipoRequerimientoDescripcion FROM TipoRequerimiento " +
                "ORDER BY TipoRequerimientoDescripcion ASC";
            var reader = cmd.ExecuteReader();
            Dictionary<int, string> results = ParseResult(reader);
            reader.Close();
            Close();
            return results;
        }

        public Dictionary<int, string> GetPrioridad()
        {
            Connect();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT idPrioridad, PrioridadNombre FROM Prioridad ORDER BY PrioridadNombre ASC";
            var reader = cmd.ExecuteReader();
            Dictionary<int, string> results = ParseResult(reader);
            reader.Close();
            Close();
            return results;
        }

        private Dictionary<int, string> ParseResult(MySqlDataReader reader)
        {
            Dictionary<int, string> results = new Dictionary<int, string>();
            while (reader.Read())
            {
                results.Add(reader.GetInt32(0), reader.GetString(1));
            }

            return results;
        }
    }

    public class LoginException: System.Exception { }

    public class TooManyAttemptsException: System.Exception { }

}
