using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CommandTree : NotifyBase
    {
        public string Name;
        public int CurrentBranchLayer;
        public BaseCommand Root { get; set; }
        public BaseCommand Active { get; set; }
        public List<BaseCommand> undone { get; set; } = new List<BaseCommand>();
        public int NextShapeId { get; set; }
        public int NextCommandId { get; set; }
        public ObservableCollection<BaseCommand> Commands { get; set; } = ShapeCollector.GetI().Commands;
        public ObservableCollection<Connection> CommandConnections { get; set; } = ShapeCollector.GetI().treeArrows;

        public CommandTree()
        {
            BaseCommand.nextid = 0;
        }

        public void addAndExecute(BaseCommand command)
        {
            if (Root == null)
            {
                //Root node - serialization starts here...
                Root = command;
                setActive(Root);
            }
            else
            {
                //Add child to tree
                command.Parent = Active;
                var newLayer = Active.addChild(command, CurrentBranchLayer);
                if (newLayer > CurrentBranchLayer) CurrentBranchLayer = newLayer;
                setActive(command);
            }
            //if a parentnode exist, and the current branchlayer is greater than the parents.
            if (Active.Parent != null && Active.Parent.BranchLayer < Active.BranchLayer)
            {
                //Update scroll area size.
                ShapeCollector.GetI().MaxBranchLayer.Add(CurrentBranchLayer);
                NotifyPropertyChanged(() => ShapeCollector.GetI().MaxBranchLayer);

                //Draw new arrow, from parent to child.
                ShapeCollector.GetI().treeArrows.Add(new Connection(Active.Parent, Active));
            }
                

            ShapeCollector.GetI().Commands.Add(command);
            NotifyPropertyChanged(() => ShapeCollector.GetI().Commands);

            //       foreach (BaseCommand baseCommand in ShapeCollector.getI().commands)
            //     {
            //       Console.WriteLine(baseCommand.Id + baseCommand.Color.ToString() + baseCommand.BranchLayer);
            // }
            //ececute new command
            Active.execute();

        }

        private void setActive(BaseCommand node)
        {
            Console.WriteLine("set color: " + node);
            if (Active != null)
            {
                Console.WriteLine("set color to inactive: " + Active.Id);
                Active.Color = new SolidColorBrush(Colors.Azure);
            }
            Active = node;
            if (Active != null)
            {
                Console.WriteLine("set color to active: " + Active.Id);
                Active.Color = new SolidColorBrush(Colors.DarkBlue);
            }
        }

        public void setActiveCommand(BaseCommand command)
        {
            //Update activeCommand
            BaseCommand newActiveNode = reParseTree(Root, command.Id);
            setActive(newActiveNode);
            reExecute();
        }

        public void reExecute()
        {
            //Remove all objects from canvas
            ShapeCollector.GetI().Reset();
            //Execute all Commands on branch to active node.
            LinkedList<BaseCommand> reExecuteList = new LinkedList<BaseCommand>();
            BaseCommand curCommand = Active;
            while (curCommand != null)
            {
                reExecuteList.AddFirst(curCommand);
                curCommand = curCommand.Parent;
            }
            //Console.WriteLine("ReExecuting " + reExecuteList.Count + " Commands");
            foreach (BaseCommand b in reExecuteList)
            {
                b.execute();
                //Console.WriteLine("reExecuted " + b.Id);
            }

        }

        private static BaseCommand reParseTree(BaseCommand node, int id)
        {
            //Console.WriteLine("looking for: " + id);
            //Console.WriteLine("Looking at:" + (node == null ? "null" : ""+node.Id));
            BaseCommand activeNode = null;
            if (node == null) return null;

            if (node.Id == id)
            {
                //Console.WriteLine("Found active node");
                activeNode = node;
            }
            if (!node.Children.Equals(null) && node.Children.Count > 0)
            {

                foreach (BaseCommand child in node.Children)
                {
                    child.Parent = node;
                    BaseCommand recNode = CommandTree.reParseTree(child, id);
                    if (recNode != null) activeNode = recNode;

                }
            }
            //Console.WriteLine("ReparseTree - Found activeNode: " + activeNode);
            return activeNode;

        }

        public static void save(CommandTree commandTree)
        {
            save(commandTree, new StreamWriter(@"output.xml"));
        }

        public static async void asyncSave(CommandTree commandTree, FileStream fileStream)
        {
            await Task.Run(() => save(commandTree, fileStream));
        }

        public static async void asyncSave(CommandTree commandTree)
        {
            await Task.Run(() => save(commandTree));
        }

        public static void save(CommandTree commandTree, StreamWriter saveWriter)
        {

            //Making sure that new shapes will get a new ID when deSerializing
            commandTree.NextShapeId = Shape.nextId;
            //Serialize CommandTree TODO: Add FileSelectBox
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (StreamWriter writer = saveWriter)
                serializer.Serialize(writer, commandTree);
        }

        public static void save(CommandTree commandTree, FileStream fileStream)
        {

            //Making sure that new shapes will get a new ID when deSerializing
            commandTree.NextShapeId = Shape.nextId;
            commandTree.NextCommandId = BaseCommand.nextid;
            //Serialize CommandTree TODO: Add FileSelectBox
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
            {
                serializer.Serialize(writer, commandTree);
            }
            fileStream.Close();
        }

        public static CommandTree load()
        {
            return load(new StreamReader(@"output.xml"));
        }

        public static CommandTree load(StreamReader loadReader)
        {
            //Empty ShapeCollector Singleton
            ShapeCollector.GetI().Reset();
            ShapeCollector.GetI().Commands.Clear();
            ShapeCollector.GetI().treeArrows.Clear();
            //restore Tree
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            CommandTree restoredTree;
            using (StreamReader reader = loadReader)
                restoredTree = serializer.Deserialize(reader) as CommandTree;
            //Make sure that newly Added Shapes get a new ID
            Shape.nextId = restoredTree.NextShapeId;
            BaseCommand.nextid = restoredTree.NextCommandId;
            


            Console.WriteLine("Load: Active node:" + restoredTree.Active.Id);
            Console.WriteLine("Load: RootNode: " + restoredTree.Root);
            //Reestablishing parents and finding active node
            restoredTree.setActive(CommandTree.reParseTree(restoredTree.Root, restoredTree.Active.Id));
            ShapeCollector.GetI().Commands = restoredTree.Commands;
            ShapeCollector.GetI().treeArrows = restoredTree.CommandConnections;
            //Moving diagram to active state
            restoredTree.reExecute();

            return restoredTree;
        }

        public void undo()
        {
            if (Active == Root)
            {

                return;
            }
            Active.unExecute();
            undone.Add(Active);
            setActive(Active.Parent);

        }

        public void redo()
        {
            if (undone == null || undone.Count == 0) return;
            BaseCommand reDoCommand = undone.Last();
            reDoCommand.execute();
            undone.Remove(reDoCommand);
            setActive(reDoCommand);
        }

    }
}
