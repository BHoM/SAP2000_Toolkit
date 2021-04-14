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
using System.IO;
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
        
            public SAP2000PushConfig SAPPushConfig { get; set; } = new SAP2000PushConfig();

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
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.FullPush;

            if (active)
            {
                string pathToSAP = @"C:\Program Files\Computers and Structures\SAP2000 21\SAP2000.exe";
                cHelper helper = new Helper();

                Open openCommand = new Open();

                if (System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 1)
                {
                    BH.Engine.Reflection.Compute.RecordError("More than one SAP2000 instance is open. BHoM will attach to the most recently updated process, " +
                        "but you should only work with one SAP2000 instance at a time with BHoM.");
                }

                if (File.Exists(filePath))
                    openCommand.FileName = filePath;

                // Try to attach to an existing instance of SAP2000
                try
                {
                    object runningInstance = (cOAPI) System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SapObject");
                    m_app = (cOAPI)runningInstance;
                    // Attach to current model
                    m_model = m_app.SapModel;
                    // Continue if filepath provided
                    if (openCommand.FileName != null)
                    {
                        FileInfo file = new FileInfo(filePath);
                        string extension = file.Extension;
                        if (String.Equals(extension, ".sdb"))
                        {
                            String openModelFileName = m_model.GetModelFilename();
                            // Filepath matches open file
                            if (String.Equals(openModelFileName, filePath) == false)
                            {
                                // Attempt to save current file, close, and open SAP2000 file
                                m_model.File.Save();
                                m_model.File.OpenFile(filePath);
                                BH.Engine.Reflection.Compute.RecordWarning("A SAP2000 Model was already open, and it has been saved if it was a named file. " +
                                    "The model at the filePath has been opened instead.");
                            }
                        }
                        else
                        {
                            BH.Engine.Reflection.Compute.RecordError("Invalid extension. SAP2000 model filename must end with .sdb. " +
                                "BHoM is attached to the current SAP2000 instance, but a valid SAP2000 model path has not been provided.");
                        }
                    }
                    else
                    {
                        RunCommand(new NewModel());
                        BH.Engine.Reflection.Compute.RecordWarning("File path is either not provided or invalid. " +
                                                        "BHoM is attached to the current SAP2000 instance.");
                    }
                }
                catch
                // No open instance of SAP2000
                {
                    try
                    {
                        m_app = helper.CreateObject(pathToSAP);
                        m_app.ApplicationStart();
                        m_model = m_app.SapModel;
                        // File exists and can be loaded
                        if (openCommand.FileName != null)
                            RunCommand(openCommand);
                        // File does not exist, open new model
                        else
                        {
                            RunCommand(new NewModel());
                            BH.Engine.Reflection.Compute.RecordWarning("File path is either not provided or invalid. " +
                                                                "BHoM is attached to the current SAP2000 instance.");
                        }
                    }
                    catch
                    // Unable to launch SAP2000
                    {
                        BH.Engine.Reflection.Compute.RecordError("Cannot load SAP2000, check that SAP2000 is installed and a license is available");
                    }
                }
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("SAP2000 Adapter is not currently active.");
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

