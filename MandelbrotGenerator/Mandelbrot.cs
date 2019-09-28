using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                MinimumRealValue = minimumRealValue;
                MaximumRealValue = maximumRealValue;
                MinimumImaginaryValue = minimumImaginaryValue;
                MaximumImaginaryValue = maximumImaginaryValue;
            }

            public double MinimumRealValue { get; }
            public double MaximumRealValue { get; }
            public double MinimumImaginaryValue { get; }
            public double MaximumImaginaryValue { get; }
        }

        /// <summary>
        /// Underlying mathematics to generate the images. 
        /// </summary>
        public static class Mathematics
        {

        }

        /// <summary>
        /// Scaling class contains utilities to map imaginary coordinates to image pixels. 
        /// </summary>
        public static class ScalingUtils
        {

        }
    }
}
