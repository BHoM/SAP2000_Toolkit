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
            int ret = 0;

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
                ret += m_model.PropArea.SetShell(constantThickness.Name, 1, constantThickness.Material.Name, 0, constantThickness.Thickness, constantThickness.Thickness);
            }

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                ret += m_model.PropArea.SetModifiers(propertyName, ref modifier);
            }

            return ret == 0;
        }

        /***************************************************/
    }
}
