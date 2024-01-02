/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000.Results;
using BH.oM.Adapters.SAP2000.Requests;
using SAP2000v1;


namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IObject> ReadResults(SteelUtilisationRequest request,
                                                ActionConfig actionConfig = null)
        {
            CheckAndSetUpCases(request);
            List<string> barIds = CheckGetBarIds(request);

            switch (request.Code)
            {
                case oM.Adapters.SAP2000.SteelDesignCode.AISC_360_05:
                case oM.Adapters.SAP2000.SteelDesignCode.AISC_360_10:
                case oM.Adapters.SAP2000.SteelDesignCode.AISC_360_16:
                    return ReadAISCBarUtilisation(barIds);
                default:
                    Engine.Base.Compute.RecordError("Result extraction for request design code is not yet supported");
                    return new List<IResult>();
            }

        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<AISCSteelUtilisation> ReadAISCBarUtilisation(List<string> barIds = null)
        {
            List<AISCSteelUtilisation> barUtilisations = new List<AISCSteelUtilisation>();

            int tableId = 2; //Per table number in SAP, 2 corresponds to "Steel Design 2 - PMM Details"
            List<string> fieldNamesVal = new List<string>() { "TotalRatio", "PRatio", "MMajRatio", "MMinRatio", "VMajRatio", "VMinRatio", "TorRatio" };

            string designCode = null;
            m_model.DesignSteel.GetCode(ref designCode);

            for (int i = 0; i < barIds.Count; i++)
            {
                int ret;
                int numberItems = 0;
                string[] frameNames = null;
                string combo = "";
                double totalRatio, pRatio, vMajRatio, vMinRatio, torRatio, mMajRatio, mMinRatio;
                totalRatio = pRatio = vMajRatio = vMinRatio = torRatio = mMajRatio = mMinRatio = double.NaN;
                string designType = ""; double[] resultVals = null;
                string[] resultTextVals = null;
                Dictionary<string, double> valDict = new Dictionary<string, double>();

                AISCSteelUtilisation bu = null;

                foreach (string fieldNameVal in fieldNamesVal)
                {
                    ret = m_model.DesignSteel.GetDetailResultsValue(barIds[i],
                                                       eItemType.Objects,
                                                       tableId,
                                                       fieldNameVal,
                                                       ref numberItems,
                                                       ref frameNames,
                                                       ref resultVals);
                    if (ret == 0 && resultVals != null)
                    {
                         valDict.Add(fieldNameVal, resultVals[0]);
                    }
                }
                if (valDict.Count() == 0)
                {
                    barUtilisations.Add(bu);
                    continue;
                }

                ret = m_model.DesignSteel.GetDetailResultsText(barIds[i],
                                                       eItemType.Objects,
                                                       tableId,
                                                       "DesignType",
                                                       ref numberItems,
                                                       ref frameNames,
                                                       ref resultTextVals);
               if (ret == 0 && resultTextVals != null)
               {
                    designType = resultTextVals[0];
               }
               ret = m_model.DesignSteel.GetDetailResultsText(barIds[i],
                                        eItemType.Objects,
                                        tableId,
                                        "Combo",
                                        ref numberItems,
                                        ref frameNames,
                                        ref resultTextVals);
                if (ret == 0 && resultTextVals != null)
                {
                    combo = resultTextVals[0];
                }

                valDict.TryGetValue("TotalRatio", out totalRatio);
                valDict.TryGetValue("PRatio", out pRatio);
                valDict.TryGetValue("VMajRatio", out vMajRatio);
                valDict.TryGetValue("VMinRatio", out vMinRatio);
                valDict.TryGetValue("TorRatio", out torRatio);
                valDict.TryGetValue("MMajRatio", out mMajRatio);
                valDict.TryGetValue("MMinRatio", out mMinRatio);

                bu = new AISCSteelUtilisation(barIds[i], combo, 0, 0, 0, 0, designCode, "", "", designType, totalRatio, pRatio, vMajRatio, vMinRatio, torRatio, mMajRatio, mMinRatio);
                barUtilisations.Add(bu);
            }

            return barUtilisations;
        }

    }
}




