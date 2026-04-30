using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace аттестация_муниципальных_служащих
{
    public partial class Comission : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";

        public Comission()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            RefreshGrid();
        }

        // 1. Метод обновления таблицы (теперь максимально простой)
        private async void RefreshGrid() // Метод теперь async
        {
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

            try
            {
                // 1. Ждем 1 секунду (имитация загрузки), чтобы полоска успела погулять
                await Task.Delay(1000);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // 2. Открываем соединение асинхронно
                    await conn.OpenAsync();

                    string query = "SELECT Id_comission, date, nameofcom FROM [Comission]";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    // 3. Заполняем таблицу в фоновом потоке, чтобы интерфейс не зависал
                    await Task.Run(() => da.Fill(dt));

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления таблицы: " + ex.Message);
            }
            finally
            {
                // 4. Когда всё загрузилось — прячем прогрессбар
                progressBar1.Visible = false;
            }
        }


        // 2. Событие загрузки (связываем его в дизайнере!)
        private void Comission_Load(object sender, EventArgs e)
        {
            
        }

        // 3. Кнопка ДОБАВИТЬ
        private void button1_Click(object sender, EventArgs e)
        {
            using (var form = new redcomission())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "INSERT INTO [Comission] (date, nameofcom) VALUES (@d, @n)";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@d", form.ComDate);
                            cmd.Parameters.AddWithValue("@n", form.ComName);
                            cmd.ExecuteNonQuery();
                        }
                        RefreshGrid();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка сохранения: " + ex.Message); }
                }
            }
        }

        // 4. Кнопка РЕДАКТИРОВАТЬ
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) { MessageBox.Show("Выберите строку!"); return; }

            var row = dataGridView1.CurrentRow;
            using (var form = new redcomission())
            {
                form.FillData(
                    row.Cells["date"].Value.ToString(),
                    row.Cells["nameofcom"].Value.ToString()
                );

                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "UPDATE [Comission] SET date=@d, nameofcom=@n WHERE Id_comission=@id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", row.Cells["Id_comission"].Value);
                            cmd.Parameters.AddWithValue("@d", form.ComDate);
                            cmd.Parameters.AddWithValue("@n", form.ComName);
                            cmd.ExecuteNonQuery();
                        }
                        RefreshGrid();
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка обновления: " + ex.Message); }
                }
            }
        }

        // 5. Кнопка УДАЛИТЬ
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                if (MessageBox.Show("Удалить выбранную комиссию?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        string id = dataGridView1.CurrentRow.Cells["Id_comission"].Value.ToString();
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM [Comission] WHERE Id_comission = @id";
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

        // 6. Кнопка ОЧИСТИТЬ ВСЁ
        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить ВСЕ комиссии?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM [Comission]";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        string resetQuery = "DBCC CHECKIDENT ('[Comission]', RESEED, 0)";
                        new SqlCommand(resetQuery, conn).ExecuteNonQuery();
                    }
                    RefreshGrid();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка очистки: " + ex.Message); }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;

            if (dt != null)
            {
                if (textBox1.Text == "Поиск" || string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    dt.DefaultView.RowFilter = "";
                    return;
                }

                string search = textBox1.Text.Replace("'", "''");

                try
                {
                    dt.DefaultView.RowFilter = string.Format(
                        "Convert(Id_comission, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(date, 'System.String') LIKE '%{0}%' OR " +
                        "nameofcom LIKE '%{0}%'",
                        search);
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
