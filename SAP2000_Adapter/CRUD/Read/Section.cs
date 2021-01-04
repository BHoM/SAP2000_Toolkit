/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Spatial.ShapeProfiles;
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
            Dictionary<string, IMaterialFragment> bhomMaterials = ReadMaterial().ToDictionary(x => GetAdapterId<string>(x));

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
                SAP2000Id sap2000id = new SAP2000Id();

                sap2000id.Id = id;

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
                            bhomProfile = BH.Engine.Spatial.Create.ISectionProfile(t3, t2, tw, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Spatial.Create.FabricatedISectionProfile(t3, t2, t2b, tw, tf, tfb, 0);
                        break;
                    case eFramePropType.Channel:
                        m_model.PropFrame.GetChannel(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.ChannelProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case eFramePropType.T:                        
                        break;
                    case eFramePropType.Angle:
                        m_model.PropFrame.GetAngle(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.AngleProfile(t3, t2, tw, tf, 0, 0);
                        break;
                    case eFramePropType.DblAngle:
                        break;
                    case eFramePropType.Box:
                        m_model.PropFrame.GetTube(id, ref fileName, ref materialName, ref t3, ref t2, ref tf, ref tw, ref color, ref notes, ref guid);
                        if (tf == tw)
                            bhomProfile = BH.Engine.Spatial.Create.BoxProfile(t3, t2, tf, 0, 0);
                        else
                            bhomProfile = BH.Engine.Spatial.Create.FabricatedBoxProfile(t3, t2, tw, tf, tf, 0);
                        break;
                    case eFramePropType.Pipe:
                        m_model.PropFrame.GetPipe(id, ref fileName, ref materialName, ref t3, ref tw, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.TubeProfile(t3, tw);
                        break;
                    case eFramePropType.Rectangular:
                        m_model.PropFrame.GetRectangle(id, ref fileName, ref materialName, ref t3, ref t2, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.RectangleProfile(t3, t2, 0);
                        break;
                    case eFramePropType.Auto://not member will have this assigned but it still exists in the propertyType list
                        bhomProfile = BH.Engine.Spatial.Create.CircleProfile(0.2);
                        break;
                    case eFramePropType.Circle:
                        m_model.PropFrame.GetCircle(id, ref fileName, ref materialName, ref t3, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.CircleProfile(t3);
                        break;
                    case eFramePropType.General:
                        m_model.PropFrame.GetGeneral(id, ref fileName, ref materialName, ref t3, ref t2, ref Area, ref As2, ref As3, ref Torsion, ref I22, ref I33, ref S22, ref S33, ref Z22, ref Z33, ref R22, ref R33, ref color, ref notes, ref guid);
                        constructor = "explicit";
                        break;
                    case eFramePropType.Cold_Z:
                        m_model.PropFrame.GetColdZ(id, ref fileName, ref materialName, ref t3, ref t2, ref tw, ref radius, ref tfb, ref angle, ref color, ref notes, ref guid);
                        bhomProfile = BH.Engine.Spatial.Create.ZSectionProfile(t3, t2, tw, tw, radius, 0);
                        break;
                    case eFramePropType.DbChannel:
                    case eFramePropType.SD:
                    case eFramePropType.Variable:
                    case eFramePropType.Joist:
                    case eFramePropType.Bridge:
                    case eFramePropType.Cold_C:
                    case eFramePropType.Cold_2C:
                    case eFramePropType.Cold_L:
                    case eFramePropType.Cold_2L:
                    case eFramePropType.Cold_Hat:
                    case eFramePropType.BuiltupICoverplate:
                    case eFramePropType.PCCGirderI:
                    case eFramePropType.PCCGirderU:
                    case eFramePropType.BuiltupIHybrid:
                    case eFramePropType.BuiltupUHybrid:
                    case eFramePropType.Concrete_L:
                    case eFramePropType.FilledTube:
                    case eFramePropType.FilledPipe:
                    case eFramePropType.EncasedRectangle:
                    case eFramePropType.EncasedCircle:
                    case eFramePropType.BucklingRestrainedBrace:
                    case eFramePropType.CoreBrace_BRB:
                    case eFramePropType.ConcreteTee:
                    case eFramePropType.ConcreteBox:
                    case eFramePropType.ConcretePipe:
                    case eFramePropType.ConcreteCross:
                    case eFramePropType.SteelPlate:
                    case eFramePropType.SteelRod:
                    default:                        
                        break;
                }

                IMaterialFragment material = null;
                if (!bhomMaterials.TryGetValue(materialName, out material))
                {
                    Engine.Reflection.Compute.RecordWarning($"Could not get material for SectionProperty {id}. A generic has been returned.");
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
                        bhomProperty = BH.Engine.Structure.Create.SectionPropertyFromProfile(bhomProfile, material, id);
                        break;
                }

                bhomProperty.SetAdapterId(sap2000id);
                propList.Add(bhomProperty);
            }
            return propList;
        }

        /***************************************************/
    }
}

