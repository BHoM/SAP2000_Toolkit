using BH.Engine.SAP2000;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using SAP2000v19;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (loadcase.Nature == LoadNature.Dead)
                selfWeight = 1;
            if (m_model.LoadPatterns.Add(loadcase.Name, loadcase.Nature.ToCSI(), selfWeight, true) != 0)
                CreateElementError("LoadCase", loadcase.Name);
            return true;
        }

        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            string name = loadcombination.Name;
            eCNameType nameType = eCNameType.LoadCase;
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
                if (m_model.PointObj.SetLoadForce(name, loadPat, ref val, replace, cSys, eItemType.Objects) != 0)
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
            eItemType type = eItemType.Objects;
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
            int[] dirs = { 4, 5, 6 };
            bool replace = true;
            string cSys = bhLoad.Axis.ToCSI();
            eItemType type = eItemType.Objects;

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
    }
}
