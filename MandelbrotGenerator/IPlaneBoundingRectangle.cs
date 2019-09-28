using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotGenerator
{
    public class IPlaneBoundingRectangle
    {
        /// <summary>
        /// Object for storing readonly, rectangular dimensions in the imaginary plane.
        /// </summary>
        public IPlaneBoundingRectangle(double minimumRealValue, double maximumRealValue, double minimumImaginaryValue, double maximumImaginaryValue)
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
    }
}
