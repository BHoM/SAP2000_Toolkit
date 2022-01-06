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

using BH.Engine.Structure;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Fragments;
using BH.Engine.Base;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/
        private bool CreateObject(ISurfaceProperty surfaceProperty)
        {
            string propName = surfaceProperty.DescriptionOrName();
            string matName = "Default";
            if (surfaceProperty.Material != null)
            {
                matName = GetAdapterId<string>(surfaceProperty.Material);
            }
            else Engine.Reflection.Compute.RecordWarning($"SurfaceProperty {propName} had no material defined. Using a default material.");

            SAP2000Id sap2000id = new SAP2000Id();
            
            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "Panel", propName);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "Panel", propName);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "Panel", propName);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                int shellType = 1;
                bool includeDrillingDOF = true;
                if (m_model.PropArea.SetShell_1(propName, shellType, includeDrillingDOF, matName, 0, constantThickness.Thickness, constantThickness.Thickness) != 0)
                    CreatePropertyError("ConstantThickness", "SurfaceProperty", propName);
            }

            sap2000id.Id = propName;
            surfaceProperty.SetAdapterId(sap2000id);

            SurfacePropertyModifier modifier = surfaceProperty.FindFragment<SurfacePropertyModifier>();
            if (modifier != null)
            {
                double[] modifiers = new double[] 
                {
                    modifier.FXX,
                    modifier.FYY,
                    modifier.FXY,
                    modifier.MXX,
                    modifier.MYY,
                    modifier.MXY,
                    modifier.VXZ,
                    modifier.VYZ,
                    modifier.Mass,
                    modifier.Weight
                };

            if (m_model.PropArea.SetModifiers(propName, ref modifiers) != 0)
                    CreatePropertyError("Modifiers", "SurfaceProperty", propName);
            }

            return true;
        }

        /***************************************************/
    }
}


