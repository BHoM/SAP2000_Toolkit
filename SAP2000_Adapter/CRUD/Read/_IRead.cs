using BH.oM.Base;
using BH.oM.Common.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            if (type == typeof(Node))
                return ReadNodes(ids as dynamic);
            else if (type == typeof(Bar))
                return ReadBars(ids as dynamic);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return ReadSectionProperties(ids as dynamic);
            else if (type == typeof(IMaterialFragment) || type.GetInterfaces().Contains(typeof(IMaterialFragment)))
                return ReadMaterial(ids as dynamic);
            else if (type == typeof(Panel))
                return ReadPanel(ids as dynamic);
            else if (type == typeof(ISurfaceProperty))
                return ReadSurfaceProperty(ids as dynamic);
            else if (type == typeof(LoadCombination))
                return ReadLoadCombination(ids as dynamic);
            else if (type == typeof(Loadcase))
                return ReadLoadcase(ids as dynamic);
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return ReadLoad(type, ids as dynamic);
            else if (type == typeof(RigidLink))
                return ReadRigidLink(ids as dynamic);
            else if (type == typeof(LinkConstraint))
                return ReadLinkConstraints(ids as dynamic);


            return null;
        }

        /***************************************************/
    }
}
