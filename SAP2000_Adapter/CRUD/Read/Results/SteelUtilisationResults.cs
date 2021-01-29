/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

            return ReadBarUtilisation(barIds);

        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<CustomObject> ReadBarUtilisation(List<string> barIds = null)
        {
            List<BH.oM.Base.CustomObject> barUtilisations = new List<BH.oM.Base.CustomObject>();

            int tableId = 2; //Per table number in SAP, 2 corresponds to "Steel Design 2 - PMM Details"
            List<string> fieldNamesVal = new List<string>() { "TotalRatio", "PRatio", "MMajRatio", "MMinRatio", "VMajRatio", "VMinRatio", "TorRatio" };
            List<string> fieldNamesText = new List<string>() { "DesignSect", "DesignType", "Status", "Combo" };

            int numberItems = 0;
            string[] frameNames = null;
            double[] resultValues = null;
            string[] resultTextVals = null;
            string designCode = null;
            m_model.DesignSteel.GetCode(ref designCode);

            for (int i = 0; i < barIds.Count; i++)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (string fieldNameVal in fieldNamesVal)
                {
                    int ret = m_model.DesignSteel.GetDetailResultsValue(barIds[i],
                                                       eItemType.Objects,
                                                       tableId,
                                                       fieldNameVal,
                                                       ref numberItems,
                                                       ref frameNames,
                                                       ref resultValues);
                    if (ret == 0 && resultValues != null)
                    {
                        dict.Add(fieldNameVal, resultValues[0]);
                    }

                }
                foreach (string fieldNameText in fieldNamesText)
                {
                    int ret = m_model.DesignSteel.GetDetailResultsText(barIds[i],
                                                       eItemType.Objects,
                                                       tableId,
                                                       fieldNameText,
                                                       ref numberItems,
                                                       ref frameNames,
                                                       ref resultTextVals);
                    if (ret == 0 && resultTextVals != null)
                    {
                        dict.Add(fieldNameText, resultTextVals[0]);
                    }

                }
                dict.Add("DesignCode", designCode);
                CustomObject bu = BH.Engine.Base.Create.CustomObject(dict, barIds[i]);
                barUtilisations.Add(bu);
            }

            return barUtilisations;
        }

    }
}

