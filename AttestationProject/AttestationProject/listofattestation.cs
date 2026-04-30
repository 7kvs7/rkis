using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace аттестация_муниципальных_служащих
{
    public partial class listofattestation : Form
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Attestation.mdf;Integrated Security=True";

        public listofattestation()
        {
            InitializeComponent();
            this.Load += listofattestation_Load;
        }

        private void listofattestation_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private async void RefreshGrid()
        {
            progressBar1.Visible = true;
            label1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;

            try
            {
                await Task.Delay(1000);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT NumOfAtt, FIO, Status, Otdel, Phone, StatOfAtt, Time FROM [Table]";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    await Task.Run(() => adapter.Fill(dt));

                    dataGridView1.AutoGenerateColumns = false;
                    dataGridView1.DataSource = dt;

                    dataGridView1.Columns["NumOfAtt"].DataPropertyName = "NumOfAtt";
                    dataGridView1.Columns["FIO"].DataPropertyName = "FIO";
                    dataGridView1.Columns["Status"].DataPropertyName = "Status";
                    dataGridView1.Columns["Otdel"].DataPropertyName = "Otdel";
                    dataGridView1.Columns["Phone"].DataPropertyName = "Phone";
                    dataGridView1.Columns["StatOfAtt"].DataPropertyName = "StatOfAtt";
                    dataGridView1.Columns["Time"].DataPropertyName = "Time";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                progressBar1.Visible = false;
                label1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var formQuestionnaire = new questionnaire())
            {
                if (formQuestionnaire.ShowDialog() == DialogResult.OK)
                {
                    this.SetData(
                        formQuestionnaire.NumOfAtt,
                        formQuestionnaire.FIO,
                        formQuestionnaire.Status,
                        formQuestionnaire.Otdel,
                        formQuestionnaire.Phone,
                        formQuestionnaire.StatOfAtt,
                        formQuestionnaire.Time
                    );
                }
            }
        }

        public void SetData(string num, string fio, string stat, string otdel, string ph, string sAtt, string t)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO [Table] (FIO, Status, Otdel, Phone, StatOfAtt, Time) " +
                                   "VALUES (@fio, @stat, @otdel, @ph, @sAtt, @t)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fio", fio);
                    cmd.Parameters.AddWithValue("@stat", stat);
                    cmd.Parameters.AddWithValue("@otdel", otdel);
                    cmd.Parameters.AddWithValue("@ph", ph);
                    cmd.Parameters.AddWithValue("@sAtt", sAtt);
                    cmd.Parameters.AddWithValue("@t", t);
                    cmd.ExecuteNonQuery();
                }
                RefreshGrid();
                MessageBox.Show("Запись успешно добавлена в БД!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) { MessageBox.Show("Выберите запись!"); return; }
            var row = dataGridView1.CurrentRow;
            var form = new questionnaire();
            form.FillData(
                row.Cells["NumOfAtt"].Value.ToString(),
                row.Cells["FIO"].Value.ToString(),
                row.Cells["Status"].Value.ToString(),
                row.Cells["Otdel"].Value.ToString(),
                row.Cells["Phone"].Value.ToString(),
                row.Cells["StatOfAtt"].Value.ToString(),
                row.Cells["Time"].Value.ToString()
            );

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE [Table] SET FIO=@f, Status=@s, Otdel=@o, Phone=@p, StatOfAtt=@sa, Time=@t WHERE NumOfAtt=@n";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@n", form.NumOfAtt);
                        cmd.Parameters.AddWithValue("@f", form.FIO);
                        cmd.Parameters.AddWithValue("@s", form.Status);
                        cmd.Parameters.AddWithValue("@o", form.Otdel);
                        cmd.Parameters.AddWithValue("@p", form.Phone);
                        cmd.Parameters.AddWithValue("@sa", form.StatOfAtt);
                        cmd.Parameters.AddWithValue("@t", form.Time);
                        cmd.ExecuteNonQuery();
                    }
                    RefreshGrid();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка обновления: " + ex.Message); }
            }
        }

        private void button3_Click(object sender, EventArgs e)
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
                            string query = "DELETE FROM [Table] WHERE NumOfAtt = @id";
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
                if (textBox1.Text == "Поиск" || string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    dt.DefaultView.RowFilter = ""; // Показываем все записи
                    return;
                }

                string search = textBox1.Text.Replace("'", "''");

                try
                {
                    dt.DefaultView.RowFilter = string.Format(
                        "Convert(NumOfAtt, 'System.String') LIKE '%{0}%' OR " + 
                        "FIO LIKE '%{0}%' OR " +
                        "Status LIKE '%{0}%' OR " +
                        "Otdel LIKE '%{0}%' OR " +
                        "Phone LIKE '%{0}%' OR " +
                        "StatOfAtt LIKE '%{0}%' OR " +
                        "Time LIKE '%{0}%'",
                        search);
                }
                catch (Exception)
                {
     
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    dataGridView1.DataSource = null;
                    RefreshGrid();
                    conn.Open();
                    string query = "TRUNCATE TABLE [Table]";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                RefreshGrid();
                MessageBox.Show("Данные успешно очищены");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void комиссияToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Comission formComission = new Comission();
            formComission.Show();
        }

        private void графикАттестацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            schedule formSchedule = new schedule();
            formSchedule.Show();
        }

        private void аттестацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proc formProc = new proc();
            formProc.Show();
        }
    }
}
