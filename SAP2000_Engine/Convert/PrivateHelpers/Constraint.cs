using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Properties.Constraint;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
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

        public static void SetConstraint6DOF(Constraint6DOF bhConstraint, ref bool[] restraint, ref double[] spring)
        {
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
    }
}
