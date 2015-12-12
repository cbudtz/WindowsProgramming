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
        private static ShapeCollector _shapeCollector;
        //Objects of interest for ViewModel
        [XmlIgnore]
        public ObservableCollection<Class> ObsShapes { get; set;   }
        [XmlIgnore]
        public ObservableCollection<Connection> ObsConnections { get; set; }
        [XmlIgnore]
        public ObservableCollection<BaseCommand> Commands { get; set; }
        [XmlIgnore]
        public ObservableCollection<int> MaxBranchLayer { get; set; }
        [XmlIgnore]
        public ObservableCollection<Connection> treeArrows { get; set; }


        private ShapeCollector()
        {
            ObsShapes = new ObservableCollection<Class>();            
            ObsConnections = new ObservableCollection<Connection>();
            Commands = new ObservableCollection<BaseCommand>();

            //contains the "connections" used to draw lines in the command-tree.
            treeArrows = new ObservableCollection<Connection>();

            //adding a few extra buffers to the size of the scroll-area. (yes it's an ugly fix)
            MaxBranchLayer = new ObservableCollection<int> {1, 2, 3};
        }
        ///<summary>Get instance</summary>
        public static ShapeCollector GetI()
        {
            //lazy loading
            return _shapeCollector ?? (_shapeCollector = new ShapeCollector());
        }

        internal void Reset()
        {
            ObsShapes.Clear();
            ObsConnections.Clear();
        }

        internal Class GetShapeById(int? shapeId)
        {
            //LINQ
            return ObsShapes.FirstOrDefault(obsShape => obsShape.id == shapeId);
        }
    }
}
