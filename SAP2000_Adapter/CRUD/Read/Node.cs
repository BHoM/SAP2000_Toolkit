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
using BH.oM.Geometry;
using BH.oM.Adapters.SAP2000;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;
using BH.oM.Structure.Constraints;
using System;
using BH.Engine.Adapter;

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

            m_model.PointObj.GetNameList(ref nameCount, ref nameArr);

            ids = FilterIds(ids, nameArr);

            foreach (string id in ids)
            {
                Node bhNode = new Node();
                string guid = null;
                SAP2000Id sap2000id = new SAP2000Id();
                sap2000id.Id = id;

                bhNode.Position = ReadNodeCoordinates(id);

                bhNode.Orientation = ReadNodeLocalAxes(id);

                bhNode.Support = ReadNodeSupport(id);

                // Get the groups the node is assigned to
                int numGroups = 0;
                string[] groupNames = new string[0];
                if (m_model.PointObj.GetGroupAssign(id, ref numGroups, ref groupNames) == 0)
                {
                    foreach (string grpName in groupNames)
                        bhNode.Tags.Add(grpName);
                }

                if (m_model.PointObj.GetGUID(id, ref guid) == 0)
                    sap2000id.PersistentId = guid;
                bhNode.SetAdapterId(sap2000id);
                nodeList.Add(bhNode); 
                
            }

            return nodeList;
        }

        /***************************************************/
        
        private Point ReadNodeCoordinates(string id)
        {
            double x = 0, y = 0, z = 0;

            if (m_model.PointObj.GetCoordCartesian(id, ref x, ref y, ref z) == 0)
                return BH.Engine.Geometry.Create.Point(x, y, z);

            return null;
        }

        /***************************************************/
        private Basis ReadNodeLocalAxes(string id)
        {
            Basis basis = Basis.XY;

            double a = 0;
            double b = 0;
            double c = 0;
            bool isAdvanced = false;

            m_model.PointObj.GetLocalAxes(id, ref a, ref b, ref c, ref isAdvanced);

            if (isAdvanced)
            {
                bool myActive = false;
                int myAxVectOpt = 0;
                string myAxCSys = null;
                int[] myAxDir = null;
                string[] myAxPt = null;
                double[] myAxVect = null;
                int myPlane2 = 0;
                int myPlVectOpt = 0;
                string myPlCSys = null;
                int[] myPlDir = null;
                string[] myPlPt = null;
                double[] myPlVect = null;
                m_model.PointObj.GetLocalAxesAdvanced(id, ref myActive,
                    ref myAxVectOpt, ref myAxCSys, ref myAxDir, ref myAxPt, ref myAxVect, ref myPlane2,
                    ref myPlVectOpt, ref myPlCSys, ref myPlDir, ref myPlPt, ref myPlVect);

                if (myAxCSys != "GLOBAL" || myPlCSys != "GLOBAL")
                {
                    Engine.Base.Compute.RecordWarning("No support for reading node orientations not in Global Coordinates. Node orientation set to default");
                    return basis;
                }

                Vector vec1 = null;
                Vector vec2 = null;
                Vector vec3 = null;

                List<Vector> axes = new List<Vector>() { Vector.XAxis, Vector.YAxis, Vector.ZAxis };

                switch (myAxVectOpt)
                {
                    case 1: // Coordinate Direction
                        vec1 = System.Math.Sign(myAxDir[0]) * axes[(System.Math.Abs(myAxDir[0]) - 1)];
                        break;
                    case 2: // Two Joints
                        IEnumerable<string> axPtIds = myAxPt.AsEnumerable().Select(x => (x == "None" ? id : x));
                        List<Point> pts1 = axPtIds.Select( x => ReadNodeCoordinates(x)).ToList();
                        vec1 = BH.Engine.Geometry.Create.Line(pts1[0], pts1[1]).Direction();
                        break;
                    case 3: // User Vector
                        vec1 = BH.Engine.Geometry.Create.Vector(myAxVect[0], myAxVect[1], myAxVect[2]);
                        break;
                }

                switch (myPlVectOpt)
                {
                    case 1: // Coordinate Direction
                        vec2 = System.Math.Sign(myPlDir[0]) * axes[(System.Math.Abs(myPlDir[0]) - 1)];
                        vec3 = System.Math.Sign(myPlDir[1]) * axes[(System.Math.Abs(myPlDir[1]) - 1)];
                        break;
                    case 2: // Two Joints
                        IEnumerable<string> plPtIds = myPlPt.AsEnumerable().Select(x => (x == "None" ? id : x));
                        List<Point> pts2 = plPtIds.Select(x => ReadNodeCoordinates(x)).ToList();
                        vec2 = BH.Engine.Geometry.Create.Line(pts2[0], pts2[1]).Direction();
                        break;
                    case 3: // User Vector
                        vec2 = BH.Engine.Geometry.Create.Vector(myPlVect[0], myPlVect[1], myPlVect[2]);
                        break;
                }

                try
                {
                    switch (myPlane2)
                    {
                        case 12:
                            basis = BH.Engine.Geometry.Create.Basis(vec1, vec2);
                            break;
                        case 13:
                            basis = BH.Engine.Geometry.Create.Basis(vec1, vec1.CrossProduct(-vec2));
                            break;
                        case 21:
                            basis = BH.Engine.Geometry.Create.Basis(vec2, vec1);
                            break;
                        case 23:
                            basis = BH.Engine.Geometry.Create.Basis(vec1.CrossProduct(vec2), vec1);
                            break;
                        case 31:
                            basis = BH.Engine.Geometry.Create.Basis(vec2, vec2.CrossProduct(-vec1));
                            break;
                        case 32:
                            basis = BH.Engine.Geometry.Create.Basis(vec2.CrossProduct(vec1), vec2);
                            break;
                        default:
                            CreatePropertyWarning("Orientation", "Node", id);
                            basis = Basis.XY;
                            break;
                    }
                }
                catch
                {
                    Engine.Base.Compute.RecordWarning($"Could not create basis for node {id}. Returning XY basis. Vectors were {vec1.ToString()}, {vec2.ToString()}, {vec3.ToString()}. Plane was {myPlane2}");
                    basis = Basis.XY;
                }
            }

            if (a == 0 && b == 0 && c == 0)
                return basis;

            a = ToRadians(a);
            b = ToRadians(b);
            c = ToRadians(c);

            Vector basisX = basis.X.Rotate(a, basis.Z).Rotate(b, basis.Y).Rotate(c, basis.X);
            Vector basisY = basis.Y.Rotate(a, basis.Z).Rotate(b, basis.Y).Rotate(c, basis.X);

            return BH.Engine.Geometry.Create.Basis(basisX, basisY);
            
        }

        /***************************************************/
        private Constraint6DOF ReadNodeSupport(string id)
        {
            bool[] restraint = new bool[6];
            double[] spring = new double[6];

            m_model.PointObj.GetRestraint(id, ref restraint);
            m_model.PointObj.SetSpring(id, ref spring);
            return Convert.GetConstraint6DOF(restraint, spring);
        }

        /***************************************************/

        private double ToRadians(double a)
        {
            return a * System.Math.PI / 180;
        }

        /***************************************************/
    }
}






