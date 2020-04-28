﻿/*
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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<ISurfaceProperty> ReadSurfaceProperty(List<string> ids = null)
        {
            List<ISurfaceProperty> propertyList = new List<ISurfaceProperty>();

            Dictionary<string, IMaterialFragment> bhomMaterials = ReadMaterial().ToDictionary(x => x.CustomData[AdapterIdName].ToString());

            int nameCount = 0;
            string[] nameArr = { };

            if (ids == null)
            {
                m_model.PropArea.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }

            foreach (string id in ids)
            {
                int shellType = 0;
                bool includeDrillingDOF = true;
                string materialName = "";
                double matAng = 0;
                double thickness = 0;
                double bending = 0;
                int color = 0;
                string notes = "";
                string guid = "";

                double[] modifiers = new double[] { };

                if (m_model.PropArea.GetShell_1(id, ref shellType, ref includeDrillingDOF, ref materialName, ref matAng, ref thickness, ref bending, ref color, ref notes, ref guid) != 0)
                    Engine.Reflection.Compute.RecordWarning("Error while pulling Surface Property {id}. Check results carefully.");

                m_model.PropArea.GetModifiers(id, ref modifiers);
                                
                ConstantThickness panelConstant = new ConstantThickness();
                panelConstant.CustomData[AdapterIdName] = id;
                panelConstant.Name = id;
                panelConstant.Material = bhomMaterials[materialName];
                panelConstant.Thickness = thickness;
                panelConstant.CustomData.Add("MaterialAngle", matAng);
                panelConstant.CustomData.Add("BendingThickness", bending);
                panelConstant.CustomData.Add("Color", color);
                panelConstant.CustomData.Add("Notes", notes);
                panelConstant.CustomData.Add("GUID", guid);
                
                panelConstant.CustomData.Add("MembraneF11Modifier", modifiers[0]);
                panelConstant.CustomData.Add("MembraneF22Modifier", modifiers[1]);
                panelConstant.CustomData.Add("MembraneF12Modifier", modifiers[2]);
                panelConstant.CustomData.Add("BendingM11Modifier", modifiers[3]);
                panelConstant.CustomData.Add("BendingM22Modifier", modifiers[4]);
                panelConstant.CustomData.Add("BendingM12Modifier", modifiers[5]);
                panelConstant.CustomData.Add("ShearV13Modifier", modifiers[6]);
                panelConstant.CustomData.Add("ShearV23Modifier", modifiers[7]);
                panelConstant.CustomData.Add("MassModifier", modifiers[8]);
                panelConstant.CustomData.Add("WeightModifier", modifiers[9]);                

                propertyList.Add(panelConstant);
            }

            return propertyList;
        }
        
        /***************************************************/
    }
}
