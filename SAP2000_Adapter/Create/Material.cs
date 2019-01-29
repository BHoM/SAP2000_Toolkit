using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private bool CreateObject(Material material)
        {
            int ret = 0;

            eMatType matType = eMatType.NoDesign;
            int colour = 0;
            string guid = "";
            string notes = "";

            if (m_model.PropMaterial.GetMaterial(material.Name, ref matType, ref colour, ref notes, ref guid) != 0)
            {
                ret += m_model.PropMaterial.SetMaterial(material.Name, Helper.GetMaterialType(material.Type));
                ret += m_model.PropMaterial.SetMPIsotropic(material.Name, material.YoungsModulus, material.PoissonsRatio, material.CoeffThermalExpansion);
                ret += m_model.PropMaterial.SetWeightAndMass(material.Name, 0, material.Density);
            }

            return ret == 0;
            
        }
    }
}
