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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Geometry;
using BH.oM.Dimensional;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapters.SAP2000;
using BH.Engine.Adapter;
using BH.Engine.Units;
using System;
using SAP2000v1;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Panel> ReadPanel(List<string> ids = null)
        {
            List<Panel> bhomPanels = new List<Panel>();

            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => GetAdapterId<string>(x));
            Dictionary<string, ISurfaceProperty> bhomProperties = ReadSurfaceProperty().ToDictionary(x => GetAdapterId<string>(x));
                
            int nameCount = 0;
            string[] nameArr = { };
            m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);

            ids = FilterIds(ids, nameArr);

            foreach (string id in ids)
            {
                Panel bhomPanel = new Panel();
                SAP2000Id sap2000id = new SAP2000Id();
                string guid = null;

                //Set the Adapter ID
                sap2000id.Id = id;

                //Get outline of panel
                string[] pointNames = null;
                int pointCount = 0;

                if (m_model.AreaObj.GetPoints(id, ref pointCount, ref pointNames) == 0)
                {
                    List<Point> pts = new List<Point>();
                    foreach (string name in pointNames)
                        pts.Add(bhomNodes[name].Position);
                    pts.Add(pts[0]);
                    Polyline outline = new Polyline() { ControlPoints = pts };
                    List<Edge> outEdges = new List<Edge>() { new Edge { Curve = outline, Release = new oM.Structure.Constraints.Constraint4DOF() } };

                    bhomPanel.ExternalEdges = outEdges;
                }

                //There are no openings in SAP2000 
                bhomPanel.Openings = new List<Opening>();
                
                //Get the section property
                string propertyName = "";
                if (m_model.AreaObj.GetProperty(id, ref propertyName) == 0)
                {
                    ISurfaceProperty bhProp = new ConstantThickness();
                    bhomProperties.TryGetValue(propertyName, out bhProp);
                    bhomPanel.Property = bhProp;
                }

                //Get the groups the panel is assigned to
                int numGroups = 0;
                string[] groupNames = new string[0];
                if (m_model.AreaObj.GetGroupAssign(id, ref numGroups, ref groupNames) == 0)
                {
                    foreach (string grpName in groupNames)
                        bhomPanel.Tags.Add(grpName);
                }

                if (m_model.AreaObj.GetGUID(id, ref guid) == 0)
                    sap2000id.PersistentId = guid;

                bhomPanel.SetAdapterId(sap2000id);

                /***************************************************/
                /* SAP Fragments                                   */
                /***************************************************/

                // Automesh

                int meshType = (int)PanelAutoMeshType.None;
                int n1 = 0;
                int n2 = 0;
                double maxSize1 = 0;
                double maxSize2 = 0;
                bool pointOnEdgeFromLine = false;
                bool pointOnEdgeFromPoint = false;
                bool extendCookieCutLines = false;
                double rotation = 0;
                double maxSizeGeneral = 0;
                bool localAxesOnEdge = false;
                bool localAxesOnFace = false;
                bool restraintsOnEdge = false;
                bool restraintsOnFace = false;
                string group = null;
                bool subMesh = false;
                double subMeshSize = 0;

                m_model.AreaObj.GetAutoMesh(id, ref meshType, ref n1, ref n2, ref maxSize1, ref maxSize2,
                    ref pointOnEdgeFromLine, ref pointOnEdgeFromPoint, ref extendCookieCutLines,
                    ref rotation, ref maxSizeGeneral, ref localAxesOnEdge, ref localAxesOnFace,
                    ref restraintsOnEdge, ref restraintsOnFace, ref group, ref subMesh, ref subMeshSize);

                if (meshType != (int)PanelAutoMeshType.None)
                {
                    bhomPanel = bhomPanel.SetPanelAutoMesh((PanelAutoMeshType)meshType, n1, n2, maxSize1, maxSize2, 
                        pointOnEdgeFromLine, pointOnEdgeFromPoint, extendCookieCutLines, Engine.Units.Convert.FromDegree(rotation), 
                        maxSizeGeneral, localAxesOnEdge, localAxesOnFace, restraintsOnEdge, 
                        restraintsOnFace, group, subMesh, subMeshSize);
                }

                // Edge Constraint

                bool constraintExists = false;

                m_model.AreaObj.GetEdgeConstraint(id, ref constraintExists);
                if (constraintExists)
                    bhomPanel = bhomPanel.SetPanelEdgeConstraint(constraintExists);

                // Material Overwrite

                string propName = "";

                m_model.AreaObj.GetMaterialOverwrite(id, ref propName);

                if (propName != "None")
                {
                    ISurfaceProperty property = Engine.Base.Query.ShallowClone(bhomPanel.Property);
                    property.Material = ReadMaterial(new List<string> { propName }).FirstOrDefault();
                    bhomPanel.Property = property;
                }

                // Offsets

                int offsetType = (int)PanelOffsetType.None;
                string offsetPattern = "";
                double offsetPatternSF = 0;
                double[] offset = null;

                m_model.AreaObj.GetOffsets(id, ref offsetType, ref offsetPattern, ref offsetPatternSF, ref offset);

                if (offsetType > 0)
                    bhomPanel = bhomPanel.SetPanelOffset((PanelOffsetType)offsetType, offsetPattern, offsetPatternSF, offset);

                //Add the panel to the list
                bhomPanels.Add(bhomPanel);
            }

            return bhomPanels;
        }

        /***************************************************/
    }


}



