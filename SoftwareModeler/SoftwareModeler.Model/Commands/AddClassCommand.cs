using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Area51.SoftwareModeler.Model;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddClassCommand : BaseCommand
    {
        [XmlIgnore]
        public Shape shapeToAdd {get; set; }
        public int shapeID;
        [XmlIgnore]
        public ObservableCollection<Shape> shapes {get; set;}
        public AddClassCommand()
        {

        }

        public override void execute()
        {
            Class c = new Class();
            ShapeCollector.getI().obsShapes.Add(c);
            shapeToAdd = c;
        }


        public override void unExecute()
        {
            ShapeCollector.getI().obsShapes.Remove(shapeToAdd);
        }


    }
}
