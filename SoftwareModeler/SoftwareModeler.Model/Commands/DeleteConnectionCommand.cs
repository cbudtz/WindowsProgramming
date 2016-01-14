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
        [XmlIgnore]
        public ConnectionData ConnectionDataToDelete;
        public int ConnId;
        //public int? EndId;
        //public int? StartId;
        //public ConnectionType ConnType;
        //public string StartMult;
        //public string EndMult;

        public DeleteConnectionCommand()
        {
            // serializing constructor
        }
       
        public DeleteConnectionCommand(ConnectionData connToDel)
        {
            ConnectionDataToDelete = connToDel;     
            ConnId = connToDel.ConnectionId;
            //EndId = connToDel.EndShapeId;
            //StartId = connToDel.StartShapeId;
            //ConnType = connToDel.Type;
            //StartMult = connToDel.StartMultiplicity;
            //EndMult = connToDel.EndMultiplicity;
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
    }
}
