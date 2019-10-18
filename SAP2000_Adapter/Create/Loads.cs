using BH.Engine.SAP2000;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using System.Linq;

#if Debug19 || Release19
using SAP = SAP2000v19;
#else
using SAP = SAP2000v1;
#endif

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private bool CreateObject(Loadcase loadcase)
        {
            double selfWeight = 0;
            m_model.LoadPatterns.Add(loadcase.Name, loadcase.Nature.ToCSI(), selfWeight, false);

            //Create Load Case based on Pattern:
            string[] loadTypes   = { "Load" };
            string[] loadNames   = { loadcase.Name };
            double[] loadFactors = { 1 };

            m_model.LoadCases.StaticLinear.SetCase(loadcase.Name);
            m_model.LoadCases.StaticLinear.SetLoads(loadcase.Name, 1, ref loadTypes, ref loadNames, ref loadFactors);

            loadcase.CustomData[AdapterId] = loadcase.Name;
            loadcase.Number = m_model.LoadPatterns.Count();
            
            return true;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            string name = loadcombination.Name;
            SAP.eCNameType nameType = SAP.eCNameType.LoadCase;
            if (m_model.RespCombo.Add(name, 0) == 0)
            {
                List<Tuple<double, ICase>> bhomCases = loadcombination.LoadCases;
                foreach (Tuple<double, ICase> comboCase in loadcombination.LoadCases)
                {
                    double factor = comboCase.Item1;
                    ICase bhomCase = comboCase.Item2;
                    if (m_model.RespCombo.SetCaseList(name, ref nameType, bhomCase.Name, factor) != 0)
                        Engine.Reflection.Compute.RecordWarning("Could not add case " + bhomCase.Name + " to combo " + name);
                }
            }
            else
            {
                CreateElementError("Load Combination", name);
            }

            return true;
        }

        /***************************************************/

        private bool CreateObject(ILoad bhLoad)
        {
            return CreateLoad(bhLoad as dynamic);
        }

        /***************************************************/

        private bool CreateLoad(PointLoad bhLoad)
        {
            List<Node> nodes = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterId].ToString();
            string cSys = bhLoad.Axis.ToCSI();
            double[] val = 
            {
                bhLoad.Force.X,
                bhLoad.Force.Y,
                bhLoad.Force.Z,
                bhLoad.Moment.X,
                bhLoad.Moment.Y,
                bhLoad.Moment.Z,
            };

            bool replace = true;

            foreach (Node bhNode in nodes)
            {
                string name = bhNode.CustomData[AdapterId].ToString();
                if (m_model.PointObj.SetLoadForce(name, loadPat, ref val, replace, cSys, SAP.eItemType.Objects) != 0)
                    CreateElementError("Point Load", name);
            }

            return true;
        }

        /***************************************************/

        private bool CreateLoad(BarUniformlyDistributedLoad bhLoad)
        {
            List<Bar> bars = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterId].ToString();
            double dist1 = 0;
            double dist2 = 1;
            int[] dirs = null;
            double[] forceVals = null;
            double[] momentVals = null;
            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    dirs = new int[] { 4, 5, 6 };
                    forceVals = bhLoad.Force.ToDoubleArray();
                    momentVals = bhLoad.Moment.ToDoubleArray();
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    forceVals = bhLoad.Force.BarLocalAxisToCSI().ToDoubleArray();
                    momentVals = bhLoad.Moment.BarLocalAxisToCSI().ToDoubleArray();
                    break;
            }
            bool relDist = true;
            string cSys = bhLoad.Axis.ToCSI();
            SAP.eItemType type = SAP.eItemType.Objects;
            bool replace = true;
            

            foreach (Bar bhBar in bars)
            {
                string name = bhBar.CustomData[AdapterId].ToString();
                bool replaceNow = replace;
                for (int i = 0; i < dirs.Count(); i++)
                {
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 1, dirs[i], dist1, dist2, forceVals[i], forceVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                    replaceNow = false;
                    if (m_model.FrameObj.SetLoadDistributed(name, loadPat, 2, dirs[i], dist1, dist2, momentVals[i], momentVals[i], cSys, relDist, replaceNow, type) != 0)
                        CreateElementError("BarLoad", bhBar.Name + dirs[i]);
                }
            }

            return true;
        }

        /***************************************************/

        private bool CreateLoad(AreaUniformlyDistributedLoad bhLoad)
        {
            List<IAreaElement> panels = bhLoad.Objects.Elements.ToList();
            string loadPat = bhLoad.Loadcase.CustomData[AdapterId].ToString();
            double[] vals = bhLoad.Pressure.ToDoubleArray();
            int[] dirs = null;
            switch (bhLoad.Axis)
            {
                case LoadAxis.Global:
                    if (bhLoad.Projected)
                    {
                        dirs = new int[] { 7, 8, 9 };
                    }
                    else
                    {
                        dirs = new int[] { 4, 5, 6 };
                    }
                    break;
                case LoadAxis.Local:
                    dirs = new int[] { 1, 2, 3 };
                    break;
            }
            bool replace = true;
            string cSys = bhLoad.Axis.ToCSI();
            SAP.eItemType type = SAP.eItemType.Objects;

            foreach (Panel panel in panels)
            {
                string name = panel.CustomData[AdapterId].ToString();
                bool replaceNow = replace;
                for (int i = 0; i < dirs.Count(); i++)
                {
                    if (m_model.AreaObj.SetLoadUniform(name, loadPat, vals[i], dirs[i], replaceNow, cSys, type) != 0)
                        CreateElementError("AreaLoad Dir", panel.Name + dirs[i]);
                    replaceNow = false;
                }
            }

            return true;
        }

        /***************************************************/

        public void SetLoad(GravityLoad gravityLoad, bool replace)
        {
            double selfWeightExisting = 0;
            double selfWeightNew = -gravityLoad.GravityDirection.Z;

            string caseName = gravityLoad.Loadcase.CustomData[AdapterId].ToString();

            m_model.LoadPatterns.GetSelfWTMultiplier(caseName, ref selfWeightExisting);

            if (selfWeightExisting != 0)
                BH.Engine.Reflection.Compute.RecordWarning($"The self weight for loadcase {gravityLoad.Loadcase.Name} will be overwritten. Previous value: {selfWeightExisting}, new value: {selfWeightNew}");

            m_model.LoadPatterns.SetSelfWTMultiplier(caseName, selfWeightNew);

            if (gravityLoad.GravityDirection.X != 0 || gravityLoad.GravityDirection.Y != 0)
                Engine.Reflection.Compute.RecordError("SAP2000 can only handle gravity loads in global z direction");

            BH.Engine.Reflection.Compute.RecordNote("SAP2000 handles gravity loads via loadcases, so only one gravity load per loadcase can be used. This gravity load will be applied to all objects");
        }

        /***************************************************/
        
    }
}
