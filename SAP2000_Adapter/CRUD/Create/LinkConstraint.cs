/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Physical.Materials;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Constraints;
using BH.oM.Geometry.ShapeProfiles;
using System;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(LinkConstraint bhLinkConstraint)
        {
            string name = bhLinkConstraint.DescriptionOrName();
            
            bhLinkConstraint.ToSAP2000(out bool[] DOF, out bool[] Fixed, out double[] Ke, out double[] Ce, out double DJ2, out double DJ3);

            if (m_model.PropLink.SetLinear(name, ref DOF, ref Fixed, ref Ke, ref Ce, DJ2, DJ3) != 0)
                Engine.Reflection.Compute.RecordWarning($"SAP returned an error pushing LinkConstraint {name}. Check results.");

            bhLinkConstraint.CustomData[AdapterIdName] = name;
            
            return false;
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

    }
}
