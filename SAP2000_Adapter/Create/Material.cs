using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.Engine.SAP2000;
using SAP2000v19;

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
                material.CustomData[AdapterId] = name;
            }
            else if (m_model.PropMaterial.SetMaterial(material.Name, matType, color, notes, guid) == 0) //create the material
            {
                material.CustomData[AdapterId] = material.Name;

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
