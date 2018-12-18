using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// Class used to remap numeric values
    /// </summary>
    public static class Remapper
    {
        /// <summary>
        /// Remap a numeric values given the boundary conditions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start1"></param>
        /// <param name="stop1"></param>
        /// <param name="start2"></param>
        /// <param name="stop2"></param>
        /// <returns></returns>
        public static double Map(double value, double start1, double stop1, int start2, int stop2)
        {
            double remappedVal = 0.0;

            remappedVal = start2 + (stop2 - start2) *((value - start1) / (stop1 - start1));

            return remappedVal;
        }


        /// <summary>
        /// Remap a numeric value given the starting and ending reference domains
        /// </summary>
        /// <param name="value"></param>
        /// <param name="refDom"></param>
        /// <param name="targetDom"></param>
        /// <returns></returns>
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
