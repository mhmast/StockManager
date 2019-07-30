using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Kirsch3X3Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Kirsch3x3Horizontal,Matrix.Kirsch3x3Vertical, 1, 0, true);
        }


        public bool IsEdge(Color c, Color previousColor)
            => c.A == 255 ;
    }
}