using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartratePresentator
{
    class Circle
    {
        //test
        public int x { get; set; }
        public int y { get; set; }
        public int r { get; set; }
        public bool onScreen { get; set; }

        public Circle(int x, int y, int r)
        {
            this.x = x;
            this.y = y;
            this.r = r;
            this.onScreen = false;
        }
    }
}
