using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000
{
    public class SAP2000Config : BHoMObject
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Sets whether the loads being pushed should overwrite existing loads on the same object within the same loadcase")]
        public bool ReplaceLoads { get; set; } = false;

        /***************************************************/
    }
}