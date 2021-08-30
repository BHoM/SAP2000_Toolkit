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

using BH.oM.Physical.Materials;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Structure;
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using System;
using System.Collections.Generic;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(BHoMGroup<SteelSection> bhomGroup)
        {
            bool success = false;
            List<string> names = new List<string>();

            foreach (SteelSection obj in bhomGroup.Elements)
            {
                success &= CreateObject(obj as dynamic);
                names.Add(GetAdapterId<string>(obj));
            }

            string[] nameArr = names.ToArray();

            if (m_model.PropFrame.SetAutoSelectSteel(bhomGroup.Name, nameArr.Length, ref nameArr, "Median", "Made By BHoM") != 0)
            {
                Engine.Reflection.Compute.RecordWarning("Could not create autoselect group");
            }

            SAP2000Id sap2000IdFragment = new SAP2000Id { Id = bhomGroup.Name };
            bhomGroup.AddFragment(sap2000IdFragment);

            return success;
        }

        /***************************************************/

        private bool CreateObject(BHoMGroup<AluminiumSection> bhomGroup)
        {
            bool success = false;
            List<string> names = new List<string>();

            foreach (AluminiumSection obj in bhomGroup.Elements)
            {
                success &= CreateObject(obj as dynamic);
                names.Add(GetAdapterId<string>(obj));
            }

            string[] nameArr = names.ToArray();

            if (m_model.PropFrame.SetAutoSelectSteel(bhomGroup.Name, nameArr.Length, ref nameArr, "Median", "Made By BHoM") != 0)
            {
                Engine.Reflection.Compute.RecordWarning("Could not create autoselect group");
            }

            SAP2000Id sap2000IdFragment = new SAP2000Id { Id = bhomGroup.Name };
            bhomGroup.AddFragment(sap2000IdFragment);

            return success;
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

    }
}

