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
        int selectX;
        int selectY;
        double MinR { get; set; }
        double MaxR { get; set; }
        double MinI { get; set; }
        double MaxI { get; set; }

        public Pen selectPen;
        public MainDisplay()
        {
            InitializeComponent();
            Application.StartupMain(ref pictureBox1);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();
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
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();           
        }

        /// <summary>
        /// Edits the color map of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            Application.FrequencyScale = frequencyTrackBar.Value;
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();                        
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //validate if right-click was trigger
            if (IsDrawingActive)
            {
                //refresh picture box
                pictureBox1.Refresh();
                //set corner square to mouse coordinates
                double selectWidth = e.X - selectX;
                double selectHeight = e.Y - selectY;
                int absSelectWidth = Math.Abs(e.X - selectX);
                int absSelectHeight = Math.Abs(e.Y - selectY);
                int posX1 = (selectWidth >= 0) ? selectX : e.X;
                int posY1 = (selectHeight >= 0) ? selectY : e.Y;
                int posX2 = (selectWidth >= 0) ? e.X: selectX;
                int posY2 = (selectHeight >= 0) ? e.Y: selectY;
                int offsetX = ((pictureBox1.Width - pictureBox1.Image.Width) / 2);
                int offsetY = ((pictureBox1.Height - pictureBox1.Image.Height) / 2);

                Mandelbrot.IPoint point1 = new Mandelbrot.IPoint(Application.CurrentIPlaneBounds, new Point(posX1 - offsetX, posY1 - offsetY));
                Mandelbrot.IPoint point2 = new Mandelbrot.IPoint(Application.CurrentIPlaneBounds, new Point(posX2 - offsetX, posY2 - offsetY));
                MinR = (point1.R > point2.R) ? point2.R : point1.R;
                MaxR = (point1.R < point2.R) ? point2.R : point1.R;
                MinI = (point1.I > point2.I) ? point2.I : point1.I;
                MaxI = (point1.I < point2.I) ? point2.I : point1.I;               

                //draw dotted rectangle
                FontFamily fontFamily = new FontFamily("Arial");
                Font font = new Font(
                   fontFamily,
                   10,
                   FontStyle.Regular,
                   GraphicsUnit.Point);
                SolidBrush solidBrush = new SolidBrush(Application.ContrastingColor);


                Rectangle rect = new Rectangle(posX1, posY1, absSelectWidth, absSelectHeight);
                pictureBox1.CreateGraphics().DrawRectangle(selectPen, rect);
                pictureBox1.CreateGraphics().DrawString(string.Format("posx:{0},\n posy:{1}", posX1, posY1), font, solidBrush, new PointF(posX1, posY1));
                pictureBox1.CreateGraphics().DrawString(string.Format("posx:{0},\n posy:{1}", posX2, posY2), font, solidBrush, new PointF(posX2, posY2));
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!IsDrawingActive)
                {
                    selectX = e.X;
                    selectY = e.Y;
                    selectPen = new Pen(Application.ContrastingColor, 1);
                    selectPen.DashStyle = DashStyle.Dot;
                }
                else
                {
                    pictureBox1.Invalidate();
                    Mandelbrot.IPlaneDimensions iPlaneDimensions = new Mandelbrot.IPlaneDimensions(MinR, MaxR, MinI, MaxI);
                    Application.UpdateIPlaneAndBitmapDimensions(ref pictureBox1, iPlaneDimensions);
                    if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();
                }

                IsDrawingActive = !IsDrawingActive;
            }
        }

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.PreviewMandelbrotImage(ref pictureBox1, Application.CurrentIPlaneBounds);
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
