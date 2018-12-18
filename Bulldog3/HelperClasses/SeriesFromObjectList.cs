using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// Create a numeric series from object lists
    /// </summary>
    public static class SeriesFromObjectList
    {
        /// <summary>
        /// Given a referenceList of object it creates the resulting list with specified starting values and step
        /// </summary>
        /// <param name="referenceList"></param>
        /// <param name="startingVal"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static List<double> SeriesFromObjectLists(List<object> referenceList, double startingVal, double step)
        {
            List<double> outList = new List<double>();
            double counter = startingVal;

            foreach (var item in referenceList)
            {
                outList.Add(counter);
                counter += step;
            }

            return outList;
        }
    }
}
