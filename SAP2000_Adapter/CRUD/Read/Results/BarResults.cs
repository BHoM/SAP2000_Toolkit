/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Adapter;
using SAP2000v1;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(BarResultRequest request,
                                                ActionConfig actionConfig = null)
        {
            CheckAndSetUpCases(request);
            List<string> barIds = CheckGetBarIds(request);

            switch (request.ResultType)
            {
                case BarResultType.BarForce:
                    return ReadBarForce(barIds, request.Divisions);
                case BarResultType.BarDisplacement:
                    return ReadBarDisplacements(barIds, request.Divisions);
                case BarResultType.BarDeformation:
                    Engine.Reflection.Compute.RecordError("SAP2000 cannot export localised BarDeformations." +
                    "To get the full displacement of the bars in global coordinates, try pulling BarDisplacements");
                    return new List<IResult>();
                case BarResultType.BarStrain:
                case BarResultType.BarStress:
                default:
                    Engine.Reflection.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }
        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<BarDisplacement> ReadBarDisplacements(List<string> barIds = null,
                                                           int divisions = 0)
        {
            List<BarDisplacement> displacements  = new List<BarDisplacement>();
            if (divisions != 0)
            {
                Engine.Reflection.Compute.RecordWarning("Displacements will only be extracted at SAP2000 calculation nodes." +
                                                    "'Divisions' parameter will not be considered in result extraction");
            }

            int resultCount = 0;
            string[] Obj = null;
            string[] Elm = null;
            string[] LoadCase = null;
            string[] StepType = null;
            double[] StepNum = null;

            double[] ux = null;
            double[] uy = null;
            double[] uz = null;
            double[] rx = null;
            double[] ry = null;
            double[] rz = null;


            for (int i = 0; i < barIds.Count; i++)
            {
                Dictionary<string, double> nodes = new Dictionary<string, double>();
                string[] intElems = null;
                double[] di = null;
                double[] dj = null;
                int div = 0;

                m_model.FrameObj.GetElm(barIds[i], ref div, ref intElems, ref di, ref dj);

                divisions = div + 1;

                string p1Id = "";
                string p2Id = "";

                //get first point
                m_model.LineElm.GetPoints(intElems[0], ref p1Id, ref p2Id);
                nodes[p1Id] = di[0];

                //get the rest of the points
                for (int j = 0; j < div; j++)
                {
                    m_model.LineElm.GetPoints(intElems[j], ref p1Id, ref p2Id);

                    nodes[p2Id] = dj[j];
                }

                foreach (var point in nodes)
                {
                    if (m_model.Results.JointDispl(point.Key,
                                                        eItemTypeElm.Element,
                                                        ref resultCount,
                                                        ref Obj,
                                                        ref Elm,
                                                        ref LoadCase,
                                                        ref StepType,
                                                        ref StepNum,
                                                        ref ux,
                                                        ref uy,
                                                        ref uz,
                                                        ref rx,
                                                        ref ry,
                                                        ref rz) != 0)
                    {
                        Engine.Reflection.Compute.RecordError($"Could not extract results for an output station in bar {barIds}.");
                    }
                    else
                    {
                        for (int j = 0; j < resultCount; j++)
                        {
                            BarDisplacement disp = new BarDisplacement(barIds[i], LoadCase[j], -1, StepNum[j], point.Value, divisions, ux[j], uy[j], uz[j], rx[j], ry[j], rz[j]);
                            displacements.Add(disp);
                        }
                    }

                }

            }

            return displacements;
        }

        /***************************************************/

        private List<BarForce> ReadBarForce(List<string> barIds, int divisions = 0)
        {
            List<BarForce> barForces = new List<BarForce>();
            if (divisions != 0)
            {
                Engine.Reflection.Compute.RecordWarning("Forces will only be extracted at SAP2000 calculation nodes." +
                                                    "'Divisions' parameter will not be considered in result extraction");
            }

            int resultCount = 0;
            string[] loadcaseNames = null;
            string[] objects = null;
            string[] elm = null;
            double[] objStation = null;
            double[] elmStation = null;
            double[] stepNum = null;
            string[] stepType = null;

            int div = 0;
            string[] intElems = null;
            double[] di = null;
            double[] dj = null;

            double[] p = null;
            double[] v2 = null;
            double[] v3 = null;
            double[] t = null;
            double[] m2 = null;
            double[] m3 = null;

            Dictionary<string, Point> points = new Dictionary<string, Point>();

            for (int i = 0; i < barIds.Count; i++)
            {
                //Get element length
                double length = GetBarLength(barIds[i], points);

                //get number of divisions
                m_model.FrameObj.GetElm(barIds[i], ref div, ref intElems, ref di, ref dj);
                divisions = div + 1;

                if (m_model.Results.FrameForce(barIds[i],
                                                     eItemTypeElm.ObjectElm,
                                                     ref resultCount,
                                                     ref objects,
                                                     ref objStation,
                                                     ref elm,
                                                     ref elmStation,
                                                     ref loadcaseNames,
                                                     ref stepType,
                                                     ref stepNum,
                                                     ref p,
                                                     ref v2,
                                                     ref v3,
                                                     ref t,
                                                     ref m2,
                                                     ref m3) != 0)
                {
                    Engine.Reflection.Compute.RecordError($"Could not extract results for an output station in bar {barIds}.");
                }
                else
                {
                    for (int j = 0; j < resultCount; j++)
                    {
                        BarForce bf = new BarForce(barIds[i], loadcaseNames[j], -1, stepNum[j], objStation[j] / length, divisions, p[j], v3[j], v2[j], t[j], -m3[j], m2[j]);
                        barForces.Add(bf);
                    }
                }
            }

            return barForces;
        }

        /***************************************************/

        private List<BarResult> ReadBarStrains(List<string> barIds = null, int divisions = 5)
        {
            throw new NotImplementedException("Bar strain results are not supported yet!");
        }

        /***************************************************/

        private List<BarResult> ReadBarStresses(List<string> barIds = null, int divisions = 5)
        {
            throw new NotImplementedException("Bar stress results are not supported yet!");
        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<string> CheckGetBarIds(IStructuralResultRequest request)
        {

            List<string> barIds = new List<string>();
            var ids = request.ObjectIds;

            if (ids == null || ids.Count == 0)
            {
                int bars = 0;
                string[] names = null;
                m_model.FrameObj.GetNameList(ref bars, ref names);
                barIds = names.ToList();
            }
            else
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    barIds.Add(ids[i].ToString());
                }
            }

            return barIds;
        }

        /***************************************************/

        private double GetBarLength(string barId, Dictionary<string, Point> pts)
        {
            string p1Id = "";
            string p2Id = "";

            m_model.FrameObj.GetPoints(barId, ref p1Id, ref p2Id);

            Point p1 = CheckGetPoint(p1Id, pts);
            Point p2 = CheckGetPoint(p2Id, pts);

            return p1.Distance(p2);
        }

        /***************************************************/

        private Point CheckGetPoint(string pointId, Dictionary<string, Point> pts)
        {
            Point pt;
            double x = 0;
            double y = 0;
            double z = 0;

            if (!pts.TryGetValue(pointId, out pt))
            {
                m_model.PointObj.GetCoordCartesian(pointId, ref x, ref y, ref z);
                pt = new Point() {X = x, Y = y, Z = z};
                pts[pointId] = pt;
            }
            return pt;
        }
    }
}

