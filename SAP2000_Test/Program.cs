using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Common.Materials;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Data.Requests;
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
            FileAdapter doc = new FileAdapter("C: /Users/jtaylor/GitHub/SAP2000_Toolkit/SAP2000_Test", "Test_Structure");

            FilterRequest barQuery = new FilterRequest { Type = typeof(Bar) };
            FilterRequest panelQuery = new FilterRequest { Type = typeof(Panel) };

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

            //Console.WriteLine("pulled " + barsPulled.Count() + " bars and " + panelsPulled.Count() + " Panels");
            Console.WriteLine("Pushed " + numPushed + " Objects, pulled " + numPulled + " Objects.");
            //foreach (Bar bar in barsPulled)
            //{
            //    Console.WriteLine("Bar with ID " + bar.CustomData["SAP2000_id"].ToString() + " and property " + bar.SectionProperty.Name);
            //}
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");
        }

        /***************************************************/
    }
}
