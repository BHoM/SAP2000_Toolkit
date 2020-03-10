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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.Linq;

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

            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => x.CustomData[AdapterIdName].ToString());
            Dictionary<string, ISurfaceProperty> bhomProperties = ReadSurfaceProperty().ToDictionary(x => x.CustomData[AdapterIdName].ToString());
            
            if (ids == null)
            {
                int nameCount = 0;
                string[] nameArr = { };
                m_model.AreaObj.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }

            foreach (string id in ids)
            {
                //Get outline of panel
                string[] pointNames = null;
                int pointCount = 0;
                m_model.AreaObj.GetPoints(id, ref pointCount, ref pointNames);

                List<Point> pts = new List<Point>();
                foreach (string name in pointNames)
                    pts.Add(bhomNodes[name].Position);
                pts.Add(pts[0]);
                Polyline outline = new Polyline() { ControlPoints = pts };
                List<Edge> outEdges = new List<Edge>() { BH.Engine.Structure.Create.Edge(outline, new oM.Structure.Constraints.Constraint4DOF()) };
                
                //Get the section property
                string propertyName = "";
                m_model.AreaObj.GetProperty(id, ref propertyName);
                List<Opening> noOpenings = null;

                //Create the panel
                Panel bhomPanel = BH.Engine.Structure.Create.Panel(outEdges, noOpenings, bhomProperties[propertyName], id);
                
                //Set the properties
                bhomPanel.CustomData[AdapterIdName] = id;
                
                //Add the panel to the list
                bhomPanels.Add(bhomPanel);
            }

            return bhomPanels;
        }

        /***************************************************/
    }
}
