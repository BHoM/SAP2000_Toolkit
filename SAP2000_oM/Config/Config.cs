using BH.oM.Adapter;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000
{
    [Description("This Config can be specified in the `ActionConfig` input of any Adapter Action (e.g. Push).")]
    // Note: this will get passed within any CRUD method (see their signature). 
    // In order to access its properties, you will need to cast it to `SAP2000ActionConfig`.
    public class SAP2000ActionConfig : ActionConfig
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Sets whether the loads being pushed should overwrite existing loads on the same object within the same loadcase")]
        public bool ReplaceLoads { get; set; } = false;

        /***************************************************/
    }
}