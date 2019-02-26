using BH.oM.Common.Materials;
using SAP2000v19;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialType GetMaterialType(eMatType materialType)
        {
            switch (materialType)
            {
                case eMatType.Steel:
                    return MaterialType.Steel;
                case eMatType.Concrete:
                    return MaterialType.Concrete;
                case eMatType.NoDesign://No material of this type in BHoM !!!
                    return MaterialType.Steel;
                case eMatType.Aluminum:
                    return MaterialType.Aluminium;
                case eMatType.ColdFormed:
                    return MaterialType.Steel;
                case eMatType.Rebar:
                    return MaterialType.Rebar;
                case eMatType.Tendon:
                    return MaterialType.Tendon;
                case eMatType.Masonry://No material of this type in BHoM !!!
                    return MaterialType.Concrete;
                default:
                    return MaterialType.Steel;
            }
        }

        /***************************************************/
    }
}
