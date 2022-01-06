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

using BH.oM.Physical.Materials;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Constraints;
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
            bool[] dof;
            bool[] fix;
            double[] stiff;
            double[] damp;
            double dj2;
            double dj3;

            bhLinkConstraint.ToSAP2000(out dof, out fix, out stiff, out damp, out dj2, out dj3);

            if (m_model.PropLink.SetLinear(name, ref dof, ref fix, ref stiff, ref damp, dj2, dj3) != 0)
                Engine.Reflection.Compute.RecordWarning($"SAP returned an error pushing LinkConstraint {name}. Check results.");

            SetAdapterId(bhLinkConstraint, name);
            
            return true;
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

    }
}


