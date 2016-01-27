using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class DeleteConnectionCommand : BaseCommand
    {
        [XmlIgnore] public ConnectionData ConnectionDataToDelete;
        public int ConnId;

        public DeleteConnectionCommand()
        {
            // serializing constructor
        }
       
        public DeleteConnectionCommand(ConnectionData connToDel)
        {
            ConnectionDataToDelete = connToDel;     
            ConnId = connToDel.ConnectionId;
        }
        public override void execute()
        {
            if (ConnectionDataToDelete == null)
                ConnectionDataToDelete = ShapeCollector.GetI().GetConnectionById(ConnId);
             
            Console.WriteLine("deleting connection: " + ConnId);   
            ShapeCollector.GetI().ObsConnections.Remove(ConnectionDataToDelete);
        }

        public override void unExecute()
        {
            
            ShapeCollector.GetI().ObsConnections.Add(ConnectionDataToDelete);
        }

        public override string CommandName => "Delete Connection";

        public override string Info
        {

            get {
                ConnectionData c = ShapeCollector.GetI().GetConnectionById(ConnId)?? ConnectionDataToDelete;
                string type = c.Type.ToString();
                string fromName = ShapeCollector.GetI().GetShapeById(ConnectionDataToDelete.StartShapeId)?.Name;
                string toName = ShapeCollector.GetI().GetShapeById(ConnectionDataToDelete.EndShapeId)?.Name;
                
                return InfoBackup = "Connection Type: " + type +
                                    "\nFrom: " + fromName +
                                    "\nTo: " + toName;
            }
        }
    }
}
