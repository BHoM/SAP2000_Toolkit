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


using BH.Engine.Structure;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private int DeleteLoadcase(List<string> ids = null)
        {
            int count = 0;

            string[] patternNames = null;
            string[] caseNames = null;

            if (ids == null)
            {
                int nameCount = 0;
                m_model.LoadCases.GetNameList(ref nameCount, ref patternNames);
                m_model.LoadCases.GetNameList(ref nameCount, ref caseNames);
            }
            else
            {
                patternNames = ids.ToArray();
                caseNames = ids.ToArray();
            }

            foreach (string patternId in patternNames)
            {
                if (m_model.LoadPatterns.Delete(patternId) == 0)
                    count += 1;
                else
                    DeleteElementError("Load Pattern", patternId);
            }

            foreach (string caseId in caseNames)
            {
                if (m_model.LoadCases.Delete(caseId) == 0)
                    count += 1;
                else
                    DeleteElementError("Load Case", caseId);
            }

            return count;
        }

        /***************************************************/

        private int DeleteLoadCombination(List<string> ids = null)
        {
            int count = 0;

            if (ids == null)
            {
                int nameCount = 0;
                string[] nameArr = { };
                m_model.RespCombo.GetNameList(ref nameCount, ref nameArr);
                ids = nameArr.ToList();
            }

            foreach (string id in ids)
            {
                if (m_model.RespCombo.Delete(id) == 0)
                    count += 1;
                else
                    DeleteElementError("LoadCombination", id);
            }

            return count;
        }

        /***************************************************/
    }
}






