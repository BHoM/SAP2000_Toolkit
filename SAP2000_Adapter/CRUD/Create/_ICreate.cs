/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Common.Materials;
using BH.oM.Dimensional;
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
using BH.oM.Analytical;
using BH.oM.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            if (typeof(BH.oM.Base.IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                return (CreateCollection(objects));
            }
            else
            {
                return false;
            }
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
    }
}
