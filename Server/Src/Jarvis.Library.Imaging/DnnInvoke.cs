//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Emgu.CV.Dnn
//{
//    public static  partial class DnnInvoke
//    {
//        public static void RegisterLayer(string name,Layer layer)
//        {
//            using (var nameStr = new CvString(name))
//            {
//                dnn_registerLayer(nameStr.Ptr,layer.)
//            }
//        }

//        [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
//        internal static extern void dnn_registerLayer(IntPtr name,);
//    }
//}
