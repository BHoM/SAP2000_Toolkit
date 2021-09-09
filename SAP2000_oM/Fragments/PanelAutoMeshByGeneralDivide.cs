using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel based on points and lines in the meshing group and a maximum size.")]
    public class PanelAutoMeshByGeneralDivide : IPanelAutoMesh, IFragment
    {
        [Length]
        [Description("This is the maximum size of objects created by " +
"the General Divide Tool.")]
        public virtual double MaxSizeGeneral { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}