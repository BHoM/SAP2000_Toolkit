using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;

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

        public static SAP.eMatType GetMaterialType(this IMaterialFragment material)
        {
            if (material is Steel)
                return SAP.eMatType.Steel;
            else if (material is Concrete)
                return SAP.eMatType.Concrete;
            else if (material is Aluminium)
                return SAP.eMatType.Aluminum;
            else if (material is Timber)
                return SAP.eMatType.NoDesign;
            else
                return SAP.eMatType.NoDesign;
        }

        /***************************************************/

        public static double[] ToDoubleArray(this Vector v)
        {
            return new double[] { v.X, v.Y, v.Z };
        }

        /***************************************************/
    }
}
