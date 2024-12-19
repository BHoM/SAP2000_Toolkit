/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Base.Attributes;
using BH.oM.Base;

namespace BH.oM.Adapters.SAP2000.Results
{

    public class AISCSteelUtilisation : BH.oM.Structure.Results.BarResult, IImmutable
    {

        public virtual string DesignCodeReference { get; }

        public virtual string MajorEffectiveLength { get; }

        public virtual string MinorEffectiveLength { get; }

        public virtual string DesignType { get; }

        public virtual double TotalRatio { get; }

        public virtual double TensionCompressionRatio { get; }

        public virtual double MajorShearRatio { get; }

        public virtual double MinorShearRatio { get; }

        public virtual double TorsionRatio { get; }

        public virtual double MajorBendingRatio { get; }

        public virtual double MinorBendingRatio { get; }

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public AISCSteelUtilisation(IComparable objectId, IComparable resultCase, int modeNumber, double timeStep, double position, int divisions,
                                string designCodeReference,
                                string majorEffectiveLength,
                                string minorEffectiveLength,
                                string designType,
                                double totalRatio,
                                double tensionCompressionRatio,
                                double majorShearRatio,
                                double minorShearRatio,
                                double torsionRatio,
                                double majorBendingRatio,
                                double minorBendingRatio)
            : base(objectId, resultCase, modeNumber, timeStep, position, divisions)
        {
            DesignCodeReference = designCodeReference;
            MajorEffectiveLength = majorEffectiveLength;
            MinorEffectiveLength = minorEffectiveLength;
            DesignType = designType;
            TotalRatio = totalRatio;
            TensionCompressionRatio = tensionCompressionRatio;
            MajorShearRatio = majorShearRatio;
            MinorShearRatio = minorShearRatio;
            TorsionRatio = torsionRatio;
            MajorBendingRatio = majorBendingRatio;
            MinorBendingRatio = minorBendingRatio;

        }

        /***************************************************/

    }
}





