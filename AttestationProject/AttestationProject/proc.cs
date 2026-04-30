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
        int personCounter = 1;

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
                        comboBox2.Items.Clear(); // Исправлено: используем comboBox2 вместо textBox1
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
            // Используем BeginInvoke, чтобы получить актуальное состояние CheckedItems
            this.BeginInvoke(new MethodInvoker(() =>
            {
                checkedListBox2.Items.Clear();
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    checkedListBox2.Items.Add(item.ToString());
                }
            }));
        }

        // КНОПКА СОСТАВИТЬ ПРОТОКОЛ
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Собираем текст протокола
            string result = "\n\t\tПРОТОКОЛ ЗАСЕДАНИЯ\n";
        

            // Списки присутствующих и выступающих
            result += "\nПРИСУТСТВОВАЛИ:\n" + string.Join(", ", checkedListBox1.CheckedItems.Cast<object>()) + "\n\n";
            result += "ВЫСТУПАЛИ:\n" + string.Join(", ", checkedListBox2.CheckedItems.Cast<object>()) + "\n";

            result += "\nПОВЕСТКА ДНЯ:\nАттестация муниципальных служащих.\n\n";

            // Список решений из вашего TextBox
            result += "\nРЕШИЛИ:\n" + textBox1.Text;

            // 2. СОЕДИНЯЕМ ФОРМЫ:
            // Создаем окно отчета
            Attestation reportForm = new Attestation();

            // Передаем собранный текст в метод ShowProtocol, который вы написали выше
            reportForm.ShowProtocol(result);

            // Показываем вторую форму пользователю
            reportForm.Show();
        }


        // КНОПКА: ДОБАВИТЬ АТТЕСТУЕМОГО В СПИСОК
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null && comboBox1.SelectedItem != null)
            {
                string entry = $"{personCounter}. {comboBox2.Text} — {comboBox1.Text};" + Environment.NewLine;

                // Добавляем запись в наш новый большой TextBox
                textBox1.AppendText(entry);

                personCounter++;
                comboBox2.SelectedIndex = -1; // Сброс выбора ФИО для удобства
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Показываем окно с вопросом
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите очистить форму?", // Текст сообщения
                "Подтверждение очистки",                                                   
                MessageBoxButtons.YesNo                                                    
                                                         
            );

            // Если пользователь нажал "Да" (Yes)
            if (result == DialogResult.Yes)
            {
                // 1. Очищаем текстбокс
                textBox1.Clear();

                // 2. Сбрасываем счетчик
                personCounter = 1;

                // 3. Сбрасываем выпадающие списки
                comboBox2.SelectedIndex = -1;
                comboBox1.SelectedIndex = -1;

                // 4. Снимаем галочки в первом списке (второй очистится автоматически)
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }

                // На всякий случай принудительно чистим второй список
                checkedListBox2.Items.Clear();
            }
            // Если нажали "Нет", код внутри if просто проигнорируется
        }

    }
}
