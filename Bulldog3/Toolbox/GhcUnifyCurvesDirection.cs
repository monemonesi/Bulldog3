using System;
using System.Collections.Generic;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcUnifyCurvesDirection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcUnifyCurvesDirection class.
        /// </summary>
        public GhcUnifyCurvesDirection()
          : base("UnifyCurveDirections", "UnifyCurveDirections",
                "Unify planar curve direction (Clockwise) based on a plane",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "c", "Closed planar curves to be uniformed", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Planes", "P", "Reference plane", GH_ParamAccess.list, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Oriented Curves", "c", "Oriented Curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inCurves = new List<Curve>();
            DA.GetDataList<Curve>("Curves", inCurves);
            List<Plane> inPlanes = new List<Plane>();
            DA.GetDataList<Plane>("Planes", inPlanes);
            ValuesAllocator.MatchLists(inCurves, inPlanes);
            List<Curve> orientedCurves = new List<Curve>();
            for (int i = 0; i < inCurves.Count; i++)
            {
                if(inCurves[i].ClosedCurveOrientation(inPlanes[i]) == CurveOrientation.Clockwise)
                {
                    inCurves[i].Reverse();
                }
                orientedCurves.Add(inCurves[i]);
            }
            DA.SetDataList(0, orientedCurves);
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
            get { return new Guid("70a0d2ad-2dda-4946-ba45-0e7f5133ea05"); }
        }
    }
}