using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Results;
using BH.oM.Common;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.SAP2000;
using BH.oM.Structure.Loads;
using BH.oM.Data.Requests;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        protected override IEnumerable<IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            IResultRequest request = Engine.Structure.Create.IResultRequest(type, ids?.Cast<object>(), cases?.Cast<object>(), divisions);

            if (request != null)
                return this.ReadResults(request as dynamic);
            else
                return new List<IResult>();
        }
    }
