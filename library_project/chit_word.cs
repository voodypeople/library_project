using Microsoft.Office.Interop.Word;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;
using ZXing;
using Word = Microsoft.Office.Interop.Word;

namespace library_project
{
    public partial class chit_word : Form
    {
        private ZXing.QrCode.QRCodeWriter qr;
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        private string chitNumber;
        PrivateFontCollection font;
        public chit_word()
        {
            InitializeComponent();
            
            ToolTip Generation_code_tip = new ToolTip();
            Generation_code_tip.SetToolTip(button2, "Сгенерировать номер");

            ToolTip Clear_textbox = new ToolTip();
            Clear_textbox.SetToolTip(button4, "Очистить поля");

            button3.Visible = false;
            this.Text = "Создать читательский билет";
            //textBox3.
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
            label2.Font = new System.Drawing.Font(font.Families[0], 9);//Modestina.ttf
            label3.Font = new System.Drawing.Font(font.Families[0], 9);
            label4.Font = new System.Drawing.Font(font.Families[0], 9);
            label5.Font = new System.Drawing.Font(font.Families[0], 9);
            //label3.Font = new System.Drawing.Font(font.Families[0], 10);//serp_and_molot.ttf
            button1.Font = new System.Drawing.Font(font.Families[0], 10);
            button3.Font = new System.Drawing.Font(font.Families[0], 10);

        }

        public void generateQR(string text)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;

            //QRCodeWriter w = new QRCodeWriter();
            ZXing.Common.BitMatrix p = writer.Encode(text);
            //ZXing.Common.BitMatrix color32 = qr.encode(text, BarcodeFormat.QR_CODE, 255, 255);
            Bitmap bp = writer.Write(p);


            //Image encoded = (Image)bp;

