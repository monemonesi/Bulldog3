using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// This class hosts a collection of analysis methods for 3D curves
    /// </summary>
    public static class CurveProcessor
    {
        /// <summary>
        /// Close an open curve by adding a line
        /// </summary>
        /// <param name="inTollerances"></param>
        /// <param name="endPoints"></param>
        /// <param name="closedCurves"></param>
        /// <param name="closingResults"></param>
        /// <param name="i"></param>
        /// <param name="crv"></param>
        public static void CloseCrvAddingLine(List<double> inTollerances, List<Point3d> endPoints, List<Curve> closedCurves, List<bool> closingResults, int i, Curve crv)
        {
            bool succes = false;
            Curve finalClosedCrv = null;

            List<Point3d> endPts = CurveProcessor.GetEndPtsFromOpenCurve(crv);
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


        /// <summary>
        /// Join an open curve with the line connecting its end points
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="localEndPts"></param>
        /// <returns></returns>
        private static List<Curve> CreateJoinedCurves(Curve crv, List<Point3d> localEndPts)
        {
            Curve addedCurve = Curve.CreateInterpolatedCurve(localEndPts, 1);
            List<Curve> curvesToJoin = new List<Curve> { crv, addedCurve };
            List<Curve> joinedCurves = new List<Curve>(Curve.JoinCurves(curvesToJoin));
            return joinedCurves;
        }

        /// <summary>
        /// Extract end points from curve
        /// </summary>
        /// <param name="inCurve"></param>
        /// <returns></returns>
        public static List<Point3d> GetEndPtsFromOpenCurve(Curve inCurve)
        {
            List<Point3d> endPts = new List<Point3d>
                            {
                                inCurve.PointAtStart,
                                inCurve.PointAtEnd
                            };
            return endPts;
        }

        /// <summary>
        /// Cull the curves shorter then a given threshold
        /// </summary>
        /// <param name="inCurves"></param>
        /// <param name="inThres"></param>
        /// <returns></returns>
        public static List<Curve> CullShortCrv(List<Curve> inCurves, List<double> inThres)
        {
            List<Curve> culledCrvs = new List<Curve>();

            for (int i = 0; i < inCurves.Count; i++)
            {
                if (inCurves[i].GetLength() > inThres[i])
                {
                    culledCrvs.Add(inCurves[i]);
                }
            }

            return culledCrvs;
        }
    }
}
