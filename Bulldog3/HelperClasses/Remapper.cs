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
        public static double Map(double var, double minRef, double maxRef, int minTarget, int maxTarget)
        {
            double remappedVal = 0;

            remappedVal = minTarget + ((var - minRef) * (maxTarget - minTarget) / (maxRef - minRef));

            return remappedVal;
        }

        public static double Map(double var, Interval refDom, Interval targetDom)
        {
            double remappedVal = 0.0;

            double minRef = refDom.T0;
            double maxRef = refDom.T1;
            double minTarget = targetDom.T0;
            double maxTarget = targetDom.T1;

            remappedVal = minTarget + ((var - minRef) * (maxTarget - minTarget) / (maxRef - minRef));

            return remappedVal;
        }
    }
}
