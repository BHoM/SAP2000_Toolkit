using BH.Engine.Base.Objects;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** BHoM Adapter Interface                    ****/
        /***************************************************/
        
        //Compares nodes by distance (down to 3 decimal places -> mm)
        //Compares Materials, SectionProprties, LinkConstraints, and Property2D by name
        //Add/remove any type in the dictionary below that you want (or not) a specific comparison method for.

        private void SetupComparers()
        {
            AdapterComparers = new Dictionary<Type, object>
            {
                {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) },   //The 3 in here sets how many decimal places to look at for node merging. 3 decimal places gives mm precision
                {typeof(ISectionProperty), new BHoMObjectNameOrToStringComparer() },
                {typeof(IMaterialFragment), new BHoMObjectNameComparer() },
                {typeof(LinkConstraint), new BHoMObjectNameComparer() },
                {typeof(ISurfaceProperty), new BHoMObjectNameComparer() },
                {typeof(ICase), new BHoMObjectNameComparer() },
            };
        }

        /***************************************************/
    }
}