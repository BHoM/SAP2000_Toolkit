using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Structure;
using BH.Engine.SAP2000;

#if Debug21 || Release21
using SAP2000v1;
#else
using SAP2000v19;
#endif

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Loadcase> ReadLoadcase(List<string> ids = null)
        {
            List<Loadcase> loadCases = new List<Loadcase>();

            int count = 0;
            string[] names = null;

            m_model.LoadPatterns.GetNameList(ref count, ref names);

            for (int i = 0; i < count; i++ )
            {
                eLoadPatternType patternType = eLoadPatternType.Dead;

                if (m_model.LoadPatterns.GetLoadType(names[i], ref patternType) != 0)
                {
                    ReadElementError("Load Pattern", names[i]);
                }
                else
                {
                    Loadcase bhomCase = BH.Engine.Structure.Create.Loadcase(names[i], i, patternType.ToBHoM());
                    bhomCase.CustomData[AdapterId] = names[i];
                    loadCases.Add(bhomCase);
                }
            }

            return loadCases;
        }

        /***************************************************/

        private List<LoadCombination> ReadLoadCombination(List<string> ids = null)
        {
            List<LoadCombination> combinations = new List<LoadCombination>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());

            int count = 0;
            string[] names = null;

            m_model.RespCombo.GetNameList(ref count, ref names);
            
            for (int i = 0; i < count; i++)
            {
                double[] factors = null;
                int caseCount = 0;
                eCNameType[] caseTypes = null;
                string[] caseNames = null;

                if (m_model.RespCombo.GetCaseList(names[i], ref caseCount, ref caseTypes, ref caseNames, ref factors) != 0)
                {
                    ReadElementError("Load Combo", names[i]);
                }
                else
                {
                    List<Loadcase> comboCases = new List<Loadcase>();
                    foreach (string caseName in caseNames)
                    {
                        comboCases.Add(bhomCases[caseName]);
                    }
                    LoadCombination bhomCombo = BH.Engine.Structure.Create.LoadCombination(names[i], i, comboCases, factors.ToList());
                    bhomCombo.CustomData[AdapterId] = names[i];
                    combinations.Add(bhomCombo);
                }
            }

            return combinations;
        }

        /***************************************************/

        private List<ILoad> ReadLoad(Type type, List<string> ids = null)
        {

            if (type == typeof(PointLoad))
                return ReadPointLoad();
            else if (type == typeof(BarUniformlyDistributedLoad))
                return ReadBarLoad();
            else if (type == typeof(AreaUniformlyDistributedLoad))
                return ReadAreaLoad();
            else
            {
                List<ILoad> loads = new List<ILoad>();
                loads.AddRange(ReadPointLoad());
                loads.AddRange(ReadBarLoad());
                loads.AddRange(ReadAreaLoad());
                return loads;
            }
        }

        /***************************************************/

        private List<ILoad> ReadPointLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterId].ToString());

            int count = 0;
            string[] nodeNames = null;
            string[] caseNames = null;
            int[] steps = null;
            string[] cSys = null;
            double[] f1 = null;
            double[] f2 = null;
            double[] f3 = null;
            double[] m1 = null;
            double[] m2 = null;
            double[] m3 = null;


            if (m_model.PointObj.GetLoadForce("All", ref count, ref nodeNames, ref caseNames, ref steps, ref cSys, ref f1, ref f2, ref f3, ref m1, ref m2, ref m3, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    loads.Add(new PointLoad()
                    {
                        Force = new Vector() { X = f1[i], Y = f2[i], Z = f3[i] },
                        Moment = new Vector() { X = m1[i], Y = m2[i], Z = m3[i] },
                        Loadcase = bhomCases[caseNames[i]],
                        Axis = cSys[i].LoadAxisToBHoM(),
                        Objects = new BHoMGroup<Node>() { Elements = { bhomNodes[nodeNames[i]] } }
                    });
                }
            }



            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => x.CustomData[AdapterId].ToString());

            int count = 0;
            string[] frameNames = null;
            string[] caseNames = null;
            int[] myTypes = null;
            string[] cSys = null;
            int[] dir = null;
            double[] rd1 = null;
            double[] rd2 = null;
            double[] dist1 = null;
            double[] dist2 = null;
            double[] val1 = null;
            double[] val2 = null;


            if (m_model.FrameObj.GetLoadDistributed("All", ref count, ref frameNames, ref caseNames, ref myTypes, ref cSys, ref dir, ref rd1, ref rd2, ref dist1, ref dist2, ref val1, ref val2, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Bar bhomBar = bhomBars[frameNames[i]];

                    if (dist1[i] != 0 || rd1[i] != 0 || dist2[i] != bhomBar.Length() || rd2[i] != 1)
                    {
                        Engine.Reflection.Compute.RecordWarning("Partial distributed loads are not supported. Smearing load all over the bar like jelly.");
                    }
                    double val = ((val1[i] + val2[i]) / 2) * (dist2[i] - dist1[i]) / bhomBar.Length();
                    Vector force = new Vector();
                    LoadAxis axis = cSys[i].LoadAxisToBHoM();

                    switch (dir[i])
                    {
                        case 1:
                            force.X = val;
                            break;
                        case 2:
                            force.Z = val;
                            break;
                        case 3:
                            force.Y = -val;
                            break;
                        case 4:
                            force.X = val;
                            break;
                        case 5:
                            force.Y = val;
                            break;
                        case 6:
                            force.Z = val;
                            break;
                        case 10:
                            force.Z = -val;
                            break;
                        default:
                            BH.Engine.Reflection.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
                            break;
                    }
                    switch (myTypes[i])
                    {
                        case 1:
                            loads.Add(new BarUniformlyDistributedLoad()
                            {
                                Force = force,
                                Loadcase = bhomCases[caseNames[i]],
                                Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                Axis = axis
                            });
                            break;
                        case 2:
                            loads.Add(new BarUniformlyDistributedLoad()
                            {
                                Moment = force,
                                Loadcase = bhomCases[caseNames[i]],
                                Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                Axis = axis
                            });
                            break;
                        default:
                            BH.Engine.Reflection.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + myTypes[i].ToString());
                            break;
                    }
                }
            }



            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadAreaLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Panel> bhomPanels = ReadPanel().ToDictionary(x => x.CustomData[AdapterId].ToString());

            int count = 0;
            string[] areaNames = null;
            string[] caseNames = null;
            string[] cSys = null;
            int[] dir = null;
            double[] val = null;


            if (m_model.AreaObj.GetLoadUniform("All", ref count, ref areaNames, ref caseNames, ref cSys, ref dir, ref val, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Panel bhomPanel = bhomPanels[areaNames[i]];
                    
                    Vector force = new Vector();
                    LoadAxis axis = cSys[i].LoadAxisToBHoM();
                    switch (dir[i])
                    {
                        case 1:
                            force.X = val[i];
                            break;
                        case 2:
                            force.Y = val[i];
                            break;
                        case 3:
                            force.Z = val[i];
                            break;
                        case 4:
                            force.X = val[i];
                            break;
                        case 5:
                            force.Y = val[i];
                            break;
                        case 6:
                            force.Z = val[i];
                            break;
                        case 10:
                            force.Z = -val[i];
                            break;
                        default:
                            BH.Engine.Reflection.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
                            break;
                    }

                    loads.Add(new AreaUniformlyDistributedLoad()
                    {
                        Pressure = force,
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<IAreaElement>() { Elements = { bhomPanel as IAreaElement} },
                        Axis = axis
                    });
                }
            }

            return loads;
        }

        /***************************************************/
    }
}
