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
        public double XOffset;
        public double YOffset;

        public MoveShapeCommand()
        {
            //Deserialization constructor...
        }

        public MoveShapeCommand(Shape shape, double xOffset, double yOffset)
        {
            //When first constructed
            ShapeId = shape.id;
            XOffset = xOffset;
            YOffset = yOffset;
        }


        public override void execute()
        {
            
            Shape shape = ShapeCollector.GetI().GetShapeById(ShapeId);
            
            shape.CanvasCenterX += XOffset;
            shape.CanvasCenterY += YOffset;
            foreach (Connection conn in ShapeCollector.GetI().ObsConnections)
            {
                if (conn.EndShapeId == ShapeId || conn.StartShapeId == ShapeId) conn.updatePoints();

            }
        }

        public override void unExecute()
        {
            Shape shape = ShapeCollector.GetI().GetShapeById(ShapeId);
            shape.CanvasCenterX -= XOffset;
            shape.CanvasCenterY -= YOffset;
            foreach (Connection conn in ShapeCollector.GetI().ObsConnections)
            {
                if (conn.EndShapeId == ShapeId || conn.StartShapeId == ShapeId) conn.updatePoints();

            }
        }

    }
}

