/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Structure.Loads;

namespace BH.oM.Adapters.SAP2000.Loads
{
    [Description("Load Patterns in SAP are distinct from Load Cases.")]
    public class LoadPattern : Loadcase, ICase, IBHoMObject, IObject
    {
        [Description("A unique Name for the LoadPattern.")]
        public override string Name { get; set; }
        public override LoadNature Nature { get; set; }
        [Description("SAP2000 Load Patterns do not have numbers. This field is unused.")]
        public override int Number { get; set; }
    }    
}


