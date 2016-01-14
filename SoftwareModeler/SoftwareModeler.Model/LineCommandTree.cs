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

        public Point StartPoint { get; set; }
        public PointCollection Points { get; set; }
        private bool isActive = false;
        public bool IsActive { get { return isActive; } set { isActive = value; NotifyPropertyChanged(); } }
        //used for adding lines to the command-tree.
        public LineCommandTree(BaseCommand start, BaseCommand end)
        {
            Points = new PointCollection();
            
            if (start.BranchLayer == end.BranchLayer)
            {
                // 40 is offset to get to center of square containing command letter
                // 25 is offset to get to bottom of same square
                StartPoint = new Point(40 + start.BranchLayer*30, start.Id*30 + 25);
                Point p = new Point(StartPoint.X, end.Id*30);
                Points.Add(p);
            }
            else
            {

                StartPoint = new Point(50 + (start.BranchLayer*30), 15 + (start.Id*30));
                Point p = new Point(40 + (end.BranchLayer * 30), 15 + (start.Id * 30));
                Points.Add(p);
                p.Y = (end.Id * 30);
                Points.Add(p);
            }
            

            //notify
            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => Points);
        }

        
    }
}
