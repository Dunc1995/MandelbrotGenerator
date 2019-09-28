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
        public Color ContrastingColor { get; set; }
        public Bitmap CurrentBitmap { get; set; }
        public Mandelbrot.IPlaneDimensions CurrentIPlaneBounds { get { return currentIPlaneBounds; } }
        public Bitmap CurrentEmptyBitmap { get { return currentEmptyBitmap; } }
        private Mandelbrot.IPlaneDimensions currentIPlaneBounds { get; set; } = Mandelbrot.DefaultIPlaneDimensions;
        private Bitmap currentEmptyBitmap { get; set; }
        

        public void StartupMain(ref PictureBox pictureBox)
        {
            try
            {
                CurrentBitmap = GetPreviewImage();
                ContrastingColor = GetContrastingColor(CurrentBitmap.GetPixel(0, 0));
                pictureBox.Image = CurrentBitmap;
                currentEmptyBitmap = Mandelbrot.ScalingUtils.GetEmptyPreviewBitmap(pictureBox, CurrentIPlaneBounds); 
            }
            catch (Exception)
            {

            }           
        }

        public void PreviewMandelbrotImage(ref PictureBox pictureBox, Mandelbrot.IPlaneDimensions iPlaneDimensions)
        {
            Bitmap image = CurrentEmptyBitmap;
            Mandelbrot.Mathematics.GenerateMandelbrotImage(ref image, iPlaneDimensions, FrequencyScale, PhaseOffset);
            CurrentBitmap = image;
            ContrastingColor = GetContrastingColor(image.GetPixel(0, 0));
            pictureBox.Image = CurrentBitmap;
        }

        public void UpdateIPlaneAndBitmapDimensions(ref PictureBox pictureBox, Mandelbrot.IPlaneDimensions iPlaneDimensions)
        {
            currentIPlaneBounds = iPlaneDimensions;
            currentEmptyBitmap = Mandelbrot.ScalingUtils.GetEmptyPreviewBitmap(pictureBox, iPlaneDimensions);
        }

        private Bitmap GetPreviewImage()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("MandelbrotGenerator.resources.preview.bmp");
            return new Bitmap(myStream);
        }

        private static Color GetContrastingColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
