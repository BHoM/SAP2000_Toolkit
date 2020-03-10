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

using BH.Engine.SAP2000;
using BH.oM.Structure.MaterialFragments;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(IMaterialFragment material)
        {
            eMatType matType = material.GetMaterialType();
            int color = 0;
            string guid = "";
            string notes = "";
            string name = "";

            if (m_model.PropMaterial.AddMaterial(ref name, matType, "United States", material.Name, material.Name, guid) == 0) //try to get the material from a dataset
            {
                material.CustomData[AdapterIdName] = name;
            }
            else if (m_model.PropMaterial.SetMaterial(material.Name, matType, color, notes, guid) == 0) //create the material
            {
                material.CustomData[AdapterIdName] = material.Name;

                if (material is IIsotropic)
                {
                    IIsotropic isotropic = material as IIsotropic;
                    if (m_model.PropMaterial.SetMPIsotropic(material.Name, isotropic.YoungsModulus, isotropic.PoissonsRatio, isotropic.ThermalExpansionCoeff) != 0)
                        CreatePropertyWarning("Isotropy", "Material", material.Name);

                }
                else if (material is IOrthotropic)
                {
                    IOrthotropic orthoTropic = material as IOrthotropic;
                    double[] e = orthoTropic.YoungsModulus.ToDoubleArray();
                    double[] v = orthoTropic.PoissonsRatio.ToDoubleArray();
                    double[] a = orthoTropic.ThermalExpansionCoeff.ToDoubleArray();
                    double[] g = orthoTropic.ShearModulus.ToDoubleArray();
                    if (m_model.PropMaterial.SetMPOrthotropic(material.Name, ref e, ref v, ref a, ref g) != 0)
                        CreatePropertyWarning("Orthotropy", "Material", material.Name);
                }

                if (m_model.PropMaterial.SetWeightAndMass(material.Name, 2, material.Density) != 0)
                    CreatePropertyWarning("Density", "Material", material.Name);
            }
            else
            {
                CreateElementError("Material", material.Name);
            }

            return true;
        }

        /***************************************************/
    }
}
