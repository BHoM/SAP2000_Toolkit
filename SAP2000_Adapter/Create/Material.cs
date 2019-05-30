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
            int ret = 0;

            eMatType matType = eMatType.NoDesign;
            int colour = 0;
            string guid = "";
            string notes = "";
            string name = "";

            if (m_model.PropMaterial.GetMaterial(material.Name, ref matType, ref colour, ref notes, ref guid) != 0)
            {
                if (m_model.PropMaterial.AddMaterial(ref name, material.GetMaterialType(), "", "", "", material.Name) != 0)
                {
                    ret += m_model.PropMaterial.SetMaterial(material.Name, material.GetMaterialType());
                }
                if (material is IIsotropic)
                {
                    IIsotropic isotropic = material as IIsotropic;
                    ret += m_model.PropMaterial.SetMPIsotropic(material.Name, isotropic.YoungsModulus, isotropic.PoissonsRatio, isotropic.ThermalExpansionCoeff);
                }
                else if (material is IOrthotropic)
                {
                    IOrthotropic orthoTropic = material as IOrthotropic;
                    double[] e = orthoTropic.YoungsModulus.ToDoubleArray();
                    double[] v = orthoTropic.PoissonsRatio.ToDoubleArray();
                    double[] a = orthoTropic.ThermalExpansionCoeff.ToDoubleArray();
                    double[] g = orthoTropic.ShearModulus.ToDoubleArray();
                    ret += m_model.PropMaterial.SetMPOrthotropic(material.Name, ref e, ref v, ref a, ref g);
                }
                ret += m_model.PropMaterial.SetWeightAndMass(material.Name, 0, material.Density);
            }

            return ret == 0;

        }

        /***************************************************/
    }
}
