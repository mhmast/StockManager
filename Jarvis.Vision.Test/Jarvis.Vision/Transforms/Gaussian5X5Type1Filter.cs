using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Gaussian5X5Type1Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Gaussian5x5Type1,Matrix.Gaussian5x5Type1, 1, 0, true);
        }


        public bool IsEdge(Color c, Color previousColor)
            =>  c.R + c.G + c.B < 750;
    }
}