using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace MandelbrotGenerator
{
    class ApplicationWorker
    {
        public ApplicationWorker(PictureBox pictureBox)
        {
            UpdateCurrentIplaneBoundsAndBitmap(pictureBox);
        }

        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;
        private Point FirstClickCoords { get; set; }
        private Complex FirstComplex { get; set; } = new Complex(-2, -1.3);
        private Complex SecondComplex { get; set; } = new Complex(1.1, 1.3);
        private Complex PendingFirstComplex { get; set; }
        private Complex PendingSecondComplex { get; set; }
        private static Color ContrastingColor { get; set; }
        private IPlaneBoundingRectangle CurrentIPlaneBounds { get; set; }
        private Bitmap CurrentEmptyBitmap { get; set; }
        private Pen Pen { get; set; }
        private static FontFamily FontFamily { get; } = new FontFamily("Arial");
        private static Font Font { get; } = new Font(FontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);
        private static SolidBrush Brush { get; set; }

        public void PreviewMandelbrotImage(ref PictureBox pictureBox)
        {            
            Bitmap mandelbrotImage = MandelbrotUtils.GetMandelbrotImage(CurrentEmptyBitmap, CurrentIPlaneBounds, FrequencyScale, PhaseOffset);
            ContrastingColor = GetContrastingColor(mandelbrotImage.GetPixel(0, 0));
            Pen = new Pen(ContrastingColor, 1){ DashStyle = DashStyle.Dot };
            Brush = new SolidBrush(ContrastingColor);
            pictureBox.Image = mandelbrotImage;
        }

        public void UpdateRectangleGraphics(ref PictureBox pictureBox, MouseEventArgs e)
        {
            //refresh picture box
            pictureBox.Refresh();
            //set corner square to mouse coordinates
            double selectWidth = e.X - FirstClickCoords.X;
            double selectHeight = e.Y - FirstClickCoords.Y;
            int absSelectWidth = Math.Abs(e.X - FirstClickCoords.X);
            int absSelectHeight = Math.Abs(e.Y - FirstClickCoords.Y);
            int posX1 = (selectWidth >= 0) ? FirstClickCoords.X : e.X;
            int posY1 = (selectHeight >= 0) ? FirstClickCoords.Y : e.Y;
            int posX2 = (selectWidth >= 0) ? e.X : FirstClickCoords.X;
            int posY2 = (selectHeight >= 0) ? e.Y : FirstClickCoords.Y;

            Bitmap scaledBitmap = ScalingUtils.GetZoomedBitmapDimensions(pictureBox);
            int offsetX = ((pictureBox.Width - scaledBitmap.Width) / 2);
            int offsetY = ((pictureBox.Height - scaledBitmap.Height) / 2);

            Point point1 = new Point(posX1 - offsetX, posY1 - offsetY);
            Point point2 = new Point(posX2 - offsetX, posY2 - offsetY);
            PendingFirstComplex = CurrentIPlaneBounds.GetTranslatedCoordinates(point1);
            PendingSecondComplex = CurrentIPlaneBounds.GetTranslatedCoordinates(point2);
            
            Rectangle rect = new Rectangle(posX1, posY1, absSelectWidth, absSelectHeight);

            pictureBox.CreateGraphics().DrawRectangle(Pen, rect);
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1:#.###e+00}", PendingFirstComplex.Real, PendingFirstComplex.Imaginary), Font, Brush, new Point(posX1, posY1));
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1:#.###e+00}", PendingSecondComplex.Real, PendingSecondComplex.Imaginary), Font, Brush, new Point(posX2, posY2));
        }

        public void SetFirstClick(Point point)
        {
            FirstClickCoords = point;
        }

        public void ConfirmSelection()
        {
            FirstComplex = PendingFirstComplex;
            SecondComplex = PendingSecondComplex;
        }

        private static Color GetContrastingColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public void UpdateCurrentIplaneBoundsAndBitmap(PictureBox pictureBox)
        {
            CurrentIPlaneBounds = new IPlaneBoundingRectangle(FirstComplex, SecondComplex);
            CurrentEmptyBitmap = MandelbrotUtils.GetEmptyPreviewBitmap(pictureBox, CurrentIPlaneBounds);
            CurrentIPlaneBounds.SetScaling(CurrentEmptyBitmap);
        }
    }
}
