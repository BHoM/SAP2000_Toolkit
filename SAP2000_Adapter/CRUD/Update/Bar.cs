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

using System.Collections.Generic;
using System.Linq;
using BH.Engine.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.Engine.Adapters.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Bar                                ****/
        /***************************************************/
        private bool UpdateObjects(IEnumerable<Bar> bhBars)
        {
            bool success = true;
            m_model.SelectObj.ClearSelection();

            int nameCount = 0;
            string[] nameArr = { };
            m_model.FrameObj.GetNameList(ref nameCount, ref nameArr);

            foreach (Bar bhBar in bhBars)
            {
                object id = bhBar.AdapterId(typeof(SAP2000Id));
                if (id == null)
                {
                    Engine.Reflection.Compute.RecordWarning("The Bar must have a SAP2000 adapter id to be updated.");
                    continue;
                }

                string name = id as string;
                if (!nameArr.Contains(name))
                {
                    Engine.Reflection.Compute.RecordWarning("The Bar must be present in SAP2000 to be updated");
                    continue;
                }

                // check Start and End node, which are not dealt with in SetObject
                // Check for dealbreaking BHoM validity
                //if (bhBar.StartNode == null || bhBar.EndNode == null)
                //{
                //    Engine.Reflection.Compute.RecordError($"Bar {bhBar.Name} failed to update because its nodes are null");
                //    return false;
                //}

                //string startId = GetAdapterId<string>(bhBar.StartNode);
                //string endId = GetAdapterId<string>(bhBar.EndNode);

                //if (startId == null || endId == null)
                //{
                //    Engine.Reflection.Compute.RecordError($"Bar {bhBar.Name} failed to update because its nodes were not found in SAP2000. Check that geometry is valid.");
                //    return false;
                //}

                //Node[] barNodes = new Node[2];
                //barNodes[0] = bhBar.StartNode;
                //barNodes[1] = bhBar.EndNode;
                //UpdateObjects(barNodes);
                SetObject(bhBar);

            }
            m_model.View.RefreshView();
            return success;
        }
    }
}