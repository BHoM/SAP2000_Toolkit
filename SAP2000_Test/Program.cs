using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.SAP2000;
using BH.oM.Common.Materials;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.DataManipulation.Queries;

namespace SAP2000_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SAP Should now open...");
            SAP2000Adapter app = new SAP2000Adapter();
            Console.WriteLine("If SAP is open, I worked! press any key");
            Console.ReadKey();

        }
    }
}
