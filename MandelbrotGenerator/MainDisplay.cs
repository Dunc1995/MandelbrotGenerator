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
using System.Drawing.Drawing2D;

namespace MandelbrotGenerator
{
    public partial class MainDisplay : Form
    {
        public Bitmap CurrentImage { get; set; }
        private ApplicationWorker Application { get; } = new ApplicationWorker();
        private bool IsDrawingActive { get; set; } = false;

        public Pen selectPen;
        public MainDisplay()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            ExecuteBackgroundWorker();
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
            ExecuteBackgroundWorker();
        }

        /// <summary>
        /// Edits the color map of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            Application.FrequencyScale = frequencyTrackBar.Value;
            ExecuteBackgroundWorker();
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrawingActive)
            {
                Application.UpdateRectangleGraphics(ref pictureBox1, e);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!IsDrawingActive)
                {
                    Application.FirstClickCoords = new Point(e.X, e.Y);
                }
                else
                {
                    pictureBox1.Invalidate();
                    IPlaneBoundingRectangle iPlaneDimensions = new IPlaneBoundingRectangle(Application.MinR, Application.MaxR, Application.MinI, Application.MaxI);
                    Application.UpdateIPlaneAndBitmapDimensions(ref pictureBox1, iPlaneDimensions);
                    ExecuteBackgroundWorker();
                }

                IsDrawingActive = !IsDrawingActive;
            }
        }

        private void ExecuteBackgroundWorker()
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();
        }

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.PreviewMandelbrotImage(ref pictureBox1);
        }

        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
//            resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled == true)
            //{
            //    resultLabel.Text = "Canceled!";
            //}
            //else if (e.Error != null)
            //{
            //    resultLabel.Text = "Error: " + e.Error.Message;
            //}
            //else
            //{
            //    resultLabel.Text = "Done!";
            //}
        }
    }
}
