using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Laplacian5X5Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Laplacian5x5,Matrix.Laplacian5x5, 1, 0, true);
        }

        public bool IsEdge(Color c, Color previousColor)
            => c.R + c.G + c.B < 750;
    }
}