using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

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
            Parallel.ForEach(result.Layers, ScanImage);
            return result;
        }

        private void ScanImage(Layer layer)
        {
            var bmp = new Bitmap(layer.TransformedImage);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var pixelData = new PixelData(data);
            var dataSurfacePerc = data.Width * data.Height / 20000;
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var point = new Point(x, y);
                    if (!layer.Contains(point))
                    {
                        var bucket = layer.AddBucket(pixelData.GetPixel(point), point);
                        FloodFill(pixelData, new Point(x, y), bucket, layer);
                        if (bucket.Count < dataSurfacePerc)//remove less than .05 perc of the image
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

        //private bool IsColor(Color c, Color baseColor)
        //{
        //    return baseColor.A != 0 &&;
        //}

        private void FloodFill(PixelData pixelData, Point point, PixelBucket bucket, Layer layer)
        {
            var pixelsToScan = new Queue<Tuple<Color, Point>>();
            pixelsToScan.Enqueue(new Tuple<Color, Point>(bucket.BaseColor, point));

            while (pixelsToScan.Count > 0)
            {
                var pointColor = pixelsToScan.Dequeue();
                var prevColor = pointColor.Item1;
                point = pointColor.Item2;

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

                var color = pixelData.GetPixel(point);
                if (bucket.Contains(color, prevColor))
                {

                    bucket.Add(point);
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X - 1, point.Y)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X + 1, point.Y)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X, point.Y - 1)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X, point.Y + 1)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X - 1, point.Y - 1)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X + 1, point.Y + 1)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X + 1, point.Y - 1)));
                    pixelsToScan.Enqueue(new Tuple<Color, Point>(color, new Point(point.X - 1, point.Y + 1)));
                }
            }
        }
    }
}
