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
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Constraints;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Splits a RigidLink into one or more RigidLinks, each of which has exactly one SecondaryNode.")]
        [Input("link", "The RigidLink to be split; generally a link with one PrimaryNode and several SecondaryNodes.")]
        [Output("SingleLinks", "A list of RigidLinks, each with only one SecondaryNode. The name of each link will have ':::<i>' appended, where i is sequential for the list of links.")]
        public static List<RigidLink> SplitRigidLink(this RigidLink link)
        {
            if (link == null || link.PrimaryNode == null || link.SecondaryNodes == null)
            {
                Base.Compute.RecordError("Could not split the RigidLink because it is null.");
                return null;
            }

            List<RigidLink> singleLinks = new List<RigidLink>();

            if (link.SecondaryNodes.Count() <= 1)
            {
                singleLinks.Add(link);
            }
            else
            {
                for (int i = 0; i < link.SecondaryNodes.Count(); i++)
                {
                    RigidLink subLink = BH.Engine.Base.Query.ShallowClone(link);
                    subLink.SecondaryNodes = new List<Node> { link.SecondaryNodes[i] };
                    if (link.Name != "")
                    {
                        subLink.Name = $"{link.Name}:::{i}";
                        subLink.Tags.Add($"BHoM_Link_{link.Name}");
                    }

                    singleLinks.Add(subLink);
                }
            }

            return singleLinks;
        }

        /***************************************************/
    }
}






