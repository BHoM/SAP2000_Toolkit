using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.Constraints;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Bar bhBar)
        {
            int ret = 0;

            string name = "";
            string bhId = bhBar.CustomData[AdapterId].ToString();

            ret = m_model.FrameObj.AddByPoint(bhBar.StartNode.CustomData[AdapterId].ToString(), bhBar.EndNode.CustomData[AdapterId].ToString(), ref name);

            if (ret != 0)
            {
                CreateElementError("Bar", name);
                return false;
            }

            string barProp = bhBar.SectionProperty != null ? bhBar.SectionProperty.Name : "None";

            if (m_model.FrameObj.SetSection(name, barProp) != 0)
            {
                CreatePropertyWarning("SectionProperty", "Bar", name);
                ret++;
            }

            if (bhBar.OrientationAngle != 0)
            {
                if (m_model.FrameObj.SetLocalAxes(name, bhBar.OrientationAngle * 180 / System.Math.PI) != 0)
                {
                    CreatePropertyWarning("Orientation angle", "Bar", name);
                    ret++;
                }
            }
           
            if (bhBar.Release != null)
            {
                bool[] restraintStart = null;
                double[] springStart = null;
                bool[] restraintEnd = null;
                double[] springEnd = null;

                bhBar.GetSAPBarRelease(ref restraintStart, ref springStart, ref restraintEnd, ref springEnd);

                if (m_model.FrameObj.SetReleases(name, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd) != 0)
                {
                    CreatePropertyWarning("Release", "Bar", name);
                    ret++;
                }
            }

            else if (bhBar.Offset != null)
            {
                if (m_model.FrameObj.SetEndLengthOffset(name, false, -1 * (bhBar.Offset.Start.X), bhBar.Offset.End.X, 1) != 0)
                {
                    CreatePropertyWarning("Length offset", "Bar", name);
                    ret++;
                }
            }

            return ret == 0;
        }

        /***************************************************/
    }
}
