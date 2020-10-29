using CapaNegocio;
using System;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class Listado_Requerimientos : Form
    {
        readonly Gestion_Rquerimientos_Negocio negocio = new Gestion_Rquerimientos_Negocio();
        public Listado_Requerimientos()
        {
            InitializeComponent();
            comboBoxTipo.DataSource = new BindingSource(negocio.GetTipoItems(), null);
            comboBoxPrioridad.DataSource = new BindingSource(negocio.GetPrioridad(), null);
            dataGridRequerimientos.DataSource = negocio.GetRequerimientos(null, null, null);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool? resuelto = null;
            if (radioButton1.Checked)
            {
                resuelto = true;
            } else if (radioButton2.Checked)
            {
                resuelto = false;
            }

            dataGridRequerimientos.DataSource = negocio.GetRequerimientos(
                (int)comboBoxTipo.SelectedValue, (int)comboBoxPrioridad.SelectedValue, resuelto);
        }

        private void Listado_Requerimientos_Load(object sender, EventArgs e)
        {
        }
    }
}
