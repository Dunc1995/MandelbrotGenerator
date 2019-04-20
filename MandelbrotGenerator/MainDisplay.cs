using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace MandelbrotGenerator
{
    public partial class MainDisplay : Form
    {
        public int pictureWidth { get; }
        public int pictureHeight { get; }

        public MainDisplay()
        {
            int scaleFactor = 16;

            InitializeComponent();
            pictureWidth = pictureBox1.Width * scaleFactor;
            pictureHeight = pictureBox1.Height * scaleFactor;         

            Bitmap image = new Bitmap(pictureWidth, pictureHeight);
            Random rand = new Random();

            for (int x = 0; x < pictureWidth; x++)
            {
                for (int y = 0; y < pictureHeight; y++)
                {
                    int a = 255;
                    
                    PointF point = GetFloatingPointValue(x, y);
                    int sample = IterateValue(point);

                    double trigScaling = 0.0246;

                    int r = (int)Math.Round(Math.Sin(trigScaling * sample - 0) * 127 + 128);
                    int g = (int)Math.Round(Math.Sin(trigScaling * sample - (2*Math.PI/3)) * 127 + 128);
                    int b = (int)Math.Round(Math.Sin(trigScaling * sample - (4*Math.PI/3)) * 127 + 128);

                    image.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            image.Save(@"C:\Users\Duncan\Desktop\Mandelbrot.bmp");
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

            int width = pictureWidth;
            int height = pictureHeight;
            int minorViewportDimension = (width > height) ? height : width;

            float scalingValue = minorViewportDimension / 3;

            float centreX = width / 2;
            float centreY = height / 2;

            float scaledX = (pixelX - centreX) / scalingValue;
            float scaledY = (pixelY - centreY) / scalingValue;

            return new PointF(scaledX, scaledY);
        }

        public int IterateValue(PointF complexPoint)
        {
            int count = 0;
            Complex complexNumber = new Complex(complexPoint.X, complexPoint.Y);

            Complex iteratedComplexNumber = complexNumber;

            while (count < 255 && !iteratedComplexNumber.Magnitude.Equals(0) && iteratedComplexNumber.Magnitude < 2)
            {
                iteratedComplexNumber = Complex.Add(
                    Complex.Pow(iteratedComplexNumber, 2), 
                    complexNumber);
                count++;
            }

            return count;
        }
    }
}
