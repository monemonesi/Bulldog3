using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// This class hosts a collection of optimisation method for 3D curves
    /// </summary>
    public static class CurvesOptimizer
    {
        public static void DeCasteljauCurvePts(Curve crv, double tStart, double tEnd, double tolerance, SortedList<double, Point3d> separationPts)
        {
            Point3d startPt = crv.PointAt(tStart);
            Point3d endPt = crv.PointAt(tEnd);
            Point3d middlePt = 0.5 * (startPt + endPt);

            double tParam;
            crv.ClosestPoint(middlePt, out tParam);

            double distanceToCompare = crv.PointAt(tParam).DistanceTo(middlePt);

            if (distanceToCompare <= tolerance)
            {
                if (!separationPts.ContainsKey(tStart))
                {
                    separationPts.Add(tStart, startPt);
                }
                if (!separationPts.ContainsKey(tEnd))
                {
                    separationPts.Add(tEnd, endPt);
                }
            }
            else
            {
                double tMid = 0.5 * (tStart + tEnd);
                DeCasteljauCurvePts(crv, tStart, tMid, tolerance, separationPts);
                DeCasteljauCurvePts(crv, tMid, tEnd, tolerance, separationPts);
            }
        }
    }
}
