using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class Sobel3X3Filter : IImageFilter
    {
        public Image Transform(Image i)
        {
            return new Bitmap(i).ConvolutionFilter(Matrix.Sobel3x3Horizontal,Matrix.Sobel3x3Vertical,1,0,true);
        }


        public bool IsEdge(Color c, Color previousColor)
            => c.A== 255;
    }
}