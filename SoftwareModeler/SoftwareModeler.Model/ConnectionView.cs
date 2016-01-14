using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Area51.SoftwareModeler.Models
{
    public abstract class ConnectionView : NotifyBase
    {
        public ClassData StartTemp { get; set; }

        public ClassData EndTemp { get; set; }


        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }


        private Point _p1;

        public Point P1 { get { return _p1; } set { _p1 = value; } }

        private Point _p2;

        public Point P2 { get { return _p2; } set { _p2 = value; } }


        public PointCollection PointCollection { get; set; }

        public PointCollection PolygonPoints { get; set; }

        private bool _isSelected;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; NotifyPropertyChanged(); } }

        public ConnectionType Type;

      
            public enum Direction { Left, Right, Up, Down };
        
        public void UpdatePoints(int startId, int? endId)
        {
            UpdatePoints(null,startId, endId);
        }
        public void UpdatePoints(Point? tempEndpoint,int startId, int? endId)
        {

            PointCollection = new PointCollection();
            PolygonPoints = null;
            StartTemp = ShapeCollector.GetI().GetShapeById(startId);
            if (endId == null)
            {
                // drawing temporary line while dragging
                if (tempEndpoint == null) return;
                StartPoint = new Point(StartTemp.CanvasCenterX, StartTemp.CanvasCenterY);
                PointCollection.Add(StartPoint);

                PointCollection.Add((Point)tempEndpoint);
                NotifyPropertyChanged(() => StartPoint);
                NotifyPropertyChanged(() => PointCollection);
                return;
            }
            EndTemp = ShapeCollector.GetI().GetShapeById(endId);



            Point[] sP =             {  new Point(StartTemp.CanvasCenterX + StartTemp.Width / 2, StartTemp.CanvasCenterY),  //0 right
                                        new Point(StartTemp.CanvasCenterX - StartTemp.Width / 2, StartTemp.CanvasCenterY),  //1 left
                                        new Point(StartTemp.CanvasCenterX, StartTemp.CanvasCenterY + StartTemp.Height/2),   //2 bottom
                                        new Point(StartTemp.CanvasCenterX, StartTemp.CanvasCenterY - StartTemp.Height/2) }; //3 top
            Point[] eP =             {  new Point(EndTemp.CanvasCenterX + EndTemp.Width / 2, EndTemp.CanvasCenterY),        //0 right
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
            Direction d;
            if (eInd == 0) d = Direction.Left;
            else if (eInd == 1) d = Direction.Right;
            else if (eInd == 2) d = Direction.Up;
            else d = Direction.Down;


            switch (Type)
            {
                case ConnectionType.Aggregation:
                    DrawAggregation(d);
                    break;
                case ConnectionType.Composition:
                    DrawComposition(d);
                    break;
                case ConnectionType.Association:
                    DrawAssociation(d);
                    break;
                    case ConnectionType.Inheritance:
                        DrawInheritance(d);
                    break;
                default:
                    //whats wrong with you? this is off limits!
                    throw new ArgumentOutOfRangeException("should never happen!? Thats not part of the enum. Type was: " + Type);
            }

            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => PointCollection);
            NotifyPropertyChanged(() => PolygonPoints);
        }

        private void DrawInheritance(Direction d)
        {
            PointCollection.RemoveAt(PointCollection.Count - 1);
            switch (d)
            {
                case Direction.Down:
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 7));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y - 7));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y - 7));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 7));
                    break;
                case Direction.Up:
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 7));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y + 7));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y + 7));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 7));
                    break;
                case Direction.Left:
                    PointCollection.Add(new Point(EndPoint.X + 7, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X + 7, EndPoint.Y));
                    break;
                case Direction.Right:
                    PointCollection.Add(new Point(EndPoint.X - 7, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X - 7, EndPoint.Y));
                    break;
            }
        }

        //association
        private void DrawAssociation(Direction d)
        {
            switch (d)
            {
                case Direction.Down:
                    PointCollection.Add(new Point(EndPoint.X - 2.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 2.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.Up:
                    PointCollection.Add(new Point(EndPoint.X - 2.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 2.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.Left:
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y - 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y + 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
                case Direction.Right:
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y - 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y + 2.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    break;
            }
        }

        //agregation
        private void DrawAggregation(Direction d)
        {
            PointCollection.RemoveAt(PointCollection.Count - 1);
            switch (d)
            {
                case Direction.Down:

                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y - 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y - 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    break;
                case Direction.Up:
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    PointCollection.Add(new Point(EndPoint.X + 5, EndPoint.Y + 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 5, EndPoint.Y + 7.5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    break;
                case Direction.Left:
                    PointCollection.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X + 7.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    break;
                case Direction.Right:
                    PointCollection.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7.5, EndPoint.Y + 5));
                    PointCollection.Add(new Point(EndPoint.X, EndPoint.Y));
                    PointCollection.Add(new Point(EndPoint.X - 7.5, EndPoint.Y - 5));
                    PointCollection.Add(new Point(EndPoint.X - 15, EndPoint.Y));
                    break;
            }

        }

        //composition
        private void DrawComposition(Direction d)
        {

            PolygonPoints = new PointCollection();
            Polygon poly = new Polygon();
            poly.Fill = Brushes.Black;
            switch (d)
            {
                case Direction.Down:
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    PolygonPoints.Add(new Point(EndPoint.X + 5, EndPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 5, EndPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y - 15));
                    break;
                case Direction.Up:
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    PolygonPoints.Add(new Point(EndPoint.X + 5, EndPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X - 5, EndPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y + 15));
                    break;
                case Direction.Left:
                    PolygonPoints.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X + 7.5, EndPoint.Y + 5));
                    PolygonPoints.Add(new Point(EndPoint.X, EndPoint.Y));
                    PolygonPoints.Add(new Point(EndPoint.X + 7.5, EndPoint.Y - 5));
                    PolygonPoints.Add(new Point(EndPoint.X + 15, EndPoint.Y));
                    break;
                case Direction.Right:
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
