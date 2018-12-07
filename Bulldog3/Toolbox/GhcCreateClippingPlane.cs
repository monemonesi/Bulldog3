using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcCreateClippingPlane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcCreateClippingPlane class.
        /// </summary>
        public GhcCreateClippingPlane()
          : base("CreateClippingPlane", "CreateClippingPlane",
              "Transform a simple GH plane in clipping Plane:",
              "Bulldog3", "CpPlane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Create clipping plane", "run", "Create the clipping plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Original Plane", "pln", "Starting plane location", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ClippingPlane GUID", "guid", "Contain the Guid of the new generated clipping plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool createClippingPlane = false;
            if (!DA.GetData(0, ref createClippingPlane)) return;

            Plane referencePlane = new Plane();
            bool success = DA.GetData(1, ref referencePlane);

            Guid id = Guid.Empty;

            if (success && createClippingPlane)
            {
                //define active document
                RhinoDoc activeDoc = RhinoDoc.ActiveDoc;

                //Get the active view
                RhinoView currentView = activeDoc.Views.ActiveView;

                //Viewport where use the clipping plane
                Guid currentViewId = currentView.ActiveViewportID;


                //add clipping Plane
                id = activeDoc.Objects.AddClippingPlane(referencePlane, 1, 1, currentViewId);
                
            }

            DA.SetData(0, id);
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
            get { return new Guid("770d6b7c-58ef-4229-8ee9-5c1cc51ea54e"); }
        }
    }
}