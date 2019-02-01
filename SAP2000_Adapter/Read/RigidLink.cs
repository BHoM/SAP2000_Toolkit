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
        private List<RigidLink> ReadRigidLink(List<string> ids = null)
        {
            List<RigidLink> linkList = new List<RigidLink>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterId].ToString());

            //Read all links, filter by id at end, so that we can join multi-links.
            int nameCount = 0;
            string[] names = { };
            m_model.LinkObj.GetNameList(ref nameCount, ref names);

            foreach (string name in names)
            {
                string masterId = "";
                string SlaveId = "";
                m_model.LinkObj.GetPoints(name, ref masterId, ref SlaveId);
                RigidLink newLink = BH.Engine.Structure.Create.RigidLink(bhomNodes[masterId], new List<Node> { bhomNodes[SlaveId] });

                linkList.Add(newLink);
            }

            List<RigidLink> joinedList = BH.Engine.SAP2000.Convert.JoinRigidLink(linkList);

            List<RigidLink> filteredList = joinedList.Where(x => ids.Contains(x.Name)).ToList();

            return filteredList;
        }
    }
}
