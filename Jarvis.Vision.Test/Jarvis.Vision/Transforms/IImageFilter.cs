using System.Drawing;

namespace Jarvis.Vision.Transforms
{
    public interface IImageFilter
    {
        Image Transform(Image i);
        bool IsEdge(Color c, Color previousColor);
    }
}
