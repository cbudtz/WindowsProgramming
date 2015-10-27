using System;
using System.Linq;
using System.Windows;
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
            BaseCommand command2 = new AddClassCommand();
            BaseCommand command3 = new DummyCommand();
            //BaseCommand command4 = new MoveShapeCommand();
            //command2.Parent = command;
            //command2.addChild(command3);
            ShapeCollector.getI();
            cTree.addAndExecute(command);
            cTree.addAndExecute(command2);
            cTree.addAndExecute(command3);
           // cTree.addAndExecute(command4);
            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            Console.WriteLine("Serialized CommandTree - now trying to restore");
            Console.ReadKey();
            CommandTree commandTreeCopy = CommandTree.load();
            Console.WriteLine("no cmd: " + (cTree.active.id + 1));
            Console.WriteLine("active id: " + commandTreeCopy.active.id);
            Console.WriteLine("root id: " + commandTreeCopy.root.id);
            Console.WriteLine("root child id: " + commandTreeCopy.root.Children.ElementAt(0).id);
            Console.WriteLine("root child child id: " + commandTreeCopy.root.Children.ElementAt(0).Children.ElementAt(0).id);
            Console.WriteLine("active parent id: " + commandTreeCopy.active.Parent.id);
            Console.WriteLine("active parent parent id: " + commandTreeCopy.active.Parent.Parent.id);
            Console.WriteLine("nextShapeID: " + Class.nextId);
            Console.ReadKey();

            
        }
    }
}
