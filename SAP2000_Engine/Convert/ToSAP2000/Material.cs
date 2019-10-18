using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;

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
