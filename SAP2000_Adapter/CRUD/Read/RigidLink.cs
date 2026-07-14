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

using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;
using System;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<RigidLink> ReadRigidLink(List<string> ids = null)
        {
            List<RigidLink> linkList = new List<RigidLink>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => GetAdapterId<string>(x));
            Dictionary<string, LinkConstraint> bhomLinkConstraints = ReadLinkConstraints().ToDictionary(x => GetAdapterId<string>(x));


            //Read all links, filter by id at end, so that we can join multi-links.
            int nameCount = 0;
            string[] nameArr = { };
            m_model.LinkObj.GetNameList(ref nameCount, ref nameArr);



            foreach (string id in nameArr)
            {
                RigidLink newLink = new RigidLink();
                SAP2000Id sap2000id = new SAP2000Id();
                string guid = null;

                sap2000id.Id = id;

                try
                {
                    string primaryId = "";
                    string secondaryId = "";
                    string propName = "";
                    m_model.LinkObj.GetPoints(id, ref primaryId, ref secondaryId);
                    newLink.PrimaryNode = bhomNodes[primaryId];
                    newLink.SecondaryNodes = new List<Node> { bhomNodes[secondaryId] };

                    if (m_model.LinkObj.GetProperty(id, ref propName) == 0)
                    {
                        LinkConstraint bhProp = new LinkConstraint();
                        bhomLinkConstraints.TryGetValue(propName, out bhProp);
                        newLink.Constraint = bhProp; m_model.LinkObj.GetProperty(id, ref propName);
                    }
                    else
                    {
                        Engine.Base.Compute.RecordWarning("Could not get link property for RigidLink " + id + ".");
                    }
                    
                    // Get the groups the link is assigned to
                    int numGroups = 0;
                    string[] groupNames = new string[0];
                    if (m_model.LinkObj.GetGroupAssign(id, ref numGroups, ref groupNames) == 0)
                    {
                        foreach (string grpName in groupNames)
                            newLink.Tags.Add(grpName);
                    }

                    if (m_model.LinkObj.GetGUID(id, ref guid) == 0)
                        sap2000id.PersistentId = guid;

                    newLink.SetAdapterId(sap2000id);
                    linkList.Add(newLink);
                }

                catch
                {
                    ReadElementError("RigidLink", id.ToString());
                }
            }

            Dictionary<string, RigidLink> joinedLinks = BH.Engine.Adapters.SAP2000.Query.JoinRigidLink(linkList).ToDictionary(x => x.Name);

            ids = FilterIds(ids, joinedLinks.Keys);

            if (ids != null)
            {
                return joinedLinks
                     .Where(x => ids.Contains(x.Key))
                     .Select(x => x.Value).ToList();
            }
            else
            {
                return joinedLinks.Values.ToList();
            }
        }

        /***************************************************/
    }
}






