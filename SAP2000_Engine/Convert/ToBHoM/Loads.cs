using BH.oM.Structure.Loads;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static LoadAxis LoadAxisToBHoM(this string cSys)
        {
            LoadAxis axis = new LoadAxis();

            switch (cSys)
            {
                case "Global":
                    axis = LoadAxis.Global;
                    break;
                case "Local":
                    axis = LoadAxis.Local;
                    break;
                default:
                    axis = LoadAxis.Global;
                    break;
            }

            return axis;
        }

        /***************************************************/
    }
}
