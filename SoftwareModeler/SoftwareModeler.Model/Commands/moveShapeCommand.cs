using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public class MoveShapeCommand : ICommandExt
    {
        private Shape shape;
        private double xOffset;
        private double yOffset;

        private ICommandExt parent;
        private List<ICommandExt> children;
    
        public ICommandExt Parent { get { return parent; } }
        public List<ICommandExt> Children { get { return children; } }


        public MoveShapeCommand(ICommandExt _parent, Shape _shape, double _xOffset, double _yOffset)
        {
            parent = _parent;
            children = new List<ICommandExt>();
            shape = _shape;
            xOffset = _xOffset;
            yOffset = _yOffset;
        }
        public void addChild(ICommandExt child)
        {
            children.Add(child);
        }

        public void execute()
        {
            shape.CanvasCenterX += xOffset;
            shape.CanvasCenterY += yOffset;
        }

        public void unExecute()
        {
            shape.CanvasCenterX -= xOffset;
            shape.CanvasCenterY -= yOffset;
        }
    }
}

