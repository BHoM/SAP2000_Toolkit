using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Section.ShapeProfiles;
using BH.Engine.Structure;
using CE = BH.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common.Materials;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{

    public static partial class Helper
    {
        public static void SetSpecificDimensions(TubeProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetPipe(sectionName, materialName, dimensions.Diameter, dimensions.Thickness);
        }

        public static void SetSpecificDimensions(BoxProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetTube(sectionName, materialName, dimensions.Height, dimensions.Width, dimensions.Thickness, dimensions.Thickness);
        }

        public static void SetSpecificDimensions(FabricatedBoxProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            if (dimensions.TopFlangeThickness != dimensions.BotFlangeThickness)
                throw new NotImplementedException("different thickness of top and bottom flange is not supported in SAP2000");
            model.PropFrame.SetTube(sectionName, materialName, dimensions.Height, dimensions.Width, dimensions.TopFlangeThickness, dimensions.WebThickness);
        }

        public static void SetSpecificDimensions(ISectionProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            Int32 ret = model.PropFrame.SetISection(sectionName, materialName, dimensions.Height, dimensions.Width, dimensions.FlangeThickness, dimensions.WebThickness, dimensions.Width, dimensions.FlangeThickness);
        }

        public static void SetSpecificDimensions(FabricatedISectionProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetISection(sectionName, materialName, dimensions.Height, dimensions.TopFlangeWidth, dimensions.TopFlangeThickness, dimensions.WebThickness, dimensions.BotFlangeWidth, dimensions.BotFlangeThickness);
        }

        public static void SetSpecificDimensions(ChannelProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetChannel(sectionName, materialName, dimensions.Height, dimensions.FlangeWidth, dimensions.FlangeThickness, dimensions.WebThickness);
        }

        public static void SetSpecificDimensions(AngleProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetAngle(sectionName, materialName, dimensions.Height, dimensions.Width, dimensions.FlangeThickness, dimensions.WebThickness);
        }

        public static void SetSpecificDimensions(TSectionProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetTee(sectionName, materialName, dimensions.Height, dimensions.Width, dimensions.FlangeThickness, dimensions.WebThickness);
        }

        public static void SetSpecificDimensions(ZSectionProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            if (dimensions.FlangeThickness != dimensions.WebThickness)
                throw new NotImplementedException("different thickness of web and flange is not supported in SAP2000");
            model.PropFrame.SetColdZ(sectionName, materialName, dimensions.Height, dimensions.FlangeWidth, dimensions.FlangeThickness, dimensions.RootRadius, 0, 0);
        }

        public static void SetSpecificDimensions(RectangleProfile dimensions, string sectionName, string materialName, cSapModel model)
        {
            model.PropFrame.SetRectangle(sectionName, materialName, dimensions.Height, dimensions.Width);
        }
        
    }
}
