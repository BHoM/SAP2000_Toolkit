using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Common.Materials;
using BH.oM.Structural.Properties;
using BH.oM.Structural.Elements;
using BH.oM.DataManipulation.Queries;
using BH.Adapter.SAP2000;

namespace SAP2000_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SAP Should now open...");
            SAP2000Adapter app = new SAP2000Adapter("", true);
            //Console.WriteLine("If SAP is open, I worked! press any key to continue");
            //Console.ReadKey();
            //Console.WriteLine("");

            TestPushMaterials(app);
                
            //TestPushSections(app);

            //TestPushBars(app);

            //TestPullNodes(app);

            //TestPullBars(app);

            //TestPullSections(app);

            //TestPullPanels(app);

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void TestPushMaterials(SAP2000Adapter app)
        {
            // CREATE MATERIALS //

            Material steel = BH.Engine.Common.Create.Material("mySteel", MaterialType.Steel, 210000, 0.3, 0.00012, 78500);
            Material concrete = BH.Engine.Common.Create.Material("myConcrete", MaterialType.Concrete, 10, 10, 10, 10);
            
            List<Material> materials = new List<Material>
            {
                steel,
                concrete
            };

            app.Push(materials, "Materials");

        }

        private static void TestPushSections(SAP2000Adapter app)
        {
            // CREATE MATERIALS //

            Material steel = BH.Engine.Common.Create.Material("mySteel", MaterialType.Steel, 210000, 0.3, 0.00012, 78500);
            Material concrete = BH.Engine.Common.Create.Material("myConcrete", MaterialType.Concrete, 10, 10, 10, 10);
            //Material steelOther = BH.Engine.Common.Create.Material("otherSteel", MaterialType.Steel,    210000, 0.3, 0.00012, 78500);

            ISectionProperty sec0 = BH.Engine.Structure.Create.SteelISection(110, 10, 80, 20);
            sec0.Material = steel;
            sec0.Name = "Steel Section";

            ISectionProperty sec1 = BH.Engine.Structure.Create.ConcreteRectangleSection(200, 120);
            sec1.Material = concrete;
            sec1.Name = "Concrete Section";

            IProperty2D panelProp = BH.Engine.Structure.Create.ConstantThickness(100, concrete);
            panelProp.Name = "Concrete Slab";

            List<ISectionProperty> barProps = new List<ISectionProperty>
            {

                sec1,
                sec0
            };

            app.Push(barProps, "BarSecs");

        }

        private static void TestPushBars(SAP2000Adapter app)
        {
            // CREATE MATERIALS //

            Material steel = BH.Engine.Common.Create.Material("mySteel", MaterialType.Steel, 210000, 0.3, 0.00012, 78500);
            Material concrete = BH.Engine.Common.Create.Material("myConcrete", MaterialType.Concrete, 10, 10, 10, 10);
            //Material steelOther = BH.Engine.Common.Create.Material("otherSteel", MaterialType.Steel,    210000, 0.3, 0.00012, 78500);

            ISectionProperty sec0 = BH.Engine.Structure.Create.SteelISection(110, 10, 80, 20);
            sec0.Material = steel;
            sec0.Name = "Steel Section";

            ISectionProperty sec1 = BH.Engine.Structure.Create.ConcreteRectangleSection(200, 120);
            sec1.Material = concrete;
            sec1.Name = "Concrete Section";

            IProperty2D panelProp = BH.Engine.Structure.Create.ConstantThickness(100, concrete);
            panelProp.Name = "Concrete Slab";

            // CREATE CONSTRAINTS //

            Constraint6DOF pin = BH.Engine.Structure.Create.PinConstraint6DOF();
            Constraint6DOF fix = BH.Engine.Structure.Create.FixConstraint6DOF();
            Constraint6DOF full = BH.Engine.Structure.Create.FullReleaseConstraint6DOF();

            // DEFINE GEOMETRY //

            Point p01 = new Point { X = 0, Y = 0, Z = 0 };
            Point p02 = new Point { X = 1, Y = 0, Z = 0 };
            Point p03 = new Point { X = 1, Y = 1, Z = 0 };
            Point p04 = new Point { X = 0, Y = 1, Z = 0 };

            Point p11 = new Point { X = 0, Y = 0, Z = 1 };
            Point p12 = new Point { X = 1, Y = 0, Z = 1 };
            Point p13 = new Point { X = 1, Y = 1, Z = 1 };
            Point p14 = new Point { X = 0, Y = 1, Z = 1 };

            Point p21 = new Point { X = 0, Y = 0, Z = 2 };
            Point p22 = new Point { X = 1, Y = 0, Z = 2 };
            Point p23 = new Point { X = 1, Y = 1, Z = 2 };
            Point p24 = new Point { X = 0, Y = 1, Z = 2 };

            Point op11 = new Point { X = 0.2, Y = 0.2, Z = 1 };
            Point op12 = new Point { X = 0.8, Y = 0.2, Z = 1 };
            Point op13 = new Point { X = 0.8, Y = 0.8, Z = 1 };
            Point op14 = new Point { X = 0.2, Y = 0.8, Z = 1 };

            // CREATE NODES //

            List<Node> nodesA = new List<Node>();

            Node n11 = new Node { Position = p11, Name = "1" };
            Node n12 = new Node { Position = p12, Name = "2" };
            Node n13 = new Node { Position = p13, Name = "3" };
            Node n14 = new Node { Position = p14, Name = "4" };

            n11.Constraint = pin;
            n12.Constraint = pin;
            n13.Constraint = fix;
            n14.Constraint = fix;

            nodesA.Add(n11);
            nodesA.Add(n12);
            nodesA.Add(n13);
            nodesA.Add(n14);

            List<Node> nodesB = new List<Node>();

            Node n21 = new Node { Position = p21, Name = "1" };
            Node n22 = new Node { Position = p22, Name = "2" };
            Node n23 = new Node { Position = p23, Name = "3" };
            Node n24 = new Node { Position = p24, Name = "4" };

            n21.Constraint = pin;
            n22.Constraint = pin;
            n23.Constraint = full;
            n24.Constraint = fix;

            nodesB.Add(n21);
            nodesB.Add(n22);
            nodesB.Add(n23);
            nodesB.Add(n24);

            // CREATE BARS //

            Bar bar01 = BH.Engine.Structure.Create.Bar(new Node { Position = p01 }, new Node { Position = p02 });
            Bar bar02 = BH.Engine.Structure.Create.Bar(new Node { Position = p02 }, new Node { Position = p03 });
            Bar bar03 = BH.Engine.Structure.Create.Bar(new Node { Position = p03 }, new Node { Position = p04 });
            Bar bar04 = BH.Engine.Structure.Create.Bar(new Node { Position = p04 }, new Node { Position = p01 });

            Bar bar11 = BH.Engine.Structure.Create.Bar(new Node { Position = p11 }, new Node { Position = p12 });
            Bar bar12 = BH.Engine.Structure.Create.Bar(new Node { Position = p12 }, new Node { Position = p13 });
            Bar bar13 = BH.Engine.Structure.Create.Bar(new Node { Position = p13 }, new Node { Position = p14 });
            Bar bar14 = BH.Engine.Structure.Create.Bar(new Node { Position = p14 }, new Node { Position = p11 });

            Bar col11 = BH.Engine.Structure.Create.Bar(new Node { Position = p01 }, new Node { Position = p11 });
            Bar col12 = BH.Engine.Structure.Create.Bar(new Node { Position = p02 }, new Node { Position = p12 });
            Bar col13 = BH.Engine.Structure.Create.Bar(new Node { Position = p03 }, new Node { Position = p13 });
            Bar col14 = BH.Engine.Structure.Create.Bar(new Node { Position = p04 }, new Node { Position = p14 });

            Bar bar21 = BH.Engine.Structure.Create.Bar(new Node { Position = p21 }, new Node { Position = p22 });
            Bar bar22 = BH.Engine.Structure.Create.Bar(new Node { Position = p22 }, new Node { Position = p23 });
            Bar bar23 = BH.Engine.Structure.Create.Bar(new Node { Position = p23 }, new Node { Position = p24 });
            Bar bar24 = BH.Engine.Structure.Create.Bar(new Node { Position = p24 }, new Node { Position = p21 });

            Bar col21 = BH.Engine.Structure.Create.Bar(new Node { Position = p11 }, new Node { Position = p21 });
            Bar col22 = BH.Engine.Structure.Create.Bar(new Node { Position = p12 }, new Node { Position = p22 });
            Bar col23 = BH.Engine.Structure.Create.Bar(new Node { Position = p13 }, new Node { Position = p23 });
            Bar col24 = BH.Engine.Structure.Create.Bar(new Node { Position = p14 }, new Node { Position = p24 });

            List<Bar> bars0 = new List<Bar>();
            List<Bar> bars1 = new List<Bar>();
            List<Bar> bars2 = new List<Bar>();

            bars0.Add(bar01);
            bars0.Add(bar02);
            bars0.Add(bar03);
            bars0.Add(bar04);

            bars1.Add(bar11);
            bars1.Add(bar12);
            bars1.Add(bar13);
            bars1.Add(bar14);
            bars1.Add(col11);
            bars1.Add(col12);
            bars1.Add(col13);
            bars1.Add(col14);

            bars2.Add(bar21);
            bars2.Add(bar22);
            bars2.Add(bar23);
            bars2.Add(bar24);
            bars2.Add(col21);
            bars2.Add(col22);
            bars2.Add(col23);
            bars2.Add(col24);

            foreach (Bar b in bars0)
                b.SectionProperty = sec0;

            foreach (Bar b in bars1)
                b.SectionProperty = sec1;

            foreach (Bar b in bars2)
                b.SectionProperty = sec0;

            // CREATE PANELS //

            List<PanelPlanar> panels = new List<PanelPlanar>();

            Polyline outline = new Polyline()
            {
                ControlPoints = new List<Point>() { p01, p02, p03, p04, p01 }
            };
            List<Opening> openings = null;

            PanelPlanar panelA = BH.Engine.Structure.Create.PanelPlanar(outline, openings);
            panelA.Property = panelProp;
            panels.Add(panelA);

            outline.ControlPoints = new List<Point>() { p11, p12, p13, p14, p11 };
            Opening opening = new Opening() { Edges = new List<Edge>() { new Edge() { Curve = new Polyline() { ControlPoints = new List<Point>() { op11, op12, op13, op14 } } } } };
            openings = new List<Opening> { opening };

            PanelPlanar panelB = BH.Engine.Structure.Create.PanelPlanar(outline, openings);
            panelB.Property = panelProp;
            panels.Add(panelB);


            app.Push(nodesA, "Nodes");
            app.Push(nodesB, "Nodes");
            app.Push(bars0, "Bars0");
            app.Push(bars1, "Bars1");
            app.Push(bars2, "Bars2");

            app.Push(panels, "panels");

            Console.WriteLine("All elements Pushed!");
            Console.ReadKey();
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

        private static void TestPullPanels(SAP2000Adapter app)
        {
            Console.WriteLine("Test Pull Panels");
            FilterQuery panelQuery = new FilterQuery { Type = typeof(PanelPlanar) };

            IEnumerable<object> panelObjects = app.Pull(panelQuery);

            Console.WriteLine("I found " + panelObjects.Count() + " panels");


            foreach (object bObject in panelObjects)
            {
                PanelPlanar panel = bObject as PanelPlanar;
                string panelId = panel.CustomData[SAP2000Adapter.ID].ToString();
                string panelSection = panel.Property.Name.ToString();

                string info = "Panel with ID: " + panelId + " and Section Property: " + panelSection;
                Console.WriteLine(info);
            }

            Console.WriteLine("Pulled all panels");

        }
    }
}
