using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Vision
{
    public static class NumberExtensions
    {
        public static bool InRange(this byte i,int min, int max)
        {
            return min <= i && i <= max;
        }

        public static int Max(this int i, int max)
            => Math.Min(i, max);
    }
}
