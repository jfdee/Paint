using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PAINT
{
    public partial class Form1 : Form
    {
        Bitmap picture;
        int x1, y1;
        //Настройки формы
        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;

            menuFile();
            fillListBox();

            picture = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            x1 = y1 = 0;


        }

        //Заполнение листбокса с толщиной
        public void fillListBox()
        {
            for(int i = 0; i < 40; i++)
            {
                if(i % 2 == 0)
                {
                    listBox1.Items.Add(i);
                }
            }
        }
        public void menuFile ()
        {
            ToolStripMenuItem fileItem = new ToolStripMenuItem("Файл");

            ToolStripMenuItem openFile = new ToolStripMenuItem("Открыть");
            openFile.Click += openFile_Click;

            ToolStripMenuItem saveFile = new ToolStripMenuItem("Сохранить");
            saveFile.Click += saveFile_Click;

            ToolStripMenuItem settingsFile = new ToolStripMenuItem("Настройки");
            settingsFile.Click += settingsFile_Click;

            fileItem.DropDownItems.Add(openFile);
            fileItem.DropDownItems.Add(saveFile);
            fileItem.DropDownItems.Add(settingsFile);

            menuStrip1.Items.Add(fileItem);
        }

        //Открытие файла
        void openFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    picture = new Bitmap(ofd.FileName);
                    pictureBox1.Size = picture.Size;
                    pictureBox1.Image = picture;
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult result = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Сохранение файла
        void saveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfg = new SaveFileDialog();
            sfg.Title = "Сохранить как ...";
            sfg.OverwritePrompt = true;
            sfg.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)" + 
                "|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            sfg.ShowHelp = true;

            if(sfg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    picture.Save(sfg.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }



        }

        //Настройки
        void settingsFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Настройки");
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
        Pen pen1 = new Pen(Color.Black, 5);
        SolidBrush brush1 = new SolidBrush(Color.Black);
        String elementType;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(picture);
            if(e.Button == MouseButtons.Left)
            {
                using (Pen pen = new Pen(pen1.Color, pen1.Width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    if(elementType == "Brush")
                    {
                        g.DrawLine(pen, x1, y1, e.X, e.Y);
                    }                                

                    pictureBox1.Image = picture;
                }
            }
            x1 = e.X;
            y1 = e.Y;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        //Очистка
        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Graphics g = Graphics.FromImage(picture);
                g.Clear(Color.White);
                pictureBox1.Invalidate();
            }
        }

        //Выбор толщины
        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            pen1.Width = Convert.ToInt32(listBox1.SelectedItem);
        }


        private void ElementList_MouseClick(object sender, MouseEventArgs e)
        {
            elementType = Convert.ToString(ElementList.SelectedItem);
        }


        Point startPoint = new Point();
        List<int> screenBuffer = new List<20>;
        void floodFill(int w, int h, int x, int y, Color newColor, Color oldColor)
        {
            if (x >= 0 && x < w && y >= 0 && y < h && screenBuffer[y][x] == oldColor && screenBuffer[y][x] != newColor)
            {
                screenBuffer[y * w + x] = newColor; //set color before starting recursion!

                floodFill(w, h, x + 1, y, newColor, oldColor);
                floodFill(w, h, x - 1, y, newColor, oldColor);
                floodFill(w, h, x, y + 1, newColor, oldColor);
                floodFill(w, h, x, y - 1, newColor, oldColor);
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(elementType != "Brush")
            {
                startPoint = e.Location;
            }
            if (elementType == "Filling")
            {
                int w = picture.Width;
                int h = picture.Height;
                Color clickedColor = picture.GetPixel(e.X, e.Y);
                floodFill(w, h, e.X, e.Y, pen1.Color ,clickedColor);
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if(elementType == "Rectangle")
            {
                Graphics g = Graphics.FromImage(picture);
                g.DrawRectangle(pen1, startPoint.X, startPoint.Y, e.X, e.Y);
            }

            if(elementType == "Ellipse")
            {
                Graphics g = Graphics.FromImage(picture);
                g.DrawEllipse(pen1, startPoint.X, startPoint.Y, e.X, e.Y);
            }
            pictureBox1.Image = picture;
        }

        private void FillingButton_Click(object sender, EventArgs e)
        {
            elementType = "Filling";
        }

        public void SetPixel(int x, int y)
        {
            picture.SetPixel(x, y, pen1.Color);
        }

        public bool checkPixel(int x, int y, Color clickedColor)
        {
            Color current = picture.GetPixel(x, y);
            if(clickedColor == current)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void RecursiveFill(int x, int y, Color clickedColor)
        {
            picture = new Bitmap(pictureBox1.Image);

            //Если дошли до границы, то выходим
            if (checkPixel(x, y, clickedColor) == true)
            {
                return;
            }

            //Иначе меняем цвет пикселя на нужный

            SetPixel(x, y);
            
            //идем в глубь рекурсии по новому алгоритму
            /*int i=yCoordInt + 1;
            while (i < hsv_frame.height && CompareBool(xCoordInt, i))
            {
                RecursiveFill(xCoordInt, i);
                i++;
            }
            RecursiveFill(xCoordInt, yCoordInt - 1);
            RecursiveFill(xCoordInt - 1, yCoordInt);
            RecursiveFill(xCoordInt + 1, yCoordInt);*/

            //Идём в глубь рекурсии
            RecursiveFill(x, y + 1, clickedColor);
            RecursiveFill(x, y - 1, clickedColor);
            RecursiveFill(x - 1, y, clickedColor);
            RecursiveFill(x + 1, y, clickedColor);

        }

        


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }



        //Выбор цвета
        private void ColorButton_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            pen1.Color = colorDialog1.Color;
            brush1.Color = colorDialog1.Color;
        }
    }
}
