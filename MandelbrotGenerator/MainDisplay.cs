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

        public Pen selectPen;
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
                int posX = (selectWidth >= 0) ? selectX : e.X;
                int posY = (selectHeight >= 0) ? selectY : e.Y;

                //draw dotted rectangle
                FontFamily fontFamily = new FontFamily("Arial");
                Font font = new Font(
                   fontFamily,
                   12,
                   FontStyle.Regular,
                   GraphicsUnit.Point);
                SolidBrush solidBrush = new SolidBrush(Color.Red);


                Rectangle rect = new Rectangle(posX, posY, absSelectWidth, absSelectHeight);
                pictureBox1.CreateGraphics().DrawRectangle(selectPen, rect);
                pictureBox1.CreateGraphics().DrawString(string.Format("width:{0}, height:{1}", absSelectWidth, absSelectHeight), font, solidBrush, new PointF(posX, posY));
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
                    selectPen = new Pen(Color.Red, 2);
                    selectPen.DashStyle = DashStyle.Dot;
                }
                else
                {
                    pictureBox1.Invalidate();
                }

                IsDrawingActive = !IsDrawingActive;
            }
        }
    }
}
