using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel based on lines in the meshing group.")]
    public class PanelAutoMeshByCookieCutLines : IPanelAutoMesh, IFragment
    {
        [Description("If this item is True, " +
            "all straight line objects included in the group specified by the Group item " +
            "are extended to intersect the area object edges for the purpose of meshing the area object.")]
        public virtual bool ExtendCookieCutLines { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}