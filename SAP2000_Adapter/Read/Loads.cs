using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using SAP2000v19;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Loadcase> ReadLoadcase(List<string> ids = null)
        {
            List<Loadcase> loadCases = new List<Loadcase>();

            int count = 0;
            string[] names = null;

            m_model.LoadPatterns.GetNameList(ref count, ref names);

            for (int i = 0; i < count; i++ )
            {
                eLoadPatternType patternType = eLoadPatternType.Dead;

                if (m_model.LoadPatterns.GetLoadType(names[i], ref patternType) != 0)
                {
                    CreateElementError("Load Pattern", names[i]);
                }
                else
                {
                    loadCases.Add(BH.Engine.Structure.Create.Loadcase(names[i], i, patternType.ToBHoM()));
                }
            }

            return loadCases;
        }

        /***************************************************/

        private List<LoadCombination> ReadLoadCombination(List<string> ids = null)
        {
            throw new NotImplementedException();
            //List<LoadCombination> combinations = new List<LoadCombination>();

            //int number;
            //int count = 0;
            //string[] cName = null;
            //double[] factors = null;
            //int caseNum = 0;
            //eCNameType[] nameTypes = null;

            //foreach ( string name in ids)
            //{
            //    if (m_model.RespCombo.GetCaseList(name, ref count, ref nameTypes, ref cName, ref factors) == 0)
            //    {
            //        LoadCombination combination = new LoadCombination();
            //        combination.LoadCases = cName.ToString();
            //    }
            //}


            //return combinations;
        }

        /***************************************************/

        private List<ILoad> ReadLoad(Type type, List<string> ids = null)
        {
            // not implemented!
            throw new NotImplementedException();

            //List<ILoad> loadList = new List<ILoad>();

            ////get loadcases first
            //List<Loadcase> loadcaseList = ReadLoadcase();

            //loadList = Helper.GetLoads(model, loadcaseList);

            ////filter the list to return only the right type - No, this is not a clever way of doing it !
            //loadList = loadList.Where(x => x.GetType() == type).ToList();

            //return loadList;
        }

        /***************************************************/
    }
}
