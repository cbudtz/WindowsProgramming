using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
namespace Area51.SoftwareModeler.Models
{
    public class CommandTree : INotifyPropertyChanged
    {
        public string Name;
        ICommandExt root;
        ICommandExt active;
        LinkedList<ICommandExt> undone;
        //TODO implement
        public event PropertyChangedEventHandler PropertyChanged;

        public void addCommand(ICommandExt command)
        {
            if (root == null)
            {
                root = command;
            }
            else active.addChild(command);
            
        }

        public void setActive(ICommandExt command)
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
            using (StreamReader reader = new StreamReader(@"c:\temp\output.xml"))
                return serializer.Deserialize(reader) as CommandTree;

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
