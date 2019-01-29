using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Loads;
using BH.oM.Common.Materials;
using SAP2000v19;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private List<PanelPlanar> ReadPanel(List<string> ids = null)
        {
            List<PanelPlanar> panelList = new List<PanelPlanar>();
            int nameCount = 0;
            string[] nameArr = { };

            if (ids == null)
            {
                m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }

            //get openings, if any
            m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);
            bool isOpening = false;
            Dictionary<string, Polyline> openingDict = new Dictionary<string, Polyline>();
            foreach (string name in nameArr)
            {
                m_model.AreaObj.GetOpening(name, ref isOpening);
                if (isOpening)
                {
                    openingDict.Add(name, Helper.GetPanelPerimeter(m_model, name));
                }
            }

            foreach (string id in ids)
            {
                if (openingDict.ContainsKey(id))
                    continue;

                string propertyName = "";

                m_model.AreaObj.GetProperty(id, ref propertyName);
                ISurfaceProperty panelProperty = ReadSurfaceProperty(new List<string>() { propertyName })[0];

                PanelPlanar panel = new PanelPlanar();
                panel.CustomData[AdapterId] = id;

                Polyline pl = Helper.GetPanelPerimeter(m_model, id);

                Edge edge = new Edge();
                edge.Curve = pl;
                //edge.Constraint = new Constraint4DOF();// <---- cannot see anyway to set this via API and for some reason constraints are not being set in old version of etabs toolkit TODO

                panel.ExternalEdges = new List<Edge>() { edge };
                foreach (KeyValuePair<string, Polyline> kvp in openingDict)
                {
                    if (pl.IsContaining(kvp.Value.ControlPoints))
                    {
                        Opening opening = new Opening();
                        opening.Edges = new List<Edge>() { new Edge() { Curve = kvp.Value } };
                        panel.Openings.Add(opening);
                    }
                }

                panel.Property = panelProperty;

                panelList.Add(panel);
            }

            return panelList;
        }
    }
}
