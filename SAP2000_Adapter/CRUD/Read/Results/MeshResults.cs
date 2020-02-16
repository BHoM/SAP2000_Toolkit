/*
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Results;
using BH.oM.Common;
using BH.oM.Structure.Elements;
using BH.Engine.SAP2000;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Requests;
using BH.oM.Adapters.SAP2000;
using BH.oM.Geometry;
using BH.Engine.Geometry;
/*using SAP2000v19;*/
using BH.oM.Adapter;
using SAP2000v1;


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
            CheckAndSetUpCases(request)
            List<string> panelIds = CheckGetPanelIds(request);

            switch (request.ResultType)
            {
                case MeshResultType.Forces:
                    return ReadMeshForce(panelIds, request.smoothing)
                case MeshResultType.Displacements:
                case MeshResultType.Stresses:
                case MeshResultType.VonMises:
                default:
                    Engine.Reflection.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }
        }

        /***************************************************/
        /**** Private methods - Extraction methods      ****/
        /***************************************************/

        private List<MeshResults> ReadMeshForce(List<string> panelIds, MeshResultSmoothingType smoothing)
        {
            return meshResults;
        }

        /***************************************************/



        /***************************************************/



        /***************************************************/


        /***************************************************/
        /**** Private methods - Support methods         ****/
        /***************************************************/
 
    }
}
