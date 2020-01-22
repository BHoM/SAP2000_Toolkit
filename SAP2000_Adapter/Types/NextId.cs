using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
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
