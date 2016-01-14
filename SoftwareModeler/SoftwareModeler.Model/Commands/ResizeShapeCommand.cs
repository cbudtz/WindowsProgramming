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
        public int? ShapeId { get; set; }
        public double xResize;
        public double YResize;

        public ResizeShapeCommand()
        {
            //Deserialization constructor...
        }

        public ResizeShapeCommand(ClassView _shape, double _xResize, double _yResize)
        {
            //When first constructed
            ShapeId = _shape.id;
            xResize = _xResize;
            YResize = _yResize;
        }


        public override void execute()
        {
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);

            Console.WriteLine("Executing ResizeShapeCommand");
            Console.WriteLine("ClassView =" + classView);
            classView.Width += xResize;
            classView.Height += YResize;
        }

        public override void unExecute()
        {
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);
            classView.Width -= xResize;
            classView.Height -= YResize;
        }

    }
}

