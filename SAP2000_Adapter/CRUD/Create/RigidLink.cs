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

using BH.Engine.Adapter;
using BH.oM.Adapters.SAP2000;
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
            List<RigidLink> subLinks = BH.Engine.Adapters.SAP2000.Query.SplitRigidLink(bhLink);
            List<string> linkIds = new List<string>();
            SAP2000Id sap2000id = new SAP2000Id();

            if (subLinks.Count > 1)
                Engine.Reflection.Compute.RecordNote($"The RigidLink {bhLink.Name} was split into {subLinks.Count} separate links. They will be added to a new group called \"BHoM_Link_{bhLink.Name}\"");

            foreach (RigidLink subLink in subLinks)
            {
                string name = "";

                string primaryNode = GetAdapterId<string>(subLink.PrimaryNode);
                string secondaryNode = GetAdapterId<string>(subLink.SecondaryNodes[0]);


                if (m_model.LinkObj.AddByPoint(primaryNode, secondaryNode, ref name, false, "Default", subLink.Name) == 0)
                {
                    //Check if SAP respected the link name.
                    if (subLink.Name != "" && subLink.Name != name)
                        Engine.Reflection.Compute.RecordNote($"RigidLink {bhLink.Name} was assigned SAP2000_id of {name}");
                    
                    //Attempt to set property (if property has been pushed)
                    if(subLink.Constraint != null)
                    {
                        if (m_model.LinkObj.SetProperty(name, GetAdapterId<string>(subLink.Constraint)) != 0)
                            CreatePropertyWarning("LinkConstraint", "RigidLink", bhLink.Name);
                    }
                    else
                        CreatePropertyWarning("LinkConstraint", "RigidLink", bhLink.Name);                   


                    //Add to groups per tags. For links that have been split, the original name will be tagged
                    foreach (string gName in subLink.Tags)
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
                else
                {
                    //The sublink had a problem in SAP. the sublink property has not been set and the sublink was not added to a group. The sublink may or may not have been created.
                    CreateElementError("RigidLink", subLink.Name);
                }
            }

            sap2000id.Id = linkIds;
            bhLink.SetAdapterId(sap2000id);

            return true;
        }

        /***************************************************/
    }
}

