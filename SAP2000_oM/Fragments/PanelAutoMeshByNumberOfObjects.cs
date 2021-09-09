using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel into a given number of elements in each direction.")]
    public class PanelAutoMeshByNumberOfObjects : IPanelAutoMesh, IFragment
    {
        [Description("This is the number of objects " +
    "created along the edge of the meshed area object that runs from point 1 to point 2.")]
        public virtual int N1 { get; set; }

        [Description("This is the number of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 3.")]
        public virtual int N2 { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}