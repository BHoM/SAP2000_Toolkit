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

using BH.Engine.Adapter;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override bool IUpdate<T>(IEnumerable<T> objects, ActionConfig actionConfig)
        {
            this.SAPPushConfig = actionConfig as SAP2000PushConfig;

            if (SAPPushConfig != null && SAPPushConfig.UpdateOnlyBarPropAssigns) // Only update bar assigns
            {
                return UpdateBarPropAssigns(objects.OfType<Bar>());
            }
            else
            {
                return UpdateObjects(objects as dynamic);
            }
        }

        /***************************************************/

        private bool UpdateObjects(IEnumerable<IBHoMObject> objects)
        {
            return base.IUpdate(objects, null);
        }

        /***************************************************/

        [Description("Concatenates the last 7 characters of the SAP2000 Element GUID and the Node Name to get the Unique Name to assign to the SAP2000 Element.")]
        private bool UpdateUniqueName(BHoMObject obj)
        {
            int ret01 = 1;
            int ret02 = 1;
            string guid = null;
            string tempObjName = "";

            /* 1. GET THE SAP2000 ELEMENT GUID */
            string uniqueName = GetAdapterId<string>(obj);

            if (obj.GetType() == typeof(Node)) ret01 = m_model.PointObj.GetGUID(uniqueName, ref guid);
            if (obj.GetType() == typeof(Bar)) ret01 = m_model.FrameObj.GetGUID(uniqueName, ref guid);
            if (obj.GetType() == typeof(Panel) || obj.GetType() == typeof(Opening)) ret01 = m_model.AreaObj.GetGUID(uniqueName, ref guid);

            /* 2. CREATE THE NEW UNIQUE NAME */
            if (obj.Name == "" || obj.Name == guid.Substring(guid.Length - 7))
            {
                tempObjName = guid.Substring(guid.Length - 7);
            }
            else
            {
                tempObjName = obj.Name + "::" + guid.Substring(guid.Length - 7);
            }

            /* 3. ASSIGN THE NEW UNIQUE NAME TO THE SAP2000 ELEMENT */
            if (obj.GetType() == typeof(Node)) ret02 = m_model.PointObj.ChangeName(uniqueName, tempObjName);
            if (obj.GetType() == typeof(Bar)) ret02 = m_model.FrameObj.ChangeName(uniqueName, tempObjName);
            if (obj.GetType() == typeof(Panel) || obj.GetType() == typeof(Opening)) ret02 = m_model.AreaObj.ChangeName(uniqueName, tempObjName);

            if (!(ret01 == 0 && ret02 == 0)) return false;

            SAP2000Id SAP2000IdFragment = new SAP2000Id { Id = tempObjName };

            obj.SetAdapterId(SAP2000IdFragment);

            return true;
        }

        /***************************************************/
    }
}






