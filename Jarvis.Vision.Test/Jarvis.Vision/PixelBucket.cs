using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Jarvis.Vision
{
    public class PixelBucket : IEnumerable<Point>
    {
        private readonly Layer _l;

        public PixelBucket(Color baseColor, Point basePoint, Layer l)
        {
            _l = l;
            BaseColor = baseColor;
            BasePoint = basePoint;
            Count = 0;
        }

        public int Count { get; private set; }

        public Color BaseColor { get; }
        public Point BasePoint { get; }

        public void Add(Point p)
        {
            _l[p.Y][p.X] = this;
            Count++;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            foreach (var y in _l)
            {
                foreach (var x in y.Value.Where(v=>v.Value == this))
                {
                    yield return new Point(x.Key, y.Key);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
