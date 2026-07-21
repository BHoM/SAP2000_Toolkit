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
using BH.oM.Adapters.SAP2000;
using BH.oM.Analytical;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig)
        {
            bool success = true;

            this.SAPPushConfig = actionConfig as SAP2000PushConfig;

            if (!objects.Any()) //Return if no objects
                return true;

            if (typeof(BH.oM.Base.IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                success = CreateCollection(objects);
            }
            else
            {
                success = false;
            }

            // Refresh the view to ensure that the newly created objects are displayed correctly
            m_model.View.RefreshView(0, false);

            return success;
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateCollection<T>(IEnumerable<T> objects) where T : BH.oM.Base.IObject
        {
            bool success = true;

            foreach (T obj in objects)
            {
                success &= CreateObject(obj as dynamic);
            }
            
            return success;
        }

        /***************************************************/
        private bool CreateObject(IBHoMObject obj)
        {
            Engine.Base.Compute.RecordWarning($"Objects of type {obj.GetType()} are not supported by the SAP2000 Adapter");
            return false;
        }

        /***************************************************/

        [Description("Concatenates the the BHoM Object Name and the last 7 characters of the SAP2000 Element GUID to get the Unique Name to assign to the SAP2000 Element.")]
        private string SetUniqueName(BHoMObject obj, string name)
        {
            /* 1. CHECK OBJECT TYPE IS ACCEPTABLE */
            if (!(obj.GetType() == typeof(Node) ||
                  obj.GetType() == typeof(Bar) ||
                  obj.GetType() == typeof(Panel) ||
                  obj.GetType() == typeof(Opening)))
            {
                return null;
            }

            /* 2. GET THE SAP2000 ELEMENT GUID */
            int ret01 = 1;
            int ret02 = 1;
            string guid = null;
            string tempObjName = "";

            if (obj.GetType() == typeof(Node)) ret01 = m_model.PointObj.GetGUID(name, ref guid);
            if (obj.GetType() == typeof(Bar)) ret01 = m_model.FrameObj.GetGUID(name, ref guid);
            if (obj.GetType() == typeof(Panel) || obj.GetType() == typeof(Opening)) ret01 = m_model.AreaObj.GetGUID(name, ref guid);

            /* 3. CREATE THE NEW UNIQUE NAME */
            if (obj.Name == "")
            {
                tempObjName = guid.Substring(guid.Length - 7);
            }
            else
            {
                tempObjName = obj.Name + "::" + guid.Substring(guid.Length - 7);
            }

            /* 4. ASSIGN THE NEW UNIQUE NAME TO THE SAP2000 ELEMENT */
            if (obj.GetType() == typeof(Node)) ret02 = m_model.PointObj.ChangeName(name, tempObjName);
            if (obj.GetType() == typeof(Bar)) ret02 = m_model.FrameObj.ChangeName(name, tempObjName);
            if (obj.GetType() == typeof(Panel) || obj.GetType() == typeof(Opening)) ret02 = m_model.AreaObj.ChangeName(name, tempObjName);

            if (!(ret01 == 0 && ret02 == 0)) return null;

            return tempObjName;

            /***************************************************/
        }
    }
}






