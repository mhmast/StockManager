using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Jarvis.Vision.Transforms;

namespace Jarvis.Vision
{
    public class Layer : IEnumerable<KeyValuePair<int, IDictionary<int, PixelBucket>>>
    {
        private readonly IDictionary<int, IDictionary<int, PixelBucket>> _points = new Dictionary<int, IDictionary<int, PixelBucket>>();
        public Image TransformedImage { get; }
        
        public Layer(Image originalImage,IImageFilter filter)
        {
            Filter = filter;
            Name = filter.GetType().Name;
            TransformedImage = filter.Transform(originalImage);
            
            for (var y = 0; y < originalImage.Height; y++)
            {
                var dict = new Dictionary<int, PixelBucket>();
                _points.Add(y, dict);
                for (var x = 0; x < originalImage.Width; x++)
                {
                    dict.Add(x, null);
                }
            }
        }

        public IDictionary<int, PixelBucket> this[int index] => _points[index];

        public List<PixelBucket> Buckets { get; } = new List<PixelBucket>();
        public string Name { get; }
        public IImageFilter Filter { get; }

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
