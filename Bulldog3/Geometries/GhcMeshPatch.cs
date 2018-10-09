using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.Geometries
{
    public class GhcMeshPatch : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcMeshPatch class.
        /// </summary>
        public GhcMeshPatch()
          : base("MeshPatch", "MPatch",
                "Create a fast mesh Patch between polylines",
                "Bulldog3", "Geometries")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("polylines", "pl", "plolylines to patch", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "m", "resulting mesh", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = Constants.Constants.PARALLEL_MESSAGE;
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, Constants.Constants.PARALLEL_WARNING);
            int processorCount = Environment.ProcessorCount - 1;
            double docTollerance = DocumentTolerance();

            GH_Structure<GH_Mesh> ghPatches = new GH_Structure<GH_Mesh>();

            GH_Structure<GH_Curve> inGhCurves = new GH_Structure<GH_Curve>();
            bool areCurvesOK = DA.GetDataTree(0, out inGhCurves);
            CheckGetDataConversion(areCurvesOK);

            ConcurrentDictionary<GH_Path, Mesh> patchesPA = new ConcurrentDictionary<GH_Path, Mesh>();
            Parallel.ForEach( inGhCurves.Paths, new ParallelOptions { MaxDegreeOfParallelism = processorCount },
                path => {
                    Polyline firstPolyline = null;
                    inGhCurves.get_DataItem(path, 0).Value.TryGetPolyline(out firstPolyline);
                    if ((!firstPolyline.IsValid ? true : firstPolyline == null))
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Impossible convert the first Curve in Polyline");
                    }
                    List<Curve> otherBranchCurves = new List<Curve>();
                    for (int i = 0; i < inGhCurves.get_Branch(path).Count; i++)
                    {
                        otherBranchCurves.Add(inGhCurves.get_DataItem(path, i).Value.DuplicateCurve());
                    }
                    patchesPA[path] = Mesh.CreatePatch(firstPolyline, docTollerance, null, otherBranchCurves, null, null, true, 1);
                });

            foreach (KeyValuePair<GH_Path,Mesh> patch in patchesPA)
            {
                GH_Mesh ghPatchMesh = null;
                if (GH_Convert.ToGHMesh(patch.Value, GH_Conversion.Both, ref ghPatchMesh))
                {
                    ghPatches.Append(ghPatchMesh, patch.Key);
                }
                else
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conversion Failed");
                    return;
                }
            }


            DA.SetDataTree(0, ghPatches);
        }

        private void CheckGetDataConversion(bool getData)
        {
            if (!getData) ShowInputsError();
        }

        private void ShowInputsError()
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
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
            get { return new Guid("f1116e92-91a7-4dfb-9259-6d0b3615dcd5"); }
        }
    }
}