using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override int Delete(Type type, IEnumerable<object> ids)
        {
            return 0;
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
