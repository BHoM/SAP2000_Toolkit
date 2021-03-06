/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using SAP2000v1;
using System;
using BH.oM.Adapters.SAP2000;
using BH.oM.Adapter.Commands;
using BH.Engine.Adapter;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter : BHoMAdapter
    {        
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/
        
            public SAP2000PushConfig SAPConfig { get; set; } = new SAP2000PushConfig();

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SAP2000Adapter(string filePath = "", bool active = false)
        {

            //Initialization
            Modules.Structure.ModuleLoader.LoadModules(this);
            SetupComparers();
            SetupDependencies();
            AdapterIdFragmentType = typeof(SAP2000Id);
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateNonExisting;

            if (active)
            {
                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 21\SAP2000.exe";
                cHelper helper = new Helper();

                Open openCommand = new Open();
                if (System.IO.File.Exists(filePath))
                    openCommand.FileName = filePath;

                if (System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 0)
                {
                    object runningInstance = System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SAPObject");

                    m_app = (cOAPI)runningInstance;
                    m_model = m_app.SapModel;
                    if (openCommand.FileName != null)
                        RunCommand(openCommand);
                    else
                        RunCommand(new NewModel());
                }
                else 
                {
                    //open SAP if not running
                    try
                    {
                        m_app = helper.CreateObject(pathToSAP);
                        m_app.ApplicationStart();
                        m_model = m_app.SapModel;
                        if (openCommand.FileName != null)
                            RunCommand(openCommand);
                        else
                            RunCommand(new NewModel() );
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

