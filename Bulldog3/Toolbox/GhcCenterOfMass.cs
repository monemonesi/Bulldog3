using System;
using System.Collections.Generic;
using Bulldog3.HelperClasses;
using Bulldog3.Constants;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace Bulldog3.Toolbox
{
    /// <summary>
    /// Define points` center of mass
    /// </summary>
    public class GhcCenterOfMass : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcCenterOfMass class.
        /// </summary>
        public GhcCenterOfMass()
          : base("CenterOfMass", "CenterMass",
                "Find the Center of mass between points",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Masses", "m", "Mass", GH_ParamAccess.list, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("CenterOfMass", "c", "CenterOfMass", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "d", "distances from the center", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> inPoints = new List<Point3d>();
            if (!DA.GetDataList<Point3d>(0, inPoints))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
            }
            else
            {
                List<double> inMasses = new List<double>();
                DA.GetDataList<double>(1, inMasses);
                ValuesAllocator.MatchLists(inPoints, inMasses);

                Point3d point3D = new Point3d(0, 0, 0);
                List<double> distances = new List<double>();
                double totalMass=0;
                for (int i = 0; i < inPoints.Count; i++)
                {
                    point3D = point3D + (inPoints[i] * inMasses[i]);
                    totalMass += inMasses[i];
                }
                Point3d center = point3D / totalMass;
                GH_Point ghCenter = new GH_Point(center);
                foreach (Point3d pt in inPoints)
                {
                    distances.Add(ghCenter.Value.DistanceTo(pt));
                }
                DA.SetData(0, ghCenter);
                DA.SetDataList(1, distances);
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
            get { return new Guid("74dee131-808e-4c6b-8b78-9be939b1cbc8"); }
        }
    }
}