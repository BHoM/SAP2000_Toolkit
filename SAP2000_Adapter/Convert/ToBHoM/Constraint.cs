/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

            bhStartRelease.TranslationX = startRelease[0] == true ? startSpring[0] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhStartRelease.TranslationY = startRelease[1] == true ? startSpring[1] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhStartRelease.TranslationZ = startRelease[2] == true ? startSpring[2] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhStartRelease.RotationX = startRelease[3] == true ? startSpring[3] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhStartRelease.RotationY = startRelease[4] == true ? startSpring[4] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhStartRelease.RotationZ = startRelease[5] == true ? startSpring[5] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;

            bhStartRelease.TranslationalStiffnessX = startSpring[0];
            bhStartRelease.TranslationalStiffnessY = startSpring[1];
            bhStartRelease.TranslationalStiffnessZ = startSpring[2];
            bhStartRelease.RotationalStiffnessX = startSpring[3];
            bhStartRelease.RotationalStiffnessY = startSpring[4];
            bhStartRelease.RotationalStiffnessZ = startSpring[5];

            Constraint6DOF bhEndRelease = new Constraint6DOF();

            bhEndRelease.TranslationX = endRelease[0] == true ? endSpring[0] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhEndRelease.TranslationY = endRelease[1] == true ? endSpring[1] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhEndRelease.TranslationZ = endRelease[2] == true ? endSpring[2] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhEndRelease.RotationX = endRelease[3] == true ? endSpring[3] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhEndRelease.RotationY = endRelease[4] == true ? endSpring[4] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;
            bhEndRelease.RotationZ = endRelease[5] == true ? endSpring[5] == 0 ? DOFType.Free : DOFType.Spring : DOFType.Fixed;

            bhEndRelease.TranslationalStiffnessX = endSpring[0];
            bhEndRelease.TranslationalStiffnessY = endSpring[1];
            bhEndRelease.TranslationalStiffnessZ = endSpring[2];
            bhEndRelease.RotationalStiffnessX = endSpring[3];
            bhEndRelease.RotationalStiffnessY = endSpring[4];
            bhEndRelease.RotationalStiffnessZ = endSpring[5];

            BarRelease barRelease = new BarRelease() { StartRelease = bhStartRelease, EndRelease = bhEndRelease };

            return barRelease;
        }

        /***************************************************/
    }
}


