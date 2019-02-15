using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Constraint;

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
            name = bhId;

            ret = m_model.FrameObj.AddByPoint(bhBar.StartNode.CustomData[AdapterId].ToString(), bhBar.EndNode.CustomData[AdapterId].ToString(), ref name);

            if (ret != 0)
            {
                CreateElementError("Bar", name);
                return false;
            }

            if (m_model.FrameObj.SetSection(name, bhBar.SectionProperty.Name) != 0)
            {
                CreatePropertyWarning("SectionProperty", "Bar", name);
                ret++;
            }

            if (m_model.FrameObj.SetLocalAxes(name, bhBar.OrientationAngle * 180 / System.Math.PI) != 0)
            {
                CreatePropertyWarning("Orientation angle", "Bar", name);
                ret++;
            }

            Offset offset = bhBar.Offset;

            double[] offset1 = new double[3];
            double[] offset2 = new double[3];

            if (offset != null)
            {
                offset1[1] = offset.Start.Z;
                offset1[2] = offset.Start.Y;
                offset2[1] = offset.End.Z;
                offset2[2] = offset.End.Y;
            }

            BarRelease barRelease = bhBar.Release;
            if (barRelease != null)
            {
                bool[] restraintStart = barRelease.StartRelease.Fixities();// Helper.GetRestraint6DOF(barRelease.StartRelease);
                double[] springStart = barRelease.StartRelease.ElasticValues();// Helper.GetSprings6DOF(barRelease.StartRelease);
                bool[] restraintEnd = barRelease.EndRelease.Fixities();// Helper.GetRestraint6DOF(barRelease.EndRelease);
                double[] springEnd = barRelease.EndRelease.ElasticValues();// Helper.GetSprings6DOF(barRelease.EndRelease);

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
