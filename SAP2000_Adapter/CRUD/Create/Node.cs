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

using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Node bhNode)
        {

            string name = "";

            if (m_model.PointObj.AddCartesian(bhNode.Position.X, bhNode.Position.Y, bhNode.Position.Z, ref name, bhNode.Name.ToString()) == 0)
            {
                if (name != bhNode.Name)
                    Engine.Reflection.Compute.RecordNote($"Node {bhNode.Name} was assigned {AdapterIdName} of {name}");
                bhNode.CustomData[AdapterIdName] = name;

                if (! bhNode.Orientation.Equals(BH.oM.Geometry.Basis.XY))
                {
                    int myVectOpt = 3; //specify orientation by vectors
                    string globalCSys = "GLOBAL"; //specify point orientation relative to the global coordinate system
                    int[] myDir = { 1, 2 }; //not used for VecOpt = 3
                    string[] noPts = { "None", "None" }; //not used for VecOpt = 3
                    int myPlane2 = 12; //specify point orientation by local 1(X) and 2(Y) vectors
                    double[] myAxVect = bhNode.Orientation.X.ToDoubleArray();
                    double[] myPlVect = bhNode.Orientation.Y.ToDoubleArray();

                    if (m_model.PointObj.SetLocalAxesAdvanced(name, true, 
                            myVectOpt, globalCSys, ref myDir, ref noPts, ref myAxVect, myPlane2, 
                            myVectOpt, globalCSys, ref myDir, ref noPts, ref myPlVect) != 0)
                        CreatePropertyWarning("Node Local Axes", "Node", name);
                }


                if (bhNode.Support != null)
                {
                    bool[] restraint = new bool[6];
                    double[] spring = new double[6];
                    
                    bhNode.GetSAPConstraint(ref restraint, ref spring);

                    if (m_model.PointObj.SetRestraint(name, ref restraint) != 0)
                        CreatePropertyWarning("Node Restraint", "Node", name);
                    if (m_model.PointObj.SetSpring(name, ref spring) != 0)
                        CreatePropertyWarning("Node Spring", "Node", name);
                }

                foreach (string gName in bhNode.Tags)
                {
                    string groupName = gName.ToString();
                    if (m_model.PointObj.SetGroupAssign(name, groupName) != 0)
                    {
                        m_model.GroupDef.SetGroup(groupName);
                        m_model.PointObj.SetGroupAssign(name, groupName);
                    }
                }
            }

            return true;
        }

        /***************************************************/
    }
}
