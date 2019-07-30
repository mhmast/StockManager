using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Jarvis.Library.Imaging;
using Jarvis.Library.Imaging.Filters;
using Jarvis.Library.Math;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Jarvis.Tools.Vision
{
    public class MainForm : Form
    {
        private SplitContainer splitContainer1;
        private Button loadImageButton;
        private OpenFileDialog openFileDialog;
        private ProgressBar progressBar1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private CustomProps customProps2;
        private Cyotek.Windows.Forms.ImageBox pictureBox;
        private Bitmap _img;
        private VectorOfVectorOfPoint _imageContour;

        public MainForm()
        {
            InitializeComponent();
            customProps2.SelectedPropertyChanged += OnSelectedItemCHnaged;
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.loadImageButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox = new Cyotek.Windows.Forms.ImageBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.customProps2 = new Jarvis.Tools.Vision.CustomProps();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.elementHost1);
            this.splitContainer1.Panel1.Controls.Add(this.loadImageButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox);
            this.splitContainer1.Size = new System.Drawing.Size(1650, 695);
            this.splitContainer1.SplitterDistance = 323;
            this.splitContainer1.TabIndex = 1;
            // 
            // loadImageButton
            // 
            this.loadImageButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.loadImageButton.Location = new System.Drawing.Point(0, 0);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(323, 40);
            this.loadImageButton.TabIndex = 1;
            this.loadImageButton.Text = "Load Image";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.LoadImageButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(500, 363);
            this.progressBar1.Maximum = 3;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(317, 32);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Visible = false;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1323, 695);
            this.pictureBox.TabIndex = 1;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Images|*.jpg;*.gif*.png;*.bmp";
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 40);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(323, 655);
            this.elementHost1.TabIndex = 3;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.customProps2;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1650, 695);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var stream = openFileDialog.OpenFile();
                _img = new Bitmap(System.Drawing.Image.FromStream(stream));
                pictureBox.Image = _img;
                RefreshImage();
            }
        }

        private void RefreshImage()
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            try
            {


                var imageColor = new Image<Gray, byte>(_img);
                //var imageGray = new Mat();
                ////var imageGray32 = new Image<Gray, float>(img.Width, img.Height);
                //var imageBlurred = new Image<Gray, byte>(_img.Width, _img.Height);
                //var imageCanny = new Image<Gray, byte>(_img.Width, _img.Height);
                _imageContour = new VectorOfVectorOfPoint();
                var imageContourHierarchy = new Mat();
                //CvInvoke.CvtColor(imageColor, imageGray, ColorConversion.Bgr2Gray);
                ////CvInvoke.CvtColor(imageColor, imageGray32, ColorConversion.Bgr2Gray,sizeof(float)/sizeof(byte));
                //CvInvoke.GaussianBlur(imageGray, imageBlurred, new Size(3, 3), 0);
                //var v = new Median<byte>(imageBlurred.Bytes).Value;
                //var sigma = 0.33;
                //var lower = (byte)Math.Max(0, (1.0 - sigma) * v);
                //var upper = (byte)Math.Min(255, (1.0 + sigma) * v);

                //var grad_x = new Mat();
                //var grad_y = new Mat();
                //var abs_grad_x = new Mat();
                //var abs_grad_y = new Mat();
                //var grad = new Mat();
                //var edges = new UMat();
                //CvInvoke.Sobel(imageBlurred, grad_x, DepthType.Default, 1, 0);
                //CvInvoke.Sobel(imageBlurred, grad_y, DepthType.Default, 0, 1);
                //CvInvoke.ConvertScaleAbs(grad_x, abs_grad_x, 1, 0);
                //CvInvoke.ConvertScaleAbs(grad_y, abs_grad_y, 1, 0);
                //CvInvoke.AddWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, grad);
                //CvInvoke.Canny(grad, edges, lower, upper, 3, true);
                
                 CvInvoke.FindContours(imageColor, _imageContour, imageContourHierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxTc89Kcos);

                progressBar1.Value = 1;
                //pictureBox.Image = grad.Bitmap;
                pictureBox.Image = imageColor.Bitmap;// RedrawContours(imageColor.Size, _imageContour);
                customProps2.SelectedObject = new ContourData
                {
                    //Edges = edges.GetData(),
                    Contours = _imageContour.ToArrayOfArray().Where(a => a.Length > 7).ToArray(),
                    Hierarchy = imageContourHierarchy.GetData()
                };
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private Bitmap RedrawContours(Size s, VectorOfVectorOfPoint imageContour, VectorOfVectorOfPoint emphasis = null)
        {
            var blackImg = new Image<Gray, byte>(s.Width, s.Height, new Gray());
            CvInvoke.DrawContours(blackImg, imageContour, -1, new MCvScalar(0,0, 255), 1, LineType.EightConnected);
            //if (emphasis != null)
            //{
            //    CvInvoke.DrawContours(blackImg, emphasis, 0, new MCvScalar(255, 0, 0), 1, LineType.EightConnected);
            //}
            return blackImg.Bitmap;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        class ContourData
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Point[][] Contours { get; internal set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Array Hierarchy { get; internal set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Array Edges { get; internal set; }
        }

        private void OnSelectedItemCHnaged(object e)
        {
            if (e is Point[] a)
            {
                pictureBox.Image  = RedrawContours(_img.Size, _imageContour, new VectorOfVectorOfPoint(new[] { a }));
            }
           
        }

    }
}
