using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    public static class ValuesAllocator
    {
        #region AllocateTolists
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

        #endregion

        #region AllocateGH_StructureFromCurves
        public static GH_Structure<GH_Integer> IntegerDSFromCurves(GH_Structure<GH_Curve> refGhCurves, GH_Structure<GH_Integer> inputIntegers, GH_Structure<GH_Integer> outputIntegers)
        {
            bool curvesTopoEqualCornersTopo = refGhCurves.TopologyDescription.Equals(inputIntegers.TopologyDescription);
            if (curvesTopoEqualCornersTopo)
            {
                outputIntegers = inputIntegers.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in refGhCurves.Paths)
                {
                    for (int i = 0; i < refGhCurves.get_Branch(ghPath).Count; i++)
                    {
                        outputIntegers.Insert(inputIntegers.get_LastItem(true), ghPath, i);
                    }
                }
            }

            return outputIntegers;
        }

        public static GH_Structure<GH_Plane> PlanesDSFromCurves(GH_Structure<GH_Curve> refGhCurves, GH_Structure<GH_Plane> inputPlanes, GH_Structure<GH_Plane> outputPlanes)
        {
            bool planesTopoEqualCurvesTopo = refGhCurves.TopologyDescription.Equals(inputPlanes.TopologyDescription);
            if (planesTopoEqualCurvesTopo)
            {
                outputPlanes = inputPlanes.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in refGhCurves.Paths)
                {
                    for (int i = 0; i < refGhCurves.get_Branch(ghPath).Count; i++)
                    {
                        outputPlanes.Insert(inputPlanes.get_LastItem(true), ghPath, i);
                    }
                }
            }

            return outputPlanes;
        }

        public static GH_Structure<GH_Number> NumsDSFromCurves(GH_Structure<GH_Curve> refGhCurves, GH_Structure<GH_Number> inputNumbers, GH_Structure<GH_Number> outputDistances)
        {
            bool curvesTopoEqualDistancesTopo = refGhCurves.TopologyDescription.Equals(inputNumbers.TopologyDescription);

            if (curvesTopoEqualDistancesTopo)
            {
                outputDistances = inputNumbers.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in refGhCurves.Paths)
                {
                    for (int i = 0; i < refGhCurves.get_Branch(ghPath).Count; i++)
                    {
                        outputDistances.Insert(inputNumbers.get_LastItem(true), ghPath, i);
                    }
                }
            }

            return outputDistances;
        }


        #endregion

        #region AllocateGH_StructureFromBreps

        public static GH_Structure<GH_Boolean> BoolDSFromBreps(GH_Structure<GH_Brep> inGhBreps, GH_Structure<GH_Boolean> inGhBool, GH_Structure<GH_Boolean> outGhBool)
        {
            bool brepTopologyEqualBoolTopology = inGhBool.TopologyDescription.Equals(inGhBreps.TopologyDescription);
            if (brepTopologyEqualBoolTopology)
            {
                outGhBool = inGhBool.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in inGhBreps.Paths)
                {
                    for (int i = 0; i < inGhBreps.get_Branch(ghPath).Count; i++)
                    {
                        outGhBool.Insert(inGhBool.get_LastItem(true), ghPath, i);
                    }
                }
            }

            return outGhBool;
        }

        public static GH_Structure<GH_Number> NumbersDSFromBreps(GH_Structure<GH_Brep> inGhBreps, GH_Structure<GH_Number> inGhNums, GH_Structure<GH_Number> outGhNums)
        {
            bool brepTopoEqualNumsTopo = inGhNums.TopologyDescription.Equals(inGhBreps.TopologyDescription);
            if (brepTopoEqualNumsTopo)
            {
                outGhNums = inGhNums.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in inGhBreps.Paths)
                {
                    for (int i = 0; i < inGhBreps.get_Branch(ghPath).Count; i++)
                    {
                        outGhNums.Insert(inGhNums.get_LastItem(true), ghPath, i);
                    }
                }
            }

            return outGhNums;
        }


        #endregion
    }
}
