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
        public static int nextID = 0;
        public int? startShapeID;
        public Shape Start { get { return ShapeCollector.getI().getShapeByID(startShapeID); } set { startShapeID = value.id; updatePoints(); } }
        //private string startMultiplicity;
        public string StartMultiplicity { get;  set ;  }
        //private string endMultiplicity;
        public string EndMultiplicity { get ;  set ;  }

        public int? endShapeID;

        public Shape End { get { return ShapeCollector.getI().getShapeByID(endShapeID); } set { endShapeID = value.id; updatePoints(); } }
        private Point startPoint;
        public Point StartPoint { get { return startPoint; } set { startPoint = value; } }
        private Point p1;
        public Point P1 { get { return p1; } set { p1 = value; } }
        private Point p2;
        public Point P2 { get { return p2; } set { p2 = value; } }
        private Point endPoint;
        public Point EndPoint { get { return endPoint; } set { endPoint = value; } }
        private PointCollection pointCollection;
        public PointCollection PointCollection { get { return pointCollection; } set { pointCollection = value; } }
        private PointCollection polygonPoints;
        public PointCollection PolygonPoints { get { return polygonPoints; } set { polygonPoints = value; } }

        public ConnectionType type;
        //Unique identifyer
        public int connectionID { get; set; }

        public Connection()
        {
            //Deserialization Constructor - Shapes must be deserialized first!
        }

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            this.connectionID = nextID++;
            startShapeID = _start.id; //Saving ID for serialization!

            StartMultiplicity = _startMultiplicity;
            if (_end !=null) endShapeID = _end.id; //Saving ID for serialization
            EndMultiplicity = _endMultiplicity;
            type = _type;

            startPoint = new Point();
            p1 = new Point();
            p2 = new Point();
            endPoint = new Point();

            updatePoints();
        }

        private class Direction
        {
            public enum direction {LEFT,RIGHT,UP,DOWN };
        }
        public void updatePoints()
        {
            updatePoints(null);
        }
        public void updatePoints(Point? tempEndpoint)
        {
            
            pointCollection = new PointCollection();
            PolygonPoints = null;
            if (End == null)
            {
                // drawing temporary line while dragging
                if (tempEndpoint == null) return;
                StartPoint = new Point(Start.CanvasCenterX, Start.CanvasCenterY);
                pointCollection.Add(StartPoint);

                pointCollection.Add((Point) tempEndpoint);
                NotifyPropertyChanged(() => StartPoint);
                NotifyPropertyChanged(() => PointCollection);
                return;
            }
            

            Point[] sP = new Point[] {  new Point(Start.CanvasCenterX + Start.Width / 2, Start.CanvasCenterY),  //0 right
                                        new Point(Start.CanvasCenterX - Start.Width / 2, Start.CanvasCenterY),  //1 left
                                        new Point(Start.CanvasCenterX, Start.CanvasCenterY + Start.Height/2),   //2 bottom
                                        new Point(Start.CanvasCenterX, Start.CanvasCenterY - Start.Height/2) }; //3 top
            Point[] eP = new Point[] {  new Point(End.CanvasCenterX + End.Width / 2, End.CanvasCenterY),        //0 right
                                        new Point(End.CanvasCenterX - End.Width / 2, End.CanvasCenterY),        //1 left
                                        new Point(End.CanvasCenterX, End.CanvasCenterY + End.Height / 2),       //2 bottom
                                        new Point(End.CanvasCenterX, End.CanvasCenterY - End.Height / 2)        //3 top
            };
            double dist = -1;
            int sInd = -1;
            int eInd = -1;
            // finds index of startPoint and endPoint. all possible combinations are created in sP and eP arrays above
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
                            p1.X = (EndPoint.X + StartPoint.X) / 2;
                            p1.Y = StartPoint.Y;
                            p2.X = p1.X;
                            p2.Y = EndPoint.Y;
                            break;
                        case 2:
                        case 3:
                            //right/left ->bottom/top. 4 possible combinations. all the same.
                            p1.X = EndPoint.X;
                            p1.Y = StartPoint.Y;
                            p2 = p1;
                            break;
                        default: break;
                    }
                    break;
                case 2:
                case 3:
                    switch (eInd)
                    {
                        case 0:
                        case 1:
                            // bottom/top -> right->left. 4 possible combinations. all work the same.
                            p1.X = EndPoint.X;
                            p1.Y = StartPoint.Y;
                            p2 = p1;
                            break;
                        case 2:
                        case 3:
                            // bottom/top -> top/bottom. 2 possible combinations. 
                            p1.X = StartPoint.X;
                            p1.Y = (StartPoint.Y + EndPoint.Y) / 2;
                            p2.X = EndPoint.X;
                            p2.Y = p1.Y;
                            break;
                        default: break;
                    }
                    break;
            }

            pointCollection.Add(p1);
            pointCollection.Add(p2);
            pointCollection.Add(endPoint);
            Direction.direction d;
            if (eInd == 0) d = Direction.direction.LEFT;
            else if (eInd == 1) d = Direction.direction.RIGHT;
            else if (eInd == 2) d = Direction.direction.UP;
            else d = Direction.direction.DOWN;
      
           
            switch (type)
            {
                case ConnectionType.Aggregation:
                    drawRhombus(d);
                    break;
                case ConnectionType.Composition:
                    drawFilledRhombus(d);
                    break;
                case ConnectionType.Association:
                    drawArrow(d);
                    break;
                default:
                    //whats wrong with you? this is off limits!
                    throw new ArgumentOutOfRangeException("should never happen!? Thats not part of the enum. type was: " + type);
            }

            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => PointCollection);
            NotifyPropertyChanged(() => PolygonPoints);
        }

        //association
        private void drawArrow(Direction.direction d)
        {
            switch (d)
            {
                case Direction.direction.DOWN:
                    pointCollection.Add(new Point(endPoint.X - 2.5, endPoint.Y - 5 ));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X + 2.5, endPoint.Y - 5 ));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    break;
                case Direction.direction.UP:
                    pointCollection.Add(new Point(endPoint.X - 2.5, endPoint.Y + 5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X + 2.5, endPoint.Y + 5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    break;
                case Direction.direction.LEFT:
                    pointCollection.Add(new Point(endPoint.X + 5, endPoint.Y - 2.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X + 5, endPoint.Y + 2.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    break;
                case Direction.direction.RIGHT:
                    pointCollection.Add(new Point(endPoint.X - 5, endPoint.Y - 2.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X - 5, endPoint.Y + 2.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    break;
            } 
        }

        //agregation
        private void drawRhombus(Direction.direction d)
        {
            pointCollection.RemoveAt(pointCollection.Count - 1);
            switch (d)
            {
                case Direction.direction.DOWN:

                    pointCollection.Add(new Point(endPoint.X, endPoint.Y - 15));
                    pointCollection.Add(new Point(endPoint.X + 5, endPoint.Y - 7.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X - 5, endPoint.Y - 7.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y - 15));
                    break;
                case Direction.direction.UP:
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y + 15));
                    pointCollection.Add(new Point(endPoint.X + 5, endPoint.Y + 7.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X - 5, endPoint.Y + 7.5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y + 15));
                    break;
                case Direction.direction.LEFT:
                    pointCollection.Add(new Point(endPoint.X + 15, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X + 7.5, endPoint.Y + 5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X + 7.5, endPoint.Y - 5));
                    pointCollection.Add(new Point(endPoint.X + 15, endPoint.Y));
                    break;
                case Direction.direction.RIGHT:
                    pointCollection.Add(new Point(endPoint.X - 15, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X - 7.5, endPoint.Y + 5));
                    pointCollection.Add(new Point(endPoint.X, endPoint.Y));
                    pointCollection.Add(new Point(endPoint.X - 7.5, endPoint.Y - 5));
                    pointCollection.Add(new Point(endPoint.X - 15, endPoint.Y));
                    break;
            }
            
        }

        //composition
        private void drawFilledRhombus(Direction.direction d)
        {
            
            PolygonPoints = new PointCollection();
            Polygon poly = new Polygon();
            poly.Fill = Brushes.Black;
            switch (d)
            {
                case Direction.direction.DOWN:

                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y - 15));
                    PolygonPoints.Add(new Point(endPoint.X + 5, endPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X - 5, endPoint.Y - 7.5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y - 15));
                    break;
                case Direction.direction.UP:
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y + 15));
                    PolygonPoints.Add(new Point(endPoint.X + 5, endPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X - 5, endPoint.Y + 7.5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y + 15));
                    break;
                case Direction.direction.LEFT:
                    PolygonPoints.Add(new Point(endPoint.X + 15, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X + 7.5, endPoint.Y + 5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X + 7.5, endPoint.Y - 5));
                    PolygonPoints.Add(new Point(endPoint.X + 15, endPoint.Y));
                    break;
                case Direction.direction.RIGHT:
                    PolygonPoints.Add(new Point(endPoint.X - 15, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X - 7.5, endPoint.Y + 5));
                    PolygonPoints.Add(new Point(endPoint.X, endPoint.Y));
                    PolygonPoints.Add(new Point(endPoint.X - 7.5, endPoint.Y - 5));
                    PolygonPoints.Add(new Point(endPoint.X - 15, endPoint.Y));
                    break;
            }

            poly.Points = PolygonPoints;
        }
    }
}
