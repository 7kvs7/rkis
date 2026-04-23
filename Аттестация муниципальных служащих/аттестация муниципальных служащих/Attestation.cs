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
        }
    }
}
