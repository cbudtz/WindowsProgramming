using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models
{
    public class MoveShapeCommand : ICommandExt
    {
        private Shape shape;
        private double xOffset;
        private double yOffset;

        private ICommandExt parent;
        private List<ICommandExt> children;
    
        public override ICommandExt Parent { get { return parent; } }
        public override List<ICommandExt> Children { get { return children; } }


        public MoveShapeCommand(ICommandExt _parent, Shape _shape, double _xOffset, double _yOffset)
        {
            parent = _parent;
            children = new List<ICommandExt>();
            shape = _shape;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }
        public override void addChild(ICommandExt child)
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

        public override bool canExecute()
        {
            throw new NotImplementedException();
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

