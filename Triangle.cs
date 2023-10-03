using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Assignment1
{/// <summary>
/// this class will draw the triangle by getting the values
/// </summary>
    class Triangle : Shape


    {
        int c1, c2, h, b, pr;
        /// <summary>
        /// it recieves two parameters and then draws the triangle in the panel
        /// </summary>
        /// <param name="g"></param>
        /// <param name="values"></param>
        public override void DrawTo(Graphics g, params int[] values)
        {
            int c1 = values[0];
            int c2 = values[1];
            int h = values[2];
            int b = values[3];
            int pr = values[4];
            Pen p = new Pen(Color.Black, 1);
            Point[] points = { new Point(c1, c2), new Point(c1 + b, c2), new Point(c1 + b, c2 + pr) };
            g.DrawPolygon(p, points);
        }
    }
}
