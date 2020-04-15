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
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;

namespace BH.Engine.SAP2000
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> JoinRigidLink(List<RigidLink> linkList)
        {
            List<RigidLink> joinedList = null;

            Dictionary<string, Node> masterDict = null;
            Dictionary<string, List<Node>> slaveDict = null;

            //Use first constraint for all
            LinkConstraint constraint = linkList.First().Constraint;

            foreach (RigidLink link in linkList)
            {
                string[] nameParts = link.Name.Split(new[] { ":::" }, StringSplitOptions.None);
                if (nameParts.Count() == 1)
                    joinedList.Add(link);
                else
                {
                    string name = nameParts[0];
                    if (masterDict.ContainsKey(name))
                    {
                        slaveDict[name].Add(link.SlaveNodes[0]);
                    }
                    else
                    {
                        masterDict.Add(name, link.MasterNode);
                        slaveDict.Add(name, new List<Node> { link.SlaveNodes[0] });
                    }
                }
            }

            foreach (KeyValuePair<string, Node> kvp in masterDict)
            {
                RigidLink newLink = Structure.Create.RigidLink(kvp.Value, slaveDict[kvp.Key], constraint, kvp.Key);
            }           

            return joinedList;
        }

        /***************************************************/
    }
}
