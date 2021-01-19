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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapters.SAP2000;
using BH.Engine.Adapter;
using BH.Engine.Adapters.SAP2000;
using System;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.Fragments;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Bar> ReadBars(List<string> ids = null)
        {

            List<Bar> bhomBars = new List<Bar>();
            Dictionary<string, Node> bhomNodes = ReadNodes().ToDictionary(x => GetAdapterId<string>(x));
            Dictionary<string, ISectionProperty> bhomSections = ReadSectionProperties().ToDictionary(x => GetAdapterId<string>(x));

            int nameCount = 0;
            string[] nameArr = { };
            m_model.FrameObj.GetNameList(ref nameCount, ref nameArr);

            ids = FilterIds(ids, nameArr);

            foreach (string id in ids)
            {
                SAP2000Id sap2000id = new SAP2000Id();
                sap2000id.Id = id; 

                try
                {
                    Bar bhomBar = new Bar();
                    string startId = "";
                    string endId = "";
                    m_model.FrameObj.GetPoints(id, ref startId, ref endId);
                    
                    bhomBar.StartNode = bhomNodes[startId];
                    bhomBar.EndNode = bhomNodes[endId];

                    bool[] restraintStart = new bool[6];
                    double[] springStart = new double[6];
                    bool[] restraintEnd = new bool[6];
                    double[] springEnd = new double[6];

                    m_model.FrameObj.GetReleases(id, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd);
                    bhomBar.Release = Adapter.SAP2000.Convert.GetBarRelease(restraintStart, springStart, restraintEnd, springEnd);

                    //bhomBar.Release.StartRelease = Adapter.SAP2000.Convert.GetConstraint6DOF(restraintStart, springStart);
                    //bhomBar.Release.EndRelease = Adapter.SAP2000.Convert.GetConstraint6DOF(restraintEnd, springEnd);
                    
                    string propertyName = "";
                    string sAuto = ""; //This is the name of the auto select list assigned to the frame object, if any.
                    
                    if (m_model.FrameObj.GetSection(id, ref propertyName, ref sAuto) == 0)
                    {
                        ISectionProperty bhProp = new ExplicitSection();
                        bhomSections.TryGetValue(propertyName, out bhProp);
                        bhomBar.SectionProperty = bhProp;
                    }

                    double angle = 0;
                    bool advanced = false;

                    if (m_model.FrameObj.GetLocalAxes(id, ref angle, ref advanced) == 0)
                    {
                        if (advanced)
                        {
                            Engine.Reflection.Compute.RecordWarning("Advanced local axes are not yet supported by this toolkit. Bar " + id + " has been created with orientation angle = 0");
                            angle = 0;
                        }
                        bhomBar.OrientationAngle = angle * System.Math.PI / 180;
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordWarning("Could not get local axes for bar " + id + ". Orientation angle is 0 by default");
                    }

                    // Get the groups the bar is assigned to
                    string guid = null; 
                    int numGroups = 0;
                    string[] groupNames = new string[0];
                    if (m_model.FrameObj.GetGroupAssign(id, ref numGroups, ref groupNames) == 0)
                    {
                        foreach (string grpName in groupNames)
                            bhomBar.Tags.Add(grpName);
                    }

                    if (m_model.FrameObj.GetGUID(id, ref guid) == 0)
                    {
                        sap2000id.PersistentId = guid;
                    }

                    bhomBar.SetAdapterId(sap2000id);

                    /***************************************************/
                    /* SAP Fragments                                   */
                    /***************************************************/

                    // Automesh

                    bool autoMesh = false;
                    bool autoMeshAtPoints = false;
                    bool autoMeshAtLines = false;
                    int numSegs = 0;
                    double autoMeshMaxLength = 0.0;

                    m_model.FrameObj.GetAutoMesh(id, ref autoMesh, ref autoMeshAtPoints, ref autoMeshAtLines, ref numSegs, ref autoMeshMaxLength);
                    if (autoMesh)
                    {
                        bhomBar = bhomBar.SetAutoMesh(autoMesh, autoMeshAtPoints, autoMeshAtLines, numSegs, autoMeshMaxLength);
                    }

                    // Design Procedure

                    int designProcedure = (int)DesignProcedureType.NoDesign;

                    if (m_model.FrameObj.GetDesignProcedure(id, ref designProcedure) == 0)
                    {
                        DesignProcedureType designProcedureType = (DesignProcedureType)designProcedure;
                        bhomBar = bhomBar.SetDesignProcedure(designProcedureType);
                    }

                    // Insertion Point Offset
                    // Need to add more information to capture coordinate system?? GetCoordSys method and transform local csys
                    int insertionPoint = (int)BarInsertionPoint.Centroid;
                    bool mirror = false;
                    bool modifyStiffness = false;

                    double[] offset1 = new double[3];
                    double[] offset2 = new double[3];
                    string cSys = "";

                    if (m_model.FrameObj.GetInsertionPoint(id, ref insertionPoint, ref mirror, ref modifyStiffness, ref offset1, ref offset2, ref cSys) == 0)
                    {
                        BarInsertionPoint barInsertionPoint = (BarInsertionPoint)insertionPoint;
                        bhomBar = bhomBar.SetInsertionPoint(barInsertionPoint, modifyStiffness);
                    }

                    // Section Property Modifiers

                    double[] sectionModifiers = new double[8];
                    // first check if material assigned 
                    if (m_model.FrameObj.GetModifiers(id, ref sectionModifiers) == 0)
                    {
                        SectionModifier sectionModifier = new SectionModifier();
                        sectionModifier.Area = sectionModifiers[0];
                        sectionModifier.Asz = sectionModifiers[1];
                        sectionModifier.Asy = sectionModifiers[2];
                        sectionModifier.J = sectionModifiers[3];
                        sectionModifier.Iz = sectionModifiers[4];
                        sectionModifier.Iy = sectionModifiers[5];
                        // mass modifier = 6
                        // weight modifier = 7
                        if (bhomBar.SectionProperty == null)
                        {
                            ISectionProperty sectionProperty = new ExplicitSection();
                            bhomBar.SectionProperty = sectionProperty;
                        }
                        bhomBar.SectionProperty.Fragments.Add(sectionModifier);
                    }

                    bhomBars.Add(bhomBar);
                }

                catch
                {
                    ReadElementError("Bar", id.ToString());
                }
            }
            return bhomBars;
        }

        /***************************************************/
    }
}

