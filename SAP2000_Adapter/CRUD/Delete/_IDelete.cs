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

using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;
using BH.oM.Data.Requests;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        
        protected override int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            List<string> idString = ids.Cast<string>().ToList();

            if (type == typeof(Node))
                return DeleteNodes(idString);
            else if (type == typeof(Bar))
                return DeleteBars(idString);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return DeleteSectionProperties(idString);
            else if (type == typeof(IMaterialFragment) || type.GetInterfaces().Contains(typeof(IMaterialFragment)))
                return DeleteMaterial(idString);
            else if (type == typeof(Panel))
                return DeletePanel(idString);
            else if (type == typeof(ISurfaceProperty))
                return DeleteSurfaceProperty(idString);
            else if (type == typeof(LoadCombination))
                return DeleteLoadCombination(idString);
            else if (type == typeof(Loadcase))
                return DeleteLoadcase(idString);
            else if (type == typeof(RigidLink))
                return DeleteRigidLink(idString);
            else if (type == typeof(LinkConstraint))
                return DeleteLinkConstraints(idString);

            return 0;
        }

        /***************************************************/

        protected override int Delete(FilterRequest request, ActionConfig actionConfig = null)
        {
                // Get object ids
                List<object> objectIds = new List<object>();
                object idObject;
            if (request.Equalities.TryGetValue("ObjectIds", out idObject))

            {
                objectIds = (List<object>)idObject;

                // Delete 
                return IDelete(request.Type, objectIds, actionConfig);
            }

            return 0;
    
        }

        /***************************************************/
    }
}






