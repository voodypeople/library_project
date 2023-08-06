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
using Microsoft.Office.Interop.Word;
using System.Drawing.Text;
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace library_project
{
    public partial class deleteHistory : Form
    {
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        PrivateFontCollection font;
        private List<string> List_to_delete;
        public deleteHistory()
        {
            InitializeComponent();
            this.Text = "Удалить читателя из базы данных";
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
            button1.Font = new System.Drawing.Font(font.Families[0], 8);
            button2.Font = new System.Drawing.Font(font.Families[0], 8);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List_to_delete= new List<string>();
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";
            string query = String.Format("select number from aboniment where number == {0}", textBox1.Text);
            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(query, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        //list_sdacha.Add(Convert.ToString(reader["number"]));
                        //reader.NextResult();
                        while (reader.Read())   // построчно считываем данные
                        {
                            //list_bookName.Add(Convert.ToString(reader["bookName"]));
                            List_to_delete.Add(Convert.ToString(reader["number"]));
                            //list_sdacha.Add(reader.GetString(0));
                            //chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
                //con.Close();
            }


            if (List_to_delete.Capacity == 0)
            {
                MessageBox.Show("Не найден читатель\nс данным номером билета!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                cmd.CommandText = String.Format("Delete from aboniment where number == '{0}'", textBox1.Text);
                cmd.ExecuteReader();
                MessageBox.Show(String.Format("Читатель с номером {0} удалён!", textBox1.Text), "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            
        }

        private void deleteHistory_Load(object sender, EventArgs e)
        {
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + "employers.db" + ";New=False;Version=3";

            cmd = new SQLiteCommand();
            cmd.Connection = con;
            con.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
