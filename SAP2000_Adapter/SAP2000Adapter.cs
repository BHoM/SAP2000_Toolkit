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
        private cOAPI app;
        private cSapModel model;

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter(string filePath = "", bool Active = false)
        {
            if (Active)
            {
                AdapterId = ID;

                Config.SeparateProperties = true;
                Config.MergeWithComparer = true;
                Config.ProcessInMemory = false;
                Config.CloneBeforePush = true;

                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 19\SAP2000.exe";
                cHelper helper = new SAP2000v19.Helper();

                object runningInstance = null;
                if (IsApplicationRunning())
                {
                    runningInstance = System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SAPObject");

                    app = (cOAPI)runningInstance;
                    model = app.SapModel;
                    if (System.IO.File.Exists(filePath))
                        model.File.OpenFile(filePath);
                    model.SetPresentUnits(eUnits.N_m_C);
                }
                else 
                {
                    //open ETABS if not running
                    try
                    {
                        app = helper.CreateObject(pathToSAP);
                        app.ApplicationStart();
                        model = app.SapModel;
                        model.InitializeNewModel(eUnits.N_m_C);
                        if (System.IO.File.Exists(filePath))
                            model.File.OpenFile(filePath);
                        else
                            model.File.NewBlank();
                    }
                    catch
                    {
                        Console.WriteLine("Cannot load SAP2000, check that SAP2000 is installed and a license is available");
                    }
                }
            }
        }

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static bool IsApplicationRunning()
        {
            return (System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 0) ? true : false;
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        //Add any comlink object as a private field here, example named:

        //private SoftwareComLink m_softwareNameCom;


        /***************************************************/


    }
}
