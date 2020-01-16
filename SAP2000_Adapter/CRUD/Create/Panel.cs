﻿using BH.Engine.Geometry;
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

            string name = "";

            if (m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, "Default", bhPanel.Name.ToString()) == 0)
            {
                if (name != bhPanel.Name & bhPanel.Name != "")
                    Engine.Reflection.Compute.RecordNote($"Panel {bhPanel.Name} was assigned {AdapterIdName} of {name}");
                bhPanel.CustomData[AdapterIdName] = name;

                string propName = bhPanel.Property.CustomData[AdapterIdName].ToString();
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