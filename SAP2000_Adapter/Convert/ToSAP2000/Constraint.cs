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

using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using System.Linq;

namespace BH.Adapter.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void GetSAPConstraint(this Node node, ref bool[] restraint, ref double[] spring)
        {
            Constraint6DOF bhConstraint = node.Support;

            restraint = new bool[6];
            restraint[0] = bhConstraint.TranslationX == DOFType.Fixed;
            restraint[1] = bhConstraint.TranslationY == DOFType.Fixed;
            restraint[2] = bhConstraint.TranslationZ == DOFType.Fixed;
            restraint[3] = bhConstraint.RotationX == DOFType.Fixed;
            restraint[4] = bhConstraint.RotationY == DOFType.Fixed;
            restraint[5] = bhConstraint.RotationZ == DOFType.Fixed;

            spring = new double[6];
            spring[0] = bhConstraint.TranslationalStiffnessX;
            spring[1] = bhConstraint.TranslationalStiffnessY;
            spring[2] = bhConstraint.TranslationalStiffnessZ;
            spring[3] = bhConstraint.RotationalStiffnessX;
            spring[4] = bhConstraint.RotationalStiffnessY;
            spring[5] = bhConstraint.RotationalStiffnessZ;
        }

        /***************************************************/

        public static bool ToSAP(this BarRelease release, ref bool[] startRestraint, ref double[] startSpring, ref bool[] endRestraint, ref double[] endSpring)
        {
            if (release.StartRelease == null)
            {
                Engine.Reflection.Compute.RecordNote("Start Release was null, no release was set");
                return false;
            }

            startRestraint = new bool[6];
            startRestraint[0] = release.StartRelease.TranslationX == DOFType.Free;
            startRestraint[1] = release.StartRelease.TranslationY == DOFType.Free;
            startRestraint[2] = release.StartRelease.TranslationZ == DOFType.Free;
            startRestraint[3] = release.StartRelease.RotationX == DOFType.Free;
            startRestraint[4] = release.StartRelease.RotationY == DOFType.Free;
            startRestraint[5] = release.StartRelease.RotationZ == DOFType.Free;

            startSpring = new double[6];
            startSpring[0] = release.StartRelease.TranslationalStiffnessX;
            startSpring[1] = release.StartRelease.TranslationalStiffnessY;
            startSpring[2] = release.StartRelease.TranslationalStiffnessZ;
            startSpring[3] = release.StartRelease.RotationalStiffnessX;
            startSpring[4] = release.StartRelease.RotationalStiffnessY;
            startSpring[5] = release.StartRelease.RotationalStiffnessZ;


            if (release.EndRelease == null)
            {
                Engine.Reflection.Compute.RecordNote("End Release was null, no release was set");
                return false;
            }

            endRestraint = new bool[6];
            endRestraint[0] = release.EndRelease.TranslationX == DOFType.Free;
            endRestraint[1] = release.EndRelease.TranslationY == DOFType.Free;
            endRestraint[2] = release.EndRelease.TranslationZ == DOFType.Free;
            endRestraint[3] = release.EndRelease.RotationX == DOFType.Free;
            endRestraint[4] = release.EndRelease.RotationY == DOFType.Free;
            endRestraint[5] = release.EndRelease.RotationZ == DOFType.Free;

            endSpring = new double[6];
            endSpring[0] = release.EndRelease.TranslationalStiffnessX;
            endSpring[1] = release.EndRelease.TranslationalStiffnessY;
            endSpring[2] = release.EndRelease.TranslationalStiffnessZ;
            endSpring[3] = release.EndRelease.RotationalStiffnessX;
            endSpring[4] = release.EndRelease.RotationalStiffnessY;
            endSpring[5] = release.EndRelease.RotationalStiffnessZ;

            bool[] startReleased = startRestraint.Zip(startSpring, (x, y) => x && y == 0).ToArray();
            bool[] endReleased = endRestraint.Zip(endSpring, (x, y) => x && y == 0).ToArray();

            if (startReleased[0] && endReleased[0])
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release TranslationX for both ends"); return false; }
            if (startReleased[1] && endReleased[1])
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release TranslationZ for both ends"); return false; }
            if (startReleased[2] && endReleased[2])
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release TranslationY for both ends"); return false; }
            if (startReleased[3] && endReleased[3])
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release RotationX for both ends"); return false; }
            if (startReleased[4] && endReleased[4] && (startReleased[2] || endReleased[2]))
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release TranslationY when RotationZ is released for both ends"); return false; }
            if (startReleased[5] && endReleased[5] && (startReleased[1] || endReleased[1]))
            { Engine.Reflection.Compute.RecordWarning($"Unstable releases have not been set, can not release TranslationZ when RotationY is released for both ends"); return false; }

            return true;
        }

        /***************************************************/

        public static void ToSAP2000(this LinkConstraint linkConstraint, out bool[] dof, out bool[] fix, out double[] stiff, out double[] damp, out double dj2, out double dj3)
        {
            dof = new bool[6] { true,true,true,true,true,true };

            fix = new bool[6] {
                linkConstraint.XtoX,
                linkConstraint.YtoY,
                linkConstraint.ZtoZ,
                linkConstraint.XXtoXX,
                linkConstraint.YYtoYY,
                linkConstraint.ZZtoZZ
            };

            stiff = new double[6] { 0, 0, 0, 0, 0, 0 };

            damp = new double[6] { 0, 0, 0, 0, 0, 0 };

            dj2 = 0;

            dj3 = 0;
        }
        
    }
}
