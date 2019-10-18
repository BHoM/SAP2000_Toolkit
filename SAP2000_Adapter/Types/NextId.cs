using System;
using System.Collections.Generic;
using System.Linq;

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

        private Dictionary<Type, string> idDictionary = new Dictionary<Type, string>();

        protected override object NextId(Type objectType, bool refresh = false)
        {
            return -1;
        }
    }
}
