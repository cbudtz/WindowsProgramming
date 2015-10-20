using Area51.SoftwareModeler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Model
{
    public class ShapeCollector
    {
        private static ShapeCollector shapeCollector;

        public ObservableCollection<Shape> obsShapes { get; set;   }
        public ObservableCollection<Shape> removedShapes { get; set; }

        private ShapeCollector()  {
            obsShapes = new ObservableCollection<Shape>();
            removedShapes = new ObservableCollection<Shape>();


        }
        public static ShapeCollector getI()
        {
            if (shapeCollector== null)
            {
                shapeCollector = new ShapeCollector();
            }
            return shapeCollector;
        }

    }
}
