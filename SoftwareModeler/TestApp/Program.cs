using System;
using System.Linq;
using System.Windows;
using Area51.SoftwareModeler.Model;
using Area51.SoftwareModeler.Model.Commands;

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
            BaseCommand command2 = new AddClassCommand("Test Class","Test StereoType",false, new Point(), Visibility.Public)  ;
            cTree.addAndExecute(command2);
            Shape newShape = ((AddClassCommand) command2).classRep;

            BaseCommand command3 = new DummyCommand();
            cTree.addAndExecute(command3);
            BaseCommand command4 = new AddClassCommand();
            cTree.addAndExecute(command4);
            BaseCommand command5 = new MoveShapeCommand(((AddClassCommand)command2).classRep, 3.5,4.5  );
            cTree.addAndExecute(command5);





            cTree.Name = "Fancy Name";
           // command.
            CommandTree.save(cTree);
            Console.WriteLine("Shape.nextID: " + Shape.nextId);
            Console.WriteLine("Serialized CommandTree - now trying to restore");
            Console.WriteLine("ShapeCollector has shapes: " + ShapeCollector.getI().obsShapes.Count);
            Console.ReadKey();
            CommandTree commandTreeCopy = CommandTree.load();
            Console.WriteLine("no cmd: " + (cTree.active.id + 1));
            Console.WriteLine("active id: " + commandTreeCopy.active.id);
            Console.WriteLine("root id: " + commandTreeCopy.root.id);
            Console.WriteLine("root child id: " + commandTreeCopy.root.Children.ElementAt(0).id);
            Console.WriteLine("root child child id: " + commandTreeCopy.root.Children.ElementAt(0).Children.ElementAt(0).id);
            Console.WriteLine("active parent id: " + commandTreeCopy.active.Parent.id);
            Console.WriteLine("active parent parent id: " + commandTreeCopy.active.Parent.Parent.id);
            Console.WriteLine("nextShapeID: " + Shape.nextId);
            Console.WriteLine("Shapes In shapeCollector: " + ShapeCollector.getI().obsShapes.Count);


            Console.ReadKey();

            
        }
    }
}
