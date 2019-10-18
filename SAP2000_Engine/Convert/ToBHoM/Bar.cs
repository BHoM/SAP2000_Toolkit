﻿using BH.oM.Structure.Loads;

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

        public static oM.Geometry.Vector BarLocalAxisToBHoM(this oM.Geometry.Vector axisCSI)
        {
            return Engine.Geometry.Modify.Transform(axisCSI, barLocalAxisToBHoM);
        }

        /***************************************************/

        public static oM.Geometry.TransformMatrix barLocalAxisToBHoM = new oM.Geometry.TransformMatrix()
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
