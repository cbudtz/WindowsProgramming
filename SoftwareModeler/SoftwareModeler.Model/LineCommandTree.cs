using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Models
{
    public class LineCommandTree : NotifyBase
    {

        public Point ParentPoint { get; set; }
        public PointCollection Points { get; set; }

        private readonly BaseCommand _parent;
        private readonly BaseCommand _child;
        private bool isActive = false;
        public bool IsActive { get { return isActive; } set { isActive = value; NotifyPropertyChanged(); } }
        //used for adding lines to the command-tree.
        public LineCommandTree(BaseCommand parent, BaseCommand child)
        {
            _parent = parent;
            _child = child;
            SetEndPoint(parent, child);
        }

        public void UpdateLine()
        {
            SetEndPoint(_parent, _child);
        }

        private void SetEndPoint(BaseCommand parent, BaseCommand child)
        {
            Points = new PointCollection();

            if (parent.BranchLayer == child.BranchLayer)
            {
                // 40 is offset to get to center of square containing command letter
                // 25 is offset to get to bottom of same square
                double parentX = 25 + parent.X * 30 + 5;
                double parentY = 25 + parent.Y * 30 + 5;
                double childY = child.Y * 30 + 5;
                ParentPoint = new Point(parentX, parentY);
                Points.Add(new Point(parentX, childY));
            }
            else
            {
                double parentX = 50 + parent.X*30 + 5;
                double parentY = 15 + parent.Y*30 + 5;
                double childX =  25 +  child.X*30 + 5;
                double childY =        child.Y*30 + 5;
                // _parent point set
                ParentPoint = new Point(parentX, parentY);
                // intermediate point (corner)
                Points.Add(new Point(childX, parentY));
                // lowest point
                Points.Add(new Point(childX, childY));
                
            }


            //notify
            NotifyPropertyChanged(() => ParentPoint);
            NotifyPropertyChanged(() => Points);
        }
        
    }
}
