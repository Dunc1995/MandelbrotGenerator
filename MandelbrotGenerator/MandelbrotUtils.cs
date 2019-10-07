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
        /// Compares dimensions in the imaginary place with dimensions
        /// of the input Picture Box dimensions and returns an empty Bitmap
        /// with appropriate dimensions for previewing the Mandelbrot Set.
        /// </summary>
        /// <param name="uiImage"></param>
        /// <param name="iPlaneDimensions"></param>
        /// <returns></returns>
        public static Bitmap GetEmptyPreviewBitmap(PictureBox uiImage, IPlaneBoundingRectangle iPlaneBoundingRectangle)
        {
            int height;
            int width;

            if (uiImage.Height < uiImage.Width)
            {
                height = uiImage.Height;
                width = GetCorrespondingPixelCount(height, false, iPlaneBoundingRectangle);
            }
            else
            {
                width = uiImage.Width;
                height = GetCorrespondingPixelCount(width, true, iPlaneBoundingRectangle);
            }

            return new Bitmap(width, height);
        }

        /// <summary>
        /// Takes an empty bitmap, and corresponding bounds in the imaginary plane to generate the Mandelbrot Set, pixel by pixel.
        /// </summary>
        /// <param name="inputBitmap"></param>
        /// <param name="iPlaneDimensions"></param>
        public static Bitmap GetMandelbrotImage(Bitmap emptyBitmap, IPlaneBoundingRectangle iPlaneBoundingRectangle, double frequencyScale, double phaseOffset)
        {
            Bitmap NewImage = emptyBitmap;
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
        /// Takes a known pixel range and calculates the unknown pixel range given a set of iPlaneDimensions.
        /// </summary>
        /// <param name="existingPixelRange"></param>
        /// <param name="isPixelRangeOnXAxis"></param>
        /// <param name="iPlaneDimensions"></param>
        /// <returns></returns>
        private static int GetCorrespondingPixelCount(int existingPixelRange, bool isPixelRangeOnXAxis, IPlaneBoundingRectangle iPlaneBoundingRectangle)
        {
            int correspondingPixels;
            if (isPixelRangeOnXAxis)
            {
                double pixelScaling = existingPixelRange / iPlaneBoundingRectangle.GetRealRange();
                correspondingPixels = (int)Math.Round(pixelScaling * iPlaneBoundingRectangle.GetImaginaryRange());
            }
            else
            {
                double pixelScaling = existingPixelRange / iPlaneBoundingRectangle.GetImaginaryRange();
                correspondingPixels = (int)Math.Round(pixelScaling * iPlaneBoundingRectangle.GetRealRange());
            }
            return correspondingPixels;
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
