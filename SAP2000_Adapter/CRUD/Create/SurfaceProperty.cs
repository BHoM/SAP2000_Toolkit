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
            string materialName = surfaceProperty.Material.CustomData[AdapterIdName].ToString();

            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "Panel", surfaceProperty.Name);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                int shellType = 1;
                bool includeDrillingDOF = true;
                string material = constantThickness.Material.CustomData[AdapterIdName].ToString();
                if (m_model.PropArea.SetShell_1(surfaceProperty.Name, shellType, includeDrillingDOF, material, 0, constantThickness.Thickness, constantThickness.Thickness) != 0)
                    CreatePropertyError("ConstantThickness", "SurfaceProperty", surfaceProperty.Name);
            }

            surfaceProperty.CustomData[AdapterIdName] = surfaceProperty.Name;

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                if (m_model.PropArea.SetModifiers(surfaceProperty.Name, ref modifier) != 0)
                    CreatePropertyError("Modifiers", "SurfaceProperty", surfaceProperty.Name);
            }

            return true;
        }

        /***************************************************/
    }
}
