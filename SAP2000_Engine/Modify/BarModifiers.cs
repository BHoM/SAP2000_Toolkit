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
        public static Bar SetAutoMesh(this Bar bar, bool autoMesh = false, bool autoMeshAtPoints = false, bool autoMeshAtLines = false, int numSegs = 0, double autoMeshMaxLength = 0.0)
        {
            if (numSegs < 0)
            {
                numSegs = 0;
                Engine.Reflection.Compute.RecordWarning("Number of segments must be positive or zero. If zero, number of elements is not checked when automatic meshing is done.");
            }

            if (autoMeshMaxLength < 0)
            {
                autoMeshMaxLength = 0.0;
                Engine.Reflection.Compute.RecordWarning("Max length must be positive. If zero, element length is not checked when automatic meshing is done.");
            }

            return (Bar)bar.AddFragment(new BarAutoMesh { AutoMesh = autoMesh, AutoMeshAtPoints = autoMeshAtPoints, AutoMeshAtLines = autoMeshAtLines, NumSegs = numSegs, AutoMeshMaxLength = autoMeshMaxLength }, true);
        }

        public static Bar SetDesignProcedure(this Bar bar, DesignProcedureType designProcedure)
        {
            return (Bar)bar.AddFragment(new BarDesignProcedure { DesignProcedure = designProcedure }, true);
        }

        public static Bar SetInsertionPoint(this Bar bar, BarInsertionPoint barInsertionPoint = BarInsertionPoint.Centroid)
        {
            return bar.SetInsertionPoint(barInsertionPoint, true);
        }

        public static Bar SetInsertionPoint(this Bar bar, BarInsertionPoint barInsertionPoint = BarInsertionPoint.Centroid, bool modifyStiffness = true)
        {
            return (Bar)bar.AddFragment(new InsertionPoint() { BarInsertionPoint = barInsertionPoint, ModifyStiffness = modifyStiffness }, true);
        }
    }
}
