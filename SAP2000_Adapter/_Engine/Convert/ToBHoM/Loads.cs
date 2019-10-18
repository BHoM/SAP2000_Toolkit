using BH.oM.Structure.Loads;

#if Debug19 || Release19
using SAP2000v19;
#else
using SAP2000v1;
#endif

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadNature ToBHoM(this eLoadPatternType patType)
        {
            LoadNature nature = new LoadNature();

            switch (patType)
            {
                case eLoadPatternType.Dead:
                    nature = LoadNature.Dead;
                    break;
                case eLoadPatternType.SuperDead:
                    nature = LoadNature.SuperDead;
                    break;
                case eLoadPatternType.Live:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.Quake:
                    nature = LoadNature.Seismic;
                    break;
                case eLoadPatternType.Wind:
                    nature = LoadNature.Wind;
                    break;
                case eLoadPatternType.Snow:
                    nature = LoadNature.Snow;
                    break;
                case eLoadPatternType.Other:
                    nature = LoadNature.Other;
                    break;
                case eLoadPatternType.Temperature:
                    nature = LoadNature.Temperature;
                    break;
                case eLoadPatternType.Rooflive:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.Notional:
                    nature = LoadNature.Notional;
                    break;
                case eLoadPatternType.PatternLive:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.TemperatureGradient:
                    nature = LoadNature.Temperature;
                    break;
                case eLoadPatternType.Prestress:
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
