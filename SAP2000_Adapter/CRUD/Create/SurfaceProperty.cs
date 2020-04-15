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

using BH.Engine.Structure;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/
        private bool CreateObject(ISurfaceProperty surfaceProperty)
        {
            string materialName = surfaceProperty.Material.CustomData[AdapterIdName].ToString();

            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                int shellType = 1;
                bool includeDrillingDOF = true;
                string material = constantThickness.Material.CustomData[AdapterIdName].ToString();
                if (m_model.PropArea.SetShell_1(surfaceProperty.Name, shellType, includeDrillingDOF, material, 0, constantThickness.Thickness, constantThickness.Thickness) != 0)
                    CreatePropertyError("ConstantThickness", "SurfaceProperty", surfaceProperty.Name);
            }

            surfaceProperty.CustomData[AdapterIdName] = surfaceProperty.Name;

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                if (m_model.PropArea.SetModifiers(surfaceProperty.Name, ref modifier) != 0)
                    CreatePropertyError("Modifiers", "SurfaceProperty", surfaceProperty.Name);
            }

            return true;
        }

        /***************************************************/
    }
}
