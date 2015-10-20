using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;

namespace Area51.SoftwareModeler.Model.Commands
{
    class DeleteShapeCommand : BaseCommand
    {
        private ObservableCollection<Shape> shapes;
        private ObservableCollection<Connection> connections;
        private Shape shapeToDelete;
        private List<Connection> connectionsToDelete = new List<Connection>();

        public DeleteShapeCommand(ObservableCollection<Shape> _shapes, ObservableCollection<Connection> _connections, Shape _shapeToDelete)
        {
            shapes = _shapes;
            connections = _connections;
            shapeToDelete = _shapeToDelete;
            // if x,y of either of connection endpoint shapes, remove connection // TODO alternatively use id
            connectionsToDelete = _connections.Where(x => (_shapeToDelete.X == x.End.X && _shapeToDelete.Y == x.End.Y) || (_shapeToDelete.X == x.Start.X && _shapeToDelete.Y == x.Start.Y)).ToList(); //TODO check if it works
        }

        public override void execute()
        {
            shapes.Remove(shapeToDelete);
            connectionsToDelete.ForEach(x => connections.Remove(x));
        }

        public override void unExecute()
        {
            shapes.Add(shapeToDelete);
            connectionsToDelete.ForEach(x => connections.Add(x));
        }
    }
}
