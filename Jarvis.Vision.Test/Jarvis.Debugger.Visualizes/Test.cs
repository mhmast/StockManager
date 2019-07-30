using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace Jarvis.Debugger.Visualizes
{ 
    public static class Test
    {
        public static void Main()
        {
            VisualizerDevelopmentHost myHost = new VisualizerDevelopmentHost(Color.Aqua, typeof(ColorVisualizer));
            myHost.ShowVisualizer();
        }
    }
}
