using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Area51.SoftwareModeler.Models.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Area51.SoftwareModeler.ViewModels;
using Area51.SoftwareModeler.Views;

namespace SoftwareModelerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCommandId()
        {
            //BaseCommand cmd1 = new AddClassCommand("test1", "interface", true, new Point(10,10));
            //BaseCommand cmd2 = new AddClassCommand("test1", "interface", true, new Point(10, 10));
            Random r = new Random();
            int randomNum = r.Next(10, 100);
            
            for (int i = 0; i < randomNum; i++)
            {
                new AddClassCommand();
            }
            Assert.AreEqual(randomNum, BaseCommand.getNextId(), "checking id of basecommand");
        }

        [TestMethod]
        public void TestCommandTree1()
        {

            List<BaseCommand> cmd = new List<BaseCommand>();
            //new AddClassCommand("method1", "interface", false, new Point(10,10));
            Random r = new Random();

            for (int i = 0; i < r.Next(10, 100); i++)
            {
                int type = r.Next(0, )
                cmd.Add(new AddClassCommand("method" + i, "interface", false, new Point(r.Next(0,1000), r.Next(1000))));
            }

            CommandTree tree = new CommandTree();

            for (int i = 0; i < r.Next(30, 120); i++)
            {
                if (r.Next(0, 1) == 0)
                {
                    tree.addAndExecute(cmd.ElementAt(r.Next(0,cmd.Count)));
                }
            }
            tree.addAndExecute(cmd1);
        }
    }
}
