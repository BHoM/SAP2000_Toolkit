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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Structure.Elements;
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Units;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapters.SAP2000;
using BH.oM.Adapters.SAP2000.Fragments;
using BH.Engine.Adapter;
using System.Reflection;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Panel bhPanel)
        {
            double mergeTol = 1E-6;

            List<Point> boundaryPoints = null;

            //Check for dealbreaking BHoM invalidity
            try
            { 
                boundaryPoints = bhPanel.ControlPoints(true).CullDuplicates(mergeTol);
            }
            catch
            {
                Engine.Base.Compute.RecordError($"Panel {bhPanel.Name} could not be created, because its geometry could not be determined");
                return false;
            }

            int segmentCount = boundaryPoints.Count();

            double[] x = boundaryPoints.Select(item => item.X).ToArray();
            double[] y = boundaryPoints.Select(item => item.Y).ToArray();
            double[] z = boundaryPoints.Select(item => item.Z).ToArray();

            string name = "";

            // Create Geometry in SAP
            if (m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, "None", bhPanel.Name.ToString()) != 0)
            {
                CreateElementError("Panel", bhPanel.Name);
                return false;
            }

            // Set AdapterID
            if (name != bhPanel.Name & bhPanel.Name != "")
                Engine.Base.Compute.RecordNote($"Panel {bhPanel.Name} was assigned SAP2000_id of {name}");

            string guid = null;
            m_model.AreaObj.GetGUID(name, ref guid);

            SAP2000Id sap2000IdFragment = new SAP2000Id { Id = name, PersistentId = guid };
            bhPanel.SetAdapterId(sap2000IdFragment);

            // Set Properties
            SetObject(bhPanel);

            return true;
        }

        /***************************************************/

        private bool SetObject(Panel bhPanel)
        {
            string name = GetAdapterId<string>(bhPanel);

            if (bhPanel.Property != null)
            {
                if (m_model.AreaObj.SetProperty(name, GetAdapterId<string>(bhPanel.Property), 0) != 0)
                    CreatePropertyError("Surface Property", "Panel", name);
            }

            foreach (string gName in bhPanel.Tags)
            {
                if (gName != null)
                {
                    string groupName = gName.ToString();
                    if (m_model.AreaObj.SetGroupAssign(name, groupName) != 0)
                    {
                        m_model.GroupDef.SetGroup(groupName);
                        m_model.AreaObj.SetGroupAssign(name, groupName);
                    }
                }
            }

            //Set AutoMesh
            foreach (IFragment fragment in bhPanel.GetAllFragments())
            {
                if (SetPanelFragment(fragment as dynamic, name) == false)
                {
                    CreatePropertyError(fragment.GetType().ToString(), "Panel", name);
                }
            }

            return true;
        }

        /***************************************************/

        private bool SetPanelFragment(SAP2000Id fragment, string name)
        {
            return true;
        }

        /***************************************************/

        private bool SetPanelFragment(IFragment fragment, string name)
        {
            return false;
        }

        /***************************************************/

        private bool SetPanelFragment(IPanelAutoMesh fragment, string name)
        {
            return false;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByNumberOfObjects fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                1,
                fragment.N1,
                fragment.N2,
                0,
                0,
                false,
                false,
                false,
                0,
                0,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByMaximumSize fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                2,
                2,
                2,
                fragment.MaxSize1,
                fragment.MaxSize2,
                false,
                false,
                false,
                0,
                0,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByPointsOnEdges fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                3,
                2,
                2,
                0,
                0,
                fragment.PointOnEdgeFromLine,
                fragment.PointOnEdgeFromPoint,
                false,
                0,
                0,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByCookieCutLines fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                4,
                2,
                2,
                0,
                0,
                false,
                false,
                fragment.ExtendCookieCutLines,
                0,
                0,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByCookieCutPoints fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                5,
                2,
                2,
                0,
                0,
                false,
                false,
                false,
                fragment.Rotation.ToDegree(),
                0,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelAutoMeshByGeneralDivide fragment, string name)
        {
            return m_model.AreaObj.SetAutoMesh(name,
                6,
                2,
                2,
                0,
                0,
                false,
                false,
                false,
                0,
                fragment.MaxSizeGeneral,
                fragment.LocalAxesOnEdge,
                fragment.LocalAxesOnFace,
                fragment.RestraintsOnEdge,
                fragment.RestraintsOnFace,
                fragment.Group,
                fragment.SubMesh,
                fragment.SubMeshSize) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelEdgeConstraint fragment, string name)
        {
            return m_model.AreaObj.SetEdgeConstraint(name, fragment.EdgeConstraint) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(IPanelOffset fragment, string name)
        {
            return false;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelOffsetByJointPattern fragment, string name)
        {
            double[] offsets = new double[4];
            return m_model.AreaObj.SetOffsets(name, 1, fragment.OffsetPattern, fragment.OffsetPatternSF, ref offsets) == 0;
        }

        /***************************************************/

        private bool SetPanelFragment(PanelOffsetByPoint fragment, string name)
        {
            double[] offsets = fragment.Offset;
            bool success =  m_model.AreaObj.SetOffsets(name, 2, "", 0, ref offsets) == 0;
            return success && offsets.SequenceEqual(fragment.Offset);
        }

        /***************************************************/
    }
}



