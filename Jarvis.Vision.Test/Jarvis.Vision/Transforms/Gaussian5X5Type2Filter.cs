using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Gaussian5X5Type2Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Gaussian5x5Type2,Matrix.Gaussian5x5Type2, 1, 0, true);
        }


        public bool IsEdge(Color c, Color previousColor)
            =>  c.R + c.G + c.B < 750;
    }
}