using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Vision.Transforms
{
    internal class NormalHsbFilter : IImageFilter
    {
        public Image Transform(Image i)
            => i;

        public bool IsEdge(Color c, Color previousColor)
        {
            var hueDif = (c.GetHue().CosDeg() - previousColor.GetHue().CosDeg()).Positive();
            
            var satDif = c.GetSaturation() - previousColor.GetSaturation();
            satDif = satDif < 0 ? satDif * -1 : satDif;

            var brightDif = c.GetBrightness() - previousColor.GetBrightness();
            brightDif = brightDif < 0 ? brightDif * -1 : brightDif;

            return hueDif < 0.01 && satDif < 0.01 && brightDif < 0.01;
        }

       
    }
}
