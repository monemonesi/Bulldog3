using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Optimization
{
    /// <summary>
    /// Subdivide a given curve using De Casteljau`s algorithm
    /// </summary>
    public class GhcAdaptiveCurveSubD : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcAdaptiveCurveSubD class.
        /// </summary>
        public GhcAdaptiveCurveSubD()
          : base("AdaptiveCurveSubD", "AdaptiveCurveSubD",
              "Using De Casteljau's algorithm to create polylines from Curves",
              "Bulldog3", "Optimization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "crvs", "Curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Threshold", "Thre", "Threshold", GH_ParamAccess.item, 0.01);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "pl", "Resulting Polylines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inCurves = new List<Curve>();
            if (!DA.GetDataList(0, inCurves))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
                return;
            }
            double tolerance = Constants.Constants.STANDARD_TOLLERANCE;
            DA.GetData(1, ref tolerance);

            List<Curve> optimizedCurves = new List<Curve>();
            
            foreach (var crv in inCurves)
            {

                SortedList<double, Point3d> separationPts = new SortedList<double, Point3d>();
                crv.Domain = new Interval(0.0, 1.0);

                CurvesOptimizer.DeCasteljauCurvePts(crv, 0.0, 1.0, tolerance, separationPts);

                optimizedCurves.Add(new PolylineCurve(separationPts.Values));
            }

            DA.SetDataList(0, optimizedCurves);

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
            get { return new Guid("34998bb8-e193-4fd7-8022-ae3238ab7fa2"); }
        }
    }
}