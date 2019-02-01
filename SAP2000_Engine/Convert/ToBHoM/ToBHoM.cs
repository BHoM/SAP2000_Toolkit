using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> JoinRigidLink(List<RigidLink> linkList)
        {
            List<RigidLink> joinedList = null;

            Dictionary<string, Node> masterDict = null;
            Dictionary<string, List<Node>> slaveDict = null;

            foreach (RigidLink link in linkList)
            {
                string[] nameParts = link.Name.Split(new[] { ":::" }, StringSplitOptions.None);
                if (nameParts.Count() == 1)
                    joinedList.Add(link);
                else
                {
                    string name = nameParts[0];
                    if (masterDict.ContainsKey(name))
                    {
                        slaveDict[name].Add(link.SlaveNodes[0]);
                    }
                    else
                    {
                        masterDict.Add(name, link.MasterNode);
                        slaveDict.Add(name, new List<Node> { link.SlaveNodes[0] });
                    }
                }
            }

            foreach (KeyValuePair<string, Node> kvp in masterDict)
            {
                RigidLink newLink = BH.Engine.Structure.Create.RigidLink(kvp.Value, slaveDict[kvp.Key]);
                newLink.Name = kvp.Key;
            }           

            return joinedList;
        }

        /***************************************************/


    }
}
