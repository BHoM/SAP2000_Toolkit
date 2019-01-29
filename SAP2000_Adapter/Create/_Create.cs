using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {

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
