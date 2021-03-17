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
using BH.oM.Analytical.Results;
using BH.oM.Structure.Results;
using BH.oM.Structure.Requests;
using System.ComponentModel;
using BH.oM.Data.Requests;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {

            List<string> listIds = null;
            if (ids != null && ids.Count != 0)
                listIds = ids.Cast<string>().ToList();

            if (type == typeof(Node))
                return ReadNodes(listIds);
            else if (type == typeof(Bar))
                return ReadBars(listIds);
            else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                return ReadSectionProperties(listIds);
            else if (type == typeof(IMaterialFragment) || type.GetInterfaces().Contains(typeof(IMaterialFragment)))
                return ReadMaterial(listIds);
            else if (type == typeof(Panel))
                return ReadPanel(listIds);
            else if (type == typeof(ISurfaceProperty) || type.GetInterfaces().Contains(typeof(ISurfaceProperty)))
                return ReadSurfaceProperty(listIds);
            else if (type == typeof(LoadCombination))
                return ReadLoadCombination(listIds);
            else if (type == typeof(Loadcase))
                return ReadLoadcase(listIds);
            else if (type == typeof(ILoad) || type.GetInterfaces().Contains(typeof(ILoad)))
                return ReadLoad(type, listIds);
            else if (type == typeof(RigidLink))
                return ReadRigidLink(listIds);
            else if (type == typeof(LinkConstraint))
                return ReadLinkConstraints(listIds);
            else if (typeof(IResult).IsAssignableFrom(type))
            {
                Modules.Structure.ErrorMessages.ReadResultsError(type);
                return null;
            }


            return new List<IBHoMObject>();

        }
        /***************************************************/

        public IEnumerable<IBHoMObject> Read(SelectionRequest request, ActionConfig actionConfig = null)
        {
            List<IBHoMObject> results = new List<IBHoMObject>();

            foreach (KeyValuePair<Type, List<string>> keyVal in SelectedElements())
            {
                results.AddRange(IRead(keyVal.Key, keyVal.Value, actionConfig));
            }

            return results;
        }

        /***************************************************/

        public Dictionary<Type, List<string>> SelectedElements()
        {
            int numItems = 0;
            int[] objectTypes = new int[0];
            string[] objectIds = new string[0];

            m_model.SelectObj.GetSelected(ref numItems, ref objectTypes, ref objectIds);

            Dictionary<int, List<string>> dict = objectTypes.Distinct().ToDictionary(x => x, x => new List<string>());

            for (int i = 0; i < numItems; i++)
            {
                dict[objectTypes[i]].Add(objectIds[i]);
            }

            Func<int, Type> ToType = x =>
            {
                switch (x)
                {
                    case 1:
                        return typeof(Node);
                    case 2:
                    case 3:
                    case 4:
                        return typeof(Bar);
                    case 5:
                        return typeof(Panel);
                    case 6:
                        return null;
                    case 7:
                        return typeof(RigidLink);
                    default:
                        return null; 


                }
            };

            return dict.ToDictionary(x => ToType(x.Key), x => x.Value);
            

        }
        /***************************************************/

        [Description("Ensures that all elements in the first list are present in the second list, warning if not, and returns the second list if the first list is empty.")]
        private static List<string> FilterIds(IEnumerable<string> ids, IEnumerable<string> sapIds)
        {
            if (sapIds == null)
            {
                sapIds = Enumerable.Empty<string>();
            }
            if (ids == null || ids.Count() == 0)
            {
                return sapIds.ToList();
            }
            else
            {
                List<string> result = ids.Intersect(sapIds).ToList();
                if (result.Count() != ids.Count())
                    Engine.Reflection.Compute.RecordWarning("Some requested SAP2000 ids were not present in the model.");
                return result;
            }
        }

        /***************************************************/
    }
}

