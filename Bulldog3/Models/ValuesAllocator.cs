using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.Models
{
    public static class ValuesAllocator
    {

        public static void MatchLists(IList reference, IList listToFill)
        {
            if (listToFill.Count < reference.Count)
            {
                AssignFromListToList(reference, listToFill);
            }
        }

        private static void AssignFromListToList(IList reference, IList listToFill)
        {
            int count = reference.Count - listToFill.Count;
            for (int i = 0; i < count; i++)
            {
                listToFill.Add(listToFill[listToFill.Count - 1]);
            }
        }


    }
}
