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
    public partial class Attestation : Form
    {
        public Attestation()
        {
            InitializeComponent();
        }

        public void ShowProtocol(string text)
        {
            richTextBox1.Text = text;
            richTextBox1.Font = new Font("Times New Roman", 12);

            // Выделяем весь текст, чтобы применить отступы
            richTextBox1.SelectAll();

            // Делаем отступ слева и справа (как поля у листа бумаги)
            richTextBox1.SelectionIndent = 30; // Отступ слева в пикселях
            richTextBox1.SelectionRightIndent = 30; // Отступ справа

            // Снимаем выделение
            richTextBox1.DeselectAll();
        }
    }
}
