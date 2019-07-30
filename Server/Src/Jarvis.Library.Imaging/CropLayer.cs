//using Emgu.CV;
//using Emgu.CV.Dnn;
//using System.Linq;

//namespace Jarvis.Library.Imaging
//{
//    public class CropLayer 
//    {
//        private int xstart;
//        private int xend;
//        private int ystart;
//        private int yend;
//        public CropLayer()
//        {
//            this.xstart = 0;
//            this.xend = 0;
//            this.ystart = 0;
//            this.yend = 0;
//        }

//        //Our layer receives two inputs. We need to crop the first input blob
//        //to match a shape of the second one (keeping batch size and number of channels)
//        public IInputArray getMemoryShapes(IInputArray inputs)
//        {
//            var inputMat = inputs.GetInputArray().GetMat();
//            var inputShape = inputMat.Col(0);
//            var targetShape = inputMat.Col(1);
//            var batchSize = inputShape.Row(0);
//            var numChannels = inputShape.Row(1);
//            var height = targetShape[2];
//            var width = targetShape[3];

//            ystart = (inputShape[2] - targetShape[2]); // 2
//            xstart = (inputShape[3] - targetShape[3]); // 2
//            yend = ystart + height;
//            xend = xstart + width;

//            var output = new Mat();
//            return new[] { new[] { batchSize, numChannels, height, width } };
//        }

//        public int[][] forward(int[][] inputs)
//        {
//            var input = inputs[0];
//            return new[] { input, input, input.Skip(ystart).Take(yend - ystart).ToArray(), input.Skip(xstart).Take(xend - xstart).ToArray() };
//        }
//    }
//}

