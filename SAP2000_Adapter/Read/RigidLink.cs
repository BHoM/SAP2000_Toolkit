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
        private List<LinkConstraint> ReadLinkConstraints(List<string> ids = null)
        {
            List<LinkConstraint> propList = new List<LinkConstraint>();
            int nameCount = 0;
            string[] names = { };

            if (ids == null)
            {
                m_model.PropLink.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            foreach (string id in ids)
            {
                eLinkPropType linkType = eLinkPropType.Linear;
                m_model.PropLink.GetTypeOAPI(id, ref linkType);
                LinkConstraint constr = Helper.LinkConstraint(id, linkType, m_model);
                if (constr != null)
                    propList.Add(constr);
                else
                    Engine.Reflection.Compute.RecordError("Failed to read link constraint with id :" + id);

            }
            return propList;
        }

        private List<RigidLink> ReadRigidLink(List<string> ids = null)
        {
            List<RigidLink> linkList = new List<RigidLink>();

            int nameCount = 0;
            string[] names = { };

            if (ids == null)
            {
                m_model.LinkObj.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            //read master-multiSlave nodes if these were initially created from (non-etabs)BHoM side
            Dictionary<string, List<string>> idDict = new Dictionary<string, List<string>>();
            string[] masterSlaveId;

            foreach (string id in ids)
            {
                masterSlaveId = id.Split(new[] { ":::" }, StringSplitOptions.None);
                if (masterSlaveId.Count() > 1)
                {
                    //has multi slaves
                    if (idDict.ContainsKey(masterSlaveId[0]))
                        idDict[masterSlaveId[0]].Add(masterSlaveId[1]);
                    else
                        idDict.Add(masterSlaveId[0], new List<string>() { masterSlaveId[1] });
                }
                else
                {
                    //normal single link
                    idDict.Add(id, null);
                }
            }


            foreach (KeyValuePair<string, List<string>> kvp in idDict)
            {
                RigidLink bhLink = new RigidLink();

                if (kvp.Value == null)
                {
                    bhLink.CustomData.Add(AdapterId, kvp.Key);
                    string startId = "";
                    string endId = "";
                    m_model.LinkObj.GetPoints(kvp.Key, ref startId, ref endId);

                    List<Node> endNodes = ReadNodes(new List<string> { startId, endId });
                    bhLink.MasterNode = endNodes[0];
                    bhLink.SlaveNodes = new List<Node>() { endNodes[1] };

                    linkList.Add(bhLink);
                }
                else
                {

                    bhLink.CustomData.Add(AdapterId, kvp.Key);
                    string startId = "";
                    string endId = "";
                    string multiLinkId = kvp.Key + ":::0";
                    List<string> nodeIdsToRead = new List<string>();

                    m_model.LinkObj.GetPoints(multiLinkId, ref startId, ref endId);
                    nodeIdsToRead.Add(startId);

                    for (int i = 1; i < kvp.Value.Count(); i++)
                    {
                        multiLinkId = kvp.Key + ":::" + i;
                        m_model.LinkObj.GetPoints(multiLinkId, ref startId, ref endId);
                        nodeIdsToRead.Add(endId);
                    }

                    List<Node> endNodes = ReadNodes(nodeIdsToRead);
                    bhLink.MasterNode = endNodes[0];
                    endNodes.RemoveAt(0);
                    bhLink.SlaveNodes = endNodes;

                    linkList.Add(bhLink);
                }
            }

            return linkList;
        }
    }
}
