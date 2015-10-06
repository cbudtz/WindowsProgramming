using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public void save()
        {
            //TODO: implement save of whole stack

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
