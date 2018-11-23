using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Jarvis.Vision
{
    public class Scan
    {
        
        public Scan(Image image)
        {
            OriginalImage = image;
        }

        private Image OriginalImage { get; }

        public ScanResult GetResult()
        {
            var result = new ScanResult(OriginalImage);
            for (byte i = 5; i <= 50; i += 5)
            {
                var layer = result.AddLayer(i);
                ScanImage(layer, OriginalImage);
            }
            return result;
        }

        private void ScanImage(Layer layer, Image originalImage)
        {
            var bmp = new Bitmap(originalImage);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var point = new Point(x, y);

                    var pixelData = new PixelData(data);
                    var bucket = layer.AddBucket(pixelData.GetPixel(point), point);

                    FloodFill(pixelData, new Point(x, y), bucket, c => IsInTolerance(c, bucket.BaseColor, layer.Tolerance), layer);
                }
            }
            bmp.UnlockBits(data);
        }

        class PixelData
        {
            private readonly BitmapData _data;

            public PixelData(BitmapData data)
            {
                _data = data;
            }

            public int Width => _data.Width;

            public int Height => _data.Height;

            public unsafe Color GetPixel(Point point)
            {
                var bitsPerPixel = 4;
                var img = (byte*)_data.Scan0.ToPointer();
                img += point.Y * _data.Stride + point.X * bitsPerPixel;
                var a = *img;
                var r = *(img + 1);
                var g = *(img + 2);
                var b = *(img + 3);
                return Color.FromArgb(a, r, g, b);
            }


        }

        private bool IsInTolerance(Color c, Color baseColor, byte tolerance)
        {
            return c.A.InRange(baseColor.A - tolerance, baseColor.A + tolerance);
        }

        private void FloodFill(PixelData pixelData, Point point, PixelBucket bucket, Func<Color, bool> isInBucket, Layer layer)
        {
            var pixelsToScan = new Queue<Point>();
            pixelsToScan.Enqueue(point);
            while (pixelsToScan.Count > 0)
            {
                point = pixelsToScan.Dequeue();

                if (point.X < 0 || point.X > pixelData.Width - 1)
                {
                    continue;
                }

                if (point.Y < 0 || point.Y > pixelData.Height - 1)
                {
                    continue;
                }

                if (layer.Contains(point))
                {
                    continue;
                }

                if (isInBucket(pixelData.GetPixel(point)))
                {
                    bucket.Add(point);
                    pixelsToScan.Enqueue(new Point(point.X - 1, point.Y));
                    pixelsToScan.Enqueue(new Point(point.X + 1, point.Y));
                    pixelsToScan.Enqueue(new Point(point.X, point.Y - 1));
                    pixelsToScan.Enqueue(new Point(point.X, point.Y + 1));
                }
            }
        }
    }
}
