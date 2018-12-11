using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace Bulldog3.Geometries
{
    public class GhcDuplicateBorder : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcDuplicateBorder class.
        /// </summary>
        public GhcDuplicateBorder()
          : base("DupBorderPA", "DupBorderPA",
                "DupBorder joining each face border (As per dupBorder command in Rhino)",
                "Bulldog3", "Geometries")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Border", "jB", "joined border", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> inBreps = new List<Brep>();
            bool areInputsOk = DA.GetDataList<Brep>(0, inBreps);
            if (!areInputsOk)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
            }
            else
            {
                GH_Structure<GH_Curve> joinedCurves = BrepBorderExtractor.GetJoined(inBreps);
                DA.SetDataTree(0, joinedCurves);
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
            get { return new Guid("939b8765-5970-44da-8534-a73bca4a0437"); }
        }
    }
}