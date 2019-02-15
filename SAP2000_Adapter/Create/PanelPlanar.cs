using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(PanelPlanar bhPanel)
        {
            int ret = 0;

            List<BH.oM.Geometry.Point> boundaryPoints = bhPanel.ControlPoints();

            int segmentCount = boundaryPoints.Count() - 1;

            double[] x = new double[segmentCount];
            double[] y = new double[segmentCount];
            double[] z = new double[segmentCount];

            for (int j = 0; j < segmentCount; j++)
            {
                x[j] = boundaryPoints[j].X;
                y[j] = boundaryPoints[j].Y;
                z[j] = boundaryPoints[j].Z;
            }
            string name = "";
            ret += m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, bhPanel.Property.Name); 

            return ret == 0;
        }
    }
}
