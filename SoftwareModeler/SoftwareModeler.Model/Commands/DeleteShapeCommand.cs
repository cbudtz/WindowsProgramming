using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class DeleteShapeCommand : BaseCommand
    {
        public int? ShapeId;
        private List<Connection> _connectionsToDelete = new List<Connection>();
        public string ShapeName { get; set; }
        public double ShapeX { get; set; }
        public double ShapeY { get; set; }
        public double Shapewidth { get; set; }
        public double Shapeheight { get; set; }
        public bool ShapeSelected { get; set; }
        public string ShapeStereotype { get; set; }
        public bool ShapeAbstract { get; set; }
        public Visibility ShapeVisibility { get; set; }

        public DeleteShapeCommand()
        {
            //Deserialization Constructor.
        }
        
        public DeleteShapeCommand(Shape shapeToDelete)
        {
            ShapeId = shapeToDelete.id;
            ShapeName = shapeToDelete.name;
            ShapeX = shapeToDelete.X;
            ShapeY = shapeToDelete.Y;
            Shapewidth = shapeToDelete.Width;
            Shapeheight = shapeToDelete.Height;
            ShapeSelected = shapeToDelete.IsSelected;
            ShapeStereotype = (shapeToDelete as Class).StereoType;
            ShapeAbstract = (shapeToDelete as Class).IsAbstract;
            
            // if x,y of either of connection endpoint shapes, remove connection // TODO alternatively use id

        }



        public override void execute()
        {
            //Aquire shape
            Class shapeToDelete = ShapeCollector.GetI().GetShapeById(ShapeId);
            _connectionsToDelete =
                ShapeCollector.GetI()
                    .ObsConnections.Where(x => x.StartShapeId == ShapeId || x.EndShapeId == ShapeId)
                    .ToList();

            foreach (Connection connection in _connectionsToDelete)
            {
                ShapeCollector.GetI().ObsConnections.Remove(connection);

            }
            _connectionsToDelete.ForEach(x => ShapeCollector.GetI().ObsConnections.Remove(x));
            ShapeCollector.GetI().ObsShapes.Remove(shapeToDelete);
        }

        public override void unExecute()
        {
            Class shapeToAdd = new Class(ShapeId,ShapeName,ShapeStereotype,ShapeAbstract,new Point(ShapeX,ShapeY));
            //Create new shape and add it!
            ShapeCollector.GetI().ObsShapes.Add(shapeToAdd);
            _connectionsToDelete.ForEach(x => ShapeCollector.GetI().ObsConnections.Add(x));
        }
    }
}
