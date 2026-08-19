/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Analytical.Results;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Requests;
using BH.oM.Structure.Results;
using SAP2000v1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(MeshResultRequest request, ActionConfig actionConfig = null)
        {
            List<string> cases = GetAllCases(request.Cases);
            CheckAndSetUpCases(request);
            List<string> panelIds = CheckGetPanelIds(request);

            switch (request.ResultType)
            {
                case MeshResultType.Forces:
                    return ReadMeshForce(panelIds, request.Smoothing);
                case MeshResultType.Displacements:
                case MeshResultType.Stresses:
                case MeshResultType.VonMises:
                default:
                    Engine.Base.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }

        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/


        private List<MeshResult> ReadMeshForce(List<string> panelIds, MeshResultSmoothingType smoothing)
        {
            switch (smoothing)
            {
                case MeshResultSmoothingType.BySelection:
                case MeshResultSmoothingType.Global:
                case MeshResultSmoothingType.ByFiniteElementCentres:
                    Engine.Base.Compute.RecordWarning("Smoothing type not supported for MeshForce. No results extracted");
                    return new List<MeshResult>();
            }

            eItemTypeElm itemTypeElm = eItemTypeElm.ObjectElm;
            int resultCount = 0;
            string[] obj = null;
            string[] elm = null;
            string[] pointElm = null;
            string[] loadCase = null;
            string[] stepType = null;
            double[] stepNum = null;
            double[] f11 = null;
            double[] f22 = null;
            double[] f12 = null;
            double[] fMax = null;
            double[] fMin = null;
            double[] fAngle = null;
            double[] fvm = null;
            double[] m11 = null;
            double[] m22 = null;
            double[] m12 = null;
            double[] mMax = null;
            double[] mMin = null;
            double[] mAngle = null;
            double[] v13 = null;
            double[] v23 = null;
            double[] vMax = null;
            double[] vAngle = null;

            List<MeshResult> results = new List<MeshResult>();

            if (smoothing == MeshResultSmoothingType.ByPanel)
                Engine.Base.Compute.RecordWarning("Force values have been smoothed outside the API by averaging all force values in each node");

            for (int i = 0; i < panelIds.Count; i++)
            {
                List<MeshForce> forces = new List<MeshForce>();

                int ret = m_model.Results.AreaForceShell(panelIds[i], itemTypeElm, ref resultCount, ref obj, ref elm,
                    ref pointElm, ref loadCase, ref stepType, ref stepNum, ref f11, ref f22, ref f12, ref fMax, ref fMin, ref fAngle, ref fvm,
                    ref m11, ref m22, ref m12, ref mMax, ref mMin, ref mAngle, ref v13, ref v23, ref vMax, ref vAngle);

                for (int j = 0; j < resultCount; j++)
                {
                    int mode;
                    double timeStep;

                    if (stepType[j] == "Single Value" || stepNum.Length < j)
                    {
                        mode = 0;
                        timeStep = 0;
                    }
                    else
                        GetStepAndMode(stepType[j], stepNum[j], out timeStep, out mode);

                    MeshForce pf = new MeshForce(panelIds[i], pointElm[j], elm[j], loadCase[j], mode, timeStep, 0, 0, 0,
                        oM.Geometry.Basis.XY, f11[j], f22[j], f12[j], m11[j], m22[j], m12[j], v13[j], v23[j]);

                    forces.Add(pf);
                }

                if (smoothing == MeshResultSmoothingType.ByPanel)
                    forces = SmoothenForces(forces);

                results.AddRange(GroupMeshResults(forces));
            }

            return results;
        }


        /***************************************************/

        
        private List<MeshResults> ReadMeshDisplacement(List<string> panelIds,
                                                       MeshResultSmoothingType smoothing)
        {
            int resultCount = 0;
            string[] obj = null;
            string[] elm = null;
            string[] loadCase = null;
            string[] stepType = null;
            double[] stepNum = null;

            double[] ux = null;
            double[] uy = null;
            double[] uz = null;
            double[] rx = null;
            double[] ry = null;
            double[] rz = null;

            List<MeshResult> results = new List<MeshResult>();

            for (int i = 0; i < panelIds.Count; i++)
            {
                List<MeshDisplacement> displacements = new List<MeshDisplacement>();

                HashSet<string> ptNbs = new HashSet<string>();

                int nbElem = 0;
                string[] elemNames = new string[0];
                m_model.AreaObj.GetElm(panelIds[i], ref nbElem, ref elemNames);

                for (int j = 0; j < nbElem; j++)
                {
                    int nbPts = 0;
                    string[] ptsNames = new string[0];
                    m_model.AreaElm.GetPoints(elemNames[j], ref nbPts, ref ptsNames);

                    foreach (string ptId in ptsNames);
                    {
                        ptNbs.Add(ptId);
                    }
                }

                foreach (string ptId in ptNbs)
                {
                    int ret = m_model.Results.JointDispl(ptId,
                                                         eItemTypeElm.Element,
                                                         ref resultCount,
                                                         ref obj,
                                                         ref elm,
                                                         ref loadCase,
                                                         ref stepType,
                                                         ref stepNum,
                                                         ref ux,
                                                         ref uy,
                                                         ref uz,
                                                         ref rx,
                                                         ref ry,
                                                         ref fz);

                    for (int j = 0; j < resultCount; j++)
                    {
                        MeshDisplacement disp = new MeshDisplacement(panelIds[i],
                                                                     ptId,
                                                                     "",
                                                                     loadCase[j],
                                                                     stepNum[j],
                                                                     MeshResultLayer.Middle,
                                                                     0,
                                                                     MeshResultSmoothingType.Global,
                                                                     Basis.Xy,
                                                                     ux[j],
                                                                     uy[j],
                                                                     uz[j],
                                                                     rx[j],
                                                                     ry[j],
                                                                     rz[j]);
                    }
                }
                results.AddRange(GroupMeshResults(displacements));
            }
            return results
        }

        /***************************************************/
        /*
        private List<MeshResults> ReadMeshStress(List<string> panelIds, List<string> cases,
                                                 MeshResultSmoothingType smoothing, MeshResultLayer layer)
        {
            return results
        }

       
        /***************************************************/
        /**** Private methods - Support methods         ****/
        /***************************************************/

        private List<MeshForce> SmoothenForces(List<MeshForce> forces)
        {
            List<MeshForce> smoothenedForces = new List<MeshForce>();

            foreach (IEnumerable<MeshForce> group in forces.GroupBy(x => new { x.ResultCase, x.TimeStep, x.NodeId }))
            {
                MeshForce first = group.First();

                double nxx = group.Average(x => x.NXX);
                double nyy = group.Average(x => x.NYY);
                double nxy = group.Average(x => x.NXY);

                double mxx = group.Average(x => x.MXX);
                double myy = group.Average(x => x.MYY);
                double mxy = group.Average(x => x.MXY);

                double vx = group.Average(x => x.VX);
                double vy = group.Average(x => x.VY);

                smoothenedForces.Add(new MeshForce(first.ObjectId, first.NodeId, "", first.ResultCase, first.ModeNumber, first.TimeStep, first.MeshResultLayer, first.LayerPosition, MeshResultSmoothingType.ByPanel, first.Orientation, nxx, nyy, nxy, mxx, myy, mxy, vx, vy));
            }

            return smoothenedForces;
        }






