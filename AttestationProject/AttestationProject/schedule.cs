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
    public partial class schedule : Form
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";
        public schedule()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                if (MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        string id = dataGridView1.CurrentRow.Cells["NumOfAtt"].Value.ToString();
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM [Schedule] WHERE NumOfAtt = @id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        RefreshGrid();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;

            if (dt != null)
            {
                // Проверка на пустоту или слово-подсказку
                if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text == "Поиск")
                {
                    dt.DefaultView.RowFilter = "";
                    return;
                }

                string search = textBox1.Text.Replace("'", "''");

                try
                {
                    // Ищем по ID графика, датам и номеру комиссии
                    dt.DefaultView.RowFilter = string.Format(
                        "Convert(Id_schedule, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(DateStart, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(DateEnd, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(Id_comission, 'System.String') LIKE '%{0}%'",
                        search);
                }
                catch (Exception) { /* Игнорируем ошибки при наборе */ }
            }
        }
        public void SetData(string start, string end, string comission)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Указываем только те поля, которые заполняем вручную
                    string query = "INSERT INTO [Schedule] (DateStart, DateEnd, Id_comission) " +
                                   "VALUES (@start, @end, @com)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@end", end);
                    cmd.Parameters.AddWithValue("@com", comission);
                    cmd.ExecuteNonQuery();
                }
                RefreshGrid();
                MessageBox.Show("График успешно добавлен!");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка сохранения: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var formSchedulered = new schedulered())
            {
                if (formSchedulered.ShowDialog() == DialogResult.OK)
                {
                    this.SetData(
                        formSchedulered.DateStart,
                        formSchedulered.DateEnd, 
                        formSchedulered.IdComission 
                    );
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) { MessageBox.Show("Выберите запись!"); return; }

            var row = dataGridView1.CurrentRow;
            var form = new schedulered(); // Наша форма редактирования

            // Передаем данные из таблицы в форму редактирования
            form.FillData(
                row.Cells["DateStart"].Value.ToString(),
                row.Cells["DateEnd"].Value.ToString(),
                row.Cells["Id_comission"].Value.ToString()
            );

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // Обновляем запись по её ID (Id_schedule)
                        string query = "UPDATE [Schedule] SET DateStart=@s, DateEnd=@e, Id_comission=@c WHERE Id_schedule=@id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", row.Cells["Id_schedule"].Value);
                        cmd.Parameters.AddWithValue("@s", form.DateStart);
                        cmd.Parameters.AddWithValue("@e", form.DateEnd);
                        cmd.Parameters.AddWithValue("@c", form.IdComission);
                        cmd.ExecuteNonQuery();
                    }
                    RefreshGrid();
                    MessageBox.Show("График обновлен!");
                }
                catch (Exception ex) { MessageBox.Show("Ошибка обновления: " + ex.Message); }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    dataGridView1.DataSource = null;
                    RefreshGrid();
                    conn.Open();
                    string query = "TRUNCATE TABLE [Schedule]";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                RefreshGrid();
                MessageBox.Show("Данные успешно очищены");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
        private async void RefreshGrid()
        {
            // Включаем визуал загрузки
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

            try
            {
                await Task.Delay(1000); // Искусственная пауза 1 сек

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT Id_schedule, DateStart, DateEnd, Id_comission FROM [Schedule]";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    // Грузим данные в фоновом потоке
                    await Task.Run(() => da.Fill(dt));

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки графика: " + ex.Message);
            }
            finally
            {
                // Скрываем загрузку
                progressBar1.Visible = false;
            }
        }

        private void schedule_load(object sender, EventArgs e)
        {
            RefreshGrid();
        }


    }

}

