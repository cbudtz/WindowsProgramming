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
        [XmlIgnore]
        private Shape startShape;
        //TODO: Clean up
        public int? startShapeID;
        public Shape Start { get { return startShape; } set { startShape = value; updatePoints(); } }
        private string startMultiplicity;
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        private string endMultiplicity;
        public string EndMultiplicity { get { return endMultiplicity; } set { endMultiplicity = value; } }
        [XmlIgnore]
        private Shape endShape;

        public int? endShapeID;

        public Shape End { get { return endShape; } set { endShape = value; updatePoints(); } }
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
        private int connectionID;

        public Connection()
        {
            //Deserialization Constructor - Shapes must be deserialized first!
            startShape = ShapeCollector.getI().getShapeByID(startShapeID);
            endShape = ShapeCollector.getI().getShapeByID(endShapeID);
        }

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            this.connectionID = nextID++;
            startShape = _start;
            startShapeID = startShape.id; //Saving ID for serialization!

            startMultiplicity = _startMultiplicity;
            endShape = _end;
            endShapeID = endShape == null ? -1 : endShape.id; //Saving ID for serialization
            endMultiplicity = _endMultiplicity;
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
            
            pointCollection = new PointCollection();
            PolygonPoints = null;
            if (End == null)
            {
                //StartPoint = new Point(Start.CanvasCenterX, Start.CanvasCenterY);
                //pointCollection.Add(StartPoint);

                //pointCollection.Add(EndPoint);
                //NotifyPropertyChanged(() => StartPoint);
                //NotifyPropertyChanged(() => PointCollection);
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
            01,02,03,10,12,13,20,21,23,30,31,32
    */      Point sPoint = sP[sInd];
            Point ePoint = eP[eInd];

            StartPoint = sPoint;
            EndPoint = ePoint;
            switch (sInd)
            {
                case 0:
                case 1:
                    switch (eInd)
                    {
                        case 0:
                        case 1:
                            p1.X = (ePoint.X + sPoint.X) / 2;
                            p1.Y = sPoint.Y;
                            p2.X = p1.X;
                            p2.Y = ePoint.Y;
                            break;
                        case 2:
                        case 3:
                            p1.X = ePoint.X;
                            p1.Y = sPoint.Y;
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
                            p1.X = ePoint.X;
                            p1.Y = sPoint.Y;
                            p2 = p1;
                            break;
                        case 2:
                        case 3:
                            p1.X = sPoint.X;
                            p1.Y = (sPoint.Y + ePoint.Y) / 2;
                            p2.X = ePoint.X;
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
                    throw new ArgumentOutOfRangeException("whats wrong with you!? Thats not part of the enum");
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
