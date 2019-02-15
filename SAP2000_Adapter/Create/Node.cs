using BH.Engine.Structure;
using BH.oM.Structure.Elements;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Node bhNode)
        {
            int ret = 0;

            string name = "";
            string bhId = bhNode.CustomData[AdapterId].ToString();
            name = bhId;

            ret += m_model.PointObj.AddCartesian(bhNode.Position().X, bhNode.Position().Y, bhNode.Position().Z, ref name);

            if (name != bhId)
                bhNode.CustomData[AdapterId] = name;

            if (bhNode.Constraint != null)
            {
                bool[] restraint = new bool[6];
                double[] spring = new double[6];

                Engine.SAP2000.Convert.SetConstraint6DOF(bhNode.Constraint, ref restraint, ref spring);

                ret += m_model.PointObj.SetRestraint(name, ref restraint);
                ret += m_model.PointObj.SetSpring(name, ref spring);
            }

            return ret == 0;
        }
    }
}
