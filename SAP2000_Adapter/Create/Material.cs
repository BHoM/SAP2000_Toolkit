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
            eMatType matType = eMatType.NoDesign;
            int color = 0;
            string guid = "";
            string notes = "";
            string name = "";

            if (m_model.PropMaterial.GetMaterial(material.Name, ref matType, ref color, ref notes, ref guid) == 0) //use existing material if present
            {
                material.CustomData[AdapterId] = material.Name;
            }
            else if (m_model.PropMaterial.AddMaterial(ref name, matType, "United States", material.Name, material.Name, guid) == 0) //try to get the material from a dataset
            {
                material.CustomData[AdapterId] = name;
            }
            else if (m_model.PropMaterial.SetMaterial(material.Name, matType, color, notes, guid) == 0) //create the material
            {
                material.CustomData[AdapterId] = name = material.Name;

                if (material is IIsotropic)
                {
                    IIsotropic isotropic = material as IIsotropic;
                    if (m_model.PropMaterial.SetMPIsotropic(name, isotropic.YoungsModulus, isotropic.PoissonsRatio, isotropic.ThermalExpansionCoeff) != 0)
                        CreatePropertyWarning("Isotropy", "Material", name);

                }
                else if (material is IOrthotropic)
                {
                    IOrthotropic orthoTropic = material as IOrthotropic;
                    double[] e = orthoTropic.YoungsModulus.ToDoubleArray();
                    double[] v = orthoTropic.PoissonsRatio.ToDoubleArray();
                    double[] a = orthoTropic.ThermalExpansionCoeff.ToDoubleArray();
                    double[] g = orthoTropic.ShearModulus.ToDoubleArray();
                    if (m_model.PropMaterial.SetMPOrthotropic(name, ref e, ref v, ref a, ref g) != 0)
                        CreatePropertyWarning("Orthotropy", "Material", name);
                }

                if (m_model.PropMaterial.SetWeightAndMass(name, 2, material.Density) != 0)
                    CreatePropertyWarning("Density", "Material", name);
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
