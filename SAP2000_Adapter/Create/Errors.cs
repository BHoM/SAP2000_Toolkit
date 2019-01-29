using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private void CreateElementError(string elemType, string elemName)
        {
            Engine.Reflection.Compute.RecordError("Failed to create the element of type " + elemType + ", with id: " + elemName);
        }

        /***************************************************/

        private void CreatePropertyError(string failedProperty, string elemType, string elemName)
        {
            CreatePropertyEvent(failedProperty, elemType, elemName, oM.Reflection.Debugging.EventType.Error);
        }

        /***************************************************/

        private void CreatePropertyWarning(string failedProperty, string elemType, string elemName)
        {
            CreatePropertyEvent(failedProperty, elemType, elemName, oM.Reflection.Debugging.EventType.Warning);
        }

        /***************************************************/

        private void CreatePropertyEvent(string failedProperty, string elemType, string elemName, oM.Reflection.Debugging.EventType eventType)
        {
            Engine.Reflection.Compute.RecordEvent("Failed to set property " + failedProperty + " for the " + elemType + "with id: " + elemName, eventType);
        }

        /***************************************************/
    }
}
