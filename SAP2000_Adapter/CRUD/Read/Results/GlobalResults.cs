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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Elements;
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

        public IEnumerable<IResult> ReadResults(GlobalResultRequest request, ActionConfig actionConfig = null)
        {
            CheckAndSetUpCases(request);

            switch (request.ResultType)
            {
                case GlobalResultType.Reactions:
                    return GetGlobalReactions();
                case GlobalResultType.ModalDynamics:
                case GlobalResultType.StoreyDrifts:
                default:
                    Engine.Base.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }
        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<GlobalReactions> GetGlobalReactions()
        {
            List<GlobalReactions> globalReactions = new List<GlobalReactions>();

            int resultCount = 0;
            string[] loadcaseNames = null;
            string[] stepType = null; double[] stepNum = null;
            double[] fx = null; double[] fy = null; double[] fz = null;
            double[] mx = null; double[] my = null; double[] mz = null;
            double gx = 0; double gy = 0; double gz = 0;

            m_model.Results.BaseReact(ref resultCount, ref loadcaseNames, ref stepType, ref stepNum, ref fx, ref fy, ref fz, ref mx, ref my, ref mz, ref gx, ref gy, ref gz);

            for (int i = 0; i < resultCount; i++)
            {
                int mode;
                double timeStep;
                GetStepAndMode(stepType[i], stepNum[i], out timeStep, out mode);

                GlobalReactions g = new GlobalReactions("", loadcaseNames[i], mode, timeStep, fx[i], fy[i], fz[i], mx[i], my[i], mz[i]);

                globalReactions.Add(g);
            }

            return globalReactions;
        }

        /***************************************************/

    }
}






