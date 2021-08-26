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
using BH.oM.Structure.Elements;
using BH.oM.Adapters.SAP2000.Elements;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Base;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Modify
    {
        public static Panel SetPanelAutoMesh(this Panel panel, PanelAutoMeshType meshType, int n1, int n2, double maxSize1, double maxSize2, bool
                        pointOnEdgeFromLine, bool pointOnEdgeFromPoint, bool extendCookieCutLines, double rotation,
                        double maxSizeGeneral, bool localAxesOnEdge, bool localAxesOnFace, bool restraintsOnEdge,
                        bool restraintsOnFace, string group, bool subMesh, double subMeshSize)
        {

            PanelAutoMesh fragment = new PanelAutoMesh()
            {
                MeshType = meshType,
                N1 = n1,
                N2 = n2,
                MaxSize1 = maxSize1,
                MaxSize2 = maxSize2,
                PointOnEdgeFromLine = pointOnEdgeFromLine,
                PointOnEdgeFromPoint = pointOnEdgeFromPoint,
                ExtendCookieCutLines = extendCookieCutLines,
                Rotation = rotation, 
                MaxSizeGeneral = maxSizeGeneral,
                LocalAxesOnEdge = localAxesOnEdge,
                LocalAxesOnFace = localAxesOnFace,
                RestraintsOnEdge = restraintsOnEdge,
                RestraintsOnFace = restraintsOnFace,
                Group = group,
                SubMesh = subMesh,
                SubMeshSize = subMeshSize, 
            };
            return (Panel)panel.AddFragment(fragment);
        }
        public static Panel SetPanelEdgeConstraint(this Panel panel, bool edgeConstraint)
        {

            PanelEdgeConstraint fragment = new PanelEdgeConstraint()
            {
                EdgeConstraint = edgeConstraint                
            };
            return (Panel)panel.AddFragment(fragment);
        }

        public static Panel SetPanelOffset(this Panel panel, PanelOffsetType offsetType, string offsetPattern, double offsetPatternSF, double[] offset)
        {
            PanelOffset fragment = new PanelOffset()
            {
                OffsetType = offsetType,
                OffsetPattern = offsetPattern,
                OffsetPatternSF = offsetPatternSF,
                Offset = offset
            };
            return (Panel)panel.AddFragment(fragment);
        }
    }
}
