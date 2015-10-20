using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddShapeCommand : BaseCommand
    {

        private Shape shapeToAdd;
        private ObservableCollection<Shape> shapes;
        public AddShapeCommand()
        {

        }
        public AddShapeCommand(Shape _shapeToAdd, ObservableCollection<Shape> _shapes)
        {
            shapeToAdd = _shapeToAdd;
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
