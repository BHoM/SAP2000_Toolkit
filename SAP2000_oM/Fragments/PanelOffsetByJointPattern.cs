using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.oM.Adapters.SAP2000.Fragments
{

    public class PanelOffsetByJointPattern : IPanelOffset
    {
        [Description("This is the name of the defined joint pattern that is used to calculate the joint offsets.")]
        public virtual string OffsetPattern { get; set; } = "";

        [Length]
        [Description("This is the scale factor applied to the joint pattern when calculating the joint offsets.")]
        public virtual double OffsetPatternSF { get; set; } = 0;
    }
}