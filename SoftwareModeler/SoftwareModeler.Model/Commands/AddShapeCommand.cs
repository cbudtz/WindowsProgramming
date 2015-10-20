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

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddShapeCommand : BaseCommand
    {
        [XmlIgnore]
        public Shape shapeToAdd {get; set; }
        public int shapeID;
        [XmlIgnore]
        public ObservableCollection<Shape> shapes {get; set;}
        public AddShapeCommand()
        {

        }
        public AddShapeCommand(Shape _shapeToAdd, ObservableCollection<Shape> _shapes)
        {
            shapeToAdd = _shapeToAdd;
            shapeID = _shapeToAdd.id;
            shapes = _shapes;
        }

        public override void execute()
        {
            shapes.Add(shapeToAdd);
        }


        public override void unExecute()
        {
            shapes.Remove(shapeToAdd);
        }


    }
}
