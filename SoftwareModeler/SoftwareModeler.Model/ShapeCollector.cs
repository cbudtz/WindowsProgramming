using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models
{
    public class ShapeCollector
    {
        //Singleton
        private static ShapeCollector shapeCollector;
        //Objects of interest for ViewModel
        [XmlIgnore]
        public ObservableCollection<Shape> obsShapes { get; set;   }
        [XmlIgnore]
        public ObservableCollection<Connection> obsConnections { get; set; }
        [XmlIgnore]
        public ObservableCollection<BaseCommand> commands { get; set; }


        private ShapeCollector()  {
            obsShapes = new ObservableCollection<Shape>();            
            obsConnections = new ObservableCollection<Connection>();
            commands = new ObservableCollection<BaseCommand>(); 
        }
        ///<summary>Get instance</summary>
        public static ShapeCollector getI()
        {
            if (shapeCollector== null)
            {
                shapeCollector = new ShapeCollector();
            }
            return shapeCollector;
        }
        public static void setI(ShapeCollector shape)
        {
            shapeCollector = shape;
        }

        internal void reset()
        {
            obsShapes.Clear();
            obsConnections.Clear();
        }

        internal Shape getShapeByID(int? shapeId)
        {

            foreach (Shape obsShape in obsShapes)
            {
                Console.WriteLine(obsShape);
                if (obsShape.id == shapeId) return obsShape;

            }
            return null;
        }
    }
}
