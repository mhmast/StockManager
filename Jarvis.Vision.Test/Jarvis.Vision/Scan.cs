using Jarvis.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Jarvis.Vision
{
    public class Scan2
    {
        private Image _originalImage;

        public Scan2(Image image)
        {
            _originalImage = image;
        }

        public ScanResult GetResult()
        {
            var result = new ScanResult(_originalImage);
            for (byte i = 0; i < 12; i++)
            {
                var layer = result.AddLayer(i);
                ScanImage(layer, _originalImage);
            }
            return result;
        }

        private void ScanImage(Layer layer, Image originalImage)
        {
            var bmp = new Bitmap(originalImage);
            List<Point> pixelsToScan = new List<Point> { new Point(0, 0) };
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
        }

        class PixelData
        {
            private BitmapData _data;

            public PixelData(BitmapData data)
            {
                _data = data;
            }

            public int Width => _data.Width;

            public int Height => _data.Height;

            public Color GetPixel(Point point)
            {
                unsafe
                {
                    byte* img = (byte*)_data.Scan0;
                    img += point.Y * _data.Stride + ((point.X * 4) - 1);
                    var a = *img;
                    var r = *(img + 1);
                    var g = *(img + 2);
                    var b = *(img + 3);
                    return Color.FromArgb(a, r, g, b);
                }
            }

        }

        private bool IsInTolerance(Color c, Color baseColor, byte tolerance)
        {
            return c.A.InRange(baseColor.A - tolerance, baseColor.A + tolerance);
        }

        private void FloodFill(PixelData pixelData, Point point, PixelBucket bucket, Func<Color, bool> isInBucket, Layer layer)
        {
            if (point.X < 0 || point.X > pixelData.Width)
            {
                return;
            }

            if (point.Y < 0 || point.Y > pixelData.Height)
            {
                return;
            }
            if (layer.Contains(point))
            {
                return;
            }
            if (isInBucket(pixelData.GetPixel(point)))
            {
                bucket.Add(point);
                FloodFill(pixelData, new Point(point.X - 1, point.Y), bucket, isInBucket, layer);
                FloodFill(pixelData, new Point(point.X + 1, point.Y), bucket, isInBucket, layer);
                FloodFill(pixelData, new Point(point.X, point.Y - 1), bucket, isInBucket, layer);
                FloodFill(pixelData, new Point(point.X, point.Y + 1), bucket, isInBucket, layer);
            }
        }
    }
}
