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


using BH.oM.Structure.MaterialFragments;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Structure;
using BH.oM.Adapters.SAP2000;
using System.ComponentModel;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(IMaterialFragment material)
        {
            bool success = true;
            eMatType matType = material.GetMaterialType();
            string bhName = material.DescriptionOrName();
            int color = 0;
            string guid = null;
            string notes = "";
            string name = "";
            SAP2000Id sap2000id = new SAP2000Id();

            if (m_model.PropMaterial.AddMaterial(ref name, matType, "United States", bhName, bhName, guid) == 0) //try to get the material from a dataset
            {
                sap2000id.Id = name;
            }
            else if (m_model.PropMaterial.SetMaterial(bhName, matType, color, notes, guid) == 0) //create the material
            {
                sap2000id.Id = bhName;
                SetObject(material);

            }
            else
            {
                CreateElementError("Material", bhName);
            }
            SetAdapterId(material, sap2000id);

            return success;
        }

        [Description("Does all the SAP2000 Interaction which does not initiate a new material object in SAP2000.")]
        private bool SetObject(IMaterialFragment material)
        {
            bool success = true;
            string bhName = material.DescriptionOrName();

            if (material is IIsotropic)
            {
                IIsotropic isotropic = material as IIsotropic;
                if (m_model.PropMaterial.SetMPIsotropic(bhName, isotropic.YoungsModulus, isotropic.PoissonsRatio, isotropic.ThermalExpansionCoeff) != 0)
                    CreatePropertyWarning("Isotropy", "Material", bhName);

            }
            else if (material is IOrthotropic)
            {
                IOrthotropic orthoTropic = material as IOrthotropic;
                double[] e = orthoTropic.YoungsModulus.ToDoubleArray();
                double[] v = orthoTropic.PoissonsRatio.ToDoubleArray();
                double[] a = orthoTropic.ThermalExpansionCoeff.ToDoubleArray();
                double[] g = orthoTropic.ShearModulus.ToDoubleArray();
                if (m_model.PropMaterial.SetMPOrthotropic(bhName, ref e, ref v, ref a, ref g) != 0)
                    CreatePropertyWarning("Orthotropy", "Material", bhName);
            }

            // Set Material Strengths
            eMatType matType = MaterialTypeToCSI(material.IMaterialType());

            switch (matType)
            {
                case eMatType.Aluminum:
                    Engine.Reflection.Compute.RecordWarning("BHoM material aluminum does not have SAP2000 material parameters yet.");
                    break;
                case eMatType.Steel:
                    // try/catch for casting to steel?
                    Steel steel = material as Steel;
                    if (m_model.PropMaterial.SetOSteel_1(bhName, steel.YieldStress, steel.UltimateStress, steel.YieldStress, steel.UltimateStress, 0, 0, 0, 0, 0, 0, 0) != 0)
                    {
                        CreatePropertyWarning("YieldStress", "Material", bhName);
                        CreatePropertyWarning("UltimateStress", "Material", bhName);
                    }
                    break;
                case eMatType.Concrete:
                    Concrete concrete = material as Concrete;
                    // name as NW/LW?
                    if (m_model.PropMaterial.SetOConcrete_2(bhName, concrete.CylinderStrength, concrete.CylinderStrength, false, 0, 0, 0, 0, 0, 0) != 0)
                    {
                        CreatePropertyWarning("ConcreteStrength", "Material", bhName);
                    }
                    break;
                case eMatType.Rebar:
                    Steel reinf = material as Steel;
                    if (m_model.PropMaterial.SetORebar_1(bhName, reinf.YieldStress, reinf.UltimateStress, reinf.YieldStress, reinf.UltimateStress, 0, 0, 0, 0, 0, false) != 0)
                    {
                        CreatePropertyWarning("YieldStress", "Material", bhName);
                        CreatePropertyWarning("UltimateStress", "Material", bhName);
                    }
                    break;
                case eMatType.Tendon:
                    Steel tendon = material as Steel;
                    if (m_model.PropMaterial.SetOTendon_1(bhName, tendon.YieldStress, tendon.UltimateStress, 0, 0, 0) != 0)
                    {
                        CreatePropertyWarning("YieldStress", "Material", bhName);
                        CreatePropertyWarning("UltimateStress", "Material", bhName);
                    }
                    break;
                default:
                    Engine.Reflection.Compute.RecordWarning("BHoM material type not found, no additional design material parameters passed to SAP2000.");
                    m_model.PropMaterial.SetONoDesign(bhName, 0, 0, 0);
                    break;
            }


            if (m_model.PropMaterial.SetWeightAndMass(bhName, 2, material.Density) != 0)
                CreatePropertyWarning("Density", "Material", bhName);

            SetAdapterId(material, material.Name);

            return success;
        }

        private eMatType MaterialTypeToCSI(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Aluminium:
                    return eMatType.Aluminum;
                case MaterialType.Steel:
                    return eMatType.Steel;
                case MaterialType.Concrete:
                    return eMatType.Concrete;
                case MaterialType.Timber:
                    Engine.Reflection.Compute.RecordWarning("SAP2000 does not contain a definition for Timber materials, the material has been set to type 'Other' with 'Orthotropic' directional symmetry");
                    return eMatType.NoDesign;
                case MaterialType.Rebar:
                    return eMatType.Rebar;
                case MaterialType.Tendon:
                    return eMatType.Tendon;
                case MaterialType.Glass:
                    Engine.Reflection.Compute.RecordWarning("SAP2000 does not contain a definition for Glass materials, the material has been set to type 'Other'");
                    return eMatType.NoDesign;
                case MaterialType.Cable:
                    Engine.Reflection.Compute.RecordWarning("SAP2000 does not contain a definition for Cable materials, the material has been set to type 'Steel'");
                    return eMatType.Steel;
                default:
                    Engine.Reflection.Compute.RecordWarning("BHoM material type not found, the material has been set to type 'Other'");
                    return eMatType.NoDesign;
            }
        }

        /***************************************************/
    }
}


