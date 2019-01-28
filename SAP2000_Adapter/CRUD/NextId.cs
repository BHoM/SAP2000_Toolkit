using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        
        private Dictionary<Type, string> m_indexDict = new Dictionary<Type, string>();

        protected override object NextId(Type objectType, bool refresh = false)
        {
            string index;

            if (!refresh && m_indexDict.TryGetValue(objectType, out index))
                {
                index = "0";
                try
                {
                    index = (int.Parse(index) + 1).ToString();
                }
                catch
                {
                    index = GetNextIdOfType(objectType);
                }
                
                m_indexDict[objectType] = index;
            }
            else
            {
                index = GetNextIdOfType(objectType);
                m_indexDict[objectType] = index;
            }

            return index;
        }

        private string GetNextIdOfType(Type objectType)
        {
            string nextId;
            string typeString = objectType.Name;
            int nameCount = 0;
            string[] names = { };

            switch (typeString)
            {
                case "Node":
                    model.PointObj.GetNameList(ref nameCount, ref names);
                    nextId = nameCount == 0 ? "1" : (Array.ConvertAll(names, int.Parse).Max() + 1).ToString();
                    break;
                case "Bar":
                    model.FrameObj.GetNameList(ref nameCount, ref names);
                    nextId = nameCount == 0 ? "1" : (Array.ConvertAll(names, int.Parse).Max() + 1).ToString();
                    break;
                case "PanelPlanar":
                    model.AreaObj.GetNameList(ref nameCount, ref names);
                    nextId = nameCount == 0 ? "1" : (Array.ConvertAll(names, int.Parse).Max() + 1).ToString();
                    break;
                case "Material":
                    model.PropMaterial.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                case "SectionProperty":
                    model.PropFrame.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                case "Property2D":
                    model.PropArea.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                case "Loadcase":
                    model.LoadPatterns.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                case "LoadCombination":
                    model.AreaObj.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                case "RigidLink":
                    model.LinkObj.GetNameList(ref nameCount, ref names);
                    nextId = (nameCount + 1).ToString();
                    break;
                default:
                    nextId = "0";
                    ErrorLog.Add("Could not get count of type: " + typeString);
                    break;
            }

            return nextId;

        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Change from object to the index type used by the specific software


        /***************************************************/
    }
}
