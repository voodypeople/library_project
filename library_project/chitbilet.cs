using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Client.Result;
using System.Data.SQLite;
using Word = Microsoft.Office.Interop.Word;
using ZXing.QrCode;
using ZXing.Common;
using Microsoft.Office.Interop.Word;
using System.Drawing.Text;

namespace library_project
{
    public partial class chitbilet : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private ZXing.BarcodeReader reader;
        private bool selected_tbox = false;
        private string[] splitstr;
        PrivateFontCollection font;
        private string chitNumber;
        public chitbilet()
        {
            InitializeComponent();
            this.Text = "Сканирование QR-кода в базу данных";

            
            fontsProjects();
            //Применяем шрифты к компонентам
            fonts();
            btn_disconnect.Enabled=false;
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
            label1.Font = new System.Drawing.Font(font.Families[0], 8);//Alice.ttf
            label2.Font = new System.Drawing.Font(font.Families[0], 8);//Modestina.ttf
            label3.Font = new System.Drawing.Font(font.Families[0], 8);//serp_and_molot.ttf
            label4.Font = new System.Drawing.Font(font.Families[0], 8);
            label5.Font = new System.Drawing.Font(font.Families[0], 8);
            button1.Font = new System.Drawing.Font(font.Families[0], 9);
            btn_disconnect.Font = new System.Drawing.Font(font.Families[0], 9);
            button2.Font = new System.Drawing.Font(font.Families[0], 10);
            button3.Font = new System.Drawing.Font(font.Families[0], 9);
            button4.Font = new System.Drawing.Font(font.Families[0], 10);
            button5.Font = new System.Drawing.Font(font.Families[0], 8);
            действияToolStripMenuItem.Font = new System.Drawing.Font(font.Families[0], 8);
            оПрограммеToolStripMenuItem.Font = new System.Drawing.Font(font.Families[0], 8);

        }
        delegate void SetStringDelegate(String parameter);
        void SetResult(string result)
        {
            if(!InvokeRequired)
            {
               if(textBox1.Text == "" && !result.Contains('-')){
                    textBox1.Text = result;
                    return;
               }
               if(result.Contains('-')){
                    splitstr = result.Split('-');
                    tbText.Text = splitstr[0];
                    textBox2.Text = splitstr[1];
                    textBox3.Text= splitstr[2];
                    return;
                }
                
            }
            else
            {
                Invoke(new SetStringDelegate(SetResult), new object[] { result });
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(lbCams.Items.Count == 0)
            {
                MessageBox.Show("Камеры отсутствуют в списке\nУбедитесь, что камера подключена\nи перезайдите в приложение!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            videoSource = new VideoCaptureDevice(videoDevices[lbCams.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
            button1.Enabled = false;
            btn_disconnect.Enabled = true;
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;

            ZXing.Result result = reader.Decode((Bitmap)eventArgs.Frame.Clone());
            if(result != null)
            {
                SetResult(result.Text);
            }
        }

        private void chitbilet_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            reader = new ZXing.BarcodeReader();
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
            if(videoDevices.Count > 0)
            {
                foreach(FilterInfo device in videoDevices)
                {
                    lbCams.Items.Add(device.Name);
                }
                lbCams.SelectedIndex = 0;
            }
        }

        private void chitbilet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(videoSource != null)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Не введен номер читательского билета!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                
            }
            if(textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Не введенны данные книги!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            SQLiteConnection con;
            SQLiteCommand cmd;
            

            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + Environment.CurrentDirectory.ToString() + "\\employers.db" + "; New=False;Version=3";
            //con.ConnectionString = @"Data Source="\\employers.db" + "; New=False;Version=3";
            cmd = new SQLiteCommand();
            cmd.Connection = con;
            con.Open();

            string sqlExpression = String.Format("select number from readers where number == '{0}'", textBox1.Text);

            using (con = new SQLiteConnection("Data Source=employers.db"))
            {
                con.Open();

                SQLiteCommand command = new SQLiteCommand(sqlExpression, con);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            chitNumber = Convert.ToString(reader[0]); //Логин и пароль из бд

                        }
                    }
                }
            }


            if (chitNumber != textBox1.Text)
            {

                MessageBox.Show("Читатель на зарегестрирован в системе!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            else
            {
                //cmd.CommandText = string.Format("INSERT INTO aboniment(number, book, dateStart, expired) VALUES('{0}', '{1}', date(), 0);", textBox1.Text, tbText.Text);
                cmd.CommandText = string.Format("INSERT INTO aboniment(number, bookAuthor, bookName, Datest, dateStart, expired) values('{0}', '{1}', '{2}', '{3}', date(), 0);", textBox1.Text, textBox2.Text, tbText.Text, textBox3.Text);

                //string sql = "INSERT INTO aboniment(number, book, dateStart, expired) VALUES(@userid, @book,@date, 1);";
                /*using (var command = new SQLiteCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@userid", textBox1.Text);
                    command.Parameters.AddWithValue("@book", tbText.Text);
                    command.Parameters.AddWithValue("@date", DateTime.Now);

                    command.ExecuteNonQuery();
                }*/

                cmd.ExecuteReader();
                //con.Close();
                Form2 form = new Form2();
                form.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа позволяет автоматизировать\nсистему ведения учёта книг библиотеки.", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void создатьЧитательскийБилетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!button1.Enabled)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox1.Image = null;
            }
                button1.Enabled = true;
            btn_disconnect.Enabled = false;
            chit_word form2 = new chit_word();
            form2.ShowDialog(this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if (textBox1.Text == "")
            {
                MessageBox.Show("Не отсканирован номер читательского билета!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            
            else
            {
                /*videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox1.Image = null;*/
                btn_disconnect.Enabled = false;
                button1.Enabled = true;

                Form2 fform = new Form2();
                fform.ChangeId(textBox1.Text);
                fform.ShowDialog();
            }
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            videoSource.SignalToStop();
            videoSource.WaitForStop();
            pictureBox1.Image = null;
            button1.Enabled = true;
            btn_disconnect.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chitbilet_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void действияToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            lbCams.Items.Clear();
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            reader = new ZXing.BarcodeReader();
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
            if (videoDevices.Count > 0)
            {
                foreach (FilterInfo device in videoDevices)
                {
                    lbCams.Items.Add(device.Name);
                }
                lbCams.SelectedIndex = 0;
            }
        }
    }
}

