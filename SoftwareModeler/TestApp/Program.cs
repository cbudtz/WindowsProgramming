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
            cTree.AddAndExecute(command);
            BaseCommand command2 = new AddClassCommand("Test ClassData","Test StereoType",false, new Point())  ;
            cTree.AddAndExecute(command2);
            ClassView newClassView = ((AddClassCommand) command2).ClassDataRep;

            BaseCommand command3 = new DummyCommand();
            cTree.AddAndExecute(command3);
            BaseCommand command4 = new AddClassCommand();
            cTree.AddAndExecute(command4);
            BaseCommand command5 = new MoveShapeCommand(((AddClassCommand)command2).ClassDataRep, 3.5,4.5  );
            cTree.AddAndExecute(command5);





            cTree.Name = "Fancy Name";
           // command.
            CommandTree.Save(cTree);
            Console.WriteLine("ClassView.NextId: " + ClassData.nextId);
            Console.WriteLine("Serialized CommandTree - now trying to restore");
            Console.WriteLine("ShapeCollector has shapes: " + ShapeCollector.GetI().ObsShapes.Count);
            Console.ReadKey();
            CommandTree commandTreeCopy = CommandTree.Load();
            Console.WriteLine("no cmd: " + (cTree.Active.Id + 1));
            Console.WriteLine("active id: " + commandTreeCopy.Active.Id);
            Console.WriteLine("root id: " + commandTreeCopy.Root.Id);
            Console.WriteLine("root child id: " + commandTreeCopy.Root.Children.ElementAt(0).Id);
            Console.WriteLine("root child child id: " + commandTreeCopy.Root.Children.ElementAt(0).Children.ElementAt(0).Id);
            Console.WriteLine("active parent id: " + commandTreeCopy.Active.Parent.Id);
            Console.WriteLine("active parent parent id: " + commandTreeCopy.Active.Parent.Parent.Id);
            Console.WriteLine("nextShapeID: " + ClassData.nextId);
            Console.WriteLine("Shapes In shapeCollector: " + ShapeCollector.GetI().ObsShapes.Count);


            Console.ReadKey();

            
        }
    }
}
