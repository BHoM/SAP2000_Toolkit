using BH.oM.Physical.Materials;
using BH.Engine.Physical;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Adapters.SAP2000;
using SAP2000v19;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<IMaterialFragment> ReadMaterial(List<string> ids = null)
        {
            List<IMaterialFragment> materialList = new List<IMaterialFragment>();

            int nameCount = 0;
            string[] names = { };
            m_model.PropMaterial.GetNameList(ref nameCount, ref names);

            if (ids == null)
            {
                ids = names.ToList();
            }

            foreach (string materialName in ids)
            {
                eMatType matType = eMatType.NoDesign;
                int symType = 0;
                int colour = 0;
                string guid = "";
                string notes = "";

                if (m_model.PropMaterial.GetMaterial(materialName, ref matType, ref colour, ref notes, ref guid) == 0)
                {
                    m_model.PropMaterial.GetTypeOAPI(materialName, ref matType, ref symType);

                    double e = 0;
                    double v = 0;
                    double thermCo = 0;
                    double g = 0;

                    double[] E = new double[3];
                    double[] V = new double[3];
                    double[] ThermCo = new double[3];
                    double[] G = new double[3];
                    
                    if (symType == 0)// Isotropic
                    {
                        m_model.PropMaterial.GetMPIsotropic(materialName, ref e, ref v, ref thermCo, ref g);
                    }
                    else if (symType == 1) // Orthotropic
                    {
                        m_model.PropMaterial.GetMPOrthotropic(materialName, ref E, ref V, ref ThermCo, ref G);
                    }
                    else if (symType == 2) //Anisotropic
                    {
                        m_model.PropMaterial.GetMPAnisotropic(materialName, ref E, ref V, ref ThermCo, ref G);
                    }
                    else if (symType == 3) //Uniaxial
                    {
                        m_model.PropMaterial.GetMPUniaxial(materialName, ref e, ref thermCo);
                    }


                    double mass = 0;
                    double weight = 0;

                    m_model.PropMaterial.GetWeightAndMass(materialName, ref weight, ref mass);

                    double fc = 0;//compressive stress
                    double ft = 0;//tensile stress
                    double fy = 0;//yield stress
                    double fu = 0;//ultimate stress
                    double efy = 0;//expected yield stress
                    double efu = 0;//expected tensile stress
                    double strainHardening = 0;//strain at hardening
                    double strainMaxF = 0;//strain at max stress
                    double strainRupture = 0;//strain at rupture
                    double strainFc = 0;//strain at f'c
                    int i0 = 0;//stress-strain curvetype
                    int i1 = 0;//stress-strain hysteresis type
                    bool b0 = false;//is lightweight

                    IMaterialFragment m;

                    switch (matType)
                    {
                        case eMatType.Steel:
                            m_model.PropMaterial.GetOSteel(materialName, ref fy, ref fu, ref efy, ref efu, ref i0, ref i1, ref strainHardening, ref strainMaxF, ref strainRupture);
                            m = BH.Engine.Structure.Create.Steel(materialName, e, v, thermCo, mass, 0, fy, fu);
                            break;
                        case eMatType.Concrete:
                            m_model.PropMaterial.GetOConcrete(materialName, ref fc, ref b0, ref ft, ref i0, ref i1, ref efy, ref efu, ref strainFc, ref strainMaxF);
                            m = BH.Engine.Structure.Create.Concrete(materialName, e, v, thermCo, mass, 0, 0, fy);
                            break;
                        case eMatType.Aluminum:
                            m = BH.Engine.Structure.Create.Aluminium(materialName, e, v, thermCo, mass, 0);
                            break;
                        case eMatType.ColdFormed:
                            m_model.PropMaterial.GetOColdFormed(materialName, ref fy, ref fu, ref i1);
                            m = BH.Engine.Structure.Create.Steel(materialName, e, v, thermCo, mass, 0, fy, fu);
                            break;
                        case eMatType.Rebar:
                            m_model.PropMaterial.GetORebar(materialName, ref fy, ref fu, ref efy, ref efu, ref i0, ref i1, ref strainHardening, ref strainMaxF, ref b0);
                            m = BH.Engine.Structure.Create.Steel(materialName, e, v, thermCo, mass, 0, fy, fu);
                            break;
                        case eMatType.Tendon:
                            m_model.PropMaterial.GetOTendon(materialName, ref fy, ref fu, ref i0, ref i1);
                            m = BH.Engine.Structure.Create.Steel(materialName, e, v, thermCo, mass, 0, fy, fu);
                            break;
                        default:
                            m = BH.Engine.Structure.Create.Steel(materialName, mass);
                            Engine.Reflection.Compute.RecordWarning("Could not extract structural properties for material " + materialName);
                            break;
                    }

                    m.CustomData.Add(AdapterId, materialName);

                    materialList.Add(m);
                }
            }

            return materialList;
        }

        /***************************************************/
    }
}
