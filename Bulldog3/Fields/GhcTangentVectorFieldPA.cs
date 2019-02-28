using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bulldog3.HelperClasses;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Fields
{
    /// <summary>
    /// Create a 3D vector field from a series of points and curves
    /// </summary>
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
            int ptNumber = inPts.Count;
            Point3d[] pts = inPts.ToArray();

            List<Curve> inCurves = new List<Curve>();
            bool canGetCrvs = DA.GetDataList(1, inCurves);
            inputChecker.StopIfConversionIsFailed(canGetCrvs);
            Curve[] crvs = inCurves.ToArray();
            #endregion

            Vector3d[] outputVectors = new Vector3d[ptNumber];
            ConcurrentDictionary<Point3d, Vector3d> vecField = new ConcurrentDictionary<Point3d, Vector3d>();
            double[] outputScalar = new double[ptNumber];
            ConcurrentDictionary<Point3d, double> scalarField = new ConcurrentDictionary<Point3d, double>();

            this.Message = Constants.Constants.PARALLEL_MESSAGE;
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, Constants.Constants.PARALLEL_WARNING);

            int processorCount = Environment.ProcessorCount - 1;

            Parallel.ForEach(inPts, new ParallelOptions { MaxDegreeOfParallelism = processorCount },
                pt =>
                {
                    GetClosestCrv(pt, crvs, ref vecField, ref scalarField);
                });

            for (int i = 0; i < ptNumber; i++)
            {
                outputVectors[i] = vecField[inPts[i]];
                outputScalar[i] = scalarField[inPts[i]];
            }

            DA.SetDataList(0, outputVectors);
            DA.SetDataList(1, outputScalar);

        }

        /// <summary>
        /// Find the closest crv to the point
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="crvs"></param>
        /// <param name="vecField"></param>
        /// <param name="scalarField"></param>
        private void GetClosestCrv(Point3d pt, Curve[] crvs, ref ConcurrentDictionary<Point3d, Vector3d> vecField, ref ConcurrentDictionary<Point3d, double> scalarField)
        {
            double closestDist = double.MaxValue;
            int closestCrvId = 0;
            double closestParamT = 0.0;

            //Find the closest crv.
            for (int i = 0; i < crvs.Length; i++)
            {
                if (crvs[i].ClosestPoint(pt, out double t, closestDist))
                {
                    pt = UpdateClosestPtValues(pt, crvs, out closestDist, out closestCrvId, out closestParamT, i, t);

                }
            }

            Vector3d closestTan = new Vector3d();
            closestTan = crvs[closestCrvId].TangentAt(closestParamT);

            vecField[pt] = closestTan;
            scalarField[pt] = closestDist;
        }

        /// <summary>
        /// Update the values relative to the closest point.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="crvs"></param>
        /// <param name="closestDist"></param>
        /// <param name="closestCrvId"></param>
        /// <param name="closestParamT"></param>
        /// <param name="i"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Point3d UpdateClosestPtValues(Point3d pt, Curve[] crvs, 
            out double closestDist, out int closestCrvId, out double closestParamT, 
            int i, double t)
        {
            double distance = pt.DistanceTo(crvs[i].PointAt(t));
            closestDist = distance;
            closestCrvId = i;
            closestParamT = t;
            return pt;
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