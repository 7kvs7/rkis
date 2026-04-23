using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.ComponentModel;

namespace аттестация_муниципальных_служащих
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var con = new SqlConnection(Properties.Settings.Default.AttestationConnectionString);
            con.Open();

            if (button1.Text == "Войти")
            {
                string query = "SELECT * FROM Users WHERE логин = @login AND пароль = @password";
                SqlDataAdapter adap = new SqlDataAdapter(query, con);
                adap.SelectCommand.Parameters.AddWithValue("@login", textBox1.Text);
                adap.SelectCommand.Parameters.AddWithValue("@password", textBox2.Text);

                AttestationDataSet ds = new AttestationDataSet();
                adap.Fill(ds, "Users");

                if (ds.Tables["Users"].Rows.Count != 0)
                {
                    DataRow userRow = ds.Tables["Users"].Rows[0]; // берем первого пользователя

                    string roleId = userRow["id_role"].ToString().Trim();

                    // Общее обращение к форме
                    Form frm = new listofattestation();
                    frm.Left = this.Left;
                    frm.Top = this.Top;
                    frm.Show();
                    this.Hide();

                    button1.Text = "Выйти";
                    label1.Visible = false;
                    label2.Visible = false;
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                }
                else
                {
                    MessageBox.Show("Логин или пароль не верен");
                }
            }
            else 
            {
                button1.Text = "Войти";
                label1.Visible = true;
                label2.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label3.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                this.Show(); // Возвращаем форму авторизации
            }

            con.Close(); // закрываем соединение
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }

    }