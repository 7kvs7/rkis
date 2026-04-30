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
    public partial class redcomission : Form
    {
        public redcomission()
        {
            InitializeComponent();
        }

        public string ComDate => dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
        public string ComName => comboBox1.Text;
        public void FillData(string date, string name)
        {
            if (DateTime.TryParse(date, out DateTime d))
                dateTimePicker1.Value = d;

            comboBox1.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Введите или выберите наименование комиссии!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
