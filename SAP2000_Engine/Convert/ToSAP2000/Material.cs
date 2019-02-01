using SAP2000v19;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common.Materials;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
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
    }
}
