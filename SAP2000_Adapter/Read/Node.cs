using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Node> ReadNodes(List<string> ids = null)
        {
            List<Node> nodeList = new List<Node>();

            int nameCount = 0;
            string[] nameArr = { };

            if (ids == null)
            {
                if (m_model.PointObj.GetNameList(ref nameCount, ref nameArr) == 0)
                {
                    ids = nameArr.ToList();
                }
            }

            foreach (string id in ids)
            {
                Node bhNode = new Node();
                double x, y, z;
                x = y = z = 0;
                bool[] restraint = new bool[6];
                double[] spring = new double[6];

                m_model.PointObj.GetCoordCartesian(id, ref x, ref y, ref z);
                bhNode.Position = Engine.Geometry.Create.Point(x, y, z);
                bhNode.CustomData.Add(AdapterId, id);

                m_model.PointObj.GetRestraint(id, ref restraint);
                m_model.PointObj.SetSpring(id, ref spring);
                bhNode.Support = restraint.GetConstraint6DOF(spring);


                nodeList.Add(bhNode);
            }


            return nodeList;
        }

        /***************************************************/
    }
}
