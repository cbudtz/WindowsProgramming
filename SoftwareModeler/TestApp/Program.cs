using System;
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
            //command2.Parent = command;
            //command2.addChild(command3);

            cTree.addCommand(command);
            cTree.addCommand(command2);
            cTree.addCommand(command3);
            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            Console.WriteLine(cTree.active.Parent.id);
            CommandTree commandTreeCopy = CommandTree.load();
            Console.WriteLine(commandTreeCopy.active.id);
            Console.WriteLine(commandTreeCopy.root.id);
            Console.WriteLine(commandTreeCopy.root.Children.ElementAt(0).id);
            Console.WriteLine(commandTreeCopy.active.Parent.id);
            Console.WriteLine(commandTreeCopy.active.Parent.Parent.id);

            Console.ReadKey();

            
        }
    }
}
