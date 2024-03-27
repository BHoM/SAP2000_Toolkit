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

using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
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

        public static double GetUniformComponent(this BarDifferentialTemperatureLoad load)
        {
            return (load.TemperatureProfile[0] + load.TemperatureProfile[1])/2;
        }

        /***************************************************/
    
        public static double GetDifferentialComponent(this BarDifferentialTemperatureLoad load, Bar bar, out int myType)
        {
            Dictionary<double, double> profile = load.TemperatureProfile;
            double length;
            myType = 1;

            if (bar.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("Cannot assign a BarDifferentialTemperature load to a bar with no SectionProperty.");
                return double.NaN;
            }

            switch (load.LoadDirection)
            {
                case DifferentialTemperatureLoadDirection.LocalY:
                    length = bar.SectionProperty.Vy + bar.SectionProperty.Vpy;
                    myType = 2;
                    break;
                case DifferentialTemperatureLoadDirection.LocalZ:
                    length = bar.SectionProperty.Vz + bar.SectionProperty.Vpz;
                    myType = 3;
                    break;
                default:
                    Engine.Base.Compute.RecordError("Could not understand BarDifferentialTemperatureLoad Direction.");
                    return double.NaN;

            }

            double temp = (profile[1] - profile[0])/length;

            foreach (double key in profile.Keys)
            {
                if (profile[key] - GetUniformComponent(load) != key * temp) Engine.Base.Compute.RecordWarning("Only linear temperature gradients are allowed.");
                break;
            }

            return temp;
        }

        /***************************************************/

    }
}




