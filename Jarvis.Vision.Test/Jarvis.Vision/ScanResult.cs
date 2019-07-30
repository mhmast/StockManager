using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using Jarvis.Vision.Transforms;

namespace Jarvis.Vision
{
    public class ScanResult
    {
        public List<Layer> Layers { get; } = new List<Layer>();
        public Image OriginalImage { get; }

        public ScanResult(Image originalImage)
        {
            OriginalImage = originalImage;
            InitLayers(originalImage);
        }

        private void InitLayers(Image originalImage)
        {
           // Layers.Add(new Layer(originalImage,new Laplacian5X5Filter()));
            Layers.Add(new Layer(originalImage,new Median3x3Filter()));
            Layers.Add(new Layer(originalImage,new Prewitt3X3Filter()));
             //Layers.Add(new Layer(originalImage,new Kirsch3X3Filter()));
             Layers.Add(new Layer(originalImage,new NormalHsbFilter()));
            //Layers.Add(new Layer(originalImage,new CombinedFilter(new Median3x3Filter(), new NormalHsbFilter())));
             //Layers.Add(new Layer(originalImage,new CombinedFilter(new Kirsch3X3Filter(),new Prewitt3X3Filter())));
        }
    }
}
