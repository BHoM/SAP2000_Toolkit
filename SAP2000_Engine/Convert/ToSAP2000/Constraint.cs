﻿using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;

namespace BH.Engine.SAP2000
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

        public static void GetSAPBarRelease(this Bar bar, ref bool[] startRestraint, ref double[] startSpring, ref bool[] endRestraint, ref double[] endSpring)
        {
            BarRelease release = bar.Release;

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
        }

        /***************************************************/
    }
}