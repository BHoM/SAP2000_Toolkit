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
            List<string> linkIds = null;

            foreach (RigidLink link in bhomLinks)
            {
                string name = "";
                Node masterNode = link.MasterNode;
                Node slaveNode = link.SlaveNodes[0];

                ret = m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterId].ToString(), 
                    slaveNode.CustomData[AdapterId].ToString(), ref name, false, "Default");
                
                linkIds.Add(name);
            }

            bhLink.CustomData[AdapterId] = linkIds;

            return ret == 0;
        }

        /***************************************************/
    }
}
