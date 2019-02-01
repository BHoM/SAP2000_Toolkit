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
        private List<Bar> ReadBars(List<string> ids = null)
        {

            List<Bar> bhomBars = new List<Bar>();
            IEnumerable<Node> bhomNodesList = ReadNodes();
            Dictionary<string, Node> bhomNodes = bhomNodesList.ToDictionary(x => x.CustomData[AdapterId].ToString());
            Dictionary<string, ISectionProperty> bhomSections = ReadSectionProperties().ToDictionary(x => x.Name.ToString());

            int nameCount = 0;
            string[] names = { };
            m_model.FrameObj.GetNameList(ref nameCount, ref names);

            if (ids == null)
            {
                ids = names.ToList();
            }
            
            foreach (string id in ids)
            {
                try
                {
                    Bar bhomBar = new Bar();
                    bhomBar.CustomData.Add(AdapterId, id);
                    string startId = "";
                    string endId = "";
                    m_model.FrameObj.GetPoints(id, ref startId, ref endId);
                    
                    bhomBar.StartNode = bhomNodes[startId];
                    bhomBar.EndNode = bhomNodes[endId];

                    bool[] restraintStart = new bool[6];
                    double[] springStart = new double[6];
                    bool[] restraintEnd = new bool[6];
                    double[] springEnd = new double[6];

                    m_model.FrameObj.GetReleases(id, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd);
                    bhomBar.Release = new BarRelease();
                    bhomBar.Release.StartRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintStart, springStart);
                    bhomBar.Release.EndRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintEnd, springEnd);
                    
                    string propertyName = "";
                    string sAuto = ""; //This is the name of the auto select list assigned to the frame object, if any.
                    m_model.FrameObj.GetSection(id, ref propertyName, ref sAuto);
                    bhomBar.SectionProperty = bhomSections[propertyName];
                    
                    bhomBars.Add(bhomBar);
                }
                catch
                {
                    ReadElementError("Bar", id.ToString());
                }
            }
            return bhomBars;
        }
    }
}
