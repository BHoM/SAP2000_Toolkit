using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    [Description("Divide the panel based on points coincident with the panel edges.")]
    public class PanelAutoMeshByPointsOnEdges : IPanelAutoMesh, IFragment
    {
        [Description("If this is True, points on the area " +
            "object edges are determined from intersections of straight line objects included in " +
            "the group specified by the Group item with the area object edges.")]
        public virtual bool PointOnEdgeFromLine { get; set; }

        [Description("If this is True, points on the area " +
            "object edges are determined from point objects included in the group specified by" +
            " the Group item that lie on the area object edges.")]
        public virtual bool PointOnEdgeFromPoint { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}