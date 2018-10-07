using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Bulldog3.Models
{
    class BrepBorderExtractor
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
    }
}
