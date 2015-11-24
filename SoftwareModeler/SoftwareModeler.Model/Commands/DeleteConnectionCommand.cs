using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;

namespace Area51.SoftwareModeler.Models.Commands
{
    class DeleteConnectionCommand : BaseCommand
    {
        public Connection ConnectionToDelete { get; set; }
       
        public DeleteConnectionCommand(Connection _connectionToDelete)
        {
            ConnectionToDelete = _connectionToDelete;
        }
        public override void execute()
        {
            foreach (Connection obsConnection in ShapeCollector.getI().obsConnections)
            {
                if (obsConnection != null && obsConnection.connectionID == ConnectionToDelete.connectionID)
                    ShapeCollector.getI().obsConnections.Remove(obsConnection);
            }
            ShapeCollector.getI().obsConnections.Remove(ConnectionToDelete);
        }

        public override void unExecute()
        {
            ShapeCollector.getI().obsConnections.Add(ConnectionToDelete);
        }
    }
}
