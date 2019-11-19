using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static eMatType GetMaterialType(this IMaterialFragment material)
        {
            if (material is Steel)
                return eMatType.Steel;
            else if (material is Concrete)
                return eMatType.Concrete;
            else if (material is Aluminium)
                return eMatType.Aluminum;
            else if (material is Timber)
                return eMatType.NoDesign;
            else
                return eMatType.NoDesign;
        }

        /***************************************************/

        public static double[] ToDoubleArray(this Vector v)
        {
            return new double[] { v.X, v.Y, v.Z };
        }

        /***************************************************/
    }
}
