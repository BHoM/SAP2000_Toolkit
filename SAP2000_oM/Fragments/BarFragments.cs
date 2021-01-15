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
    public class BarAutoMesh : IFragment
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/
        [Description("True if the frame object is to be automatically meshed by the program when the SAP analysis model is created.")]
        public virtual bool AutoMesh { get; set; } = false;
        [Description("If AutoMesh is True, the frame object is automatically meshed at intermediate joints along its length.")]
        public virtual bool AutoMeshAtPoints { get; set; } = false;
        [Description("If AutoMesh is True, the frame object is automatically meshed at intersections with other frames, area object edges, and solid object edges.")]
        public virtual bool AutoMeshAtLines { get; set; } = false;
        [Description("If AutoMesh is True, the minimum number of elements into which the frame object is automatically meshed. If zero, the number of elements is not checked when the automatic meshing is done.")]
        public virtual int NumSegs { get; set; } = 0;
        [Description("If AutoMesh is True, the maximum length of auto meshed frame elements. If zero, the element length is not checked when automatic meshing is done.")]
        public virtual double AutoMeshMaxLength { get; set; } = 0.0;

        /***************************************************/
    }
    public class BarDesignProcedure : IFragment
    {
        [Description("Design procedure based on material type.")]
        public virtual DesignProcedureType DesignProcedure { get; set; } = DesignProcedureType.NoDesign;
    }
    public class InsertionPoint : IFragment
    { 
        [Description("Bar insertion point based on cross section.")]
        public virtual BarInsertionPoint BarInsertionPoint { get; set; } = BarInsertionPoint.Centroid;

        [Description("Transform frame stiffness for offsets from centroid.")]
        public virtual bool ModifyStiffness { get; set; } = true;
    }

}
