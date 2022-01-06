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

        public static BarRelease GetBarRelease(bool[] startRestraint, double[] startSpring, bool[] endRestraint, double[] endSpring)
        {
            Constraint6DOF startRelease = new Constraint6DOF();

            startRelease.TranslationX = startRestraint[0] == true ? DOFType.Free : DOFType.Fixed;
            startRelease.TranslationY = startRestraint[1] == true ? DOFType.Free : DOFType.Fixed;
            startRelease.TranslationZ = startRestraint[2] == true ? DOFType.Free : DOFType.Fixed;
            startRelease.RotationX = startRestraint[3] == true ? DOFType.Free : DOFType.Fixed;
            startRelease.RotationY = startRestraint[4] == true ? DOFType.Free : DOFType.Fixed;
            startRelease.RotationZ = startRestraint[5] == true ? DOFType.Free : DOFType.Fixed;

            startRelease.TranslationalStiffnessX = startSpring[0];
            startRelease.TranslationalStiffnessY = startSpring[1];
            startRelease.TranslationalStiffnessZ = startSpring[2];
            startRelease.RotationalStiffnessX = startSpring[3];
            startRelease.RotationalStiffnessY = startSpring[4];
            startRelease.RotationalStiffnessZ = startSpring[5];

            Constraint6DOF endRelease = new Constraint6DOF();

            endRelease.TranslationX = endRestraint[0] == true ? DOFType.Free : DOFType.Fixed;
            endRelease.TranslationY = endRestraint[1] == true ? DOFType.Free : DOFType.Fixed;
            endRelease.TranslationZ = endRestraint[2] == true ? DOFType.Free : DOFType.Fixed;
            endRelease.RotationX = endRestraint[3] == true ? DOFType.Free : DOFType.Fixed;
            endRelease.RotationY = endRestraint[4] == true ? DOFType.Free : DOFType.Fixed;
            endRelease.RotationZ = endRestraint[5] == true ? DOFType.Free : DOFType.Fixed;

            endRelease.TranslationalStiffnessX = endSpring[0];
            endRelease.TranslationalStiffnessY = endSpring[1];
            endRelease.TranslationalStiffnessZ = endSpring[2];
            endRelease.RotationalStiffnessX = endSpring[3];
            endRelease.RotationalStiffnessY = endSpring[4];
            endRelease.RotationalStiffnessZ = endSpring[5];

            BarRelease barRelease = new BarRelease() { StartRelease = startRelease, EndRelease = endRelease };

            return barRelease;
        }

        /***************************************************/
    }
}


