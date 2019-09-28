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

        private Bitmap GetPreviewImage()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("MandelbrotGenerator.resources.preview.bmp");
            return new Bitmap(myStream);
        }
    }
}
