using System;
using System.Collections.Generic;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcDoubleOffset : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcDoubleCurveOffset class.
        /// </summary>
        public GhcDoubleOffset()
          : base("Doubleoffset", "Doubleoffset",
                "Make the offset in both direction",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curve to offset", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Distance", "D", "Offsett Distance", GH_ParamAccess.tree, 1);
            pManager.AddPlaneParameter("Planes", "P", "reference plane", GH_ParamAccess.tree, Plane.WorldXY);
            pManager.AddIntegerParameter("Corners", "C", "corner type", GH_ParamAccess.tree, 1);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region get Input From GH Canvas
            GH_Structure<GH_Curve> inCurves = new GH_Structure<GH_Curve>();
            bool areCurvesOK = DA.GetDataTree(0, out inCurves);
            inputChecker.DisplayIfConversionFailed(areCurvesOK);
            GH_Structure<GH_Curve> ghCurves = new GH_Structure<GH_Curve>();
            ghCurves = inCurves.Duplicate();
            ghCurves.Graft(GH_GraftMode.GraftAll);
            ghCurves.Simplify(GH_SimplificationMode.CollapseAllOverlaps);

            GH_Structure<GH_Number> inDistances = new GH_Structure<GH_Number>();
            bool areDistancesOk = DA.GetDataTree(1, out inDistances);
            inputChecker.DisplayIfConversionFailed(areDistancesOk);
            GH_Structure<GH_Number> ghDistances = new GH_Structure<GH_Number>();
            ghDistances = ValuesAllocator.NumsDSFromCurves(ghCurves, inDistances, ghDistances);

            GH_Structure<GH_Plane> inPlanes = new GH_Structure<GH_Plane>();
            bool arePlanesOk = DA.GetDataTree(2, out inPlanes);
            inputChecker.DisplayIfConversionFailed(arePlanesOk);
            GH_Structure<GH_Plane> ghPlanes = new GH_Structure<GH_Plane>();
            ghPlanes = ValuesAllocator.PlanesDSFromCurves(ghCurves, inPlanes, ghPlanes);

            GH_Structure<GH_Integer> inCorners = new GH_Structure<GH_Integer>();
            bool areCornerssOk = DA.GetDataTree(3, out inCorners);
            inputChecker.DisplayIfConversionFailed(areCornerssOk);
            GH_Structure<GH_Integer> ghCorners = new GH_Structure<GH_Integer>();
            ghCorners = ValuesAllocator.IntegerDSFromCurves(ghCurves, inCorners, ghCorners);
            #endregion

            GH_Structure<GH_Curve> ghCurveOffset = new GH_Structure<GH_Curve>();
            double docTollerance = DocumentTolerance();

            int pathIndex = 0;
            foreach (GH_Path ghPath in ghCurves.Paths)
            {
                for (int i = 0; i < ghCurves.get_Branch(ghPath).Count; i++)
                {
                    CurveOffsetCornerStyle cornerStyle = CurveOffsetCornerStyle.None;
                    int cornerStyleInInt = ghCorners.get_DataItem(ghPath, i).Value;
                    GetCornerStyle(ref cornerStyleInInt, ref cornerStyle);

                    Curve crv = ghCurves.get_DataItem(ghPath, i).Value;
                    Plane plane = ghPlanes.get_DataItem(ghPath, i).Value;
                    double dist = ghDistances.get_DataItem(ghPath, i).Value;

                    List<Curve> resultingCurves = new List<Curve>();

                    resultingCurves.AddRange(crv.Offset(plane, dist, docTollerance, cornerStyle));
                    resultingCurves.AddRange(crv.Offset(plane, dist *= -1, docTollerance, cornerStyle));
                    foreach (Curve resultingCrv in resultingCurves)
                    {
                        GH_Curve ghResultingCrv = null;
                        if (GH_Convert.ToGHCurve(resultingCrv, GH_Conversion.Both, ref ghResultingCrv))
                        {
                            ghCurveOffset.Append(ghResultingCrv, ghPath);
                        }

                    }
                }
                pathIndex++;
            }

            DA.SetDataTree(0, ghCurveOffset);
        }


        
        #region helper methods
        private static void GetCornerStyle(ref int cornerStyleInInt, ref CurveOffsetCornerStyle cornerStyle)
        {
            if (cornerStyleInInt < 0)
                cornerStyleInInt = 0;
            if (cornerStyleInInt > 4)
                cornerStyleInInt = 4;
            switch (cornerStyleInInt)
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
        }
        #endregion
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