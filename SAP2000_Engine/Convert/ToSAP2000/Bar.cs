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

        public static oM.Geometry.Vector BarLocalAxisToCSI(this oM.Geometry.Vector axisBHoM)
        {
            return Engine.Geometry.Modify.Transform(axisBHoM, barLocalAxisToCSI);
        }

        /***************************************************/

        public static oM.Geometry.TransformMatrix barLocalAxisToCSI = Engine.Geometry.Modify.Transpose(barLocalAxisToBHoM);
    }
}
