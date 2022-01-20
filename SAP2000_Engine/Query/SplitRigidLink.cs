/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<RigidLink> SplitRigidLink(RigidLink link)
        {
            if (link == null)
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



