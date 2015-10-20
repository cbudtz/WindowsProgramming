using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using Area51.SoftwareModeler.Model;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CommandTree : INotifyPropertyChanged
    {
        public string Name;
        public BaseCommand root;
        public BaseCommand active;
        public List<BaseCommand> undone;
        //TODO implement
        public event PropertyChangedEventHandler PropertyChanged;

        public void addAndExecute(BaseCommand command)
        {
            if (root == null)
            {
                root = command;
                active = root;
            }
            else
            {
                command.Parent = active;
                active.addChild(command);
                active = command;
            }
            active.execute();
            
        }

        public void setActive(BaseCommand command)
        {
            //TODO: Implement recursive function to crawl up and down tree;
        }

        public static void save(CommandTree commandTree)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (StreamWriter writer = new StreamWriter(@"output.xml"))
                serializer.Serialize(writer, commandTree);
        }
        public static CommandTree load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            CommandTree restoredTree;
            using (StreamReader reader = new StreamReader(@"output.xml"))
                restoredTree = serializer.Deserialize(reader) as CommandTree;
            restoredTree.active = CommandTree.reParseTree(restoredTree.root, restoredTree.active.id);
            restoredTree.reExecute();
            return restoredTree;

        }

        private void reExecute()
        {
            ShapeCollector.reset();
            LinkedList<BaseCommand> reExecuteList = new LinkedList<BaseCommand>();
            BaseCommand curCommand = active;
            while (curCommand.Parent != null)
            {
                reExecuteList.AddFirst(curCommand);
                curCommand = curCommand.Parent;
            }
            foreach (BaseCommand b in reExecuteList)
            {
                b.execute();
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
            //TODO: Implemnt
        }

        public void redo()
        {
            //TODO:Implement - uses undone list
        }

    }
}
