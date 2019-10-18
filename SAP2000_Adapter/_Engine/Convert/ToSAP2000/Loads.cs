using BH.oM.Structure.Loads;

#if Debug19 || Release19
using SAP = SAP2000v19;
#else
using SAP = SAP2000v1;
#endif

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SAP.eLoadPatternType ToCSI(this LoadNature loadNature)
        {
            SAP.eLoadPatternType patType = new SAP.eLoadPatternType();

            switch (loadNature)
            {
                case LoadNature.Dead:
                    patType = SAP.eLoadPatternType.Dead;
                    break;
                case LoadNature.SuperDead:
                    patType = SAP.eLoadPatternType.SuperDead;
                    break;
                case LoadNature.Live:
                    patType = SAP.eLoadPatternType.Live;
                    break;
                case LoadNature.Seismic:
                    patType = SAP.eLoadPatternType.Quake;
                    break;
                case LoadNature.Wind:
                    patType = SAP.eLoadPatternType.Wind;
                    break;
                case LoadNature.Snow:
                    patType = SAP.eLoadPatternType.Snow;
                    break;
                case LoadNature.Other:
                    patType = SAP.eLoadPatternType.Other;
                    break;
                case LoadNature.Temperature:
                    patType = SAP.eLoadPatternType.Temperature;
                    break;
                case LoadNature.Notional:
                    patType = SAP.eLoadPatternType.Notional;
                    break;
                case LoadNature.Prestress:
                    patType = SAP.eLoadPatternType.Prestress;
                    break;
                default:
                    patType = SAP.eLoadPatternType.Other;
                    break;
            }

            return patType;
        }

        /***************************************************/
    }
}
