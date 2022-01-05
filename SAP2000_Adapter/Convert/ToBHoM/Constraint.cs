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

using BH.oM.Structure.Constraints;

namespace BH.Adapter.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint6DOF GetConstraint6DOF(bool[] restraint, double[] spring)
        {
            Constraint6DOF bhConstraint = new Constraint6DOF();
            bhConstraint.TranslationX = restraint[0] == true ? DOFType.Fixed : DOFType.Free;
            bhConstraint.TranslationY = restraint[1] == true ? DOFType.Fixed : DOFType.Free;
            bhConstraint.TranslationZ = restraint[2] == true ? DOFType.Fixed : DOFType.Free;
               bhConstraint.RotationX = restraint[3] == true ? DOFType.Fixed : DOFType.Free;
               bhConstraint.RotationY = restraint[4] == true ? DOFType.Fixed : DOFType.Free;
               bhConstraint.RotationZ = restraint[5] == true ? DOFType.Fixed : DOFType.Free;

            bhConstraint.TranslationalStiffnessX = spring[0];
            bhConstraint.TranslationalStiffnessY = spring[1];
            bhConstraint.TranslationalStiffnessZ = spring[2];
            bhConstraint.RotationalStiffnessX = spring[3];
            bhConstraint.RotationalStiffnessY = spring[4];
            bhConstraint.RotationalStiffnessZ = spring[5];

            return bhConstraint;
        }

        /***************************************************/

        public static BarRelease GetBarRelease(bool[] startRelease, double[] startSpring, bool[] endRelease, double[] endSpring)
        {
            Constraint6DOF bhStartRelease = new Constraint6DOF();

            bhStartRelease.TranslationX = GetDofType(startRelease, startSpring, 0);
            bhStartRelease.TranslationY = GetDofType(startRelease, startSpring, 2);
            bhStartRelease.TranslationZ = GetDofType(startRelease, startSpring, 1);
            bhStartRelease.RotationX = GetDofType(startRelease, startSpring, 3);
            bhStartRelease.RotationY = GetDofType(startRelease, startSpring, 5);
            bhStartRelease.RotationZ = GetDofType(startRelease, startSpring, 4);

            bhStartRelease.TranslationalStiffnessX = startSpring[0];
            bhStartRelease.TranslationalStiffnessY = startSpring[2];
            bhStartRelease.TranslationalStiffnessZ = startSpring[1];
            bhStartRelease.RotationalStiffnessX = startSpring[3];
            bhStartRelease.RotationalStiffnessY = startSpring[5];
            bhStartRelease.RotationalStiffnessZ = startSpring[4];

            Constraint6DOF bhEndRelease = new Constraint6DOF();

            bhEndRelease.TranslationX = GetDofType(endRelease, endSpring, 0);
            bhEndRelease.TranslationY = GetDofType(endRelease, endSpring, 2);
            bhEndRelease.TranslationZ = GetDofType(endRelease, endSpring, 1);
            bhEndRelease.RotationX = GetDofType(endRelease, endSpring, 3);
            bhEndRelease.RotationY = GetDofType(endRelease, endSpring, 5);
            bhEndRelease.RotationZ = GetDofType(endRelease, endSpring, 4);

            bhEndRelease.TranslationalStiffnessX = endSpring[0];
            bhEndRelease.TranslationalStiffnessY = endSpring[2];
            bhEndRelease.TranslationalStiffnessZ = endSpring[1];
            bhEndRelease.RotationalStiffnessX = endSpring[3];
            bhEndRelease.RotationalStiffnessY = endSpring[5];
            bhEndRelease.RotationalStiffnessZ = endSpring[4];

            BarRelease barRelease = new BarRelease() { StartRelease = bhStartRelease, EndRelease = bhEndRelease };

            return barRelease;
        }

        /***************************************************/

        private static DOFType GetDofType(bool[] isReleased, double[] springValue, int i)
        {
            if (isReleased[i])
            {
                if (springValue[i] != 0)
                    return DOFType.Spring;
                else
                    return DOFType.Free;
            }
            else
                return DOFType.Fixed;
        }

        /***************************************************/
    }
}

