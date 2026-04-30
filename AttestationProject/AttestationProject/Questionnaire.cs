using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace аттестация_муниципальных_служащих
{
    public partial class questionnaire : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";


        public questionnaire()
        {
            InitializeComponent();
        }

        public string NumOfAtt => textBox1.Text;
        public string FIO => textBox2.Text;
        public string Status => comboBox1.Text;
        public string Otdel => comboBox2.Text;
        public string Phone => maskedTextBox1.Text;
        public string StatOfAtt => comboBox3.Text;
        public string Time => dateTimePicker1.Text;

        public void FillData(string numOfAtt, string fio, string status, string otdel, string phone, string statOfAtt, string time)
        {
            textBox1.Text = numOfAtt;
            textBox2.Text = fio;
            comboBox1.Text = status;
            comboBox2.Text = otdel;
            maskedTextBox1.Text = phone;
            comboBox3.Text = statOfAtt;
            dateTimePicker1.Text = time;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Можно добавить отмену
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "TRUNCATE TABLE [Table]";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                RefreshGrid();
                MessageBox.Show("Данные очищены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void RefreshGrid()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void questionnaire_Load(object sender, EventArgs e)
        {

        }

    }
}