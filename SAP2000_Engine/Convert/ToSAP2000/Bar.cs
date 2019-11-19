using BH.oM.Geometry;
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

        public static Vector BarLocalAxisToCSI(this Vector axisBHoM)
        {
            return Geometry.Modify.Transform(axisBHoM, barLocalAxisToCSI);
        }

        /***************************************************/

        public static TransformMatrix barLocalAxisToCSI = Geometry.Modify.Transpose(barLocalAxisToBHoM);
    }
}
