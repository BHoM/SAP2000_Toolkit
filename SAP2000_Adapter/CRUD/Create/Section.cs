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

using BH.oM.Physical.Materials;
using BH.Engine.Structure;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using System;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(ISectionProperty bhomSection)
        {
            if (SetSection(bhomSection as dynamic))
            {
                string propertyName = bhomSection.DescriptionOrName();
                SetAdapterId(bhomSection, propertyName);

                return true;
            }
            else { return false; }
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

        private bool SetSection(CableSection bhomSection)
        {
            string name = bhomSection.DescriptionOrName();
            // string matName = bhomSection.Material.CustomData[AdapterIdName].ToString();
            string matName = GetAdapterId<string>(bhomSection.Material); 
            if (m_model.PropCable.SetProp(name, matName, bhomSection.Area) != 0)
                Engine.Reflection.Compute.RecordError($"Could not create Cable section with name {name}");
            return false;
        }

        /***************************************************/

        private bool SetSection(CompositeSection bhomSection)
        {
            return false;
        }

        /***************************************************/

        private bool SetSection(ConcreteSection bhomSection)
        {
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), GetAdapterId<string>(bhomSection.Material));
        }

        /***************************************************/

        private bool SetSection(TimberSection bhomSection)
        {
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), GetAdapterId<string>(bhomSection.Material));
        }

        /***************************************************/

        private bool SetSection(SteelSection bhomSection)
        {
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), GetAdapterId<string>(bhomSection.Material));
        }

        /***************************************************/

        private bool SetSection(AluminiumSection bhomSection)
        {
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.DescriptionOrName(), GetAdapterId<string>(bhomSection.Material));
        }

        /***************************************************/

        private bool SetSection(ExplicitSection bhomSection)
        {
            string matName = GetAdapterId<string>(bhomSection.Material);
            int ret = m_model.PropFrame.SetGeneral(bhomSection.DescriptionOrName(), matName, bhomSection.CentreZ * 2, 
                bhomSection.CentreY * 2, bhomSection.Area, bhomSection.Asy, bhomSection.Asz, bhomSection.J, 
                bhomSection.Iy, bhomSection.Iz, bhomSection.Wply, bhomSection.Wplz, bhomSection.Wely, 
                bhomSection.Wely, bhomSection.Rgy, bhomSection.Rgz);
            return ret == 0;
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
            //Not implemented
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
            //Not implemented
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
            //Not implemented
            return false;
        }

        /***************************************************/
    }
}
