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
        public double shapeX { get; set; }
        public double shapeY { get; set; }
        public double shapewidth { get; set; }
        public double shapeheight { get; set; }
        public bool shapeSelected { get; set; }
        public string shapeStereotype { get; set; }
        public bool shapeAbstract { get; set; }
        public Visibility shapeVisibility { get; set; }

        public DeleteShapeCommand()
        {
            //Deserialization Constructor.
        }
        
        public DeleteShapeCommand(Shape shapeToDelete)
        {
            shapeID = shapeToDelete.id;
            ShapeName = shapeToDelete.name;
            shapeX = shapeToDelete.X;
            shapeY = shapeToDelete.Y;
            shapewidth = shapeToDelete.Width;
            shapeheight = shapeToDelete.Height;
            shapeSelected = shapeToDelete.IsSelected;
            shapeStereotype = (shapeToDelete as Class).StereoType;
            shapeAbstract = (shapeToDelete as Class).IsAbstract;
            shapeVisibility = (shapeToDelete as Class).Visibility;
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
            Shape shapeToAdd = new Class(shapeID,ShapeName,shapeStereotype,shapeAbstract,new Point(shapeX,shapeY),shapeVisibility );
            //Create new shape and add it!
            ShapeCollector.getI().obsShapes.Add(shapeToAdd);
            connectionsToDelete.ForEach(x => ShapeCollector.getI().obsConnections.Add(x));
        }
    }
}
