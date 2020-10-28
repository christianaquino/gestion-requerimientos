using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CapaNegocio;

namespace Presentacion
{
    public partial class Listado_Requerimientos : Form
    {
        readonly Gestion_Rquerimientos_Negocio negocio = new Gestion_Rquerimientos_Negocio();
        public Listado_Requerimientos()
        {
            InitializeComponent();
            dataGridRequerimientos.Rows.Add(negocio.GetRequerimientos());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Listado_Requerimientos_Load(object sender, EventArgs e)
        {

        }
    }
}
