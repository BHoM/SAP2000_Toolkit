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
            string[] names = { };
            m_model.LinkObj.GetNameList(ref nameCount, ref names);
            
            foreach (string name in names)
            {
                RigidLink newLink = new RigidLink();
                SAP2000Id sap2000id = new SAP2000Id();
                string guid = null;

                sap2000id.Id = name;

                string primaryId = "";
                string secondaryId = "";
                string propName = "";
                m_model.LinkObj.GetPoints(name, ref primaryId, ref secondaryId);
                newLink.PrimaryNode = bhomNodes[primaryId];
                newLink.SecondaryNodes = new List<Node> { bhomNodes[secondaryId] };

                m_model.LinkObj.GetProperty(name, ref propName);
                LinkConstraint bhProp = new LinkConstraint();
                bhomLinkConstraints.TryGetValue(propName, out bhProp);
                newLink.Constraint = bhProp;

                // Get the groups the link is assigned to
                int numGroups = 0;
                string[] groupNames = new string[0];
                if (m_model.LinkObj.GetGroupAssign(name, ref numGroups, ref groupNames) == 0)
                {
                    foreach (string grpName in groupNames)
                        newLink.Tags.Add(grpName);
                }

                if (m_model.LinkObj.GetGUID(name, ref guid) == 0)
                    sap2000id.PersistentId = guid;

                newLink.SetAdapterId(sap2000id);
                linkList.Add(newLink);
            }

            List<RigidLink> joinedList = BH.Engine.Adapters.SAP2000.Query.JoinRigidLink(linkList);

            if (ids != null)
                return joinedList.Where(x => ids.Contains(x.Name)).ToList();
            else
                return joinedList;

        }

        /***************************************************/
    }
}

