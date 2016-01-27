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
        public double XResize;
        public double YResize;

        public ResizeShapeCommand()
        {
            //Deserialization constructor...
        }

        public ResizeShapeCommand(ClassView shape, double xResize, double yResize)
        {
            //When first constructed
            ShapeId = shape.id;
            XResize = xResize;
            YResize = yResize;
        }


        public override void execute()
        {
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);

            Console.WriteLine("Executing ResizeShapeCommand");
            Console.WriteLine("ClassView =" + classView);
            classView.Width += XResize;
            classView.Height += YResize;
        }

        public override void unExecute()
        {
            ClassView classView = ShapeCollector.GetI().GetShapeById(ShapeId);
            if (classView == null) classView = ShapeCollector.GetI().GetCommentById(ShapeId);
            classView.Width -= XResize;
            classView.Height -= YResize;
        }

        public override string CommandName => "Resize Class";

        public override string Info
        {
            get
            {
                ClassData c = ShapeCollector.GetI().GetShapeById(ShapeId);
                if (c == null) return InfoBackup;
                return InfoBackup = "Class: " + c.Name +
                                    "\nOffset X: " + XResize +
                                    "\nOffset Y: " + YResize;
            }
        }
    }
}

