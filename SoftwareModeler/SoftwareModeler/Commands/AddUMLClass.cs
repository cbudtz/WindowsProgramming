using Area51.SoftwareModeler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Commands
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



    }
}
