using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Area51.SoftwareModeler.Models
{
    public class Connection : NotifyBase
    {
        private Shape startShape;
        public Shape Start { get { return startShape; } set { startShape = value; updatePoints(); } }
        private string startMultiplicity;
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        private string endMultiplicity;
        public string EndMultiplicity { get { return endMultiplicity; } set { endMultiplicity = value; } }
        private Shape endShape;
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

        private ConnectionType type;

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            startShape = _start;
            startMultiplicity = _startMultiplicity;
            endShape = _end;
            endMultiplicity = _endMultiplicity;
            type = _type;

            startPoint = new Point();
            p1 = new Point();
            p2 = new Point();
            endPoint = new Point();

            updatePoints();
        }

        private void updatePoints()
        {
            pointCollection = new PointCollection();

            //direktional modifier to turn the arrows the correct way.
            double dm = (endShape.CanvasCenterY - startShape.CanvasCenterY) > 0 ? -1 : 1;

            //horisontalt center
            startPoint.X = startShape.CanvasCenterX;
            endPoint.X = endShape.CanvasCenterX;

            //vertikalt center, minus halvdelen af shapens højde.
            startPoint.Y = startShape.CanvasCenterY - (dm * (startShape.Height / 2 - 2));
            endPoint.Y = endShape.CanvasCenterY - ((endShape.Height / 2.0));

            p1.X = startShape.CanvasCenterX;
            p1.Y = (startShape.CanvasCenterY + endShape.CanvasCenterY) / 2.0;
            p2.Y = P1.Y;
            p2.X = endShape.CanvasCenterX;



            pointCollection.Add(p1);
            pointCollection.Add(p2);
            pointCollection.Add(endPoint);

            switch (type)
            {
                case ConnectionType.Aggregation:
                    drawRhombus(dm);
                    break;
                case ConnectionType.Composition:
                    drawFilledRhombus(dm);
                    break;
                case ConnectionType.Association:
                    drawArrow(dm);
                    break;
                default:
                    //whats wrong with you? this is off limits!
                    throw new ArgumentOutOfRangeException("whats wrong with you!? Thats not part of the enum");
            }

            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => PointCollection);
        }

        //association
        private void drawArrow(double dm)
        {
            //Point p = new Point();
            //p.X = endShape.CanvasCenterX - (2);
            //p.Y = dm * (endShape.CanvasCenterY - (endShape.Height / 2) - 4);
            //pointCollection.Add(p);
            //pointCollection.Add(endPoint);
            //p.X = endShape.CanvasCenterX + (2);
            //p.Y = dm * (endShape.CanvasCenterY - (endShape.Height / 2) - 4);
            //pointCollection.Add(p);
            //pointCollection.Add(endPoint);
        }

        //agregation
        private void drawRhombus(double dm)
        {
            pointCollection.RemoveAt(pointCollection.Count - 1);
            Point p = new Point();
            p.X = endPoint.X;
            p.Y = dm * (endPoint.Y - 30);
            PointCollection.Add(p);
            p.X = endPoint.X - 5;
            p.Y = endPoint.Y - 15;
            PointCollection.Add(p);
            PointCollection.Add(endPoint);
            p.X = endPoint.X + 5;
            p.Y = dm * (endPoint.Y - 15);
            PointCollection.Add(p);
            p.X = endPoint.X;
            p.Y = dm * (endPoint.Y - 30);
            PointCollection.Add(p);
        }

        //composition
        private void drawFilledRhombus(double dm)
        {
            pointCollection.RemoveAt(pointCollection.Count - 1);
            Point p = new Point();
            p.X = endPoint.X;
            p.Y = endPoint.Y - 30;
            PointCollection.Add(p);
            p.X = endPoint.X - 5;
            p.Y = endPoint.Y - 15;
            PointCollection.Add(p);
            PointCollection.Add(endPoint);
            p.X = endPoint.X + 5;
            p.Y = endPoint.Y - 15;
            PointCollection.Add(p);
            p.X = endPoint.X;
            p.Y = endPoint.Y - 30;
            PointCollection.Add(p);
        }
    }
}
