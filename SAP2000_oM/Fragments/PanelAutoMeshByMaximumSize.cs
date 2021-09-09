using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel so that elements do not exceed a maximum size.")]
    public class PanelAutoMeshByMaximumSize : IPanelAutoMesh, IFragment
    {
        [Length]
        [Description("This is the maximum size of objects " +
    "created along the edge of the meshed area object that runs from point 1 to point 2.")]
        public virtual double MaxSize1 { get; set; }

        [Length]
        [Description("This is the maximum size of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 3.")]
        public virtual double MaxSize2 { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}