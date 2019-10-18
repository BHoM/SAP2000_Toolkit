using System;

#if Debug19 || Release19
using SAP2000v19;
#else
using SAP2000v1;
#endif

namespace BH.Adapter.SAP2000
{
#if Debug19 || Release19
    public partial class SAP2000v19Adapter : BHoMAdapter
#else
    public partial class SAP2000v21Adapter : BHoMAdapter
#endif
    {        
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public const string ID = "SAP2000_id";

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

#if Debug19 || Release19
        public SAP2000v19Adapter(string filePath = "", bool Active = false)
#else
        public SAP2000v21Adapter(string filePath = "", bool Active = false)
#endif
        {
            if (Active)
            {
                AdapterId = ID;

                Config.SeparateProperties = true;
                Config.MergeWithComparer = true;
                Config.ProcessInMemory = false;
                Config.CloneBeforePush = true;

#if Debug19 || Release19
                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 19\SAP2000.exe";

                cHelper helper = new Helper();

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
#else
                cHelper helper = new Helper();

                m_app = helper.CreateObjectProgID("CSI.SAP2000.API.SapObject");

                m_app.ApplicationStart(eUnits.N_m_C, true);

                m_model = m_app.SapModel;

#endif

                
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
