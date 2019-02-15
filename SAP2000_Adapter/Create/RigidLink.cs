using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(RigidLink bhLink)
        {
            int ret = 0;
            List < RigidLink> bhomLinks = BH.Engine.SAP2000.Modify.SplitRigidLink(bhLink);

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
