using System.Drawing;
using System.Windows.Forms;
using Jarvis.Visualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(ColorVisualizer),
    typeof(VisualizerObjectSource),
    Target = typeof(System.Drawing.Color),
    Description = "ColorVisualizer")]
namespace Jarvis.Visualizer
{
    public class ColorVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var backColor = (Color) objectProvider.GetObject();
            var bmp = new Bitmap(100,100);
            var graph = Graphics.FromImage(bmp);
            graph.Clear(backColor);
            new Form {BackgroundImage = bmp}.ShowDialog();
        }
    }
}
