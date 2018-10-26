using System;
using System.Collections.Generic;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcDoubleCurveOffset : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcDoubleCurveOffset class.
        /// </summary>
        public GhcDoubleCurveOffset()
          : base("Doubleoffset", "Doubleoffset",
                "Make the offsett in both direction",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //TODO: consider the GH_Structure for more reusable component.
            pManager.AddCurveParameter("Curves", "C", "Curve to offset", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Offsett Distance", GH_ParamAccess.list, 1);
            pManager.AddPlaneParameter("Planes", "P", "reference plane", GH_ParamAccess.list, Plane.WorldXY);
            pManager.AddIntegerParameter("Corners", "C", "corner type", GH_ParamAccess.list, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddCurveParameter("First Curve", "c1", "Internal curve", GH_ParamAccess.list);
            //pManager.AddCurveParameter("Second Curve", "c2", "External curve", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves", "C", "Curves", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inCurves = new List<Curve>();
            if (!DA.GetDataList(0, inCurves)) return;

            List<double> inDistances = new List<double>();
            if (!DA.GetDataList(1, inDistances)) return;
            ValuesAllocator.MatchLists(inCurves, inDistances);

            List<Plane> inPlanes = new List<Plane>();
            if (!DA.GetDataList(2, inPlanes)) return;
            ValuesAllocator.MatchLists(inCurves, inPlanes);

            List<int> inCorners = new List<int>();
            if (!DA.GetDataList(3, inCorners)) return;
            ValuesAllocator.MatchLists(inCurves, inCorners);

            GH_Structure<GH_Curve> ghCurveOffset = new GH_Structure<GH_Curve>();
            double docTollerance = DocumentTolerance();


            for (int i = 0; i < inCurves.Count; i++)
            {
                CurveOffsetCornerStyle cornerStyle = CurveOffsetCornerStyle.None;
          
                if (inCorners[i] < 0)
                {
                    inCorners[i] = 0;
                }
                if (inCorners[i] > 4)
                {
                    inCorners[i] = 4;
                }
                int cornerInInteger = inCorners[i];
                switch (cornerInInteger)
                {
                    case (0):
                        cornerStyle = CurveOffsetCornerStyle.None;
                        break;
                    case (1):
                        cornerStyle = CurveOffsetCornerStyle.Sharp;
                        break;
                    case (2):
                        cornerStyle = CurveOffsetCornerStyle.Round;
                        break;
                    case (3):
                        cornerStyle = CurveOffsetCornerStyle.Smooth;
                        break;
                    case (4):
                        cornerStyle = CurveOffsetCornerStyle.Chamfer;
                        break;
                }

                List<Curve> resultingCurves = new List<Curve>();

                resultingCurves.AddRange(inCurves[i].Offset(inPlanes[i],
                    inDistances[i], docTollerance, cornerStyle));
                resultingCurves.AddRange(inCurves[i].Offset(inPlanes[i],
                    inDistances[i] *= -1, docTollerance, cornerStyle));

                GH_Path pathIndex = new GH_Path(i);
                foreach (var curve in resultingCurves)
                {
                    GH_Curve ghResultingCurves = null;
                    if(GH_Convert.ToGHCurve(curve, GH_Conversion.Both, ref ghResultingCurves))
                    {
                        ghCurveOffset.Append(ghResultingCurves, new GH_Path(i));
                    }
                }

            }

            //DA.SetDataList(0, internalCrvs);
            //DA.SetDataList(1, externalCrvs);
            DA.SetDataTree(0, ghCurveOffset);
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
            get { return new Guid("1d634856-c1e1-4d78-8fae-ab92e0d63277"); }
        }
    }
}