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
                    int a = 255;
                    int r = 255;
                    int g = 255;
                    int b = 255;
                    
                    PointF point = GetFloatingPointValue(x, y);

                    double Magnitude = Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
                    if (Magnitude >= 2)
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }

                    image.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            pictureBox1.Image = image;
        }

        /// <summary>
        /// Translate pixel coordinates to dimensions within the complex plane.
        /// Magnitudes greater than 2 will not be within the Mandelbrot Set, therefore
        /// this determines the bounds within the pixel dimensions.
        /// </summary>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
        /// <returns></returns>
        public PointF GetFloatingPointValue(int pixelX, int pixelY)
        {

            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            int minorViewportDimension = (width > height) ? height : width;

            float scalingValue = minorViewportDimension / 4;

            float centreX = width / 2;
            float centreY = height / 2;

            float scaledX = (pixelX - centreX) / scalingValue;
            float scaledY = (pixelY - centreY) / scalingValue;

            return new PointF(scaledX, scaledY);
        }
    }
}
