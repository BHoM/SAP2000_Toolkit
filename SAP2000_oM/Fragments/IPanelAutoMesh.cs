/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{
    [Description("Base interface for panel auto mesh settings in SAP2000. Contains the type of auto mesh and the properties common to all types.")]
    [Unique]
    public interface IPanelAutoMesh : IFragment
    {
        [Description("If this item is True, and if both points along an edge of the original area " + "object have the same local axes, the program makes the local axes for added points " + "along the edge the same as the edge end points.")]
        bool LocalAxesOnEdge { get; set; }

        [Description("If this item is True, and if all points around the perimeter of the original " + "area object have the same local axes, the program makes the local axes for all added " + "points the same as the perimeter points.")]
        bool LocalAxesOnFace { get; set; }

        [Description("If this item is True, and if both points along an edge of the original area " + "object have the same restraint/constraint, then, if the added point and the adjacent " + "corner points have the same local axes definition, the program includes the " + "restraint/constraint for added points along the edge.")]
        bool RestraintsOnEdge { get; set; }

        [Description("If this item is True, and if all points around the perimeter of the original " + "area object have the same restraint/constraint, then, if an added point and the " + "perimeter points have the same local axes definition, the program includes the " + "restraint/constraint for the added point.")]
        bool RestraintsOnFace { get; set; }

        [Description("The name of a defined group. Some of the meshing options make use of point " + "and line objects included in this group.")]
        string Group { get; set; }

        [Description("If this item is True, after initial meshing, the program further meshes any area " + "objects that have an edge longer than the length specified by the SubMeshSize item.")]
        bool SubMesh { get; set; }

        [Description("This item applies when the SubMesh item is True. It is the maximum size of area " + "objects to remain when the auto meshing is complete.")]
        double SubMeshSize { get; set; }
    }
}

