using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public object[] GetRequerimientos()
        {
            return database.GetRequerimientos();
        }
        
        public object[] GetTipoItems()
        {
            return database.GetTipoRequerimiento();
        }

        public object[] GetUsuarios()
        {
            return database.GetUsuarios();
        }

        public object[] GetPrioridad()
        {
            return database.GetPrioridad();
        }
    }
}
