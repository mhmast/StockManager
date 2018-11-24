using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Jarvis.Vision
{
    public class ScanResult
    {
        public List<Layer> Layers { get; } = new List<Layer>();
        public Image OriginalImage { get; }
        public Bitmap FilteredImage { get; }

        public ScanResult(Image originalImage, Bitmap filteredImage)
        {
            OriginalImage = originalImage;
            FilteredImage = filteredImage;
        }

        public Layer AddLayer(byte tolerance)
        {
            var layer = new Layer(tolerance,OriginalImage.Size);
            Layers.Add(layer);
            return layer;
        }
    }
}
