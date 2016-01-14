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
        public ObservableCollection<ClassData> ObsShapes { get; set;   }
        [XmlIgnore]
        public ObservableCollection<Comment> ObsComments { get; set; }
        [XmlIgnore]
        public ObservableCollection<ConnectionData> ObsConnections { get; set; }
        [XmlIgnore]
        public ObservableCollection<BaseCommand> Commands { get; set; }
        [XmlIgnore]
        public ObservableCollection<int> MaxBranchLayer { get; set; }
        [XmlIgnore]
        public ObservableCollection<LineCommandTree> treeArrows { get; set; }


        private ShapeCollector()
        {
            ObsShapes = new ObservableCollection<ClassData>();   
            ObsComments = new ObservableCollection<Comment>();         
            ObsConnections = new ObservableCollection<ConnectionData>();
            Commands = new ObservableCollection<BaseCommand>();

            //contains the "connections" used to draw lines in the command-tree.
            treeArrows = new ObservableCollection<LineCommandTree>();

            //adding a few extra buffers to the size of the scroll-area. (yes it's an ugly fix)
            MaxBranchLayer = new ObservableCollection<int>();
            MaxBranchLayer.Add(1);
            MaxBranchLayer.Add(2);
            MaxBranchLayer.Add(3);
        }
        ///<summary>Get instance</summary>
        public static ShapeCollector GetI()
        {
            return _shapeCollector ?? (_shapeCollector = new ShapeCollector());
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

        internal ConnectionData GetConnectionById(int id) => ObsConnections.FirstOrDefault(x => x.ConnectionId == id);

        internal ClassData GetShapeById(int? shapeId) => ObsShapes.FirstOrDefault(x => x.id == shapeId);

        internal Comment GetCommentById(int? id) => ObsComments.FirstOrDefault(x => x.id == id);
    }
}
