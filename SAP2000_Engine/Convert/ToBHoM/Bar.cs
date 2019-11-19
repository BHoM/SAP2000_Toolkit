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

        public static Vector BarLocalAxisToBHoM(this Vector axisCSI)
        {
            return Geometry.Modify.Transform(axisCSI, barLocalAxisToBHoM);
        }

        /***************************************************/

        public static TransformMatrix barLocalAxisToBHoM = new TransformMatrix()
        {
            Matrix = new double[4, 4]
            {
                { 1,  0,  0, 0 },
                { 0,  0, -1, 0 },
                { 0,  1,  0, 0 },
                { 0,  0,  0, 1 }
            }
        };
    }
}
