using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.Models
{
    public static class SeriesFromLists
    {
        public static List<double> SeriesFromObjectLists(List<object> referenceList, double startingVal, double step)
        {
            List<double> outList = new List<double>();
            double counter = startingVal;
            //outList.Add(counter);
            foreach (var item in referenceList)
            {
                outList.Add(counter);
                counter += step;
            }

            return outList;
        }
    }
}
