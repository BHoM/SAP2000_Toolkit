using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Results;
using BH.oM.Common;
using BH.oM.Structure.Elements;
using BH.Engine.SAP2000;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Requests;
using BH.oM.Adapters.SAP2000.
using BH.oM.Geometry;
using BH.Engine.Geometry;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public method - Read override             ****/
        /***************************************************/

        public IEnumerable<IResult> ReadResults(NodeResultRequest request)
        {
            CheckAndSetUpCases(request);
            List<string> nodeIds = CheckGetNodeIds(request);

            switch (request.ResultType)
            {
                case NodeResultType.NodeReaction:
                    return ReadNodeReaction(nodeIds);
                default:
                    Engine.Reflection.Compute.RecordError("Result extraction of type " + request.ResultType + " is not yet supported");
                    return new List<IResult>();
            }
        }

        /***************************************************/
        /**** Private method - Extraction methods       ****/
        /***************************************************/

        private List<NodeReaction> ReadNodeReaction(List<string> nodeIds)
        {
            List<NodeReaction> nodeReactions = new List<NodeReaction>();

            int resultCount = 0;
            string[] loadcaseNames = null;
            string[] objects = null;
            string[] elm = null;
            string[] stepType = null;
            double[] stepNum = null;

            double[] fx = null;
            double[] fy = null;
            double[] fz = null;
            double[] mx = null;
            double[] my = null;
            double[] mz = null;

            for (int i = 0; i < nodeIds.Count; i++)
            {
                int ret = m_model.Results.JointReact(nodeIds[i], eItemTypeElm.ObjectElm, ref numberResults,
                    ref objects, ref elm, ref loadcaseNames, ref stepType, ref stepNum, ref fx, ref fy, ref fz, ref mx, ref my, ref mz);
                if (ret == 0)
                {
                    for (int j = 0; j < resultCount; j++)
                    {

                        NodeReaction nr = new NodeReaction()
                        {
                            ResultCase = loadcaseNames[j],
                            ObjectId = nodeIds[i],
                            MX = mx[j],
                            MY = my[j],
                            MZ = mz[j],
                            FX = fx[j],
                            FY = fy[j],
                            FZ = fz[j],
                            TimeStep = stepNum[j]
                        };
                        nodeReactions.Add(nr);
                    }
                }
            }

            return nodeReactions;
        }

        /***************************************************/
        /**** Private method - Support methods          ****/
        /***************************************************/

        private List<string> CheckGetNodeIds(NodeResultRequest request)
        {
            List<string> nodeIds = newList<string>();
            var ids = request.ObjectIds;

            if (ids == null || ids.Count == 0)
            {
                int nodes = 0;
                string[] names = null;
                m_model.PointObj.GetNameList(ref nodes, ref names);
                nodeIds = names.ToList();
            }
        }

    }
}
