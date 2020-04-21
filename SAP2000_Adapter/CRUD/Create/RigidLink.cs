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

using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(RigidLink bhLink)
        {
            List < RigidLink> bhomLinks = BH.Engine.SAP2000.Query.SplitRigidLink(bhLink);
            List<string> linkIds = null;

            foreach (RigidLink link in bhomLinks)
            {
                string name = "";
                Node masterNode = link.MasterNode;
                Node slaveNode = link.SlaveNodes[0];

                if ( m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterIdName].ToString(), 
                    slaveNode.CustomData[AdapterIdName].ToString(), ref name, false, "Default") != 0)
                {
                    CreateElementError("RigidLink", name);
                }

                foreach (string gName in bhLink.Tags)
                {
                    string groupName = gName.ToString();
                    if (m_model.LinkObj.SetGroupAssign(name, groupName) != 0)
                    {
                        m_model.GroupDef.SetGroup(groupName);
                        m_model.LinkObj.SetGroupAssign(name, groupName);
                    }
                }

                linkIds.Add(name);
            }

            bhLink.CustomData[AdapterIdName] = linkIds;

            return true;
        }

        /***************************************************/
    }
}
