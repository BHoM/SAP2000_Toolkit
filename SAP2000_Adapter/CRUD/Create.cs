using System.Collections.Generic;
using System.Linq;
using BH.oM.Architecture.Elements;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Common.Materials;
using BH.Engine.SAP2000;

namespace BH.Adapter.SAP2000
{
    public partial class SAP2000Adapter
    {

        /***************************************************/
        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            bool success = true;

            if (typeof(BH.oM.Base.IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                success = CreateCollection(objects);
            }
            else
            {
                success = false;
            }

            m_model.View.RefreshView();
            return success;
        }

        /***************************************************/

        private bool CreateCollection<T>(IEnumerable<T> objects) where T : BH.oM.Base.IObject
        {
            bool success = true;


            foreach (T obj in objects)
            {
                success &= CreateObject(obj as dynamic);
            }
            
            return success;
        }

        /***************************************************/

        private bool CreateObject(Node bhNode)
        {
            bool success = true;
            int retA = 0;
            int retB = 0;
            int retC = 0;

            string name = "";
            string bhId = bhNode.CustomData[AdapterId].ToString();
            name = bhId;

            retA = m_model.PointObj.AddCartesian(bhNode.Position.X, bhNode.Position.Y, bhNode.Position.Z, ref name);

            if (name != bhId)
                bhNode.CustomData[AdapterId] = name;

            if (bhNode.Constraint != null)
            {
                bool[] restraint = new bool[6];
                restraint[0] = bhNode.Constraint.TranslationX == DOFType.Fixed;
                restraint[1] = bhNode.Constraint.TranslationY == DOFType.Fixed;
                restraint[2] = bhNode.Constraint.TranslationZ == DOFType.Fixed;
                restraint[3] = bhNode.Constraint.RotationX == DOFType.Fixed;
                restraint[4] = bhNode.Constraint.RotationY == DOFType.Fixed;
                restraint[5] = bhNode.Constraint.RotationZ == DOFType.Fixed;

                double[] spring = new double[6];
                spring[0] = bhNode.Constraint.TranslationalStiffnessX;
                spring[1] = bhNode.Constraint.TranslationalStiffnessY;
                spring[2] = bhNode.Constraint.TranslationalStiffnessZ;
                spring[3] = bhNode.Constraint.RotationalStiffnessX;
                spring[4] = bhNode.Constraint.RotationalStiffnessY;
                spring[5] = bhNode.Constraint.RotationalStiffnessZ;

                retB = m_model.PointObj.SetRestraint(name, ref restraint);
                retC = m_model.PointObj.SetSpring(name, ref spring);
            }

            if (retA != 0 || retB != 0 || retC != 0)
                success = false;

            return success;
        }

        /***************************************************/

        private bool CreateObject(Bar bhBar)
        {
            int ret = 0;

            string name = "";
            string bhId = bhBar.CustomData[AdapterId].ToString();
            name = bhId;

            ret = m_model.FrameObj.AddByPoint(bhBar.StartNode.CustomData[AdapterId].ToString(), bhBar.EndNode.CustomData[AdapterId].ToString(), ref name);

            if (ret != 0)
            {
                CreateElementError("Bar", name);
                return false;
            }

            if (m_model.FrameObj.SetSection(name, bhBar.SectionProperty.Name) != 0)
            {
                CreatePropertyWarning("SectionProperty", "Bar", name);
                ret++;
            }

            if (m_model.FrameObj.SetLocalAxes(name, bhBar.OrientationAngle * 180 / System.Math.PI) != 0)
            {
                CreatePropertyWarning("Orientation angle", "Bar", name);
                ret++;
            }

            Offset offset = bhBar.Offset;

            double[] offset1 = new double[3];
            double[] offset2 = new double[3];

            if (offset != null)
            {
                offset1[1] = offset.Start.Z;
                offset1[2] = offset.Start.Y;
                offset2[1] = offset.End.Z;
                offset2[2] = offset.End.Y;
            }

            BarRelease barRelease = bhBar.Release;
            if (barRelease != null)
            {
                bool[] restraintStart = barRelease.StartRelease.Fixities();// Helper.GetRestraint6DOF(barRelease.StartRelease);
                double[] springStart = barRelease.StartRelease.ElasticValues();// Helper.GetSprings6DOF(barRelease.StartRelease);
                bool[] restraintEnd = barRelease.EndRelease.Fixities();// Helper.GetRestraint6DOF(barRelease.EndRelease);
                double[] springEnd = barRelease.EndRelease.ElasticValues();// Helper.GetSprings6DOF(barRelease.EndRelease);

                if (m_model.FrameObj.SetReleases(name, ref restraintStart, ref restraintEnd, ref springStart, ref springEnd) != 0)
                {
                    CreatePropertyWarning("Release", "Bar", name);
                    ret++;
                }
            }

            else if (bhBar.Offset != null)
            {
                if (m_model.FrameObj.SetEndLengthOffset(name, false, -1 * (bhBar.Offset.Start.X), bhBar.Offset.End.X, 1) != 0)
                {
                    CreatePropertyWarning("Length offset", "Bar", name);
                    ret++;
                }
            }

            return ret == 0;
        }

        /***************************************************/

        private bool CreateObject(ISectionProperty bhSection)
        {
            bool success = true;

            Helper.SetSectionProperty(m_model, bhSection);

            return success;
        }

        /***************************************************/

        private bool CreateObject(Material material)
        {
            bool success = true;

            Helper.SetMaterial(m_model, material);

            return success;
        }

        /***************************************************/

