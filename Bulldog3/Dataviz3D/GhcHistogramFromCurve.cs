using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Geometry;

namespace Bulldog3.Dataviz3D
{
    public class GhcHistogramFromCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcHistogramFromCurve class.
        /// </summary>
        public GhcHistogramFromCurve()
          : base("HistogramFromCurve", "HistFromCurve",
              "Create an Histogram like graph from 3D curve",
              "Bulldog3", "Dataviz3D")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("BaseCurve", "crv", "base curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Data", "data", "Main data to visualize", GH_ParamAccess.list);
            pManager.AddNumberParameter("OptionalData", "dataOp", "Secondary data / Curve subdivision", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddColourParameter("Colour 0", "col0", "First reference Colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour 1", "col1", "Second reference Colour", GH_ParamAccess.item);
            pManager.AddAngleParameter("RotationAngleInDegree", "degA", "Rotation angle for the graph", GH_ParamAccess.list, 0.00);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "Lines", "Histogram 3D lines", GH_ParamAccess.list);
            pManager.AddColourParameter("Colours", "Colours", "Colours for each Line", GH_ParamAccess.list);
            //pManager.AddPlaneParameter("perpPlanes", "perpPlanes", "perpPlanes", GH_ParamAccess.list);
            //pManager.AddNumberParameter("rotAngle", "rotAngle", "rotAngle", GH_ParamAccess.list);

        }
        /// <summary>
        /// 
        /// </summary>
        private bool _useDegrees;
        protected override void BeforeSolveInstance()
        {
            _useDegrees = (Params.Input[5] as Param_Number).UseDegrees;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region AssignInput
            Curve inCrv = null;
            if (!DA.GetData(0, ref inCrv))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
                return;
            }
            inCrv.Domain = new Interval(0, 1);

            List<double> inData = new List<double>();
            if (!DA.GetDataList(1, inData))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
                return;
            }

            List<double> inDataOptional = new List<double>();
            DA.GetDataList(2, inDataOptional);
            if (inDataOptional.Count == 0)
            {
                List<double> tParams = new List<double>();
                double counter = 0.0;
                double step = 1.0 / inData.Count;
                foreach (var data in inData)
                {
                    tParams.Add(counter);
                    counter += step;
                }
                inDataOptional.AddRange(tParams);
            }
            ValuesAllocator.MatchLists(inData, inDataOptional);

            Color inFirstColor = new Color();
            if(!DA.GetData(3, ref inFirstColor)) return;

            Color inSecondColor = new Color();
            if(!DA.GetData(4, ref inSecondColor)) return;

            List<double> inRotationAngles = new List<double>();
            DA.GetDataList(5, inRotationAngles);
            ValuesAllocator.MatchLists(inData, inRotationAngles);

            #endregion

            int lastValueIndex = inData.Count - 1;
            List<double> sortedData = new List<double>(inData);
            sortedData.Sort();

            Interval dataDomain = new Interval(sortedData[0], sortedData[lastValueIndex]);


            List<Line> histogramLines = new List<Line>();
            //List<double> rotAngles = new List<double>();
            //List<Plane> perpFrames = new List<Plane>();

            for (int i = 0; i < inData.Count; i++)
            {
                Plane pFrame;
                double tValue = inDataOptional[i];
                inCrv.PerpendicularFrameAt(tValue, out pFrame);

                double angle_rad = inRotationAngles[i];
                if (_useDegrees) angle_rad = RhinoMath.ToRadians(angle_rad);
                pFrame.Rotate(angle_rad, pFrame.ZAxis);

                //perpFrames.Add(pFrame);
                //rotAngles.Add(angle_rad);

                Line line = new Line(pFrame.Origin, pFrame.YAxis, inData[i]);
                histogramLines.Add(line);

                
            }


            List<Color> histogramColors = new List<Color>();
            histogramColors.Add(inFirstColor);
            histogramColors.Add(inSecondColor);

            #region AssignOutput
            DA.SetDataList(0, histogramLines);
            DA.SetDataList(1, histogramColors);
            //DA.SetDataList(2, perpFrames);
            //DA.SetDataList(3, rotAngles);
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
            get { return new Guid("8682019b-a55f-47cd-ab45-6b264e635b1f"); }
        }
    }
}