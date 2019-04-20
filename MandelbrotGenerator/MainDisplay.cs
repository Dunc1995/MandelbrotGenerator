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
        public double MinimumRealValue { get; set; }
        public double MinimumImaginaryValue { get; set; }
        public double ScalingValue { get; set; }
        public int PreviewPixelCount { get; } = 250;
        public Bitmap CurrentImage { get; set; }
        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;

        public MainDisplay()
        {
            InitializeComponent();
            PreviewImage();
        }

        public void GenerateImage(double maxR, double minR, double maxI, double minI, int pixelCount)
        {
            int specifiedPixels = pixelCount;
            double maximumRealValue = maxR;
            MinimumRealValue = minR;
            double maximumImaginaryValue = maxI;
            MinimumImaginaryValue = minI;

            RealRange = maximumRealValue - MinimumRealValue;
            ImaginaryRange = maximumImaginaryValue - MinimumImaginaryValue;

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

                    PointF point = GetFloatingPointValue(x, y);
                    int sample = IterateValue(point);

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
        public PointF GetFloatingPointValue(int pixelX, int pixelY)
        {
            float scaledX = (float)((ScalingValue * pixelX) + MinimumRealValue);
            float scaledY = (float)((ScalingValue * pixelY) + MinimumImaginaryValue);

            return new PointF(scaledX, scaledY);
        }

        public int IterateValue(PointF complexPoint)
        {
            int count = 0;
            Complex complexNumber = new Complex(complexPoint.X, complexPoint.Y);

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

        public double MaxR { get; set; } = 2;
        public double MinR { get; set; } = -2;
        public double MaxI { get; set; } = 2;
        public double MinI { get; set; } = -2;
        public int Pixels { get; set; }

        private void generateImageButton_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                GenerateImage(MaxR, MinR, MaxI, MinI, Pixels);
            }
        }

        private void maxImaginaryTextbox_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(maxImaginaryTextbox.Text, out double value))
            {
                MaxI = value;

                if (ValidateInputs())
                {
                    PreviewImage();
                }
            }
        }

        private void minImaginaryTextbox_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(minImaginaryTextbox.Text, out double value))
            {
                MinI = value;

                if (ValidateInputs())
                {
                    PreviewImage();
                }
            }
        }

        private void maxRealTextbox_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(maxRealTextbox.Text, out double value))
            {
                MaxR = value;

                if (ValidateInputs())
                {
                    PreviewImage();
                }
            }
        }

        private void minRealTextbox_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(minRealTextbox.Text, out double value))
            {
                MinR = value;

                if (ValidateInputs())
                {
                    PreviewImage();
                }
            }
        }

        private void pixelCountTextbox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(pixelCountTextbox.Text, out int value))
            {
                Pixels = value;
            }
        }

        private void PreviewImage()
        {
            GenerateImage(MaxR, MinR, MaxI, MinI, PreviewPixelCount);
        }

        private bool ValidateInputs()
        {
            bool valid = false;

            if (MaxR > MinR && MaxI > MinI)
            {
                valid = true;
            }
            return valid;
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

        private void phaseTrackBar_Scroll(object sender, EventArgs e)
        {
            PhaseOffset = phaseTrackBar.Value * 0.628;
            PreviewImage();
        }

        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            FrequencyScale = frequencyTrackBar.Value;
            PreviewImage();
        }
    }
}
