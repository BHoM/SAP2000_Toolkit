using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> propList = new List<ISectionProperty>();
            Dictionary<string, IMaterialFragment> bhomMaterials = ReadMaterial().ToDictionary(x => x.CustomData[AdapterIdName].ToString());

            int nameCount = 0;
            string[] names = { };
            m_model.PropFrame.GetNameList(ref nameCount, ref names);

            if (ids == null)
            {
                ids = names.ToList();
            }
            
            foreach (string id in ids)
            {
                eFramePropType propertyType = eFramePropType.General;
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
                    case eFramePropType.I:
                        m_model.PropFrame.GetISection(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref t2b, ref tfb, ref color, ref notes, ref guid);
                        if (t2 == t2b)
                            bhomProfile = BH.Engine.Geometry.Create.ISectionProfile(t3, t2, tw, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Geometry.Create.FabricatedISectionProfile(t3, t2, t2b, tw, tf, tfb, 0);
                        break;
                    case eFramePropType.Channel:
                        m_model.PropFrame.GetChannel(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.ChannelProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case eFramePropType.T:                        
                        break;
                    case eFramePropType.Angle:
                        m_model.PropFrame.GetAngle(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.AngleProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case eFramePropType.DblAngle:
                        break;
                    case eFramePropType.Box:
                        m_model.PropFrame.GetTube(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        if (tf == tw)
                            bhomProfile = BH.Engine.Geometry.Create.BoxProfile(t3, t2, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Geometry.Create.FabricatedBoxProfile(t3, t2, tw, tf, tf, 0);
                        break;
                    case eFramePropType.Pipe:
                        m_model.PropFrame.GetPipe(id, ref fileName, ref materialName, ref t3, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.TubeProfile(t3, tw);
                        break;
                    case eFramePropType.Rectangular:
                        m_model.PropFrame.GetRectangle(id, ref fileName, ref materialName, ref t3, ref t2, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.RectangleProfile(t3, t2, 0);
                        break;
                    case eFramePropType.Auto://not member will have this assigned but it still exists in the propertyType list
                        bhomProfile = BH.Engine.Geometry.Create.CircleProfile(0.2);
                        break;
                    case eFramePropType.Circle:
                        m_model.PropFrame.GetCircle(id, ref fileName, ref materialName, ref t3, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.CircleProfile(t3);
                        break;
                    case eFramePropType.General:
                        m_model.PropFrame.GetGeneral(id, ref fileName, ref materialName, ref t3, ref t2, ref Area, ref As2, ref As3, ref Torsion, ref I22, ref I33, ref S22, ref S33, ref Z22, ref Z33, ref R22, ref R33, ref color, ref notes, ref guid);
                        constructor = "explicit";
                        break;
                    case eFramePropType.DbChannel:                        
                        break;
                    case eFramePropType.SD:                        
                        break;
                    case eFramePropType.Variable:                        
                        break;
                    case eFramePropType.Joist:                        
                        break;
                    case eFramePropType.Bridge:                        
                        break;
                    case eFramePropType.Cold_C:                        
                        break;
                    case eFramePropType.Cold_2C:                        
                        break;
                    case eFramePropType.Cold_Z:
                        m_model.PropFrame.GetColdZ(id, ref fileName, ref materialName, ref t3, ref t2, ref tw, ref radius, ref tfb, ref angle, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Geometry.Create.ZSectionProfile(t3, t2, tw, tw, radius, 0);
                        break;
                    case eFramePropType.Cold_L:                        
                        break;
                    case eFramePropType.Cold_2L:                        
                        break;
                    case eFramePropType.Cold_Hat:                        
                        break;
                    case eFramePropType.BuiltupICoverplate:                        
                        break;
                    case eFramePropType.PCCGirderI:                        
                        break;
                    case eFramePropType.PCCGirderU:                        
                        break;
                    case eFramePropType.BuiltupIHybrid:                        
                        break;
                    case eFramePropType.BuiltupUHybrid:                        
                        break;
                    case eFramePropType.Concrete_L:                        
                        break;
                    case eFramePropType.FilledTube:                        
                        break;
                    case eFramePropType.FilledPipe:                        
                        break;
                    case eFramePropType.EncasedRectangle:                        
                        break;
                    case eFramePropType.EncasedCircle:                        
                        break;
                    case eFramePropType.BucklingRestrainedBrace:
                        break;
                    case eFramePropType.CoreBrace_BRB:
                        break;
                    case eFramePropType.ConcreteTee:
                        break;
                    case eFramePropType.ConcreteBox:
                        break;
                    case eFramePropType.ConcretePipe:
                        break;
                    case eFramePropType.ConcreteCross:
                        break;
                    case eFramePropType.SteelPlate:
                        break;
                    case eFramePropType.SteelRod:
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
                bhomProperty.CustomData[AdapterIdName] = id;

                propList.Add(bhomProperty);
            }
            return propList;
        }

        /***************************************************/
    }
}
