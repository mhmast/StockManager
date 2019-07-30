using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Mean3x3Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Mean3x3, Matrix.Mean3x3, 1/160.0, 0, false);
        }

        public bool IsEdge(Color c, Color previousColor)
            => c.R + c.G + c.B < 750;
    }
}