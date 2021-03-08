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

            string matName = "Default";
            if (bhomSection.Material != null)
            {
                matName = bhomSection.Material.DescriptionOrName();
            }
            else
            {
                Engine.Reflection.Compute.RecordWarning($"Section {propName} had no material defined. Using a default material.");
            }

            try
            {
                SetSection(bhomSection as dynamic, matName);
                SetModifiers(bhomSection);
            }
            catch
            {
                Engine.Reflection.Compute.RecordError($"Section {bhomSection.DescriptionOrName()} could not be created. Section type may not implemented, or input data may be invalid.");
                return true;
            }

            string propertyName = bhomSection.DescriptionOrName();
            SetAdapterId(bhomSection, propertyName);

            return true;
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

        private void SetSection(CableSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (m_model.PropCable.SetProp(name, matName, bhomSection.Area) != 0)
            {
                Engine.Reflection.Compute.RecordError($"Could not create Cable section with name {name}");
                return;
            }
            return;
        }

        /***************************************************/

        private void SetSection(CompositeSection bhomSection, string matName)
        {
            return;
        }

        /***************************************************/

        private void SetSection(ConcreteSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} is null. Section was not created");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, name, matName);
            return;
        }

        /***************************************************/

        private void SetSection(TimberSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} is null. Section was not created");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, name, matName);
            return;
        }

        /***************************************************/

        private void SetSection(SteelSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} is null. Section was not created");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, name, matName);
            return;
        }

        /***************************************************/

        private void SetSection(AluminiumSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} is null.");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, name, matName);
            return;
        }

        /***************************************************/

        private void SetSection(ExplicitSection bhomSection, string matName)
        {
            string name = bhomSection.DescriptionOrName();
            if (m_model.PropFrame.SetGeneral(name, matName, bhomSection.CentreZ * 2, 
                bhomSection.CentreY * 2, bhomSection.Area, bhomSection.Asy, bhomSection.Asz, bhomSection.J, 
                bhomSection.Iy, bhomSection.Iz, bhomSection.Wply, bhomSection.Wplz, bhomSection.Wely, 
                bhomSection.Wely, bhomSection.Rgy, bhomSection.Rgz) != 0)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {name} could not be created.");
            }

            return;
        }

        /***************************************************/

        private void SetSection(GenericSection bhomSection, string matName)
        {
            if (bhomSection.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {bhomSection.DescriptionOrName()} is null. Section was not created");
                return;
            }

            SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), matName);
            return;
        }

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

        private bool SetProfile(GenericSection bhomProfile, string sectionName, string matName)
        {
            Engine.Reflection.Compute.RecordError("GenericSection is not yet implemented in SAP2000 adapter");
            return false;
        }

        /***************************************************/

        private bool SetProfile(TaperedProfile bhomProfile, string sectionName, string matName)
        {        
            if (bhomProfile.Profiles == null)
            {
                Engine.Reflection.Compute.RecordWarning($"Profile for {bhomProfile.DescriptionOrName()} is empty. Tapered section was not created");
                return false;
            }

            List<KeyValuePair<double, IProfile>> stations = bhomProfile.Profiles.OrderBy(obj => obj.Key).ToList();

            List<string> profNames = null;
            List<double> profLengths = stations.Select(obj => obj.Key).ToList();

            //Tapered Frame Property in SAP references other profiles, but BHoM defines them within the TaperedProfile. Add any un-defined profiles before continuing.
            foreach(IProfile subProf in stations.Select(obj => obj.Value).ToList())
            {
                if (GetAdapterId<string>(subProf) != null) // Profile in model
                {
                    profNames.Add(GetAdapterId<string>(subProf));
                }
                else 
                {
                    string subName = subProf.DescriptionOrName();
                    SetProfile(subProf as dynamic, subName, matName);
                    profNames.Add(subName);
                }
            }

            //Define SAP inputs
            int nProfiles = profNames.Count - 1;
            string[] startSec = null;
            string[] endSec = null;
            double[] myLength = null;
            int[] myType = null;
            int[] EI33 = null;
            int[] EI22 = null;

            //Convert list of stations to list of segments
            for (int i = 1; i < nProfiles; i++)
            {
                startSec.Append(profNames[i - 1]);
                endSec.Append(profNames[i]);
                myLength.Append(profLengths[i] - profLengths[i - 1]);
                myType.Append(1);
                EI33.Append(1);
                EI22.Append(1);
            }

            //Send the tapered profile to SAP.
            if (m_model.PropFrame.SetNonPrismatic(sectionName, nProfiles, ref startSec, ref endSec, ref myLength, ref myType, ref EI33, ref EI22) != 0)
            {
                Engine.Reflection.Compute.RecordWarning($"Could not create tapered section: {bhomProfile.DescriptionOrName()}");
                return false;
            }

            return true;
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

