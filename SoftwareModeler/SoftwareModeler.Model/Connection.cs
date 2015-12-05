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
    public class Connection : NotifyBase
    {
        //Unique ID
        [XmlIgnore]
        public static int NextId = 0;

        //private string startMultiplicity;
        public string StartMultiplicity { get;  set ;  }
        //private string endMultiplicity;
        public string EndMultiplicity { get ;  set ;  }

        private int? _startShapeId;
        public int? EndShapeId;

         
        public int? StartShapeId{get { return _startShapeId; } set{_startShapeId = value; Start = ShapeCollector.GetI().GetShapeById(value); }}

        [XmlIgnore]
        private Shape Start { get { return ShapeCollector.GetI().GetShapeById(StartShapeId); } set { _startShapeId = value.id; updatePoints();}}       
        [XmlIgnore]
        private Shape End { get { return ShapeCollector.GetI().GetShapeById(EndShapeId); } set { EndShapeId = value.id; updatePoints(); } }

         
        public Shape StartTemp { get; set; }
         
        public Shape EndTemp { get; set; }

         
        public Point StartPoint { get; set; }
         
        public Point EndPoint { get; set; }

         
        private Point _p1;
         
        public Point P1 { get { return _p1; } set { _p1 = value; } }      
         
        private Point _p2;
         
        public Point P2 { get { return _p2; } set { _p2 = value; } }
             
         
        public PointCollection PointCollection { get; set; }
         
        public PointCollection PolygonPoints { get; set; }

        public ConnectionType Type;
        //Unique identifyer
        public int ConnectionId { get; set; }

        public Connection()
        {
            //Deserialization Constructor - Shapes must be deserialized first!
        }

        public Connection(int startId, string startMultiplicity, int? endId, string endMultiplicity, ConnectionType type)
        {
            this.ConnectionId = NextId++;
            StartShapeId = startId; //Saving ID for serialization!

            StartMultiplicity = startMultiplicity;
            EndShapeId = endId; //Saving ID for serialization
            EndMultiplicity = endMultiplicity;
            Type = type;

            StartPoint = new Point();
            _p1 = new Point();
            _p2 = new Point();
            EndPoint = new Point();

            updatePoints();
        }
        //used for adding lines to the command-tree.
        public Connection(BaseCommand start, BaseCommand end)
        {
            //start at 50 out and 15 down, add 25 pr node.
            StartPoint = new Point(50 + (start.BranchLayer * 25), 15 + (start.Id * 25));
            PointCollection = new PointCollection();
            //corner at 40,15 + branchlayer and nodelayer
            Point p = new Point(40 + (end.BranchLayer * 25), 15 + (start.Id * 25));
            PointCollection.Add(p);
            //end and the top of the child node, no offset
            p.Y = (end.Id * 25);
            PointCollection.Add(p);

            //notify
            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => PointCollection);
        }

        private class Direction
        {
            public enum direction {Left,Right,Up,Down };
        }
        public void updatePoints()
        {
            updatePoints(null);
        }
        public void updatePoints(Point? tempEndpoint)
        {
            
            PointCollection = new PointCollection();
            PolygonPoints = null;
            Shape End = ShapeCollector.GetI().GetShapeById(EndShapeId);
            if (End == null)
            {
                // drawing temporary line while dragging
                if (tempEndpoint == null) return;
                StartPoint = new Point(Start.CanvasCenterX, Start.CanvasCenterY);
                PointCollection.Add(StartPoint);

                PointCollection.Add((Point) tempEndpoint);
                NotifyPropertyChanged(() => StartPoint);
                NotifyPropertyChanged(() => PointCollection);
                return;
            }
            if (Start != null) StartTemp = Start;
            if (EndShapeId != null && EndTemp != null)
            {
                EndTemp = null;
                NotifyPropertyChanged(() => PointCollection);
            }
            EndTemp = End;



            Point[] sP = new Point[] {  new Point(StartTemp.CanvasCenterX + StartTemp.Width / 2, StartTemp.CanvasCenterY),  //0 right
                                        new Point(StartTemp.CanvasCenterX - StartTemp.Width / 2, StartTemp.CanvasCenterY),  //1 left
                                        new Point(StartTemp.CanvasCenterX, StartTemp.CanvasCenterY + StartTemp.Height/2),   //2 bottom
                                        new Point(StartTemp.CanvasCenterX, StartTemp.CanvasCenterY - StartTemp.Height/2) }; //3 top
            Point[] eP = new Point[] {  new Point(EndTemp.CanvasCenterX + EndTemp.Width / 2, EndTemp.CanvasCenterY),        //0 right
                                        new Point(EndTemp.CanvasCenterX - EndTemp.Width / 2, EndTemp.CanvasCenterY),        //1 left
                                        new Point(EndTemp.CanvasCenterX, EndTemp.CanvasCenterY + EndTemp.Height / 2),       //2 bottom
                                        new Point(EndTemp.CanvasCenterX, EndTemp.CanvasCenterY - EndTemp.Height / 2)        //3 top
            };
            double dist = -1;
            int sInd = -1;
            int eInd = -1;
            // finds index of _startPoint and _endPoint. all possible combinations are created in sP and eP arrays above
            for (int i = 0; i < sP.Length; i++)
            {
                for (int j = 0; j < eP.Length; j++)
                {
                    double curDist = Math.Sqrt(Math.Pow(Math.Abs(sP[i].X - eP[j].X), 2) + Math.Pow(Math.Abs(sP[i].Y - eP[j].Y), 2));
                    if (dist < 0 || curDist < dist)
                    {
                        sInd = i;
                        eInd = j;
                        dist = curDist;
                    }
                }
            }
            /*
            startpoint index,endpoint index
            Combinations:       right->left     0,1      right->bottom   0,2
                                right->top      0,3      left->right     1,0
                                left->bottom    1,2      left->top       1,3
                                bottom->right   2,0      bottom->left    2,1
                                bottom->top     2,3      top->right      3,0
                                top->left       3,1      top->bottom     3,2
    */

            StartPoint = sP[sInd];
            EndPoint = eP[eInd];

            switch (sInd)
            {
                case 0:
                case 1:
                    switch (eInd)
                    {
                        case 0:
                        case 1:
                            // right/left -> left/right. right->left is same as left->right. 
                            // left->left and right->right is not possible
                            _p1.X = (EndPoint.X + StartPoint.X) / 2;
                            _p1.Y = StartPoint.Y;
                            _p2.X = _p1.X;
                            _p2.Y = EndPoint.Y;
                            break;
                        case 2:
                        case 3:
                            //right/left ->bottom/top. 4 possible combinations. all the same.
                            _p1.X = EndPoint.X;
                            _p1.Y = StartPoint.Y;
                            _p2 = _p1;
                            break;
                    }
                    break;
                case 2:
                case 3:
                    switch (eInd)
                    {
                        case 0:
                        case 1:
                            // bottom/top -> right->left. 4 possible combinations. all work the same.
                            _p1.X = EndPoint.X;
                            _p1.Y = StartPoint.Y;
                            _p2 = _p1;
                            break;
                        case 2:
                        case 3:
                            // bottom/top -> top/bottom. 2 possible combinations. 
                            _p1.X = StartPoint.X;
                            _p1.Y = (StartPoint.Y + EndPoint.Y) / 2;
                            _p2.X = EndPoint.X;
                            _p2.Y = _p1.Y;
                            break;
                    }
                    break;
            }

            PointCollection.Add(_p1);
            PointCollection.Add(_p2);
            PointCollection.Add(EndPoint);
            Direction.direction d;
            if (eInd == 0) d = Direction.direction.Left;
            else if (eInd == 1) d = Direction.direction.Right;
            else if (eInd == 2) d = Direction.direction.Up;
            else d = Direction.direction.Down;
      
           
            switch (Type)
            {
                case ConnectionType.Aggregation:
                    DrawRhombus(d);
                    break;
                case ConnectionType.Composition:
                    DrawFilledRhombus(d);
                    break;
                case ConnectionType.Association:
                    DrawArrow(d);
                    break;
                default:
                    //whats wrong with you? this is off limits!
                    throw new ArgumentOutOfRangeException("should never happen!? Thats not part of the enum. Type was: " + Type);
            }

            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => PointCollection);
            NotifyPropertyChanged(() => PolygonPoints);
        }

        //association
        private void DrawArrow(Direction.direction d)
        {
            switch (d)
            {
                case Direction.direction.Down:
                    PointCollection.Add(new Point(EndPoint.X - 2.5, EndPoint.Y - 5 ));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 2.5, EndPoint.Y - 5 ));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.direction.Up:
                    PointCollection.Add(new Point(EndPoint.X - 2.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 2.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.direction.Left:
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y - 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y + 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.direction.Right:
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y - 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y + 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
            } 
        }

        //agregation
        private void DrawRhombus(Direction.direction d)
        {
            PointCollection.RemoveAt(PointCollection.Count - 1);
            switch (d)
            {
                case Direction.direction.Down:

                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y - 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y - 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    break;
                case Direction.direction.Up:
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y + 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y + 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    break;
                case Direction.direction.Left:
                    PointCollection.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    break;
                case Direction.direction.Right:
                    PointCollection.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    break;
            }
            
        }

        //composition
        private void DrawFilledRhombus(Direction.direction d)
        {
            
            PolygonPoints = new PointCollection();
            Polygon poly = new Polygon();
            poly.Fill = Brushes.Black;
            switch (d)
            {
                case Direction.direction.Down:

                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    PolygonPoints.Add(new Point(EndPoint.X + 5, EndPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 5, EndPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    break;
                case Direction.direction.Up:
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    PolygonPoints.Add(new Point(EndPoint.X + 5, EndPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 5, EndPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    break;
                case Direction.direction.Left:
                    PolygonPoints.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X + 7.5, EndPoint.Y + 5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X + 7.5, EndPoint.Y - 5));
                    PolygonPoints.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    break;
                case Direction.direction.Right:
                    PolygonPoints.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 7.5, EndPoint.Y + 5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 7.5, EndPoint.Y - 5));
                    PolygonPoints.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    break;
            }

            poly.Points = PolygonPoints;
        }
    }
}
