using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Area51.SoftwareModeler.Model
{
    public class Connection : NotifyBase
    {
        private Shape start;
        private string startMultiplicity;
        private string endMultiplicity;
        private Shape end;
        private Point startPoint;
        public Point StartPoint { get { return startPoint; } set { startPoint = value; } }
        private Point p1;
        public Point P1 { get { return p1; } set { p1 = value; } }
        private Point p2;
        public Point P2 { get { return p2; } set { p2 = value; } }
        private Point endPoint;
        public Point EndPoint { get { return endPoint; } set { endPoint = value; } }
       
        private ConnectionType type;

        public Shape Start { get { return start; } set { start = value; updatePoints(); } }
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        public Shape End { get { return end; } set { end = value; updatePoints(); } }
        public string EndMultiplicity { get { return endMultiplicity; } set { endMultiplicity = value; } }

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            start = _start;
            startMultiplicity = _startMultiplicity;
            end = _end;
            endMultiplicity = _endMultiplicity;
            type = _type;
            

            //double vec0X = 1;
            //double vec0Y = 0;
            //double vec1X = _end.CanvasCenterX - _start.CanvasCenterX;
            //double vec1Y = _end.CanvasCenterY - _start.CanvasCenterY;
            //double phi1 = Math.Acos((vec0X * vec1X) / (Math.Sqrt(Math.Pow(vec1X, 2) + Math.Pow(vec1Y, 2))));
            //double phi2 = Math.Acos((vec0Y * vec1Y) / (Math.Sqrt(Math.Pow(vec1X, 2) + Math.Pow(vec1Y, 2))));

            //if (Math.Abs(_start.CanvasCenterX - _end.CanvasCenterX) < _start.Width)
            //{
            //    startX = _start.CanvasCenterX;
            //}
            //else if(_start.CanvasCenterX > _end.CanvasCenterX)
            //{
            //    startX = _start.CanvasCenterX - _start.Width/2;
            //}
            //else
            //{
            //    startX = _start.CanvasCenterX + _start.Width/2;
            //}
            //if(Math.Abs(_start.CanvasCenterY - _end.CanvasCenterY) < _start.Height)
            //{
            //    startY = _start.CanvasCenterY;
            //}else if(_start.CanvasCenterY > _end.CanvasCenterY)
            //{
            //    startY = _start.CanvasCenterY - _start.Height/2;
            //}
            //else
            //{
            //    startY = _start.CanvasCenterY + _start.Height/2;
            //}
            startPoint = new Point();
            p1 = new Point();
            p2 = new Point();
            endPoint = new Point();
            startPoint.X = _start.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? _start.Width / 2 : -_start.Width / 2);
            startPoint.Y = _start.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? _start.Height / 2 : -_start.Height / 2);
            endPoint.X = _end.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? -_end.Width / 2 : _end.Width / 2);
            endPoint.Y = _end.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? -_end.Height / 2 : _end.Height / 2);
          
            p1.X = _start.CanvasCenterX;
            p1.Y = (_start.CanvasCenterY + _end.CanvasCenterY) / 2.0;
            p2.Y = P1.Y;
            p2.X = _end.CanvasCenterX;
        }

        private void updatePoints()
        {
            Point sp = new Point(start.CanvasCenterX, start.CanvasCenterY);
            Point ep = new Point(end.CanvasCenterX, end.CanvasCenterY);
            Vector spep = ep - sp;
           
            // Console.WriteLine(Math.Atan2(-1, 5));
            startPoint.X = start.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? _start.Width / 2 : -_start.Width / 2);
            startPoint.Y = start.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? _start.Height / 2 : -_start.Height / 2);
            endPoint.X = end.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? -_end.Width / 2 : _end.Width / 2);
            endPoint.Y = end.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? -_end.Height / 2 : _end.Height / 2);
            p1.X = start.CanvasCenterX;
            p1.Y = (start.CanvasCenterY + end.CanvasCenterY) / 2.0;
            p2.Y = P1.Y;
            p2.X = end.CanvasCenterX;
            NotifyPropertyChanged(() => StartPoint);
            NotifyPropertyChanged(() => P1);
            NotifyPropertyChanged(() => P2);
            NotifyPropertyChanged(() => EndPoint);
        }
    }
}
