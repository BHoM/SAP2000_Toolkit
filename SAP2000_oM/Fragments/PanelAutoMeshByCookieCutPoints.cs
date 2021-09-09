using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel based on points in the meshing group.")]
    public class PanelAutoMeshByCookieCutPoints : IPanelAutoMesh, IFragment
    {
        [Angle]
        [Description("This is an angle in radians that the meshing lines are rotated from their default orientation." +
            "By default these lines align with the area object local 1 and 2 axes.")]
        public virtual double Rotation { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}