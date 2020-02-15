using System;
using System.Collections;
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
using BH.oM.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        protected override IEnumerable<IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5, ActionConfig actionConfig = null)
        {
            IResultRequest request = Engine.Structure.Create.IResultRequest(type, ids?.Cast<object>(), cases?.Cast<object>(), divisions);

            if (request != null)
                return this.ReadResults(request as dynamic);
            else
                return new List<IResult>();
        }

        /***************************************************/

        private List<string> CheckAndSetUpCases(IResultRequest request)
        {
            return CheckAndSetUpCases(request.Cases);
        }

        /***************************************************/

        private List<string> CheckAndSetUpCases(IList cases)
        {
            List<string> loadcaseIds = GetAllCases(cases);

            m_model.Results.Setup.DeselectAllCasesAndCombosForOutput();

            for (int loadcase = 0; loadcase < loadcaseIds.Count; loadcase++)
            {
                SetUpCaseOrCombo(loadcaseIds[loadcase]);
            }

            return loadcaseIds;
        }

        /***************************************************/

        private List<string> GetAllCases(IList cases)
        {
            List<string> loadcaseIds = new List<string>();

            if (cases == null || cases.Count == 0)
            {
                int Count = 0;
                string[] case_names = null;
                string[] combo_names = null;
                m_model.LoadCases.GetNameList(ref Count, ref case_names);
                m_model.RespCombo.GetNameList(ref Count, ref combo_names);
                loadcaseIds = case_names.ToList();

                if (combo_names != null)
                {
                    loadcaseIds.AddRange(combo_names);
                }
            }
            else
            {
                foreach (object thisCase in cases)
                {
                    if (thisCase is ICase)
                    {
                        ICase bhCase = thisCase as ICase;
                        loadcaseIds.Add(bhCase.Name.ToString());
                    }
                    else if (thisCase is string)
                    {
                        string caseId = thisCase as string;
                        loadcaseIds.Add(caseId);
                    }
                }
            }

            return loadcaseIds;
        }

        /***************************************************/

        private bool SetUpCaseOrCombo(string caseName)
        {
            if (m_model.Results.Setup.SetCaseSelectedForOutput(caseName) != 0)
            {
                if (m_model.Results.Setup.SetComboSelectedForOutput(caseName) != 0)
                {
                    Engine.Reflection.Compute.RecordWarning("Failed to setup result extraction for case " + caseName);
                    return false;
                }
            }
            return true;
        }
    }
}
