using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MandelbrotGenerator
{
    public class Mandelbrot
    {

        /// <summary>
        /// Default coordinates, in the imaginary plane, to capture the whole Mandelbrot set,
        /// </summary>
        public Dimensions DefaultDimensions { get; } = new Dimensions(-2, 1.1, -1.3, 1.3);

        /// <summary>
        /// Object for storing readonly dimensions in the imaginary plane.
        /// </summary>
        public class Dimensions
        {
            public Dimensions(double minimumRealValue, double maximumRealValue, double minimumImaginaryValue, double maximumImaginaryValue)
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
            public static Bitmap GetEmptyPreviewBitmap(PictureBox uiImage, Dimensions iPlaneDimensions)
            {
                double scaling;
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
            /// Takes a known pixel range and calculates the unknown pixel range given the set of iPlaneDimensions.
            /// </summary>
            /// <param name="existingPixelRange"></param>
            /// <param name="isPixelRangeOnXAxis"></param>
            /// <param name="iPlaneDimensions"></param>
            /// <returns></returns>
            private static int GetCorrespondingPixelCount(int existingPixelRange, bool isPixelRangeOnXAxis, Dimensions iPlaneDimensions)
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
        private static class Mathematics
        {
            
        }
    }
}
