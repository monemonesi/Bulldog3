using System;
using System.Collections.Generic;
using System.Drawing;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Dataviz3D
{
    public class GhcHistogramFromSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcHistogramFromSurface class.
        /// </summary>
        public GhcHistogramFromSurface()
          : base("HistogramFromSurface", "HistFromSrf",
              "Create an Histogram like graph from a surface",
              "Bulldog3", "Dataviz3D")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("BaseSrf", "srf", "base surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("Data", "data", "Main data to display", GH_ParamAccess.list);
            pManager.AddPointParameter("BasePoints", "pt", "base point for each data to display", GH_ParamAccess.list);
            pManager.AddColourParameter("Colour 0", "col0", "First reference Colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour 1", "col1", "Second reference Colour", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "Lines", "Histogram 3D lines", GH_ParamAccess.list);
            pManager.AddColourParameter("Colours", "Colours", "Colours for each Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region GetInpuFromCanvas
            Surface inBaseSrf = null;
            bool canGetSurface = DA.GetData(0, ref inBaseSrf);
            inputChecker.DisplayIfConversionFailed(canGetSurface);
            inBaseSrf.SetDomain(0, new Interval(0.0, 1.0));
            inBaseSrf.SetDomain(1, new Interval(0.0, 1.0));

            List<double> inData = new List<double>();
            bool canGetData = DA.GetDataList(1, inData);
            inputChecker.DisplayIfConversionFailed(canGetData);

            List<Point3d> inBasePts = new List<Point3d>();
            bool canConvertPts = DA.GetDataList(2, inBasePts);
            inputChecker.DisplayIfConversionFailed(canConvertPts);
            ValuesAllocator.MatchLists(inData, inBasePts);

            Color inFirstColor = new Color();
            if (!DA.GetData(3, ref inFirstColor)) return;

            Color inSecondColor = new Color();
            if (!DA.GetData(4, ref inSecondColor)) return;
            #endregion

            int lastValueIndex = inData.Count - 1;
            List<double> sortedData = new List<double>(inData);
            sortedData.Sort();
            double refStartDomain = sortedData[0];
            double refEndDomain = sortedData[lastValueIndex];

            List<Line> histogramLines = new List<Line>();
            List<Color> histogramColors = new List<Color>();

            int alphaStartDom = inFirstColor.A;
            int alphaEndDom = inSecondColor.A;
            int redStartDom = inFirstColor.R;
            int redEndDom = inSecondColor.R;
            int blueStartDom = inFirstColor.B;
            int blueEndDom = inSecondColor.B;
            int greenStartDom = inFirstColor.G;
            int greenEndDom = inSecondColor.G;

            for (int i = 0; i < inData.Count; i++)
            {
                Point3d inPt = inBasePts[i];
                double uDir = 0.0;
                double vDir = 0.0;
                if(!inBaseSrf.ClosestPoint(inPt, out uDir, out vDir)) return;
                Plane basePlane = new Plane();
                if(!inBaseSrf.FrameAt(uDir, vDir, out basePlane)) return;

                double dataVal = inData[i];
                Line line = new Line(basePlane.Origin, basePlane.ZAxis, dataVal);
                histogramLines.Add(line);

                int alpha = (int)Remapper.Map(dataVal, refStartDomain, refEndDomain, alphaStartDom, alphaEndDom);
                int red = (int)Remapper.Map(dataVal, refStartDomain, refEndDomain, redStartDom, redEndDom);
                int green = (int)Remapper.Map(dataVal, refStartDomain, refEndDomain, greenStartDom, greenEndDom);
                int blue = (int)Remapper.Map(dataVal, refStartDomain, refEndDomain, blueStartDom, blueEndDom);

                Color color = Color.FromArgb(alpha, red, green, blue);
                histogramColors.Add(color);
            }



            #region SetOutput
            DA.SetDataList(0, histogramLines);
            DA.SetDataList(1, histogramColors);
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
            get { return new Guid("732309a1-c770-4f56-98b6-4b00110e2ed4"); }
        }
    }
}