using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class LaplacianOfGaussianFilter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.LaplacianOfGaussian,Matrix.LaplacianOfGaussian, 1, 0, true);
        }


        public bool IsEdge(Color c, Color previousColor)
            => c.R + c.G + c.B < 750;
    }
}