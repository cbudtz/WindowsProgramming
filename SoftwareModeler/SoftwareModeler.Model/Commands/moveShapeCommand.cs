using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class MoveShapeCommand : BaseCommand
    {
        private Shape shape;
        private double xOffset;
        private double yOffset;

        public MoveShapeCommand()
        {

        }

        public MoveShapeCommand(Shape _shape, double _xOffset, double _yOffset)
        {
            shape = _shape;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }


        public override void execute()
        {
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

