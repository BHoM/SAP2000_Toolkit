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

        public static List<RigidLink> SplitRigidLink(RigidLink link)
        {
            List<RigidLink> links = null;

            if (link.SlaveNodes.Count() <= 1)
            {
                links.Add(link);
            }
            else
            {
                int i = 0;
                foreach (Node slave in link.SlaveNodes)
                {
                    RigidLink newLink = BH.Engine.Structure.Create.RigidLink(link.MasterNode, new List<Node> { slave }, link.Constraint);
                    newLink.Name = link.Name + ":::" + i;
                    i++;
                }
            }

            return links;
        }

        /***************************************************/
    }
}
