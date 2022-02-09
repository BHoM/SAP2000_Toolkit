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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000.Results;
using BH.oM.Adapters.SAP2000.Requests;
using SAP2000v1;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IObject> ReadResults(BarForceSummationRequest request,
                                                ActionConfig actionConfig = null)
        {
            CheckAndSetUpCases(request);
            List<string> barIds = CheckGetBarIds(request);

            return ReadBarForceSummation(barIds);

        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/
        private List<BarForce> ReadBarForceSummation(List<string> barIds, int divisions = 0)
        {
            List<BarForce> barForces = new List<BarForce>();
            if (divisions != 0)
            {
                Engine.Base.Compute.RecordWarning("Forces will only be extracted at SAP2000 calculation nodes." +
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

            double[] p = null;
            double[] v2 = null;
            double[] v3 = null;
            double[] t = null;
            double[] m2 = null;
            double[] m3 = null;

            Dictionary<string, Point> points = ReadNodes().ToDictionary(x => GetAdapterId<string>(x), y => y.Position);

            for (int i = 0; i < barIds.Count; i++)
            {
                //Get element length
                double length = GetBarLength(barIds[i], points);

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
                    Engine.Base.Compute.RecordError($"Could not extract results for an output station in bar {barIds}.");
                }
                else
                {
                    divisions = objStation.ToHashSet().Count(); //Get unique values of objStation, which is equal to the divisions set by SAP.

                    List<BarForce> forceItems = new List<BarForce>();

                    for (int j = 0; j < resultCount; j++)
                    {
                        BarForce bf = new BarForce(barIds[i], loadcaseNames[j], -1, stepNum[j], objStation[j] / length, divisions, p[j], v3[j], v2[j], t[j], -m3[j], m2[j]);
                        forceItems.Add(bf);
                    }

                    List<BarForce> processedForces = SumForces(forceItems);
                    barForces.AddRange(processedForces);
                    RecordForces(processedForces);
                }
            }

            return barForces;
        }

        /***************************************************/

        private static List<BarForce> SumForces(List<BarForce> barForces)
        {
            List<BarForce> summedForces = new List<BarForce>();

            var groupById = barForces.GroupBy(x => x.ObjectId);

            foreach (var forceGroup in groupById)
            {
                var id = forceGroup.FirstOrDefault().ObjectId;
                string resultCase = "summarized results"; // these are collapsing
                int modeNumber = forceGroup.FirstOrDefault().ModeNumber;
                double timeStep = 0; //ignore timestep information - these are collapsing
                int divisions = forceGroup.FirstOrDefault().Divisions; //ignore divisions - not relevant

                //These will get assignments after grouping
                double position = double.NaN;
                double fx = double.NaN;
                double fy = double.NaN;
                double fz = double.NaN;
                double mx = double.NaN;
                double my = double.NaN;
                double mz = double.NaN;

                //Group by position (do summation at each end)
                var groupbyposition = forceGroup.GroupBy(x => x.Position);

                foreach (var group in groupbyposition)
                {
                    position = group.First().Position;
                    fx = group.Sum(x => x.FX);
                    fy = group.Sum(x => x.FY);
                    fz = group.Sum(x => x.FZ);
                    mx = group.Sum(x => x.MX);
                    my = group.Sum(x => x.MY);
                    mz = group.Sum(x => x.MZ);
                }

                BarForce sumForce = new BarForce(id, resultCase, modeNumber, timeStep, position, divisions, fx, fy, fz, mx, my, mz);

                summedForces.Add(sumForce);
            }

            return summedForces;
        }

        /***************************************************/

        private static void RecordForces(List<BarForce> barForces)
        {
            //Write the current force to a file
        }

        /***************************************************/
    }
}


