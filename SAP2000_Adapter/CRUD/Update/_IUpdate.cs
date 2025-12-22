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
using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.Adapters.SAP2000;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
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
    }
}






