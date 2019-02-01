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
            List<PanelPlanar> bhomPanels = new List<PanelPlanar>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, ISurfaceProperty> bhomProperties = ReadSurfaceProperty().ToDictionary(x => x.Name);
            List<Opening> allOpenings = ReadOpening();

            int nameCount = 0;
            string[] nameArr = { };
            m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);

            if (ids == null)
            {
                ids = nameArr.ToList();
            }
            
            foreach (string id in ids)
            {
                bool isOpening = false;
                m_model.AreaObj.GetOpening(id, ref isOpening);
                if (isOpening)
                    continue;

                PanelPlanar bhomPanel = null;

                //Get outline of panel
                string[] pointNames = null;
                int pointCount = 0;
                m_model.AreaObj.GetPoints(id, ref pointCount, ref pointNames);

                List<Point> pts = new List<Point>();
                foreach (string name in pointNames)
                    pts.Add(bhomNodes[name].Position);

                pts.Add(pts[0]);
                Polyline pl = new Polyline() { ControlPoints = pts };
                ICurve outline = (ICurve)pl;

                //Make a list of any openings which are in this panel
                List<Opening> openings = null;
                foreach (Opening opening in allOpenings)
                {
                    List<Point> openPoints = null;
                    foreach (Edge edge in opening.Edges)
                        openPoints.Add(edge.Curve.IStartPoint());
                    
                    if (pl.IsContaining(openPoints))
                    {
                        openings.Add(opening);
                    }
                }

                //Try to build the panel
                try
                {
                    bhomPanel = BH.Engine.Structure.Create.PanelPlanar(outline, openings);
                }
                catch
                {
                    ReadElementError("PanelPlanar", id);
                }                    

                //Set the properties
                string propertyName = "";
                m_model.AreaObj.GetProperty(id, ref propertyName);
                bhomPanel.Property = bhomProperties[propertyName];
                bhomPanel.CustomData[AdapterId] = id;
                
                //Add the panel to the list
                bhomPanels.Add(bhomPanel);
            }

            return bhomPanels;
        }
        
    }
}
