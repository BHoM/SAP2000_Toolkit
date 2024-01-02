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

using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000
{

    /***************************************************/
    /**** Public Enums                              ****/
    /***************************************************/

    [Description("Defines the Design Code used for result extraction.")]
    public enum SteelDesignCode
    {
        Undefined,
        AASHTO_LRFD_2007,
        AISC_ASD89,
        AISC_360_05,
        AISC_360_10,
        AISC_360_16,
        IBC2006,
        AISC_LRFD93,
        API_RP2A_LRFD_97,
        API_RP2A_WSD2000,
        API_RP2A_WSD2014,
        AS_4100_1998,
        ASCE_10_97,
        BS5950_2000,
        Chinese_2010,
        CSA_S16_14,
        CSA_S16_09,
        EUROCODE_3_2005,
        Indian_IS_800_2007,
        Italian_NTC_2008,
        Italian_UNI_10011,
        KBC_2009,
        Norsok_N_004_2013,
        NZS_3404_1997,
        SP_16_13330_2011,
    }

    /***************************************************/

}




