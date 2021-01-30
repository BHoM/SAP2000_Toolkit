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
using BH.oM.Adapters.SAP2000;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;
using BH.Engine.Base;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> JoinRigidLink(List<RigidLink> linkList)
        {
            Dictionary<string, RigidLink> joinedList = new Dictionary<string, RigidLink>();
            
            foreach (RigidLink link in linkList)
            {
                SAP2000Id idFragment = link.FindFragment<SAP2000Id>();
                string Name = idFragment.Id.ToString();
                string[] nameParts = Name.Split(new[] { ":::" }, StringSplitOptions.None);
                if (nameParts.Count() == 1)
                    joinedList.Add(Name, link);
                else
                {
                    string JoinedName = nameParts[0];
                    if (joinedList.ContainsKey(JoinedName))
                    {
                        joinedList[JoinedName].SecondaryNodes.Add(link.SecondaryNodes[0]);
                    }
                    else
                    {
                        RigidLink newJoinedLink = link.ShallowClone();
                        newJoinedLink.Name = JoinedName;
                        joinedList.Add(JoinedName, newJoinedLink);
                    }
                }
            }        

            return joinedList.Values.ToList();
        }

        /***************************************************/
    }
}

