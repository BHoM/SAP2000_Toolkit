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
        private bool CreateObject(RigidLink bhLink)
        {
            int ret = 0;
            List < RigidLink> bhomLinks = BH.Engine.SAP2000.Convert.SplitRigidLink(bhLink);

            foreach (RigidLink link in bhomLinks)
            {
                string givenName = "";
                Node masterNode = link.MasterNode;
                Node slaveNode = link.SlaveNodes[0];
                string bhId = link.CustomData[AdapterId].ToString();

                ret = m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterId].ToString(), 
                    slaveNode.CustomData[AdapterId].ToString(), ref givenName, false, "Default", bhId);
            }

            return ret == 0;
        }

        
    }
}
