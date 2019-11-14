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

        //Standard implementation for dependency types (change the dictionary below to override):

        protected override List<Type> DependencyTypes<T>()
        {
            Type type = typeof(T);

            if (m_DependencyTypes.ContainsKey(type))
                return m_DependencyTypes[type];

            else if (type.BaseType != null && m_DependencyTypes.ContainsKey(type.BaseType))
                return m_DependencyTypes[type.BaseType];

            else
            {
                foreach (Type interType in type.GetInterfaces())
                {
                    if (m_DependencyTypes.ContainsKey(interType))
                        return m_DependencyTypes[interType];
                }
            }


            return new List<Type>();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Type, List<Type>> m_DependencyTypes = new Dictionary<Type, List<Type>>
        {
            {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } },
            {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
            {typeof(Panel), new List<Type> { typeof(ISurfaceProperty) } },
            {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
            {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
            {typeof(ILoad), new List<Type> {typeof(Loadcase) } },
            {typeof(LoadCombination), new List<Type> {typeof(Loadcase) } }
        };

        /***************************************************/
    }
}
