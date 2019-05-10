using BH.oM.Structure.Constraints;

namespace BH.Engine.SAP2000
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
    }
}
