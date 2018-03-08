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

        protected override object NextId(Type objectType, bool refresh = false)
        {
            //Method that returns the next free index for a specific object type. 
            //Software dependent which type of index to return. Could be int, string, Guid or whatever the specific software is using
            //At the point of index assignment, the objects have not yet been created in the target software. 
            //The if statement below is designed to grab the first free index for the first object being created and after that increment.

            //Change from object to what the specific software is using
            object index;

            if (!refresh && m_indexDict.TryGetValue(objectType, out index))
            {
                //If possible to find the next index based on the previous one (for example index++ for an int based index system) do it here

                //Example int based:
                //index++
            }
            else
            {
                index = 0;//Insert code to get the next index of the specific type
            }

            m_indexDict[objectType] = index;
            return index;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Change from object to the index type used by the specific software
        private Dictionary<Type, object> m_indexDict = new Dictionary<Type, object>();


        /***************************************************/
    }
}
