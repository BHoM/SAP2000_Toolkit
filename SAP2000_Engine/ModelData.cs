using BH.oM.Common.Materials;
using BH.oM.Structure.Properties.Section;
using SAP2000v19;
using System.Collections.Generic;

namespace BH.Engine.SAP2000
{
    public class ModelData
    {
        public Dictionary<string, ISectionProperty> sectionDict;
        public Dictionary<string, Material> materialDict;
        public cSapModel model;

        public ModelData(cSapModel data)
        {
            sectionDict = new Dictionary<string, ISectionProperty>();
            materialDict = new Dictionary<string, Material>();
            model = data;
        }

        /***************************************************/

    }
}
