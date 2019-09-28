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

        public void StartupMain(ref PictureBox pictureBox)
        {
            try
            {
                Bitmap image = GetPreviewImage();
                pictureBox.Image = image;
                pictureBox.LoadAsync();
            }
            catch (Exception)
            {

            }           
        }

        public void PreviewMandelbrotImage(ref PictureBox pictureBox)
        {
            Mandelbrot.IPlaneDimensions plane = Mandelbrot.DefaultIPlaneDimensions;
            Bitmap image = Mandelbrot.ScalingUtils.GetEmptyPreviewBitmap(pictureBox, plane);
            Mandelbrot.Mathematics.GenerateMandelbrotImage(ref image, plane, FrequencyScale, PhaseOffset);
            pictureBox.Image = image;
        }

        private Bitmap GetPreviewImage()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("MandelbrotGenerator.resources.preview.bmp");
            return new Bitmap(myStream);
        }
    }
}
