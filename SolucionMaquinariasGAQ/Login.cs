using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using Presentacion;

namespace GestionRequerimientos
{
    public partial class FormLogin : Form
    {
        Gestion_Rquerimientos_Negocio negocio = new Gestion_Rquerimientos_Negocio();
        public FormLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                negocio.Login(textBoxUsuario.Text, textBoxPassword.Text);
                this.Hide();
            }
            catch (Exception ex)
            {
                labelError.Text = (ex.Message) switch
                {
                    "LoginException" => "Ususrio y/o contraseña incorrectos",
                    "TooManyAttemptsException" => "Demasiados intentos fallidos. La cuenta ha sido bloqueada",
                    _ => "Ha ocurrido un error. Por favor vuelva a intentarlo",
                };
            }
        }

        private void updateButton()
        {
            buttonLogin.Enabled = !string.IsNullOrEmpty(textBoxUsuario.Text) && !string.IsNullOrEmpty(textBoxPassword.Text);
        }

        private void textBoxUsuario_TextChanged(object sender, EventArgs e)
        {
            updateButton();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            updateButton();
        }
    }
}
