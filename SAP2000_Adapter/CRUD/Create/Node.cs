using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Node bhNode)
        {

            string name = "";

            if (m_model.PointObj.AddCartesian(bhNode.Position.X, bhNode.Position.Y, bhNode.Position.Z, ref name, bhNode.Name.ToString()) == 0)
            {
                if (name != bhNode.Name)
                    Engine.Reflection.Compute.RecordNote($"Node {bhNode.Name} was assigned {AdapterIdName} of {name}");
                bhNode.CustomData[AdapterIdName] = name;

                if (bhNode.Support != null)
                {
                    bool[] restraint = new bool[6];
                    double[] spring = new double[6];

                    bhNode.GetSAPConstraint(ref restraint, ref spring);

                    if (m_model.PointObj.SetRestraint(name, ref restraint) != 0)
                        CreatePropertyWarning("Node Restraint", "Node", name);
                    if (m_model.PointObj.SetSpring(name, ref spring) != 0)
                        CreatePropertyWarning("Node Spring", "Node", name);
                }
            }

            return true;
        }

        /***************************************************/
    }
}
