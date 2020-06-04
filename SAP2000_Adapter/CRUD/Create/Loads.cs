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

using BH.Engine.Reflection;
using BH.Engine.SAP2000;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
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

            loadcase.CustomData[AdapterIdName] = loadcase.Name;
            loadcase.Number = m_model.LoadPatterns.Count();
            
            return true;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            eCNameType nameType = eCNameType.LoadCase;
            if (m_model.RespCombo.Add(loadcombination.Name, 0) == 0)
            {
                loadcombination.CustomData[AdapterIdName] = loadcombination.Name;
                foreach (Tuple<double, ICase> comboCase in loadcombination.LoadCases)
                {
                    double factor = comboCase.Item1;
                    ICase bhomCase = comboCase.Item2;
                    if (!bhomCase.CustomData.ContainsKey(AdapterIdName))
                        Engine.Reflection.Compute.RecordWarning($"case {bhomCase.Name} has no {AdapterIdName}. Try pushing the loadcase and using the result of that push to build the combo.");

                    if (m_model.RespCombo.SetCaseList(loadcombination.Name, ref nameType, bhomCase.CustomData[AdapterIdName].ToString(), factor) != 0)
                            Engine.Reflection.Compute.RecordWarning("Could not add case " + bhomCase.Name + " to combo " + loadcombination.Name);
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
            string loadPat = bhLoad.Loadcase.CustomData[AdapterIdName].ToString();
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

            bool replace = true;

            foreach (Node bhNode in nodes)
            {
                if (m_model.PointObj.SetLoadForce(bhNode.CustomData[AdapterIdName].ToString(), loadPat, ref val, replace, cSys, eItemType.Objects) != 0)
                {
                    CreateElementError("Point Load", bhLoad.Name);
                }
            }

            return true;
        }


        /***************************************************/

        private bool CreateLoad(PointDisplacement bhLoad)
        {
            List<Node> nodes = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterIdName].ToString();
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

            bool replace = true;

            foreach (Node bhNode in nodes)
            {
                if (m_model.PointObj.SetLoadDispl(bhNode.CustomData[AdapterIdName].ToString(), loadPat, ref val, replace, cSys, eItemType.Objects) != 0)
                {
                    CreateElementError("Point Displacement", bhLoad.Name);
                }
            }

            return true;
        }


        /***************************************************/

        private bool CreateLoad(BarUniformlyDistributedLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterIdName].ToString();
            double dist1 = 0;
            double dist2 = 1;
            int[] dirs = null;
            double[] forceVals = null;
            double[] momentVals = null;
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
            bool relDist = true;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;
            bool replace = true;
            

            foreach (Bar bhBar in bars)
            {
                string name = bhBar.CustomData[AdapterIdName].ToString();
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

            return true;
        }

        /***************************************************/

        private bool CreateLoad(AreaUniformlyDistributedLoad bhLoad)
        {
            List<IAreaElement> panels = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterIdName].ToString();
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
            bool replace = true;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

            foreach (Panel panel in panels)
            {
                string name = panel.CustomData[AdapterIdName].ToString();
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
                                Engine.Reflection.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
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
                                Engine.Reflection.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
                            replaceNow = false;
                        }
                    }
                }
            }

            return true;
        }

        /***************************************************/
        /* JL CONTOUR LOAD */
        private bool CreateLoad(ContourLoad bhLoad)
        { 
            string loadPat = bhLoad.Loadcase.CustomData[AdapterIdName].ToString();
            string name = bhLoad.Name;
            double[] vals = bhLoad.Force.ToDoubleArray();
            int[] dirs = null;

            List<Point> points = bhLoad.Contour.ControlPoints.ToList();
            double[] x_coords = new double[points.Count];
            double[] y_coords = new double[points.Count];
            double[] z_coords = new double[points.Count];

            int point_index = 0;
            foreach (Point bhPoint in points)
            {
                x_coords[point_index] = bhPoint.X;
                y_coords[point_index] = bhPoint.Y;
                z_coords[point_index] = bhPoint.Z;
                point_index++;
            }
            
            
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

            if (m_model.AreaObj.AddByCoord(points.Count - 1, ref x_coords, ref y_coords, ref z_coords, ref name, "None") != 0)
            {
                CreateElementError("Contour Load", bhLoad.Name);
            }
            /*
            bool replace = true;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

            foreach (Panel panel in panels)
            {
                string name = panel.CustomData[AdapterIdName].ToString();
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
                                Engine.Reflection.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
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
                                Engine.Reflection.Compute.RecordWarning($"Could not assign an area load in direction {dirs[i]}");
                            replaceNow = false;
                        }
                    }
                }
            }
            */
            return true;
        }


        /***************************************************/

        public void SetLoad(GravityLoad gravityLoad, bool replace)
        {
            double selfWeightExisting = 0;
            double selfWeightNew = -gravityLoad.GravityDirection.Z;

            string caseName = gravityLoad.Loadcase.CustomData[AdapterIdName].ToString();

            m_model.LoadPatterns.GetSelfWTMultiplier(caseName, ref selfWeightExisting);

            if (selfWeightExisting != 0)
                BH.Engine.Reflection.Compute.RecordWarning($"The self weight for loadcase {gravityLoad.Loadcase.Name} will be overwritten. Previous value: {selfWeightExisting}, new value: {selfWeightNew}");

            m_model.LoadPatterns.SetSelfWTMultiplier(caseName, selfWeightNew);

            if (gravityLoad.GravityDirection.X != 0 || gravityLoad.GravityDirection.Y != 0)
                Engine.Reflection.Compute.RecordError("SAP2000 can only handle gravity loads in global z direction");

            BH.Engine.Reflection.Compute.RecordNote("SAP2000 handles gravity loads via loadcases, so only one gravity load per loadcase can be used. This gravity load will be applied to all objects");
        }

        /***************************************************/
        
    }
}
