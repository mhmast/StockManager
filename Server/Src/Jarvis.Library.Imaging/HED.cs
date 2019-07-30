//using Emgu.CV;
//using Emgu.CV.Dnn;
//using Emgu.CV.Structure;
//using System.Drawing;

//namespace Jarvis.Library.Imaging
//{
//    public class HED
//    {
//        private Net _net;

//        public HED(string model)
//        {
//            Emgu.CV.dn. 
//            _net = DnnInvoke.ReadNet(model);
          
//        }

//        public IOutputArray DetectEdges(IInputArray image, Size size)
//        {
//            var inp = DnnInvoke.BlobFromImage(image, 1.0, size, new MCvScalar(104.00698793, 116.66876762, 122.67891434), false, false);
//            var layer = new CropLayer();
//            _net.SetInput(layer.getMemoryShapes(inp));
//            return _net.Forward();
//        }
//    }
//}
