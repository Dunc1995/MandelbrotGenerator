using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

namespace MandelbrotGenerator
{
    public class MandelbrotUtils
    {
        /// <summary>
        /// Takes an empty bitmap, and corresponding bounds in the imaginary plane to generate the Mandelbrot Set, pixel by pixel.
        /// </summary>
        /// <param name="inputBitmap"></param>
        /// <param name="iPlaneDimensions"></param>
        public static Bitmap GetMandelbrotImage(IPlaneBoundingRectangle iPlaneBoundingRectangle, double frequencyScale, double phaseOffset)
        {
            Bitmap NewImage = iPlaneBoundingRectangle.CurrentBitmap;
            ConcurrentBag<int[]> list = new ConcurrentBag<int[]>();
            int height = NewImage.Height;
            int width = NewImage.Width;

            Parallel.For(0, width, x =>
            {
                Parallel.For(0, height, y =>
                {
                    int a = 255;

                    Complex point = iPlaneBoundingRectangle.GetTranslatedCoordinates(new Point(x, y));
                    int iterationCount = GetIteratedIntegerValue(point);

                    double trigScaling = 0.0246;

                    int r = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset) * 127 + 128);
                    int g = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset - (2 * Math.PI / 3)) * 127 + 128);
                    int b = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset - (4 * Math.PI / 3)) * 127 + 128);
                    int[] val = { x, y, a, r, g, b };
                    list.Add(val);
                });
            });

            foreach (int[] entry in list)
            {
                NewImage.SetPixel(entry[0], entry[1], Color.FromArgb(entry[2], entry[3], entry[4], entry[5]));
            }

            return NewImage;
        }

        /// <summary>
        /// Executes iterative equation to determine stability. High integer values indicate instability (in very general terms).
        /// </summary>
        /// <param name="scaledX"></param>
        /// <param name="scaledY"></param>
        /// <returns></returns>
        private static int GetIteratedIntegerValue(Complex complexNumber)
        {
            int count = 0;

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
