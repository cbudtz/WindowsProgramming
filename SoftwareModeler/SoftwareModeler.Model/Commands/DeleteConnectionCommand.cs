using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Model.Commands;
using Area51.SoftwareModeler.Model;
using System.Collections.ObjectModel;

namespace Area51.SoftwareModeler.Model.Commands
{
    class DeleteConnectionCommand : BaseCommand
    {
        private Connection connectionToDelete;
        private ObservableCollection<Connection> connections;
        public DeleteConnectionCommand(Connection _connectionToDelete, ObservableCollection<Connection> _connections)
        {
            connectionToDelete = _connectionToDelete;
            connections = _connections;
        }
        public override void execute()
        {
            connections.Remove(connectionToDelete);
        }

        public override void unExecute()
        {
            connections.Add(connectionToDelete);
        }
    }
}
