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
            ShapeId = _shape.id;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }


        public override void execute()
        {
            
                Shape shape = ShapeCollector.getI().getShapeByID(ShapeId);
            
            Console.WriteLine("Executing moveShapeCommand");
            Console.WriteLine("Shape =" + shape + "Center : " + shape.CanvasCenterX + " ," + shape.CanvasCenterY);
            shape.CanvasCenterX += xOffset;
            shape.CanvasCenterY += yOffset;
            Console.WriteLine("Shape =" + shape + "Center : " + shape.CanvasCenterX + " ," + shape.CanvasCenterY);
        }

        public override void unExecute()
        {
            Shape shape = ShapeCollector.getI().getShapeByID(ShapeId);
            shape.CanvasCenterX -= xOffset;
            shape.CanvasCenterY -= yOffset;
        }

    }
}

