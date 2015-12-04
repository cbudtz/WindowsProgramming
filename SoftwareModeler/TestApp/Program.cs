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
            cTree.addAndExecute(command);
            BaseCommand command2 = new AddClassCommand("Test Class","Test StereoType",false, new Point())  ;
            cTree.addAndExecute(command2);
            Shape newShape = ((AddClassCommand) command2).ClassRep;

            BaseCommand command3 = new DummyCommand();
            cTree.addAndExecute(command3);
            BaseCommand command4 = new AddClassCommand();
            cTree.addAndExecute(command4);
            BaseCommand command5 = new MoveShapeCommand(((AddClassCommand)command2).ClassRep, 3.5,4.5  );
            cTree.addAndExecute(command5);





            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            Console.WriteLine("Shape.NextId: " + Shape.nextId);
            Console.WriteLine("Serialized CommandTree - now trying to restore");
            Console.WriteLine("ShapeCollector has shapes: " + ShapeCollector.GetI().ObsShapes.Count);
            Console.ReadKey();
            CommandTree commandTreeCopy = CommandTree.load();
            Console.WriteLine("no cmd: " + (cTree.Active.Id + 1));
            Console.WriteLine("active id: " + commandTreeCopy.Active.Id);
            Console.WriteLine("root id: " + commandTreeCopy.Root.Id);
            Console.WriteLine("root child id: " + commandTreeCopy.Root.Children.ElementAt(0).Id);
            Console.WriteLine("root child child id: " + commandTreeCopy.Root.Children.ElementAt(0).Children.ElementAt(0).Id);
            Console.WriteLine("active parent id: " + commandTreeCopy.Active.Parent.Id);
            Console.WriteLine("active parent parent id: " + commandTreeCopy.Active.Parent.Parent.Id);
            Console.WriteLine("nextShapeID: " + Shape.nextId);
            Console.WriteLine("Shapes In shapeCollector: " + ShapeCollector.GetI().ObsShapes.Count);


            Console.ReadKey();

            
        }
    }
}
