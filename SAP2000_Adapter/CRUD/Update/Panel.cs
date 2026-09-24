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
using BH.Engine.Structure;
using BH.oM.Adapter;
using BH.oM.Adapters.SAP2000;
using BH.oM.Architecture.Theatron;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using CSiAPIv1;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Panel                              ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<Panel> bhPanels)
        {
            //Make sure Diaphragms are pushed
            //List<Diaphragm> diaphragms = bhPanels.Select(x => x.Diaphragm()).Where(x => x != null).ToList();
            //this.FullCRUD(diaphragms, PushType.FullPush);

            bool success = true;
            m_model.SelectObj.ClearSelection();

            foreach (Panel bhPanel in bhPanels)
            {
                string name = GetAdapterId<string>(bhPanel);
                string propertyName = GetAdapterId<string>(bhPanel.Property);

                Engine.Base.Compute.RecordWarning("The SAP2000 API does not allow for updating of the geometry of panels. This includes the external edges as well as the openings. To update the panel geometry, delete the existing panel you want to update and create a new one.");

                m_model.AreaObj.SetProperty(name, propertyName);

                //Set local orientations:
                Basis orientation = bhPanel.LocalOrientation();
                //m_model.AreaObj.SetLocalAxes(name, Convert.ToEtabsPanelOrientation(orientation.Z, orientation.Y));

                //Diaphragm diaphragm = bhPanel.Diaphragm();

                //if (diaphragm != null)
                //{
                //    m_model.AreaObj.SetDiaphragm(name, diaphragm.Name);
                //}

                //Update Unique Name
                UpdateUniqueName(bhPanel);

                //Update Groups Assignment
                //if (!UpdateGroup(bhPanel)) success = false;

            }

            //Force refresh to make sure panel local orientation are set correctly
            ForceRefresh();

            return success;
        }

        /***************************************************/
    }
}




