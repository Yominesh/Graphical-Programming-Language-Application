using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Assignment1
{
   /// <summary>
   /// This class will draw the circle by getting the values.
   /// </summary>
    class Circle:Shape
    {
        int radius;
        /// <summary>
        /// it receives two parameters and then draws the circle in the panel
        /// </summary>
        /// <param name="g"></param>
        /// <param name="values"></param>
        public override void DrawTo(Graphics g, params int[] values)
        {
            int c1 = values[0];
            int c2 = values[1];
             radius = values[2];
            Pen p = new Pen(Color.Black, 1);
            g.DrawEllipse(p,c1,c2,radius, radius);
        }
    }
}
