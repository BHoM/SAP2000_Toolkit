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
            List<Bar> barList = new List<Bar>();
            int nameCount = 0;
            string[] names = { };

            if (ids == null)
            {
                m_model.FrameObj.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            //Storing the sectionproperties as they are being pulled out, to only pull each section once.
            Dictionary<string, ISectionProperty> sectionProperties = new Dictionary<string, ISectionProperty>();

            foreach (string id in ids)
            {
                try
                {
                    Bar bhBar = new Bar();
                    bhBar.CustomData.Add(AdapterId, id);
                    string startId = "";
                    string endId = "";
                    m_model.FrameObj.GetPoints(id, ref startId, ref endId);

                    List<Node> endNodes = ReadNodes(new List<string> { startId, endId });
                    bhBar.StartNode = endNodes[0];
                    bhBar.EndNode = endNodes[1];

                    bool[] restraintStart = new bool[6];
                    double[] springStart = new double[6];
                    bool[] restraintEnd = new bool[6];
                    double[] springEnd = new double[6];

                    m_model.FrameObj.GetReleases(id, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd);
                    bhBar.Release = new BarRelease();
                    bhBar.Release.StartRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintStart, springStart);
                    bhBar.Release.EndRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintEnd, springEnd);

                    eFramePropType propertyType = eFramePropType.General;
                    string propertyName = "";
                    string sAuto = "";
                    m_model.FrameObj.GetSection(id, ref propertyName, ref sAuto);
                    if (propertyName != "None")
                    {
                        ISectionProperty property;

                        //Check if section already has been pulled once
                        if (!sectionProperties.TryGetValue(propertyName, out property))
                        {
                            //if not pull it and store it
                            m_model.PropFrame.GetTypeOAPI(propertyName, ref propertyType);
                            property = Helper.GetSectionProperty(m_model, propertyName, propertyType);
                            sectionProperties[propertyName] = property;
                        }

                        bhBar.SectionProperty = property;
                    }

                    barList.Add(bhBar);
                }
                catch
                {
                    ReadElementError("Bar", id.ToString());
                }
            }
            return barList;
        }
    }
}
