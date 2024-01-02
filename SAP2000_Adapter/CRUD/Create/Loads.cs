/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Adapter;
using BH.Engine.Base;

using BH.Engine.Structure;
using BH.Engine.Spatial;
using BH.oM.Adapter.Commands;
using BH.oM.Adapters.SAP2000;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Loadcase loadcase)
        {
            double selfWeight = 0;
            m_model.LoadPatterns.Add(loadcase.Name, loadcase.Nature.ToCSI(), selfWeight, false);

            //Create Load Case based on Pattern:
            string[] loadTypes   = { "Load" };
            string[] loadNames   = { loadcase.Name };
            double[] loadFactors = { 1 };

            m_model.LoadCases.StaticLinear.SetCase(loadcase.Name);
            m_model.LoadCases.StaticLinear.SetLoads(loadcase.Name, 1, ref loadTypes, ref loadNames, ref loadFactors);

            SetAdapterId(loadcase, loadcase.Name);
            loadcase.Number = m_model.LoadPatterns.Count();
            
            return true;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            eCNameType nameType = eCNameType.LoadCase;

            int caseNameCount = 0;
            string[] caseNameArr = null;

            m_model.LoadPatterns.GetNameList(ref caseNameCount, ref caseNameArr);

            if (m_model.RespCombo.Add(loadcombination.Name, 0) == 0)
            {
                SetAdapterId(loadcombination, loadcombination.Name);
                foreach (Tuple<double, ICase> comboCase in loadcombination.LoadCases)
                {
                    double factor = comboCase.Item1;
                    ICase bhomCase = comboCase.Item2;
                    string caseName = "";

                    if (bhomCase.HasAdapterIdFragment(typeof(SAP2000Id)))
                    {
                        caseName = GetAdapterId<string>(bhomCase);
                    }
                    else if (caseNameArr.Contains(bhomCase.Name))
                    {
                        caseName = bhomCase.Name;
                    }
                    else 
                    {
                        Engine.Base.Compute.RecordWarning($"case {bhomCase.Name} has no SAP2000_id, and no case with that name was found in the model. Try pushing the loadcase and using the result of that push to build the combo.");
                    }


                    if (m_model.RespCombo.SetCaseList(loadcombination.Name, ref nameType, caseName, factor) != 0)
                            Engine.Base.Compute.RecordWarning("Could not add case " + bhomCase.Name + " to combo " + loadcombination.Name);
                }
            }
            else
            {
                CreateElementError("Load Combination", loadcombination.Name);
            }

            return true;
        }

        /***************************************************/

        private bool CreateObject(ILoad bhLoad)
        {
            return CreateLoad(bhLoad as dynamic);
        }

        /***************************************************/

        private bool CreateLoad(PointLoad bhLoad)
        {
            List<Node> nodes = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            string cSys = bhLoad.Axis.ToCSI();
            double[] val = 
            {
                bhLoad.Force.X,
                bhLoad.Force.Y,
                bhLoad.Force.Z,
                bhLoad.Moment.X,
                bhLoad.Moment.Y,
                bhLoad.Moment.Z,
            };

            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Node bhNode in nodes)
            {
                if (m_model.PointObj.SetLoadForce(GetAdapterId<string>(bhNode), loadPat, ref val, replace, cSys, eItemType.Objects) != 0)
                {
                    CreateElementError("Point Load", bhLoad.Name);
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }


        /***************************************************/

        private bool CreateLoad(PointDisplacement bhLoad)
        {
            List<Node> nodes = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            string cSys = bhLoad.Axis.ToCSI();
            double[] val =
            {
                bhLoad.Translation.X,
                bhLoad.Translation.Y,
                bhLoad.Translation.Z,
                bhLoad.Rotation.X,
                bhLoad.Rotation.Y,
                bhLoad.Rotation.Z,
            };

            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Node bhNode in nodes)
            {
                if (m_model.PointObj.SetLoadDispl(GetAdapterId<string>(bhNode), loadPat, ref val, replace, cSys, eItemType.Objects) != 0)
                {
                    CreateElementError("Point Displacement", bhLoad.Name);
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }


        /***************************************************/

        private bool CreateLoad(BarUniformlyDistributedLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            double dist1 = 0;
            double dist2 = 1;
            int[] dirs = null;
            double[] forceVals = null;
            double[] momentVals = null;
            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    if (bhLoad.Projected)
                    {
                        dirs = new int[] { 7, 8, 9};
                    }
                    else
                    {
                        dirs = new int[] { 4, 5, 6 };
                    }
                    forceVals = bhLoad.Force.ToDoubleArray();
                    momentVals = bhLoad.Moment.ToDoubleArray();
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    forceVals = bhLoad.Force.BarLocalAxisToCSI().ToDoubleArray();
                    momentVals = bhLoad.Moment.BarLocalAxisToCSI().ToDoubleArray();
                    break;
            }
            bool relDist = true;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Bar bhBar in bars)
            {
                string name = GetAdapterId<string>(bhBar);
                bool replaceNow = replace;
                for (int i = 0; i < dirs.Count(); i++)
                {
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 1, dirs[i], dist1, dist2, forceVals[i], forceVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                    replaceNow = false;
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 2, dirs[i], dist1, dist2, momentVals[i], momentVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }


        /***************************************************/

        private bool CreateLoad(BarVaryingDistributedLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            double dist1 = bhLoad.StartPosition;
            double dist2 = 1.0;
            int[] dirs = null;
            double[] forceValsA = null;
            double[] momentValsA = null;
            double[] forceValsB = null;
            double[] momentValsB = null;

            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    if (bhLoad.Projected)
                    {
                        dirs = new int[] { 7, 8, 9 };
                    }
                    else
                    {
                        dirs = new int[] { 4, 5, 6 };
                    }
                    forceValsA = bhLoad.ForceAtStart.ToDoubleArray();
                    momentValsA = bhLoad.MomentAtStart.ToDoubleArray();
                    forceValsB = bhLoad.ForceAtEnd.ToDoubleArray();
                    momentValsB = bhLoad.MomentAtEnd.ToDoubleArray();
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    forceValsA = bhLoad.ForceAtStart.BarLocalAxisToCSI().ToDoubleArray();
                    momentValsA = bhLoad.MomentAtStart.BarLocalAxisToCSI().ToDoubleArray();
                    forceValsB = bhLoad.ForceAtEnd.BarLocalAxisToCSI().ToDoubleArray();
                    momentValsB = bhLoad.MomentAtEnd.BarLocalAxisToCSI().ToDoubleArray();
                    break;
            }
            bool relDist = bhLoad.RelativePositions;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Bar bhBar in bars)
            {
                string name = GetAdapterId<string>(bhBar);
                dist2 = bhLoad.EndPosition;
                bool replaceNow = replace;
                for (int i = 0; i < dirs.Count(); i++)
                {
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 1, dirs[i], dist1, dist2, forceValsA[i], forceValsB[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                    replaceNow = false;
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 2, dirs[i], dist1, dist2, momentValsA[i], momentValsB[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }


        /***************************************************/

        private bool CreateLoad(AreaUniformlyDistributedLoad bhLoad)
        {
            List<IAreaElement> panels = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase); 
            double[] vals = bhLoad.Pressure.ToDoubleArray();
            int[] dirs = null;
            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    if (bhLoad.Projected)
                    {
                        dirs = new int[] { 7, 8, 9 };
                    }
                    else
                    {
                        dirs = new int[] { 4, 5, 6 };
                    }
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    break;
            }

            bool replace = SAPPushConfig.ReplaceLoads;

            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

            foreach (Panel panel in panels)
            {
                string name = GetAdapterId<string>(panel);
                string propName = "";
                m_model.AreaObj.GetProperty(name, ref propName);
                if (propName == "None")
                {
                    bool replaceNow = replace;
                    for (int i = 0; i < dirs.Count(); i++)
                    {
                        if (vals[i] != 0)
                        {
                            if (m_model.AreaObj.SetLoadUniformToFrame(name, loadPat, vals[i], dirs[i], 2, replaceNow, cSys, type) != 0)
                                Engine.Base.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
                            replaceNow = false;
                        }
                    }
                }
                else
                {
                    bool replaceNow = replace;
                    for (int i = 0; i < dirs.Count(); i++)
                    {
                        if (vals[i] != 0)
                        {
                            if (m_model.AreaObj.SetLoadUniform(name, loadPat, vals[i], dirs[i], replaceNow, cSys, type) != 0)
                                Engine.Base.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
                            replaceNow = false;
                        }
                    }
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(AreaUniformTemperatureLoad bhLoad)
        {
            List<IAreaElement> panels = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            double vals = bhLoad.TemperatureChange;
            int loadType = 1; // BHoM currently supports uniform temperature change, no support yet for temperature gradient

            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Panel panel in panels)
            {
                string name = GetAdapterId<string>(panel);
                bool replaceNow = replace;
                if (m_model.AreaObj.SetLoadTemperature(name, loadPat, loadType, vals, Replace:replaceNow) != 0)
                {
                    CreateElementError("AreaUniformTemperatureLoad", bhLoad.Name);
                }

            }

            SetAdapterId(bhLoad, null);

            Engine.Base.Compute.RecordNote("SAP2000 includes functionality for temperature gradient application, but that feature is not yet supported here.");

            return true;
        }

        /***************************************************/

        private bool CreateLoad(ContourLoad bhLoad)
        { 
            double[] loadVals = bhLoad.Force.ToDoubleArray();

            List<Opening> openingsList = new List<Opening>();
            Panel loadPanel = Engine.Structure.Create.Panel(bhLoad.Contour as ICurve, openingsList);
            CreateObject(loadPanel);

            List<Panel> panelsToLoad = new List<Panel>();
            panelsToLoad.Add(loadPanel);
            AreaUniformlyDistributedLoad contourLoadArea = Engine.Structure.Create.AreaUniformlyDistributedLoad(bhLoad.Loadcase, loadVals.ToVector(), panelsToLoad, 
                                                                                                                bhLoad.Axis, bhLoad.Projected, bhLoad.Name);
            CreateLoad(contourLoadArea);

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(GeometricalLineLoad bhLoad)
        {
            Node startNode = new Node { Position = bhLoad.Location.Start };
            Node endNode = new Node { Position = bhLoad.Location.End };
            CreateObject(startNode);
            CreateObject(endNode);

            Bar nullBar = new Bar { StartNode = startNode, EndNode = endNode };
            CreateObject(nullBar);

            BarVaryingDistributedLoad barVaryLoad = Engine.Structure.Create.BarVaryingDistributedLoad(
                bhLoad.Loadcase, 
                new List<Bar> { nullBar }, 
                0, //start
                bhLoad.ForceA,
                bhLoad.MomentA,
                1, //end
                bhLoad.ForceB,
                bhLoad.MomentB, 
                false, 
                bhLoad.Axis, 
                bhLoad.Projected, 
                bhLoad.Name
                );

            CreateLoad(barVaryLoad);

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(GravityLoad bhLoad)
        {
            double selfWeightExisting = 0;
            double selfWeightNew = -bhLoad.GravityDirection.Z;

            string caseName = GetAdapterId<string>(bhLoad.Loadcase);

            m_model.LoadPatterns.GetSelfWTMultiplier(caseName, ref selfWeightExisting);

            if (selfWeightExisting != 0)
                BH.Engine.Base.Compute.RecordWarning($"The self weight for loadcase {bhLoad.Loadcase.Name} will be overwritten. Previous value: {selfWeightExisting}, new value: {selfWeightNew}");

            m_model.LoadPatterns.SetSelfWTMultiplier(caseName, selfWeightNew);

            if (bhLoad.GravityDirection.X != 0 || bhLoad.GravityDirection.Y != 0)
                Engine.Base.Compute.RecordError("SAP2000 can only handle gravity loads in global z direction");

            BH.Engine.Base.Compute.RecordNote("SAP2000 handles gravity loads via loadcases, so only one gravity load per loadcase can be used. This gravity load will be applied to all objects.");

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(BarUniformTemperatureLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            double tempChange = bhLoad.TemperatureChange;
            int loadType = 1; // BHoM currently only supports uniform temperature change (=1); SAP supports gradients in local 2 and 3 axes (=2 and =3).
            
            bool replace = SAPPushConfig.ReplaceLoads;

            foreach (Bar bar in bars)
            {
                string name = GetAdapterId<string>(bar);
                if (m_model.FrameObj.SetLoadTemperature(name, loadPat, loadType, tempChange, Replace: replace) != 0)
                    CreateElementError("BarUniformTemperatureLoad", bhLoad.Name);
            }

            Engine.Base.Compute.RecordNote("SAP2000 includes functionality for temperature gradient in 2 and 3 local bar axes, but the BHoM currently only supports uniform temperature changes.");

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(BarPointLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            int loadType = 0;
            int[] dirs = null;
            double[] forceVals = null;
            double[] momentVals = null;
            double dist = bhLoad.DistanceFromA;
            bool relDist = false; // BarPointLoad is defined as absolute distance from i (A) end of bar.

            // Determine coordinate system (local vs global) and set directions vector for CSI protocol
            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    dirs = new int[] { 4, 5, 6 };
                    forceVals = bhLoad.Force.ToDoubleArray();
                    momentVals = bhLoad.Moment.ToDoubleArray();
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    forceVals = bhLoad.Force.BarLocalAxisToCSI().ToDoubleArray();
                    momentVals = bhLoad.Moment.BarLocalAxisToCSI().ToDoubleArray();
                    break;
            }

            // Loop through bars and set point loads
            string cSys = bhLoad.Axis.ToCSI();
            bool replace = SAPPushConfig.ReplaceLoads;
            eItemType type = eItemType.Objects;
            foreach (Bar bar in bars)
            {
                string barName = GetAdapterId<string>(bar);
                bool replaceNow = replace;

                for (int i = 0; i < dirs.Count(); i++)
                {
                    loadType = 1; // handle forces first
                    if (m_model.FrameObj.SetLoadPoint(barName, loadPat, loadType, dirs[i], dist, forceVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarPointLoad-Force", bar.Name + dirs[i]);
                    replaceNow = false;
                    loadType = 2; // handle moments second
                    if (m_model.FrameObj.SetLoadPoint(barName, loadPat, loadType, dirs[i], dist, momentVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarPointLoad-Moment", bar.Name + dirs[i]);
                }

            }

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(BarPrestressLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            bool[] targetForceBooleanByDOF = new bool[6];
            double[] targetForceValuesByDOF = new double[6];
            double[] relativeDistByDOF = new double[6];
            eItemType type = eItemType.Objects;

            // Loop through bars and set Target Force for each bar
            foreach (Bar bar in bars)
            {
                string barName = GetAdapterId<string>(bar);

                // Generic BooleanByDOF array allows for future prestressing moment to be implemented
                targetForceBooleanByDOF[0] = true;
                targetForceValuesByDOF[0] = bhLoad.Prestress;
                relativeDistByDOF[0] = 0.5;

                if (m_model.FrameObj.SetLoadTargetForce(barName, loadPat, ref targetForceBooleanByDOF, ref targetForceValuesByDOF, ref relativeDistByDOF, type) != 0)
                    CreateElementError("BarPrestressLoad", bar.Name);
                BH.Engine.Base.Compute.RecordWarning($"Target Force load case must be nonlinear static. Verify {loadPat} prior to running analysis.");
            }

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(BarDifferentialTemperatureLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = GetAdapterId<string>(bhLoad.Loadcase);
            string patternName = "None";
            bool replace = SAPPushConfig.ReplaceLoads;
            int myType = 1;

            double tempUniform = bhLoad.GetUniformComponent();

            // Loop through bars and set load for each bar
            foreach (Bar bar in bars)
            {
                string barName = GetAdapterId<string>(bar);

                double tempGradient = bhLoad.GetDifferentialComponent(bar, out myType);

                if (double.IsNaN(tempGradient))
                {
                    CreateElementError("BarDifferentialTemperatureLoad", barName);
                    continue; // skip this bar if it didn't work.
                }

                if (m_model.FrameObj.SetLoadTemperature(barName, loadPat, myType, tempGradient, patternName, replace, eItemType.Objects) != 0)
                    CreateElementError("BarDifferentialTemperatureLoad", bar.Name);

                if (tempUniform != 0)
                {
                    Engine.Base.Compute.RecordNote("BarDifferentialTemperatureLoad has been split into gradient and uniform components");
                    if (m_model.FrameObj.SetLoadTemperature(barName, loadPat, 1, tempUniform, patternName, false, eItemType.Objects) != 0)                    
                        CreateElementError("BarDifferentialTemperatureLoad", bar.Name);
                }
            }

            SetAdapterId(bhLoad, null);

            return true;
        }

        /***************************************************/

        private bool CreateLoad(ILoad bhLoad)
        {
            Engine.Base.Compute.RecordError($"TheLoad type {bhLoad.GetType()} is not implemented!");
            return false;
        }

        /***************************************************/
    }
}




