using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Jarvis.Vision
{
    public class Scan
    {

        public Scan(Image image)
        {
            OriginalImage = image;
            FilteredImage = new Bitmap(image).ConvolutionFilter(Matrix.Gaussian3x3, Matrix.Gaussian3x3, 1, 0, true);
            //FilteredImage = new Bitmap(image).ConvolutionFilter(Matrix.Prewitt3x3Horizontal,Matrix.Prewitt3x3Vertical,2);
        }

        public Bitmap FilteredImage { get; }

        private Image OriginalImage { get; }

        public ScanResult GetResult()
        {
            var result = new ScanResult(OriginalImage, FilteredImage);
            for (byte i = 0; i <= 1; i++)
            {
                var layer = result.AddLayer(i);
                ScanImage(layer, FilteredImage);
            }
            return result;
        }

        private void ScanImage(Layer layer, Image originalImage)
        {
            var bmp = new Bitmap(originalImage);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var pixelData = new PixelData(data);
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var point = new Point(x, y);
                    if (!layer.Contains(point))
                    {
                        var bucket = layer.AddBucket(pixelData.GetPixel(point), point);
                        FloodFill(pixelData, new Point(x, y), bucket,
                            c => IsColor(c, bucket.BaseColor), layer);
                        if (bucket.Count == 0)
                        {
                            layer.RemoveBucket(bucket);
                        }
                    }
                }
            }
            bmp.UnlockBits(data);
        }


        private bool IsInTolerance(Color c, Color baseColor, byte tolerance)
        {
            return c.A.InRange(baseColor.A - tolerance, baseColor.A + tolerance)
            && c.R.InRange(baseColor.R - tolerance, baseColor.R + tolerance)
            && c.G.InRange(baseColor.G - tolerance, baseColor.G + tolerance)
            && c.B.InRange(baseColor.B - tolerance, baseColor.B + tolerance);
        }

        private bool IsColor(Color c, Color baseColor)
        {
            return baseColor.A != 0 &&c.A != 0 && c.R + c.G + c.B != 765;
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
                    pixelsToScan.Enqueue(new Point(point.X - 1, point.Y - 1));
                    pixelsToScan.Enqueue(new Point(point.X + 1, point.Y + 1));
                    pixelsToScan.Enqueue(new Point(point.X + 1, point.Y - 1));
                    pixelsToScan.Enqueue(new Point(point.X - 1, point.Y + 1));
                }
            }
        }
    }
}
