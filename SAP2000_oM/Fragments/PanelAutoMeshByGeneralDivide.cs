/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{
    [Description("Divide the panel based on points and lines in the meshing group and a maximum size.")]
    public class PanelAutoMeshByGeneralDivide : IPanelAutoMesh, IFragment
    {
        [Length]
        [Description("This is the maximum size of objects created by " + "the General Divide Tool.")]
        public virtual double MaxSizeGeneral { get; set; }
        public virtual bool LocalAxesOnEdge { get; set; } = false;
        public virtual bool LocalAxesOnFace { get; set; } = false;
        public virtual bool RestraintsOnEdge { get; set; } = false;
        public virtual bool RestraintsOnFace { get; set; } = false;
        public virtual string Group { get; set; } = "ALL";
        public virtual bool SubMesh { get; set; } = false;
        public virtual double SubMeshSize { get; set; } = 0;
    }
}
