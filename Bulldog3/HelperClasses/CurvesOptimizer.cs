using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
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
        /// <summary>
        /// Curve semplification via De Casteljau`s algorithm
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="tStart"></param>
        /// <param name="tEnd"></param>
        /// <param name="tolerance"></param>
        /// <param name="separationPts"></param>
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


        /// <summary>
        /// Rebuild a curve proportionally to its control points
        /// </summary>
        /// <param name="i"></param>
        /// <param name="startingCurves"></param>
        /// <param name="startingDegrees"></param>
        /// <param name="reductionFactors"></param>
        /// <param name="preserveTangents"></param>
        /// <param name="rebuildedCurves"></param>
        public static void RebuildProportionally
            (int i, Curve[] startingCurves, int[] startingDegrees, double[] reductionFactors, bool[] preserveTangents, ConcurrentDictionary<int, Curve> rebuildedCurves)
        {
            NurbsCurve startingNCurve = startingCurves[i].ToNurbsCurve();
            int startingControlPointCount = startingNCurve.Points.Count;
            int newControlPointCount = Convert.ToInt32((double)startingControlPointCount * reductionFactors[i]);
            if (startingDegrees[i] < 0)
            {
                startingDegrees[i] = startingNCurve.Degree;
            }
            NurbsCurve rebuildedCurve = startingNCurve.Rebuild(newControlPointCount, startingDegrees[i], preserveTangents[i]);
            rebuildedCurves[i] = rebuildedCurve;
        }
    }


}
