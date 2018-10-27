using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bulldog3.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Bulldog3.Geometries
{
    public class GhcCreateSolidPA : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcSolidifyPA class.
        /// </summary>
        public GhcCreateSolidPA()
          : base("CreateSolid", "CreateSolid", 
                "Create a solid from a Brep. NOTE: use parallel only with a lot of flattened elements",
                "Bulldog3", "Geometries")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("B", "B", "Starting Brep for solid", GH_ParamAccess.tree);
            pManager.AddNumberParameter("D", "D", "Distance", GH_ParamAccess.tree, 1);
            pManager.AddBooleanParameter("bS", "bS", "Both sides", GH_ParamAccess.tree, false);
            pManager.AddBooleanParameter("f", "f", "flip normal?", GH_ParamAccess.tree, false);
            pManager.AddBooleanParameter("||", "||", "Use Parallel", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("B", "B", "Solid from surface Or Surfaces", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InputChecker inputChecker = new InputChecker(this);

            GH_Structure<GH_Brep> inGhBreps = new GH_Structure<GH_Brep>();
            bool areBrepsOk = DA.GetDataTree(0, out inGhBreps);
            inputChecker.CheckAndShowConversionError(areBrepsOk);

            GH_Structure<GH_Number> inGhDistances = new GH_Structure<GH_Number>();
            bool areDistancesOk = DA.GetDataTree(1, out inGhDistances);
            inputChecker.CheckAndShowConversionError(areDistancesOk);

            bool brepTopologyEqualDistanceTopology = inGhDistances.TopologyDescription.Equals(inGhBreps.TopologyDescription);
            GH_Structure<GH_Number> ghDistances = new GH_Structure<GH_Number>();
            if (brepTopologyEqualDistanceTopology)
            {
                ghDistances = inGhDistances.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in inGhBreps.Paths)
                {
                    for (int i = 0; i < inGhBreps.get_Branch(ghPath).Count; i++)
                    {
                        ghDistances.Insert(inGhDistances.get_LastItem(true), ghPath, i);
                    }
                }
            }

            GH_Structure<GH_Boolean> inGhBothSides = new GH_Structure<GH_Boolean>();
            bool areBoolBothSidesOk = DA.GetDataTree(2, out inGhBothSides);
            inputChecker.CheckAndShowConversionError(areBoolBothSidesOk);

            bool brepTopologyEqualBothSidesTopology = inGhBothSides.TopologyDescription.Equals(inGhBreps.TopologyDescription);
            GH_Structure<GH_Boolean> ghBothSides = new GH_Structure<GH_Boolean>();
            if (brepTopologyEqualBothSidesTopology)
            {
                ghBothSides = inGhBothSides.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in inGhBreps.Paths)
                {
                    for (int i = 0; i < inGhBreps.get_Branch(ghPath).Count; i++)
                    {
                        ghBothSides.Insert(inGhBothSides.get_LastItem(true), ghPath, i);
                    }
                }
            }

            GH_Structure<GH_Boolean> inGhFlipNormals = new GH_Structure<GH_Boolean>();
            bool areBoolFlipNormalsOk = DA.GetDataTree(3, out inGhFlipNormals);
            inputChecker.CheckAndShowConversionError(areBoolFlipNormalsOk);

            bool brepTopologyEqualFlipNTopology = inGhFlipNormals.TopologyDescription.Equals(inGhBreps.TopologyDescription);
            GH_Structure<GH_Boolean> ghFlipNormals = new GH_Structure<GH_Boolean>();
            if (brepTopologyEqualFlipNTopology)
            {
                ghFlipNormals = inGhFlipNormals.Duplicate();
            }
            else
            {
                foreach (GH_Path ghPath in inGhBreps.Paths)
                {

                    for (int i = 0; i < inGhBreps.get_Branch(ghPath).Count; i++)
                    {
                        ghFlipNormals.Insert(inGhFlipNormals.get_LastItem(true), ghPath, i);
                    }

                }
            }

            bool useParallel = false;
            DA.GetData<bool>(4, ref useParallel);

            inGhBreps.Graft(GH_GraftMode.GraftAll);
            ghDistances.Graft(GH_GraftMode.GraftAll);
            ghBothSides.Graft(GH_GraftMode.GraftAll);
            ghFlipNormals.Graft(GH_GraftMode.GraftAll);

            double docTollerance = DocumentTolerance();
            GH_Structure<GH_Brep> ghSolidBreps = new GH_Structure<GH_Brep>();
            if (useParallel)
            {
                this.Message = Constants.Constants.PARALLEL_MESSAGE;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, Constants.Constants.PARALLEL_WARNING);

                int processorCount = Environment.ProcessorCount - 1;
                //WORK IN PARALLEL
                ConcurrentDictionary<GH_Path, Brep> solidBrepsPA = new ConcurrentDictionary<GH_Path, Brep>();
                Parallel.ForEach(inGhBreps.Paths, new ParallelOptions { MaxDegreeOfParallelism = processorCount },
                    path =>
                    {
                        Brep brepToSolidify = inGhBreps.get_DataItem(path, 0).Value;
                        bool flipTheNormal = ghFlipNormals.get_DataItem(path, 0).Value;
                        bool bothSide = ghBothSides.get_DataItem(path, 0).Value;
                        double distance = ghDistances.get_DataItem(path, 0).Value;
                        if (flipTheNormal)
                        {
                            distance *= -1;
                        }
                        foreach (var brepFace in brepToSolidify.Faces)
                        {
                            solidBrepsPA[path] = Brep.CreateFromOffsetFace(brepFace, distance, docTollerance, bothSide, true);
                        }
                    });
                foreach (KeyValuePair<GH_Path, Brep> keyValueBrep in solidBrepsPA)
                {
                    GH_Brep ghBrep = null;
                    if (GH_Convert.ToGHBrep(keyValueBrep.Value, GH_Conversion.Both, ref ghBrep))
                    {
                        ghSolidBreps.Append(ghBrep, keyValueBrep.Key);
                    }
                    else
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conversion Failed");
                        return;
                    }
                }
            }//End Parallel Computation
            else
            {
                this.Message = Constants.Constants.SERIAL_MESSAGE;

                foreach (GH_Path path in inGhBreps.Paths)
                {
                    Brep brepToSolidify = inGhBreps.get_DataItem(path, 0).Value;
                    bool flipTheNormal = ghFlipNormals.get_DataItem(path, 0).Value;
                    bool bothSide = ghBothSides.get_DataItem(path, 0).Value;
                    double distance = ghDistances.get_DataItem(path, 0).Value;
                    if (flipTheNormal)
                    {
                        distance *= -1;
                    }
                    foreach (var brepFace in brepToSolidify.Faces)
                    {
                        Brep solidBrep = Brep.CreateFromOffsetFace(brepFace, distance, docTollerance, bothSide, true);
                        GH_Brep ghBrep = null;
                        if (GH_Convert.ToGHBrep(solidBrep, GH_Conversion.Both, ref ghBrep))
                        {
                            ghSolidBreps.Append(ghBrep, path);
                        }
                        else
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Conversion Failed");
                            return;
                        }
                    }
                }
            }//End Serial Computation
            ghSolidBreps.Simplify(GH_SimplificationMode.CollapseAllOverlaps);
            DA.SetDataTree(0, ghSolidBreps);

        }//end SolveInstance

        

        //private void CheckGetDataConversion(bool getData)
        //{
        //    if (!getData) ShowInputsError();
        //}

        //private void ShowInputsError()
        //{
        //    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Constants.Constants.INPUT_ERROR_MESSAGE);
        //}

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
            get { return new Guid("4917c506-e9b8-476b-94fd-107462cae6de"); }
        }
    }
}