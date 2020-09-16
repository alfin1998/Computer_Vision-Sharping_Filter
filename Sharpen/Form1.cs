using Sharpen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sharpen
{
    public partial class Form1 : Form
    {
        private Bitmap copy;
        private olahCitra2 oc;
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabelDirectory.Text = "";
            toolStripStatusLabelSize.Text = "";
            toolStripProgressBar1.Visible = false;

            oc = new olahCitra2(toolStripProgressBar1);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog bukaFile = new OpenFileDialog
            {
                Filter = "Image File (*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png"
            };

            if (DialogResult.OK == bukaFile.ShowDialog())
            {
                pictureBox1.Image = new Bitmap(bukaFile.FileName);

                toolStripStatusLabelDirectory.Text = Path.GetFullPath(bukaFile.FileName);
                toolStripStatusLabelSize.Text = "Width: " + pictureBox1.Image.Width + " px. Height: " + pictureBox1.Image.Height + " px.";

            }
        }

        private void SharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copy = new Bitmap((Bitmap)pictureBox1.Image);
            pictureBox2.Image = oc.keSharpen(copy);
        }
    }
}
