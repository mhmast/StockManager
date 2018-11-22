using System.Collections.Generic;
using System.Drawing;

namespace Jarvis.Vision
{
    public class PixelBucket  : List<Point>
    {
        private Point basePoint;

        public PixelBucket(Color baseColor, Point basePoint)
        {
            BaseColor = baseColor;
            BasePoint = basePoint;
        }

        public Color BaseColor { get; }
        public Point BasePoint { get; }
    }
}
