using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Library.Imaging
{
    struct Usan
    {

        //size of USAN area
        public float usan_value { get; set; }

        //center of gravity of USAN area 
        public int core_x { get; set; }
        public int core_y { get; set; }

        //arctan value (radian)
        public float direction { get; set; }
    };
}
