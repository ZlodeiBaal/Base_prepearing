using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Threading;
using System.IO;

namespace DrawerForImages
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            frm = new Form2();
            
        }
        Form2 frm;
        bool shown = false;
        FileInfo[] FI;
        int ennumerator = 0;
        //Старт
        private void button1_Click(object sender, EventArgs e)
        {
            //Открыли форму
            if (!shown)
            {
                shown = true;
                frm.Show();
            }
            //Загрузили всё из директоррии
            var dir = new DirectoryInfo(textBox1.Text);
            FI = dir.GetFiles();
            ennumerator = 0;
            if (FI.Length != 0)
            {
                //Загрузили первую картинку
                frm.LoadImg(FI[0].FullName);
                string s = textBox3.Text + "\\" + FI[ennumerator].Name.Substring(0, FI[ennumerator].Name.Length - 3) + "png";
                if (File.Exists(s))
                {
                    frm.K = new Image<Bgr, Byte>(s);
                    frm.I = new Image<Bgr, Byte>(FI[ennumerator].FullName);
                    frm.J = frm.I.Clone();
                    for (int x = 0; x < frm.I.Width; x++)
                        for (int y = 0; y < frm.I.Height; y++)
                        {
                            byte b = frm.K.Data[y, x, 0];
                            if (b != 0)
                            {
                                frm.ps.TypeOrBrush = b;
                                byte[] C = frm.ps.GetColores();
                                for (int j = 0; j < 3; j++)
                                    frm.J.Data[y, x, j] = C[j];
                            }
                        }
                    frm.ReDraw();
                }
            }
        }
        //Загрузим следующее изображение. Проверим, есть ли у нас уже его обработка.
        public void NEXTImage()
        {
            if (ennumerator < FI.Length)
            {
                string s = textBox3.Text + "\\" + FI[ennumerator].Name.Substring(0, FI[ennumerator].Name.Length - 3) + "png";
                frm.K.Save(s);
                ennumerator++;
                if (ennumerator < FI.Length)
                {
                    frm.LoadImg(FI[ennumerator].FullName);
                    s = textBox3.Text + "\\" + FI[ennumerator].Name.Substring(0, FI[ennumerator].Name.Length - 3) + "png";
                    if (File.Exists(s))
                    {
                        frm.K = new Image<Bgr, Byte>(s);
                        frm.I = new Image<Bgr, Byte>(FI[ennumerator].FullName);
                        frm.J = frm.I.Clone();
                        //Если нашли что у картинки уже есть образ раскрашенный, подгрузили его
                        for (int x = 0; x < frm.I.Width; x++)
                            for (int y = 0; y < frm.I.Height; y++)
                            {
                                byte b = frm.K.Data[y, x, 0];
                                if (b != 0)
                                {
                                    frm.ps.TypeOrBrush = b;
                                    byte[] C = frm.ps.GetColores();
                                    for (int j = 0; j < 3; j++)
                                        frm.J.Data[y, x, j] = C[j];
                                }
                            }
                        frm.ReDraw();
                    }

                }
            }
        }
        //Загрузим прошлое изображение
        public void PrevImage()
        {
            if (ennumerator > 0)
            {
                ennumerator--;
                string s = textBox3.Text + "\\" + FI[ennumerator].Name.Substring(0, FI[ennumerator].Name.Length - 3) + "png";
                frm.K = new Image<Bgr, Byte>(s);
                frm.I = new Image<Bgr, Byte>(FI[ennumerator].FullName);
                frm.J = frm.I.Clone();
                //Если нашли что у картинки уже есть образ раскрашенный, подгрузили его
                for (int x=0;x<frm.I.Width;x++)
                    for (int y=0;y<frm.I.Height;y++)
                    {
                        byte b =frm.K.Data[y,x,0];
                        if (b!=0)
                        {
                            frm.ps.TypeOrBrush = b;
                            byte[] C = frm.ps.GetColores();
                            for (int j = 0; j < 3; j++)
                                frm.J.Data[y, x, j] = C[j];
                        }
                    }
                frm.ReDraw();
            }
        }
        //Инициализируем всё
        private void Form1_Load(object sender, EventArgs e)
        {
            Image<Bgr, Byte> IP = new Image<Bgr, byte>(50, 50, new Bgr(255, 0, 0));
            pictureBox1.Image = IP.ToBitmap();
            IP = new Image<Bgr, byte>(50, 50, new Bgr(0, 0, 255));
            pictureBox2.Image = IP.ToBitmap();
            IP = new Image<Bgr, byte>(50, 50, new Bgr(0, 255, 0));
            pictureBox3.Image = IP.ToBitmap();
        }
        //Так как у нас 2 формы, 2 потока, вся синхронизация - тут:)
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (frm != null)
            {
                textBox2.Text = frm.ps.TypeOrBrush.ToString();
                numericUpDown1.Value = frm.ps.Radius;
                if (frm.wantnext)
                {
                    NEXTImage();
                    frm.wantnext = false;
                }
                if (frm.wantprev)
                {
                    PrevImage();
                    frm.wantprev = false;
                }
            }
        }
        //Изменяем тип кисточки. Каждый тип - один объект
        private void button2_Click(object sender, EventArgs e)
        {
            frm.ps.TypeOrBrush = 1;
        }
        //Изменяем тип кисточки. Каждый тип - один объект
        private void button3_Click(object sender, EventArgs e)
        {
            frm.ps.TypeOrBrush = 2;
        }
        //Изменяем тип кисточки. Каждый тип - один объект
        private void button4_Click(object sender, EventArgs e)
        {
            frm.ps.TypeOrBrush = 3;
        }
        //Изменяем тип кисточки. Каждый тип - один объект. Нулевой тип - принимаем отсутствие объекта. Им закрашшено всё по умолчанию
        private void button5_Click(object sender, EventArgs e)
        {
            frm.ps.TypeOrBrush = 0;
        }
        //Изменяем размер кисточки
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            frm.ps.Radius=(int)numericUpDown1.Value;
        }
        //Следующее изображение
        private void button7_Click(object sender, EventArgs e)
        {
            NEXTImage();
        }
        //Прошлое изображение
        private void button6_Click(object sender, EventArgs e)
        {
            PrevImage();
        }
    }
}
