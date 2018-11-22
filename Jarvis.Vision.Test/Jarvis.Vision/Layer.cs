using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jarvis.Vision
{
    public class Layer
    {
        public Layer(byte tolerance)
        {
            Tolerance = tolerance;
        }                             

        public byte Tolerance { get; } 

        public List<PixelBucket> Buckets { get; } = new List<PixelBucket>();

        public PixelBucket AddBucket(Color baseColor, Point basePoint)
        {
            var bucket = new PixelBucket(baseColor, basePoint);
            Buckets.Add(bucket);
            return bucket;
        }

        internal bool Contains(Point point)
        {
            foreach(var bucket in Buckets)
            {
                if(bucket.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
