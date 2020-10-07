﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine.Reflection;
using System.Threading;
using BH.Engine.Adapter;

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
                Loadcase bhomCase = new Loadcase();
                SetAdapterId(bhomCase, names[i]);

                eLoadPatternType patternType = eLoadPatternType.Dead;

                if (m_model.LoadPatterns.GetLoadType(names[i], ref patternType) == 0)
                {
                    bhomCase.Name = names[i];
                    bhomCase.Number = i;
                    bhomCase.Nature = patternType.ToBHoM();
                }
                else
                {
                    ReadElementError("Load Pattern", names[i]);
                }

                loadCases.Add(bhomCase);
            }

            return loadCases;
        }

        /***************************************************/

        private List<LoadCombination> ReadLoadCombination(List<string> ids = null)
        {
            List<LoadCombination> combinations = new List<LoadCombination>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => GetAdapterId<string>(x));

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
                    LoadCombination bhomCombo = new LoadCombination()
                    {
                        Name = names[i],
                        Number = i,
                    };
                    SetAdapterId(bhomCombo, names[i]);
                    if (caseCount > 0)
                    {
                        List<ICase> comboCases = new List<ICase>();
                        for (int j = 0; j < caseCount; j++)
                        {
                            comboCases.Add(bhomCases[caseNames[j]]);
                            bhomCombo.LoadCases.Add(new Tuple<double, ICase>(factors[j], comboCases[j]));
                        }
                    }

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
                return ReadBarUniformDistributedLoad();
            else if (type == typeof(BarVaryingDistributedLoad))
                return ReadBarVaryingDistributedLoad();
            else if (type == typeof(BarTemperatureLoad))
                return ReadBarTemperatureLoad();
            else if (type == typeof(AreaUniformlyDistributedLoad))
                return ReadAreaLoad();
            else if (type == typeof(AreaTemperatureLoad))
                return ReadAreaTemperatureLoad();
            else if (type == typeof(PointDisplacement))
                return ReadPointDispl();
            else if (type == typeof(GravityLoad))
                return ReadGravityLoad();
            else
            {
                List<ILoad> loads = new List<ILoad>();
                loads.AddRange(ReadPointLoad());
                loads.AddRange(ReadBarUniformDistributedLoad());
                loads.AddRange(ReadBarVaryingDistributedLoad());
                loads.AddRange(ReadBarTemperatureLoad());
                loads.AddRange(ReadAreaLoad());
                loads.AddRange(ReadAreaTemperatureLoad());
                loads.AddRange(ReadPointDispl());
                return loads;
            }
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
                                Engine.Reflection.Compute.RecordWarning("That load direction is not supported. Dir = " + dir[i].ToString());
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
                                Engine.Reflection.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + myTypes[i].ToString());
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
                                Engine.Reflection.Compute.RecordWarning("That load direction is not yet supported. Dir = " + dir[i].ToString());
                                break;
                        }

                        switch (myTypes[i])
                        {
                            case 1:
                                loads.Add(new BarVaryingDistributedLoad()
                                {
                                    DistanceFromA = dist1[i],
                                    ForceA = forceA,
                                    DistanceFromB = dist2[i],
                                    ForceB = forceB,
                                    Loadcase = bhomCases[caseNames[i]],
                                    Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                    Axis = axis
                                });
                                break;
                            case 2:
                                loads.Add(new BarVaryingDistributedLoad()
                                {
                                    DistanceFromA = dist1[i],
                                    MomentA = forceA,
                                    DistanceFromB = dist2[i],
                                    MomentB = forceB,
                                    Loadcase = bhomCases[caseNames[i]],
                                    Objects = new BHoMGroup<Bar>() { Elements = { bhomBar } },
                                    Axis = axis
                                });
                                break;
                            default:
                                Engine.Reflection.Compute.RecordWarning("Could not create the load. It's not 'MyType'. MyType = " + myTypes[i].ToString());
                                break;
                        }
                    }
                }
            }

            return loads;
        }

        /***************************************************/

        private List<ILoad> ReadBarTemperatureLoad(List<string> id = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<string, Loadcase> bhomCases = ReadLoadcase().ToDictionary(x => x.Name.ToString());
            Dictionary<string, Bar> bhomBars = ReadBars().ToDictionary(x => x.CustomData[AdapterIdName].ToString());

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

                    if (loadType[i] != 1)
                        Engine.Reflection.Compute.RecordError("The BHoM currently only supports uniform temperature changes applied to bars, not temperature gradients across the local 2 or 3 axes (SAP2000 convention)." +
                            "\nApplied SAP2000 gradients will be pulled as single temperature value into the BHoM.");

                    loads.Add(new BarTemperatureLoad()
                    {
                        TemperatureChange = tempChange,
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<Bar> { Elements = { bhomBar } },
                        Axis = LoadAxis.Global,
                        Projected = false,
                    }) ;
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

                        BH.Engine.Reflection.Compute.RecordNote("SAP2000 can only handle gravity loads in the Z direction.");
                        BH.Engine.Reflection.Compute.RecordNote("SAP2000 handles gravity loads via load patterns, " +
                                                                "so the returned gravity loads correlate to load patterns. " +
                                                                "Gravity loads are applied to all members.");
                    }
                }
            }

            return loads;

        }

        /***************************************************/

        private List<ILoad> ReadAreaTemperatureLoad(List<string> ids = null)
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
                        
                        Engine.Reflection.Compute.RecordWarning("Temperature gradient not currently implemented in the BHoM. An attempt has been made to convert SAP2000's gradient to a constant temperature change.");
                    }

                    loads.Add(new AreaTemperatureLoad()
                    {
                        TemperatureChange = tempForce,
                        Loadcase = bhomCases[caseNames[i]],
                        Objects = new BHoMGroup<IAreaElement>() { Elements = { bhomPanel as IAreaElement } },
                        Axis = LoadAxis.Global
                    });
                }
            }
            return loads;
        }

        /***************************************************/
    }
}
