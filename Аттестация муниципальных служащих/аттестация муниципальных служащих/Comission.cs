using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace аттестация_муниципальных_служащих
{
    public partial class Comission : Form
    {
        public Comission()
        {
            InitializeComponent();
            dataGridView1.Rows.Clear();

            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
