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

using BH.oM.Physical.Materials;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Spatial;
using BH.oM.Structure.Fragments;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapters.SAP2000;
using BH.oM.Adapter;
using BH.Engine.Adapter;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(ISectionProperty bhomSection)
        {
            string propName = bhomSection.DescriptionOrName();

            if (bhomSection.Material == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Section {propName} had no material defined. Using a default material.");
            }

            SetSection(bhomSection as dynamic);

            SetAdapterId(bhomSection, bhomSection.DescriptionOrName());

            SetModifiers(bhomSection);

            return true;
        }

        /***************************************************/
        /**** Set Section                               ****/
        /***************************************************/

        private void SetSection(IGeometricalSection bhomSection)
        {
            string name = bhomSection.DescriptionOrName();
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} is null. Section was not created");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), bhomSection.Material?.DescriptionOrName() ?? "");
            return;
        }

        /***************************************************/

        private void SetSection(ExplicitSection bhomSection)
        {
            SetGeneral(bhomSection);
        }

        /***************************************************/

        private void SetSection(ISectionProperty bhomSection)
        {
            Engine.Reflection.Compute.RecordError($"Sections of type {bhomSection.GetType().Name} are not explicitly supported by SAP2000. Section with name {bhomSection.DescriptionOrName()} will be pushed as an explicit section.");
            SetGeneral(bhomSection);
        }

        /***************************************************/

        private void SetGeneral(ISectionProperty bhomSection)
        {
            m_model.PropFrame.SetGeneral(bhomSection.DescriptionOrName(),
                bhomSection.Material?.DescriptionOrName() ?? "",
                bhomSection.CentreZ * 2,
                bhomSection.CentreY * 2,
                bhomSection.Area,
                bhomSection.Asy,
                bhomSection.Asz,
                bhomSection.J, bhomSection.Iy, bhomSection.Iz,
                bhomSection.Wply, bhomSection.Wplz,
                bhomSection.Wely, bhomSection.Wely,
                bhomSection.Rgy, bhomSection.Rgz);
        }

        /***************************************************/
        /****** Set Profile                          *******/
        /***************************************************/
        private bool SetProfile(AngleProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetAngle(sectionName, matName, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(BoxProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetTube(sectionName, matName, bhomProfile.Height, bhomProfile.Width, bhomProfile.Thickness, bhomProfile.Thickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(ChannelProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetChannel(sectionName, matName, bhomProfile.Height, bhomProfile.FlangeWidth, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(CircleProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetCircle(sectionName, matName, bhomProfile.Diameter);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FabricatedBoxProfile bhomProfile, string sectionName, string matName)
        {
            if (bhomProfile.TopFlangeThickness != bhomProfile.BotFlangeThickness)
            {
                CreatePropertyWarning(sectionName, "Fabricated Box with unequal flanges", bhomProfile.BHoM_Guid.ToString());
            }
            double tf = Math.Max(bhomProfile.TopFlangeThickness, bhomProfile.BotFlangeThickness);
            int ret = m_model.PropFrame.SetTube(sectionName, matName, bhomProfile.Height, bhomProfile.Width, tf, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FabricatedISectionProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetISection(sectionName, matName, bhomProfile.Height, bhomProfile.TopFlangeWidth, bhomProfile.TopFlangeThickness, bhomProfile.WebThickness, bhomProfile.BotFlangeWidth, bhomProfile.BotFlangeThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FreeFormProfile bhomProfile, string sectionName, string matName)
        {
            Engine.Reflection.Compute.RecordError("FreeFormProfile is not yet implemented in SAP2000 adapter");
            return false;
        }

        /***************************************************/

        private bool SetProfile(ISectionProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetISection(sectionName, matName, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness, bhomProfile.Width, bhomProfile.FlangeThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(KiteProfile bhomProfile, string sectionName, string matName)
        {
            Engine.Reflection.Compute.RecordError("KiteProfile is not yet implemented in SAP2000 adapter");
            return false;
        }

        /***************************************************/

        private bool SetProfile(RectangleProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetRectangle(sectionName, matName, bhomProfile.Height, bhomProfile.Width);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(TSectionProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetTee(sectionName, matName, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(ZSectionProfile bhomProfile, string sectionName, string matName)
        {
            if (bhomProfile.FlangeThickness != bhomProfile.WebThickness)
            {
                CreatePropertyWarning(sectionName, "Z Section with unequal web and flange thickness", bhomProfile.BHoM_Guid.ToString());
            }
            double t = Math.Max(bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            int ret = m_model.PropFrame.SetColdZ(sectionName, matName, bhomProfile.Height, bhomProfile.FlangeWidth, t, bhomProfile.RootRadius, 0, 0);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(TubeProfile bhomProfile, string sectionName, string matName)
        {
            int ret = m_model.PropFrame.SetPipe(sectionName, matName, bhomProfile.Diameter, bhomProfile.Thickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(TaperedProfile taperProfile, string sectionName, string matName)
        {
            //Check and fix taperProfile
            taperProfile.MapPositionDomain();

            //Decompose the taperProfile dictionary
            IProfile[] profiles = taperProfile.Profiles.Values.ToArray();
            double[] positions = taperProfile.Profiles.Keys.ToArray();

            //Add the sub-profiles to the model and create a list of names
            string[] profNames = new string[profiles.Length];
            for (int i = 0; i < profiles.Length; i++)
            {
                profNames[i] = sectionName + $"_{i}";
                SetProfile(profiles[i] as dynamic, profNames[i], matName);
            }

            //initialize SAP inputs
            int nSegments = profiles.Length - 1;
            string[] startSec = new string[nSegments];
            string[] endSec = new string[nSegments];
            double[] myLength = new double[nSegments];
            int[] myType = new int[nSegments];
            int[] EI33 = new int[nSegments];
            int[] EI22 = new int[nSegments];

            //Convert list of stations to list of segments
            for (int i = 1; i <= nSegments; i++)
            {
                startSec[i-1] = profNames[i - 1];
                endSec[i - 1] = profNames[i];
                myLength[i - 1] = positions[i] - positions[i - 1];
                myType[i - 1] = 1;
                EI33[i - 1] = 1; //using linear interpolation for now. SAP allows other options, to be set before this is PR'd!
                EI22[i - 1] = 1;
            }

            //Send the tapered profile to SAP
            return (m_model.PropFrame.SetNonPrismatic(sectionName, nSegments, ref startSec, ref endSec, ref myLength, ref myType, ref EI33, ref EI22) == 0);
        }

        /***************************************************/

        private void SetModifiers(ISectionProperty bhomSection)
        {
            string propertyName = bhomSection.DescriptionOrName();

            SectionModifier modifier = bhomSection.FindFragment<SectionModifier>();

            if (modifier != null)
            {
                double[] sap2000Mods = SectionModifierToCSI(modifier);

                if (m_model.PropFrame.SetModifiers(propertyName, ref sap2000Mods) != 0)
                {
                    Engine.Reflection.Compute.RecordError($"Could not add user specified section modifiers for {bhomSection.DescriptionOrName()}.");
                }
            }
        }

        private double[] SectionModifierToCSI(SectionModifier modifier)
        {
            double[] sap2000Mods = new double[8];

            sap2000Mods[0] = modifier.Area;   //Area
            sap2000Mods[1] = modifier.Asz;    //Major axis shear
            sap2000Mods[2] = modifier.Asy;    //Minor axis shear
            sap2000Mods[3] = modifier.J;      //Torsion
            sap2000Mods[4] = modifier.Iz;     //Minor bending
            sap2000Mods[5] = modifier.Iy;     //Major bending
            sap2000Mods[6] = 1;               //Mass, not currently implemented
            sap2000Mods[7] = 1;               //Weight, not currently implemented

            return sap2000Mods;

        }
    }
}

