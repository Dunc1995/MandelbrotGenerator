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
        public ApplicationWorker()
        {
            CurrentIPlaneBounds = new IPlaneBoundingRectangle(FirstComplex, SecondComplex);
        }

        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;
        private Point FirstClickCoords { get; set; }
        private Point FirstRawCoords { get; set; }
        private Point SecondRawCoords { get; set; }
        private Rectangle UserSelectionRectangle { get; set; }
        private Complex FirstComplex { get; set; } = new Complex(-2, -1.3);
        private Complex SecondComplex { get; set; } = new Complex(1.1, 1.3);
        private Complex PendingFirstComplex { get; set; }
        private Complex PendingSecondComplex { get; set; }
        private static Color ContrastingColor { get; set; }
        private IPlaneBoundingRectangle CurrentIPlaneBounds { get; set; }
        private Pen Pen { get; set; }
        private static FontFamily FontFamily { get; } = new FontFamily("Arial");
        private static Font Font { get; } = new Font(FontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);
        private static SolidBrush Brush { get; set; }

        public void PreviewMandelbrotImage(ref PictureBox pictureBox)
        {            
            Bitmap mandelbrotImage = MandelbrotUtils.GetMandelbrotImage(CurrentIPlaneBounds, FrequencyScale, PhaseOffset);
            ContrastingColor = GetContrastingColor(mandelbrotImage.GetPixel(0, 0));
            Pen = new Pen(ContrastingColor, 1){ DashStyle = DashStyle.Dot };
            Brush = new SolidBrush(ContrastingColor);
            pictureBox.Image = mandelbrotImage;
        }

        public void MouseMoveUpdate(ref PictureBox pictureBox, MouseEventArgs e)
        {
            //refresh picture box
            pictureBox.Refresh();
            UpdateUserSelectionGraphics(e);
            CalculateImaginaryCoordinates(ref pictureBox);
            DrawUserSelection(ref pictureBox);
        }

        public void SetFirstClick(Point point)
        {
            FirstClickCoords = point;
        }

        public void ConfirmSelection()
        {
            FirstComplex = PendingFirstComplex;
            SecondComplex = PendingSecondComplex;
            CurrentIPlaneBounds = new IPlaneBoundingRectangle(FirstComplex, SecondComplex);
        }

        public void UpdateUserSelectionGraphics(MouseEventArgs e)
        {
            int relativeXDistance = e.X - FirstClickCoords.X;
            int relativeYDistance = e.Y - FirstClickCoords.Y;

            //Raw PictureBox coordinates needed for drawing on the form.
            int posX1 = (relativeXDistance >= 0) ? FirstClickCoords.X : e.X;
            int posY1 = (relativeYDistance >= 0) ? FirstClickCoords.Y : e.Y;
            int posX2 = (relativeXDistance >= 0) ? e.X : FirstClickCoords.X;
            int posY2 = (relativeYDistance >= 0) ? e.Y : FirstClickCoords.Y;

            int absSelectWidth = Math.Abs(FirstRawCoords.X - SecondRawCoords.X);
            int absSelectHeight = Math.Abs(FirstRawCoords.Y - SecondRawCoords.Y);

            FirstRawCoords = new Point(posX1, posY1);
            SecondRawCoords = new Point(posX2, posY2);
            UserSelectionRectangle = new Rectangle(FirstRawCoords.X, FirstRawCoords.Y, absSelectWidth, absSelectHeight);
        }

        private void CalculateImaginaryCoordinates(ref PictureBox pictureBox)
        {           

            Bitmap scaledBitmap = GetZoomedBitmapDimensions(pictureBox);
            int offsetX = ((pictureBox.Width - scaledBitmap.Width) / 2);
            int offsetY = ((pictureBox.Height - scaledBitmap.Height) / 2);

            Point point1 = new Point(FirstRawCoords.X - offsetX, FirstRawCoords.Y - offsetY);
            Point point2 = new Point(SecondRawCoords.X - offsetX, SecondRawCoords.Y - offsetY);
            PendingFirstComplex = CurrentIPlaneBounds.GetTranslatedCoordinates(point1, scaledBitmap);
            PendingSecondComplex = CurrentIPlaneBounds.GetTranslatedCoordinates(point2, scaledBitmap);
        }

        private void DrawUserSelection(ref PictureBox pictureBox)
        {
            pictureBox.CreateGraphics().DrawRectangle(Pen, UserSelectionRectangle);
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1:#.###e+00}", PendingFirstComplex.Real, PendingFirstComplex.Imaginary), Font, Brush, FirstRawCoords);
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1:#.###e+00}", PendingSecondComplex.Real, PendingSecondComplex.Imaginary), Font, Brush, SecondRawCoords);
        }

        private static Bitmap GetZoomedBitmapDimensions(PictureBox pictureBox)
        {
            int width;
            int height;
            double pictureBoxAR = (double)pictureBox.Height/pictureBox.Width;
            double pictureBoxImageAR = (double)pictureBox.Image.Height / pictureBox.Image.Width;
            bool ImgARIsGreaterThanPicBoxAR = pictureBoxImageAR > pictureBoxAR;
            height = (ImgARIsGreaterThanPicBoxAR) ? pictureBox.Height : (int)(pictureBox.Width * pictureBoxImageAR);
            width = (!ImgARIsGreaterThanPicBoxAR) ? pictureBox.Width : (int)(pictureBox.Height / pictureBoxImageAR);

            return new Bitmap(width, height);
        }

        private static Color GetContrastingColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
