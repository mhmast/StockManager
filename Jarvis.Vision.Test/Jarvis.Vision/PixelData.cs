using System.Drawing;
using System.Drawing.Imaging;

namespace Jarvis.Vision
{
    public class PixelData
    {
        private readonly BitmapData _data;

        public PixelData(BitmapData data)
        {
            _data = data;
        }

        public int Width => _data.Width;

        public int Height => _data.Height;

        public unsafe Color GetPixel(Point pixel)
        {
            var bitsPerPixel = 4;
            var img = (byte*)_data.Scan0.ToPointer();
            img += pixel.Y * _data.Stride + pixel.X * bitsPerPixel;
            var a = *img;
            var r = *(img + 1);
            var g = *(img + 2);
            var b = *(img + 3);
            return Color.FromArgb(a, r, g, b);
        }


        public unsafe void SetPixel(Point pixel, Color color)
        {
            var bitsPerPixel = 4;
            var img = (byte*)_data.Scan0.ToPointer();
            img += pixel.Y * _data.Stride + pixel.X * bitsPerPixel;
            *img = color.A;
            *(img + 1) = color.R;
            *(img + 2) = color.G;
            *(img + 3) = color.B;
        }
    }
}