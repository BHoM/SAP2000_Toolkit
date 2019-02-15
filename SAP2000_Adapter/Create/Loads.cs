using BH.oM.Structure.Loads;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Loadcase loadcase)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", loadcase.Name);
            return false;
            //bool success = true;

            //Helper.SetLoadcase(model, loadcase);

            //return success;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", loadcombination.Name);
            return false;
            //bool success = true;

            //Helper.SetLoadCombination(model, loadcombination);

            //return success;
        }

        /***************************************************/

        private bool CreateObject(ILoad bhLoad)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", bhLoad.Name);
            return false;
            //bool success = true;

            //Helper.SetLoad(model, bhLoad as dynamic);


            //return success;
        }
    }
}
