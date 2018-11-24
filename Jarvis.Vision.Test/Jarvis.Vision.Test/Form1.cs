using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jarvis.Vision.Test
{
    public partial class Form1 : Form
    {
        private ScanResult result;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var imageBoxImage = Image.FromFile(openFileDialog1.FileName);
                imageBox.Image = imageBoxImage;
                imageBox.Size = imageBox.Image.Size;
                var result = new Scan(imageBox.Image).GetResult();
                GenerateTreeView(result);
            }
        }

        private void GenerateTreeView(ScanResult scanResult)
        {
            treeView1.Tag = scanResult;
            treeView1.Nodes.Clear();
            var treeNode = new TreeNode("Scan Result");
            treeView1.Nodes.Add(treeNode);
            foreach (var scanResultLayer in scanResult.Layers)
            {
                var node = treeNode.Nodes.Add($"Layer (tolerance {scanResultLayer.Tolerance}");
                node.Tag = scanResultLayer;
            }
        }

        private static void LoadLayerBucketNodes(Layer scanResultLayer, TreeNode node)
        {

            var bucketnr = 0;
            foreach (var bucket in scanResultLayer.Buckets)
            {
                var buckNode = node.Nodes.Add($"Bucket {bucketnr} ({bucket.Count()} pixels)");
                buckNode.Tag = bucket;
                foreach (var propertyInfo in bucket.GetType()
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    buckNode.Nodes.Add($"{propertyInfo.Name} : {propertyInfo.GetValue(bucket)}");
                }

                bucketnr++;
            }

        }

        private void OnNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DrawNode(e.Node);
        }

        private void DrawNode(TreeNode treeNode)
        {
            if (treeNode.Tag is Layer layer && treeNode.Nodes.Count == 0)
            {
                LoadLayerBucketNodes(layer, treeNode);
            }

            if (treeNode.Tag is PixelBucket bucket)
            {
                if (treeNode.TreeView.Tag is ScanResult scanResult)
                {
                    DrawBucketOn(bucket, scanResult.FilteredImage);
                }
            }
        }

        private void DrawBucketOn(PixelBucket bucket, Image originalImage)
        {
            var bmp = new Bitmap(originalImage);
            var bmpData = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var data = new PixelData(bmpData);
            foreach (var pixel in bucket)
            {
                var pixData = data.GetPixel(pixel);
                data.SetPixel(pixel, pixData.Blend(Color.CornflowerBlue, 0.7f));
            }
            bmp.UnlockBits(bmpData);
            imageBox.Image = bmp;
        }

        private void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            DrawNode(e.Node);
        }
    }
}
