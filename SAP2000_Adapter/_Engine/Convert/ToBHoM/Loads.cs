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

        public static LoadNature ToBHoM(this SAP.eLoadPatternType patType)
        {
            LoadNature nature = new LoadNature();

            switch (patType)
            {
                case SAP.eLoadPatternType.Dead:
                    nature = LoadNature.Dead;
                    break;
                case SAP.eLoadPatternType.SuperDead:
                    nature = LoadNature.SuperDead;
                    break;
                case SAP.eLoadPatternType.Live:
                    nature = LoadNature.Live;
                    break;
                case SAP.eLoadPatternType.Quake:
                    nature = LoadNature.Seismic;
                    break;
                case SAP.eLoadPatternType.Wind:
                    nature = LoadNature.Wind;
                    break;
                case SAP.eLoadPatternType.Snow:
                    nature = LoadNature.Snow;
                    break;
                case SAP.eLoadPatternType.Other:
                    nature = LoadNature.Other;
                    break;
                case SAP.eLoadPatternType.Temperature:
                    nature = LoadNature.Temperature;
                    break;
                case SAP.eLoadPatternType.Rooflive:
                    nature = LoadNature.Live;
                    break;
                case SAP.eLoadPatternType.Notional:
                    nature = LoadNature.Notional;
                    break;
                case SAP.eLoadPatternType.PatternLive:
                    nature = LoadNature.Live;
                    break;
                case SAP.eLoadPatternType.TemperatureGradient:
                    nature = LoadNature.Temperature;
                    break;
                case SAP.eLoadPatternType.Prestress:
                    nature = LoadNature.Prestress;
                    break;
                default:
                    nature = LoadNature.Other;
                    break;
            }

            return nature;
        }

        /***************************************************/
    }
}
