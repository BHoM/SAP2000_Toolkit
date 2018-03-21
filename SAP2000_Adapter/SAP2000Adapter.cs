using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter;
using BH.Engine.SAP2000;
using SAP2000v19;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {
        public const string ID = "SAP2000_id";
        private cOAPI app;
        private cSapModel model;
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter(string filePath = "")
        {
            AdapterId = ID;

            Config.SeparateProperties = true;
            Config.MergeWithComparer = true;
            Config.ProcessInMemory = false;
            Config.CloneBeforePush = true;

            string pathToSAP = System.IO.Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES"), "Computers and Structures", "SAP2000 19", "SAP2000.exe");
            cHelper helper = new Helper();

            object runningInstance = null;
            runningInstance = System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SapObject");
            if (runningInstance != null)
            {
                app = (cOAPI)runningInstance;
                model = app.SapModel;
                if (System.IO.File.Exists(filePath))
                    model.File.OpenFile(filePath);
                model.SetPresentUnits(eUnits.kN_m_C);
            }
            else
            {
                //open SAP if not running - NOTE: this behaviour is different from other adapters
                app = helper.CreateObject(pathToSAP);
                model = app.SapModel;
                model.InitializeNewModel(eUnits.kN_m_C);
                if (System.IO.File.Exists(filePath))
                    model.File.OpenFile(filePath);
                else
                    model.File.NewBlank();
            }

        }




        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        //Add any comlink object as a private field here, example named:

        //private SoftwareComLink m_softwareNameCom;


        /***************************************************/


    }
}
