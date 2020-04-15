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
using BH.oM.Structure.Offsets;
using BH.oM.Structure.Constraints;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Bar bhBar)
        {
            string name = "";


            if (m_model.FrameObj.AddByPoint(bhBar.StartNode.CustomData[AdapterIdName].ToString(), bhBar.EndNode.CustomData[AdapterIdName].ToString(), ref name, "Default", bhBar.Name.ToString()) == 0)
            {
                if (name != bhBar.Name & bhBar.Name != "")
                    Engine.Reflection.Compute.RecordNote($"Bar {bhBar.Name} was assigned {AdapterIdName} of {name}");
                bhBar.CustomData[AdapterIdName] = name;

                string barProp = bhBar.SectionProperty != null ? bhBar.SectionProperty.CustomData[AdapterIdName].ToString() : "None";

                if (m_model.FrameObj.SetSection(name, barProp) != 0)
                {
                    CreatePropertyWarning("SectionProperty", "Bar", name);
                }

                if (bhBar.OrientationAngle != 0)
                {
                    if (m_model.FrameObj.SetLocalAxes(name, bhBar.OrientationAngle * 180 / System.Math.PI) != 0)
                    {
                        CreatePropertyWarning("Orientation angle", "Bar", name);
                    }
                }

                if (bhBar.Release != null)
                {
                    bool[] restraintStart = null;
                    double[] springStart = null;
                    bool[] restraintEnd = null;
                    double[] springEnd = null;

                    bhBar.GetSAPBarRelease(ref restraintStart, ref springStart, ref restraintEnd, ref springEnd);

                    if (m_model.FrameObj.SetReleases(name, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd) != 0)
                    {
                        CreatePropertyWarning("Release", "Bar", name);
                    }
                }

                else if (bhBar.Offset != null)
                {
                    if (m_model.FrameObj.SetEndLengthOffset(name, false, -1 * (bhBar.Offset.Start.X), bhBar.Offset.End.X, 1) != 0)
                    {
                        CreatePropertyWarning("Length offset", "Bar", name);
                    }
                }
            }
            else
            {
                CreateElementError("Bar", name);
            }

            return true;
        }

        /***************************************************/
    }
}
