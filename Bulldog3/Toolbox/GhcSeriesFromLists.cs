using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcSeriesFromLists : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcSeriesFromLists class.
        /// </summary>
        public GhcSeriesFromLists()
          : base("SeriesFromLists", "SeriesFromList",
                "Create a series from a list of objects",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("referenceList", "List", "Objects to use for create series", GH_ParamAccess.list);
            pManager.AddNumberParameter("StartingValue", "Start", "First number in series", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("StepSize", "StepSize", "Step dimension", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("series", "S", "Series from Lists", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> inReferenceList = new List<object>();
            bool success = DA.GetDataList(0, inReferenceList);
            if(success)
            {
                double startingValue = 0;
                double stepSize = 1;
                DA.GetData(1, ref startingValue);
                DA.GetData(2, ref stepSize);

                List<double> series = SeriesFromLists.SeriesFromObjectLists(inReferenceList, startingValue, stepSize);
                DA.SetDataList(0, series);
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
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
            get { return new Guid("a2a9b4b7-70d3-4bfd-80d4-87e73e029fd2"); }
        }
    }
}