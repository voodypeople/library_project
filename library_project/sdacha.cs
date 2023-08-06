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
using System.Reflection.Emit;
using System.Data.SqlClient;
using System.Data.SqlClient;

namespace library_project
{
    public partial class sdacha : Form
    {
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        private string[] SplitBook;
        PrivateFontCollection font;
        private List<string> list_sdacha;
        private List<string> list_bookName;
        private List<string> list_bookAuthor;
        public sdacha()
        {
            InitializeComponent();
            //cmd = new SQLiteCommand();
            this.Text = "Сдача книги";
            fontsProjects();
            //Применяем шрифты к компонентам
            fonts();
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
            label1.Font = new System.Drawing.Font(font.Families[0], 9);//Alice.ttf
            label2.Font = new System.Drawing.Font(font.Families[0], 9);
            label3.Font = new System.Drawing.Font(font.Families[0], 9);
            //label2.Font = new System.Drawing.Font(font.Families[0], 10);//Modestina.ttf
            //label3.Font = new System.Drawing.Font(font.Families[0], 10);//serp_and_molot.ttf
            button1.Font = new System.Drawing.Font(font.Families[0], 8);
            button2.Font = new System.Drawing.Font(font.Families[0], 8);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Введите все данные в поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";
            con.Open();
            string sqlExpression = String.Format("UPDATE aboniment set expired = 1 where number == '{0}' and bookName == '{1}' and bookAuthor like '{2}%'", comboBox1.Text, comboBox2.Text, comboBox3.SelectedItem.ToString());
            SQLiteCommand command = new SQLiteCommand(sqlExpression, con);
            //cmd.CommandText = String.Format("UPDATE aboniment set expired = 1 where number == '{0}' and bookName == '{1}' and bookAuthor like '{2}%'", comboBox1.Text, comboBox2.Text, textBox3.Text);
            //cmd = new SQLiteCommand(sqlExpression); 
            //cmd.CommandText= sqlExpression;
            
            //cmd.CommandText = String.Format("update aboniment set expired = 1 where number == {0};", textBox1.Text);
            //cmd.CommandText = String.Format("UPDATE aboniment set expired = 1 where number == '{0}' and bookAuthor == '{1}' and bookName like '{2}%'", comboBox1.Text, comboBox2.Text, textBox3.Text);
            command.ExecuteReader();

            MessageBox.Show("Изменения были внедрены!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        
        }

        private void sdacha_Load(object sender, EventArgs e)
        {
            list_sdacha = new List<string>();
            list_bookName = new List<string>();
            list_bookAuthor = new List<string>();
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";

            //cmd = new SQLiteCommand();
            //cmd.Connection = con;
            //con.Open();

            // cmd.CommandText = "select readers.number from aboniment join readers on aboniment.number == readers.number where expired == 0";
            string sqlExpression = "select readers.number from aboniment join readers on aboniment.number == readers.number where expired == 0";

            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(sqlExpression, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        //list_sdacha.Add(Convert.ToString(reader["number"]));
                        //reader.NextResult();
                        while (reader.Read())   // построчно считываем данные
                        {
                            list_sdacha.Add(Convert.ToString(reader["number"]));

                            //list_sdacha.Add(reader.GetString(0));
                            //chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
                //con.Close();
            }
            //}
            var sorted_list = list_sdacha.Distinct();
            foreach(string str in sorted_list) 
                comboBox1.Items.Add(str);
            //comboBox1.SelectedIndex = 0;

            

            string query_bookName = "select bookName from aboniment join readers on aboniment.number == readers.number where expired == 0";

            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(query_bookName, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        //list_sdacha.Add(Convert.ToString(reader["number"]));
                        //reader.NextResult();
                        while (reader.Read())   // построчно считываем данные
                        {
                            list_bookName.Add(Convert.ToString(reader["bookName"]));
                            //list_sdacha.Add(reader.GetString(0));
                            //chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
                //con.Close();
            }
            foreach(string str in list_bookName)
                comboBox2.Items.Add(str);
            //comboBox1.Items.Add(list_sdacha);
        }
            //cmd.ExecuteReader();
        

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //list_bookName.Clear();
            
            list_bookAuthor.Clear();
            list_bookName.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            //comboBox2.SelectedItem = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            //comboBox3.SelectedItem = "";
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";

            string query_bookName = String.Format("select bookName from aboniment join readers on aboniment.number == readers.number and readers.number == '{0}' and expired == 0;", comboBox1.Text);

            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(query_bookName, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        //list_sdacha.Add(Convert.ToString(reader["number"]));
                        //reader.NextResult();
                        while (reader.Read())   // построчно считываем данные
                        {
                            list_bookName.Add(Convert.ToString(reader["bookName"]));
                            //list_sdacha.Add(reader.GetString(0));
                            //chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
                //con.Close();
            }
            
            foreach (string str in list_bookName)
                comboBox2.Items.Add(str);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_bookAuthor.Clear();
            comboBox3.Items.Clear();
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";

            string query_bookName = String.Format("select bookAuthor from aboniment join readers on aboniment.number == readers.number and readers.number == '{0}' and expired == 0 and bookName == '{1}';", comboBox1.Text, comboBox2.Text);

            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(query_bookName, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        //list_sdacha.Add(Convert.ToString(reader["number"]));
                        //reader.NextResult();
                        while (reader.Read())   // построчно считываем данные
                        {
                            //list_bookName.Add(Convert.ToString(reader["bookName"]));
                            list_bookAuthor.Add(Convert.ToString(reader["bookAuthor"]));
                            //list_sdacha.Add(reader.GetString(0));
                            //chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
                //con.Close();
            }

            foreach(var i in list_bookAuthor)
                comboBox3.Items.Add(i);

        }
    }
}
