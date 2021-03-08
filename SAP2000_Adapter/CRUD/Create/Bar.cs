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

using BH.Engine.Structure;
using BH.Engine.Adapters.SAP2000;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.Fragments;
using BH.oM.Structure.Constraints;
using BH.oM.Adapters.SAP2000;
using BH.oM.Adapters.SAP2000.Elements;
using BH.Engine.Adapter;
using System.ComponentModel;
using BH.Engine.Base;
using System;
using BH.oM.Base;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
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
            SetSectionProperty(bhBar, name);
            SetOrientationAngle(bhBar, name);
            SetRelease(bhBar, name);
            SetOffsets(bhBar, name);
            SetGroups(bhBar, name);
            SetAutomesh(bhBar, name);
            SetDesignProcedure(bhBar, name);
            SetInsertionPoint(bhBar, name);

            return true;
        }

        /***************************************************/
        //[Description("Does all the SAP2000 interaction which does not initiate a new object in SAP2000.")]
        //private bool SetObject(Bar bhBar)
        //{
        //    string name = GetAdapterId<string>(bhBar);

        //    SetSectionProperty(bhBar, name);
        //    SetOrientationAngle(bhBar, name);
        //    SetRelease(bhBar, name);
        //    SetOffsets(bhBar, name);
        //    SetGroups(bhBar, name);
        //    SetAutomesh(bhBar, name);
        //    SetDesignProcedure(bhBar, name);
        //    SetInsertionPoint(bhBar, name);

        //    return true;
        //}

        private void SetSectionProperty(Bar bhBar, string name)
        {
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
        }
        private void SetOrientationAngle(Bar bhBar, string name)
        {
            if (bhBar.OrientationAngle != 0)
            {
                if (m_model.FrameObj.SetLocalAxes(name, bhBar.OrientationAngle * 180 / System.Math.PI) != 0)
                {
                    CreatePropertyWarning("Orientation angle", "Bar", name);
                }
            }
        }
        private void SetRelease(Bar bhBar, string name)
        {
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
        }
        private void SetOffsets(Bar bhBar, string name)
        {
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
        }
        private void SetGroups(Bar bhBar, string name)
        {
            foreach (string gName in bhBar.Tags)
            {
                string groupName = gName.ToString();
                if (m_model.FrameObj.SetGroupAssign(name, groupName) != 0)
                {
                    m_model.GroupDef.SetGroup(groupName);
                    m_model.FrameObj.SetGroupAssign(name, groupName);
                }
            }
        }
        private void SetAutomesh(Bar bhBar, string name)
        {
            BarAutoMesh barAutoMesh = bhBar.BarAutoMesh();

            if (barAutoMesh != null)
            {
                bool autoMesh = barAutoMesh.AutoMesh;
                bool autoMeshAtPoints = barAutoMesh.AutoMeshAtPoints;
                bool autoMeshAtLines = barAutoMesh.AutoMeshAtLines;
                int numSegs = barAutoMesh.NumSegs;
                double autoMeshMaxLength = barAutoMesh.AutoMeshMaxLength;

                if (m_model.FrameObj.SetAutoMesh(name, autoMesh, autoMeshAtPoints, autoMeshAtLines, numSegs, autoMeshMaxLength) != 0)
                {
                    CreatePropertyWarning("AutoMesh", "Bar", name);
                }
            }
        }
        private void SetDesignProcedure(Bar bhBar, string name)
        {
            BarDesignProcedure barDesignProcedure = bhBar.BarDesignProcedure();

            if (barDesignProcedure != null)
            {
                // issue with cold form as a material not being able to be pushed...
                if (barDesignProcedure.DesignProcedure == BarDesignProcedureType.Aluminum ||
                    barDesignProcedure.DesignProcedure == BarDesignProcedureType.ColdFormed ||
                    barDesignProcedure.DesignProcedure == BarDesignProcedureType.Steel ||
                    barDesignProcedure.DesignProcedure == BarDesignProcedureType.Concrete)
                {
                    // Design Procedure "MyType" is 1 if specified from material list available (limited to enums shown)
                    if (m_model.FrameObj.SetDesignProcedure(name, 1, 0) != 0)
                    {
                        CreatePropertyWarning("DesignProcedure", "Bar", name);
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordNote($"Bar {bhBar.Name} with SAP id {name} was set with a design procedure automatically based on its material (Steel/Concrete/Cold Form/Aluminum) regardless of selected input.");
                    }
                }
                else
                {
                    // Design Procedure "MyType" is 2 if no design specified - this defaults to aluminum rather than nodesign in the api call...
                    if (m_model.FrameObj.SetDesignProcedure(name, 2, 0) != 0)
                    {
                        CreatePropertyWarning("DesignProcedure", "Bar", name);
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordNote($"Bar {bhBar.Name} with SAP id {name} does not have a design procedure.");
                    }
                }
            }

        }
        private void SetInsertionPoint(Bar bhBar, string name)
        {
                Offset offset = bhBar.Offset;

                double[] offset1 = new double[3];
                double[] offset2 = new double[3];

                if (offset != null)
                {
                    if (offset.Start != null)
                    {
                        offset1[1] = offset.Start.Z;
                        offset1[2] = offset.Start.Y;
                    }

                    if (offset.End != null)
                    {
                        offset2[1] = offset.End.Z;
                        offset2[2] = offset.End.Y;
                    }
                }

                if (m_model.FrameObj.SetInsertionPoint(name, (int)bhBar.BarInsertionPoint(), false, bhBar.BarModifyStiffnessInsertionPoint(), ref offset1, ref offset2) != 0)
                {
                    CreatePropertyWarning("Insertion point and perpendicular offset", "Bar", name);
                }
        }
    }
}

