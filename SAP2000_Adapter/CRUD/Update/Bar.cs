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

using BH.Engine.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

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
                string id = bhBar.AdapterId<string>(typeof(SAP2000Id));
                if (id == null)
                {
                    Engine.Base.Compute.RecordWarning("The Bar must have a SAP2000 adapter id to be updated.");
                    continue;
                }

                if (!nameArr.Contains(id))
                {
                    Engine.Base.Compute.RecordWarning("The Bar must be present in SAP2000 to be updated");
                    continue;
                }

                // check Start and End node, which are not dealt with in SetObject
                // Check for dealbreaking BHoM validity
                if (bhBar.StartNode == null || bhBar.EndNode == null)
                {
                    Engine.Base.Compute.RecordError($"Bar {bhBar.Name} failed to update because its nodes are null");
                    return false;
                }

                // Retrieve original end points from SAP2000
                string sapBarI = "";
                string sapBarJ = "";

                if (m_model.FrameObj.GetPoints(id, ref sapBarI, ref sapBarJ) != 0)
                {
                    Engine.Base.Compute.RecordError($"Bar {bhBar.Name} failed to update because its nodes were not found in SAP2000. Check that geometry is valid.");
                    return false;
                }
                else
                {
                    bhBar.StartNode.SetAdapterId(new SAP2000Id() { Id = sapBarI });
                    bhBar.EndNode.SetAdapterId(new SAP2000Id() { Id = sapBarJ });
                    List<Node> barNodes = new List<Node>() { bhBar.StartNode, bhBar.EndNode };
                    UpdateObjects(barNodes);
                }

                // Set Properties
                SetObject(bhBar);

            }
            return success;
        }

        private bool UpdateBarPropAssigns(IEnumerable<Bar> bhBars)
        {
            bool success = true;
            m_model.SelectObj.ClearSelection();

            int nameCount = 0;
            string[] nameArr = { };
            m_model.FrameObj.GetNameList(ref nameCount, ref nameArr);

            foreach (Bar bhBar in bhBars)
            {
                object id = bhBar.AdapterId<string>(typeof(SAP2000Id));
                if (id == null)
                {
                    Engine.Base.Compute.RecordWarning("The Bar must have a SAP2000 adapter id to be updated.");
                    continue;
                }

                string name = id as string;
                if (!nameArr.Contains(name))
                {
                    Engine.Base.Compute.RecordWarning("The Bar must be present in SAP2000 to be updated");
                    continue;
                }

                // Set Properties
                SetSectionProperty(bhBar, name);

            }
            return success;
        }
    }
}
