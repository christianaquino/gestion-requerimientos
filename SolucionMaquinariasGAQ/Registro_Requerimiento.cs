using CapaNegocio;
using GestionRequerimientos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class Registro_Requerimiento : Form
    {
        private Gestion_Rquerimientos_Negocio negocio = new Gestion_Rquerimientos_Negocio();
        public Registro_Requerimiento()
        {
            InitializeComponent();
            comboBoxTipo.Items.AddRange(negocio.GetTipoItems());
            comboBoxUsuario.Items.AddRange(negocio.GetUsuarios());
            comboBoxPrioridad.Items.AddRange(negocio.GetPrioridad());
        }

        private void Registro_Requerimiento_Load(object sender, EventArgs e)
        {
            if (negocio.GetUsuarioLogueado() == 0)
            {
                FormLogin login = new FormLogin();
                login.ShowDialog();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
