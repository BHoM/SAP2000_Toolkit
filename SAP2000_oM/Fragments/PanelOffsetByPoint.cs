using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    public class PanelOffsetByPoint : IPanelOffset
    {
        [Length]
        [Description("This is an array of joint offsets for each of the points that define the area object.")]
        public virtual double[] Offset { get; set; } = null;
    }
}