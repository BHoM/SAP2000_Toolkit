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

using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Panel bhPanel)
        {                        
            List<Point> boundaryPoints = bhPanel.ExternalEdgeCurves().Select( item => item.IStartPoint()).ToList();

            int segmentCount = boundaryPoints.Count();

            double[] x = boundaryPoints.Select(item => item.X).ToArray();
            double[] y = boundaryPoints.Select(item => item.Y).ToArray();
            double[] z = boundaryPoints.Select(item => item.Z).ToArray();

            string name = "";

            if (m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, "Default", bhPanel.Name.ToString()) == 0)
            {
                if (name != bhPanel.Name & bhPanel.Name != "")
                    Engine.Reflection.Compute.RecordNote($"Panel {bhPanel.Name} was assigned {AdapterIdName} of {name}");
                bhPanel.CustomData[AdapterIdName] = name;

                string propName = bhPanel.Property.CustomData[AdapterIdName].ToString();
                if (m_model.AreaObj.SetProperty(name, propName, 0) != 0)
                    CreatePropertyError("Surface Property", "Panel", name);
            }
            else
                CreateElementError("Panel", bhPanel.Name);

            return true;
        }

        /***************************************************/
    }
}
