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
    /// <summary>
    /// This class check the inputs` data structure to guarantee a correct match
    /// </summary>
    public static class ValuesAllocator
    {
        #region AllocateTolists
        /// <summary>
        /// If two lists are not matching calls AssignFromListsToLists to solve
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="listToFill"></param>
        public static void MatchLists(IList reference, IList listToFill)
        {
            if (listToFill.Count < reference.Count)
            {
                AssignFromListToList(reference, listToFill);
            }
        }

        /// <summary>
        /// Fill the shorten List to match 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="listToFill"></param>
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
        /// <summary>
        /// Match the number of values of two given data structure, modifing the one with less elements. GH_Curve => GH_Integer
        /// </summary>
        /// <param name="refGhCurves"></param>
        /// <param name="inputIntegers"></param>
        /// <param name="outputIntegers"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Match the number of values of two given data structure, modifing the one with less elements. GH_Curve => GH_Plane
        /// </summary>
        /// <param name="refGhCurves"></param>
        /// <param name="inputPlanes"></param>
        /// <param name="outputPlanes"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Match the number of values of two given data structure, modifing the one with less elements. GH_Curve => GH_Number
        /// </summary>
        /// <param name="refGhCurves"></param>
        /// <param name="inputNumbers"></param>
        /// <param name="outputDistances"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Match the number of values of two given data structure, modifing the one with less elements. GH_Breps => GH_Boolean
        /// </summary>
        /// <param name="inGhBreps"></param>
        /// <param name="inGhBool"></param>
        /// <param name="outGhBool"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Match the number of values of two given data structure, modifing the one with less elements. GH_Breps => GH_Numbers
        /// </summary>
        /// <param name="inGhBreps"></param>
        /// <param name="inGhNums"></param>
        /// <param name="outGhNums"></param>
        /// <returns></returns>
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
