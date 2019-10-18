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
using BH.oM.Analytical;

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            bool success = true;

            if (typeof(BH.oM.Base.IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                success = CreateCollection(objects);
            }
            else
            {
                success = false;
            }

            m_model.View.RefreshView();
            return success;
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateCollection<T>(IEnumerable<T> objects) where T : BH.oM.Base.IObject
        {
            bool success = true;

            foreach (T obj in objects)
            {
                success &= CreateObject(obj as dynamic);
            }
            
            return success;
        }

        /***************************************************/
    }
}
