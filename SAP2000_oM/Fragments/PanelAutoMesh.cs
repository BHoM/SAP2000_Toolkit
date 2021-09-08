/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{
    [Description("Base interface for panel auto mesh settings in SAP2000. Contains the type of auto mesh and the properties common to all types.")]
    public interface IPanelAutoMesh : IFragment
    {
        [Description("If this item is True, and if both points along an edge of the original area " +
            "object have the same local axes, the program makes the local axes for added points " +
            "along the edge the same as the edge end points.")]
        bool LocalAxesOnEdge { get; set; }

        [Description("If this item is True, and if all points around the perimeter of the original " +
            "area object have the same local axes, the program makes the local axes for all added " +
            "points the same as the perimeter points.")]
        bool LocalAxesOnFace { get; set; }

        [Description("If this item is True, and if both points along an edge of the original area " +
            "object have the same restraint/constraint, then, if the added point and the adjacent " +
            "corner points have the same local axes definition, the program includes the " +
            "restraint/constraint for added points along the edge.")]
        bool RestraintsOnEdge { get; set; }

        [Description("If this item is True, and if all points around the perimeter of the original " +
            "area object have the same restraint/constraint, then, if an added point and the " +
            "perimeter points have the same local axes definition, the program includes the " +
            "restraint/constraint for the added point.")]
        bool RestraintsOnFace { get; set; }

        [Description("The name of a defined group. Some of the meshing options make use of point " +
            "and line objects included in this group.")]
        string Group { get; set; }

        [Description("If this item is True, after initial meshing, the program further meshes any area " +
            "objects that have an edge longer than the length specified by the SubMeshSize item.")]
        bool SubMesh { get; set; }

        [Description("This item applies when the SubMesh item is True. It is the maximum size of area " +
            "objects to remain when the auto meshing is complete.")]
        double SubMeshSize { get; set; }
    }

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
