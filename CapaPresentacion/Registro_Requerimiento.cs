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
            comboBoxTipo.DataSource = new BindingSource(negocio.GetTipoItems(), null);
            comboBoxUsuario.DataSource = new BindingSource(negocio.GetUsuarios(), null);
            comboBoxPrioridad.DataSource = new BindingSource(negocio.GetPrioridad(), null);
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

        private void button3_Click(object sender, EventArgs e)
        {
            Listado_Requerimientos listado = new Listado_Requerimientos();
            listado.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int tipoRequerimiento = (int)comboBoxTipo.SelectedValue;
                int prioridad = (int)comboBoxPrioridad.SelectedValue;
                int usuarioAsignado = (int)comboBoxUsuario.SelectedValue;
                string descripcion = textBoxDescripcion.Text;
                negocio.RegistrarRequerimiento(tipoRequerimiento, prioridad, usuarioAsignado, descripcion);
                MessageBox.Show("El requerimiento fue ingresado, el plazo para resolverlo es de");
            } catch(Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar el requerimiento" + ex);
            }
            
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Usuario logueado: " + negocio.GetUsuarioLogueado().ToString());
        }
    }
}
