using BH.oM.Structure.Properties.Constraint;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

        /***************************************************/
    }
}
