using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

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

                //Assuming all constraints are fixed constraints
                LinkConstraint constraint = Engine.Structure.Create.LinkConstraintFixed();
                Engine.Reflection.Compute.RecordWarning("All Rigid Link constraints are being read as fully fixed. Check results carefully.");

                RigidLink newLink = BH.Engine.Structure.Create.RigidLink(bhomNodes[masterId], new List<Node> { bhomNodes[SlaveId] }, constraint);

                linkList.Add(newLink);
            }

            List<RigidLink> joinedList = BH.Engine.SAP2000.Modify.JoinRigidLink(linkList);

            List<RigidLink> filteredList = joinedList.Where(x => ids.Contains(x.Name)).ToList();

            return filteredList;
        }

        /***************************************************/
    }
}
