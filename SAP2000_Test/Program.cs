using System;
using System.Collections.Generic;
using System.Linq;
using BH.Adapter.SAP2000;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Elements;
using BH.oM.DataManipulation.Queries;

namespace SAP2000_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SAP Should now open...");
            SAP2000Adapter app = new SAP2000Adapter("", true);
            Console.WriteLine("If SAP is open, I worked! press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");

            TestPullNodes(app);

            TestPullBars(app);

            TestPullSections(app);

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void TestPullNodes(SAP2000Adapter app)
        {
            Console.WriteLine("Test Pull Nodes");
            FilterQuery nodeQuery = new FilterQuery { Type = typeof(Node) };

            IEnumerable<object> nodeObjects = app.Pull(nodeQuery);

            Console.WriteLine("I found " + nodeObjects.Count() + " nodes");

            foreach (object bObject in nodeObjects)
            {
                Node node = bObject as Node;
                string nodeId = node.CustomData[SAP2000Adapter.ID].ToString();
                
                string nodeInfo = "Node with ID: " + nodeId;
                Console.WriteLine(nodeInfo);
            }

            Console.WriteLine("Pulled all nodes");
        }

        private static void TestPullBars(SAP2000Adapter app)
        {
            Console.WriteLine("Test Pull Bars");
            FilterQuery barQuery = new FilterQuery { Type = typeof(Bar) };

            IEnumerable<object> barObjects = app.Pull(barQuery);

            Console.WriteLine("I found " + barObjects.Count() + " bars");
            
            foreach (object bObject in barObjects)
            {
                Bar bar = bObject as Bar;
                string barId = bar.CustomData[SAP2000Adapter.ID].ToString();
                string startNodeId = bar.StartNode.CustomData[SAP2000Adapter.ID].ToString();
                string endNodeId = bar.EndNode.CustomData[SAP2000Adapter.ID].ToString();

                string barInfo = "Bar with ID: " + barId + " Connecting Nodes " + startNodeId + " and " + endNodeId;
                Console.WriteLine(barInfo);
            }

            Console.WriteLine("Pulled all bars");

        }

        private static void TestPullSections(SAP2000Adapter app)
        {
            Console.WriteLine("Test Pull Sections");
            FilterQuery sectionQuery = new FilterQuery { Type = typeof(ISectionProperty) };

            IEnumerable<object> sectionObjects = app.Pull(sectionQuery);

            Console.WriteLine("I found " + sectionObjects.Count() + " sections");

            foreach (object bObject in sectionObjects)
            {
                ISectionProperty section = bObject as ISectionProperty;
                string sectionId = section.CustomData[SAP2000Adapter.ID].ToString();

                string sectionInfo = "Sections with ID: " + sectionId;
                Console.WriteLine(sectionInfo);
            }

            Console.WriteLine("Pulled all Sections");
        }

    }
}
