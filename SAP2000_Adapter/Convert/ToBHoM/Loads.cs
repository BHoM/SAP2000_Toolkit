/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

        public static LoadNature ToBHoM(this eLoadPatternType patType)
        {
            LoadNature nature = new LoadNature();

            switch (patType)
            {
                case eLoadPatternType.Dead:
                    nature = LoadNature.Dead;
                    break;
                case eLoadPatternType.SuperDead:
                    nature = LoadNature.SuperDead;
                    break;
                case eLoadPatternType.Live:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.Quake:
                    nature = LoadNature.Seismic;
                    break;
                case eLoadPatternType.Wind:
                    nature = LoadNature.Wind;
                    break;
                case eLoadPatternType.Snow:
                    nature = LoadNature.Snow;
                    break;
                case eLoadPatternType.Other:
                    nature = LoadNature.Other;
                    break;
                case eLoadPatternType.Temperature:
                    nature = LoadNature.Temperature;
                    break;
                case eLoadPatternType.Rooflive:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.Notional:
                    nature = LoadNature.Notional;
                    break;
                case eLoadPatternType.PatternLive:
                    nature = LoadNature.Live;
                    break;
                case eLoadPatternType.TemperatureGradient:
                    nature = LoadNature.Temperature;
                    break;
                case eLoadPatternType.Prestress:
                    nature = LoadNature.Prestress;
                    break;
                default:
                    nature = LoadNature.Other;
                    break;
            }

            return nature;
        }

        /***************************************************/

        public static LoadAxis LoadAxisToBHoM(this string cSys)
        {
            LoadAxis axis = new LoadAxis();

            switch (cSys)
            {
                case "Global":
                    axis = LoadAxis.Global;
                    break;
                case "Local":
                    axis = LoadAxis.Local;
                    break;
                default:
                    axis = LoadAxis.Global;
                    break;
            }

            return axis;
        }

        /***************************************************/
    }
}
