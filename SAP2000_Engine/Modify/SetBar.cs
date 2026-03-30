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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.SAP2000.Elements;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Base;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Modify
    {
        [Description("Sets SAP2000 automatic meshing settings on a Bar by adding or replacing a BarAutoMesh fragment.")]
        [Input("bar", "The Bar to set the auto mesh settings on.")]
        [Input("autoMesh", "If true, the bar is automatically meshed by the SAP2000 program when the analysis model is created.")]
        [Input("autoMeshAtPoints", "If true and autoMesh is true, the bar is automatically meshed at intermediate joints along its length.")]
        [Input("autoMeshAtLines", "If true and autoMesh is true, the bar is automatically meshed at intersections with other frames, area edges and solid edges.")]
        [Input("numSegs", "The minimum number of elements into which the bar is automatically meshed. If zero, the number of elements is not checked when automatic meshing is done.")]
        [Input("autoMeshMaxLength", "The maximum length of auto meshed elements. If zero, the element length is not checked when automatic meshing is done.")]
        [Output("bar", "The Bar with the BarAutoMesh fragment added or replaced.")]
        public static Bar SetBarAutoMesh(this Bar bar, bool autoMesh = false, bool autoMeshAtPoints = false, bool autoMeshAtLines = false, int numSegs = 0, double autoMeshMaxLength = 0.0)
        {
            if (numSegs < 0)
            {
                numSegs = 0;
                Engine.Base.Compute.RecordWarning("Number of segments must be positive or zero. If zero, number of elements is not checked when automatic meshing is done.");
            }

            if (autoMeshMaxLength < 0)
            {
                autoMeshMaxLength = 0.0;
                Engine.Base.Compute.RecordWarning("Max length must be positive. If zero, element length is not checked when automatic meshing is done.");
            }

            return (Bar)bar.AddFragment(new BarAutoMesh { AutoMesh = autoMesh, AutoMeshAtPoints = autoMeshAtPoints, AutoMeshAtLines = autoMeshAtLines, NumSegs = numSegs, AutoMeshMaxLength = autoMeshMaxLength }, true);
        }

        [Description("Sets the SAP2000 design procedure on a Bar by adding or replacing a BarDesignProcedure fragment.")]
        [Input("bar", "The Bar to set the design procedure on.")]
        [Input("designProcedure", "The design procedure type to assign to the bar.")]
        [Output("bar", "The Bar with the BarDesignProcedure fragment added or replaced.")]
        public static Bar SetBarDesignProcedure(this Bar bar, BarDesignProcedureType designProcedure)
        {
            return (Bar)bar.AddFragment(new BarDesignProcedure { DesignProcedure = designProcedure }, true);
        }

        [Description("Sets the SAP2000 insertion point on a Bar with stiffness modification enabled by default. Calls the overload with modifyStiffness set to true.")]
        [Input("bar", "The Bar to set the insertion point on.")]
        [Input("barInsertionPoint", "The insertion point location to assign to the bar.")]
        [Output("bar", "The Bar with the BarInsertionPoint fragment added or replaced.")]
        public static Bar SetBarInsertionPoint(this Bar bar, BarInsertionPointLocation barInsertionPoint = BarInsertionPointLocation.Centroid)
        {
            return bar.SetBarInsertionPoint(barInsertionPoint, true);
        }

        [Description("Sets the SAP2000 insertion point on a Bar by adding or replacing a BarInsertionPoint fragment, with explicit control over stiffness modification.")]
        [Input("bar", "The Bar to set the insertion point on.")]
        [Input("insertionPoint", "The insertion point location to assign to the bar.")]
        [Input("modifyStiffness", "If true, the frame stiffness is transformed to account for offsets from the centroid.")]
        [Output("bar", "The Bar with the BarInsertionPoint fragment added or replaced.")]
        public static Bar SetBarInsertionPoint(this Bar bar, BarInsertionPointLocation insertionPoint = BarInsertionPointLocation.Centroid, bool modifyStiffness = true)
        {
            return (Bar)bar.AddFragment(new BarInsertionPoint() { InsertionPoint = insertionPoint, ModifyStiffness = modifyStiffness }, true);
        }
    }
}





