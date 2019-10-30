using SAP2000v19;
using System;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {        
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public const string ID = "SAP2000_id";

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter(string filePath = "", bool Active = false)
        {
            if (Active)
            {
                AdapterId = ID;

                Config.HandleDependencies = true;

                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 19\SAP2000.exe";
                cHelper helper = new SAP2000v19.Helper();

                object runningInstance = null;
                if (System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 0)
                {
                    runningInstance = System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SAPObject");

                    m_app = (cOAPI)runningInstance;
                    m_model = m_app.SapModel;
                    if (System.IO.File.Exists(filePath))
                        m_model.File.OpenFile(filePath);
                    m_model.SetPresentUnits(eUnits.N_m_C);
                }
                else 
                {
                    //open SAP if not running
                    try
                    {
                        m_app = helper.CreateObject(pathToSAP);
                        m_app.ApplicationStart();
                        m_model = m_app.SapModel;
                        m_model.InitializeNewModel(eUnits.N_m_C);
                        if (System.IO.File.Exists(filePath))
                            m_model.File.OpenFile(filePath);
                        else
                            m_model.File.NewBlank();
                    }
                    catch
                    {
                        Console.WriteLine("Cannot load SAP2000, check that SAP2000 is installed and a license is available");
                    }
                }
            }
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private cOAPI m_app;
        private cSapModel m_model;

        /***************************************************/
    }
}
