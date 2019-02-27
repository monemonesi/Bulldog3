using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Fields
{
    public class GhcTangentVectorFieldPA : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcVectorField class.
        /// </summary>
        public GhcTangentVectorFieldPA()
          : base("TangentVectorField", "TanField",
              "Given a series of points and curves defines the tangent vector field working in parallel",
              "Bulldog3", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Base Vectorfield pts", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves", "Crv", "Vectorfield curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Vectors", "Vec", "Resulting vectors", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "d", "Distances from the curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            #region GetInputFromCanvas

            List<Point3d> inPts = new List<Point3d>();
            bool canGetPts = DA.GetDataList(0, inPts);
            inputChecker.StopIfConversionIsFailed(canGetPts);

            List<Curve> inCurves = new List<Curve>();
            bool canGetCrvs = DA.GetDataList(1, inCurves);
            inputChecker.StopIfConversionIsFailed(canGetCrvs);
            #endregion

            List<Vector3d> outputVectors = new List<Vector3d>();
            ConcurrentDictionary<Point3d, Vector3d> vecField = new ConcurrentDictionary<Point3d, Vector3d>();
            List<double> outputScalar = new List<double>();
            ConcurrentDictionary<Point3d, double> scalarField = new ConcurrentDictionary<Point3d, double>();

            this.Message = Constants.Constants.PARALLEL_MESSAGE;
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, Constants.Constants.PARALLEL_WARNING);

            int processorCount = Environment.ProcessorCount - 1;

            Parallel.ForEach(inPts, new ParallelOptions { MaxDegreeOfParallelism = processorCount },
                pt => {
                    GetClosestTan(pt, inCurves, ref vecField, ref scalarField);
                });

            foreach (KeyValuePair<Point3d,Vector3d> point in vecField)
            {
                outputVectors.Add(point.Value);
                outputScalar.Add(scalarField[point.Key]);
            }

            DA.SetDataList(0, outputVectors);
            DA.SetDataList(1, outputScalar);

        }

        private void GetClosestTan(Point3d pt, List<Curve> inCurves, ref ConcurrentDictionary<Point3d, Vector3d> vecField, ref ConcurrentDictionary<Point3d, double> scalarField)
        {
            double closestDistSquared = double.MaxValue;
            int closestCrvId = 0;
            double closestParamT = 0.0;

            //Find the closest crv.
            for (int i = 0; i < inCurves.Count; i++)
            {
                double t;
                inCurves[i].ClosestPoint(pt, out t, closestDistSquared);

                double distanceSquared = pt.DistanceToSquared(inCurves[i].PointAt(t));

                if (distanceSquared < closestDistSquared)
                {
                    closestDistSquared = distanceSquared;
                    closestCrvId = i;
                    closestParamT = t;
                }
            }

            Vector3d closestTan = new Vector3d();
            closestTan = inCurves[closestCrvId].TangentAt(closestParamT);

            vecField[pt] = closestTan;
            scalarField[pt] = Math.Sqrt(closestDistSquared);
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
            get { return new Guid("4a150c21-3e91-49ed-88fc-0b4ce3d988b3"); }
        }
    }
}