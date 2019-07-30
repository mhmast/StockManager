using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    internal class CombinedFilter : IImageFilter
    {
        private readonly IImageFilter _left;
        private readonly IImageFilter _right;

        public CombinedFilter(IImageFilter left,IImageFilter right)
        {
            _left = left;
            _right = right;
        }
        public Image Transform(Image i)
        {
            return _right.Transform(_left.Transform(i));
        }

        public bool IsEdge(Color c, Color previousColor)
        {
            return _right.IsEdge(c, previousColor);
        }
    }
}
