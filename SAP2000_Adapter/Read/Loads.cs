using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<LoadCombination> ReadLoadCombination(List<string> ids = null)
        {
            // not implemented!
            throw new NotImplementedException();

            //    List<LoadCombination> combinations = new List<LoadCombination>();

            //    //get all load cases before combinations
            //    int number = 0;
            //    string[] names = null;
            //    model.LoadPatterns.GetNameList(ref number, ref names);
            //    Dictionary<string, ICase> caseDict = new Dictionary<string, ICase>();

            //    //ensure id can be split into name and number
            //    names = Helper.EnsureNameWithNum(names.ToList()).ToArray();

            //    foreach (string name in names)
            //        caseDict.Add(name, Helper.GetLoadcase(model, name));

            //    int nameCount = 0;
            //    string[] nameArr = { };

            //    if (ids == null)
            //    {
            //        model.RespCombo.GetNameList(ref nameCount, ref nameArr);
            //        ids = nameArr.ToList();
            //    }

            //    //ensure id can be split into name and number
            //    ids = Helper.EnsureNameWithNum(ids);

            //    foreach (string id in ids)
            //    {
            //        combinations.Add(Helper.GetLoadCombination(model, caseDict, id));
            //    }

            //    return combinations;
        }

        /***************************************************/

        private List<Loadcase> ReadLoadcase(List<string> ids = null)
        {
            // not implemented!
            throw new NotImplementedException();

            //int nameCount = 0;
            //string[] nameArr = { };

            //List<Loadcase> loadcaseList = new List<Loadcase>();

            //if (ids == null)
            //{
            //    model.LoadPatterns.GetNameList(ref nameCount, ref nameArr);
            //    ids = nameArr.ToList();
            //}

            ////ensure id can be split into name and number
            //ids = Helper.EnsureNameWithNum(ids);

            //foreach (string id in ids)
            //{
            //    loadcaseList.Add(Helper.GetLoadcase(model, id));
            //}

            //return loadcaseList;
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
