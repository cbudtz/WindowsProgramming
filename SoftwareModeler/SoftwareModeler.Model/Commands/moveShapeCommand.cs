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
    public class MoveShapeCommand : BaseCommand
    {
        [XmlIgnore]
        private Shape shape;
        public int? ShapeId { get; set; }
        public double xOffset;
        public double yOffset;

        public MoveShapeCommand()
        {
            //Deserialization constructor...
        }

        public MoveShapeCommand(Shape _shape, double _xOffset, double _yOffset)
        {
            //When first constructed
            shape = _shape;
            ShapeId = _shape.id;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }


        public override void execute()
        {
            if (shape == null)
            {
                shape = ShapeCollector.getI().getShapeByID(ShapeId);
            }
            Console.WriteLine("Executing moveShapeCommand");
            Console.WriteLine("Shape =" + shape);
            shape.CanvasCenterX += xOffset;
            shape.CanvasCenterY += yOffset;
        }

        public override void unExecute()
        {
            shape.CanvasCenterX -= xOffset;
            shape.CanvasCenterY -= yOffset;
        }

    }
}

