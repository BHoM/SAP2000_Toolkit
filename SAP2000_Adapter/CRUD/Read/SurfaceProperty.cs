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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Structure.Fragments;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapter;

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

            Dictionary<string, IMaterialFragment> bhomMaterials = ReadMaterial().ToDictionary(x => GetAdapterId<string>(x));

            int nameCount = 0;
            string[] nameArr = { };
            m_model.PropArea.GetNameList(ref nameCount, ref nameArr);

            ids = FilterIds(ids, nameArr);

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
                string guid = null;
                SAP2000Id sap2000id = new SAP2000Id();
                sap2000id.Id = id;


                if (m_model.PropArea.GetShell_1(id, ref shellType, ref includeDrillingDOF, ref materialName, ref matAng, ref thickness, ref bending, ref color, ref notes, ref guid) != 0)
                    Engine.Base.Compute.RecordWarning($"Error while pulling Surface Property {id}. Check results carefully.");
                              
                ConstantThickness bhSurfProp = new ConstantThickness();

                bhSurfProp.Name = id;
                bhSurfProp.Thickness = thickness;
                bhSurfProp.CustomData.Add("MaterialAngle", matAng);
                bhSurfProp.CustomData.Add("BendingThickness", bending);
                bhSurfProp.CustomData.Add("Color", color);
                bhSurfProp.CustomData.Add("Notes", notes);
                bhSurfProp.CustomData.Add("GUID", guid);

                IMaterialFragment bhMat = new GenericIsotropicMaterial();
                bhomMaterials.TryGetValue(materialName, out bhMat);
                bhSurfProp.Material = bhMat;

                double[] modifiers = new double[6];

                if (m_model.PropArea.GetModifiers(id, ref modifiers) == 0)
                {
                    SurfacePropertyModifier modifier = new SurfacePropertyModifier
                    {
                        FXX = modifiers[0],
                        FYY = modifiers[1],
                        FXY = modifiers[2],
                        MXX = modifiers[3],
                        MYY = modifiers[4],
                        MXY = modifiers[5],
                        VXZ = modifiers[6],
                        VYZ = modifiers[7],
                        Mass = modifiers[8],
                        Weight = modifiers[9]
                    };
                    bhSurfProp.Fragments.Add(modifier);
                }

                bhSurfProp.SetAdapterId(sap2000id);
                propertyList.Add(bhSurfProp);
            }

            return propertyList;
        }
        
        /***************************************************/
    }
}



