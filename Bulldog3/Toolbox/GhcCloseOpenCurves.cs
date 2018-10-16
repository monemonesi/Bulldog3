using System;
using System.Collections.Generic;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Bulldog3.Toolbox
{
    public class GhcCloseOpenCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcCloseOpen class.
        /// </summary>
        public GhcCloseOpenCurves()
          : base("CloseOpenCurves", "CloseOpen",
                "Close open Curves and highlight end points when the closing command fail.",
                "Bulldog3", "Toolbox")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curve to test & Close", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Close Type", "cT", "Close type: [0] for adding line, [1] for move the end points", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Closing Tollerance", "tol", "set the tollerance", GH_ParamAccess.list, 0.01);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("EndPointsOpen", "endPts", "End points for open curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("ClosedCurves", "closedC", "Closed Curves", GH_ParamAccess.list);
            pManager.AddBooleanParameter("ClosingResult", "results", "Has been closed?", GH_ParamAccess.list);
        
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inCurves = new List<Curve>();
            if (!DA.GetDataList(0, inCurves))
                return;

            List<int> inClosingTypes = new List<int>();
            if (!DA.GetDataList(1, inClosingTypes))
                return;
            ValuesAllocator.MatchLists(inCurves, inClosingTypes);

            List<double> inTollerances = new List<double>();
            if (!DA.GetDataList(2, inTollerances))
                return;
            ValuesAllocator.MatchLists(inCurves, inTollerances);

            List<Point3d> endPoints = new List<Point3d>();
            List<Curve> closedCurves = new List<Curve>();
            List<bool> closingResults = new List<bool>();

            for (int i = 0; i < inCurves.Count; i++)
            {
                Curve crv = inCurves[i];
                if (crv.IsClosed)
                {
                    closedCurves.Add(crv);
                    closingResults.Add(true);
                }
                else
                {
                    if(inClosingTypes[i] <= 0)
                    {
                        CloseCrvAddingLine(inTollerances, endPoints, closedCurves, closingResults, i, crv);
                    }
                    else if (inClosingTypes[i]>=1)
                    {
                        bool success = crv.MakeClosed(inTollerances[i]);
                        if (!success)
                        {
                            List<Point3d> endPts = GetEndPtsFromOpenCurve(crv);
                            endPoints.AddRange(endPts);
                            closedCurves.Add(null);
                        }
                        else
                        {
                            closedCurves.Add(crv);
                        }
                        
                        closingResults.Add(success);
                        
                    }
                    //TODO: Add blend option for the future
                }
            }

            DA.SetDataList(0, endPoints);
            DA.SetDataList(1, closedCurves);
            DA.SetDataList(2, closingResults);
        }

        private static void CloseCrvAddingLine( List<double> inTollerances, List<Point3d> endPoints, List<Curve> closedCurves, List<bool> closingResults, int i, Curve crv)
        {
            bool succes = false;
            Curve finalClosedCrv = null;

            List<Point3d> endPts = GetEndPtsFromOpenCurve(crv);
            if (endPts[0].DistanceTo(endPts[1]) <= inTollerances[i])
            {
                List<Curve> joinedCurves = CreateJoinedCurves(crv, endPts);
                if (joinedCurves[0].IsClosed && joinedCurves.Count == 1)
                {
                    succes = true;
                    finalClosedCrv = joinedCurves[0];
                }
            }

            closedCurves.Add(finalClosedCrv);
            closingResults.Add(succes);
            if (!succes)
            {
                endPoints.AddRange(endPts);
            }
        }

        private static List<Curve> CreateJoinedCurves(Curve crv, List<Point3d> localEndPts)
        {
            Curve addedCurve = Curve.CreateInterpolatedCurve(localEndPts, 1);
            List<Curve> curvesToJoin = new List<Curve> { crv, addedCurve };
            List<Curve> joinedCurves = new List<Curve>(Curve.JoinCurves(curvesToJoin));
            return joinedCurves;
        }


        private static List<Point3d> GetEndPtsFromOpenCurve(Curve inCurve)
        {
            List<Point3d> nonClosedCrvPts = new List<Point3d>
                            {
                                inCurve.PointAtStart,
                                inCurve.PointAtEnd
                            };
            return nonClosedCrvPts;
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
            get { return new Guid("2597cec0-0b28-464a-98bb-23505bc4be5f"); }
        }
    }
}