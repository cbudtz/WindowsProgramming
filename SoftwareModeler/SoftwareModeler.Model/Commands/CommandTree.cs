using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CommandTree : INotifyPropertyChanged
    {
        public string Name;
        public BaseCommand root;
        public BaseCommand active;
        public List<BaseCommand> undone { get; set; } = new List<BaseCommand>();
        public int NextShapeId { get; set; }
        public ObservableCollection<BaseCommand> commands { get; set; } = new ObservableCollection<BaseCommand>();

        //TODO: implement
        public event PropertyChangedEventHandler PropertyChanged;

        public void addAndExecute(BaseCommand command)
        {
            if (root == null)
            {
                //Root node - serialization starts here...
                root = command;
                setActive(root);
            }
            else
            {
                
                //Add child to tree
                command.Parent = active;
                active.addChild(command);
                setActive(command);
            }
            commands.Add(command);
            foreach (BaseCommand baseCommand in commands)
            {
                Console.WriteLine(baseCommand.id + baseCommand.color.ToString() + baseCommand.BranchLayer);
            }
            //ececute new command
            active.execute();
            
        }

        private void setActive(BaseCommand root)
        {
            
            if (active !=null)active.color = Colors.Transparent;
            active = root;
            active.color = Colors.Aquamarine;
        }

        public void setActiveCommand(BaseCommand command)
        {
            //Update activeCommand
            setActive(command);
            reExecute();
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

        public static void save(CommandTree commandTree)
        {
            save(commandTree, new StreamWriter(@"output.xml"));
        }

        public static CommandTree load(StreamReader loadReader)
        {
            //Empty ShapeCollector Singleton
            ShapeCollector.getI().reset();
            //restore Tree
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            CommandTree restoredTree;
            using (StreamReader reader = loadReader)
                restoredTree = serializer.Deserialize(reader) as CommandTree;
            //Make sure that newly Added Shapes get a new ID
            Shape.nextId = restoredTree.NextShapeId;
            //Reestablishing parents and finding active node
            restoredTree.setActive(CommandTree.reParseTree(restoredTree.root, restoredTree.active.id));
            //Moving diagram to active state
            restoredTree.reExecute();

            return restoredTree;

        }

        public static CommandTree load()
        {
            return load(new StreamReader(@"output.xml"));
        }


        public void reExecute()
        {
            
            //Remove all objects from canvas
            ShapeCollector.getI().reset();
            //Execute all commands on branch to active node.
            LinkedList<BaseCommand> reExecuteList = new LinkedList<BaseCommand>();
            BaseCommand curCommand = active;
            while (curCommand != null)
            {
                reExecuteList.AddFirst(curCommand);
                curCommand = curCommand.Parent;
            }
            Console.WriteLine("ReExecuting " + reExecuteList.Count + " Commands");
            foreach (BaseCommand b in reExecuteList)
            {
                b.execute();
                Console.WriteLine("reExecuted " + b.id);
            }


        }

        private static BaseCommand reParseTree(BaseCommand node, int id)
        {
            BaseCommand activeNode = null;
         
            if(node.id == id)
            {
                activeNode = node;
            }
            if(!node.Children.Equals(null) && node.Children.Count > 0)
            {
               
                foreach (BaseCommand child in node.Children)
                {
                    child.Parent = node;
                    BaseCommand recNode = CommandTree.reParseTree(child, id);
                    if (recNode != null) activeNode = recNode;

                }
            }
            return activeNode;
            
        }

        public void undo()
        {
            if(active == root)
            {
                
                return;
            }
            active.unExecute();
            undone.Add(active);
            setActive(active.Parent);
            
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
