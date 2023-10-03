using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class ShapeFactory
    {

        public Shape getShape(String shapeType)
        {
            if (shapeType == null)
            {
                return null;
            }
            if (shapeType=="CIRCLE")
            {
                return new Circle();

            }
            else if (shapeType=="RECTANGLE")
            {
                return new Rectangle();

            }
            else if (shapeType=="TRIANGLE")
            {
                return new Triangle();
            }

            return null;
        }
    }
}
