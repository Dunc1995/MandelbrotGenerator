using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MandelbrotGenerator
{
    public static class ScalingUtils
    {
        public static Bitmap GetZoomedBitmapDimensions(PictureBox pictureBox)
        {
            int width = 0;
            int height = 0;
            double scale;
            double pictureBoxAR = GetAspectRatio(pictureBox.Width, pictureBox.Height);
            double pictureBoxImageAR = GetAspectRatio(pictureBox.Image.Width, pictureBox.Image.Height);
            
            if (pictureBoxImageAR > pictureBoxAR)
            {
                height = pictureBox.Height;
                scale = height / pictureBox.Image.Height;
                width = (int)(scale * pictureBox.Image.Width);
            }
            else
            {
                width = pictureBox.Width;
                scale = width / pictureBox.Image.Width;
                height = (int)(scale * pictureBox.Image.Height);
            }

            return new Bitmap(width, height);
        }

        private static double GetAspectRatio(double width, double height)
        {
            return (height/width);
        }
    }
}
