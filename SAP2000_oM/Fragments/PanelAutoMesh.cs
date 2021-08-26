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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base;

namespace BH.oM.Adapters.SAP2000.Elements
{
    public class PanelAutoMesh : IFragment
    {
        [Description("This item is 0, 1, 2, 3, 4, 5 or 6, indicating the automatic mesh type for the area object.")]
        public virtual PanelAutoMeshType MeshType { get; set; } = PanelAutoMeshType.None;

        [Description("This item applies when MeshType = 1. It is the number of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 2.")]
        public virtual int N1 { get; set; } = 0;

        [Description("This item applies when MeshType = 1. It is the number of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 3.")]
        public virtual int N2 { get; set; } = 0;

        [Description("This item applies when MeshType = 2. It is the maximum size of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 2.")]
        public virtual double MaxSize1 { get; set; } = 0;

        [Description("This item applies when MeshType = 2. It is the maximum size of objects " +
            "created along the edge of the meshed area object that runs from point 1 to point 3.")]
        public virtual double MaxSize2 { get; set; } = 0;

        [Description("This item applies when MeshType = 3. If it is True, points on the area " +
            "object edges are determined from intersections of straight line objects included in " +
            "the group specified by the Group item with the area object edges.")]
        public virtual bool PointOnEdgeFromLine { get; set; } = false;

        [Description("This item applies when MeshType = 3. If it is True, points on the area " +
            "object edges are determined from point objects included in the group specified by" +
            " the Group item that lie on the area object edges.")]
        public virtual bool PointOnEdgeFromPoint { get; set; } = false;

        [Description("This item applies when MeshType = 4. MeshType = 4 provides cookie cut " +
            "meshing based on straight line objects included in the group specified by the Group " +
            "item that intersect the area object edges. If the ExtendCookieCutLines item is True, " +
            "all straight line objects included in the group specified by the Group item " +
            "are extended to intersect the area object edges for the purpose of meshing the area object.")]
        public virtual bool ExtendCookieCutLines { get; set; } = false;

        [Description("This item applies when MeshType = 5. MeshType = 5 provides cookie cut meshing based " +
            "on two perpendicular lines passing through point objects included in the group specified by " +
            "the Group item. By default these lines align with the area object local 1 and 2 axes. The " +
            "Rotation item is an angle in degrees that the meshing lines are rotated from their default orientation. [deg]")] //CONVERT TO RADIANS
        public virtual double Rotation { get; set; } = 0;

        [Description("This item applies when MeshType = 6. It is the maximum size of objects created by " +
            "the General Divide Tool.")]
        public virtual double MaxSizeGeneral { get; set; } = 0;

        [Description("If this item is True, and if both points along an edge of the original area " +
            "object have the same local axes, the program makes the local axes for added points " +
            "along the edge the same as the edge end points.")]
        public virtual bool LocalAxesOnEdge { get; set; } = false;

        [Description("If this item is True, and if all points around the perimeter of the original " +
            "area object have the same local axes, the program makes the local axes for all added " +
            "points the same as the perimeter points.")]
        public virtual bool LocalAxesOnFace { get; set; } = false;

        [Description("If this item is True, and if both points along an edge of the original area " +
            "object have the same restraint/constraint, then, if the added point and the adjacent " +
            "corner points have the same local axes definition, the program includes the " +
            "restraint/constraint for added points along the edge.")]
        public virtual bool RestraintsOnEdge { get; set; } = false;

        [Description("If this item is True, and if all points around the perimeter of the original " +
            "area object have the same restraint/constraint, then, if an added point and the " +
            "perimeter points have the same local axes definition, the program includes the " +
            "restraint/constraint for the added point.")]
        public virtual bool RestraintsOnFace { get; set; } = false;

        [Description("The name of a defined group. Some of the meshing options make use of point " +
            "and line objects included in this group.")]
        public virtual string Group { get; set; } = null;

        [Description("If this item is True, after initial meshing, the program further meshes any area " +
            "objects that have an edge longer than the length specified by the SubMeshSize item.")]
        public virtual bool SubMesh { get; set; } = false;

        [Description("This item applies when the SubMesh item is True. It is the maximum size of area " +
            "objects to remain when the auto meshing is complete.")]
        public virtual double SubMeshSize { get; set; } = 0;
    }
}
