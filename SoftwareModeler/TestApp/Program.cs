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
            DummyCommand command = new DummyCommand();
            cTree.addCommand(command);
            cTree.Name = "Fancy Name";
            CommandTree.save(cTree);
            
        }
    }
}
