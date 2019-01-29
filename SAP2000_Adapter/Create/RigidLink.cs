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

            string name = "";
            string givenName = "";
            string bhId = bhLink.CustomData[AdapterId].ToString();
            name = bhId;

            LinkConstraint constraint = bhLink.Constraint;//not used yet
            Node masterNode = bhLink.MasterNode;
            List<Node> slaveNodes = bhLink.SlaveNodes;
            bool multiSlave = slaveNodes.Count() == 1 ? false : true;

            for (int i = 0; i < slaveNodes.Count(); i++)
            {
                name = multiSlave == true ? name + ":::" + i : name;
                ret = m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterId].ToString(), slaveNodes[i].CustomData[AdapterId].ToString(), ref givenName, false, "Default", name);
            }

            return ret == 0;
        }
    }
}
