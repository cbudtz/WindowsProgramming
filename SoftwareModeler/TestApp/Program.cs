﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");
            Console.ReadKey();
            CommandTree cTree = new CommandTree();
            BaseCommand command = new DummyCommand();
            BaseCommand command2 = new AddShapeCommand();
            BaseCommand command3 = new DummyCommand();
            BaseCommand command4 = new MoveShapeCommand();
            //command2.Parent = command;
            //command2.addChild(command3);

            cTree.addCommand(command);
            cTree.addCommand(command2);
            cTree.addCommand(command3);
            cTree.addCommand(command4);
            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            CommandTree commandTreeCopy = CommandTree.load();
            Console.WriteLine("no cmd: " + (cTree.active.id + 1));
            Console.WriteLine("active id: " + commandTreeCopy.active.id);
            Console.WriteLine("root id: " + commandTreeCopy.root.id);
            Console.WriteLine("root child id: " + commandTreeCopy.root.Children.ElementAt(0).id);
            Console.WriteLine("root child child id: " + commandTreeCopy.root.Children.ElementAt(0).Children.ElementAt(0).id);
            Console.WriteLine("active parent id: " + commandTreeCopy.active.Parent.id);
            Console.WriteLine("active parent parent id: " + commandTreeCopy.active.Parent.Parent.id);

            Console.ReadKey();

            
        }
    }
}
