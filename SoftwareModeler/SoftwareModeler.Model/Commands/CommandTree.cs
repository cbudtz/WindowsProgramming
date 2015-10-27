using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CommandTree : INotifyPropertyChanged
    {
        public string Name;
        public BaseCommand root;
        public BaseCommand active;
        public List<BaseCommand> undone { get; set; }
        public int NextShapeId { get; set; }
        //TODO implement
        public event PropertyChangedEventHandler PropertyChanged;

        public void addAndExecute(BaseCommand command)
        {
            if (root == null)
            {
                //Root node - serialization starts here...
                root = command;
                active = root;
            }
            else
            {
                //Add child to tree
                command.Parent = active;
                active.addChild(command);
                active = command;
            }
            //excecute new command
            active.execute();
            
        }

        public void setActive(BaseCommand command)
        {
            //TODO: Implement recursive function to crawl up and down tree;
        }

        public static void save(CommandTree commandTree)
        {
            
            //Making sure that new shapes will get a new ID when deSerializing
            commandTree.NextShapeId = Shape.nextId;
            //Serialize CommandTree TODO: Add FileSelectBox
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (StreamWriter writer = new StreamWriter(@"output.xml"))
                serializer.Serialize(writer, commandTree);
        }
        public static CommandTree load()
        {
            //Empty ShapeCollector Singleton
            ShapeCollector.getI().reset();
            //restore Tree
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            CommandTree restoredTree;
            using (StreamReader reader = new StreamReader(@"output.xml"))
                restoredTree = serializer.Deserialize(reader) as CommandTree;
            //Make sure that newly Added Shapes get a new ID
            Shape.nextId = restoredTree.NextShapeId;
            //Reestablishing parents and finding active node
            restoredTree.active = CommandTree.reParseTree(restoredTree.root, restoredTree.active.id);
            //Moving diagram to active state
            restoredTree.reExecute();

            return restoredTree;

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
            active.unExecute();
            active = active.Parent;
            undone.Add(active);
        }

        public void redo()
        {
            if (undone == null || undone.Count == 0) return;
            BaseCommand reDoCommand = undone.Last();
            reDoCommand.execute();
            undone.Remove(reDoCommand);
            active = reDoCommand;
        }

    }
}
