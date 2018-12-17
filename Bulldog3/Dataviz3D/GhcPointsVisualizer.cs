using System;
using System.Collections.Generic;
using System.Drawing;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Dataviz3D
{
    public class GhcPointsVisualizer : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcPointsVisualizer class.
        /// </summary>
        public GhcPointsVisualizer()
          : base("PointsVisualizer", "PtsViz",
              "Visual points with gradient distance",
              "Bulldog3", "Dataviz3D")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "Points To Show", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Reference planes", "pln", "Reference planes", GH_ParamAccess.list, Plane.WorldXY);
            pManager.AddNumberParameter("Range", "r", "Pt influence range", GH_ParamAccess.list, 1.00);
            pManager.AddIntegerParameter("Density", "d", "Filed Density", GH_ParamAccess.list, 10);
            pManager.AddColourParameter("Colour 0", "col0", "First reference Colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour 1", "col1", "Second reference Colour", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Field Crv", "crv", "Crvs representing the field", GH_ParamAccess.list);
            pManager.AddColourParameter("Colours", "col", "Colours for each curve", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region AssignInputs
            List<Point3d> inPts = new List<Point3d>();
            bool canGetPts = DA.GetDataList(0, inPts);
            inputChecker.StopIfConversionIsFailed(canGetPts);

            List<Plane> inPlanes = new List<Plane>();
            bool canGetPlns = DA.GetDataList(1, inPlanes);
            inputChecker.StopIfConversionIsFailed(canGetPlns);
            ValuesAllocator.MatchLists(inPts, inPlanes);

            List<double> inRange = new List<double>();
            bool canGetRange = DA.GetDataList(2, inRange);
            inputChecker.StopIfConversionIsFailed(canGetRange);
            ValuesAllocator.MatchLists(inPts, inRange);

            List<int> inDensity = new List<int>();
            bool canGetDensity = DA.GetDataList(3, inDensity);
            inputChecker.StopIfConversionIsFailed(canGetDensity);
            ValuesAllocator.MatchLists(inPts, inDensity);

            Color inFirstColor = new Color();
            bool canGetFirstCol = DA.GetData(4, ref inFirstColor);
            inputChecker.StopIfConversionIsFailed(canGetFirstCol);

            Color inSecondColor = new Color();
            bool canGetSecondColor = DA.GetData(5, ref inSecondColor);
            inputChecker.StopIfConversionIsFailed(canGetSecondColor);
            #endregion

            List<Circle> crvsOut = new List<Circle>();
            List<Color> crvsColor = new List<Color>();

            int alphaStartDom = inFirstColor.A;
            int alphaEndDom = inSecondColor.A;
            int redStartDom = inFirstColor.R;
            int redEndDom = inSecondColor.R;
            int blueStartDom = inFirstColor.B;
            int blueEndDom = inSecondColor.B;
            int greenStartDom = inFirstColor.G;
            int greenEndDom = inSecondColor.G;

            for (int i = 0; i < inPts.Count; i++)
            {
                Plane refPlane = inPlanes[i];
                double maxRad = inRange[i];
                int density = inDensity[i] > 0 ? inDensity[i] : 1;
                double distance = maxRad / density;
                double counter = distance;
                while(counter < maxRad)
                {
                    crvsOut.Add(new Circle(refPlane, inPts[i], counter));

                    int alpha = (int)Remapper.Map(counter, distance, maxRad, alphaStartDom, alphaEndDom);
                    int red = (int)Remapper.Map(counter, distance, maxRad, redStartDom, redEndDom);
                    int green = (int)Remapper.Map(counter, distance, maxRad, greenStartDom, greenEndDom);
                    int blue = (int)Remapper.Map(counter, distance, maxRad, blueStartDom, blueEndDom);

                    Color color = Color.FromArgb(alpha, red, green, blue);
                    crvsColor.Add(color);

                    counter += distance;
                }

            }

            #region SendOutputsToCanvas
            DA.SetDataList(0, crvsOut);
            DA.SetDataList(1, crvsColor);

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
            get { return new Guid("77d6b8ce-284f-4c9c-9745-6b8755b0fefb"); }
        }
    }
}