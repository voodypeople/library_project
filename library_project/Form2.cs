using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing.Text;
using System.Collections;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace library_project
{
    public partial class Form2 : Form
    {
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        private DataTable dt;
        PrivateFontCollection font;
        private bool emailRecieved = false;

        public List<KeyValuePair<string, string>> list_numbers;
        public Form2()
        {
            
            InitializeComponent();
            this.Text = "База данных читателей";
            comboBox1.Items.Add("Да");
            comboBox1.Items.Add("Нет");
            fontsProjects();
            //Применяем шрифты к компонентам
            fonts();
            //comboBox1.SelectedItem = "Да";
            
        }

        private void fontsProjects()
        {
            //Добавляем шрифт из указанного файла в em.Drawing.Text.PrivateFontCollection
            this.font = new PrivateFontCollection();
            this.font.AddFontFile(Environment.CurrentDirectory.ToString() + "\\Fonts\\Bowler.ttf");

        }

        private void fonts()
        {
            //Задаем шрифт текста, отображаемого элементом управления.
            groupBox1.Font = new System.Drawing.Font(font.Families[0], 9);
            label1.Font = new System.Drawing.Font(font.Families[0], 8);//Alice.ttf
            label2.Font = new System.Drawing.Font(font.Families[0], 8);//Modestina.ttf
            label3.Font = new System.Drawing.Font(font.Families[0], 8);
            //label3.Font = new System.Drawing.Font(font.Families[0], 10);//serp_and_molot.ttf
            button1.Font = new System.Drawing.Font(font.Families[0], 10);
            button2.Font = new System.Drawing.Font(font.Families[0], 10);
            button3.Font = new System.Drawing.Font(font.Families[0], 10);
            действияToolStripMenuItem.Font = new System.Drawing.Font(font.Families[0], 8);

        }
        public void ChangeId(string str)
        {
            textBox1.Text = str;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //string connection_string = "Data Source=D:\\employers_db\\employers.db;New=False;Version=3";
            //SQLiteConnection con;
            //SQLiteCommand cmd;
            //DataTable dt;
            
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";

            cmd = new SQLiteCommand();
            cmd.Connection = con;

            dt = new DataTable();
            dataGridView1.DataSource = dt; // связываешь DataTable и таблицу на форме (просто dt)
            
            con.Open(); // открываешь соединение с БД
            cmd.CommandText = "select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number";
            dt.Clear();


            dt.Load(cmd.ExecuteReader()); // выполняешь SQL-запрос
            //Console.WriteLine(dataGridView1.Rows[0].Cells[0].ToString());
            DateTime da;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                da = Convert.ToDateTime( dataGridView1.Rows[i].Cells[4].Value);
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == false)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);

                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
                else
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            //con.Close(); // закрываешь соединение с БД
            //dt.Columns[0].ColumnName = "Номер билета";
            dataGridView1.Columns[0].HeaderText = "Номер билета";
            dataGridView1.Columns[1].HeaderText = "Автор книги";
            dataGridView1.Columns[2].HeaderText = "Название книги";
            dataGridView1.Columns[3].HeaderText = "Дата издания";
            dataGridView1.Columns[4].HeaderText = "Дата взятия";
            dataGridView1.Columns[5].HeaderText = "Сдана ли книга";
            dataGridView1.Columns[6].HeaderText = "Почта";

            dataGridView1.Columns[1].Width = 170;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[5].Width = 180;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //cmd.CommandText = "select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number";

            if (comboBox1.SelectedItem == "Да" && textBox1.Text != "" && textBox2.Text == "")
                cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where expired == 1 and readers.number == '{0}'", textBox1.Text);

            if (comboBox1.SelectedItem == "Нет" && textBox1.Text != "" && textBox2.Text == "")
                cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where expired == 0 and readers.number == '{0}'", textBox1.Text);

            if (comboBox1.SelectedItem == "Да")
                cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where expired == 1;");
            
            if(comboBox1.SelectedItem == "Нет" && textBox2.Text == "")
               cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where expired == 0");
            
            if(comboBox1.SelectedItem == "" && textBox2.Text == "")
               cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where number == '{0}';", textBox1.Text);
       
            if(textBox1.Text != "" && textBox2.Text != "" && comboBox1.SelectedItem == "Да")
               cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where bookAuthor like '{0}%' and readers.number == '{1}' and expired == 1;", textBox2.Text, textBox1.Text);
            
            if (textBox1.Text != "" && textBox2.Text != "" && comboBox1.SelectedItem == "Нет")
               cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where bookAuthor like '{0}%' and readers.number == '{1}' and expired == 0;", textBox2.Text, textBox1.Text);
            if (textBox2.Text == "" && comboBox1.Text == "" && textBox1.Text != "")
                cmd.CommandText = String.Format("select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number where readers.number == '{0}';", textBox1.Text);
            //cmd.CommandText = "select * from aboniment where number = \"666\";";
            dt.Clear();
            dt.Load(cmd.ExecuteReader());
            for(int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == false)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);

                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
                else
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void отметитьСдавшегоКнигуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sdacha sdachaKnigi = new sdacha();
            sdachaKnigi.ShowDialog();
        }

        private void удалитьИсториюЧитателяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteHistory history= new deleteHistory();
            history.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cmd.CommandText = "select readers.number, bookAuthor, bookName, Datest, dateStart, expired, readers.email from aboniment join readers on aboniment.number == readers.number";
            dt.Clear();


            dt.Load(cmd.ExecuteReader());
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == false)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);

                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {

                        dataGridView1.Rows[i].Cells[j].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
                else
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Application.ExitThread();
            
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[5].Value) == false)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71);

                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {

                        dataGridView1.Rows[i].Cells[j].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
                else
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }
        public string getnum()
        {
            return "hellow";
        }
        public Dictionary<string, string> Get_listNum()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < 2; i++)
            {
                dic.Add(list_numbers[i].Key, list_numbers[i].Value);
            }
            return dic;
        }
        private void отправитьEmailЧитателюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selected;
            int num;
            list_numbers = new List<KeyValuePair<string, string>>();
            selected = dataGridView1.SelectedRows.Count;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].DefaultCellStyle.BackColor == Color.FromArgb(255, 99, 71))
                {
                    list_numbers.Add(new KeyValuePair<string, string>( dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString()));
                    
                }
            }
            list_numbers.Sort();

            List<string> key   = new List<string>();


            
            /*List<string> keys = new List<string>();
            for(int i =0; i < list_numbers.Count; i++)
            {
                keys.Add(list_numbers[i].Key);
            }
            List<string> key_books = new List<string>();
            for(int i =0; i < list_numbers.Count; i++)
            {
                key_books.Add(list_numbers[i].Value);
            }*/
            //email_send win = new email_send();
            //win.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(button3.Enabled == false)
            {
                MessageBox.Show("", "");
            }

            int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
            DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];

            if (Convert.ToBoolean(selectedRow.Cells[5].Value))
            {
                MessageBox.Show("Книга уже сдана!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cellValue = Convert.ToString(selectedRow.Cells[0].Value);
            string bookAuthor = Convert.ToString(selectedRow.Cells[1].Value);
            string bookvalue = Convert.ToString(selectedRow.Cells[2].Value);
            string datEst = Convert.ToString(selectedRow.Cells[3].Value);
            string email_sent = Convert.ToString(selectedRow.Cells[6].Value);
            MailAddress from = new MailAddress("b5hlbv33v571b6@lenta.ru", "МБУ Библиотека-филиал №13");
            // кому отправляем
            MailAddress to = new MailAddress(email_sent);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Здравствуйте! Вам необходимо сдать книгу!";
            // текст письма
            m.Body = String.Format("Вы должны сдать: <h2>{0}-{1}-{2}</h2>",bookAuthor, bookvalue, datEst);
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.rambler.ru");
            // логин и пароль
            smtp.Credentials = new NetworkCredential("b5hlbv33v571b6@lenta.ru", "Z!Y93Xidz0@@");
            //smtp.EnableSsl = true;
            emailRecieved= true;
            try {
                smtp.Send(m);
                smtp.Dispose();
            }
            catch(SmtpException ex) {

                SmtpStatusCode status = ex.StatusCode;
                emailRecieved= false;
                MessageBox.Show("Ошибка отправления!\nПроверьте подключение к сети\nи попробуйте ещё раз", status.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 
                
            }
            if(emailRecieved)
            {
                MessageBox.Show("Письмо успешно отправлено читателю!", "Выполнено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0) { 
            
                button3.Enabled= true;
            }
            else
            {
                button3.Enabled= false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
