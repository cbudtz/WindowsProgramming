using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Test");
            Console.ReadKey();
            CommandTree cTree = new CommandTree();
            BaseCommand command = new DummyCommand();
            BaseCommand command2 = new DummyCommand();
            BaseCommand command3 = new DummyCommand();
            //command2.Parent = command;
            //command2.addChild(command3);

            cTree.addCommand(command);
            cTree.addCommand(command2);
            cTree.addCommand(command3);
            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            CommandTree commandTreeCopy = CommandTree.load();
            //Console.Write(commandTreeCopy);
            
        }
    }
}
