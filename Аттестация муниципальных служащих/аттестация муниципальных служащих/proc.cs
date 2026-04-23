using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace аттестация_муниципальных_служащих
{
    public partial class proc : Form
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";
        int personCounter = 1; // Счетчик для нумерации строк

        public proc()
        {
            InitializeComponent();
            this.Load += Attestation_Load;

            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            checkedListBox1.CheckOnClick = true;
        }

        private void Attestation_Load(object sender, EventArgs e)
        {
            LoadDataFromDB();
        }

        private void LoadDataFromDB()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT FIO FROM [Table]";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        checkedListBox1.Items.Clear();
                        comboBox2.Items.Clear();

                        while (reader.Read())
                        {
                            string fio = reader["FIO"].ToString();
                            checkedListBox1.Items.Add(fio);
                            comboBox2.Items.Add(fio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                checkedListBox2.Items.Clear();
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    checkedListBox2.Items.Add(item.ToString());
                }
            }));
        }

        // КНОПКА СОСТАВИТЬ ПРОТОКОЛ (Финальное действие)
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Собираем заголовок и списки комиссии
            string result = "\t\tПРОТОКОЛ ЗАСЕДАНИЯ\n";
            result += "------------------------------------------------------\n";
            result += "ПРИСУТСТВОВАЛИ: " + string.Join(", ", checkedListBox1.CheckedItems.Cast<object>()) + "\n";
            result += "ВЫСТУПАЛИ: " + string.Join(", ", checkedListBox2.CheckedItems.Cast<object>()) + "\n\n";

            result += "ПОВЕСТКА ДНЯ: Аттестация сотрудников.\n\n";

            // 2. Добавляем тот список "Решили", который мы копили кнопкой "Добавить"
            // (Замени label3.Text на textBox, если ты в итоге использовала его)
            result += "РЕШИЛИ:\n" + label3.Text;

            // 3. Открываем форму с протоколом
            Attestation reportForm = new Attestation();
            reportForm.Show(); // Сначала показываем форму
            reportForm.ShowProtocol(result); // Потом передаем в неё текст
        }

        // КНОПКА: ДОБАВИТЬ АТТЕСТУЕМОГО В СПИСОК
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null && comboBox1.SelectedItem != null)
            {
                string entry = $"{personCounter}. {comboBox2.Text} — {comboBox1.Text};" + Environment.NewLine;

                // ВАЖНО: Добавляем строку в текстовое поле на форме, чтобы её было видно!
                // Замени 'labelReshili' на имя своего TextBox или Label, где должен расти список
                label3.Text += entry;

                personCounter++;
                comboBox2.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Сначала выберите ФИО и Статус!");
            }
        }

        // КНОПКА ОЧИСТИТЬ (добавил для удобства)
        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "Решили:" + Environment.NewLine;
            personCounter = 1;
        }
    }
} // Удалил лишнюю скобку здесь
