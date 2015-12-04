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
    public class ShapeCollector
    {
        //Singleton
        private static ShapeCollector _shapeCollector;
        //Objects of interest for ViewModel
        [XmlIgnore]
        public ObservableCollection<Class> ObsShapes { get; set;   }
        [XmlIgnore]
        public ObservableCollection<Connection> ObsConnections { get; set; }
        [XmlIgnore]
        public ObservableCollection<BaseCommand> Commands { get; set; }

        [XmlIgnore]
        public ObservableObject MaxBranchLayer {
            get
            {
                int max = 0;
                foreach (BaseCommand b in Commands)
                {
                    if (b.BranchLayer > max) max = b.BranchLayer;
                }
                return new ObservableObject();
            } }


        private ShapeCollector()  {
            ObsShapes = new ObservableCollection<Class>();            
            ObsConnections = new ObservableCollection<Connection>();
            Commands = new ObservableCollection<BaseCommand>(); 
        }
        ///<summary>Get instance</summary>
        public static ShapeCollector GetI()
        {
            if (_shapeCollector== null)
            {
                _shapeCollector = new ShapeCollector();
            }
            return _shapeCollector;
        }
        public static void SetI(ShapeCollector shape)
        {
            _shapeCollector = shape;
        }

        internal void Reset()
        {
            ObsShapes.Clear();
            ObsConnections.Clear();
        }

        internal Class GetShapeById(int? shapeId)
        {

            foreach (Class obsShape in ObsShapes)
            {
                //Console.WriteLine("ShapeCollector - get shapeBy ID:" + obsShape);
                if (obsShape.id == shapeId) return obsShape;

            }
            return null;
        }
    }
}
