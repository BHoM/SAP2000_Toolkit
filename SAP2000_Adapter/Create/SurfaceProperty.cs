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
        private bool CreateObject(ISurfaceProperty surfaceProperty)
        {
            int ret = 0;

            string propertyName = surfaceProperty.Name;// surfaceProperty.CustomData[AdapterId].ToString();

            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                ret += m_model.PropArea.SetShell(constantThickness.Name, 1, constantThickness.Material.Name, 0, constantThickness.Thickness, constantThickness.Thickness);
            }

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                ret += m_model.PropArea.SetModifiers(propertyName, ref modifier);
            }

            return ret == 0;
        }
    }
}
