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

        // Свойства для получения данных после закрытия формы
        public string NumOfAtt => textBox1.Text;
        public string FIO => textBox2.Text;
        public string Status => textBox3.Text;
        public string Otdel => textBox4.Text;
        public string Phone => maskedTextBox1.Text;
        public string StatOfAtt => textBox5.Text;
        public string Time => maskedTextBox2.Text;

        // Метод для заполнения данных (если нужно)
        public void FillData(string numOfAtt, string fio, string status, string otdel, string phone, string statOfAtt, string time)
        {
            textBox1.Text = numOfAtt;
            textBox2.Text = fio;
            textBox3.Text = status;
            textBox4.Text = otdel;
            maskedTextBox1.Text = phone;
            textBox5.Text = statOfAtt;
            maskedTextBox2.Text = time;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Можно сделать валидацию, если нужно

            // Указываем, что пользователь подтвердил
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