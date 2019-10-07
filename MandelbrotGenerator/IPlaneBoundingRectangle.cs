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
        }

        public double MinimumReal { get; }
        public double MaximumReal { get; }
        public double MinimumImaginary { get; }
        public double MaximumImaginary { get; }
        public double GetRealRange() { return MaximumReal - MinimumReal; }
        public double GetImaginaryRange() { return MaximumImaginary - MinimumImaginary; }
        public bool IsScalingSet { get { return isScalingSet; } }
        private double scaling { get; set; }
        private bool isScalingSet { get; set; } = false;
        public Complex GetTranslatedCoordinates(Point pixelPoint)
        {
            if (!IsScalingSet) throw new Exception("Scaling has not been set or updated! Unable to find coordinates in the imaginary plane...");
            double R = ((scaling * pixelPoint.X) + this.MinimumReal);
            double I = ((scaling * pixelPoint.Y) + this.MinimumImaginary);
            return new Complex(R, I);
        }

        public void SetScaling(Bitmap bitmap)
        {
            double scalingX = this.GetRealRange() / bitmap.Width;
            double scalingY = this.GetImaginaryRange() / bitmap.Height;
            double delta = Math.Abs(scalingX - scalingY);
            if (delta > 0.0001) throw new Exception("Specified imaginary plane bounds do not match the Bitmap dimensions! They need identical aspect ratios.");
            scaling = scalingX; //For the avoidance of doubt, scalingX and scalingY should be identical - otherwise scaling assumptions do not make sense.
            isScalingSet = true;
        }
    }
}
