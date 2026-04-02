/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{
    [Description("Divide the panel based on points coincident with the panel edges.")]
    public class PanelAutoMeshByPointsOnEdges : IPanelAutoMesh, IFragment
    {
        [Description("If this is True, points on the area " + "object edges are determined from intersections of straight line objects included in " + "the group specified by the Group item with the area object edges.")]
        public virtual bool PointOnEdgeFromLine { get; set; }

        [Description("If this is True, points on the area " + "object edges are determined from point objects included in the group specified by" + " the Group item that lie on the area object edges.")]
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



