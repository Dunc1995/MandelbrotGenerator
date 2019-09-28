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
        public Bitmap CurrentImage { get; set; }
        private ApplicationWorker Application { get; } = new ApplicationWorker();
        public MainDisplay()
        {
            InitializeComponent();
            Application.StartupMain(ref pictureBox1);
        }

        private void generateImageButton_Click(object sender, EventArgs e)
        {
                
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
            Application.PhaseOffset = phaseTrackBar.Value * 0.628;
            Application.PreviewMandelbrotImage(ref pictureBox1);
        }

        /// <summary>
        /// Edits the color map of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            Application.FrequencyScale = frequencyTrackBar.Value;
            Application.PreviewMandelbrotImage(ref pictureBox1);
        }
    }
}
