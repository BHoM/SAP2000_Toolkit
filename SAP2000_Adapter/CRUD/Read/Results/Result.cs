/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Adapter;
using BH.oM.Analytical.Results;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
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
            int caseCount = 0;
            int comboCount = 0;
            string[] caseNames = { };
            string[] comboNames = { };
            m_model.LoadCases.GetNameList(ref caseCount, ref caseNames);
            m_model.RespCombo.GetNameList(ref comboCount, ref comboNames);

            List<string> loadcaseIds = new List<string>();

            //Get the ID for each case (so that we can accept both case names and case objects)
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

            loadcaseIds = FilterIds(loadcaseIds, caseNames.Concat(comboNames).ToArray());
            

            return loadcaseIds;
        }

        /***************************************************/

        private bool SetUpCaseOrCombo(string caseName)
        {
            if (m_model.Results.Setup.SetCaseSelectedForOutput(caseName) != 0)
            {
                if (m_model.Results.Setup.SetComboSelectedForOutput(caseName) != 0)
                {
                    Engine.Base.Compute.RecordWarning("Failed to setup result extraction for case " + caseName);
                    return false;
                }
            }
            return true;
        }


        /***************************************************/

        private void GetStepAndMode(string stepType, double stepNum, out double timeStep, out int mode)
        {
            /* Based on ETABS API outputs data structure, depending on the value of stepType, the stepNum parameter
             * has a different meaning. 
             * If stepType = "Mode", stepNum is assigned with the number of the corresponding Mode, otherwise it is 
             * assigned with the value of the timeStep.
             * Hence, the output parameters timeStep and mode are assigned with values based on a different logic
             * as in the below if statement.
             */
            
            if (stepType == "Mode")
            {
                mode = (int)stepNum;
                timeStep = 0;
            }
            else
            {
                timeStep = stepNum;
                mode = 0;
            }
        }

        /***************************************************/

        private List<string> CheckAndGetIds<T>(IEnumerable ids) where T : IBHoMObject
        {
            if (ids == null)
            {
                return null;
            }
            else
            {
                List<string> idsOut = new List<string>();
                foreach (object o in ids)
                {
                    if (o is string)
                        idsOut.Add((string)o);
                    else if (o is int || o is double)
                        idsOut.Add(o.ToString());
                    else if (o is T)
                    {
                        string id = GetAdapterId<string>((T)o);
                        if (id != null)
                            idsOut.Add(id);
                    }
                }
                return idsOut;
            }
        }

        /***************************************************/
    }
}





