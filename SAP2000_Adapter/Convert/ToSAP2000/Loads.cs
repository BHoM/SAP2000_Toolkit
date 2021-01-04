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

using BH.oM.Structure.Loads;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static eLoadPatternType ToCSI(this LoadNature loadNature)
        {
            eLoadPatternType patType = new eLoadPatternType();

            switch (loadNature)
            {
                case LoadNature.Dead:
                    patType = eLoadPatternType.Dead;
                    break;
                case LoadNature.SuperDead:
                    patType = eLoadPatternType.SuperDead;
                    break;
                case LoadNature.Live:
                    patType = eLoadPatternType.Live;
                    break;
                case LoadNature.Seismic:
                    patType = eLoadPatternType.Quake;
                    break;
                case LoadNature.Wind:
                    patType = eLoadPatternType.Wind;
                    break;
                case LoadNature.Snow:
                    patType = eLoadPatternType.Snow;
                    break;
                case LoadNature.Other:
                    patType = eLoadPatternType.Other;
                    break;
                case LoadNature.Temperature:
                    patType = eLoadPatternType.Temperature;
                    break;
                case LoadNature.Notional:
                    patType = eLoadPatternType.Notional;
                    break;
                case LoadNature.Prestress:
                    patType = eLoadPatternType.Prestress;
                    break;
                default:
                    patType = eLoadPatternType.Other;
                    break;
            }

            return patType;
        }

        /***************************************************/

        public static string ToCSI(this LoadAxis axis)
        {
            string cSys = "";

            switch (axis)
            {
                case LoadAxis.Global:
                    cSys = "Global";
                    break;
                case LoadAxis.Local:
                    cSys = "Local";
                    break;
                default:
                    cSys = "Global";
                    break;
            }

            return cSys;
        }

        /***************************************************/
    }
}

