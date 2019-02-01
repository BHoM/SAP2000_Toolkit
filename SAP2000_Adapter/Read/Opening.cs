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
        private List<Opening> ReadOpening(List<string> ids = null)
        {
            List<Opening> bhomOpenings = new List<Opening>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterId].ToString());

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
                if (!isOpening)
                    continue;
                
                Opening bhomOpening = null;

                int pointCount = 0;
                string[] pointNames = null;
                m_model.AreaObj.GetPoints(id, ref pointCount, ref pointNames);

                List<Point> pts = new List<Point>();
                foreach (string name in pointNames)
                    pts.Add(bhomNodes[name].Position);

                pts.Add(pts[0]);
                Polyline pl = new Polyline() { ControlPoints = pts };
                ICurve outline = (ICurve)pl;

                bhomOpening = BH.Engine.Structure.Create.Opening(outline);
                
                bhomOpening.CustomData[AdapterId] = id;
                bhomOpening.Name = id;

                bhomOpenings.Add(bhomOpening);
            }

            return bhomOpenings;
        }
    }
}
