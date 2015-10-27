using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    class DeleteShapeCommand : BaseCommand
    {
        private ObservableCollection<Shape> shapes;
        private ObservableCollection<Connection> connections;
        [XmlIgnore]
        private Shape shapeToDelete;

        public int? shapeID;
        private List<Connection> connectionsToDelete = new List<Connection>();

        public DeleteShapeCommand()
        {
            //Deserialization Constructor.
        }
        
        public DeleteShapeCommand(Shape _shapeToDelete)
        {
            shapeToDelete = _shapeToDelete;
            shapeID = shapeToDelete.id;
            shapeToDelete = _shapeToDelete;
            // if x,y of either of connection endpoint shapes, remove connection // TODO alternatively use id
            
        }

        public override void execute()
        {
            //Check if deserialized and aquire shape
            if (shapeToDelete == null) shapeToDelete = ShapeCollector.getI().getShapeByID(shapeID);
            connectionsToDelete = ShapeCollector.getI().obsConnections.Where(x => (shapeToDelete.X == x.End.X && shapeToDelete.Y == x.End.Y) || (shapeToDelete.X == x.Start.X && shapeToDelete.Y == x.Start.Y)).ToList(); //TODO check if it works
            connectionsToDelete.ForEach(x => ShapeCollector.getI().obsConnections.Remove(x));
            ShapeCollector.getI().obsShapes.Remove(shapeToDelete);
        }

        public override void unExecute()
        {
            ShapeCollector.getI().obsShapes.Add(shapeToDelete);
            connectionsToDelete.ForEach(x => ShapeCollector.getI().obsConnections.Add(x));
        }
    }
}
