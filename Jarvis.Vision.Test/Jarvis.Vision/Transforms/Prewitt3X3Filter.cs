using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Prewitt3X3Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Prewitt3x3Horizontal,Matrix.Prewitt3x3Vertical, 0.01);
        }


        public bool IsEdge(Color c, Color previousColor)
            => c.A > 120;
    }
}