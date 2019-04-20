using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotGenerator
{
    public partial class MainDisplay : Form
    {
        public MainDisplay()
        {
            InitializeComponent();
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            Bitmap image = new Bitmap(width, height);
            Random rand = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int a = rand.Next(256);
                    int r = rand.Next(256);
                    int g = rand.Next(256);
                    int b = rand.Next(256);

                    image.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            pictureBox1.Image = image;
        }
    }
}
