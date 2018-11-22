using System.Collections.Generic;
using System.Drawing;

namespace Jarvis.Vision
{
    public class ScanResult
    {
        public List<Layer> Layers { get; } = new List<Layer>();
        public Image OriginalImage { get; }

        public ScanResult(Image originalImage)
        {
            OriginalImage = originalImage;
        }

        public Layer AddLayer(byte tolerance)
        {
            var layer = new Layer(tolerance);
            Layers.Add(layer);
            return layer;
        }
    }
}
