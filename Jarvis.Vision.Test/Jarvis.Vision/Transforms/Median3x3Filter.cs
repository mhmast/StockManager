using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Median3x3Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).MedianFilter(3);
        }

        public bool IsEdge(Color c, Color previousColor)
            => c.R + c.G + c.B < 750;
    }
}