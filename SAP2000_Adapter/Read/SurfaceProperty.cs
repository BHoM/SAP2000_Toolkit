using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Properties.Surface;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<ISurfaceProperty> ReadSurfaceProperty(List<string> ids = null)
        {
            List<ISurfaceProperty> propertyList = new List<ISurfaceProperty>();
            int nameCount = 0;
            string[] nameArr = { };

            if (ids == null)
            {
                m_model.PropArea.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }

            foreach (string id in ids)
            {
                int shellType = 0;
                bool includeDrillingDOF = true;
                string material = "";
                double matAng = 0;
                double thickness = 0;
                double bending = 0;
                int color = 0;
                string notes = "";
                string guid = "";

                double[] modifiers = new double[] { };
                bool hasModifiers = false;

                m_model.PropArea.GetShell_1(id, ref shellType, ref includeDrillingDOF, ref material, ref matAng, ref thickness, ref bending, ref color, ref notes, ref guid);
                if (m_model.PropArea.GetModifiers(id, ref modifiers) == 0)
                    hasModifiers = true;

                if (thickness == 0)
                {
                    ReadElementError("Panel", id);
                }
                else
                {
                    ConstantThickness panelConstant = new ConstantThickness();
                    panelConstant.CustomData[AdapterId] = id;
                    panelConstant.Name = id;
                    panelConstant.Material = ReadMaterial(new List<string>() { material })[0];
                    panelConstant.Thickness = thickness;
                    panelConstant.CustomData.Add("MaterialAngle", matAng);
                    panelConstant.CustomData.Add("BendingThickness", bending);
                    panelConstant.CustomData.Add("Color", color);
                    panelConstant.CustomData.Add("Notes", notes);
                    panelConstant.CustomData.Add("GUID", guid);

                    if (hasModifiers)
                    {
                        panelConstant.CustomData.Add("MembraneF11Modifier", modifiers[0]);
                        panelConstant.CustomData.Add("MembraneF22Modifier", modifiers[1]);
                        panelConstant.CustomData.Add("MembraneF12Modifier", modifiers[2]);
                        panelConstant.CustomData.Add("BendingM11Modifier", modifiers[3]);
                        panelConstant.CustomData.Add("BendingM22Modifier", modifiers[4]);
                        panelConstant.CustomData.Add("BendingM12Modifier", modifiers[5]);
                        panelConstant.CustomData.Add("ShearV13Modifier", modifiers[6]);
                        panelConstant.CustomData.Add("ShearV23Modifier", modifiers[7]);
                        panelConstant.CustomData.Add("MassModifier", modifiers[8]);
                        panelConstant.CustomData.Add("WeightModifier", modifiers[9]);
                    }

                    propertyList.Add(panelConstant);
                }

            }

            return propertyList;
        }
        
        /***************************************************/
    }
}
