using BH.oM.Structure.Constraints;

namespace BH.Engine.SAP2000
{
    public static partial class Convert
    {   
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint6DOF GetConstraint6DOF(this bool[] restraint, double[] spring)
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

        public static BarRelease GetBarRelease(this bool[] startRestraint, double[] startSpring, bool[] endRestraint, double[] endSpring)
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
