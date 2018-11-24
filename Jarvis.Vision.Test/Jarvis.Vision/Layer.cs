using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Jarvis.Vision
{
    public class Layer : IEnumerable<KeyValuePair<int, IDictionary<int, PixelBucket>>>
    {
        private readonly IDictionary<int, IDictionary<int, PixelBucket>> _points = new Dictionary<int, IDictionary<int, PixelBucket>>();

        public Layer(byte tolerance, Size imageSize)
        {
            Tolerance = tolerance;
            for (var y = 0; y < imageSize.Height; y++)
            {
                var dict = new Dictionary<int, PixelBucket>();
                _points.Add(y, dict);
                for (var x = 0; x < imageSize.Width; x++)
                {
                    dict.Add(x, null);
                }
            }
        }

        public IDictionary<int, PixelBucket> this[int index] => _points[index];

        public byte Tolerance { get; }

        public List<PixelBucket> Buckets { get; } = new List<PixelBucket>();

        public PixelBucket AddBucket(Color baseColor, Point basePoint)
        {
            var bucket = new PixelBucket(baseColor, basePoint, this);
            Buckets.Add(bucket);
            return bucket;
        }

        internal bool Contains(Point point) => _points[point.Y][point.X] != null;
        public IEnumerator<KeyValuePair<int, IDictionary<int, PixelBucket>>> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RemoveBucket(PixelBucket bucket)
        {
            Buckets.Remove(bucket);
        }
    }
}
