using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jarvis.Vision.Test
{
    public partial class Form1 : Form
    {
        private ScanResult _result;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imageBox.Image = Image.FromFile(openFileDialog1.FileName);
                imageBox.Size = imageBox.Image.Size;
                _result = new Scan(imageBox.Image).GetResult();
                propertyGrid.SelectedObject = _result;
            }
        }
    }
}
