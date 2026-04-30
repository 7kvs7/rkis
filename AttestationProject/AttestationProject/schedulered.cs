using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace аттестация_муниципальных_служащих
{
    public partial class schedulered : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";

        public string DateStart => dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
        public string DateEnd => dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
        public string IdComission => comboBox1.Text;
        public schedulered()
        {
            InitializeComponent();
            LoadComissions();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите номер комиссии!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "TRUNCATE TABLE [Schedule]";
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
        public void FillData(string start, string end, string comission)
        {
            if (DateTime.TryParse(start, out DateTime s)) dateTimePicker1.Value = s;
            if (DateTime.TryParse(end, out DateTime e)) dateTimePicker2.Value = e;
            comboBox1.Text = comission;
        }
        private void LoadComissions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Берем все ID из таблицы комиссий
                    string query = "SELECT Id_comission FROM [Comission]";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBox1.Items.Clear(); // Очищаем старые данные
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["Id_comission"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка комиссий: " + ex.Message);
            }
        }

        private void schedulered_load(object sender, EventArgs e)
        {
        }
    }
}
