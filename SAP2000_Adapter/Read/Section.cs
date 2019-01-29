using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Loads;
using BH.oM.Common.Materials;
using SAP2000v19;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Reflection;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> propList = new List<ISectionProperty>();
            int nameCount = 0;
            string[] names = { };

            if (ids == null)
            {
                m_model.PropFrame.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            eFramePropType propertyType = eFramePropType.General;

            foreach (string id in ids)
            {
                m_model.PropFrame.GetTypeOAPI(id, ref propertyType);
                propList.Add(Helper.GetSectionProperty(m_model, id, propertyType));
            }
            return propList;
        }
    }
}
