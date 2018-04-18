using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using BH.Adapter;
using BH.Engine.SAP2000;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        public const string ID = "SAP2000_id";

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter()
        {
            AdapterId = ID;

            Config.SeparateProperties = true;
            Config.MergeWithComparer = true;
            Config.ProcessInMemory = false;
            Config.CloneBeforePush = true;

            string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 19\SAP2000.exe";
            SAP2000v19.cOAPI m_SAPObject;
            SAP2000v19.cHelper m_helper;
            SAP2000v19.cSapModel m_SAPModel;

            if (IsApplicationRunning())
            {
                //attach to a running instance of SAP2000
                try
                {
                    //get the active SAP2000 object
                    m_SAPObject = (cOAPI)GetRunningInstance();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No running instance of the program found or failed to attach.");
                    return;
                }
            }
            else
            {
                //create a new instance of SAP2000
                try
                {
                    //Create OAPI Helper object
                    m_helper = new SAP2000v19.Helper();

                    //Get SAP2000 object
                    m_SAPObject = m_helper.CreateObject(pathToSAP);                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot Start a new instance of the program");
                    return;
                }

                //Start SAP2000 application
                m_SAPObject.ApplicationStart();
            }

            //Create SAPModel object
            m_SAPModel = m_SAPObject.SapModel;

            int ret;
            ret = m_SAPModel.InitializeNewModel((eUnits.kN_m_C));
            ret = m_SAPModel.File.NewBlank();
        }

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static bool IsApplicationRunning()
        {
            return false;
            //return System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 0;
        }

        public static object GetRunningInstance()
        {
            return (System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SapObject"));
        }


        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        //Add any comlink object as a private field here, example named:

        //private SoftwareComLink m_softwareNameCom;


        /***************************************************/


    }
}
