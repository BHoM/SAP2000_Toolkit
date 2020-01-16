﻿using BH.oM.Structure.Constraints;
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

        private List<LinkConstraint> ReadLinkConstraints(List<string> ids = null)
        {
            List<LinkConstraint> propList = new List<LinkConstraint>();

            if (ids == null)
            {
                int nameCount = 0;
                string[] names = { };
                m_model.PropLink.GetNameList(ref nameCount, ref names);
                ids = names.ToList();
            }

            foreach (string id in ids)
            {
                eLinkPropType linkType = eLinkPropType.Linear;
                m_model.PropLink.GetTypeOAPI(id, ref linkType);

                LinkConstraint constr = null;

                switch (linkType)
                {
                    case eLinkPropType.Linear:
                        constr = GetLinearLinkConstraint(id);
                        break;
                    case eLinkPropType.Damper:
                    case eLinkPropType.Gap:
                    case eLinkPropType.Hook:
                    case eLinkPropType.PlasticWen:
                    case eLinkPropType.Isolator1:
                    case eLinkPropType.Isolator2:
                    case eLinkPropType.MultilinearElastic:
                    case eLinkPropType.MultilinearPlastic:
                    case eLinkPropType.Isolator3:
                    default:
                        Engine.Reflection.Compute.RecordError("Reading of LinkConstraint of type " + linkType + " not implemented");
                        break;
                }
                
                if (constr != null)
                    propList.Add(constr);
                else
                    Engine.Reflection.Compute.RecordError("Failed to read link constraint with id :" + id);

            }
            return propList;
        }
        
        /***************************************************/

        private LinkConstraint GetLinearLinkConstraint(string name)
        {
            LinkConstraint constraint = new LinkConstraint(); bool[] dof = null;

            bool[] fix = null;
            double[] stiff = null;
            double[] damp = null;
            double dj2 = 0;
            double dj3 = 0;
            bool stiffCoupled = false;
            bool dampCoupled = false;
            string notes = null;
            string guid = null;

            m_model.PropLink.GetLinear(name, ref dof, ref fix, ref stiff, ref damp, ref dj2, ref dj3, ref stiffCoupled, ref dampCoupled, ref notes, ref guid);
            
            constraint.Name = name;
            constraint.CustomData[AdapterIdName] = name;
            constraint.XtoX = fix[0];
            constraint.ZtoZ = fix[1];
            constraint.YtoY = fix[2];
            constraint.XXtoXX = fix[3];
            constraint.YYtoYY = fix[4];
            constraint.ZZtoZZ = fix[5];

            if (stiff != null && stiff.Any(x => x != 0))
                Engine.Reflection.Compute.RecordWarning("No stiffness read for link constraints");

            if (damp != null && damp.Any(x => x != 0))
                Engine.Reflection.Compute.RecordWarning("No damping read for link contraint");

            return constraint;
        }

        /***************************************************/
    }
}