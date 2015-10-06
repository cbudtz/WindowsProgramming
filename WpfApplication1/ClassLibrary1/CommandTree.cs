using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
namespace ModelLibrary
{
    public class CommandTree : INotifyPropertyChanged
    {
        ICommand root;
        ICommand active;
        LinkedList<ICommand> undone;
        //TODO implement
        public event PropertyChangedEventHandler PropertyChanged;

        public void setActive(ICommand command)
        {
            //TODO: Implement recursive function to crawl up and down tree;
        }

        public static void save(CommandTree commandTree)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (StreamWriter writer = new StreamWriter(@"c:\temp\output.xml"))
                serializer.Serialize(writer, commandTree);
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
