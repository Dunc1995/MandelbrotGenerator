using System;
using System.Collections.Generic;
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
        /// Default coordinates, in the imaginary plane, to capture the whole Mandelbrot set,
        /// </summary>
        public static IPlaneBoundingRectangle DefaultIPlaneDimensions { get; } = new IPlaneBoundingRectangle(-2, 1.1, -1.3, 1.3);
        private static double scaling { get; set; }

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
                height = (uiImage.Height <= 500) ? uiImage.Height : 500;
                width = GetCorrespondingPixelCount(height, false, iPlaneBoundingRectangle);
            }
            else
            {
                width = (uiImage.Width <= 500) ? uiImage.Width : 500;
                height = GetCorrespondingPixelCount(width, true, iPlaneBoundingRectangle);
            }

            return new Bitmap(width, height);
        }

        /// <summary>
        /// Takes an empty bitmap, and corresponding bounds in the imaginary plane to generate the Mandelbrot Set, pixel by pixel.
        /// </summary>
        /// <param name="inputBitmap"></param>
        /// <param name="iPlaneDimensions"></param>
        public static void GenerateMandelbrotImage(ref Bitmap emptyBitmap, IPlaneBoundingRectangle iPlaneBoundingRectangle, double frequencyScale, double phaseOffset)
        {
            double scalingX = iPlaneBoundingRectangle.GetRealRange() / emptyBitmap.Width;
            double scalingY = iPlaneBoundingRectangle.GetImaginaryRange() / emptyBitmap.Height;
            double delta = Math.Abs(scalingX - scalingY);
            if (delta > 0.0001) throw new Exception("Specified imaginary plane bounds do not match the Bitmap dimensions! They need identical aspect ratios.");
            scaling = scalingX; //For the avoidance of doubt, scalingX and scalingY should be identical.

            for (int x = 0; x < emptyBitmap.Width; x++)
            {
                for (int y = 0; y < emptyBitmap.Height; y++)
                {
                    int a = 255;

                    IPoint point = new IPoint(iPlaneBoundingRectangle, new Point(x, y));
                    int iterationCount = GetIteratedIntegerValue(point);

                    double trigScaling = 0.0246;

                    int r = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset) * 127 + 128);
                    int g = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset - (2 * Math.PI / 3)) * 127 + 128);
                    int b = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * iterationCount - phaseOffset - (4 * Math.PI / 3)) * 127 + 128);

                    emptyBitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
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
        private static int GetIteratedIntegerValue(IPoint point)
        {
            int count = 0;
            Complex complexNumber = new Complex(point.R, point.I);

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

        public static PointF GetTranslatedCoordinates(IPlaneBoundingRectangle bounds, Point pixelPoint)
        {
            IPoint point = new IPoint(bounds, pixelPoint);
            return new PointF((float)point.R, (float)point.I);
        }

        /// <summary>
        /// Precision coordinate in the imaginary plane.
        /// </summary>
        private class IPoint
        {
            public IPoint(IPlaneBoundingRectangle iPlaneBoundingRectangle, Point point)
            {
                R = ((scaling * point.X) + iPlaneBoundingRectangle.MinimumRealValue);
                I = ((scaling * point.Y) + iPlaneBoundingRectangle.MinimumImaginaryValue);
            }

            public double R { get; }
            public double I { get; }
        }
    }
}
