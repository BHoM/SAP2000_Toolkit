﻿using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Adapter;
using BH.Engine.Adapter;
using BH.oM.Adapters.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {        
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/
        
            public SAP2000ActionConfig SAPConfig { get; set; } = new SAP2000ActionConfig();

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter(string filePath = "", bool Active = false)
        {

            //Initialization
            AdapterIdName = Engine.SAP2000.Convert.AdapterIdName;
            Modules.Structure.ModuleLoader.LoadModules(this);
            SetupComparers();
            SetupDependencies();   

            if (Active)
            {
                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 21\SAP2000.exe";
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
            }
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private cOAPI m_app;
        private cSapModel m_model;
        private ActionConfig actionConfig;

        /***************************************************/
    }
}
