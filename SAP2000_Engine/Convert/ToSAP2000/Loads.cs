using BH.oM.Structure.Loads;

#if Debug21 || Release21
using SAP2000v1;
#else
using SAP2000v19;
#endif

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static eLoadPatternType ToCSI(this LoadNature loadNature)
        {
            eLoadPatternType patType = new eLoadPatternType();

            switch (loadNature)
            {
                case LoadNature.Dead:
                    patType = eLoadPatternType.Dead;
                    break;
                case LoadNature.SuperDead:
                    patType = eLoadPatternType.SuperDead;
                    break;
                case LoadNature.Live:
                    patType = eLoadPatternType.Live;
                    break;
                case LoadNature.Seismic:
                    patType = eLoadPatternType.Quake;
                    break;
                case LoadNature.Wind:
                    patType = eLoadPatternType.Wind;
                    break;
                case LoadNature.Snow:
                    patType = eLoadPatternType.Snow;
                    break;
                case LoadNature.Other:
                    patType = eLoadPatternType.Other;
                    break;
                case LoadNature.Temperature:
                    patType = eLoadPatternType.Temperature;
                    break;
                case LoadNature.Notional:
                    patType = eLoadPatternType.Notional;
                    break;
                case LoadNature.Prestress:
                    patType = eLoadPatternType.Prestress;
                    break;
                default:
                    patType = eLoadPatternType.Other;
                    break;
            }

            return patType;
        }

        /***************************************************/

        public static string ToCSI(this LoadAxis axis)
        {
            string cSys = "";

            switch (axis)
            {
                case LoadAxis.Global:
                    cSys = "Global";
                    break;
                case LoadAxis.Local:
                    cSys = "Local";
                    break;
                default:
                    cSys = "Global";
                    break;
            }

            return cSys;
        }

        /***************************************************/
    }
}
