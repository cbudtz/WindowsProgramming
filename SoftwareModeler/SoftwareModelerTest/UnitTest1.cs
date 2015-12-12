using System;
using System.Linq;
using System.Windows.Documents;
//using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Area51.SoftwareModeler.ViewModels;
using Area51.SoftwareModeler.Views;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.Generic;

namespace SoftwareModelerTest
{
    
    [TestClass]
    public class UnitTest1
    {
        static Random r = new Random();
        static ShapeCollector sc;
        static CommandTree ct;

        [TestInitialize]
        public void Setup()
        {
            sc = ShapeCollector.GetI();
            ct = new CommandTree();
        }


        [TestMethod]
        public void TestAddClassCommand()
        {
            int numClasses = r.Next(10, 20);
            int curNumClasses = sc.ObsShapes.Count;
            AddClasses(numClasses);
            Assert.AreEqual(curNumClasses+numClasses, BaseCommand.nextid, "checking id. should be at leas number of number of elements");
            Assert.AreEqual(curNumClasses+numClasses, sc.ObsShapes.Count, "checking number of classes");
        }

        [TestMethod]
        public void TestDeleteClassCommand()
        {
            AddClasses(r.Next(10,20));
            int num = sc.ObsShapes.Count;
            int del = r.Next(0, num-1);
            int numCmd = sc.Commands.Count;
            DeleteClasses(del);
            Assert.AreEqual(num-del, sc.ObsShapes.Count, "checking number of shapes");
            Assert.AreEqual(numCmd+del, sc.Commands.Count, "checking size of commands");
        }

        [TestMethod]
        public void TestEditClassCommand()
        {
            AddClasses(r.Next(10,20));
            Class s = sc.ObsShapes.ElementAt(r.Next(0, sc.ObsShapes.Count-1));
            Assert.IsNotNull(s.id, "shape id should never be null");
            int sId = s.id.Value;
            string oldName = s.name;
            string oldStereoType = s.StereoType;
            bool oldIsAbstract = s.IsAbstract;
            List<Area51.SoftwareModeler.Models.Attribute> oldAttributes = s.Attributes;
            List<Method> oldMethods = s.Methods;

            EditClassCmd(sId, oldName+"name", oldStereoType+"stereo", !oldIsAbstract, GetAttributes(), getMethods());

            Class newClass = sc.ObsShapes.First(x => x.id == sId);
            Assert.IsTrue(newClass.name.Contains("name"), "checking name");
            Assert.IsTrue(newClass.StereoType.Contains("stereo"), "checking stereotype");
            Assert.IsFalse(newClass.IsAbstract & oldIsAbstract, "checking isAbstract");
            Assert.IsTrue(oldAttributes.Count < newClass.Attributes.Count, "checking number of attributes");
            Assert.IsTrue(oldMethods.Count < newClass.Methods.Count, "checking number of methods");



        }

        [TestMethod]
        public void TestAddConnectionCommand()
        {
            if(sc.ObsShapes.Count < 5) AddClasses(10);
            int ind1 = r.Next(0, sc.ObsShapes.Count-1);
            int ind2 = r.Next(0, sc.ObsShapes.Count-1);
            if (ind2 == ind1) ind1 = (ind1+1)%sc.ObsShapes.Count;
            Class c1 = sc.ObsShapes.ElementAt(ind1);
            Class c2 = sc.ObsShapes.ElementAt(ind2);
            int curCount = sc.ObsConnections.Count;
            AddConnection(c1.id.Value, c2.id.Value);
            Assert.AreEqual(curCount+1, sc.ObsConnections.Count, "check count of connections");
            int nextId = Connection.NextId;
            Assert.IsTrue(nextId > curCount, "check id. should be at leas number of connections");

        }

        [TestMethod]
        public void TestConnectionRemoved()
        {
            if (sc.ObsShapes.Count < 5) AddClasses(10);
            int ind1 = r.Next(0, sc.ObsShapes.Count-1);
            int ind2 = r.Next(0, sc.ObsShapes.Count-1);
            if (ind2 == ind1) ind1 = (ind1 + 1) % sc.ObsShapes.Count;
            Class c1 = sc.ObsShapes.ElementAt(ind1);
            Class c2 = sc.ObsShapes.ElementAt(ind2);
            AddConnection(c1.id.Value, c2.id.Value);
            int curConnCount = sc.ObsConnections.Count;
            int curClassCount = sc.ObsShapes.Count;
            ct.addAndExecute(new DeleteShapeCommand(c1));
            Assert.IsTrue(curConnCount > sc.ObsConnections.Count, "check if connection also was removed");
            Assert.IsTrue(curClassCount > sc.ObsShapes.Count, "check if shape also was removed");
        }

        public static List<Method> getMethods()
        {
            List<Method> l = new List<Method>();
            l.Add(new Method());
            return l;
        }

        public void AddConnection(int id1, int id2)
        {
            ct.addAndExecute(new AddConnectionCommand(id1, "", id2, "", ConnectionType.Association));
        }

        public static List<Area51.SoftwareModeler.Models.Attribute> GetAttributes()
        {
            List<Area51.SoftwareModeler.Models.Attribute> l = new List<Area51.SoftwareModeler.Models.Attribute>();
            l.Add(new Area51.SoftwareModeler.Models.Attribute());
            return l;
        }
        public static void EditClassCmd(int id, string name, string stereoType, bool isAbstract, List<Area51.SoftwareModeler.Models.Attribute> attributes, List<Method> methods)
        {
            ct.addAndExecute(new UpdateClassInfoCommand(sc.ObsShapes.First(x => x.id == id), name, stereoType, isAbstract, methods, attributes));
        }
        public static void DeleteClasses(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Shape s = sc.ObsShapes.ElementAt(r.Next(0, sc.ObsShapes.Count-1));
                if(s != null) ct.addAndExecute(new DeleteShapeCommand(s));
            }
        }
        public static void AddClasses(int num)
        {
            
            for (int i = 0; i < num; i++)
            {
                BaseCommand cmd1 = getAddClassCmd();
                ct.addAndExecute(cmd1);
            }
        }
        public static BaseCommand getAddClassCmd()
        {
          return new AddClassCommand("", "", true, new System.Windows.Point(r.Next(0, 500), r.Next(0, 500)));
        }
        public static Visibility getVis()
        {
            int ind = r.Next(0, 3);
            switch (ind)
            {
                case 0:
                    return Visibility.Default;
                case 1:
                    return Visibility.Private;
                case 2:
                    return Visibility.Protected;
                default:
                    return Visibility.Public;
            }
        }
    }
}
