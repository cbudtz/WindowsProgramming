using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Models.Commands
{
    class AddUMLClassCommand
    {
        private ObservableCollection<Class> classes;

        private Class repClass;

        public AddUMLClassCommand(ObservableCollection<Class> _classes, Class _class)
        {
            classes = _classes;
            repClass = _class;
        }

        // For doing and redoing the command.
        public void Execute()
        {
            classes.Add(repClass);
        }

        // For undoing the command.
        public void UnExecute()
        {
            classes.Remove(repClass);
        }

    }
}
