using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public class Connection : NotifyBase
    {
        private Shape start;
        private string startMultiplicity;
        private string endMultiplicity;
        private Shape end;
        public double startX;// { get; set; }
        public double StartX { get { return startX; } set { startX = value; } }
        public double startY;// { get; set; }
        public double StartY { get { return startY; } set { startY = value; } }
        private double point1X;
        public double P1X { get { return point1X; } set {point1X = value;  } }
        private double point1Y;
        public double P1Y { get { return point1Y; } set { point1Y = value;  } }
        private double point2X;
        public double P2X { get { return point2X; } set { point2X = value;  } }
        private double point2Y;
        public double P2Y { get { return point2Y; } set { point2Y = value;} }
        public double endX;// { get; set; }
        public double EndX { get { return endX; } set { endX = value; } }
        public double endY;// { get; set; }
        public double EndY { get { return endY; } set { endY = value;  } }
        public string startStr { get; set; }
        private ConnectionType type;

        public Shape Start { get { return start; } set { start = value; updatePoints(); NotifyPropertyChanged(()=>StartX); NotifyPropertyChanged(() => StartY); } }
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        public Shape End { get { return end; } set { end = value; updatePoints(); NotifyPropertyChanged(); } }
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

            startX = _start.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? _start.Width / 2 : -_start.Width / 2);
            startY = _start.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? _start.Height / 2 : -_start.Height / 2);
            endX = _end.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? -_end.Width / 2 : _end.Width / 2);
            endY = _end.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? -_end.Height / 2 : _end.Height / 2);
            startStr = (startX + " " + startY);
            P1X = _start.CanvasCenterX;
            P1Y = (_start.CanvasCenterY + _end.CanvasCenterY) / 2.0;
            P2Y = P1Y;
            P2X = _end.CanvasCenterX;
        }

        private void updatePoints()
        {
            StartX = start.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? _start.Width / 2 : -_start.Width / 2);
            StartY = start.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? _start.Height / 2 : -_start.Height / 2);
            EndX = end.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? -_end.Width / 2 : _end.Width / 2);
            EndY = end.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? -_end.Height / 2 : _end.Height / 2);
            startStr = (startX + " " + startY);
            P1X = start.CanvasCenterX;
            P1Y = (start.CanvasCenterY + end.CanvasCenterY) / 2.0;
            P2Y = P1Y;
            P2X = end.CanvasCenterX;
        }
    }
}
