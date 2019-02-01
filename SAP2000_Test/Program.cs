using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Common.Materials;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Elements;
using BH.oM.DataManipulation.Queries;
using BH.Adapter;
using BH.Adapter.SAP2000;
using BH.Engine.Structure;
using BH.Adapter.FileAdapter;


namespace SAP2000_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SAP Should now open...");
            SAP2000Adapter app = new SAP2000Adapter("", true);
            FileAdapter doc = new FileAdapter("C: /Users/jtaylor/Documents/GitHub/SAP2000_Toolkit/SAP2000_Test", "Test_Structure", true);

            FilterQuery barQuery = new FilterQuery { Type = typeof(Bar) };
            FilterQuery panelQuery = new FilterQuery { Type = typeof(PanelPlanar) };

            IEnumerable<object> bars = doc.Pull(barQuery);
            IEnumerable<object> panels = doc.Pull(panelQuery);

            int numPushed = bars.Count() + panels.Count();

            IEnumerable<BH.oM.Base.BHoMObject> barObjects = (IEnumerable<BH.oM.Base.BHoMObject>)bars;
            IEnumerable<BH.oM.Base.BHoMObject> panelObjects = (IEnumerable<BH.oM.Base.BHoMObject>)panels;

            app.Push(barObjects, "");
            app.Push(panelObjects, "");

            IEnumerable<object> barsPulled = app.Pull(barQuery);
            IEnumerable<object> panelsPulled = app.Pull(panelQuery);

            int numPulled = barsPulled.Count() + panelsPulled.Count();

            Console.WriteLine("Pushed " + numPushed + " Objects, pulled " + numPulled + " Objects.");
            foreach (Bar bar in barsPulled)
            {
                Console.WriteLine("Bar with ID " + bar.CustomData["SAP2000_id"].ToString() + " and property " + bar.SectionProperty.Name);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");
        }
    }
}
