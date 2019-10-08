using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace MandelbrotGenerator
{
    public class IPlaneBoundingRectangle
    {
        /// <summary>
        /// Object for storing readonly, rectangular dimensions in the imaginary plane.
        /// </summary>
        public IPlaneBoundingRectangle(Complex complexPoint1, Complex complexPoint2)
        {
            MinimumReal = (complexPoint1.Real < complexPoint2.Real) ? complexPoint1.Real : complexPoint2.Real;
            MaximumReal = (complexPoint1.Real > complexPoint2.Real) ? complexPoint1.Real : complexPoint2.Real;
            MinimumImaginary = (complexPoint1.Imaginary < complexPoint2.Imaginary) ? complexPoint1.Imaginary : complexPoint2.Imaginary;
            MaximumImaginary = (complexPoint1.Imaginary > complexPoint2.Imaginary) ? complexPoint1.Imaginary : complexPoint2.Imaginary;
            currentBitmap = GetEmptyBitmap(2);
            SetScaling(currentBitmap);
        }

        public double RealRange { get { return MaximumReal - MinimumReal; } }
        public double ImaginaryRange { get { return MaximumImaginary - MinimumImaginary; } }
        public Bitmap CurrentBitmap { get { return currentBitmap; } }
        private double MinimumReal { get; }
        private double MaximumReal { get; }
        private double MinimumImaginary { get; }
        private double MaximumImaginary { get; }
        private double Scaling { get; set; }
        private Bitmap currentBitmap { get; set; }

        public Complex GetTranslatedCoordinates(Point pixelPoint)
        {
            double R = ((Scaling * pixelPoint.X) + this.MinimumReal);
            double I = ((Scaling * pixelPoint.Y) + this.MinimumImaginary);
            return new Complex(R, I);
        }

        public Complex GetTranslatedCoordinates(Point pixelPoint, Bitmap viewportBitmap)
        {
            double xMultiple = (double)currentBitmap.Width / viewportBitmap.Width;
            double yMultiple = (double)currentBitmap.Height / viewportBitmap.Height;

            pixelPoint.X = (int)(pixelPoint.X * xMultiple);
            pixelPoint.Y = (int)(pixelPoint.Y * yMultiple);

            double R = ((Scaling * pixelPoint.X) + this.MinimumReal);
            double I = ((Scaling * pixelPoint.Y) + this.MinimumImaginary);
            return new Complex(R, I);
        }

        public Bitmap GetEmptyBitmap(int quality)
        {
            int highestResolution;
            int height;
            int width;
            bool heightIsGreaterThanWidth = ImaginaryRange > RealRange;

            highestResolution = (quality > 10) ? 5000 : (quality <= 10 && quality > 0) ? 500 * quality : 500;

            height = (heightIsGreaterThanWidth) ? highestResolution : (int)((ImaginaryRange * highestResolution) / RealRange);
            width = (!heightIsGreaterThanWidth) ? highestResolution : (int)((RealRange * highestResolution) / ImaginaryRange);

            return new Bitmap(width, height);
        }

        /// <summary>
        /// Uses the input bitmap and calculates its pixel count's relative scale to the bounding rectangle within the imaginary plane.
        /// </summary>
        /// <param name="bitmap"></param>
        private void SetScaling(Bitmap bitmap)
        {
            double scalingX = this.RealRange / bitmap.Width;
            double scalingY = this.ImaginaryRange / bitmap.Height;
            double delta = Math.Abs(scalingX - scalingY);
            if (delta > 0.0001) throw new Exception("Specified imaginary plane bounds do not match the Bitmap dimensions! They need identical aspect ratios.");
            Scaling = scalingX; //For the avoidance of doubt, scalingX and scalingY should be identical - otherwise scaling assumptions do not make sense.
        }
    }
}
