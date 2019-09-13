using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Panel bhPanel)
        {                        
            List<Point> boundaryPoints = bhPanel.ExternalEdgeCurves().Select( item => item.IStartPoint()).ToList();

            int segmentCount = boundaryPoints.Count();

            double[] x = boundaryPoints.Select(item => item.X).ToArray();
            double[] y = boundaryPoints.Select(item => item.Y).ToArray();
            double[] z = boundaryPoints.Select(item => item.Z).ToArray();

            string name = bhPanel.Name.ToString();

            if (m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, "Default", name) == 0)
            {
                if (name != bhPanel.Name)
                    Engine.Reflection.Compute.RecordNote($"Panel {bhPanel.Name} was assigned {AdapterId} of {name}");
                bhPanel.CustomData[AdapterId] = name;
                if (m_model.AreaObj.SetProperty(name, bhPanel.CustomData[AdapterId].ToString()) != 0)
                    CreatePropertyError(bhPanel.CustomData[AdapterId].ToString(), "Panel", bhPanel.Name);
            }
            else
                CreateElementError("Panel", bhPanel.Name);

            return true;
        }

        /***************************************************/
    }
}
