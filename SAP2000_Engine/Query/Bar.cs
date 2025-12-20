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
    public static partial class Query
    {
        [Description("Returns the SAP2000 BarAutoMesh settings for a bar. You can also use the method FindFragment() with the type BarAutoMesh as an argument.")]
        [Input("bar", "A Bar which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("BarAutoMesh", "A fragment containing SAP2000 BarAutoMesh settings.")]
        public static BarAutoMesh BarAutoMesh(this Bar bar)
        {
            return bar?.FindFragment<BarAutoMesh>();
        }

        [Description("Returns the SAP2000 BarDesignProcedure settings for a bar. You can also use the method FindFragment() with the type BarDesignProcedure as an argument.")]
        [Input("bar", "A Bar which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("BarDesignProcedure", "A fragment containing SAP2000 BarDesignProcedure settings.")]
        public static BarDesignProcedure BarDesignProcedure(this Bar bar)
        {
            return bar?.FindFragment<BarDesignProcedure>();
        }

        [Description("Returns the SAP2000 BarInsertionPointLocation settings for a bar. You can also use the method FindFragment() with the type BarInsertionPoint as an argument.")]
        [Input("bar", "A Bar which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("BarInsertionPoint", "The insertion point in SAP2000 for the bar.")]
        public static BarInsertionPointLocation BarInsertionPoint(this Bar bar)
        {
            BarInsertionPoint o = bar?.FindFragment<BarInsertionPoint>();
            return o == null ? BarInsertionPointLocation.Centroid : o.InsertionPoint;
        }

        [Description("Checks if SAP2000 is set to modify the stiffness of a bar based on its insertion point. You can also use the method FindFragment() with the type BarInsertionPoint as an argument, and check the ModifyStiffness property of that fragment.")]
        [Input("bar", "A Bar which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("BarModifyStiffnessInsertionPoint", "Whether SAP2000 is set to transform frame stiffness for offsets from centroid.")]
        public static bool BarModifyStiffnessInsertionPoint(this Bar bar)
        {
            BarInsertionPoint o = bar?.FindFragment<BarInsertionPoint>();

            return o == null ? true : o.ModifyStiffness;
        }
    }
}





