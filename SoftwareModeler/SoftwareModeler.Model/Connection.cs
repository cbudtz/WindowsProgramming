using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public class Connection
    {
        private Shape start;
        private string startMultiplicity;
        private string endMultiplicity;
        private Shape end;
        private ConnectionType type;

        public Shape Start { get { return start; } set { start = value; } }
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        public Shape End { get { return end; } set { end = value; } }
        public string EndMultiplicity { get { return endMultiplicity; } set { endMultiplicity = value; } }

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            start = _start;
            startMultiplicity = _startMultiplicity;
            end = _end;
            endMultiplicity = _endMultiplicity;
            type = _type;
        }
    }
}
