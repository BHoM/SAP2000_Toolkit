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
using System.Linq;
using BH.oM.Structure.Constraints;

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
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterIdName].ToString());

            //Read all links, filter by id at end, so that we can join multi-links.
            int nameCount = 0;
            string[] names = { };
            m_model.LinkObj.GetNameList(ref nameCount, ref names);

            foreach (string name in names)
            {
                string masterId = "";
                string SlaveId = "";
                m_model.LinkObj.GetPoints(name, ref masterId, ref SlaveId);

                //Assuming all constraints are fixed constraints
                LinkConstraint constraint = Engine.Structure.Create.LinkConstraintFixed();
                Engine.Reflection.Compute.RecordWarning("All Rigid Link constraints are being read as fully fixed. Check results carefully.");

                RigidLink newLink = BH.Engine.Structure.Create.RigidLink(bhomNodes[masterId], new List<Node> { bhomNodes[SlaveId] }, constraint);

                linkList.Add(newLink);
            }

            List<RigidLink> joinedList = BH.Engine.SAP2000.Modify.JoinRigidLink(linkList);

            List<RigidLink> filteredList = joinedList.Where(x => ids.Contains(x.Name)).ToList();

            return filteredList;
        }

        /***************************************************/
    }
}
