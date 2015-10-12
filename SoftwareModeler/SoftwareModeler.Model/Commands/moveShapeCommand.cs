using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models
{
    public class MoveShapeCommand : BaseCommand
    {
        private Shape shape;
        private double xOffset;
        private double yOffset;


        public MoveShapeCommand(BaseCommand _parent, Shape _shape, double _xOffset, double _yOffset)
            : base(_parent)
        {
            
            this.parent = _parent;
            this.children = new List<BaseCommand>();
            shape = _shape;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }
        public override void addChild(BaseCommand child)
        {
            children.Add(child);
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

        public override XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

