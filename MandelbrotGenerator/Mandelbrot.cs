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
    public class Mandelbrot
    {

        /// <summary>
        /// Default coordinates, in the imaginary plane, to capture the whole Mandelbrot set,
        /// </summary>
        public static IPlaneDimensions DefaultIPlaneDimensions { get; } = new IPlaneDimensions(-2, 1.1, -1.3, 1.3);

        /// <summary>
        /// Precision coordinate in the imaginary plane.
        /// </summary>
        public class IPoint
        {
            public IPoint(double scalingValue, IPlaneDimensions iPlaneDimensions, Point point)
            {
                X = ((scalingValue * point.X) + iPlaneDimensions.MinimumRealValue);
                I = ((scalingValue * point.Y) + iPlaneDimensions.MinimumImaginaryValue);
            }

            public double X { get; }
            public double I { get; }
        }

        /// <summary>
        /// Object for storing readonly, rectangular dimensions in the imaginary plane.
        /// </summary>
        public class IPlaneDimensions
        {
            public IPlaneDimensions(double minimumRealValue, double maximumRealValue, double minimumImaginaryValue, double maximumImaginaryValue)
            {
                string exceptionMessage = "Bad dimensions! max values should be greater than min values.";
                if (maximumRealValue < minimumRealValue || maximumImaginaryValue < minimumImaginaryValue) throw new Exception(exceptionMessage);

                MinimumRealValue = minimumRealValue;
                MaximumRealValue = maximumRealValue;
                MinimumImaginaryValue = minimumImaginaryValue;
                MaximumImaginaryValue = maximumImaginaryValue;
            }

            public double MinimumRealValue { get; }
            public double MaximumRealValue { get; }
            public double MinimumImaginaryValue { get; }
            public double MaximumImaginaryValue { get; }
            public double GetRealRange() { return MaximumRealValue - MinimumRealValue; }
            public double GetImaginaryRange() { return MaximumImaginaryValue - MinimumImaginaryValue; }
            public double GetAspectRatio() { return GetImaginaryRange() / GetRealRange(); }
        }

        /// <summary>
        /// Scaling class contains utilities to map imaginary coordinates to image pixels. 
        /// </summary>
        public static class ScalingUtils
        {
            /// <summary>
            /// Compares dimensions in the imaginary place with dimensions
            /// of the input Picture Box dimensions and returns an empty Bitmap
            /// with appropriate dimensions for previewing the Mandelbrot Set.
            /// </summary>
            /// <param name="uiImage"></param>
            /// <param name="iPlaneDimensions"></param>
            /// <returns></returns>
            public static Bitmap GetEmptyPreviewBitmap(PictureBox uiImage, IPlaneDimensions iPlaneDimensions)
            {
                int height;
                int width;
                double pictureAspectRatio = uiImage.Height / uiImage.Width;

                if (pictureAspectRatio < 0)
                {
                    height = uiImage.Height;
                    width = GetCorrespondingPixelCount(height, false, iPlaneDimensions); 
                }
                else
                {
                    width = uiImage.Width;
                    height = GetCorrespondingPixelCount(width, true, iPlaneDimensions);
                }

                return new Bitmap(width, height);
            }

            /// <summary>
            /// Takes a known pixel range and calculates the unknown pixel range given a set of iPlaneDimensions.
            /// </summary>
            /// <param name="existingPixelRange"></param>
            /// <param name="isPixelRangeOnXAxis"></param>
            /// <param name="iPlaneDimensions"></param>
            /// <returns></returns>
            private static int GetCorrespondingPixelCount(int existingPixelRange, bool isPixelRangeOnXAxis, IPlaneDimensions iPlaneDimensions)
            {
                int correspondingPixels;
                if (isPixelRangeOnXAxis)
                {
                    double scaling = existingPixelRange / iPlaneDimensions.GetRealRange();
                    correspondingPixels = (int)Math.Round(scaling * iPlaneDimensions.GetImaginaryRange());
                }
                else
                {
                    double scaling = existingPixelRange / iPlaneDimensions.GetImaginaryRange();
                    correspondingPixels = (int)Math.Round(scaling * iPlaneDimensions.GetRealRange());
                }
                return correspondingPixels;
            }
        }

        /// <summary>
        /// Underlying mathematics to generate the images. 
        /// </summary>
        public static class Mathematics
        {
            /// <summary>
            /// Takes an empty bitmap, and corresponding bounds in the imaginary plane to generate the Mandelbrot Set, pixel by pixel.
            /// </summary>
            /// <param name="inputBitmap"></param>
            /// <param name="iPlaneDimensions"></param>
            public static void GenerateMandelbrotImage(ref Bitmap emptyBitmap, IPlaneDimensions iPlaneDimensions, double frequencyScale, double phaseOffset)
            {
                double scalingX = iPlaneDimensions.GetRealRange() / emptyBitmap.Width;
                double scalingY = iPlaneDimensions.GetImaginaryRange() / emptyBitmap.Height;
                double delta = Math.Abs(scalingX - scalingY);
                if (delta > 0.0001) throw new Exception("Specified imaginary plane bounds do not match the Bitmap dimensions");
                double scaling = scalingX; //For the avoidance of doubt, scalingX and scalingY should be identical.

                for (int x = 0; x < emptyBitmap.Width; x++)
                {
                    for (int y = 0; y < emptyBitmap.Height; y++)
                    {
                        int a = 255;

                        IPoint point = new IPoint(scaling, iPlaneDimensions, new Point(x, y));
                        int sample = GetIteratedIntegerValue(point);

                        double trigScaling = 0.0246;

                        int r = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * sample - phaseOffset) * 127 + 128);
                        int g = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * sample - phaseOffset - (2 * Math.PI / 3)) * 127 + 128);
                        int b = (int)Math.Round(Math.Sin(frequencyScale * trigScaling * sample - phaseOffset - (4 * Math.PI / 3)) * 127 + 128);

                        emptyBitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    }
                }
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
                Complex complexNumber = new Complex(point.X, point.I);

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
}
