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

        public MoveShapeCommand(ClassView shape, double xOffset, double yOffset)
        {
            //When first constructed
            ShapeId = shape.id;
            XOffset = xOffset;
            YOffset = yOffset;
        }


        public override void execute()
        {

            Console.WriteLine("searching for: " + ShapeId + " count: " + ShapeCollector.GetI().ObsShapes.Count);
            
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);
            
            classView.CanvasCenterX += XOffset;
            classView.CanvasCenterY += YOffset;
            foreach (ConnectionData conn in ShapeCollector.GetI().ObsConnections)
            {
                if (conn.EndShapeId == ShapeId || conn.StartShapeId == ShapeId) conn.updatePoints();

            }
        }

        public override void unExecute()
        {
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);

            classView.CanvasCenterX -= XOffset;
            classView.CanvasCenterY -= YOffset;
            foreach (ConnectionData conn in ShapeCollector.GetI().ObsConnections)
            {
                if (conn.EndShapeId == ShapeId || conn.StartShapeId == ShapeId) conn.updatePoints();

            }
        }

    }
}

