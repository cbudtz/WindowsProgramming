using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
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

        public void addCommand(BaseCommand command)
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
            restoredTree.active = CommandTree.findActive(restoredTree.root, restoredTree.active.id);
            return restoredTree;

        }

        private static BaseCommand findActive(BaseCommand node, int id)
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
                    BaseCommand recNode = CommandTree.findActive(child, id);
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
