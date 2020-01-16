using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
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
                bhomSection.CustomData[AdapterId] = bhomSection.Name;
                return true;
            }
            else { return false; }
        }

        /***************************************************/
        /**** Set Property                              ****/
        /***************************************************/

        private bool SetSection(CableSection bhomSection)
        {
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
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.Name, bhomSection.Material);
        }

        /***************************************************/

        private bool SetSection(ExplicitSection bhomSection)
        {
            int ret = m_model.PropFrame.SetGeneral(bhomSection.Name, bhomSection.Material.Name, bhomSection.CentreZ * 2, 
                bhomSection.CentreY * 2, bhomSection.Area, bhomSection.Asy, bhomSection.Asz, bhomSection.J, 
                bhomSection.Iy, bhomSection.Iz, bhomSection.Wply, bhomSection.Wplz, bhomSection.Wely, 
                bhomSection.Wely, bhomSection.Rgy, bhomSection.Rgz);
            return ret == 0;
        }

        /***************************************************/

        private bool SetSection(SteelSection bhomSection)
        {
            return SetProfile(bhomSection.SectionProfile as dynamic, bhomSection.Name, bhomSection.Material);
        }

        /***************************************************/

        private bool SetProfile(AngleProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetAngle(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(BoxProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetTube(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width, bhomProfile.Thickness, bhomProfile.Thickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(ChannelProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetChannel(sectionName, material.Name, bhomProfile.Height, bhomProfile.FlangeWidth, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(CircleProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetCircle(sectionName, material.Name, bhomProfile.Diameter);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FabricatedBoxProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            if (bhomProfile.TopFlangeThickness != bhomProfile.BotFlangeThickness)
            {
                CreatePropertyWarning(sectionName, "Fabricated Box with unequal flanges", bhomProfile.BHoM_Guid.ToString());                
            }
            double tf = Math.Max(bhomProfile.TopFlangeThickness, bhomProfile.BotFlangeThickness);
            int ret = m_model.PropFrame.SetTube(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width, tf, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FabricatedISectionProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetISection(sectionName, material.Name, bhomProfile.Height, bhomProfile.TopFlangeWidth, bhomProfile.TopFlangeThickness, bhomProfile.WebThickness, bhomProfile.BotFlangeWidth, bhomProfile.BotFlangeThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(FreeFormProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            //Not implemented
            return false;
        }

        /***************************************************/

        private bool SetProfile(ISectionProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetISection(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness, bhomProfile.Width, bhomProfile.FlangeThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(KiteProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            //Not implemented
            return false;
        }

        /***************************************************/

        private bool SetProfile(RectangleProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetRectangle(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(TSectionProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetTee(sectionName, material.Name, bhomProfile.Height, bhomProfile.Width, bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(ZSectionProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            if (bhomProfile.FlangeThickness != bhomProfile.WebThickness)
            {
                CreatePropertyWarning(sectionName, "Z Section with unequal web and flange thickness", bhomProfile.BHoM_Guid.ToString());
            }
            double t = Math.Max(bhomProfile.FlangeThickness, bhomProfile.WebThickness);
            int ret = m_model.PropFrame.SetColdZ(sectionName, material.Name, bhomProfile.Height, bhomProfile.FlangeWidth, t, bhomProfile.RootRadius, 0, 0);
            return ret == 0;
        }

        /***************************************************/

        private bool SetProfile(TubeProfile bhomProfile, string sectionName, IMaterialFragment material)
        {
            int ret = m_model.PropFrame.SetPipe(sectionName, material.Name, bhomProfile.Diameter, bhomProfile.Thickness);
            return ret == 0;
        }

        /***************************************************/
    }
}
