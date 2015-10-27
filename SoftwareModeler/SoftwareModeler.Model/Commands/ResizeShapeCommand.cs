using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class ResizeShapeCommand : BaseCommand
    {
        [XmlIgnore]
        private Shape shape;
        public int? ShapeId { get; set; }
        public double xResize;
        public double YResize;

        public ResizeShapeCommand()
        {
            //Deserialization constructor...
        }

        public ResizeShapeCommand(Shape _shape, double _xResize, double _yResize)
        {
            //When first constructed
            shape = _shape;
            ShapeId = _shape.id;
            xResize = _xResize;
            YResize = _yResize;
        }


        public override void execute()
        {
            if (shape == null) //If deserialized
            {
                shape = ShapeCollector.getI().getShapeByID(ShapeId);
            }
            Console.WriteLine("Executing ResizeShapeCommand");
            Console.WriteLine("Shape =" + shape);
            shape.Width += xResize;
            shape.Height += YResize;
        }

        public override void unExecute()
        {
            shape.Width -= xResize;
            shape.Height -= YResize;
        }

    }
}

