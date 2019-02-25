using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    /// <summary>
    /// Remove the curves shorter then the threshold
    /// </summary>
    public class GhcCullShortCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcCullShortCurves class.
        /// </summary>
        public GhcCullShortCurves()
          : base("CullShortCurves", "CullShort",
              "Remove the curves shorter then the threshold",
              "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "crv", "Curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Threshold", "th", "Threshold", GH_ParamAccess.list, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Culled Curves", "crv", "Curves longer then the specified th", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            List<Curve> inCurves = new List<Curve>();
            List<double> inThres = new List<double>();
            bool succes = DA.GetDataList<Curve>(0, inCurves) && DA.GetDataList<double>(1, inThres);

            inputChecker.StopIfConversionIsFailed(succes);
            ValuesAllocator.MatchLists(inCurves, inThres);

            List<Curve> culledCrvs = CurveProcessor.CullShortCrv(inCurves, inThres);

            #region SendOutputsToCanvas

            DA.SetDataList(0, culledCrvs);

            #endregion
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
            get { return new Guid("760cbf6a-e92a-4737-83fb-ae3dadf3d73e"); }
        }
    }
}