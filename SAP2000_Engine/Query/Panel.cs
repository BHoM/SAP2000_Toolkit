/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.SAP2000.Fragments;
using BH.oM.Adapters.SAP2000;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using BH.oM.Base;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Query
    {
        [Description("Returns the SAP2000 PanelAutoMesh settings for a panel. You can also use the method FindFragment() with the type IPanelAutoMesh as an argument.")]
        [Input("panel", "A panel which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("PanelAutoMesh", "A fragment containing SAP2000 PanelAutoMesh settings.")]
        public static IPanelAutoMesh PanelAutoMesh(this Panel panel)
        {
            List<IFragment> fragments = panel?.GetAllFragments(typeof(IPanelAutoMesh));

            if (fragments == null || fragments.Count == 0)
                return null;

            if (fragments.Count > 1) 
                Compute.RecordWarning($"the panel {panel.Name} has more than one PanelAutoMesh defined, which is not allowed. Only the first has been returned. Use GetAllFragments() to extract others.");
            
            return fragments.Select(x => x as IPanelAutoMesh).FirstOrDefault();
        }

        [Description("Returns the SAP2000 PanelEdgeConstraint settings for a panel. You can also use the method FindFragment() with the type PanelEdgeConstraint as an argument.")]
        [Input("panel", "A panel which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("PanelEdgeConstraint", "A fragment containing SAP2000 PanelEdgeConstraint settings.")]
        public static PanelEdgeConstraint PanelEdgeConstraint(this Panel panel)
        {
            return panel?.FindFragment<PanelEdgeConstraint>();
        }

        [Description("Returns the SAP2000 PanelOffset settings for a panel. You can also use the method FindFragment() with the type IPanelOffset as an argument.")]
        [Input("panel", "A panel which was either pulled from SAP2000 or which has had SAP2000 settings added.")]
        [Output("PanelOffset", "A fragment containing SAP2000 PanelOffset settings.")]
        public static IPanelOffset PanelOffset(this Panel panel)
        {
            List<IFragment> fragments = panel?.GetAllFragments(typeof(IPanelOffset));

            if (fragments == null || fragments.Count == 0)
                return null;
            
            if (fragments.Count > 1) 
                Compute.RecordWarning($"the panel {panel.Name} has more than one PanelOffset defined, which is not allowed. Only the first has been returned. Use GetAllFragments() to extract others.");
            
            return fragments.Select(x => x as IPanelOffset).FirstOrDefault();
        }
    }
}
