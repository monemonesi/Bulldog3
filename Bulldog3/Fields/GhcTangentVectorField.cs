using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Fields
{
    public class GhcTangentVectorField : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcVectorField class.
        /// </summary>
        public GhcTangentVectorField()
          : base("TangentVectorField", "TanField",
              "Given a series of points and curves defines the tangent vector field",
              "Bulldog3", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Base Vectorfield pts", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves", "Crv", "Vectorfield curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Vectors", "Vec", "Resulting vectors", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "d", "Distances from the curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region GetInputFromCanvas

            List<Point3d> inPts = new List<Point3d>();
            bool canGetPts = DA.GetDataList(0, inPts);
            inputChecker.StopIfConversionIsFailed(canGetPts);

            List<Curve> inCurves = new List<Curve>();
            bool canGetCrvs = DA.GetDataList(1, inCurves);
            inputChecker.StopIfConversionIsFailed(canGetCrvs);

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
            get { return new Guid("4a150c21-3e91-49ed-88fc-0b4ce3d988b3"); }
        }
    }
}