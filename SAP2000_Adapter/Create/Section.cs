using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        private bool CreateObject(ISectionProperty bhSection)
        {
            
            if (bhSection.GetType() == typeof(SteelSection))
            {
                SteelSection steelSection = (SteelSection)bhSection;
                Helper.SetSectionDimensions(steelSection.SectionProfile, bhSection.Name, bhSection.Material.Name, m_model);
            }
            else if (bhSection.GetType() == typeof(ConcreteSection))
            {
                ConcreteSection concreteSection = (ConcreteSection)bhSection;
                Helper.SetSectionDimensions(concreteSection.SectionProfile, bhSection.Name, bhSection.Material.Name, m_model);
            }
            else
            {
                //Create a general section for all unsupported types
                m_model.PropFrame.SetGeneral(bhSection.Name, bhSection.Material.Name, bhSection.CentreZ * 2,
                    bhSection.CentreY * 2, bhSection.Area, bhSection.Asy, bhSection.Asz, bhSection.J, bhSection.Iy,
                    bhSection.Iz, bhSection.Wply, bhSection.Wplz, bhSection.Wely, bhSection.Wely, bhSection.Rgy, bhSection.Rgz);
            }
            return true;
        }
    }
}
