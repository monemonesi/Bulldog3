using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bulldog3.HelperClasses;
using Bulldog3.Constants;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Optimization
{
    /// <summary>
    /// GH component to proportionally rebuild Curves based on their control points number
    /// </summary>
    public class GhcRebuildCurvesProportionallyPA : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcRebuildCurvesProportionallyPA class.
        /// </summary>
        public GhcRebuildCurvesProportionallyPA()
          : base("RebuildCurveProportionally", "Rebuild",
                "Rebuild curve proportionally to its control points number",
                "Bulldog3", "Optimization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to simplify", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Degree", "Dist", "Optional degree of curve (if omitted, input degree is used)", GH_ParamAccess.list, -1);
            pManager.AddNumberParameter("Rebuilding factor", "%", "rebuilding factor [0-1]", GH_ParamAccess.list, 0.5);
            pManager.AddBooleanParameter("Preserve end Tangent", "Tan", "preserve curve end tangents", GH_ParamAccess.list, false);
            pManager.AddBooleanParameter("||", "||", "Use Parallel", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Simplified curve", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region GetDataFromCanvas
            List<Curve> inCurves = new List<Curve>();
            bool canGetCurves = DA.GetDataList<Curve>(0, inCurves);
            inputChecker.StopIfConversionIsFailed(canGetCurves);

            List<int> inCurvesDegree = new List<int>();
            bool canGetCrvDegrees = DA.GetDataList<int>(1, inCurvesDegree);
            inputChecker.StopIfConversionIsFailed(canGetCrvDegrees);
            ValuesAllocator.MatchLists(inCurves, inCurvesDegree);

            List<double> inRebuildFactors = new List<double>();
            bool canGetFactors = DA.GetDataList<double>(2, inRebuildFactors);
            inputChecker.StopIfConversionIsFailed(canGetFactors);
            ValuesAllocator.MatchLists(inCurves, inRebuildFactors);

            List<bool> inPreserveTan = new List<bool>();
            bool canGetPreserveTan = DA.GetDataList<bool>(3, inPreserveTan);
            inputChecker.StopIfConversionIsFailed(canGetPreserveTan);
            ValuesAllocator.MatchLists(inCurves, inPreserveTan);

            bool useParallel = false;
            DA.GetData<bool>(4, ref useParallel);
            #endregion

            Curve[] curves = inCurves.ToArray();
            int[] curvesDegree = inCurvesDegree.ToArray();
            double[] rebuildFactors = inRebuildFactors.ToArray();
            bool[] preserveTan = inPreserveTan.ToArray();
            ConcurrentDictionary<int, Curve> rebuildedCurves = new ConcurrentDictionary<int, Curve>();
            if (!useParallel)
            {
                this.Message = Constants.Constants.SERIAL_MESSAGE;
                for (int i = 0; i < (int)curves.Length; i++)
                {
                    CurvesOptimizer.RebuildProportionally(i, curves, curvesDegree, rebuildFactors, preserveTan, rebuildedCurves);
                }
            }
            else
            {
                this.Message = Constants.Constants.PARALLEL_MESSAGE;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, Constants.Constants.PARALLEL_WARNING);

                int processorCount = Environment.ProcessorCount - 1;;
                Parallel.For(0, curves.Length, new ParallelOptions{ MaxDegreeOfParallelism = processorCount },
                    i => 
                        CurvesOptimizer.RebuildProportionally(i, curves, curvesDegree, rebuildFactors, preserveTan, rebuildedCurves)
                    );
            }
            List<Curve> curves1 = new List<Curve>();
            curves1.AddRange(rebuildedCurves.Values);
            DA.SetDataList(0, curves1);



        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a3abe077-12de-454a-98a9-4c7402d42c46"); }
        }
    }
}