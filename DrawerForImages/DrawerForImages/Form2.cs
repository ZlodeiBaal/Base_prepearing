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

namespace DrawerForImages
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            ps.TypeOrBrush = 1;
            ps.Radius = 15;
        }
        //Палитра
        public class PaletteState
        {
            public int TypeOrBrush;
            public int Radius;
            //Каждый цвет - тип
            public byte[] GetColores()
            {
                byte[] C = new byte[3];
                switch (TypeOrBrush)
                {
                    case 1:
                        C[0] = 255;
                        C[1] = 0;
                        C[2] = 0;
                        break;
                    case 2:
                        C[0] = 0;
                        C[1] = 0;
                        C[2] = 255;
                        break;
                    case 3:
                        C[0] = 0;
                        C[1] = 255;
                        C[2] = 0;
                        break;
                    default:
                        C[0] = 0;
                        C[1] = 0;
                        C[2] = 0;
                        break;
                }
                return C;
            }
        }
        public bool wantnext = false;
        public bool wantprev = false;
        public PaletteState ps = new PaletteState();
        public Image<Bgr, Byte> I;
        public Image<Bgr, Byte> J;
        public Image<Bgr, Byte> K;
        //Загружаем картинку
        public void LoadImg(string toload)
        {
            I = new Image<Bgr, byte>(toload);
            pictureBox1.Image = I.ToBitmap();
            J = I.Clone();
            K = new Image<Bgr, byte>(J.Size);
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
        //Кнопки управления
        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string s = e.KeyChar.ToString();
            switch (s)
            {
                case "1":
                    ps.TypeOrBrush = int.Parse(s);
                    break;
                case "2":
                    ps.TypeOrBrush = int.Parse(s);
                    break;
                case "3":
                    ps.TypeOrBrush = int.Parse(s);
                    break;
                case "+":
                    ps.Radius = (int)(ps.Radius * 1.5);
                    break;
                case "=":
                    ps.Radius = (int)(ps.Radius * 1.5);
                    break;
                case "-":
                    ps.Radius = (int)(ps.Radius / 1.5);
                    break;
                case "\b":
                    wantprev = true;
                    break;
                case " ":
                    wantnext = true;
                    break;
                case "0":
                    ps.TypeOrBrush = int.Parse(s);
                    break;

            }
        }
        bool clickon = false;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            clickon = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            AddBrush(e);
        }
        //Дорисуем кисточку по координате x, y
        private void AddBrush(MouseEventArgs e)
        {
            if (clickon)
            {
                int R = ps.Radius;
                byte[] C = ps.GetColores();
                int xpos = (int)(pictureBox1.Image.Width * (double)e.X / (double)pictureBox1.Width);
                int ypos = (int)(pictureBox1.Image.Height * (double)e.Y / (double)pictureBox1.Height);
                for (int x = Math.Max(xpos - R, 0); x < Math.Min(pictureBox1.Image.Width - 1, xpos + R + 1); x++)
                    for (int y = Math.Max(0, ypos - R); y < Math.Min(pictureBox1.Image.Height - 1, ypos + R + 1); y++)
                        if (Math.Sqrt((x - xpos) * (x - xpos) + (y - ypos) * (y - ypos)) <= R)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (C[j] != 0)
                                    J.Data[y, x, j] = 255;
                                else
                                    J.Data[y, x, j] = I.Data[y, x, j];
                                K.Data[y, x, j] = (byte)ps.TypeOrBrush;
                            }
                        }
            }
        }
        public void ReDraw()
        {
            pictureBox1.Image = J.ToBitmap();
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            AddBrush(e);
            clickon = false;
            pictureBox1.Image = J.ToBitmap();
        }

    }
}
