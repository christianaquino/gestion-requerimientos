using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using CapaDatos;

namespace CapaNegocio
{
    public class Gestion_Rquerimientos_Negocio
    {
        private readonly BD database = new CapaDatos.BD();
        private int UsuarioLogueado;

        public int GetUsuarioLogueado()
        {
            return UsuarioLogueado;
        } 
        public void Login(String user, String password) {
            try{
                UsuarioLogueado = database.Login(user, password);
            } catch (LoginException)
            {
                throw new Exception("LoginException");
            }
            catch (TooManyAttemptsException)
            {
                throw new Exception("TooManyAttemptsException");
            }
            catch(Exception)
            {
                throw new Exception("GeneralError");
            }
        }

        public DataTable GetRequerimientos(int? tipo, int? prioridad, bool? resuelto)
        {
            return database.GetRequerimientos(tipo, prioridad, resuelto);
        }

        public Dictionary<int, string> GetTipoItems()
        {
            return database.GetTipoRequerimiento();
        }

        public Dictionary<int, string>  GetUsuarios()
        {
            return database.GetUsuarios();
        }

        public Dictionary<int, string> GetPrioridad()
        {
            return database.GetPrioridad();
        }

        public int RegistrarRequerimiento(int tipoRequerimiento, int prioridad, int usuarioAsignado, string descripcion)
        {
            database.RegistrarRequerimiento(tipoRequerimiento, prioridad, usuarioAsignado, UsuarioLogueado, descripcion);
            return database.getPlazo(prioridad);
        }
    }
}
