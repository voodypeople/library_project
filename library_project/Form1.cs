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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Data.SqlClient;
using System.Net;
using System.Data.Sql;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Text;

namespace library_project
{
    public partial class Form1 : Form
    {
        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;
        string login;
        string pass;
        private bool passchar = true;
        PrivateFontCollection font;
        public Form1()
        {
            
            InitializeComponent();
            
            this.Text = "Авторизация";
            //textBox2.PasswordChar = '•';
            textBox2.UseSystemPasswordChar= true;
            fontsProjects();
            //Применяем шрифты к компонентам
            fonts();
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();

            dbFileName = "D:/employers_db/employers.db";

        }

        private void fontsProjects()
        {
            //Добавляем шрифт из указанного файла в em.Drawing.Text.PrivateFontCollection
            this.font = new PrivateFontCollection();
            this.font.AddFontFile(Environment.CurrentDirectory.ToString() + "\\Fonts\\Bowler.ttf");
            //this.font.AddFontFile("New Folder #1\\Bowler.ttf");
        }

        private void fonts()
        {
            //Задаем шрифт текста, отображаемого элементом управления.
            label1.Font = new Font(font.Families[0], 15);//Alice.ttf
            label2.Font = new Font(font.Families[0], 10);//Modestina.ttf
            label3.Font = new Font(font.Families[0], 10);//serp_and_molot.ttf
            button1.Font = new Font(font.Families[0], 10);
            //checkBox1.Font = new Font(font.Families[0], 5);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "")
            {
                MessageBox.Show("Не введен логин и пароль!\nЕсли он отсутствует,\nобратитесь к администратору!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("Не введён логин!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Не введён пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return;
            }
            
            //m_dbConn = new SQLiteConnection(@"Data Source= D:\employers_db\employers.db; Version=3;");
            //m_dbConn.Open();
            
            string sqlExpression = String.Format("SELECT * FROM workers where login == '{0}' and password == '{1}';", textBox1.Text, textBox2.Text);
            //
            //using (m_dbConn = new SQLiteConnection("Data Source=New Folder #1\\employers.db"))
            using (m_dbConn = new SQLiteConnection("Data Source=" + Environment.CurrentDirectory.ToString() +"\\employers.db"))
            {
                m_dbConn.Open();
                SQLiteCommand command = new SQLiteCommand(sqlExpression, m_dbConn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            login = Convert.ToString(reader[0]); //Логин и пароль из бд
                            pass = Convert.ToString(reader[1]);  
                        
                        }
                    }
                }
            }
            if(login == textBox1.Text && pass == textBox2.Text) //Проверка введенных данных                                                   //логина и пароля в бд
            {
                this.Hide();
                chitbilet ch = new chitbilet();
                ch.ShowDialog();
                this.Close();
            }
            else
                MessageBox.Show("Пользователь не найден в системе!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        /*string query = "select login,  from user WHERE username ='" + txtUsername.Text + "' AND password ='" + txtPassword.Text + "'";
        SQLiteCommand cmd = new SQLiteCommand(query);
        if (row.HasRows)
        {
            while (row.Read())
            {

                login = row["login"].ToString();
                password = row["password"].ToString();

            }
            MessageBox.Show("Data found your name is " + firstname + " " + lastname + " " + " and your address at " + address);
        }
        else
        {
            MessageBox.Show("Data not found", "Information");
        }
    }*/
        /*string stm = "SELECT login FROM workers where password = 12345678";

        using (SQLiteConnection Connect = new SQLiteConnection(@"D:\\employers_db\\employers.db; Version=3;"))
        {
            string commandText = "CREATE TABLE IF NOT EXISTS [dbTableName] ( [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [image] BLOB, [image_format] VARCHAR(10), " +
                "[image_name] NVARCHAR(128), [file] BINARY, [file_format] VARCHAR(10), [file_name] NVARCHAR(128))";
            SQLiteCommand Command = new SQLiteCommand(stm, Connect);
            Connect.Open();
            //int i = Command.ExecuteNonQuery();
            //Console.WriteLine(i);
            //Connect.Close();
        */
        /*if (m_dbConn.State == ConnectionState.Open)
    {
        MessageBox.Show("Open connection with database");
        return;
    }*/
        //chitbilet window = new chitbilet();
        //window.ShowDialog();

    }
}
    

