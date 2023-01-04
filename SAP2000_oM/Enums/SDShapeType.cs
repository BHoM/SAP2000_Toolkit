/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

namespace BH.oM.Adapters.SAP2000
{

    /***************************************************/
    /**** Public Enums                              ****/
    /***************************************************/

    public enum SDShapeType
    {
        ISection = 1,
        Channel = 2,
        Tee = 3,
        Angle = 4,
        DoubleAngle = 5,
        Box = 6,
        Pipe = 7,
        Plate = 8,
        SolidRectangle = 101,
        SolidCircle = 102,
        SolidSegment = 103,
        SolidSector = 104,
        Polygon = 201,
        ReinforcingSingle = 301,
        ReinforcingLine = 302,
        ReinforcingRectangle = 303,
        ReinforcingCircle = 304,
        ReferenceLine = 401,
        ReferenceCircle = 402,
        CaltransSquare = 501,
        CaltransCircle = 502,
        CaltransHexagon = 503,
        CaltransOctagon = 504,
    }

    /***************************************************/

}