        private bool CreateObject(ISurfaceProperty surfaceProperty)
        {
            int ret = 0;

            string propertyName = surfaceProperty.Name;// surfaceProperty.CustomData[AdapterId].ToString();
            
            if (surfaceProperty.GetType() == typeof(Waffle))
            {
                // not implemented!
                CreatePropertyError("Waffle Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(Ribbed))
            {
                // not implemented!
                CreatePropertyError("Ribbed Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(LoadingPanelProperty))
            {
                // not implemented!
                CreatePropertyError("Loading Panel Not Implemented!", "PanelPlanar", propertyName);
            }
            else if (surfaceProperty.GetType() == typeof(ConstantThickness))
            {
                ConstantThickness constantThickness = (ConstantThickness)surfaceProperty;
                ret = m_model.PropArea.SetShell(propertyName, 0, surfaceProperty.Material.Name, 0, constantThickness.Thickness, constantThickness.Thickness);
            }

            if (surfaceProperty.HasModifiers())
            {
                double[] modifier = surfaceProperty.Modifiers();//(double[])surfaceProperty.CustomData["Modifiers"];
                m_model.PropArea.SetModifiers(propertyName, ref modifier);
            }

            return ret == 0;
        }
        
        /***************************************************/

        private bool CreateObject(PanelPlanar bhPanel)
        {
            bool success = true;
            int retA = 0;

            double mergeTol = 1e-3; 

            string name = bhPanel.CustomData[AdapterId].ToString();
            string propertyName = bhPanel.Property.Name;
            List<BH.oM.Geometry.Point> boundaryPoints = bhPanel.ControlPoints(true).CullDuplicates(mergeTol);

            int segmentCount = boundaryPoints.Count();
            double[] x = new double[segmentCount];
            double[] y = new double[segmentCount];
            double[] z = new double[segmentCount];
            for (int i = 0; i < segmentCount; i++)
            {
                x[i] = boundaryPoints[i].X;
                y[i] = boundaryPoints[i].Y;
                z[i] = boundaryPoints[i].Z;
            }

            retA = m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref name, propertyName);

            if (retA != 0)
                return false;

            if (bhPanel.Openings != null)
            {
                for (int i = 0; i < bhPanel.Openings.Count; i++)
                {
                    boundaryPoints = bhPanel.Openings[i].ControlPoints().CullDuplicates(mergeTol);

                    segmentCount = boundaryPoints.Count();
                    x = new double[segmentCount];
                    y = new double[segmentCount];
                    z = new double[segmentCount];

                    for (int j = 0; j < segmentCount; j++)
                    {
                        x[j] = boundaryPoints[j].X;
                        y[j] = boundaryPoints[j].Y;
                        z[j] = boundaryPoints[j].Z;
                    }

                    string openingName = name + "_Opening_" + i;
                    m_model.AreaObj.AddByCoord(segmentCount, ref x, ref y, ref z, ref openingName, "");//<-- setting panel property to empty string, verify that this is correct
                    m_model.AreaObj.SetOpening(openingName, true);
                }
            }

            return success;
        }
        
        /***************************************************/

        private bool CreateObject(Loadcase loadcase)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", loadcase.Name);
            return false;
            //bool success = true;

            //Helper.SetLoadcase(model, loadcase);

            //return success;
        }
        
        /***************************************************/

        private bool CreateObject(LoadCombination loadcombination)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", loadcombination.Name);
            return false;
            //bool success = true;

            //Helper.SetLoadCombination(model, loadcombination);

            //return success;
        }

        /***************************************************/

        private bool CreateObject(ILoad bhLoad)
        {
            // not implemented!
            CreateElementError("Loads Not Implemented!", bhLoad.Name);
            return false;
            //bool success = true;

            //Helper.SetLoad(model, bhLoad as dynamic);


            //return success;
        }

        /***************************************************/

        private bool CreateObject(RigidLink bhLink)
        {
            int ret = 0;

            string name = "";
            string givenName = "";
            string bhId = bhLink.CustomData[AdapterId].ToString();
            name = bhId;
            
            LinkConstraint constraint = bhLink.Constraint;//not used yet
            Node masterNode = bhLink.MasterNode;
            List<Node> slaveNodes = bhLink.SlaveNodes;
            bool multiSlave = slaveNodes.Count() == 1 ? false : true;

            for (int i = 0; i < slaveNodes.Count(); i++)
            {
                name = multiSlave == true ? name + ":::" + i : name;
                ret = m_model.LinkObj.AddByPoint(masterNode.CustomData[AdapterId].ToString(), slaveNodes[i].CustomData[AdapterId].ToString(), ref givenName, false, "Default", name);
            }

            return ret == 0;
        }

        /***************************************************/

        private void CreateElementError(string elemType, string elemName)
        {
            Engine.Reflection.Compute.RecordError("Failed to create the element of type " + elemType + ", with id: " + elemName);
        }

        /***************************************************/

        private void CreatePropertyError(string failedProperty, string elemType, string elemName)
        {
            CreatePropertyEvent(failedProperty, elemType, elemName, oM.Reflection.Debugging.EventType.Error);
        }

        /***************************************************/

        private void CreatePropertyWarning(string failedProperty, string elemType, string elemName)
        {
            CreatePropertyEvent(failedProperty, elemType, elemName, oM.Reflection.Debugging.EventType.Warning);
        }

        /***************************************************/

        private void CreatePropertyEvent(string failedProperty, string elemType, string elemName, oM.Reflection.Debugging.EventType eventType)
        {
            Engine.Reflection.Compute.RecordEvent("Failed to set property " + failedProperty + " for the " + elemType + "with id: " + elemName, eventType);
        }

        /***************************************************/

    }
}
