using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Bar> ReadBars(List<string> ids = null)
        {

            List<Bar> bhomBars = new List<Bar>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterIdName].ToString());
            Dictionary<string, ISectionProperty> bhomSections = ReadSectionProperties().ToDictionary(x => x.CustomData[AdapterIdName].ToString());

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
                    bhomBar.CustomData[AdapterIdName] = id;
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
                    bhomBar.Release = Engine.SAP2000.Convert.GetBarRelease(restraintStart, springStart, restraintEnd, springEnd);

                    //bhomBar.Release.StartRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintStart, springStart);
                    //bhomBar.Release.EndRelease = Engine.SAP2000.Convert.GetConstraint6DOF(restraintEnd, springEnd);
                    
                    string propertyName = "";
                    string sAuto = ""; //This is the name of the auto select list assigned to the frame object, if any.
                    m_model.FrameObj.GetSection(id, ref propertyName, ref sAuto);

                    if (propertyName != "None")
                    {
                        bhomBar.SectionProperty = bhomSections[propertyName];
                    }

                    double angle = 0;
                    bool advanced = false;

                    if (m_model.FrameObj.GetLocalAxes(id, ref angle, ref advanced) == 0)
                    {
                        if (advanced)
                        {
                            Engine.Reflection.Compute.RecordWarning("Advanced local axes are not yet supported by this toolkit. Bar " + id + " has been created with orientation angle = 0");
                            angle = 0;
                        }
                        bhomBar.OrientationAngle = angle * System.Math.PI / 180;
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordWarning("Could not get local axes for bar " + id + ". Orientation angle is 0 by default");
                    }

                    
                    bhomBars.Add(bhomBar);
                }
                catch
                {
                    ReadElementError("Bar", id.ToString());
                }
            }
            return bhomBars;
        }

        /***************************************************/
    }
}
