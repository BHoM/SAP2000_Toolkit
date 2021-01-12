using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.SAP2000.Elements;
using BH.Engine.Base;

namespace BH.Engine.Adapters.SAP2000
{
    public static partial class Modify
    {
        public static Bar SetAutoMesh(this Bar bar, bool autoMesh = false, bool autoMeshAtPoints = false, bool autoMeshAtLines = false, int numSegs = 0, double autoMeshMaxLength = 0.0)
        {
            if (numSegs < 0)
            {
                numSegs = 0;
                Engine.Reflection.Compute.RecordWarning("Number of segments must be positive or zero. If zero, number of elements is not checked when automatic meshing is done.");
            }

            if (autoMeshMaxLength < 0)
            {
                autoMeshMaxLength = 0.0;
                Engine.Reflection.Compute.RecordWarning("Max length must be positive. If zero, element length is not checked when automatic meshing is done.");
            }

            return (Bar)bar.AddFragment(new BarAutoMesh { AutoMesh = autoMesh, AutoMeshAtPoints = autoMeshAtPoints, AutoMeshAtLines = autoMeshAtLines, NumSegs = numSegs, AutoMeshMaxLength = autoMeshMaxLength });
        }
    }
}
