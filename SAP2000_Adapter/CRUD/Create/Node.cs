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

using BH.Engine.Adapter;
using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Adapters.SAP2000;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;


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

            //Check for dealbreaking validity problems
            if (bhNode.Position == null)
            {
                Engine.Reflection.Compute.RecordError($"Node {bhNode.Name} has no position. Nothing was created.");
                return false;
            }

            // Create geometry in SAP
            if (m_model.PointObj.AddCartesian(bhNode.Position.X, bhNode.Position.Y, bhNode.Position.Z, ref name, bhNode.Name.ToString()) != 0)
            {
                CreateElementError("Node", bhNode.Name);
                return false;
            }

            // Set Adapter ID
            if (name != bhNode.Name & bhNode.Name != "")
                Engine.Reflection.Compute.RecordNote($"Node {bhNode.Name} was assigned SAP2000_id of {name}");

            string guid = null;
            m_model.PointObj.GetGUID(name, ref guid);

            SAP2000Id sap2000IdFragment = new SAP2000Id { Id = name, PersistentId = guid };
            bhNode.SetAdapterId(sap2000IdFragment);

            // Set Properties
            SetObject(bhNode);            

            return true;            
        }

        /***************************************************/

        private bool SetObject(Node bhNode)
        {
            string name = GetAdapterId<string>(bhNode);

            if (bhNode.Support != null)
            {
                bool[] restraint = new bool[6];
                double[] spring = new double[6];

                bhNode.GetSAPConstraint(ref restraint, ref spring);

                if (m_model.PointObj.SetRestraint(name, ref restraint) == 0) { }
                else
                {
                    CreatePropertyWarning("Node Restraint", "Node", name);
                }
                if (m_model.PointObj.SetSpring(name, ref spring) == 0) { }
                else
                {
                    CreatePropertyWarning("Node Spring", "Node", name);
                }
            }

            if (!bhNode.Orientation.Equals(Basis.XY))
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

            foreach (string gName in bhNode.Tags)
            {
                string groupName = gName.ToString();
                if (m_model.PointObj.SetGroupAssign(name, groupName) != 0)
                {
                    m_model.GroupDef.SetGroup(groupName);
                    m_model.PointObj.SetGroupAssign(name, groupName);
                }
            }

            return true;
        }
    }
}

