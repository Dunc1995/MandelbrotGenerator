using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MandelbrotGenerator
{
    class ApplicationWorker
    {
        public ApplicationWorker()
        {

        }

        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;
        public double Scaling { get; set; }
        public Color ContrastingColor { get; set; }
        public Bitmap CurrentBitmap { get; set; }
        public MandelbrotUtils.IPlaneDimensions CurrentIPlaneBounds { get { return currentIPlaneBounds; } }
        public Bitmap CurrentEmptyBitmap { get { return currentEmptyBitmap; } }
        private MandelbrotUtils.IPlaneDimensions currentIPlaneBounds { get; set; } = MandelbrotUtils.DefaultIPlaneDimensions;
        private Bitmap currentEmptyBitmap { get; set; }      

        public void PreviewMandelbrotImage(ref PictureBox pictureBox, MandelbrotUtils.IPlaneDimensions iPlaneDimensions)
        {
            currentEmptyBitmap = MandelbrotUtils.Scaling.GetEmptyPreviewBitmap(pictureBox, CurrentIPlaneBounds);
            Bitmap image = currentEmptyBitmap;
            MandelbrotUtils.Mathematics.GenerateMandelbrotImage(ref image, iPlaneDimensions, FrequencyScale, PhaseOffset);
            CurrentBitmap = image;
            ContrastingColor = GetContrastingColor(image.GetPixel(0, 0));
            pictureBox.Image = CurrentBitmap;
        }

        public void UpdateIPlaneAndBitmapDimensions(ref PictureBox pictureBox, MandelbrotUtils.IPlaneDimensions iPlaneDimensions)
        {
            currentIPlaneBounds = iPlaneDimensions;
            currentEmptyBitmap = MandelbrotUtils.Scaling.GetEmptyPreviewBitmap(pictureBox, iPlaneDimensions);
        }

        private static Color GetContrastingColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
