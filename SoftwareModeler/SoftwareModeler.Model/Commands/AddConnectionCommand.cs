using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddConnectionCommand : BaseCommand
    {
        [XmlIgnore]
        private Connection Conn;

        public int FromId;
        public int ToId;
        public ConnectionType Type;
        public string FromMult;
        public string ToMult;


        public AddConnectionCommand()
        {
            
        }
        public AddConnectionCommand(int from, string fromMult, int to, string toMult, ConnectionType type)
        {
            FromId = from;
            ToId = to;
            Type = type;
            FromMult = fromMult;
            ToMult = toMult; 
                }
        public override void execute()
        {
            Console.WriteLine("connection added");
            if(Conn == null) Conn = new Connection(FromId, FromMult, ToId, ToMult, Type);
            ShapeCollector.GetI().ObsConnections.Add(Conn);
        }

        public override void unExecute()
        {
            Connection connToRemove = null;
            //TODO - dont remove from same collection!
            foreach (Connection obsConnection in ShapeCollector.GetI().ObsConnections.
                Where(obsConnection => Conn !=null && obsConnection.ConnectionId == Conn.ConnectionId))
            {
                connToRemove = obsConnection;
            }
            if (connToRemove != null) ShapeCollector.GetI().ObsConnections.Remove(connToRemove);
        }
    }
}
