using BH.Engine.Structure;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/
        private bool CreateObject(ISurfaceProperty surfaceProperty)
        {
            string propertyName = surfaceProperty.Name;// surfaceProperty.CustomData[AdapterId].ToString();

            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "Panel", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "Panel", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "Panel", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                int shellType = 1;
                bool includeDrillingDOF = true;
                if (m_model.PropArea.SetShell_1(constantThickness.Name, shellType, includeDrillingDOF, constantThickness.Material.Name, 0, constantThickness.Thickness, constantThickness.Thickness) != 0)
                    CreatePropertyError("ConstantThickness", "SurfaceProperty", propertyName);
            }

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                if (m_model.PropArea.SetModifiers(propertyName, ref modifier) != 0)
                    CreatePropertyError("Modifiers", "SurfaceProperty", propertyName);
            }

            return true;
        }

        /***************************************************/
    }
}
