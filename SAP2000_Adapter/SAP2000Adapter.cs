/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Base.Attributes;
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

        [Description("Creates an adapter supporting SAP2000v21 and later.")]
        public SAP2000Adapter(string filePath = "", bool active = false)
        {

            //Initialization
            Modules.Structure.ModuleLoader.LoadModules(this);
            SetupComparers();
            SetupDependencies();
            AdapterIdFragmentType = typeof(SAP2000Id);
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.FullPush;
            m_AdapterSettings.CacheCRUDobjects = false;

            if (active)
            {
                string progId = "CSI.SAP2000.API.SapObject";
                cHelper helper = new Helper();

                //Check for multiple SAP instances
                if (System.Diagnostics.Process.GetProcessesByName("SAP2000").Length > 1)
                {
                    BH.Engine.Base.Compute.RecordError("More than one SAP2000 instance is open. BHoM has attached to the most recently updated process, " +
                        "but you should only work with one SAP2000 instance at a time with BHoM.");
                }

                // Set up the open command
                Open openCommand = new Open();
                if (File.Exists(filePath))
                    openCommand.FileName = filePath;

                // Try to attach to an existing instance of SAP2000
                try
                {
                    m_app = (cOAPI)System.Runtime.InteropServices.Marshal.GetActiveObject(progId);
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
                                RunCommand(openCommand);
                                BH.Engine.Base.Compute.RecordWarning("A SAP2000 Model was already open, and it has been saved if it was a named file. " +
                                    "The model at the filePath has been opened instead.");
                            }
                        }
                        else
                        {
                            BH.Engine.Base.Compute.RecordError("Invalid extension. SAP2000 model filename must end with .sdb. " +
                                "BHoM is attached to the current SAP2000 instance, but a valid SAP2000 model path has not been provided.");
                        }
                    }
                    else
                    {
                        if (m_model.GetModelFilename(false) == null)
                            RunCommand(new NewModel());
                        else m_model.SetPresentUnits(eUnits.N_m_C); //set units to SI (NewModel and OpenModel already do this)
                        BH.Engine.Base.Compute.RecordNote("File path is either not provided or invalid. " +
                                                        "BHoM is attached to the current SAP2000 instance.");
                    }
                }
                catch    // No open instance of SAP2000
                {
                    try
                    {
                        m_app = helper.CreateObjectProgID(progId);
                        m_app.ApplicationStart();
                        m_model = m_app.SapModel;
                        // File exists and can be loaded
                        if (openCommand.FileName != null)
                            RunCommand(openCommand);
                        // File does not exist, open new model
                        else
                        {
                            RunCommand(new NewModel());
                            BH.Engine.Base.Compute.RecordNote("File path is either not provided or invalid. " +
                                                                "BHoM is attached to the current SAP2000 instance.");
                        }
                    }
                    catch
                    // Unable to launch SAP2000
                    {
                        Engine.Base.Compute.RecordError("Cannot load SAP2000, check that SAP2000 is installed and a license is available");
                    }
                }
            }
            else
            {
                int sapCount = System.Diagnostics.Process.GetProcessesByName("SAP2000").Length;
                if (sapCount > 1)
                {
                    Engine.Base.Compute.RecordError("More than one SAP2000 instance is open. BHoM will attach to the most recently updated process, " +
                        "but may result in unpredictable behavior. You should only work with one SAP2000 instance at a time while using BHoM.");
                }
                else if (sapCount == 1 && !string.IsNullOrEmpty(filePath))
                    Engine.Base.Compute.RecordWarning("SAP2000 is open and a filePath has been provided. BHoM will save your work and open the specified file.");
                else if (sapCount == 1)
                    Engine.Base.Compute.RecordNote("SAP2000 is open. No filePath has been provided, so BHoM will attach to the current model, if one exists.");
                else
                    Engine.Base.Compute.RecordNote("SAP2000 Adapter is not currently active.");
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



