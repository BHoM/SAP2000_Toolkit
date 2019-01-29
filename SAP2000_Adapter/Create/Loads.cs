using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
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
