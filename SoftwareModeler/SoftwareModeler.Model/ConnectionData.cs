using Area51.SoftwareModeler.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models
{
    public class ConnectionData : ConnectionView
    {
        //Unique ID
        [XmlIgnore]
        public static int NextId = 0;

        //private string startMultiplicity;
        public string StartMultiplicity { get;  set ;  }
        //private string endMultiplicity;
        public string EndMultiplicity { get ;  set ;  }

        private int _startShapeId;
        private int? _endShapeId;

       
         
        public int StartShapeId{get { return _startShapeId; } set{_startShapeId = value; Start = ShapeCollector.GetI().GetShapeById(value); }}
        public int? EndShapeId { get { return _endShapeId;  } set { _endShapeId = value;
            End = ShapeCollector.GetI().GetShapeById(value);
        } }

        [XmlIgnore]
        private ClassData Start { get { return ShapeCollector.GetI().GetShapeById(StartShapeId); } set { _startShapeId = value.id.Value; UpdatePoints(_startShapeId, _endShapeId);}}       
        [XmlIgnore]
        private ClassData End { get { return ShapeCollector.GetI().GetShapeById(_endShapeId); } set { _endShapeId = value.id; UpdatePoints(_startShapeId, _endShapeId); } }

         
 
        //Unique identifyer
        public int ConnectionId { get; set; }

        public ConnectionData()
        {
            //Deserialization Constructor - Shapes must be deserialized first!
        }

        public ConnectionData(int startId, string startMultiplicity, int? endId, string endMultiplicity,
            ConnectionType type) : this(null, startId, startMultiplicity, endId, endMultiplicity, type)
        {

        }

        public ConnectionData(int? id, int startId, string startMultiplicity, int? endId, string endMultiplicity, ConnectionType type)
        {
            this.ConnectionId =  id?? NextId++;
            StartShapeId = startId; //Saving ID for serialization!

            StartMultiplicity = startMultiplicity;
            _endShapeId = endId; //Saving ID for serialization
            EndMultiplicity = endMultiplicity;
            Type = type;

            StartPoint = new Point();
            P1 = new Point();
            P2 = new Point();
            EndPoint = new Point();

            UpdatePoints(_startShapeId, _endShapeId);
        }



        public void updatePoints()
        {
            UpdatePoints(_startShapeId, _endShapeId);
        }

        public void updatePoints(Point tempEndpoint)
        {
            UpdatePoints(tempEndpoint, _startShapeId, _endShapeId);
        }
    }
}
