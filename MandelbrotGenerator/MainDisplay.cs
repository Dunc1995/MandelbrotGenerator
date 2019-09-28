using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace MandelbrotGenerator
{
    public partial class MainDisplay : Form
    {
        public int PictureWidth { get; set; }
        public int PictureHeight { get; set; }
        public double RealRange { get; set; }
        public double ImaginaryRange { get; set; }
        public double MinimumRealValue { get; set; } = -2;
        public double MaximumRealValue { get; set; } = 1.1;
        public double MinimumImaginaryValue { get; set; } = -1.3;
        public double MaximumImaginaryValue { get; set; } = 1.3;
        public double ScalingValue { get; set; }
        public int PreviewPixelCount { get; } = 250;
        public Bitmap CurrentImage { get; set; }
        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;
        public int Pixels { get; set; }

        public MainDisplay()
        {
            InitializeComponent();
            PreviewImage();
        }

        public void GenerateImage(double maxR, double minR, double maxI, double minI, int pixelCount)
        {
            int specifiedPixels = pixelCount;
            MaximumRealValue = maxR;
            MinimumRealValue = minR;
            MaximumImaginaryValue = maxI;
            MinimumImaginaryValue = minI;

            RealRange = MaximumRealValue - MinimumRealValue;
            ImaginaryRange = MaximumImaginaryValue - MinimumImaginaryValue;

            double aspectRatio = (RealRange > ImaginaryRange) ? RealRange / ImaginaryRange : ImaginaryRange / RealRange;
            int resultantPixels = (int)(aspectRatio * specifiedPixels);

            PictureWidth = (RealRange > ImaginaryRange) ? resultantPixels : specifiedPixels;
            PictureHeight = (ImaginaryRange > RealRange) ? resultantPixels : specifiedPixels;
            ScalingValue = ImaginaryRange / PictureHeight; //This ratio should be identical to X direction scaling.

            CurrentImage = new Bitmap(PictureWidth, PictureHeight);

            for (int x = 0; x < PictureWidth; x++)
            {
                for (int y = 0; y < PictureHeight; y++)
                {
                    int a = 255;

                    GetFloatingPointValue(x, y, out double scaledX, out double scaledY);
                    int sample = IterateValue(scaledX, scaledY);

                    double trigScaling = 0.0246;

                    int r = (int)Math.Round(Math.Sin(FrequencyScale * trigScaling * sample - PhaseOffset) * 127 + 128);
                    int g = (int)Math.Round(Math.Sin(FrequencyScale * trigScaling * sample - PhaseOffset - (2 * Math.PI / 3)) * 127 + 128);
                    int b = (int)Math.Round(Math.Sin(FrequencyScale * trigScaling * sample - PhaseOffset - (4 * Math.PI / 3)) * 127 + 128);

                    CurrentImage.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
           
            pictureBox1.Image = CurrentImage;
        }

        /// <summary>
        /// Translate pixel coordinates to dimensions within the complex plane.
        /// Magnitudes greater than 2 will not be within the Mandelbrot Set, therefore
        /// this determines the bounds within the pixel dimensions.
        /// </summary>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
        /// <returns></returns>
        public void GetFloatingPointValue(int pixelX, int pixelY, out double ScaledX, out double ScaledY)
        {
            ScaledX = ((ScalingValue * pixelX) + MinimumRealValue);
            ScaledY = ((ScalingValue * pixelY) + MinimumImaginaryValue);
        }

        /// <summary>
        /// Executes the Mandelbrot iteration to determine stability. High numbers indicate instability (in very general terms).
        /// </summary>
        /// <param name="scaledX"></param>
        /// <param name="scaledY"></param>
        /// <returns></returns>
        public int IterateValue(double scaledX, double scaledY)
        {
            int count = 0;
            Complex complexNumber = new Complex(scaledX, scaledY);

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

        private void generateImageButton_Click(object sender, EventArgs e)
        {
                GenerateImage(MaximumRealValue, MinimumRealValue, MaximumImaginaryValue, MinimumImaginaryValue, Pixels);
        }


        private void PreviewImage()
        {
                GenerateImage(MaximumRealValue, MinimumRealValue, MaximumImaginaryValue, MinimumImaginaryValue, PreviewPixelCount);
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = ".bmp";
            saveDialog.OverwritePrompt = true;
            saveDialog.ShowDialog();

            string path = saveDialog.FileName;
            CurrentImage.Save(path);
        }

        /// <summary>
        /// Edits the color map of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void phaseTrackBar_Scroll(object sender, EventArgs e)
        {
            PhaseOffset = phaseTrackBar.Value * 0.628;
            PreviewImage();
        }

        /// <summary>
        /// Edits the color map of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            FrequencyScale = frequencyTrackBar.Value;
            PreviewImage();
        }
    }
}
