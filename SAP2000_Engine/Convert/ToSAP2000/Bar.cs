using BH.oM.Structure.Loads;
using SAP2000v19;

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
