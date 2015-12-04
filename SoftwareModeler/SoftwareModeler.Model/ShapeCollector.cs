using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace Area51.SoftwareModeler.Models
{
    public class ShapeCollector : NotifyBase
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

        [XmlIgnore]
        public ObservableCollection<int> MaxBranchLayer { get; set; }


        private ShapeCollector()  {
            obsShapes = new ObservableCollection<Shape>();            
            obsConnections = new ObservableCollection<Connection>();
            commands = new ObservableCollection<BaseCommand>();

            //adding a few extra buffers to the size of the scroll-area. (yes it's an ugly fix)
            MaxBranchLayer = new ObservableCollection<int>();
            MaxBranchLayer.Add(1);
            MaxBranchLayer.Add(2);
            MaxBranchLayer.Add(3);
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
                //Console.WriteLine("ShapeCollector - get shapeBy ID:" + obsShape);
                if (obsShape.id == shapeId) return obsShape;

            }
            return null;
        }
    }
}
