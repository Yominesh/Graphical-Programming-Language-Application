using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Assignment1
{
    class Rectangle : Shape
    {
        int c1, c2, height, width;
        public override void DrawTo(Graphics g, params int[] values)
        {
            c1 = values[0];
            c2 = values[1];
            height = values[2];
            width = values[3];
            Pen p = new Pen(Color.Black, 1);
            g.DrawRectangle(p, c1, c2, height, width);
        }
    }
}
