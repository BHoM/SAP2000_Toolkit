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
using BH.Engine.Base;
using System;
using BH.oM.Base;

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

            // Check for dealbreaking BHoM validity
            if (bhBar.StartNode == null || bhBar.EndNode == null)
            {
                Engine.Reflection.Compute.RecordError($"Bar {bhBar.Name} failed to push because its nodes are null");
                return false;
            }

            string startId = GetAdapterId<string>(bhBar.StartNode);
            string endId = GetAdapterId<string>(bhBar.EndNode);

            if (startId == null || endId == null)
            {
                Engine.Reflection.Compute.RecordError($"Bar {bhBar.Name} failed to push because its nodes were not found in SAP2000. Check that geometry is valid.");
                return false;
            }

            // Create Geometry in SAP
            if (m_model.FrameObj.AddByPoint(startId, endId, ref name, "None", bhBar.Name.ToString()) != 0)
            {
                CreateElementError("Bar", name);
                return false;
            }

            // Set AdapterID
            if (name != bhBar.Name & bhBar.Name != "")
                Engine.Reflection.Compute.RecordNote($"Bar {bhBar.Name} was assigned SAP2000_id of {name}");

            string guid = null;
            m_model.FrameObj.GetGUID(name, ref guid);

            SAP2000Id sap2000IdFragment = new SAP2000Id { Id = name, PersistentId = guid };
            bhBar.SetAdapterId(sap2000IdFragment);

            // Set Properties
            SetObject(bhBar);

            return true;
        }

        /***************************************************/

        private bool SetObject(Bar bhBar)
        {
            string name = GetAdapterId<string>(bhBar);

            if (bhBar.SectionProperty != null)
            {
                string propId = GetAdapterId<string>(bhBar.SectionProperty);
                if (propId != null)
                {
                    if (m_model.FrameObj.SetSection(name, propId) != 0)
                    {
                        CreatePropertyWarning("SectionProperty", "Bar", name);
                    }
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
                if (bhBar.Offset.Start != null && bhBar.Offset.End != null)
                {
                    if (m_model.FrameObj.SetEndLengthOffset(name, false, -1 * (bhBar.Offset.Start.X), bhBar.Offset.End.X, 1) != 0)
                    {
                        CreatePropertyWarning("Length offset", "Bar", name);
                    }
                }
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

            return true;
        }
    }
}
