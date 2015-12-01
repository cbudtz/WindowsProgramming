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
        public Connection conn { get; set; }

        public AddConnectionCommand()
        {
            
        }
        public AddConnectionCommand(Shape from, string fromMult, Shape to, string toMult, ConnectionType type)
        {
            //TODO do correct implementation
           conn = new Connection(from, fromMult, to, toMult, type);
            
                }
        public override void execute()
        {
            Console.WriteLine("connection added");
            ShapeCollector.getI().obsConnections.Add(conn);
        }

        public override void unExecute()
        {
            foreach (Connection obsConnection in ShapeCollector.getI().obsConnections)
            {
                //if (conn!=null && obsConnection.connectionID == conn.connectionID)
                    ShapeCollector.getI().obsConnections.Remove(obsConnection);
            }
        }
    }
}
