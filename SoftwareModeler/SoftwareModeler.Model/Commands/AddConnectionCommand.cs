using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddConnectionCommand : BaseCommand
    {
        public Connection Conn { get; set; }

        public AddConnectionCommand()
        {
            
        }
        public AddConnectionCommand(Shape from, string fromMult, Shape to, string toMult, ConnectionType type)
        {
            //TODO do correct implementation
           Conn = new Connection(from, fromMult, to, toMult, type);
            
                }
        public override void execute()
        {
            Console.WriteLine("connection added");
            ShapeCollector.getI().obsConnections.Add(Conn);
        }

        public override void unExecute()
        {
            Connection connToRemove = null;
            //TODO - dont remove from same collection!
            foreach (Connection obsConnection in ShapeCollector.getI().obsConnections.
                Where(obsConnection => Conn !=null && obsConnection.connectionID == Conn.connectionID))
            {
                connToRemove = obsConnection;
            }
            if (connToRemove != null) ShapeCollector.getI().obsConnections.Remove(connToRemove);
        }
    }
}
