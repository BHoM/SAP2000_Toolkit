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
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapter;
using System;

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


            if (m_model.FrameObj.AddByPoint(GetAdapterId<string>(bhBar.StartNode), GetAdapterId<string>(bhBar.EndNode), ref name, "None", bhBar.Name.ToString()) == 0)
            {
                if (name != bhBar.Name & bhBar.Name != "")
                    Engine.Reflection.Compute.RecordNote($"Bar {bhBar.Name} was assigned SAP2000_id of {name}");

                SAP2000Id sap2000IdFragment = new SAP2000Id { Id = name };
                string guid = null;

                if (bhBar.SectionProperty != null)
                {
                    if (m_model.FrameObj.SetSection(name, GetAdapterId<string>(bhBar.SectionProperty)) != 0)
                    {
                        CreatePropertyWarning("SectionProperty", "Bar", name);
                    }
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

                    if (bhBar.Release.ToSAP(ref restraintStart, ref springStart, ref restraintEnd, ref springEnd))
                    {
                        if (m_model.FrameObj.SetReleases(name, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd) != 0)
                        {
                            CreatePropertyWarning("Release", "Bar", name);
                        }
                    }

                }

                if (bhBar.Offset != null)
                {
                    if (m_model.FrameObj.SetEndLengthOffset(name, false, -1 * (bhBar.Offset.Start.X), bhBar.Offset.End.X, 1) != 0)
                    {
                        CreatePropertyWarning("Length offset", "Bar", name);
                    }
                }

                if (m_model.FrameObj.GetGUID(name, ref guid) == 0)
                {
                    sap2000IdFragment.PersistentId = guid;
                }

                foreach (string gName in bhBar.Tags)
                {
                    string groupName = gName.ToString();
                    if (m_model.FrameObj.SetGroupAssign(name, groupName) != 0)
                    {
                        m_model.GroupDef.SetGroup(groupName);
                        m_model.FrameObj.SetGroupAssign(name, groupName);
                    }
                }

                bhBar.SetAdapterId(sap2000IdFragment);
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
