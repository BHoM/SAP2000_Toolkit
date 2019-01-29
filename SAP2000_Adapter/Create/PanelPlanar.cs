using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private bool CreateObject(PanelPlanar bhPanel)
        {
            bool success = true;
            int retA = 0;

            double mergeTol = 1e-3;

            string name = bhPanel.CustomData[AdapterId].ToString();
            string propertyName = bhPanel.Property.Name;
            List<BH.oM.Geometry.Point> boundaryPoints = bhPanel.ControlPoints(true).CullDuplicates(mergeTol);

            int segmentCount = boundaryPoints.Count();
            double[] x = new double[segmentCount];
            double[] y = new double[segmentCount];
            double[] z = new double[segmentCount];
            for (int i = 0; i < segmentCount; i++)
            {
                x[i] = boundaryPoints[i].X;
                y[i] = boundaryPoints[i].Y;
                z[i] = boundaryPoints[i].Z;
            }

            retA = m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, propertyName);

            if (retA != 0)
                return false;

            if (bhPanel.Openings != null)
            {
                for (int i = 0; i < bhPanel.Openings.Count; i++)
                {
                    boundaryPoints = bhPanel.Openings[i].ControlPoints().CullDuplicates(mergeTol);

                    segmentCount = boundaryPoints.Count();
                    x = new double[segmentCount];
                    y = new double[segmentCount];
                    z = new double[segmentCount];

                    for (int j = 0; j < segmentCount; j++)
                    {
                        x[j] = boundaryPoints[j].X;
                        y[j] = boundaryPoints[j].Y;
                        z[j] = boundaryPoints[j].Z;
                    }

                    string openingName = name + "_Opening_" + i;
                    m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref openingName, "");//<-- setting panel property to empty string, verify that this is correct
                    m_model.AreaObj.SetOpening(openingName, true);
                }
            }

            return success;
        }
    }
}
