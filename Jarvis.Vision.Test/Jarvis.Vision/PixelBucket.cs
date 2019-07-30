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
        private Rectangle _bounds = Rectangle.Empty;

        public PixelBucket(Color baseColor, Point basePoint, Layer l)
        {
            _l = l;
            BaseColor = baseColor;
            BasePoint = basePoint;
            Count = 0;
        }

        public int Count { get; private set; }
        public Rectangle Bounds => _bounds == Rectangle.Empty ? CreateBounds() : _bounds;

        private Rectangle CreateBounds()
        {
            int maxx;
            var first = this.First();
            var minx = maxx = first.X;
            int maxy;
            var miny = maxy = first.Y;
            foreach (var point in this.Skip(1))
            {
                if (minx > point.X)
                {
                    minx = point.X;
                }

                if (maxx < point.X)
                {
                    maxx = point.X;
                }

                if (miny > point.Y)
                {
                    miny = point.Y;
                }

                if (maxy < point.Y)
                {
                    maxy = point.Y;
                }
            }
            _bounds = Rectangle.FromLTRB(minx, miny, maxx, maxy);
            return _bounds;
        }

        public Color BaseColor { get; }
        public Point BasePoint { get; }
        public Layer Layer => _l;

        public void Add(Point p)
        {
            _l[p.Y][p.X] = this;
            Count++;
        }

        public bool Contains(Color c,Color previousColor)
            => _l.Filter.IsEdge(c, previousColor);

        public IEnumerator<Point> GetEnumerator()
        {
            foreach (var y in _l)
            {
                foreach (var x in y.Value.Where(v => v.Value == this))
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
