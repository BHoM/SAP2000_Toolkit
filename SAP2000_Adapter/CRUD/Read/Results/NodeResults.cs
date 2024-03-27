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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Adapter;
using SAP2000v1;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(NodeResultRequest request,
                                                ActionConfig actionConfig = null)
        {
            CheckAndSetUpCases(request);
            List<string> nodeIds = CheckGetNodeIds(request);

            switch (request.ResultType)
            {
                case NodeResultType.NodeReaction:
                    return ReadNodeReaction(nodeIds);
                case NodeResultType.NodeDisplacement:
                    return ReadNodeDisplacement(nodeIds);
                case NodeResultType.NodeAcceleration:
                case NodeResultType.NodeVelocity:
                default:
                    Engine.Base.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }
        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<NodeResult> ReadNodeAcceleration(IList ids = null,
                                                      IList cases = null)
        {
            throw new NotImplementedException("Node Acceleration results is not supported yet!");

        }

        /***************************************************/
        private List<NodeDisplacement> ReadNodeDisplacement(List<string> nodeIds)
        {

            List<NodeDisplacement> nodeDisplacements = new List<NodeDisplacement>();

            int resultCount = 0;
            string[] loadcaseNames = null;
            string[] objects = null;
            string[] elm = null;
            string[] stepType = null;
            double[] stepNum = null;

            double[] ux = null;
            double[] uy = null;
            double[] uz = null;
            double[] rx = null;
            double[] ry = null;
            double[] rz = null;

            for (int i = 0; i < nodeIds.Count; i++)
            {
                int ret = m_model.Results.JointDispl(nodeIds[i].ToString(),
                                                     eItemTypeElm.ObjectElm,
                                                     ref resultCount,
                                                     ref objects,
                                                     ref elm,
                                                     ref loadcaseNames,
                                                     ref stepType,
                                                     ref stepNum,
                                                     ref ux,
                                                     ref uy,
                                                     ref uz,
                                                     ref rx,
                                                     ref ry,
                                                     ref rz);
                if (ret == 0)
                {
                    for (int j = 0; j < resultCount; j++)
                    {
                        NodeDisplacement nd = new NodeDisplacement(nodeIds[i], loadcaseNames[j], -1, stepNum[j], oM.Geometry.Basis.XY, ux[j], uy[j], uz[j], rx[j], ry[j], rz[j]);
                        nodeDisplacements.Add(nd);
                    }
                }
            }
            return nodeDisplacements;
        }

        /***************************************************/

        private List<NodeReaction> ReadNodeReaction(List<string> nodeIds)
        {
            List<NodeReaction> nodeReactions = new List<NodeReaction>();

            int resultCount = 0;
            string[] loadcaseNames = null;
            string[] objects = null;
            string[] elm = null;
            string[] stepType = null;
            double[] stepNum = null;

            double[] fx = null;
            double[] fy = null;
            double[] fz = null;
            double[] mx = null;
            double[] my = null;
            double[] mz = null;

            for (int i = 0; i < nodeIds.Count; i++)
            {
                int ret = m_model.Results.JointReact(nodeIds[i],
                                                     eItemTypeElm.ObjectElm,
                                                     ref resultCount,
                                                     ref objects,
                                                     ref elm,
                                                     ref loadcaseNames,
                                                     ref stepType,
                                                     ref stepNum,
                                                     ref fx,
                                                     ref fy,
                                                     ref fz,
                                                     ref mx,
                                                     ref my,
                                                     ref mz);
                if (ret == 0)
                {
                    for (int j = 0; j < resultCount; j++)
                    {
                        NodeReaction nr = new NodeReaction(nodeIds[i], loadcaseNames[j], -1, stepNum[j], oM.Geometry.Basis.XY, fx[j], fy[j], fz[j], mx[j], my[j], mz[j]);
                        nodeReactions.Add(nr);
                    }
                }
            }

            return nodeReactions;
        }

        /***************************************************/

        
        private List<NodeResult> ReadNodeVelocity(IList ids = null,
                                                  IList cases = null)
        {
            throw new NotImplementedException("Node velocity results are not supported yet!");

        }
        /***************************************************/
        /**** Private method - Support methods          ****/
        /***************************************************/


    }
}





