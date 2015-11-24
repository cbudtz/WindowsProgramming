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
    class DeleteShapeCommand : BaseCommand
    {
        public int? shapeID;
        private List<Connection> connectionsToDelete = new List<Connection>();
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
            shapeID = shapeToDelete.id;
            ShapeName = shapeToDelete.name;
            ShapeX = shapeToDelete.X;
            ShapeY = shapeToDelete.Y;
            Shapewidth = shapeToDelete.Width;
            Shapeheight = shapeToDelete.Height;
            ShapeSelected = shapeToDelete.IsSelected;
            ShapeStereotype = (shapeToDelete as Class).StereoType;
            ShapeAbstract = (shapeToDelete as Class).IsAbstract;
            ShapeVisibility = (shapeToDelete as Class).Visibility;
            // if x,y of either of connection endpoint shapes, remove connection // TODO alternatively use id

        }



        public override void execute()
        {
            //Aquire shape
            Shape shapeToDelete = ShapeCollector.getI().getShapeByID(shapeID);
            connectionsToDelete =
                ShapeCollector.getI()
                    .obsConnections.Where(x => x.startShapeID == shapeID || x.endShapeID == shapeID)
                    .ToList();

            foreach (Connection connection in connectionsToDelete)
            {
                ShapeCollector.getI().obsConnections.Remove(connection);

            }
            connectionsToDelete.ForEach(x => ShapeCollector.getI().obsConnections.Remove(x));
            ShapeCollector.getI().obsShapes.Remove(shapeToDelete);
        }

        public override void unExecute()
        {
            Shape shapeToAdd = new Class(shapeID,ShapeName,ShapeStereotype,ShapeAbstract,new Point(ShapeX,ShapeY),ShapeVisibility );
            //Create new shape and add it!
            ShapeCollector.getI().obsShapes.Add(shapeToAdd);
            connectionsToDelete.ForEach(x => ShapeCollector.getI().obsConnections.Add(x));
        }
    }
}
