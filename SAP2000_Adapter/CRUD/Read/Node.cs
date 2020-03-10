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

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Node> ReadNodes(List<string> ids = null)
        {
            List<Node> nodeList = new List<Node>();

            int nameCount = 0;
            string[] nameArr = { };

            if (ids == null)
            {
                if (m_model.PointObj.GetNameList(ref nameCount, ref nameArr) == 0)
                {
                    ids = nameArr.ToList();
                }
            }

            foreach (string id in ids)
            {
                Node bhNode = new Node();
                double x, y, z;
                x = y = z = 0;
                bool[] restraint = new bool[6];
                double[] spring = new double[6];

                m_model.PointObj.GetCoordCartesian(id, ref x, ref y, ref z);
                bhNode.Position = Engine.Geometry.Create.Point(x, y, z);
                bhNode.CustomData[AdapterIdName] = id;

                m_model.PointObj.GetRestraint(id, ref restraint);
                m_model.PointObj.SetSpring(id, ref spring);
                bhNode.Support = Engine.SAP2000.Convert.GetConstraint6DOF(restraint, spring);


                nodeList.Add(bhNode);
            }


            return nodeList;
        }

        /***************************************************/
    }
}
