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
        [XmlIgnore] private ConnectionData Conn;

        public int ConnId;
        public int FromId;
        public int ToId;
        public ConnectionType Type;
        public string FromMult;
        public string ToMult;


        public AddConnectionCommand()
        {

        }

        public AddConnectionCommand(int connId, int from, string fromMult, int to, string toMult, ConnectionType type)
        {
            ConnId = connId;
            FromId = from;
            ToId = to;
            Type = type;
            FromMult = fromMult;
            ToMult = toMult;
        }

        public override void execute()
        {
            //Console.WriteLine("connection added");
            if (Conn == null) Conn = new ConnectionData(ConnId, FromId, FromMult, ToId, ToMult, Type);
            ShapeCollector.GetI().ObsConnections.Add(Conn);
        }

        public override void unExecute()
        {
            ShapeCollector.GetI().ObsConnections.Remove(ShapeCollector.GetI().GetConnectionById(Conn.ConnectionId));
        }

        public override string CommandName => "Add Connection";

        public override string Info
        {
            get
            {
                ConnectionData c = ShapeCollector.GetI().GetConnectionById(ConnId);
                if(c == null) return InfoBackup;
                return InfoBackup = "Connection Type: " + c.Type +
                                    "\nFrom: " + ShapeCollector.GetI().GetShapeById(FromId).Name +
                                    "\nTo: " + ShapeCollector.GetI().GetShapeById(ToId).Name;
            }
        }
    }
}
