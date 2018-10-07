using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcMiddlePT : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcMiddlePT class.
        /// </summary>
        public GhcMiddlePT()
          : base("MiddlePT", "MiddlePT",
                "Find the middle point of curves",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "c", "Curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("MidPoints", "Pt", "MidPoints", GH_ParamAccess.list);
            pManager.AddVectorParameter("TangentVectors", "T", "TangentVectors", GH_ParamAccess.list);
            pManager.AddNumberParameter("globalZ", "Z", "globalZ", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inCurves = new List<Curve>();
            bool dataList = DA.GetDataList<Curve>(0, inCurves);
            List<GH_Point> ghMidPoints = new List<GH_Point>();
            List<Vector3d> tangents = new List<Vector3d>();
            List<double> globalCoordinateZ = new List<double>();
            if (!dataList)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error , "Check your input buddy");
            }
            else
            {
                foreach (Curve curve in inCurves)
                {
                    curve.Domain = new Interval(0, 1);
                    ghMidPoints.Add(new GH_Point(curve.PointAt(0.5)));
                    globalCoordinateZ.Add(curve.PointAt(0.5).Z);
                    tangents.Add(curve.TangentAt(0.5));
                }
                DA.SetDataList(0, ghMidPoints);
                DA.SetDataList(1, tangents);
                DA.SetDataList(2, globalCoordinateZ);
            }
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
            get { return new Guid("4fd76256-d6fa-4b79-a74a-e201e604ec08"); }
        }
    }
}