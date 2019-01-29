using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Loads;
using BH.oM.Common.Materials;
using SAP2000v19;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private List<Material> ReadMaterials(List<string> ids = null)
        {
            int nameCount = 0;
            string[] names = { };
            List<Material> materialList = new List<Material>();

            if (ids == null)
            {
                m_model.PropMaterial.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            foreach (string id in ids)
            {
                eMatType matType = eMatType.NoDesign;
                int colour = 0;
                string guid = "";
                string notes = "";
                if (m_model.PropMaterial.GetMaterial(id, ref matType, ref colour, ref notes, ref guid) == 0)
                {
                    double e = 0;
                    double v = 0;
                    double thermCo = 0;
                    double g = 0;

                    m_model.PropMaterial.GetMPIsotropic(id, ref e, ref v, ref thermCo, ref g);

                    double mass = 0;
                    double weight = 0;

                    m_model.PropMaterial.GetWeightAndMass(id, ref weight, ref mass);

                    double compStr = 0;
                    double tensStr = 0;
                    double v1 = 0;//expected yield stress
                    double v2 = 0;//expected tensile stress
                    double v3 = 0;//strain at hardening
                    double v4 = 0;//strain at max stress
                    double v5 = 0;//strain at rupture
                    int i1 = 0;//stress-strain curvetype
                    int i2 = 0;
                    bool b1 = false;

                    Material m = new Material();
                    m.Name = id;
                    m.Type = Helper.GetMaterialType(matType);
                    m.PoissonsRatio = v;
                    m.YoungsModulus = e;
                    m.CoeffThermalExpansion = thermCo;
                    m.Density = mass;

                    if (m_model.PropMaterial.GetOSteel(id, ref compStr, ref tensStr, ref v1, ref v2, ref i1, ref i2, ref v3, ref v4, ref v5) == 0)
                    {
                        m.CompressiveYieldStrength = compStr;
                        m.TensileYieldStrength = compStr;
                    }
                    else if (m_model.PropMaterial.GetOConcrete(id, ref compStr, ref b1, ref tensStr, ref i1, ref i2, ref v1, ref v2, ref v3, ref v4) == 0)
                    {
                        m.CompressiveYieldStrength = compStr;
                        m.TensileYieldStrength = compStr * tensStr;
                    }

                    m.CustomData.Add(AdapterId, id);
                    //TODO: add get methods for Tendon and Rebar    

                    materialList.Add(m);
                }
            }

            return materialList;
        }
    }
}
