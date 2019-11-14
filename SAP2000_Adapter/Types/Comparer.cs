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

        //Standard implementation of the comparer class.
        //Compares nodes by distance (down to 3 decimal places -> mm)
        //Compares Materials, SectionProprties, LinkConstraints, and Property2D by name
        //Add/remove any type in the dictionary below that you want (or not) a specific comparison method for

        protected override IEqualityComparer<T> Comparer<T>()
        {
            Type type = typeof(T);

            if (m_Comparers.ContainsKey(type))
                return m_Comparers[type] as IEqualityComparer<T>;

            else if (type.BaseType != null && m_Comparers.ContainsKey(type.BaseType))
                return m_Comparers[type.BaseType] as IEqualityComparer<T>;

            else
            {
                foreach (Type interType in type.GetInterfaces())
                {
                    if (m_Comparers.ContainsKey(interType))
                        return m_Comparers[interType] as IEqualityComparer<T>;
                }

                return EqualityComparer<T>.Default;
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Type, object> m_Comparers = new Dictionary<Type, object>
        {
            {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) },   //The 3 in here sets how many decimal places to look at for node merging. 3 decimal places gives mm precision
            {typeof(ISectionProperty), new BHoMObjectNameOrToStringComparer() },
            {typeof(IMaterialFragment), new BHoMObjectNameComparer() },
            {typeof(LinkConstraint), new BHoMObjectNameComparer() },
            {typeof(ISurfaceProperty), new BHoMObjectNameComparer() },
            {typeof(ICase), new BHoMObjectNameComparer() },

        };


        /***************************************************/
    }
}