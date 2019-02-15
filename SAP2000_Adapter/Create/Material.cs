using BH.oM.Common.Materials;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Material material)
        {
            int ret = 0;

            eMatType matType = eMatType.NoDesign;
            int colour = 0;
            string guid = "";
            string notes = "";

            if (m_model.PropMaterial.GetMaterial(material.Name, ref matType, ref colour, ref notes, ref guid) != 0)
            {
                ret += m_model.PropMaterial.SetMaterial(material.Name, BH.Engine.SAP2000.Convert.GetMaterialType(material.Type));
                ret += m_model.PropMaterial.SetMPIsotropic(material.Name, material.YoungsModulus, material.PoissonsRatio, material.CoeffThermalExpansion);
                ret += m_model.PropMaterial.SetWeightAndMass(material.Name, 0, material.Density);
            }

            return ret == 0;
            
        }
    }
}
