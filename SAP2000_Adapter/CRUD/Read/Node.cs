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
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;
using BH.oM.Structure.Constraints;

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
                double x = 0, y = 0, z = 0;

                if (m_model.PointObj.GetCoordCartesian(id, ref x, ref y, ref z) == 0)
                {
                    bhNode.CustomData[AdapterIdName] = id;

                    bhNode.Position = Create.Point( x, y, z );

                    bhNode.Orientation = ReadNodeLocalAxes(id);

                    bhNode.Support = ReadNodeSupport(id);

                    nodeList.Add(bhNode);
                }
                else
                    ReadElementError("Node", id);
            }

            return nodeList;
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

                if (myAxCSys == "GLOBAL" && myPlCSys == "GLOBAL")
                {
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
                            List<Node> pts1 = ReadNodes(myAxPt.ToList());
                            vec1 = Create.Line(pts1[0].Position, pts1[1].Position).Direction();
                            break;
                        case 3: // User Vector
                            vec1 = Create.Vector(myAxVect[0], myAxVect[1], myAxVect[2]);
                            break;
                    }

                    switch (myPlVectOpt)
                    {
                        case 1: // Coordinate Direction
                            vec2 = System.Math.Sign(myPlDir[0]) * axes[(System.Math.Abs(myPlDir[0]) - 1)];
                            vec3 = System.Math.Sign(myPlDir[1]) * axes[(System.Math.Abs(myPlDir[1]) - 1)];
                            break;
                        case 2: // Two Joints
                            List<Node> pts2 = ReadNodes(myPlPt.ToList());
                            vec2 = Create.Line(pts2[0].Position, pts2[1].Position).Direction();
                            break;
                        case 3: // User Vector
                            vec2 = Create.Vector(myPlVect[0], myPlVect[1], myPlVect[2]);
                            break;
                    }

                    try
                    {
                        switch (myPlane2)
                        {
                            case 12:
                                basis = Create.Basis(vec1, vec2);
                                break;
                            case 13:
                                basis = Create.Basis(vec1, vec1.CrossProduct(-vec2));
                                break;
                            case 21:
                                basis = Create.Basis(vec2, vec1);
                                break;
                            case 23:
                                basis = Create.Basis(vec1.CrossProduct(vec2), vec1);
                                break;
                            case 31:
                                basis = Create.Basis(vec2, vec2.CrossProduct(-vec1));
                                break;
                            case 32:
                                basis = Create.Basis(vec2.CrossProduct(vec1), vec2);
                                break;
                            default:
                                CreatePropertyWarning("Orientation", "Node", id);
                                basis = Basis.XY;
                                break;
                        }
                        //switch (myPlane2)
                        //{
                        //    case 12:
                        //        basis = new Basis(vec1, vec2, vec1.CrossProduct(vec2));
                        //        break;
                        //    case 13:
                        //        basis = new Basis(vec1, vec1.CrossProduct(-vec2), vec2);
                        //        break;
                        //    case 21:
                        //        basis = new Basis(vec2, vec1, vec2.CrossProduct(vec1));
                        //        break;
                        //    case 23:
                        //        basis = new Basis(vec1.CrossProduct(vec2), vec1, vec2);
                        //        break;
                        //    case 31:
                        //        basis = new Basis(vec2, vec2.CrossProduct(-vec1), vec1);
                        //        break;
                        //    case 32:
                        //        basis = new Basis(vec2.CrossProduct(vec1), vec2, vec1);
                        //        break;
                        //    default:
                        //        CreatePropertyWarning("Orientation", "Node", id);
                        //        basis = Basis.XY;
                        //        break;
                        //}
                    }
                    catch
                    {
                        Engine.Reflection.Compute.RecordWarning($"Could not create basis for node {id}. Returning XY basis. Vectors were {vec1.ToString()}, {vec2.ToString()}, {vec3.ToString()}. Plane was {myPlane2}");
                        basis = Basis.XY;
                    }

                    if (basis.X.CrossProduct(basis.Y) != basis.Z)
                        Engine.Reflection.Compute.RecordWarning($"Local axis vectors for node {id} are not orthogonal. Check results carefully.");
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning("No support for reading node orientations not in Global Coordinates. Returning a node oriented to Global XY");
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

            return Create.Basis(basisX, basisY);
            
        }

        private Constraint6DOF ReadNodeSupport(string id)
        {
            bool[] restraint = new bool[6];
            double[] spring = new double[6];

            m_model.PointObj.GetRestraint(id, ref restraint);
            m_model.PointObj.SetSpring(id, ref spring);
            return Convert.GetConstraint6DOF(restraint, spring);
        }

        private double ToRadians(double a)
        {
            return a * System.Math.PI / 180;
        }
    }
}
