using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
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
                if (name != bhPanel.Name & bhPanel.Name != "")
                    Engine.Reflection.Compute.RecordNote($"Panel {bhPanel.Name} was assigned {AdapterId} of {name}");
                bhPanel.CustomData[AdapterId] = name;

                string propName = bhPanel.Property.CustomData[AdapterId].ToString();
                if (m_model.AreaObj.SetProperty(name, propName, 0) != 0)
                    CreatePropertyError("Surface Property", "Panel", name);
            }
            else
                CreateElementError("Panel", bhPanel.Name);

            return true;
        }

        /***************************************************/
    }
}
