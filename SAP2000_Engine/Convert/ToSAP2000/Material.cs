using BH.oM.Common.Materials;
using SAP2000v19;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static eMatType GetMaterialType(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Aluminium:
                    return eMatType.Aluminum;
                case MaterialType.Steel:
                    return eMatType.Steel;
                case MaterialType.Concrete:
                    return eMatType.Concrete;
                case MaterialType.Timber://no material of this type in ETABS !!! 
                    return eMatType.Steel;
                case MaterialType.Rebar:
                    return eMatType.Rebar;
                case MaterialType.Tendon:
                    return eMatType.Tendon;
                case MaterialType.Glass://no material of this type in ETABS !!!
                    return eMatType.Steel;
                case MaterialType.Cable://no material of this type in ETABS !!!
                    return eMatType.Steel;
                default:
                    return eMatType.Steel;
            }
        }

        /***************************************************/
    }
}
