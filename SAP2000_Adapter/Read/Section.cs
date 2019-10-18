using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.Engine.Physical;
using BH.Engine.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

#if Debug19 || Release19
using SAP = SAP2000v19;
#else
using SAP = SAP2000v1;
#endif

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> propList = new List<ISectionProperty>();
            Dictionary<String, IMaterialFragment> bhomMaterials = ReadMaterial().ToDictionary(x => x.Name);

            int nameCount = 0;
            string[] names = { };
            m_model.PropFrame.GetNameList(ref nameCount, ref names);

            if (ids == null)
            {
                ids = names.ToList();
            }
            
            foreach (string id in ids)
            {
                SAP.eFramePropType propertyType = SAP.eFramePropType.General;
                ISectionProperty bhomProperty = null;
                IProfile bhomProfile = null;

                m_model.PropFrame.GetTypeOAPI(id, ref propertyType);

                string constructor = "standard";

                string materialName = "";
                string fileName = "";
                double t3 = 0;
                double t2 = 0;
                double tf = 0;
                double tw = 0;
                double tfb = 0;
                double t2b = 0;
                double radius = 0;
                double angle = 0;
                int color = 0;
                string notes = "";
                string guid = "";
                double Area, As2, As3, Torsion, I22, I33, S22, S33, Z22, Z33, R22, R33;
                Area = As2 = As3 = Torsion = I22 = I33 = S22 = S33 = Z22 = Z33 = R22 = R33 = 0;
                
                switch (propertyType)
                {
                    case SAP.eFramePropType.I:
                        m_model.PropFrame.GetISection(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref t2b, ref tfb, ref color, ref notes, ref guid);
                        if (t2 == t2b)
                            bhomProfile = BH.Engine.Geometry.Create.ISectionProfile(t3, t2, tw, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Geometry.Create.FabricatedISectionProfile(t3, t2, t2b, tw, tf, tfb, 0);
                        break;
                    case SAP.eFramePropType.Channel:
                        m_model.PropFrame.GetChannel(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.ChannelProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case SAP.eFramePropType.T:                        
                        break;
                    case SAP.eFramePropType.Angle:
                        m_model.PropFrame.GetAngle(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.AngleProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case SAP.eFramePropType.DblAngle:
                        break;
                    case SAP.eFramePropType.Box:
                        m_model.PropFrame.GetTube(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        if (tf == tw)
                            bhomProfile = BH.Engine.Geometry.Create.BoxProfile(t3, t2, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Geometry.Create.FabricatedBoxProfile(t3, t2, tw, tf, tf, 0);
                        break;
                    case SAP.eFramePropType.Pipe:
                        m_model.PropFrame.GetPipe(id, ref fileName, ref materialName, ref t3, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.TubeProfile(t3, tw);
                        break;
                    case SAP.eFramePropType.Rectangular:
                        m_model.PropFrame.GetRectangle(id, ref fileName, ref materialName, ref t3, ref t2, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.RectangleProfile(t3, t2, 0);
                        break;
                    case SAP.eFramePropType.Auto://not member will have this assigned but it still exists in the propertyType list
                        bhomProfile = BH.Engine.Geometry.Create.CircleProfile(0.2);
                        break;
                    case SAP.eFramePropType.Circle:
                        m_model.PropFrame.GetCircle(id, ref fileName, ref materialName, ref t3, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.CircleProfile(t3);
                        break;
                    case SAP.eFramePropType.General:
                        m_model.PropFrame.GetGeneral(id, ref fileName, ref materialName, ref t3, ref t2, ref Area, ref As2, ref As3, ref Torsion, ref I22, ref I33, ref S22, ref S33, ref Z22, ref Z33, ref R22, ref R33, ref color, ref notes, ref guid);
                        constructor = "explicit";
                        break;
                    case SAP.eFramePropType.DbChannel:                        
                        break;
                    case SAP.eFramePropType.SD:                        
                        break;
                    case SAP.eFramePropType.Variable:                        
                        break;
                    case SAP.eFramePropType.Joist:                        
                        break;
                    case SAP.eFramePropType.Bridge:                        
                        break;
                    case SAP.eFramePropType.Cold_C:                        
                        break;
                    case SAP.eFramePropType.Cold_2C:                        
                        break;
                    case SAP.eFramePropType.Cold_Z:
                        m_model.PropFrame.GetColdZ(id, ref fileName, ref materialName, ref t3, ref t2, ref tw, ref radius, ref tfb, ref angle, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.ZSectionProfile(t3, t2, tw, tw, radius, 0);
                        break;
                    case SAP.eFramePropType.Cold_L:                        
                        break;
                    case SAP.eFramePropType.Cold_2L:                        
                        break;
                    case SAP.eFramePropType.Cold_Hat:                        
                        break;
                    case SAP.eFramePropType.BuiltupICoverplate:                        
                        break;
                    case SAP.eFramePropType.PCCGirderI:                        
                        break;
                    case SAP.eFramePropType.PCCGirderU:                        
                        break;
                    case SAP.eFramePropType.BuiltupIHybrid:                        
                        break;
                    case SAP.eFramePropType.BuiltupUHybrid:                        
                        break;
                    case SAP.eFramePropType.Concrete_L:                        
                        break;
                    case SAP.eFramePropType.FilledTube:                        
                        break;
                    case SAP.eFramePropType.FilledPipe:                        
                        break;
                    case SAP.eFramePropType.EncasedRectangle:                        
                        break;
                    case SAP.eFramePropType.EncasedCircle:                        
                        break;
                    case SAP.eFramePropType.BucklingRestrainedBrace:
                        break;
                    case SAP.eFramePropType.CoreBrace_BRB:
                        break;
                    case SAP.eFramePropType.ConcreteTee:
                        break;
                    case SAP.eFramePropType.ConcreteBox:
                        break;
                    case SAP.eFramePropType.ConcretePipe:
                        break;
                    case SAP.eFramePropType.ConcreteCross:
                        break;
                    case SAP.eFramePropType.SteelPlate:
                        break;
                    case SAP.eFramePropType.SteelRod:
                        break;
                    default:                        
                        break;
                }

                IMaterialFragment material = null;

                try
                {
                    material = bhomMaterials[materialName];
                }
                catch (Exception)
                {
                    material = bhomMaterials.FirstOrDefault().Value;
                    Engine.Reflection.Compute.RecordWarning("Could not get material from SAP. Using a default material");
                }

                if (bhomProfile == null)
                {
                    Engine.Reflection.Compute.RecordWarning("Reading sections of type " + propertyType.ToString() + " is not supported. An empty section with a default material has been returned.");
                    constructor = "explicit";
                }

                switch (constructor)
                {
                    case "explicit":
                        bhomProperty = new ExplicitSection()
                        {
                            Area = Area,
                            Asy = As2,
                            Asz = As3,
                            Iy = I22,
                            Iz = I33,
                            J = Torsion,
                            Rgy = R22,
                            Rgz = R33,
                            Wply = S22,
                            Wplz = S33,
                            Wely = Z22,
                            Welz = Z33
                        };
                        break;
                    case "standard":
                        if (material is Aluminium || material is Steel)
                        {
                            bhomProperty = BH.Engine.Structure.Create.SteelSectionFromProfile(bhomProfile);
                        }
                        else if (material is Concrete)
                        {
                            bhomProperty = BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhomProfile);
                        }
                        else
                        {
                            bhomProperty = BH.Engine.Structure.Create.SteelSectionFromProfile(bhomProfile);
                            Engine.Reflection.Compute.RecordWarning("Reading sections of material type " + material.GetType().Name + "is not supported. Section with name " + id + " will be returned as a steel section");
                        }
                        break;
                }

                bhomProperty.Material = material;
                bhomProperty.Name = id;
                bhomProperty.CustomData[AdapterId] = id;

                propList.Add(bhomProperty);
            }
            return propList;
        }

        /***************************************************/
    }
}
