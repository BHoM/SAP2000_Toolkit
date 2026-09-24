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

            int nameCount = 0;
            string[] names = { };
            m_model.LinkObj.GetNameList(ref nameCount, ref names);

            ids = FilterIds(ids, names);

            //read primary-multiSecondary nodes if these were initially created from (non-etabs)BHoM side
            Dictionary<string, List<string>> idDict = new Dictionary<string, List<string>>();
            string[] primarySecondaryId;

            foreach (string id in ids)
            {
                primarySecondaryId = id.Split(new[] { ":::" }, StringSplitOptions.None);
                if (primarySecondaryId.Count() > 1)
                {
                    //has plural secondaries
                    if (idDict.ContainsKey(primarySecondaryId[0]))
                        idDict[primarySecondaryId[0]].Add(primarySecondaryId[1]);
                    else
                        idDict.Add(primarySecondaryId[0], new List<string>() { primarySecondaryId[1] });
                }
                else
                {
                    //normal single link
                    idDict.Add(id, null);
                }
            }


            foreach (KeyValuePair<string, List<string>> kvp in idDict)
            {
                string bhomName = GetBhomNameFromSAP2000Id(kvp.Key);

                RigidLink bhLink = new RigidLink() { Name = bhomName };

                SetAdapterId(bhLink, kvp.Key);

                if (kvp.Value == null)
                {
                    string startId = "";
                    string endId = "";
                    m_model.LinkObj.GetPoints(kvp.Key, ref startId, ref endId);

                    //Dummy nodes with correct Id
                    bhLink.PrimaryNode = new Node { Name = startId };
                    bhLink.SecondaryNodes = new List<Node>() { new Node { Name = endId } };
                }
                else
                {
                    string startId = "";
                    string endId = "";
                    string multiLinkId = kvp.Key + ":::0";


                    m_model.LinkObj.GetPoints(multiLinkId, ref startId, ref endId);
                    bhLink.PrimaryNode = new Node { Name = startId };   //Dummy startnode with correct Id

                    List<string> endIds = new List<string>();
                    for (int i = 1; i < kvp.Value.Count(); i++)
                    {
                        multiLinkId = kvp.Key + ":::" + i;
                        m_model.LinkObj.GetPoints(multiLinkId, ref startId, ref endId);
                        endIds.Add(endId);
                    }

                    bhLink.SecondaryNodes = endIds.Select(x => new Node { Name = x }).ToList(); //Dummy endnodes with correct Id
                }
                string propName = "";
                m_model.LinkObj.GetProperty(kvp.Key, ref propName);

                bhLink.Constraint = new LinkConstraint { Name = propName }; //Dummy constraint to be populated in later loop

                /* Get the ETABS name of the Rigid Link */
                string name = GetAdapterId<string>(bhLink);


                // Get the groups the link is assigned to
                int numGroups = 0;
                string[] groupNames = new string[0];
                if (m_model.LinkObj.GetGroupAssign(name, ref numGroups, ref groupNames) == 0)
                {
                    foreach (string grpName in groupNames)
                        bhLink.Tags.Add(grpName);
                }

                linkList.Add(bhLink);
            }

            if (linkList.Count == 0)
                return linkList;

            //Get ids of primary and secondary nodes
            List<string> nodeIds = linkList.SelectMany(x => x.SecondaryNodes.Select(s => s.Name).Concat(new List<string> { x.PrimaryNode.Name })).Distinct().ToList();
            Dictionary<string, Node> nodes = GetCachedOrReadAsDictionary<string, Node>(nodeIds);

            List<string> contstrainIds = linkList.Select(x => x.Constraint.Name).Distinct().ToList();
            //Get cached or read out all constraints used by Links
            Dictionary<string, LinkConstraint> constraints = contstrainIds.Any() ? new Dictionary<string, LinkConstraint>() : GetCachedOrReadAsDictionary<string, LinkConstraint>(contstrainIds);

            foreach (RigidLink link in linkList)
            {
                LinkConstraint constraint;  //Reasign cached/read contraint
                if (constraints.TryGetValue(link.Constraint.Name, out constraint))
                    link.Constraint = constraint;

                Node stNode;
                if (nodes.TryGetValue(link.PrimaryNode.Name, out stNode))
                    link.PrimaryNode = stNode;

                for (int i = 0; i < link.SecondaryNodes.Count; i++)
                {
                    Node secNode;
                    if (nodes.TryGetValue(link.SecondaryNodes[i].Name, out secNode))
                        link.SecondaryNodes[i] = secNode;
                }
            }


            return linkList;
        }

        /***************************************************/
    }
}






