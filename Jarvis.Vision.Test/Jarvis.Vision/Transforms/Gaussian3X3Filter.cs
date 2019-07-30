using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Gaussian3X3Filter : IImageFilter
    {
        private readonly bool _grayScale;

        public Gaussian3X3Filter(bool grayScale = true)
        {
            _grayScale = grayScale;
        }
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Gaussian3x3,Matrix.Gaussian3x3, 1/16.0, 0, _grayScale);
        }

        public bool IsEdge(Color c, Color previousColor)
            => c.R + c.G + c.B < 750;
    }
}