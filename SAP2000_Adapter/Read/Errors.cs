namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void ReadElementError(string elemType, string elemName)
        {
            Engine.Reflection.Compute.RecordError("Failed to read the element of type " + elemType + ", with id: " + elemName);
        }

        /***************************************************/

        private void ReadPropertyError(string propType, string propName)
        {
            Engine.Reflection.Compute.RecordError("Failed to read property of type " + propType + ", with id: " + propName);
        }

        /***************************************************/
    }
}
