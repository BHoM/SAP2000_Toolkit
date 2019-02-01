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
            
            if (ids == null)
            {
                int nameCount = 0;
                string[] nameArr = { };
                m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }
            
            foreach (string id in ids)
            {
                //Get outline of panel
                string[] pointNames = null;
                int pointCount = 0;
                m_model.AreaObj.GetPoints(id, ref pointCount, ref pointNames);

                List<Point> pts = new List<Point>();
                foreach (string name in pointNames)
                    pts.Add(bhomNodes[name].Position);
                pts.Add(pts[0]);
                Polyline outline = new Polyline() { ControlPoints = pts };

                //Get the section property
                string propertyName = "";
                m_model.AreaObj.GetProperty(id, ref propertyName);
                List<Opening> noOpenings = null;

                //Create the panel
                PanelPlanar bhomPanel = BH.Engine.Structure.Create.PanelPlanar(outline, noOpenings, bhomProperties[propertyName], id);
                
                //Set the properties
                bhomPanel.CustomData[AdapterId] = id;
                
                //Add the panel to the list
                bhomPanels.Add(bhomPanel);
            }

            return bhomPanels;
        }
        
    }
}
