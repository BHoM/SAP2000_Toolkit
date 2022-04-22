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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Serialiser;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000.Results;
using BH.oM.Adapters.SAP2000.Requests;
using SAP2000v1;
using BH.Engine.Structure.Design.AISC15;
using BH.Engine.Spatial;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IObject> ReadResults(BarForceTimeHistoryRequest request,
                                                ActionConfig actionConfig = null)
        {
            // Don't pull all results just because some numbskull forgot to put an input here. This will take a very long time.
            if (request.ObjectIds.Count == 0 || request.Cases.Count == 0)
            {
                return null;
            }

            List<string> barIds = CheckGetBarIds(request);

            if (barIds.Count == 0)
            {
                Engine.Base.Compute.RecordError("Could not find any of these bars in the model.");
                return null;
            }

            BarResultRequest gravityRequest = new BarResultRequest() { Cases = request.GravityCases, Modes = request.Modes, ObjectIds = request.ObjectIds, ResultType = BarResultType.BarForce };
            CheckAndSetUpCases(gravityRequest);
            List<BarForce> gravityForces = ReadBarForce(barIds);
            Dictionary<string, BarForce> gravityDict = Engine.Results.Query.AbsoluteMaxEnvelopeByObject(gravityForces).ToDictionary(x => x.ObjectId.ToString());

            BarResultRequest nonSeismicRequest = new BarResultRequest() { Cases = request.NonSeismicCases(), Modes = request.Modes, ObjectIds = request.ObjectIds, ResultType = BarResultType.BarForce };
            CheckAndSetUpCases(nonSeismicRequest);
            IEnumerable<BarForce> nonSeismicForces = ReadBarForce(barIds);

            Dictionary<string, Dictionary<string, double>> barCapacities = new Dictionary<string, Dictionary<string, double>>();
            IEnumerable<Bar> bars = ReadBars(barIds);
            foreach (Bar bar in bars)
            {
                SteelSection sec = bar.SectionProperty as SteelSection;
                double len = bar.Length();
                Dictionary<string, double> barCapacity = Engine.Structure.Design.AISC15.Compute.SteelBoxSectionStrength(sec, len);
                barCapacities.Add(GetAdapterId<string>(bar), barCapacity);
            }

            CheckAndSetUpCases(request);
            return ReadBarForceTimeHistory(barIds, gravityDict, nonSeismicForces, request, barCapacities);

        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/
        private List<AISCSteelUtilisation> ReadBarForceTimeHistory
            (
            List<string> barIds,
            Dictionary<string, BarForce> gravityDict,
            IEnumerable<BarForce> nonSeismicForces,
            BarForceTimeHistoryRequest request, 
            Dictionary<string, Dictionary<string, double>> barCapacities
            )
        {
            string path = Path.Combine(m_model.GetModelFilepath(), "barForceBhomExport.json");

            List<AISCSteelUtilisation> barUtilizations = new List<AISCSteelUtilisation>();

            int divisions = 0;
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
                    Engine.Base.Compute.RecordError($"Could not extract results for an output station in bar {barIds[i]}. Stopping further calculations.");
                    return barUtilizations;
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

                    List<AISCSteelUtilisation> thisBarUtilizations = SumForces(forceItems, gravityDict, nonSeismicForces, request, barCapacities);

                    //Output the forces to a file
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        foreach (AISCSteelUtilisation util in thisBarUtilizations)
                        {
                            sw.WriteLine(util.ToJson());
                        }
                    }
                    
                    barUtilizations.AddRange(thisBarUtilizations);

                }
            }

            return barUtilizations;
        }

        /***************************************************/

        private static List<AISCSteelUtilisation> SumForces(List<BarForce> barForces, Dictionary<string, BarForce> gravityDict, IEnumerable<BarForce> nonSeismicForces, BarForceTimeHistoryRequest request, Dictionary<string, Dictionary<string, double>> barCapacities)

        {
            List<AISCSteelUtilisation> result = new List<AISCSteelUtilisation>();

            var groupById = barForces.GroupBy(x => x.ObjectId);

            foreach (var forceGroup in groupById)
            {

                Dictionary<string, double> barCapacity = new Dictionary<string, double>();

                if (!barCapacities.TryGetValue(forceGroup.FirstOrDefault().ObjectId.ToString(), out barCapacity))
                {
                    Engine.Base.Compute.RecordError("Could not find bar capacity, stopping further calculations.");
                    return result;
                }

                //Dictionary<string, double> barCapacity = new Dictionary<string, double>()
                //{
                //    {"C"  , -1},
                //    {"T"  , 1},
                //    {"Vy" , 1},
                //    {"Vz" , 1},
                //    {"Mx" , 1},
                //    {"My" , 1},
                //    {"Mz" , 1}
                //};

                string id = forceGroup.FirstOrDefault().ObjectId.ToString();
                int modeNumber = forceGroup.FirstOrDefault().ModeNumber;
                int divisions = forceGroup.FirstOrDefault().Divisions; //ignore divisions - not relevant

                //These will get assignments after grouping
                double position = double.NaN;
                AISCSteelUtilisation maxUtilization;

                IEnumerable<BarForce> nonSeismicForceAtBar = nonSeismicForces.Where(x => x.ObjectId.ToString() == id);

                //Group by position (do summation at each end)
                var groupbyposition = forceGroup.GroupBy(x => x.Position);

                foreach (var group in groupbyposition)
                {
                    position = group.First().Position;
                    IEnumerable<BarForce> nonSeismicForceAtPosition = nonSeismicForceAtBar.Where(x => x.Position == position);

                    maxUtilization = group.Max(x => CombineResults(x, barCapacity, gravityDict[id], nonSeismicForceAtPosition, request));

                    result.Add(maxUtilization);
                }
            }

            return result;
        }

        /***************************************************/

        private static AISCSteelUtilisation CombineResults(BarForce timeHistory, Dictionary<string, double> capacity, BarForce gravityDemand, IEnumerable<BarForce> nonSeismicForce, BarForceTimeHistoryRequest request)
        {
            BarForce dead = nonSeismicForce.Where(x => x.ResultCase.ToString() == request.NonSeismicCases()[0].ToString()).FirstOrDefault();
            BarForce live = nonSeismicForce.Where(x => x.ResultCase.ToString() == request.NonSeismicCases()[1].ToString()).FirstOrDefault();
            BarForce tempPlus = nonSeismicForce.Where(x => x.ResultCase.ToString() == request.NonSeismicCases()[2].ToString()).FirstOrDefault();
            BarForce tempMinus = nonSeismicForce.Where(x => x.ResultCase.ToString() == request.NonSeismicCases()[3].ToString()).FirstOrDefault();

            double factor = request.Factor;
            List<BarForce> combos = new List<BarForce>
            {
                Add(Add(Multiply(dead, 1.2), Multiply(Subtract(timeHistory, dead), factor)), Add(Multiply(live, 0.2), tempPlus)),
                Add(Add(Multiply(dead, 1.2), Multiply(Subtract(timeHistory, dead), factor)), Add(Multiply(live, 0.2), tempMinus)),
                Add(Add(Multiply(dead, 0.9), Multiply(Subtract(timeHistory, dead), factor)), tempPlus),
                Add(Add(Multiply(dead, 0.9), Multiply(Subtract(timeHistory, dead), factor)), tempMinus)
            };

            BarForce absMax = Engine.Results.Query.MaxEnvelope(combos);
            BarForce max = Engine.Results.Query.MinEnvelope(combos);
            BarForce min = Engine.Results.Query.MinEnvelope(combos);

            double tensionCompressionRatio = Math.Max(max.FX / capacity["T"], min.FX / capacity["C"]);
            double majorShearRatio = absMax.FZ / capacity["Vz"];
            double minorShearRatio = absMax.FY / capacity["Vy"];
            double torsionRatio = absMax.MX / capacity["Mx"];
            double majorBendingRatio = absMax.MY / capacity["My"];
            double minorBendingRatio = absMax.MZ / capacity["Mz"];

            double totalRatio;

            if (torsionRatio <= 0.20)
            {
                if (tensionCompressionRatio >= 0.20)
                    totalRatio = tensionCompressionRatio + (8 / 9) * (majorBendingRatio + minorBendingRatio); //H1-1a
                else
                    totalRatio = tensionCompressionRatio/2 + (majorBendingRatio + minorBendingRatio); //H1-1b
            }
            else
                totalRatio = tensionCompressionRatio + majorBendingRatio + minorBendingRatio + Math.Pow(majorShearRatio + minorShearRatio + torsionRatio, 2); //H1-1b
            
            return new AISCSteelUtilisation(timeHistory.ObjectId, timeHistory.ResultCase, timeHistory.ModeNumber, timeHistory.TimeStep, timeHistory.Position, timeHistory.Divisions, "AISC15", "NA", "NA", "designType", totalRatio, tensionCompressionRatio, majorShearRatio, minorShearRatio, torsionRatio, majorBendingRatio, minorBendingRatio);
        }

        /***************************************************/

        public static BarForce Subtract(BarForce a, BarForce b)
        {
            if (a == null || b == null)
                return null;
            return new BarForce(a.ObjectId, a.ResultCase, a.ModeNumber, a.TimeStep, a.Position, a.Divisions, a.FX - b.FX, a.FY - b.FY, a.FZ - b.FZ, a.MX - b.MX, a.MY - b.MY, a.MZ - b.MZ);
        }

        /***************************************************/

        public static BarForce Add(BarForce a, BarForce b)
        {
            if (a == null || b == null)
                return null;
            return new BarForce(a.ObjectId, a.ResultCase, a.ModeNumber, a.TimeStep, a.Position, a.Divisions, a.FX + b.FX, a.FY + b.FY, a.FZ + b.FZ, a.MX + b.MX, a.MY + b.MY, a.MZ + b.MZ);
        }


        public static BarForce Multiply(BarForce a, double b)
        {
            if (a == null)
                return null;
            return new BarForce(a.ObjectId, a.ResultCase, a.ModeNumber, a.TimeStep, a.Position, a.Divisions, a.FX * b, a.FY * b, a.FZ * b, a.MX * b, a.MY * b, a.MZ * b);
        }

        /***************************************************/

    }
}


