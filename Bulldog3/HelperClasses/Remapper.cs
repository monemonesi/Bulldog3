using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    public static class Remapper
    {
        public static double Map(double value, double start1, double stop1, int start2, int stop2)
        {
            double remappedVal = 0.0;

            remappedVal = start2 + (stop2 - start2) *((value - start1) / (stop1 - start1));

            return remappedVal;
        }

        public static double Map(double value, Interval refDom, Interval targetDom)
        {

            double start1 = refDom.T0;
            double stop1 = refDom.T1;
            int start2 = (int) targetDom.T0;
            int stop2 = (int) targetDom.T1;

            return Map(value,start1,stop1,start2,stop2);
        }
    }
}