            string FileName = Environment.CurrentDirectory.ToString() + "\\QR\\" + text + ".png";
            pb_qrcode.Image = bp;
            bp.Save(FileName, ImageFormat.Png);

        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress mail = new MailAddress(email);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
            private void button1_Click(object sender, EventArgs e)
            {
            char check_fio;
            if (textBox1.Text == "" || textBox2.Text == "" || maskedTextBox1.Text == "")
            {
                MessageBox.Show("Введите все данные в поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (char.TryParse(textBox1.Text, out check_fio))
            {
                MessageBox.Show("Введите ФИО корректно!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!IsValidEmail(textBox3.Text))
            {
                MessageBox.Show("Введите email верно!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            string sqlExpression = String.Format("SELECT number FROM readers where number == '{0}'", textBox1.Text);
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

            if (chitNumber == textBox1.Text)
            {
                MessageBox.Show("Пользователь с таким номером уже зарегестрирован!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                using (con = new SQLiteConnection("Data Source=employers.db"))
                {
                    con.Open();
                    string sql_command = String.Format("insert into readers(number, email, fio, phone) values('{0}', '{1}', '{2}', '{3}')", textBox1.Text, textBox3.Text, textBox2.Text, maskedTextBox1.Text);
                    SQLiteCommand command = new SQLiteCommand(sql_command, con);
                    command.ExecuteReader();

                }

                generateQR(textBox1.Text);
                Word._Application word_app = new Word.Application();

                // Сделать Word видимым (необязательно).

                //word_app.Visible = true;


                // Создаем документ Word.

                object missing = Type.Missing;

                Word._Document word_doc = word_app.Documents.Add(

                    ref missing, ref missing, ref missing, ref missing);
                Word.Paragraph para = word_doc.Paragraphs.Add(ref missing);



                para.Range.Text = "Читательский билет №" + textBox1.Text;

                object style_name = "Заголовок 1";

                para.Range.set_Style(ref style_name);

                para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                para.Range.InsertParagraphAfter();
                para.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                para.Range.Text = "\nГородская библиотека - филиал № 13\nАдрес: г. Набережные Челны,ул.Татарстан,12,кв.146-148\nТелефон:(8552) 56-36-40\n";
                //para.Range.Bold = 1;


                para.Range.InsertParagraphAfter();
                //para.Range.ParagraphFormat.Alignment = Word.wdalignwdAlignParagraphRight;

                // Добавить текст.
                para.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                para.Range.Text = textBox2.Text;
                para.Range.Bold = 1;
                para.Range.Font.Size = 15;
                para.Range.InsertParagraphAfter();
                para.Range.Text = "Дата рождения: " + dateTimePicker1.Text;
                para.Range.InsertParagraphAfter();
                para.Range.Text = "Номер телефона: " + maskedTextBox1.Text;
                para.Range.InsertParagraphAfter();
                para.Range.Text = "Электронная почта: " + textBox3.Text;
                para.Range.InsertParagraphAfter();
                //para.Range.InsertFile("D:\\frame.png");


                //object f = false;
                //object t = true;
                //object left = Type.Missing;
                //object top = Type.Missing;
                //object width = 300;
                //object height = 300;
                //object range = Type.Missing;
                //Microsoft.Office.Interop.Word.WdWrapType wrap = Microsoft.Office.Interop.Word.WdWrapType.wdWrapSquare;
                //Microsoft.Office.Core.ad Shapes.AddPicture("image.jpg", ref missing, ref t, ref left, ref top, ref width, ref height, ref range).WrapFormat.Type = wrap;

                //para.Range.InlineShapes.AddPicture("D:\\frame.png", ref missing, ref missing, ref missing);

                para.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                para.Range.InlineShapes.AddPicture(Environment.CurrentDirectory.ToString() + "\\QR\\" + textBox1.Text + ".png", ref missing, ref missing, ref missing);

                //word_doc.Shapes.AddPicture("D:\\frame.png");
                word_app.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                object filename = Path.GetFullPath(Environment.CurrentDirectory.ToString() + String.Format("\\Reader cards\\{0}.docx", textBox1.Text));
                //object filename = Path.GetFullPath(

                //Path.Combine(Application.StartupPath, "\\ test.docx"));
                word_doc.SaveAs(ref filename);
                object save_changes = false;

                if (word_doc.Saved)
                {
                    button3.Visible = true;
                    MessageBox.Show("Документ успешно сформирован!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                word_doc.Close(ref save_changes, ref missing, ref missing);

                word_app.Quit(ref save_changes, ref missing, ref missing);


            }
        }

        private void chit_word_Load(object sender, EventArgs e)
        {

            //con = new SQLiteConnection();
            // con.ConnectionString = @"Data Source=" + "D:\\employers_db\\employers.db" + ";New=False;Version=3";

            //cmd = new SQLiteCommand();
            //cmd.Connection = con;
            //con.Open();
            //string chitNumber = "";
            /*string sqlExpression = String.Format("SELECT number FROM aboniment where number == '{0}'", textBox1.Text);
            using (con = new SQLiteConnection("Data Source=D:\\employers_db\\employers.db"))
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

            }*/


            //textBox1.Text = rnd.Next(1, 100000).ToString();
        

    }

    private void button2_Click(object sender, EventArgs e)
    {
        generateQR("Privet");
    }

    private void button2_Click_1(object sender, EventArgs e)
    {
        Random rnd = new Random();
        string gen = rnd.Next(1, 100000).ToString();
        textBox1.Text = gen;
    }


    private void button3_Click(object sender, EventArgs e)
    {

        Process process = new Process();
        process.StartInfo.FileName = Environment.CurrentDirectory.ToString() + String.Format("\\Reader cards\\{0}.docx", textBox1.Text);
        process.Start();
        button3.Enabled = false;
        process.WaitForExit();
        if (process.HasExited)
        {
            button3.Enabled = true;
        }

        //Process.Start(Environment.CurrentDirectory.ToString() + "\\Reader cards\\test.docx");

    }

    private void button4_Click(object sender, EventArgs e)
    {
        textBox1.Clear();
        textBox2.Clear();
        maskedTextBox1.Clear();
        dateTimePicker1.Value = DateTime.Now;
    }

        private void button2_MouseHover(object sender, EventArgs e)
        {

        }
    }
}

