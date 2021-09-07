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
    public partial interface IPanelOffset : IFragment
    {

    }

    public class PanelOffsetByJointPattern : IPanelOffset
    {
        [Description("This is the name of the defined joint pattern that is used to calculate the joint offsets.")]
        public virtual string OffsetPattern { get; set; } = "";
        [Description("This is the scale factor applied to the joint pattern when calculating the joint offsets.")]
        public virtual double OffsetPatternSF { get; set; } = 0;
    }

    public class PanelOffsetByPoint : IPanelOffset
    {
        [Description("This is an array of joint offsets for each of the points that define the area object.")]
        public virtual double[] Offset { get; set; } = null;
    }
}
