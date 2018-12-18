using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// Generate joined border of breps` faces
    /// </summary>
    public static class BrepBorderExtractor
    {
        public static List<Curve> GetJoined(Brep brep)
        {
            List<Curve> joinedBorders = new List<Curve>();
            foreach (BrepFace face in brep.Faces)
            {
                joinedBorders.AddRange(Curve.JoinCurves(face.DuplicateFace(true).DuplicateEdgeCurves()));
            }
            return joinedBorders;
        }

        public static GH_Structure<GH_Curve> GetJoined(IList<Brep> inBreps)
        {
            GH_Structure<GH_Curve> joinedCurves = new GH_Structure<GH_Curve>();
            for (int i = 0; i < inBreps.Count; i++)
            {
                GH_Path path = new GH_Path(i);
                Brep brep = inBreps[i];
                List<Curve> curvesToAdd = new List<Curve>();
                curvesToAdd.AddRange(BrepBorderExtractor.GetJoined(brep));

                foreach (Curve curve in curvesToAdd)
                {
                    GH_Curve ghCurve = null;
                    if (GH_Convert.ToGHCurve(curve, GH_Conversion.Both, ref ghCurve))
                    {
                        joinedCurves.Append(ghCurve, path);
                    }
                }
            }

            return joinedCurves;
        }
    }

    
}
