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

namespace MandelbrotGenerator
{
    class ApplicationWorker
    {
        public ApplicationWorker()
        {

        }

        public double FrequencyScale { get; set; } = 1;
        public double PhaseOffset { get; set; } = 0;
        public Point FirstClickCoords { get; set; }
        public Point SecondClickCoords { get; set; }
        public double MinR { get; set; }
        public double MaxR { get; set; }
        public double MinI { get; set; }
        public double MaxI { get; set; }
        private static Color ContrastingColor { get; set; }
        private IPlaneBoundingRectangle currentIPlaneBounds { get; set; } = MandelbrotUtils.DefaultIPlaneDimensions;
        private Bitmap currentEmptyBitmap { get; set; }
        private Pen Pen { get; set; }
        private static FontFamily fontFamily { get; } = new FontFamily("Arial");
        private static Font font { get; } = new Font(fontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);
        private static SolidBrush Brush { get; set; }

        public void PreviewMandelbrotImage(ref PictureBox pictureBox)
        {
            currentEmptyBitmap = MandelbrotUtils.GetEmptyPreviewBitmap(pictureBox, currentIPlaneBounds);
            Bitmap image = currentEmptyBitmap;
            MandelbrotUtils.GenerateMandelbrotImage(ref image, currentIPlaneBounds, FrequencyScale, PhaseOffset);
            ContrastingColor = GetContrastingColor(image.GetPixel(0, 0));
            Pen = new Pen(ContrastingColor, 1){ DashStyle = DashStyle.Dot };
            Brush = new SolidBrush(ContrastingColor);
            pictureBox.Image = image;
        }

        public void UpdateIPlaneAndBitmapDimensions(ref PictureBox pictureBox, IPlaneBoundingRectangle iPlaneDimensions)
        {
            currentIPlaneBounds = iPlaneDimensions;
            currentEmptyBitmap = MandelbrotUtils.GetEmptyPreviewBitmap(pictureBox, iPlaneDimensions);
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
            int offsetX = ((pictureBox.Width - pictureBox.Image.Width) / 2);
            int offsetY = ((pictureBox.Height - pictureBox.Image.Height) / 2);

            Point point1 = new Point(posX1 - offsetX, posY1 - offsetY);
            Point point2 = new Point(posX2 - offsetX, posY2 - offsetY);
            PointF iPoint1 = MandelbrotUtils.GetTranslatedCoordinates(currentIPlaneBounds, point1);
            PointF iPoint2 = MandelbrotUtils.GetTranslatedCoordinates(currentIPlaneBounds, point2);

            MinR = (iPoint1.X > iPoint2.X) ? iPoint2.X : iPoint1.X;
            MaxR = (iPoint1.X < iPoint2.X) ? iPoint2.X : iPoint1.X;
            MinI = (iPoint1.Y > iPoint2.Y) ? iPoint2.Y : iPoint1.Y;
            MaxI = (iPoint1.Y < iPoint2.Y) ? iPoint2.Y : iPoint1.Y;
            
            Rectangle rect = new Rectangle(posX1, posY1, absSelectWidth, absSelectHeight);

            pictureBox.CreateGraphics().DrawRectangle(Pen, rect);
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1}", iPoint1.X, iPoint1.Y), font, Brush, new Point(posX1, posY1));
            pictureBox.CreateGraphics().DrawString(string.Format("posx: {0:#.###e+00},\n posy:{1}", iPoint2.X, iPoint2.Y), font, Brush, new Point(posX2, posY2));
        }

        private static Color GetContrastingColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
