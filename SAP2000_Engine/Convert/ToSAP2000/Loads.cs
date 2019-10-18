using BH.oM.Structure.Loads;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToCSI(this LoadAxis axis)
        {
            string cSys = "";

            switch (axis)
            {
                case LoadAxis.Global:
                    cSys = "Global";
                    break;
                case LoadAxis.Local:
                    cSys = "Local";
                    break;
                default:
                    cSys = "Global";
                    break;
            }

            return cSys;
        }

        /***************************************************/
    }
}
