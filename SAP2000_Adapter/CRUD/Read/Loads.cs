/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */


using BH.Engine.Structure;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using System.Threading;
using BH.Engine.Adapter;
using System.IO;
using System.ComponentModel.Design;

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

            int nameCount = 0;
            string[] nameArr = null;

            m_model.LoadPatterns.GetNameList(ref nameCount, ref nameArr);
            ids = FilterIds(ids, nameArr);

            foreach (string id in ids)
            {
                Loadcase bhomCase = new Loadcase();

                eLoadPatternType patternType = eLoadPatternType.Other;

                if (m_model.LoadPatterns.GetLoadType(id, ref patternType) == 0)
                {
                    bhomCase.Name = id;
                    bhomCase.Nature = patternType.ToBHoM();
                }
                else
                {
                    ReadElementError("Load Pattern", id);
                }

                SetAdapterId(bhomCase, id);
                loadCases.Add(bhomCase);
            }

            return loadCases;
        }

        /***************************************************/

        private List<LoadCombination> ReadLoadCombination(List<string> ids = null)
        {
            List<LoadCombination> combinations = new List<LoadCombination>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => GetAdapterId<string>(x));

            int nameCount = 0;
            string[] nameArr = null;
            m_model.RespCombo.GetNameList(ref nameCount, ref nameArr);

            ids = FilterIds(ids, nameArr);
            
            foreach (string id in ids)
            {

                LoadCombination bhomCombo = new LoadCombination();

                double[] factors = null;
                int caseCount = 0;
                eCNameType[] caseTypes = null;
                string[] caseNames = null;

                if (m_model.RespCombo.GetCaseList(id, ref caseCount, ref caseTypes, ref caseNames, ref factors) != 0)
                {
                    ReadElementError("Load Combo", id);
                }
                else
                {
                    bhomCombo.Name = id;

                    if (caseCount > 0)
                    {
                        List<ICase> comboCases = new List<ICase>();
                        for (int j = 0; j < caseCount; j++)
                        {
                            comboCases.Add(bhomCases[caseNames[j]]);
                            bhomCombo.LoadCases.Add(new Tuple<double, ICase>(factors[j], comboCases[j]));
                        }
                    }

                    SetAdapterId(bhomCombo, id);
                    combinations.Add(bhomCombo);
                }
            }

            return combinations;
        }

        /***************************************************/

        private List<ILoad> ReadLoad(Type type, List<string> ids = null)
        {
           if (ids != null)
            {
                Engine.Base.Compute.RecordWarning("Id filtering is not implemented for loads, all loads will be returned.");
            }

            if (type == typeof(PointLoad))
                return ReadPointLoad();
            else if (type == typeof(BarPointLoad))
                return ReadBarPointLoad();
            else if (type == typeof(BarUniformlyDistributedLoad))
                return ReadBarUniformDistributedLoad();
            else if (type == typeof(BarVaryingDistributedLoad))
                return ReadBarVaryingDistributedLoad();
            else if (type == typeof(BarUniformTemperatureLoad))
                return ReadBarUniformTemperatureLoad();
            else if (type == typeof(BarDifferentialTemperatureLoad))
                return ReadBarDifferentialTemperatureLoad();
            else if (type == typeof(ContourLoad))
                return ReadContourLoad();
            else if (type == typeof(GeometricalLineLoad))
                return ReadGeometricalLineLoad();
            else if (type == typeof(BarPrestressLoad))
                return ReadBarPrestressLoad();
            else if (type == typeof(AreaUniformlyDistributedLoad))
                return ReadAreaLoad();
            else if (type == typeof(AreaUniformTemperatureLoad))
                return ReadAreaUniformTemperatureLoad();
            else if (type == typeof(PointDisplacement))
                return ReadPointDispl();
            else if (type == typeof(PointVelocity))
                return ReadPointVelocity();
            else if (type == typeof(PointAcceleration))
                return ReadPointAcceleration();
            else if (type == typeof(GravityLoad))
                return ReadGravityLoad();
            else if (type is null)
            {
                List<ILoad> loads = new List<ILoad>();
                loads.AddRange(ReadPointLoad());
                loads.AddRange(ReadBarUniformDistributedLoad());
                loads.AddRange(ReadBarVaryingDistributedLoad());
                loads.AddRange(ReadBarUniformTemperatureLoad());
                loads.AddRange(ReadAreaLoad());
                loads.AddRange(ReadAreaUniformTemperatureLoad());
                loads.AddRange(ReadPointDispl());
                return loads;
            }
            else
                Engine.Base.Compute.RecordError($"Could not read loads, reading loads of type: {type} is not implemented");

            return new List<ILoad>();
        }

        /***************************************************/

        private List<ILoad> ReadPointLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => GetAdapterId<string>(x));

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

        private List<ILoad> ReadPointDispl(List<string> ids = null)
        {
            // Nodal Displacements as Nodal Loads
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] nodeNames = null;
            string[] caseNames = null;
            int[] steps = null;
            string[] cSys = null;
            double[] u1 = null;
            double[] u2 = null;
            double[] u3 = null;
            double[] r1 = null;
            double[] r2 = null;
            double[] r3 = null;


            if (m_model.PointObj.GetLoadDispl("All", ref count, ref nodeNames, ref caseNames, ref steps, ref cSys, ref u1, ref u2, ref u3, ref r1, ref r2, ref r3, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    loads.Add(new PointDisplacement()
                    {
                        Translation = new Vector() { X = u1[i], Y = u2[i], Z = u3[i] },
                        Rotation = new Vector() { X = r1[i], Y = r2[i], Z = r3[i] },
                        Loadcase = bhomCases[caseNames[i]],
                        Axis = cSys[i].LoadAxisToBHoM(),
                        Objects = new BHoMGroup<Node>() { Elements = { bhomNodes[nodeNames[i]] } }
                    });
                }
            }



            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadPointVelocity(List<string> ids = null)
        {
            Engine.Base.Compute.RecordError("Read PointVelocity is not implemented!");
            return new List<ILoad>();
        }

        /***************************************************/

        private List<ILoad> ReadPointAcceleration(List<string> ids = null)
        {
            Engine.Base.Compute.RecordError("Read PointVelocity is not implemented!");
            return new List<ILoad>();
        }

        /***************************************************/

        private List<ILoad> ReadBarUniformDistributedLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

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

                    if (dist1[i] != 0 || rd1[i] != 0 || dist2[i] != bhomBar.Length() || rd2[i] != 1 || val1[i] != val2[i])
                    {
                        // placeholder for potential action
                    }
                    else
                    {
                        double val = val1[i];
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
                                Engine.Base.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
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
                                Engine.Base.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + myTypes[i].ToString());
                                break;
                        }
                    }
                }
            }


            return loads;
        }


        /***************************************************/

        private List<ILoad> ReadBarVaryingDistributedLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

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

                    if (dist1[i] == 0 && rd1[i] == 0 && dist2[i] == bhomBar.Length() && rd2[i] == 1 && val1[i] == val2[i])
                    {
                        // placeholder for potential action
                    }
                    else
                    {

                        LoadAxis axis = cSys[i].LoadAxisToBHoM();

                        Vector forceA = new Vector();
                        Vector forceB = new Vector();

                        switch (dir[i])
                        {
                            case 1:
                                forceA.X = val1[i];
                                forceB.X = val2[i];
                                break;
                            case 2:
                                forceA.Y = val1[i];
                                forceB.Y = val2[i];
                                break;
                            case 3:
                                forceA.Z = -val1[i];
                                forceB.Z = -val2[i];
                                break;
                            case 4:
                                forceA.X = val1[i];
                                forceB.X = val2[i];
                                break;
                            case 5:
                                forceA.Y = val1[i];
                                forceB.Y = val2[i];
                                break;
                            case 6:
                                forceA.Z = val1[i];
                                forceB.Z = val2[i];
                                break;
                            case 10:
                                forceA.Z = -val1[i];
                                forceB.Z = -val2[i];
                                break;
                            default:
                                Engine.Base.Compute.RecordWarning("That load direction is not yet supported. Dir = " + dir[i].ToString());
                                break;
                        }

                        switch (myTypes[i])
                        {
                            case 1:
                                loads.Add(new BarVaryingDistributedLoad()
                                {
                                    StartPosition = dist1[i],
                                    ForceAtStart = forceA,
                                    EndPosition = dist2[i],
                                    ForceAtEnd = forceB,
                                    Loadcase = bhomCases[caseNames[i]],
                                    Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                    Axis = axis,
                                    RelativePositions = false
                                });
                                break;
                            case 2:
                                loads.Add(new BarVaryingDistributedLoad()
                                {
                                    StartPosition = dist1[i],
                                    MomentAtStart = forceA,
                                    EndPosition = dist2[i],
                                    MomentAtEnd = forceB,
                                    Loadcase = bhomCases[caseNames[i]],
                                    Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                    Axis = axis,
                                    RelativePositions = false
                                });
                                break;
                            default:
                                Engine.Base.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + myTypes[i].ToString());
                                break;
                        }
                    }
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarUniformTemperatureLoad(List<string> id = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] barNames = null;
            string[] caseNames = null;
            int[] loadType = null;
            string[] jointPattern = null;
            double[] val = null;

            if (m_model.FrameObj.GetLoadTemperature("ALL", ref count, ref barNames, ref caseNames, ref loadType, ref val, ref jointPattern, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Bar bhomBar = bhomBars[barNames[i]];
                    double tempChange = val[i];

                    if (loadType[i] == 1)
                    {
                        loads.Add(new BarUniformTemperatureLoad { Loadcase = bhomCases[caseNames[i]], TemperatureChange = tempChange, Objects = new BHoMGroup<Bar> { Elements = { bhomBar } }, Axis = LoadAxis.Global, Projected = false });
                    }
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarDifferentialTemperatureLoad(List<string> id = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] barNames = null;
            string[] caseNames = null;
            int[] loadType = null;
            string[] jointPattern = null;
            double[] val = null;
            double topTemp;
            double bottomTemp;

            if (m_model.FrameObj.GetLoadTemperature("ALL", ref count, ref barNames, ref caseNames, ref loadType, ref val, ref jointPattern, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Bar bhomBar = bhomBars[barNames[i]];

                    if (jointPattern[i] == "None")
                    {
                        Engine.Base.Compute.RecordWarning("BarDifferentialTemperatureLoads cannot vary along the bar - only constant temperatures and variation across the bar are supported. The load has been ignored.");
                    }
                    else
                    {
                        switch (loadType[i])
                        {
                            case 2:
                                topTemp = val[i] * (bhomBar.SectionProperty.Vpy + bhomBar.SectionProperty.Vy) / 2;
                                bottomTemp = -val[i] * (bhomBar.SectionProperty.Vpy + bhomBar.SectionProperty.Vy) / 2;

                                loads.Add(Engine.Structure.Create.BarDifferentialTemperatureLoad(bhomCases[caseNames[i]], topTemp, bottomTemp, DifferentialTemperatureLoadDirection.LocalY, new List<Bar> { bhomBar }));
                                break;
                            case 3:
                                topTemp = val[i] * (bhomBar.SectionProperty.Vpz + bhomBar.SectionProperty.Vz) / 2;
                                bottomTemp = -val[i] * (bhomBar.SectionProperty.Vpz + bhomBar.SectionProperty.Vz) / 2;

                                loads.Add(Engine.Structure.Create.BarDifferentialTemperatureLoad(bhomCases[caseNames[i]], topTemp, bottomTemp, DifferentialTemperatureLoadDirection.LocalZ, new List<Bar> { bhomBar }));
                                break;
                            case 1:
                            default:
                                break;
                        }
                    }
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarPointLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] frameNames = null;
            string[] caseNames = null;
            int[] loadTypes = null;
            string[] cSys = null;
            int[] dir = null;
            double[] relDis = null;
            double[] dist = null;
            double[] val = null;

            if (m_model.FrameObj.GetLoadPoint("All", ref count, ref frameNames, ref caseNames, ref loadTypes, ref cSys, ref dir, ref relDis, ref dist, ref val, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
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
                            force.Z = -val[i];
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
                            Engine.Base.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
                            break;
                    }
                    switch (loadTypes[i])
                    {
                        case 1:
                            loads.Add(new BarPointLoad()
                            {
                                DistanceFromA = relDis[i],
                                Force = force,
                                Loadcase = bhomCases[caseNames[i]],
                                Objects = new BHoMGroup<Bar>() { Elements = { bhomBars[frameNames[i]] } },
                                Axis = axis
                            });
                            break;
                        case 2:
                            loads.Add(new BarPointLoad()
                            {
                                DistanceFromA = relDis[i],
                                Moment = force,
                                Loadcase = bhomCases[caseNames[i]],
                                Objects = new BHoMGroup<Bar>() { Elements = { bhomBars[frameNames[i]] } },
                                Axis = axis
                            });
                            break;
                        default:
                            Engine.Base.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + loadTypes[i].ToString());
                            break;
                    }
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarPrestressLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] frameNames = null;
            string[] caseNames = null;

            // Arrays of booleans indicating if the DOF has a target force assignment
            bool[] pBool = null;
            bool[] v2Bool = null;
            bool[] v3Bool = null;
            bool[] tBool = null;
            bool[] m2Bool = null;
            bool[] m3Bool = null;
            // Arrays of target force values for each DOF above
            double[] pVal = null;
            double[] v2Val = null;
            double[] v3Val = null;
            double[] tVal = null;
            double[] m2Val = null;
            double[] m3Val = null;
            // Arrays of relative distances along frame objects where target force values apply
            double[] pRelDis = null;
            double[] v2RelDis = null;
            double[] v3RelDis = null;
            double[] tRelDis = null;
            double[] m2RelDis = null;
            double[] m3RelDis = null;


            if (m_model.FrameObj.GetLoadTargetForce("All", ref count, ref frameNames, ref caseNames,
                                                    ref pBool, ref v2Bool, ref v3Bool, ref tBool, ref m2Bool, ref m3Bool,
                                                    ref pVal, ref v2Val, ref v3Val, ref tVal, ref m2Val, ref m3Val,
                                                    ref pRelDis, ref v2RelDis, ref v3RelDis, ref tRelDis, ref m2RelDis, ref m3RelDis,
                                                    eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    loads.Add(new BarPrestressLoad()
                    {
                        Prestress = pVal[i],
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<Bar>() { Elements = { bhomBars[frameNames[i]] } },
                        Axis = LoadAxis.Local,
                        Projected = false
                    });
                    
                }
            }
            
            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadAreaLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Panel> bhomPanels = ReadPanel().ToDictionary(x => GetAdapterId<string>(x));

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
                            BH.Engine.Base.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
                            break;
                    }

                    loads.Add(new AreaUniformlyDistributedLoad()
                    {
                        Pressure = force,
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<IAreaElement>() { Elements = { bhomPanel } },
                        Axis = axis
                    });
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadGravityLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            
            double selfWtMultiplier = 0;

            foreach (Loadcase loadCase in bhomCases.Values)
            {
                if (m_model.LoadPatterns.GetSelfWTMultiplier(loadCase.Name.ToString(), ref selfWtMultiplier) == 0)
                {
                    if (selfWtMultiplier != 0)
                    {
                        loads.Add(new GravityLoad()
                        {
                            GravityDirection = Engine.Geometry.Create.Vector(0, 0, -selfWtMultiplier),
                            Loadcase = loadCase,
                            Axis = BH.oM.Structure.Loads.LoadAxis.Global,
                        });

                        BH.Engine.Base.Compute.RecordNote("SAP2000 can only handle gravity loads in the Z direction.");
                        BH.Engine.Base.Compute.RecordNote("SAP2000 handles gravity loads via load patterns, " +
                                                                "so the returned gravity loads correlate to load patterns. " +
                                                                "Gravity loads are applied to all members.");
                    }
                }
            }

            return loads;

        }

        /***************************************************/

        private List<ILoad> ReadAreaUniformTemperatureLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Panel> bhomPanels = ReadPanel().ToDictionary(x => GetAdapterId<string>(x));

            int count = 0;
            string[] areaNames = null;
            string[] caseNames = null;
            int[] loadType = null;
            string[] jointPattern = null;
            double[] val = null;

            if (m_model.AreaObj.GetLoadTemperature("ALL", ref count, ref areaNames, ref caseNames, ref loadType, ref val, ref jointPattern, eItemType.Group) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Panel bhomPanel = bhomPanels[areaNames[i]];
                    double tempForce = val[i];

                    if (loadType[i] != 1)
                    {
                        try
                        {
                            double avgConstTemp = 0.5 * tempForce * bhomPanel.Property.IAverageThickness();
                            tempForce = avgConstTemp;
                        }
                        catch { }

                        Engine.Base.Compute.RecordWarning("Temperature gradient not currently implemented in the BHoM. An attempt has been made to convert SAP2000's gradient to a constant temperature change.");
                    }

                    loads.Add(new AreaUniformTemperatureLoad()
                    {
                        TemperatureChange = tempForce,
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<IAreaElement>() { Elements = { bhomPanel } },
                        Axis = LoadAxis.Global
                    });
                }
            }
            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadContourLoad(List<string> ids = null)
        {
            Engine.Base.Compute.RecordError("ContourLoads are mapped to Null Areas with AreaLoads, so we can't be sure the object was originally a ContourLoad or not - suggest pulling AreaUniformlyDistributedLoad and filtering for null properties.");
            return new List<ILoad>();
        }

        /***************************************************/

        private List<ILoad> ReadGeometricalLineLoad(List<string> ids = null)
        {
            Engine.Base.Compute.RecordError("GeometricalLineLoads are mapped to Null Frames with Uniform Loads, so we can't be sure the object was originally a GeometricalLineLoads or not - suggest pulling BarUniformlyDistributedLoad and filtering for null properties.");
            return new List<ILoad>();
        }

        /***************************************************/
    }
}


