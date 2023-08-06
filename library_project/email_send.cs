using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace library_project
{
    public partial class email_send : Form
    {
        public Form2 form2;
        public email_send()
        {
            InitializeComponent();
            Form2 form = new Form2();

            listBox1.Items.Add(form.Get_listNum()["2727"]);

            listBox1.Items.Add(form.getnum());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void email_send_Load(object sender, EventArgs e)
        {
            
        }
    }
}
