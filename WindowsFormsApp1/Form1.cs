using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{

    public partial class MainForm : Form
    {
        Bitmap img;
        public MainForm()
        {
            InitializeComponent();
        }

        private void openbutton_Click(object sender, EventArgs e)
        {

        }

        public void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form2 f2 = new Form2())
            {
                f2.ShowDialog(this);
            }
        }

        public void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG) |*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG |All files (*.*) |*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                try
                {

                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    //pictureBox1.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);

                    pictureBox2.Image = pictureBox1.Image;

                    img = new Bitmap(ofd.FileName);
                    Histo(img, chart1);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void сохранитьФайлКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Created)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "*.bmp";
                sfd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG) |*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG |All files (*.*) |*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string path = sfd.FileName;
                    pictureBox2.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                MessageBox.Show("Файл сохранен", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Невозможно сохранить выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                equalizing(img);
                pictureBox2.Image = (Bitmap)img;
                Histo(img, chart2);
            }
            else
            {
                MessageBox.Show("Не загружено изображение, пожалуйста загрузите изображение для обработки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Bitmap copy = new Bitmap(img);
            //copy = img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
            //bmstackz.Push(copy);
            //bmstackr.Clear();
        }

        public Bitmap equalizing(Bitmap Bmp)
        {
            Rectangle rect = new Rectangle(0, 0, Bmp.Width, Bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = Bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, Bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * Bmp.Height;
            byte[] grayValues = new byte[bytes];
            int[] R = new int[256];
            byte[] N = new byte[256];
            byte[] left = new byte[256];
            byte[] right = new byte[256];
            System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
            for (int i = 0; i < grayValues.Length; i++) ++R[grayValues[i]];
            int z = 0;
            int Hint = 0;
            int Havg = grayValues.Length / R.Length;
            for (int i = 0; i < N.Length - 1; i++)
            {
                N[i] = 0;
            }
            for (int j = 0; j < R.Length; j++)
            {
                if (z > 255) left[j] = 255;
                else left[j] = (byte)z;
                Hint += R[j];
                while (Hint > Havg)
                {
                    Hint -= Havg;
                    z++;
                }
                if (z > 255) right[j] = 255;
                else right[j] = (byte)z;

                N[j] = (byte)((left[j] + right[j]) / 2);
            }
            for (int i = 0; i < grayValues.Length; i++)
            {
                if (left[grayValues[i]] == right[grayValues[i]]) grayValues[i] = left[grayValues[i]];
                else grayValues[i] = N[grayValues[i]];
            }

            System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
            Bmp.UnlockBits(bmpData);
            return Bmp;
        }

        public Bitmap Histo(Bitmap Bmp, Chart chart11)
        {
            if (Bmp != null)
            {
                Rectangle rect = new Rectangle(0, 0, Bmp.Width, Bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData = Bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, Bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = bmpData.Stride * Bmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                int[] R = new int[256];
                for (int j = 0; j < grayValues.Length; ++j)
                {
                    ++R[grayValues[j]];
                }
                int max = 0;
                for (int i = 0; i < R.Length; i++)
                {
                    if (max < R[i]) max = R[i];
                }
                for (int s = 0; s < 256; s++)
                {
                    chart11.Series[0].Points.Add(R[s]);   //нормировка диаграммы по оси Y не выполнена!   
                }

                Bmp.UnlockBits(bmpData);
                chart11.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart11.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart11.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
                chart11.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
                chart11.ChartAreas[0].AxisY.Interval = 2500;
                chart11.ChartAreas[0].AxisX.Interval = 10;
                
            }
            return Bmp;

        }

    }
}
