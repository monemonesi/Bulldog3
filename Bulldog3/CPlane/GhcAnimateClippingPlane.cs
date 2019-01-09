using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace Bulldog3.CPlane
{
    /// <summary>
    /// Move an existing clipping plane: It can be use for quikly generate dynamic sections
    /// </summary>
    public class GhcAnimateClippingPlane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcAnimateClippingPlane class.
        /// </summary>
        public GhcAnimateClippingPlane()
          : base("AnimateClippingPlane", "AnimateClippingPlane",
              "move an existing clipping plane to a new position",
              "Bulldog3", "CPlane")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Clipping Plane guid", "id", "Clipping Plane guid", GH_ParamAccess.item);
            pManager.AddPlaneParameter("New Plane", "pln", "New plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("New clipping plane", "cPln", "New clipping plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Guid id = Guid.Empty;
            bool successId = DA.GetData(0, ref id);
            Plane updatedPlane = new Plane();
            bool successPlane = DA.GetData(1, ref updatedPlane);
            bool succesfullConversion = successId && successPlane;
            if (succesfullConversion)
            {
                //define active document
                RhinoDoc activeDoc = RhinoDoc.ActiveDoc;

                //find the clipping plane
                var rhinoObj = activeDoc.Objects.FindId(id);
                var clippingPlane = rhinoObj as Rhino.DocObjects.ClippingPlaneObject;

                //updated the plane for the clipping plane
                clippingPlane.ClippingPlaneGeometry.Plane = updatedPlane;

                clippingPlane.CommitChanges();
            }

            DA.SetData(0, updatedPlane);
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
            get { return new Guid("a907e56d-6ae1-4f0b-ae2b-970040a68cc6"); }
        }
    }
}