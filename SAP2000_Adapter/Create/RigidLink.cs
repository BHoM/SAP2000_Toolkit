using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(RigidLink bhLink)
        {
            List < RigidLink> bhomLinks = BH.Engine.SAP2000.Modify.SplitRigidLink(bhLink);
            List<string> linkIds = null;

            foreach (RigidLink link in bhomLinks)
            {
                string name = "";
                Node masterNode = link.MasterNode;
                Node slaveNode = link.SlaveNodes[0];

                if ( m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterId].ToString(), 
                    slaveNode.CustomData[AdapterId].ToString(), ref name, false, "Default") != 0)
                {
                    CreateElementError("RigidLink", name);
                }
                
                linkIds.Add(name);
            }

            bhLink.CustomData[AdapterId] = linkIds;

            return true;
        }

        /***************************************************/
    }
}
